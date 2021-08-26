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
		public static string Combine(Assembly assembly, params string[] paths) =>
			CombineInternal(assembly.Location, paths);

		/// <summary>
		/// Gets the full path to the assembly file.
		/// </summary>
		/// <param name="assembly">An <see cref="Assembly"/> which is taken as the base path.</param>
		/// <returns>A string containing the full path to the assembly file.</returns>
		public static string GetAssemblyPath(Assembly assembly) =>
			GetAssemblyPathInternal(assembly, assembly.Location);

		/// <summary>
		/// Gets the file name if the assembly.
		/// </summary>
		/// <param name="assembly">An <see cref="Assembly"/> which is taken as the base path.</param>
		/// <returns>A string containing the file name of the assembly.</returns>
		public static string GetAssemblyFileName(Assembly assembly) =>
			assembly.ManifestModule.Name;

		/// <summary>
		/// Exposes <see cref="Combine"/> for unit-testing where it is possible to mock an empty location
		/// </summary>
		private static string CombineInternal(string location, string[] paths)
		{
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

		/// <summary>
		/// Exposes <see cref="GetAssemblyPath"/> for unit-testing where it is possible to mock an empty location
		/// </summary>
		private static string GetAssemblyPathInternal(Assembly assembly, string location)
		{
			if (!string.IsNullOrEmpty(location))
				return location;

			var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
			var assemblyName = GetAssemblyFileName(assembly);

			return Path.Combine(baseDirectory, assemblyName);
		}
	}
}
