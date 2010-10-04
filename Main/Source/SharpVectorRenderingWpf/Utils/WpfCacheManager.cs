using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Diagnostics;

using SharpVectors.Net;

namespace SharpVectors.Renderers.Utils
{
	public sealed class WpfCacheManager : ICacheManager
	{
        private XmlElement lastCacheElm;
        private Uri lastUri;

		private string cacheDir;
		private string cacheDocPath;
		private XmlDocument cacheDoc = new XmlDocument();

		public WpfCacheManager()
		{
            cacheDoc = new XmlDocument();

			cacheDir = Path.Combine(WpfApplicationContext.ExecutableDirectory.FullName, "cache/");
			cacheDocPath = Path.Combine(cacheDir, "cache.xml");

			LoadDoc();
		}

		public WpfCacheManager(string cacheDir)
		{
            cacheDoc = new XmlDocument();

            this.cacheDir = Path.Combine(
				WpfApplicationContext.ExecutableDirectory.FullName,
				cacheDir);

			cacheDocPath = Path.Combine(cacheDir, "cache.xml");

			LoadDoc();
		}

		private void LoadDoc()
		{
            if (cacheDoc == null)
            {
                cacheDoc = new XmlDocument();
            }

			if(File.Exists(cacheDocPath))
			{
				cacheDoc.Load(cacheDocPath);
			}
			else
			{
				Directory.CreateDirectory(Directory.GetParent(cacheDocPath).FullName);
				cacheDoc.LoadXml("<cache />");
			}
		}

		private void SaveDoc()
		{
            if (cacheDoc == null)
            {
                return;
            }
			cacheDoc.Save(cacheDocPath);
		}

		private XmlElement GetCacheElm(Uri uri)
		{
			if(uri == lastUri && lastCacheElm != null)
			{
				return lastCacheElm;
			}
			else
			{
				//string xpath = "/cache/resource[@url='" + uri.ToString() + "']";
                string xpath = "/cache/resource[@url='" + uri.ToString().Replace("'", "&apos;") + "']";
                XmlNode node = cacheDoc.SelectSingleNode(xpath);
				if(node != null)
				{
					lastCacheElm = node as XmlElement;
				}
				else
				{
					lastCacheElm = cacheDoc.CreateElement("resource");
					cacheDoc.DocumentElement.AppendChild(lastCacheElm);
					lastCacheElm.SetAttribute("url", uri.ToString());
				}

				lastUri = uri;
				return lastCacheElm;
			}
		}

		private Uri GetLocalPathUri(XmlElement cacheElm)
		{
			if (cacheElm.HasAttribute("local-path"))
			{
				string path = Path.Combine(cacheDir, cacheElm.GetAttribute("local-path"));
				if(File.Exists(path))
				{
					path = "file:///" + path.Replace('\\', '/');
					return new Uri(path);
				}
				else
				{
					cacheElm.RemoveAttribute("local-path");
					return null;
				}
			}
			else
			{
				return null;
			}
		}
		
		public long Size
		{
			get
			{
				DirectoryInfo di = new DirectoryInfo(cacheDir);
				FileInfo[] files = di.GetFiles();
				long size = 0;
				foreach(FileInfo file in files)
				{
					size += file.Length;
				}
				return size;				
			}
		}

		public void Clear()
		{
			DirectoryInfo di = new DirectoryInfo(cacheDir);
			FileInfo[] files = di.GetFiles();
			foreach(FileInfo file in files)
			{
				try
				{
					file.Delete();
				}
				catch{}
			}

			cacheDoc = new XmlDocument();

			LoadDoc();
		}

		public CacheInfo GetCacheInfo(Uri uri)
		{
			XmlElement cacheElm = GetCacheElm(uri);

			DateTime expires = DateTime.MinValue;
			if(cacheElm.HasAttribute("expires"))
			{
				expires = DateTime.Parse(cacheElm.GetAttribute("expires"));
			}

			DateTime lastModified = DateTime.MinValue;
			if(cacheElm.HasAttribute("last-modified"))
			{
				lastModified = DateTime.Parse(cacheElm.GetAttribute("last-modified"));
			}

			Uri cachedUri = GetLocalPathUri(cacheElm);

            return new CacheInfo(expires, cacheElm.GetAttribute("etag"), lastModified, cachedUri, cacheElm.GetAttribute("content-type"));
		}

		public void SetCacheInfo(Uri uri, CacheInfo cacheInfo, Stream stream)
		{	
			XmlElement cacheElm = GetCacheElm(uri);

			if(cacheInfo != null)
			{
				if(cacheInfo.ETag != null)
				{
					cacheElm.SetAttribute("etag", cacheInfo.ETag);
				}
				else
				{
					cacheElm.RemoveAttribute("etag");
				}

				if(cacheInfo.ContentType != null)
				{
					cacheElm.SetAttribute("content-type", cacheInfo.ContentType);
				}
				else
				{
					cacheElm.RemoveAttribute("content-type");
				}

				if(cacheInfo.Expires != DateTime.MinValue)
				{
					cacheElm.SetAttribute("expires", cacheInfo.Expires.ToString("s"));
				}
				else
				{
					cacheElm.RemoveAttribute("expires");
				}

				if(cacheInfo.LastModified != DateTime.MinValue)
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
				if(cacheElm.HasAttribute("local-path"))
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

				FileStream fs = File.OpenWrite(Path.Combine(cacheDir, localPath));
				while((count = stream.Read(buffer, 0, 4096)) > 0) fs.Write(buffer, 0, count);
				fs.Flush();
				fs.Close();
			}
			SaveDoc();
		}
	}
}
