using System;
using System.IO;
using System.Security;
using System.Security.Permissions;

namespace SharpVectors.Renderers.Utils
{
    internal static class WpfApplicationContext
    {
        public static DirectoryInfo ExecutableDirectory
        {
            get {
                DirectoryInfo di;
                try
                {
#if !NETCORE
                    FileIOPermission f = new FileIOPermission(PermissionState.None);
                    f.AllLocalFiles = FileIOPermissionAccess.Read;

                    f.Assert();
#endif
                    di = new DirectoryInfo(SharpVectors.Dom.Utils.PathUtils.Combine(
                        System.Reflection.Assembly.GetExecutingAssembly()));
                }
                catch (SecurityException)
                {
                    di = new DirectoryInfo(Directory.GetCurrentDirectory());
                }
                return di;
            }
        }

        public static DirectoryInfo DocumentDirectory
        {
            get {
                return new DirectoryInfo(Directory.GetCurrentDirectory());
            }
        }

        public static Uri DocumentDirectoryUri
        {
            get {
                string sUri = DocumentDirectory.FullName + "/";
                sUri = "file://" + sUri.Replace("\\", "/");
                return new Uri(sUri);
            }
        }
    }
}
