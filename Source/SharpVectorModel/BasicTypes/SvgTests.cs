using System;
using System.Xml;
using System.Collections.Generic;

namespace SharpVectors.Dom.Svg
{
    /// <summary>    /// A class to encapsulate all SvgTest functionality. Used by SVG elements as a helper class    /// </summary>
    public sealed class SvgTests : ISvgTests
	{
		#region Private fields

        private readonly static SvgReadOnlyStringList ReadOnlyStringList = new SvgReadOnlyStringList();
		
        private SvgElement     _ownerElement;
        private ISvgStringList _requiredFeatures;
        private ISvgStringList _requiredExtensions;
        private ISvgStringList _systemLanguage;

		#endregion

        #region Constructor and Destructor

        public SvgTests(SvgElement ownerElement)
        {
            _ownerElement = ownerElement;
            _ownerElement.attributeChangeHandler += OnAttributeChange;
        }

        #endregion

        #region ISvgTest Members

        public ISvgStringList RequiredFeatures
        {
            get 
			{ 
				if (_requiredFeatures == null)
				{
                    if (_ownerElement.HasAttribute("requiredFeatures"))
                    {
                        _requiredFeatures = new SvgStringList(_ownerElement.GetAttribute("requiredFeatures"));
                    }
                    else
                    {
                        _requiredFeatures = ReadOnlyStringList;
                    }
				}
				return _requiredFeatures; 
			}
        }

        
		public ISvgStringList RequiredExtensions
        {
			get 
			{ 
				if (_requiredExtensions == null)
				{
                    if (_ownerElement.HasAttribute("requiredExtensions"))
                    {
                        _requiredExtensions = new SvgStringList(_ownerElement.GetAttribute("requiredExtensions"));
                    }
                    else
                    {
                        _requiredExtensions = ReadOnlyStringList;
                    }
				}

				return _requiredExtensions; 
			}
        }

		public ISvgStringList SystemLanguage
        {
			get 
			{ 
				if (_systemLanguage == null)
				{
                    if (_ownerElement.HasAttribute("systemLanguage"))
                    {
                        _systemLanguage = new SvgStringList(_ownerElement.GetAttribute("systemLanguage"));
                    }
                    else
                    {
                        _systemLanguage = ReadOnlyStringList;
                    }
				}

				return _systemLanguage; 
			}
        }

        public bool HasExtension(string extension)
        {
            bool result = false;

			for (uint i = 0; i < RequiredExtensions.NumberOfItems; i++)
            {
                if (string.Equals(RequiredExtensions.GetItem(i), extension))
                {
                    result = true;
                    break;
                }
            }
            
            return result;
        }

        #endregion

		#region Private Methods

		private void OnAttributeChange(Object src, XmlNodeChangedEventArgs args)
		{
			XmlAttribute attribute = src as XmlAttribute;

			if (attribute.NamespaceURI.Length == 0)
			{
				switch (attribute.LocalName)
				{
					case "requiredFeatures":
						_requiredFeatures = null;
						break;
					case "requiredExtensions":
						_requiredExtensions = null;
						break;
					case "systemLanguage":
						_systemLanguage = null;
						break;
				}
			}
		}

		#endregion

        #region SvgReadOnlyStringList Class

        private sealed class SvgReadOnlyStringList : ISvgStringList
        {
            #region ISvgStringList Members

            public uint NumberOfItems
            {
                get 
                { 
                    return 0; 
                }
            }

            public void Clear()
            {
            }

            public string Initialize(string newItem)
            {
                return null;
            }

            public string GetItem(uint index)
            {
                return null;
            }

            public string InsertItemBefore(string newItem, uint index)
            {
                return null;
            }

            public string ReplaceItem(string newItem, uint index)
            {
                return null;
            }

            public string RemoveItem(uint index)
            {
                return null;
            }

            public string AppendItem(string newItem)
            {
                return null;
            }

            public void FromString(string listString)
            {
            }

            #endregion

            #region IEnumerable<string> Members

            public IEnumerator<string> GetEnumerator()
            {
                return new List<string>().GetEnumerator();
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return new List<string>().GetEnumerator();
            }

            #endregion
        }

        #endregion
    }
}
