using System;

namespace Labo.Common.Patterns
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class DateTimeProviderContext : IDisposable
    {
        private readonly IDateTimeProvider m_DateTimeProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeProviderContext"/> class.
        /// </summary>
        /// <param name="dateTimeProvider">The date time provider.</param>
        /// <exception cref="System.ArgumentNullException">dateTimeProvider</exception>
        public DateTimeProviderContext(IDateTimeProvider dateTimeProvider)
        {
            if (dateTimeProvider == null) throw new ArgumentNullException("dateTimeProvider");

            m_DateTimeProvider = dateTimeProvider;

            DateTimeProvider.SetDateTimeProvider(m_DateTimeProvider);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            DateTimeProvider.ResetDateTimeProvider();
        }
    }
}