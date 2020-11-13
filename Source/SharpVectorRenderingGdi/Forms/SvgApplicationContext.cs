using System;
using System.IO;
using System.Security;
using System.Security.Permissions;

namespace SharpVectors.Renderers.Forms
{
    public static class SvgApplicationContext
    {
        public static DirectoryInfo ExecutableDirectory
        {
            get {
                DirectoryInfo di;
                try
                {
#if !NET50
                    FileIOPermission f = new FileIOPermission(PermissionState.None);
                    f.AllLocalFiles = FileIOPermissionAccess.Read;

                    f.Assert();
#endif

                    di = new FileInfo(System.Windows.Forms.Application.ExecutablePath).Directory;
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
