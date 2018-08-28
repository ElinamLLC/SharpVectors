using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Xml;

using SharpVectors.Net;

namespace SharpVectors.Renderers.Forms
{
    public class SvgCacheManager : ICacheManager
    {
        private string _cacheDir;
        private string _cacheDocPath;
        private XmlDocument _cacheDoc;
        private XmlElement _lastCacheElm;
        private Uri _lastUri;

        public SvgCacheManager()
        {
            _cacheDoc     = new XmlDocument();
            _cacheDir     = Path.Combine(SvgApplicationContext.ExecutableDirectory.FullName, "cache/");
            _cacheDocPath = Path.Combine(_cacheDir, "cache.xml");

            loadDoc();
        }

        public SvgCacheManager(string cacheDir)
        {
            _cacheDoc     = new XmlDocument();
            _cacheDir     = Path.Combine(SvgApplicationContext.ExecutableDirectory.FullName, cacheDir);
            _cacheDocPath = Path.Combine(_cacheDir, "cache.xml");

            loadDoc();
        }

        public long Size
        {
            get {
                DirectoryInfo di = new DirectoryInfo(_cacheDir);
                FileInfo[] files = di.GetFiles();
                long size = 0;
                foreach (FileInfo file in files)
                {
                    size += file.Length;
                }
                return size;
            }
        }

        public void Clear()
        {
            DirectoryInfo di = new DirectoryInfo(_cacheDir);
            FileInfo[] files = di.GetFiles();
            foreach (FileInfo file in files)
            {
                try
                {
                    file.Delete();
                }
                catch { }
            }

            _cacheDoc = new XmlDocument();

            loadDoc();
        }

        public CacheInfo GetCacheInfo(Uri uri)
        {
            XmlElement cacheElm = getCacheElm(uri);

            DateTime expires = DateTime.MinValue;
            if (cacheElm.HasAttribute("expires"))
            {
                expires = DateTime.Parse(cacheElm.GetAttribute("expires"));
            }

            DateTime lastModified = DateTime.MinValue;
            if (cacheElm.HasAttribute("last-modified"))
            {
                lastModified = DateTime.Parse(cacheElm.GetAttribute("last-modified"));
            }

            Uri cachedUri = getLocalPathUri(cacheElm);

            return new CacheInfo(expires, cacheElm.GetAttribute("etag"), lastModified, 
                cachedUri, cacheElm.GetAttribute("content-type"));
        }

        public void SetCacheInfo(Uri uri, CacheInfo cacheInfo, Stream stream)
        {
            XmlElement cacheElm = getCacheElm(uri);

            if (cacheInfo != null)
            {
                if (cacheInfo.ETag != null)
                {
                    cacheElm.SetAttribute("etag", cacheInfo.ETag);
                }
                else
                {
                    cacheElm.RemoveAttribute("etag");
                }

                if (cacheInfo.ContentType != null)
                {
                    cacheElm.SetAttribute("content-type", cacheInfo.ContentType);
                }
                else
                {
                    cacheElm.RemoveAttribute("content-type");
                }

                if (cacheInfo.Expires != DateTime.MinValue)
                {
                    cacheElm.SetAttribute("expires", cacheInfo.Expires.ToString("s"));
                }
                else
                {
                    cacheElm.RemoveAttribute("expires");
                }

                if (cacheInfo.LastModified != DateTime.MinValue)
                {
                    cacheElm.SetAttribute("last-modified", cacheInfo.LastModified.ToString("s"));
                }
                else
                {
                    cacheElm.RemoveAttribute("last-modified");
                }
            }

            if (stream != null)
            {
                string localPath;
                if (cacheElm.HasAttribute("local-path"))
                {
                    localPath = cacheElm.GetAttribute("local-path");
                }
                else
                {
                    localPath = Guid.NewGuid().ToString() + ".cache";
                    cacheElm.SetAttribute("local-path", localPath);
                }

                stream.Position = 0;
                int count;
                byte[] buffer = new byte[4096];

                FileStream fs = File.OpenWrite(Path.Combine(_cacheDir, localPath));
                while ((count = stream.Read(buffer, 0, 4096)) > 0) fs.Write(buffer, 0, count);
                fs.Flush();
                fs.Close();
            }
            saveDoc();
        }

        private void loadDoc()
        {
            if (File.Exists(_cacheDocPath))
            {
                _cacheDoc.Load(_cacheDocPath);
            }
            else
            {
                Directory.CreateDirectory(Directory.GetParent(_cacheDocPath).FullName);
                _cacheDoc.LoadXml("<cache />");
            }
        }

        private void saveDoc()
        {
            _cacheDoc.Save(_cacheDocPath);
        }

        private XmlElement getCacheElm(Uri uri)
        {
            if (uri == _lastUri && _lastCacheElm != null)
            {
                return _lastCacheElm;
            }
            //string xpath = "/cache/resource[@url='" + uri.ToString() + "']";
            string xpath = "/cache/resource[@url='" + uri.ToString().Replace("'", "&apos;") + "']";
            XmlNode node = _cacheDoc.SelectSingleNode(xpath);
            if (node != null)
            {
                _lastCacheElm = node as XmlElement;
            }
            else
            {
                _lastCacheElm = _cacheDoc.CreateElement("resource");
                _cacheDoc.DocumentElement.AppendChild(_lastCacheElm);
                _lastCacheElm.SetAttribute("url", uri.ToString());
            }

            _lastUri = uri;
            return _lastCacheElm;
        }

        private Uri getLocalPathUri(XmlElement cacheElm)
        {
            if (cacheElm.HasAttribute("local-path"))
            {
                string path = Path.Combine(_cacheDir, cacheElm.GetAttribute("local-path"));
                if (File.Exists(path))
                {
                    path = "file:///" + path.Replace('\\', '/');
                    return new Uri(path);
                }
                cacheElm.RemoveAttribute("local-path");
                return null;
            }
            return null;
        }

    }
}
