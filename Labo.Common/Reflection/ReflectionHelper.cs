﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionHelper.cs" company="Labo">
//   The MIT License (MIT)
//   
//   Copyright (c) 2013 Bora Akgun
//   
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to
//   use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
//   the Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:
//   
//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.
//   
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
// <summary>
//   The reflection helper class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Labo.Common.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;

    using Labo.Common.Reflection.Exceptions;
    using Labo.Common.Resources;
    using Labo.Common.Utils;

    /// <summary>
    /// The reflection helper class.
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// The default property info binding flags.
        /// </summary>
        public const BindingFlags DEFAULT_PROPERTY_INFO_BINDING_FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        /// <summary>
        /// The default method info binding flags.
        /// </summary>
        public const BindingFlags DEFAULT_METHOD_INFO_BINDING_FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        /// <summary>
        /// The property access item class.
        /// </summary>
        internal sealed class PropertyAccessItem
        {
            /// <summary>
            /// Gets or sets the getter delegate.
            /// </summary>
            /// <value>
            /// The getter delegate.
            /// </value>
            public MemberGetter Getter { get; set; }

            /// <summary>
            /// Gets or sets the setter delegate.
            /// </summary>
            /// <value>
            /// The setter delegate.
            /// </value>
            public MemberSetter Setter { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether property [can read].
            /// </summary>
            /// <value>
            ///   <c>true</c> if property [can read]; otherwise, <c>false</c>.
            /// </value>
            public bool CanRead { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether property [can write].
            /// </summary>
            /// <value>
            ///   <c>true</c> if property [can write]; otherwise, <c>false</c>.
            /// </value>
            public bool CanWrite { get; set; }
        }

        /// <summary>
        /// The dynamic method cache
        /// </summary>
        private static readonly DynamicMethodCache s_DynamicMethodCache = new DynamicMethodCache();

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <typeparam name="T">The instance type.</typeparam>
        /// <param name="constructorInfo">The constructor information.</param>
        /// <param name="parameterTypes">The parameter types.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The created instance.</returns>
        public static T CreateInstance<T>(ConstructorInfo constructorInfo = null, Type[] parameterTypes = null, params object[] parameters)
        {
            return (T)CreateInstance(typeof(T), constructorInfo, parameterTypes, parameters);
        }

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <typeparam name="T">The instance type.</typeparam>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The created instance.</returns>
        public static T CreateInstance<T>(params object[] parameters)
        {
            return (T)CreateInstance(typeof(T), null, null, parameters);
        }

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The created instance.</returns>
        public static object CreateInstance(Type type, params object[] parameters)
        {
            return CreateInstance(type, null, null, parameters);
        }

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="constructorInfo">The constructor information.</param>
        /// <param name="parameterTypes">The parameter types.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The created instance.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// type
        /// or
        /// constructorInfo
        /// </exception>
        public static object CreateInstance(Type type, ConstructorInfo constructorInfo = null, Type[] parameterTypes = null, params object[] parameters)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (parameterTypes == null)
            {
                parameterTypes = Type.EmptyTypes;
            }

            if (constructorInfo == null && parameterTypes.Length == 0)
            {
                constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
            }

            if (constructorInfo == null)
            {
                throw new ArgumentNullException("constructorInfo");
            }

            return s_DynamicMethodCache.GetOrAddDelegate(new DynamicMethodInfo(type, MemberTypes.Constructor, constructorInfo.Name, parameterTypes), () => DynamicMethodHelper.EmitConstructorInvoker(type, constructorInfo, parameterTypes), DynamicMethodCacheStrategy.Temporary)(parameters);
        }

        /// <summary>
        /// Calls the method.
        /// </summary>
        /// <param name="object">The object.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The method return value.</returns>
        public static object CallMethod(object @object, string methodName, params object[] parameters)
        {
            return CallMethod(@object, methodName, DEFAULT_METHOD_INFO_BINDING_FLAGS, parameters);
        }

        /// <summary>
        /// Calls the method.
        /// </summary>
        /// <param name="object">The object.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="bindingFlags">The binding flags.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The method return value.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// object
        /// or
        /// methodName
        /// </exception>
        public static object CallMethod(object @object, string methodName, BindingFlags bindingFlags, params object[] parameters)
        {
            if (@object == null)
            {
                throw new ArgumentNullException("object");
            }

            if (methodName.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException("methodName");
            }

            Type objectType = @object.GetType();
            MethodInfo methodInfo = GetMethodInfo(objectType, methodName, bindingFlags, parameters);
            return CallMethod(@object, methodInfo, parameters);
        }

        /// <summary>
        /// Calls the method.
        /// </summary>
        /// <param name="object">The object.</param>
        /// <param name="methodInfo">The method information.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Method return value.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// object
        /// or
        /// methodInfo
        /// </exception>
        public static object CallMethod(object @object, MethodInfo methodInfo, params object[] parameters)
        {
            if (@object == null)
            {
                throw new ArgumentNullException("object");
            }

            if (methodInfo == null)
            {
                throw new ArgumentNullException("methodInfo");
            }

            if (parameters == null)
            {
                parameters = new object[] { null };
            }

            Type objectType = @object.GetType();

            Type[] parameterTypes;

            CheckParameters(methodInfo, parameters, out parameterTypes);

            return s_DynamicMethodCache.GetOrAddDelegate(new DynamicMethodInfo(objectType, MemberTypes.Method, methodInfo.Name, parameterTypes), () => DynamicMethodHelper.EmitMethodInvoker(objectType, methodInfo), DynamicMethodCacheStrategy.Temporary)(@object, parameters);
        }

        /// <summary>
        /// Calls the static method.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The static method return value.</returns>
        public static object CallStaticMethod(Type type, string methodName, params object[] parameters)
        {
            return CallStaticMethod(type, methodName, DEFAULT_METHOD_INFO_BINDING_FLAGS, parameters);
        }

        /// <summary>
        /// Calls the static method.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="bindingFlags">The binding flags.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The static method return value.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// type
        /// or
        /// methodName
        /// </exception>
        public static object CallStaticMethod(Type type, string methodName, BindingFlags bindingFlags, params object[] parameters)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (methodName.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException("methodName");
            }

            MethodInfo methodInfo = GetMethodInfo(type, methodName, bindingFlags, parameters);
            return CallStaticMethod(type, methodInfo, parameters);
        }

        /// <summary>
        /// Calls the static method.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="methodInfo">The method information.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The static method return value.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// type
        /// or
        /// methodInfo
        /// </exception>
        /// <exception cref="ReflectionHelperException"></exception>
        public static object CallStaticMethod(Type type, MethodInfo methodInfo, params object[] parameters)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (methodInfo == null)
            {
                throw new ArgumentNullException("methodInfo");
            }

            if (parameters == null)
            {
                parameters = new object[] { null };
            }

            Type[] parameterTypes;

            CheckParameters(methodInfo, parameters, out parameterTypes);

            return s_DynamicMethodCache.GetOrAddDelegate(new DynamicMethodInfo(type, MemberTypes.Method, methodInfo.Name, parameterTypes), () => DynamicMethodHelper.EmitMethodInvoker(type, methodInfo), DynamicMethodCacheStrategy.Temporary)(null, parameters);
        }

        /// <summary>
        /// Calls the method by named parameters.
        /// </summary>
        /// <param name="object">The object.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Method return value.</returns>
        public static object CallMethodByNamedParameters(object @object, string methodName, params NamedParameterWithValue[] parameters)
        {
            return CallMethodByNamedParameters(@object, methodName, DEFAULT_METHOD_INFO_BINDING_FLAGS, parameters);
        }

        /// <summary>
        /// Calls the method by named parameters.
        /// </summary>
        /// <param name="object">The object.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="bindingFlags">The binding flags.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The method return value.</returns>
        /// <exception cref="System.ArgumentNullException">object</exception>
        public static object CallMethodByNamedParameters(object @object, string methodName, BindingFlags bindingFlags, params NamedParameterWithValue[] parameters)
        {
            if (@object == null)
            {
                throw new ArgumentNullException("object");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            int parametersLength = parameters.Length;
            IDictionary<string, Type> parameterNames = new Dictionary<string, Type>(parametersLength);
            object[] parameterValues = new object[parametersLength];
            for (int i = 0; i < parametersLength; i++)
            {
                NamedParameterWithValue namedParameterWithValue = parameters[i];
                parameterNames.Add(namedParameterWithValue.Name, namedParameterWithValue.Type);
                parameterValues[i] = namedParameterWithValue.Value;
            }

            return CallMethod(@object, GetMethodByName(@object.GetType(), bindingFlags, methodName, parameterNames), parameterValues);
        }

        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="object">The object.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <param name="bindingFlags">The binding flags.</param>
        /// <exception cref="System.ArgumentNullException">
        /// object
        /// or
        /// propertyName
        /// </exception>
        public static void SetPropertyValue(object @object, string propertyName, object value, BindingFlags bindingFlags = DEFAULT_PROPERTY_INFO_BINDING_FLAGS)
        {
            if (@object == null)
            {
                throw new ArgumentNullException("object");
            }

            if (propertyName.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException("propertyName");
            }

            Type type = @object.GetType();
            PropertyInfo propertyInfo = GetPropertyInfo(propertyName, bindingFlags, type);

            SetPropertyValue(@object, propertyInfo, value);
        }

        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="object">The object.</param>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="value">The value.</param>
        public static void SetPropertyValue(object @object, PropertyInfo propertyInfo, object value)
        {
            if (@object == null)
            {
                throw new ArgumentNullException("object");
            }

            if (propertyInfo == null)
            {
                throw new ArgumentNullException("propertyInfo");
            }

            Type type = @object.GetType();

            if (!propertyInfo.CanWrite)
            {
                throw new ReflectionHelperException(string.Format(CultureInfo.CurrentCulture, Strings.ReflectionHelper_SetPropertyValue_the_property_has_no_set_method, propertyInfo.Name, type.AssemblyQualifiedName));
            }

            PropertyAccessItem propertyAccessItem = GetPropertyAccessItem(type, propertyInfo);

            //if (!TypeUtils.IsImplicitlyConvertible(valueType, propertyInfo.PropertyType))
            //{
            //    throw new ReflectionHelperException(string.Format(CultureInfo.CurrentCulture, Strings.ReflectionHelper_SetPropertyValue_type_is_not_implicitly_convertable, valueType, propertyInfo.PropertyType));
            //}

            CheckAreAssignable(propertyInfo, value, propertyInfo.PropertyType);

            propertyAccessItem.Setter(@object, value);
        }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="object">The object.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="bindingFlags">The binding flags.</param>
        /// <returns>The property value</returns>
        /// <exception cref="System.ArgumentNullException">
        /// object
        /// or
        /// propertyName
        /// </exception>
        public static object GetPropertyValue(object @object, string propertyName, BindingFlags bindingFlags = DEFAULT_PROPERTY_INFO_BINDING_FLAGS)
        {
            if (@object == null)
            {
                throw new ArgumentNullException("object");
            }

            if (propertyName.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException("propertyName");
            }

            Type type = @object.GetType();
            PropertyInfo propertyInfo = GetPropertyInfo(propertyName, bindingFlags, type);

            return GetPropertyValue(@object, propertyInfo);
        }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="object">The object.</param>
        /// <param name="propertyInfo">The property information.</param>
        /// <returns>The property value</returns>
        /// <exception cref="System.ArgumentNullException">
        /// object
        /// or
        /// propertyInfo
        /// </exception>
        public static object GetPropertyValue(object @object, PropertyInfo propertyInfo)
        {
            if (@object == null)
            {
                throw new ArgumentNullException("object");
            }

            if (propertyInfo == null)
            {
                throw new ArgumentNullException("propertyInfo");
            }

            Type type = @object.GetType();

            if (!propertyInfo.CanRead)
            {
                throw new ReflectionHelperException(string.Format(CultureInfo.CurrentCulture, Strings.ReflectionHelper_GetPropertyValue_property_has_no_get_method, propertyInfo.Name, type.AssemblyQualifiedName));
            }

            PropertyAccessItem propertyAccessItem = GetPropertyAccessItem(type, propertyInfo);
            return propertyAccessItem.Getter(@object);
        }

        /// <summary>
        /// Gets the method by name.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="bindingFlags">The binding flags.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The method info.</returns>
        public static MethodInfo GetMethodByName(Type type, BindingFlags bindingFlags, string methodName, IDictionary<string, Type> parameters = null)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (methodName == null)
            {
                throw new ArgumentNullException("methodName");
            }

            MethodInfo[] methodInfos = type.GetMethods(bindingFlags);
            for (int i = 0; i < methodInfos.Length; i++)
            {
                MethodInfo methodInfo = methodInfos[i];
                if (methodInfo.Name == methodName)
                {
                    ParameterInfo[] parameterInfos = methodInfo.GetParameters();

                    if (parameters == null)
                    {
                        if (parameterInfos.Length == 0)
                        {
                            return methodInfo;
                        }

                        continue;
                    }

                    if (parameterInfos.Length != parameters.Count)
                    {
                        continue;
                    }

                    bool parameterFound = true;
                    foreach (KeyValuePair<string, Type> parameter in parameters)
                    {
                        parameterFound = false;

                        for (int j = 0; j < parameterInfos.Length; j++)
                        {
                            ParameterInfo parameterInfo = parameterInfos[j];
                            if (parameterInfo.Name == parameter.Key && parameterInfo.ParameterType == parameter.Value)
                            {
                                parameterFound = true;
                                break;
                            }
                        }

                        if (!parameterFound)
                        {
                            break;
                        }
                    }

                    if (!parameterFound)
                    {
                        continue;
                    }

                    return methodInfo;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the property information.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="bindingFlags">The binding flags.</param>
        /// <param name="type">The type.</param>
        /// <param name="throwExceptionWhenNotFound">Throw exception when property not found.</param>
        /// <returns>The property info.</returns>
        internal static PropertyInfo GetPropertyInfo(string propertyName, BindingFlags bindingFlags, Type type, bool throwExceptionWhenNotFound = true)
        {
            PropertyInfo propertyInfo = type.GetProperty(propertyName, bindingFlags);
            if (propertyInfo == null && throwExceptionWhenNotFound)
            {
                throw new ReflectionHelperException(string.Format(CultureInfo.CurrentCulture, Strings.ReflectionHelper_GetPropertyInfo_property_couldnot_be_found, propertyName, type.AssemblyQualifiedName))
                          {
                              Data = { { "BindingFlags", bindingFlags.ToString() } }
                          };
            }

            return propertyInfo;
        }

        private static void CheckParameters(MethodInfo methodInfo, object[] parameters, out Type[] parameterTypes)
        {
            // TODO: cache method parameters.
            ParameterInfo[] methodParameters = methodInfo.GetParameters();
            parameterTypes = new Type[methodParameters.Length];

            if (methodParameters.Length != parameters.Length)
            {
                throw new ReflectionHelperException(string.Format(CultureInfo.CurrentCulture, Strings.ReflectionHelper_CallMethod_Incorrect_number_of_arguments, methodInfo));
            }

            for (int i = 0; i < methodParameters.Length; i++)
            {
                ParameterInfo methodParameter = methodParameters[i];
                Type parameterType = methodParameter.ParameterType;

                parameterTypes[i] = parameterType;
                object parameterValue = parameters[i];

                if (parameterValue != null)
                {
                    Type parameterValueType = parameterValue.GetType();
                    if (parameterValueType != parameterType
                        && TypeUtils.IsImplicitlyConvertible(parameterValueType, parameterType))
                    {
                        parameterValue = parameters[i] = ConvertUtils.ChangeType(parameterValue, parameterType);
                    }
                }

                CheckAreAssignable(methodInfo, parameterValue, parameterType);
            }
        }

        /// <summary>
        /// Gets the property access item.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="propertyInfo">The property information.</param>
        /// <returns>The property access item.</returns>
        private static PropertyAccessItem GetPropertyAccessItem(Type objectType, PropertyInfo propertyInfo)
        {
            return s_DynamicMethodCache.GetOrAddDelegate(new DynamicMethodInfo(objectType, MemberTypes.Property, propertyInfo.Name, new[] { propertyInfo.PropertyType }), () => CreatePropertyAccessItem(objectType, propertyInfo), DynamicMethodCacheStrategy.Temporary);
        }

        /// <summary>
        /// Creates the property access item.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="propertyInfo">The property information.</param>
        /// <returns>The property access item.</returns>
        internal static PropertyAccessItem CreatePropertyAccessItem(Type objectType, PropertyInfo propertyInfo)
        {
            if (objectType == null)
            {
                throw new ArgumentNullException("objectType");
            }

            if (propertyInfo == null)
            {
                throw new ArgumentNullException("propertyInfo");
            }

            bool canRead = propertyInfo.CanRead;
            bool canWrite = propertyInfo.CanWrite;

            PropertyAccessItem propertyAccessItem = new PropertyAccessItem
                                                        {
                                                            CanRead = canRead,
                                                            CanWrite = canWrite
                                                        };
            if (canRead)
            {
                propertyAccessItem.Getter = DynamicMethodHelper.EmitPropertyGetter(objectType, propertyInfo);
            }

            if (canWrite)
            {
                propertyAccessItem.Setter = DynamicMethodHelper.EmitPropertySetter(objectType, propertyInfo);
            }

            return propertyAccessItem;
        }

        /// <summary>
        /// Gets the method information.
        /// </summary>
        /// <param name="objectType">The object type.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="bindingFlags">The binding flags.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The method info.</returns>
        internal static MethodInfo GetMethodInfo(Type objectType, string methodName, BindingFlags bindingFlags, params object[] parameters)
        {
            if (parameters == null)
            {
                parameters = new object[] { null };
            }

            Type[] parameterTypes = GetParameterTypes(parameters);
            return objectType.GetMethod(methodName, bindingFlags, null, parameterTypes, null);
        }

        /// <summary>
        /// Checks the two types are assignable.
        /// </summary>
        /// <param name="memberInfo">The member info.</param>
        /// <param name="sourceValue">The source value.</param>
        /// <param name="destination">The destination.</param>
        /// <exception cref="ReflectionHelperException">thrown when types are not assignable.</exception>
        internal static void CheckAreAssignable(MemberInfo memberInfo, object sourceValue, Type destination)
        {
            Type source = TypeUtils.GetType(sourceValue);

            if (!destination.IsValueType && sourceValue == null)
            {
                return;
            }

            if (!TypeUtils.AreAssignable(source, destination))
            {
                throw new ReflectionHelperException(string.Format(CultureInfo.CurrentCulture, Strings.ReflectionHelper_Parameter_type_cannot_be_used_for_method, source, destination, memberInfo));
            }
        }

        /// <summary>
        /// Gets the parameter types.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The parameter types.</returns>
        private static Type[] GetParameterTypes(IList<object> parameters)
        {
            int parametersCount = parameters.Count;
            if (parametersCount == 0)
            {
                return Type.EmptyTypes;
            }

            Type[] parameterTypes = new Type[parametersCount];
            for (int i = 0; i < parametersCount; i++)
            {
                object parameter = parameters[i];
                parameterTypes[i] = TypeUtils.GetType(parameter);
            }

            return parameterTypes;
        }
    }
}
