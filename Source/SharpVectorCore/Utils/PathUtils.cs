using System;
using System.IO;
using System.Reflection;

namespace SharpVectors.Dom.Utils
{
	/// <summary>
	/// Extends functionality of <see cref="Path"/> and provides additional methods for manipulating file or directory path information.
	/// </summary>
	public static class PathUtils
	{
		/// <summary>
		/// Combines an assembly location and an array of strings into a path.
		/// </summary>
		/// <param name="assembly">An <see cref="Assembly"/> which is taken as the base path.</param>
		/// <param name="paths">Path segments which are appended to the assembly location.</param>
		/// <returns>A string containing the combined path.</returns>
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

		public static string GetAssemblyPath(Assembly assembly)
		{
			var location = assembly.Location;

			if (!string.IsNullOrEmpty(location))
				return location;

			var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
			var assemblyName = GetAssemblyFileName(assembly);

			return Path.Combine(baseDirectory, assemblyName);
		}

		public static string GetAssemblyFileName(Assembly assembly) =>
			assembly.ManifestModule.Name;
	}
}
