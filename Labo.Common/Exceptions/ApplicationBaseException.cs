﻿using System;
using System.Security.Permissions;
using System.Runtime.Serialization;

namespace Labo.Common.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public abstract class ApplicationBaseException : Exception
    {
        private readonly Guid m_Guid = Guid.NewGuid();

        /// <summary>
        /// Gets the GUID.
        /// </summary>
        public Guid Guid
        {
            get { return m_Guid; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationBaseException"/> class.
        /// </summary>
        protected ApplicationBaseException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationBaseException"/> class.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        protected ApplicationBaseException(Exception innerException)
            : base(null, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationBaseException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        protected ApplicationBaseException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationBaseException"/> class.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        /// <param name="context">The context.</param>
        protected ApplicationBaseException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationBaseException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        protected ApplicationBaseException(string message, Exception innerException)
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
