using System;
using System.IO;
using System.Security;
using System.Security.Permissions;

namespace SharpVectors.Renderers.Utils
{
    public sealed class WpfApplicationContext
    {
        public static DirectoryInfo ExecutableDirectory
        {
            get
            {
                DirectoryInfo di;
                try
                {
                    FileIOPermission f = new FileIOPermission(PermissionState.None);
                    f.AllLocalFiles = FileIOPermissionAccess.Read;

                    f.Assert();

                    di = new DirectoryInfo(Path.GetDirectoryName(
                        System.Reflection.Assembly.GetExecutingAssembly().Location));
                }
                catch(SecurityException)
                {
                    di = new DirectoryInfo(Directory.GetCurrentDirectory());
                }
                return di;
            }
        }

        public static DirectoryInfo DocumentDirectory
        {
            get
            {
                return new DirectoryInfo(Directory.GetCurrentDirectory());
            }
        }

        public static Uri DocumentDirectoryUri
        {
            get
            {
                string sUri = DocumentDirectory.FullName + "/";
                sUri = "file://" + sUri.Replace("\\", "/");
                return new Uri(sUri);
            }
        }
    }
}
