﻿using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

using Labo.Common.Exceptions;

namespace Labo.Common.Patterns.Exception
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class FactoryCreateInstanceException : CoreLevelException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryCreateInstanceException"/> class.
        /// </summary>
        public FactoryCreateInstanceException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryCreateInstanceException"/> class.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        public FactoryCreateInstanceException(System.Exception innerException)
            : base(null, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryCreateInstanceException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public FactoryCreateInstanceException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryCreateInstanceException"/> class.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        /// <param name="context">The context.</param>
        protected FactoryCreateInstanceException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryCreateInstanceException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public FactoryCreateInstanceException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is a null reference (Nothing in Visual Basic). </exception>
        ///   
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter"/>
        ///   </PermissionSet>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
