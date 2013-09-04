﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConvertUtils.cs" company="Labo">
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
//   Defines the ConvertUtils type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Labo.Common.Utils
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Security;
    using System.Security.Permissions;
    using System.Text.RegularExpressions;
    using System.Threading;

    using Labo.Common.Resources;
    using Labo.Common.Utils.Converters;
    using Labo.Common.Utils.Exceptions;

    /// <summary>
    /// 
    /// </summary>
    public static class ConvertUtils
    {
        /// <summary>
        /// Initializes static members of the <see cref="ConvertUtils"/> class. 
        /// </summary>
        static ConvertUtils()
        {
            RegisterTypeConverter<FileInfo, FileInfoConverter>();
            RegisterTypeConverter<Regex, RegexConverter>();
            RegisterTypeConverter<Uri, UriConverter>();
            RegisterTypeConverter<string[], StringArrayConverter>();
        }

        /// <summary>
        /// Changes the type of the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static T ChangeType<T>(object value)
        {
            return (T)ChangeType(value, typeof(T), Thread.CurrentThread.CurrentCulture);
        }

        /// <summary>
        /// Changes the type of the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static T ChangeType<T>(object value, T defaultValue)
        {
            return ChangeType(value, defaultValue, Thread.CurrentThread.CurrentCulture);
        }

        /// <summary>
        /// Changes the type of the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public static T ChangeType<T>(object value, T defaultValue, CultureInfo culture)
        {
            object changed = ChangeType(value, typeof(T), culture);
            if (changed == null)
            {
                return defaultValue;
            }
            return (T)changed;
        }

        /// <summary>
        /// Changes the type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public static T ChangeType<T>(object value, CultureInfo culture)
        {
            return (T)ChangeType(value, typeof(T), culture);
        }

        /// <summary>
        /// Changes the type.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static object ChangeType(object value, Type type)
        {
            return ChangeType(value, type, Thread.CurrentThread.CurrentCulture);
        }

        /// <summary>
        /// Changes the type.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public static object ChangeType(object value, Type type, CultureInfo culture)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (culture == null)
            {
                throw new ArgumentNullException("culture");
            }

            try
            {
                if (type == typeof(string))
                {
                    return Convert.ToString(value, culture);
                }
                bool isValueNull = value == null;
                Type valueType = isValueNull ? null : value.GetType();
                if ((type == valueType) || (type == typeof(object)))
                {
                    return value;
                }
                bool convertToNullableBoolean = type == typeof(bool?);
                if (type == typeof(bool) || convertToNullableBoolean)
                {
                    bool? result;
                    if (TryConvertToBoolean(value, valueType, convertToNullableBoolean, out result))
                    {
                        return result.HasValue ? result.Value : result;
                    }
                }

                TypeConverter typeConverter = TypeDescriptor.GetConverter(type);
                if (valueType != null && typeConverter.CanConvertFrom(valueType))
                {
                    return typeConverter.ConvertFrom(null, culture, value);
                }

                if (isValueNull)
                {
                    return null;
                }

                TypeConverter valueConverter = TypeDescriptor.GetConverter(valueType);
                if (valueConverter.CanConvertTo(type))
                {
                    return valueConverter.ConvertTo(value, type);
                }

                if (TypeUtils.IsNullable(type))
                {
                    type = Nullable.GetUnderlyingType(type);
                    if (valueConverter.CanConvertTo(type))
                    {
                        return valueConverter.ConvertTo(value, type);
                    }
                }

                return TryConvertByIConvertible(value, type, culture);
            }
            catch (Exception exception)
            {
                throw GetConversionException(value, type, culture, exception);
            }
        }

        private static bool TryConvertToBoolean(object value, Type valueType, bool convertToNullableBoolean, out bool? result)
        {
            if (value == null)
            {
                result = new bool?();
                return true;
            }
            if (valueType == typeof(string))
            {
                if (value.Equals("1") || value.Equals("on") || value.Equals("yes"))
                {
                    result = true;
                    return true;
                }
                if (value.Equals("0") || value.Equals("no"))
                {
                    result = false;
                    return true;
                }
                if (value.Equals(string.Empty))
                {
                    result = convertToNullableBoolean ? new bool?() : false;
                    return true;
                }
            }
            else if (valueType == typeof(int))
            {
                if (1.Equals(value))
                {
                    result = true;
                    return true;
                }
                if (0.Equals(value))
                {
                    result = false;
                    return true;                    
                }
            }
            result = false;
            return false;
        }

        private static object TryConvertByIConvertible(object value, Type type, CultureInfo culture)
        {
            IConvertible convertible = value as IConvertible;
            if (convertible != null)
            {
                if (type == typeof(bool))
                {
                    return convertible.ToBoolean(culture);
                }

                if (type == typeof(byte))
                {
                    return convertible.ToByte(culture);
                }

                if (type == typeof(char))
                {
                    return convertible.ToChar(culture);
                }

                if (type == typeof(DateTime))
                {
                    return convertible.ToDateTime(culture);
                }

                if (type == typeof(decimal))
                {
                    return convertible.ToDecimal(culture);
                }

                if (type == typeof(double))
                {
                    return convertible.ToDouble(culture);
                }

                if (type == typeof(short))
                {
                    return convertible.ToInt16(culture);
                }

                if (type == typeof(int))
                {
                    return convertible.ToInt32(culture);
                }

                if (type == typeof(long))
                {
                    return convertible.ToInt64(culture);
                }

                if (type == typeof(sbyte))
                {
                    return convertible.ToSByte(culture);
                }

                if (type == typeof(float))
                {
                    return convertible.ToSingle(culture);
                }

                if (type == typeof(ushort))
                {
                    return convertible.ToUInt16(culture);
                }

                if (type == typeof(uint))
                {
                    return convertible.ToUInt32(culture);
                }

                if (type == typeof(ulong))
                {
                    return convertible.ToUInt64(culture);
                }
            }

            throw GetConversionException(value, type, culture);
        }

        private static ConversionException GetConversionException(object value, Type type, CultureInfo culture, Exception exception = null)
        {
            string errorMessage = Strings.ConvertUtils_GetConversionException_An_Error_Occurred_While_Changing_Value;
            ConversionException conversionEx = exception == null ? new ConversionException(errorMessage) : new ConversionException(errorMessage, exception);
            conversionEx.Data.Add("VALUE", Convert.ToString(value, CultureInfo.CurrentCulture));
            conversionEx.Data.Add("TYPE", type.ToString());
            conversionEx.Data.Add("CULTURE", culture.Name);
            return conversionEx;
        }

        /// <summary>
        /// Registers the type converter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TC">The type of the C.</typeparam>
        // CA2135 violation - the LinkDemand should be removed, and the method marked [SecurityCritical] instead
        [EnvironmentPermission(SecurityAction.Demand, Unrestricted = true)]
        [SecurityCritical]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static void RegisterTypeConverter<T, TC>() where TC : TypeConverter
        {
            TypeDescriptor.AddAttributes(typeof(T), new Attribute[] { new TypeConverterAttribute(typeof(TC)) });
        }
    }
}
