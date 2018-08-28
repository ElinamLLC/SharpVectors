using System;
using System.Xml;

namespace SharpVectors.Dom.Svg
{
    public sealed class SvgElementInstanceList : ISvgElementInstanceList
    {
        #region Private Fields

        private SvgElementInstance[] _items;
        
        #endregion

        #region Constructors

        public SvgElementInstanceList(SvgUseElement useElement, SvgElementInstance parent)
        {
            if (parent.CorrespondingElement == null)
            {
                // Handle non SVGElement cases
                _items = new SvgElementInstance[0];
            }
            else if (parent.CorrespondingElement is ISvgUseElement)
            {
                // Handle recursive SVGUseElement cases
                _items    = new SvgElementInstance[1];
                ISvgUseElement iUseElement = (ISvgUseElement)parent.CorrespondingElement;
                _items[0] = (SvgElementInstance)iUseElement.InstanceRoot;
                return;
            }
            else
            {
                XmlNodeList xmlChildNodes = parent.CorrespondingElement.ChildNodes;
                for (int i = 0; i < xmlChildNodes.Count; i++)
                {
                    _items[i] = new SvgElementInstance(xmlChildNodes[i], useElement, parent);
                    if (i > 0)
                    {
                        _items[i].SetPreviousSibling(_items[i - 1]);
                        _items[i - 1].SetNextSibling(_items[i]);
                    }
                }
            }
        }

        #endregion

        #region ISvgElementInstanceList Members

        public ulong Length
        {
            get
            {
                return (ulong)_items.GetLength(0);
            }
        }

        public ISvgElementInstance Item(ulong index)
        {
            if (index < Length)
            {
                return (ISvgElementInstance)_items.GetValue((int)index);
            }
            return null;
        }

        #endregion
    }
}
