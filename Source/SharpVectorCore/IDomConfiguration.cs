using System.Collections.Generic;

namespace SharpVectors.Dom
{
	/// <summary>
	/// The <see cref="IDomConfiguration"/> interface represents the configuration of a document 
	/// and maintains a table of recognized parameters.
	/// </summary>
	/// <seealso href="https://developer.mozilla.org/en-US/docs/Web/API/DOMConfiguration">DOMConfiguration</seealso>
	public interface IDomConfiguration
	{
		/// <summary>
		/// Check if setting a parameter to a specific value is supported.
		/// </summary>
		/// <param name="name">The name of the parameter to check.</param>
		/// <param name="value">An object. If <see langword="null"/>, the returned value is <see langword="true"/>.</param>
		/// <returns>Returns <see langword="true"/> if the parameter could be successfully set to the specified value, 
		/// or <see langword="false"/> if the parameter is not recognized or the requested value is not supported. 
		/// This does not change the current value of the parameter itself.</returns>
		bool CanSetParameter(string name, object value);

		/// <summary>
		/// Gets the value of the known parameter with the specified name.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <returns>The current object associated with the specified parameter or <see langword="null"/> if no object 
		/// has been associated or if the parameter is not supported.</returns>
		object GetParameter(string name);

		/// <summary>
		/// Sets the name and the value of a parameter. If the name is already set, the value is changed.
		/// </summary>
		/// <param name="name">The name of the parameter to set.</param>
		/// <param name="value">The value or <see langword="null"/> to unset the parameter.</param>
		void SetParameter(string name, object value);

		/// <summary>
		/// The list of the parameters supported by this <see cref="IDomConfiguration"/> object.
		/// </summary>
		/// <returns>A collection specifying the names of the parameters.</returns>
		ICollection<string> GetParameterNames();
	}
}
