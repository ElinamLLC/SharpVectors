using System;
using System.IO;
using System.Reflection;

namespace SharpVectors.Dom.Utils
{
	public static class LocationUtils
	{
		public static string GetAssemblyDirectory(Assembly assembly)
		{
			var location = assembly.Location;

			return string.IsNullOrEmpty(location)
				? AppDomain.CurrentDomain.BaseDirectory
				: Path.GetDirectoryName(location);
		}
	}
}
