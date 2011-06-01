// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Microsoft.Windows.Controls
{
    /// <summary>
    ///     Provides VisualStateManager behavior for ToggleButton controls.
    /// </summary>
    public class ToggleButtonBehavior : ButtonBaseBehavior
    {
        /// <summary>
        ///     This behavior targets ToggleButton derived Controls.
        /// </summary>
        protected override internal Type TargetType
        {
            get { return typeof(ToggleButton); }
        }

        /// <summary>
        ///     Attaches to property changes and events.
        /// </summary>
        /// <param name="control">An instance of the control.</param>
        protected override void OnAttach(Control control)
        {
            base.OnAttach(control);

            ToggleButton toggle = (ToggleButton)control;
            Type targetType = typeof(ToggleButton);

            AddValueChanged(ToggleButton.IsCheckedProperty, targetType, toggle, UpdateStateHandler);
        }

        /// <summary>
        /// Detaches property changes and events.
        /// </summary>
        /// <param name="control">The control</param>
        protected override void OnDetach(Control control)
        {
            base.OnDetach(control);

            ToggleButton toggle = (ToggleButton)control;
            Type targetType = typeof(ToggleButton);

            RemoveValueChanged(ToggleButton.IsCheckedProperty, targetType, toggle, UpdateStateHandler);
        }

        /// <summary>
        ///     Called to update the control's visual state.
        /// </summary>
        /// <param name="control">The instance of the control being updated.</param>
        /// <param name="useTransitions">Whether to use transitions or not.</param>
        protected override void UpdateState(Control control, bool useTransitions)
        {
            ToggleButton toggle = (ToggleButton)control;

            if (!toggle.IsChecked.HasValue)
            {
                VisualStateManager.GoToState(toggle, "Indeterminate", useTransitions);
            }
            else if (toggle.IsChecked.Value)
            {
                VisualStateManager.GoToState(toggle, "Checked", useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(toggle, "Unchecked", useTransitions);
            }

            base.UpdateState(control, useTransitions);
        }
    }
}
