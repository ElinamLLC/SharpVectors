using System;
using System.IO;
using System.Reflection;

namespace SharpVectors.Dom.Utils
{
	/// <summary>
	/// Extends functionality of <see cref="Path"/> and provides additional methods for manipulating file or directory path information
	/// </summary>
	public static class PathUtils
	{
		/// <summary>
		/// Combines an assembly location and an array of strings into a path
		/// </summary>
		/// <param name="assembly">Assembly which is taken as the base path</param>
		/// <param name="paths">Path segments which are appended to the assembly location</param>
		/// <returns>The contained paths</returns>
		public static string Combine(Assembly assembly, params string[] paths)
		{
			var location = assembly.Location;

			var basePath = string.IsNullOrEmpty(location)
				? AppDomain.CurrentDomain.BaseDirectory
				: Path.GetDirectoryName(location);

			if (paths.Length == 0)
				return basePath;

			var newPaths = new string[paths.Length + 1];
			Array.Copy(paths, 0, newPaths, 1, paths.Length);
			newPaths[0] = basePath;

			return Path.Combine(newPaths);
		}
	}
}
