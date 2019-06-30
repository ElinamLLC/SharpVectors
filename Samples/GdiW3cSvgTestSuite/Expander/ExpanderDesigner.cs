//-------------------------------------------------------------------------------- 
// Original source: https://github.com/mkoertgen/winforms.expander
// Modified for the Windows Forms test suite application
//-------------------------------------------------------------------------------- 

using System.ComponentModel;
using System.Windows.Forms.Design;

namespace WinFormsExpander
{
    public class ExpanderDesigner : ParentControlDesigner
    {
        private Expander _expander;

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);

            _expander = (Expander) Control;
            EnableDesignMode(_expander.Content, "Content");
        }
    }
}