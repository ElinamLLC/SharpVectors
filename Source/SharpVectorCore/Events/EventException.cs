using System;
using System.Runtime.Serialization;
using System.Security;

namespace SharpVectors.Dom.Events
{
    /// <summary>
    /// Event operations may throw an
    /// <see cref="EventException">EventException</see> as specified in their
    /// method descriptions.
    /// </summary>
    [Serializable]
    public class EventException : Exception
    {
        #region Private Fields

        private EventExceptionCode _code;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EventException"/> class with event exception code.
        /// </summary>
        /// <param name="code">
        /// An instance of <see cref="EventExceptionCode"/> defining the event exception code.
        /// </param>
        public EventException(EventExceptionCode code)
        {
            _code = code;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventException"/> class with serialized data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
        /// </param>
        [SecuritySafeCritical]
        protected EventException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// An integer indicating the type of error generated.
        /// </summary>
        /// <value>
        /// An enumeration of the type <see cref="EventExceptionCode"/> specifying the type of error generated.
        /// </value>
        public EventExceptionCode Code
        {
            get {
                return _code;
            }
        }

        #endregion
    }
}
