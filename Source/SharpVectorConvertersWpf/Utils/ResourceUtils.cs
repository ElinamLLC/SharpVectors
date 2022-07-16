using System;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

namespace SharpVectors.Converters.Utils
{
    // Detect whether WPF resource exists, based on URI
    // https://stackoverflow.com/questions/2013481/detect-whether-wpf-resource-exists-based-on-uri
    internal static class WpfResources
    {
        public static bool ResourceExists(string resourcePath)
        {
            var assembly = Assembly.GetEntryAssembly();

            return ResourceExists(assembly, resourcePath);
        }

        public static bool ResourceExists(Assembly assembly, string resourcePath)
        {
            if (assembly == null)
            {
                assembly = Assembly.GetEntryAssembly();
            }

            try
            {
                var resourceName = resourcePath.ToLowerInvariant().Replace("\\", "/");
                return GetResourcePaths(assembly).Contains(resourceName, StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                return false;
            }
        }

        public static IEnumerable<string> GetResourcePaths(Assembly assembly)
        {
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var resourceName = assembly.GetName().Name + ".g";
            var resourceManager = new ResourceManager(resourceName, assembly);

            try
            {
                var resourceSet = resourceManager.GetResourceSet(culture, true, true);

                foreach (DictionaryEntry resource in resourceSet)
                {
                    yield return resource.Key.ToString();
                }
            }
            finally
            {
                resourceManager.ReleaseAllResources();
            }
        }
    }
}
