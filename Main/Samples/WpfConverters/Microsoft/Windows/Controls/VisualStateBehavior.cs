// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Windows.Controls
{
    public abstract class VisualStateBehavior
    {
        #region VisualStateBehavior Attached Property

        /// <summary>
        ///     Returns the value of the VisualStateBehavior attached property.
        /// </summary>
        public static VisualStateBehavior GetVisualStateBehavior(DependencyObject obj)
        {
            return (VisualStateBehavior)obj.GetValue(VisualStateBehaviorProperty);
        }

        /// <summary>
        ///     Sets the value of the VisualStateBehavior attached property.
        ///     Setting the value will attach the behavior to the instance of the control.
        /// </summary>
        public static void SetVisualStateBehavior(DependencyObject obj, VisualStateBehavior value)
        {
            obj.SetValue(VisualStateBehaviorProperty, value);
        }

        /// <summary>
        ///     The attached DependencyProperty for VisualStateBehavior.
        /// </summary>
        public static readonly DependencyProperty VisualStateBehaviorProperty =
            DependencyProperty.RegisterAttached("VisualStateBehavior", typeof(VisualStateBehavior), typeof(VisualStateBehavior), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnVisualStateBehaviorChanged)));

        private static void OnVisualStateBehaviorChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Control control = sender as Control;
            if (control != null)
            {
                VisualStateBehavior newBehavior = (VisualStateBehavior)e.NewValue;
                if (newBehavior != null)
                {
                    newBehavior.Attach(control);
                }
            }
        }

        /// <summary>
        ///     Checks that a VisualStateBehavior isn't already attached. If not,
        ///     then attaches to the Control using OnAttach.
        /// </summary>
        private void Attach(Control control)
        {
            if (GetIsVisualStateBehaviorAttached(control))
            {
                // TODO: Globalize this message.
                throw new InvalidOperationException("VisualStateBehavior is already attached.");
            }
            else
            {
                SetIsVisualStateBehaviorAttached(control, true);
            }

            OnAttach(control);
            control.Unloaded += DetachHandler;
            control.Loaded -= AttachHandler;
        }

                /// <summary>
        ///     Checks that a VisualStateBehavior isn't already attached. If not,
        ///     then attaches to the Control using OnAttach.
        /// </summary>
        private void Detach(Control control)
        {
            if (!GetIsVisualStateBehaviorAttached(control))
            {
                // TODO: Globalize this message.
                throw new InvalidOperationException("VisualStateBehavior is not attached.");
            }
            else
            {
                SetIsVisualStateBehaviorAttached(control, false);
            }

            OnDetach(control);
            control.Loaded += AttachHandler;
            control.Unloaded -= DetachHandler;
        }

        /// <summary>
        ///     Used to determine if a VisualStateBehavior is already attached.
        /// </summary>
        private static bool GetIsVisualStateBehaviorAttached(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsVisualStateBehaviorAttachedProperty);
        }

        /// <summary>
        ///     Sets whether a VisualStateBehavior is already attached.
        /// </summary>
        private static void SetIsVisualStateBehaviorAttached(DependencyObject obj, bool value)
        {
            obj.SetValue(IsVisualStateBehaviorAttachedProperty, value);
        }

        /// <summary>
        ///     Attached property used to flag whether a VisualStateBehavior is attached.
        /// </summary>
        private static readonly DependencyProperty IsVisualStateBehaviorAttachedProperty =
            DependencyProperty.RegisterAttached("IsVisualStateBehaviorAttached", typeof(bool), typeof(VisualStateBehavior), new FrameworkPropertyMetadata(false));

        #endregion

        #region Behavior Registration

        /// <summary>
        ///     Applies the specified behavior to all instances of the target
        ///     control type.
        /// </summary>
        /// <param name="behavior">The behavior being registered.</param>
        public static void RegisterBehavior(VisualStateBehavior behavior)
        {
            VisualStateBehaviorFactory.RegisterControlBehavior(behavior);
        }

        #endregion

        #region Abstract Members

        /// <summary>
        ///     Specifies the Type of the Control that this behavior targets.
        /// </summary>
        protected abstract internal Type TargetType
        {
            get;
        }

        /// <summary>
        ///     Attach to the appropriate events on the instance of the
        ///     control in order to update the state correctly.
        /// </summary>
        /// <param name="control">An instance of the control.</param>
        protected abstract void OnAttach(Control control);

        /// <summary>
        ///     Detach to the appropriate events on the instance of the
        ///     control in order to not leak memory.
        /// </summary>
        /// <param name="control">An instance of the control.</param>
        protected abstract void OnDetach(Control control);

        protected abstract void UpdateStateHandler(Object o, EventArgs e);


        /// <summary>
        /// This handler will be fired from the Unloaded event and causes the control to detach behaviors.
        /// </summary>
        /// <param name="sender">Control</param>
        /// <param name="e">Unused</param>
        private void  DetachHandler(object sender, RoutedEventArgs e)
        {
            Control cont = sender as Control;
            if (cont == null)
            {
                throw new InvalidOperationException("This Handler should only be on a control.");
            }

            Detach(cont);

        }

        /// <summary>
        /// This handler will be fired from the Loaded event and causes the control to re-attach behaviors.
        /// </summary>
        /// <param name="sender">Control</param>
        /// <param name="e">Unused</param>
        private void  AttachHandler(object sender, RoutedEventArgs e)
        {
            Control cont = sender as Control;
            if (cont == null)
            {
                throw new InvalidOperationException("This Handler should only be on a control.");
            }

            Attach(cont);
        }

        /// <summary>
        ///     Called to update the control's visual state.
        /// </summary>
        /// <param name="control">The instance of the control being updated.</param>
        /// <param name="useTransitions">Whether to use transitions or not.</param>
        protected abstract void UpdateState(Control control, bool useTransitions);

        #endregion

        #region Helpers

        /// <summary>
        ///     Attaches an event handler to be called when a property changes.
        /// </summary>
        /// <param name="dp">When this DependencyProperty changes on the instance, the handler will be called.</param>
        /// <param name="targetType">The target type of the property or the instance if it is an attached property.</param>
        /// <param name="instance">The instance of the object.</param>
        /// <param name="handler">The handler to call.</param>
        /// <returns>true if the handler was attached, false otherwise.</returns>
        protected static bool AddValueChanged(DependencyProperty dp, Type targetType, object instance, EventHandler handler)
        {
            if (dp == null)
            {
                throw new ArgumentNullException("dp");
            }

            if (targetType == null)
            {
                throw new ArgumentNullException("targetType");
            }

            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            var propertyDescriptor = DependencyPropertyDescriptor.FromProperty(dp, targetType);
            if (propertyDescriptor != null)
            {
                propertyDescriptor.AddValueChanged(instance, handler);
                return true;
            }

            return false;
        }

        protected static bool RemoveValueChanged(DependencyProperty dp, Type targetType, object instance, EventHandler handler)
        {
            if (dp == null)
            {
                throw new ArgumentNullException("dp");
            }

            if (targetType == null)
            {
                throw new ArgumentNullException("targetType");
            }

            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            var propertyDescriptor = DependencyPropertyDescriptor.FromProperty(dp, targetType);
            if (propertyDescriptor != null)
            {
                propertyDescriptor.RemoveValueChanged(instance, handler);
                return true;
            }

            return false;
        }

        #endregion
    }
}
