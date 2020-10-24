using System;
using System.Runtime.Serialization;

namespace SharpVectors.Dom
{
    /// <summary>
    /// Represents errors that occur during DOM operation execution.
    /// </summary>
    /// <remarks>
    /// DOM operations only raise exceptions in "exceptional" circumstances, i.e., 
    /// when an operation is impossible to perform (either for logical reasons, 
    /// because data is lost, or because the implementation has become unstable). 
    /// In general, DOM methods return specific error values in ordinary 
    /// processing situations, such as out-of-bound errors when using <see cref="INodeList"/>. 
    /// <para>Implementations should raise other exceptions under other circumstances. 
    /// For example, implementations should raise an implementation-dependent 
    /// exception if a <see langword="null"/> argument is passed. 
    /// </para>
    /// <para>Some languages and object systems do not support the concept of 
    /// exceptions. For such systems, error conditions may be indicated using 
    /// native error reporting mechanisms. For some bindings, for example, 
    /// methods may return error codes similar to those listed in the 
    /// corresponding method descriptions.
    /// </para>
    /// </remarks>
    /// <seealso href="https://www.w3.org/TR/2000/REC-DOM-Level-2-Core-20001113">
    /// Document Object Model (DOM) Level 2 Core Specification</seealso>
	[Serializable]
	public class DomException : Exception
	{
		private DomExceptionType _exceptionCode;

        /// <summary>
        /// Initializes a new instance of the System.Exception class.
        /// </summary>
		public DomException()
		{
            _exceptionCode = DomExceptionType.UnknowError;
        }

        /// <summary>
        /// Initializes a new instance of the System.Exception class with a specified error
        /// message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="msg">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, 
        /// or <see langword="null"/> if no inner exception is specified.</param>
        protected DomException(string msg, Exception innerException) 
            : base(msg, innerException)
		{
		}

        /// <summary>
        /// Initializes a new instance of the System.Exception class with a specified error code.
        /// </summary>
        /// <param name="code">The DOM exception code.</param>
        public DomException(DomExceptionType code) 
            : this(code, string.Empty)
		{
		}

        /// <summary>
        /// Initializes a new instance of the System.Exception class with a specified error message and error code.
        /// </summary>
        /// <param name="code">The DOM exception code.</param>
        /// <param name="msg"> The message that describes the error.</param>
        public DomException(DomExceptionType code, string msg) 
            : this(code, msg, null)
		{
		}

        /// <summary>
        /// Initializes a new instance of the System.Exception class with a specified error
        /// message, error code and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="code">The DOM exception code.</param>
        /// <param name="msg"> The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, 
        /// or <see langword="null"/> if no inner exception is specified.</param>
		public DomException(DomExceptionType code, string msg, Exception innerException) 
            : base(msg, innerException)
		{
			_exceptionCode = code;
		}

        /// <summary>
        /// Initializes a new instance of the System.Exception class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data 
        /// about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information 
        /// about the source or destination.</param>
		protected DomException (SerializationInfo info, StreamingContext context ) 
            : base(info, context)
		{
		}

        /// <summary>
        /// Gets or sets a code specifying the type of the error.
        /// </summary>
        /// <value>An enumeration of the type <see cref="DomExceptionType"/> specifying the code defining the error.</value>
		public DomExceptionType Code
		{
			get {
                return _exceptionCode;
            }
            protected set {
                _exceptionCode = value;
            }
		}
	}
}
