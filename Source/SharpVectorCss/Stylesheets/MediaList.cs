using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SharpVectors.Dom.Stylesheets
{
    /// <summary>
    /// <para>
    /// The MediaList interface provides the abstraction of an ordered collection of media, 
    /// without defining or constraining how this collection is implemented. An empty list 
    /// is the same as a list that contains the medium "all".
    /// </para>
    /// <para>
    /// The items in the MediaList are accessible via an integral index, starting from 0.
    /// </para>
    /// </summary>
    public sealed class MediaList : IMediaList
    {
        #region Private fields

        private static readonly Regex _splitter = new Regex(@"\s*,\s*");

        private bool _containsAll;	//speeds up matching if the list contains "all"
        private IList<string> _medias;

        #endregion

        #region Constructors

        public MediaList()
            : this(string.Empty)
        {
        }

        public MediaList(string val)
        {
            _medias = new List<string>();

            ParseString(val);
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Compares this MediaList with another and see if the second fits this
        /// </summary>
        /// <param name="inMedia">The MediaList to compare</param>
        /// <returns>True if this list fits the specified</returns>
        public bool Matches(MediaList inMedia)
        {
            if (inMedia.Length == 0) return false;
            else if (Length == 0 || _containsAll)
            {
                // is empty or this list contains "all"
                return true;
            }
            else
            {
                for (ulong i = 0; i < inMedia.Length; i++)
                {
                    if (_medias.Contains(inMedia[i]))
                        return true;
                }
            }
            return false;
        }

        #endregion

        #region Private Methods

        private void ParseString(string mediaText)
        {
            mediaText = mediaText.Trim();
            if (mediaText.Length > 0)
            {
                string[] ms = _splitter.Split(mediaText);
                foreach (string m in ms)
                {
                    AppendMedium(m);
                }
            }
        }

        private void ClearMedia()
        {
            _medias.Clear();
        }

        #endregion

        #region Implementation of IMediaList

        /// <summary>
        /// Adds the medium newMedium to the end of the list. If the newMedium is already used, it is first removed.
        /// </summary>
        /// <param name="newMedium">The new medium to add.</param>
        /// <exception cref="DomException">
        /// INVALID_CHARACTER_ERR: If the medium contains characters that are invalid in the underlying style language.
        /// </exception>
        /// <exception cref="DomException">
        /// NO_MODIFICATION_ALLOWED_ERR: Raised if this media list is readonly.
        /// </exception>
        public void AppendMedium(string newMedium)
        {
            if (newMedium.Length > 0)
            {
                _medias.Remove(newMedium);
                _medias.Add(newMedium);
                if (newMedium.Equals("all"))
                    _containsAll = true;
            }
        }

        /// <summary>
        /// Deletes the medium indicated by oldMedium from the list.
        /// </summary>
        /// <param name="oldMedium">The medium to delete in the media list.</param>
        /// <exception cref="DomException">NOT_FOUND_ERR: Raised if oldMedium is not in the list.</exception>
        /// <exception cref="DomException">
        /// NO_MODIFICATION_ALLOWED_ERR: Raised if this media list is readonly.
        /// </exception>
        public void DeleteMedium(string oldMedium)
        {
            if (oldMedium == "all")
                _containsAll = false;

            _medias.Remove(oldMedium);
        }

        /// <summary>
        /// The number of media in the list. The range of valid media is 0 to length-1 inclusive.
        /// </summary>
        public ulong Length
        {
            get {
                return (ulong)_medias.Count;
            }
        }

        /// <summary>
        /// The parsable textual representation of the media list. This is a comma-separated list of media.
        /// </summary>
        /// <exception cref="DomException">
        /// SYNTAX_ERR: Raised if the specified string value has a syntax error and is unparsable.
        /// </exception>
        /// <exception cref="DomException">
        /// NO_MODIFICATION_ALLOWED_ERR: Raised if this media list is readonly.
        /// </exception>
        public string MediaText
        {
            get {
                string result = string.Empty;
                foreach (string media in _medias)
                {
                    result += media + ",";
                }
                if (result.Length > 0)
                {
                    result = result.Substring(0, result.Length - 1);
                }
                return result;
            }
            set {
                ClearMedia();
                ParseString(value);
            }
        }

        /// <summary>
        /// Returns the indexth in the list. If index is greater than or equal to the 
        /// number of media in the list, this returns null.
        /// </summary>
        public string this[ulong index]
        {
            get {
                return (index < Length) ? _medias[(int)index] : null;
            }
        }

        /// <summary>
        /// Returns the indexth in the list. If index is greater than or equal to the 
        /// number of media in the list, this returns null.
        /// </summary>
        public string this[int index]
        {
            get {
                return (index < (int)Length) ? _medias[index] : null;
            }
        }

        #endregion
    }
}
