﻿using System;
using System.Xml;

using System.Windows;

using SharpVectors.Renderers;

namespace SharpVectors.Test.Sample
{
    internal sealed class CustomResourceKeyResolver : WpfSettings<CustomResourceKeyResolver>, IResourceKeyResolver
    {
        public CustomResourceKeyResolver()
        {
        }

        public override CustomResourceKeyResolver Clone()
        {
            return new CustomResourceKeyResolver();
        }

        public override void ReadXml(XmlReader reader)
        {
            NotNull(reader, nameof(reader));
        }

        public override void WriteXml(XmlWriter writer)
        {
            NotNull(writer, nameof(writer));
        }

        public ResourceKeyResolverType ResolverType
        {
            get {
                return ResourceKeyResolverType.Custom;
            }
        }

        public bool IsValid
        {
            get {
                return true;
            }
        }

        public void BeginResolve()
        {
        }

        public void EndResolve()
        {
        }

        public string Resolve(DependencyObject resource, int index, string fileName, string fileSource)
        {
            if (index < 0)
            {
                throw new ArgumentException("The specified index is invalid", "index");
            }
            NotNullNotEmpty(fileName, "fileName");

            var keyValue = ToUpperCamelCase(fileName.ToUpper());
            if (!string.IsNullOrWhiteSpace(keyValue) && keyValue.Length >= 3 && keyValue.Length < 255)
            {
                return keyValue;
            }
            return fileName;
        }

        internal static string ToUpperCamelCase(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return string.Empty;
            }

            string camelCaseStr = fileName[0].ToString();

            if (fileName.Length > 1)
            {
                bool isStartOfWord = false;
                for (int i = 1; i < fileName.Length; i++)
                {
                    char currChar = fileName[i];
                    if (currChar == '_' || currChar == '-')
                    {
                        isStartOfWord = true;
                    }
                    else if (char.IsUpper(currChar))
                    {
                        if (isStartOfWord)
                        {
                            camelCaseStr += currChar;
                        }
                        else
                        {
                            camelCaseStr += char.ToLower(currChar);
                        }
                        isStartOfWord = false;
                    }
                    else
                    {
                        camelCaseStr += currChar;
                        isStartOfWord = false;
                    }
                }
            }
            return camelCaseStr;
        }
    }
}
