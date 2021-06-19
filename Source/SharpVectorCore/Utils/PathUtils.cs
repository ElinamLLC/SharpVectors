using System;
using System.IO;
using System.Reflection;

namespace SharpVectors.Dom.Utils
{
	public static class PathUtils
	{
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
