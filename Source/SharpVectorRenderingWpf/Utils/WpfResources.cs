using System;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Collections;
using System.Collections.Generic;

namespace SharpVectors.Renderers.Utils
{
    // Detect whether WPF resource exists, based on URI
    // https://stackoverflow.com/questions/2013481/detect-whether-wpf-resource-exists-based-on-uri
    public static class WpfResources
    {
        public static bool ResourceExists(string resourcePath)
        {
            var assembly = Assembly.GetExecutingAssembly();

            return ResourceExists(assembly, resourcePath);
        }

        public static bool ResourceExists(Assembly assembly, string resourcePath)
        {
            return GetResourcePaths(assembly).Contains(resourcePath.ToLowerInvariant());
        }

        public static IEnumerable<object> GetResourcePaths(Assembly assembly)
        {
            var culture         = System.Threading.Thread.CurrentThread.CurrentCulture;
            var resourceName    = assembly.GetName().Name + ".g";
            var resourceManager = new ResourceManager(resourceName, assembly);

            try
            {
                var resourceSet = resourceManager.GetResourceSet(culture, true, true);

                foreach (DictionaryEntry resource in resourceSet)
                {
                    yield return resource.Key;
                }
            }
            finally
            {
                resourceManager.ReleaseAllResources();
            }
        }
    }
}
