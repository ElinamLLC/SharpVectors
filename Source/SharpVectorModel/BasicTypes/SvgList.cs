using System;using System.Collections;
using System.Collections.Generic;
namespace SharpVectors.Dom.Svg{	/// <summary>	/// Base class for all SVG*List-derived classes.
    /// Note we're using <see cref="List{T}"/> (as opposed to deriving from) to hide unneeded <see cref="List{T}"/> methods	/// Note that a CLR uint is equivalent to an IDL ulong, so uint is used for all index values	/// </summary>
	public abstract class SvgList<T> : IEnumerable<T>	{        #region Private Fields

        protected List<T> _items;        private IDictionary<T, SvgList<T>> _itemOwnerMap;        #endregion
        #region Constructor        /// <summary>        /// SvgList constructor        /// </summary>		protected SvgList()		{			_items        = new List<T>();            _itemOwnerMap = new Dictionary<T, SvgList<T>>();		}        #endregion        #region ISvgList Interface        public int Count
        {
            get {
                return _items.Count;
            }
        }        public T this[int index]
        {
            get {
                return this.GetItem((uint)index);
            }
        }        /// <summary>        /// NumberOfItems        /// </summary>        public uint NumberOfItems        {
            get { return (uint) _items.Count; }
        }

        /// <summary>        /// Clear        /// </summary>
        public void Clear()
        {
            // Note that we cannot use List<T>'s Clear method since we need to
            // remove all items from the itemOwnerMap
            while (_items.Count > 0) 
                RemoveItem(0);
        }

        /// <summary>        /// Initialize        /// </summary>        /// <param name="newItem"></param>        /// <returns></returns>
        public T Initialize(T newItem)
        {
            Clear();
            return AppendItem(newItem);
        }

        /// <summary>        /// GetItem        /// </summary>        /// <param name="index"></param>        /// <returns></returns>
        public T GetItem(uint index)
        {
            if ( index < 0 || _items.Count <= index )
                throw new DomException(DomExceptionType.IndexSizeErr);

            return _items[(int)index];
        }

        /// <summary>        /// InsertItemBefore        /// </summary>        /// <param name="newItem"></param>        /// <param name="index"></param>        /// <returns></returns>
        public T InsertItemBefore(T newItem, uint index)
        {
            if ( index < 0 || _items.Count <= index )
                throw new DomException(DomExceptionType.IndexSizeErr);

            // cache cast
            int i = (int) index;

            // if newItem exists in a list, remove it from that list
            if (_itemOwnerMap.ContainsKey(newItem) )
                _itemOwnerMap[newItem].RemoveItem(newItem);

            // insert item into this list
            _items.Insert(i, newItem);

            // update the itemOwnerMap to associate newItem with this list
            _itemOwnerMap[newItem] = this;

            return _items[i];
        }

        /// <summary>        /// ReplaceItem        /// </summary>        /// <param name="newItem"></param>        /// <param name="index"></param>        /// <returns></returns>
        public T ReplaceItem(T newItem, uint index)
        {
            if ( index < 0 || _items.Count <= index )
                throw new DomException(DomExceptionType.IndexSizeErr);

            // cache cast
            int i = (int) index;

            // if newItem exists in a list, remove it from that list
            if (_itemOwnerMap.ContainsKey(newItem))
                _itemOwnerMap[newItem].RemoveItem(newItem);

            // remove oldItem from itemOwnerMap
            _itemOwnerMap.Remove(_items[i]);

            // update the itemOwnerMap to associate newItem with this list
            _itemOwnerMap[newItem] = this;

            // store newItem and return
            return _items[i] = newItem;
        }

        /// <summary>        /// RemoveItem        /// </summary>        /// <param name="index"></param>        /// <returns></returns>
        public T RemoveItem(uint index)
        {
            if ( index < 0 || _items.Count <= index )
                throw new DomException(DomExceptionType.IndexSizeErr);

            // cache cast
            int i = (int)index;

            // save removed item so we can return it
            T result = _items[i];

            // item is longer associated with this list, so remove item from itemOwnerMap
            _itemOwnerMap.Remove(result);

            // remove item from this list
            _items.RemoveAt(i);

            // return removed item
            return result;
        }

        /// <summary>        /// AppendItem        /// </summary>        /// <param name="newItem"></param>        /// <returns></returns>
        public T AppendItem(T newItem)        {            // if item exists in a list, remove it from that list
            if (_itemOwnerMap.ContainsKey(newItem))
                _itemOwnerMap[newItem].RemoveItem(newItem);
            // update the itemOwnerMap to associate newItem with this list
            _itemOwnerMap[newItem] = this;
            // add item and return            _items.Add(newItem);
            return newItem;        }        #endregion
        #region IEnumerable Interface        public IEnumerator<T> GetEnumerator()         {
            return _items.GetEnumerator();        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }
        #endregion        #region Support Methods        /// <summary>        /// RemoveItem - used to remove an item by value as opposed to by position        /// </summary>        /// <param name="item"></param>        private void RemoveItem(T item)        {
            int index = _items.IndexOf(item);            if (index >= 0)            {
                this.RemoveItem((uint)index);            }        }        #endregion
    }}