using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace SharpVectors.Converters
{
    public class VerticalTabItem : TabItem
    {
        public VerticalTabItem()
        {   
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // this will handle the case for applying VSM on startup
            UpdateState(false);
        }

        protected override void OnPropertyChanged(
            DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == TabItem.IsSelectedProperty)
            {
                // here is the normal case when selection changes
                UpdateState(false);
            }
        }   

        private void UpdateState(bool useTransitions)
        {
            if (this.IsSelected)
            {
                VisualStateManager.GoToState(this, "Selected", useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, "Unselected", useTransitions);
            }
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            if (!this.IsSelected)
            {                                    
                VisualStateManager.GoToState(this, "MouseOver", true);
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (!this.IsSelected)
            {
                VisualStateManager.GoToState(this, "Normal", true);
            }
        }
    }
}
