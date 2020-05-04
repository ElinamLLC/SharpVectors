using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

using YDock;
using YDock.Enum;
using YDock.Interface;

namespace WpfSvgTestBox
{
    public class DockDocSourceFrame : DockSourceFrame, IDockDocSource
    {
        #region Protected Fields

        protected bool _dockIsModified;
        protected string _dockFileName;
        protected string _dockFullFileName;

        #endregion

        #region Constructors and Destructor

        public DockDocSourceFrame()
        {
        }

        #endregion

        #region IDockDocSource Members

        public string FileName
        {
            get {
                return _dockFileName;
            }
            set {
                _dockFileName = value;
            }
        }

        public string FullFileName
        {
            get {
                return _dockFullFileName;
            }
            set {
                _dockFullFileName = value;
            }
        }

        public bool IsModified
        {
            get {
                return _dockIsModified;
            }
            set {
                _dockIsModified = value;
            }
        }

        public bool AllowClose()
        {
            return false;
        }

        public void ReLoad()
        {
        }

        public void Save()
        {
        }

        #endregion
    }
}
