// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Windows.Controls
{
    internal class VisualStateBehaviorFactory : TypeHandlerFactory<VisualStateBehavior>
    {
        [ThreadStatic]
        private static VisualStateBehaviorFactory _instance;
        
        [ThreadStatic]
        private static bool _registeredKnownTypes;

        internal static VisualStateBehaviorFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new VisualStateBehaviorFactory();
                }
                return VisualStateBehaviorFactory._instance; 
            }
        }

        private VisualStateBehaviorFactory()
        {
        }

        internal static void AttachBehavior(Control control)
        {
            // If the VisualStateBehavior has already been set in some way other than the default value, 
            // then let that value win.
            if (DependencyPropertyHelper.GetValueSource(control, VisualStateBehavior.VisualStateBehaviorProperty).BaseValueSource == BaseValueSource.Default)
            {
                if (!_registeredKnownTypes)
                {
                    // When using the Toolkit version of VSM for WPF, the controls
                    // don't know about VSM and don't change states. Thus, these
                    // behaviors help bootstrap that behavior.
                    // In order to appear compatible with Silverlight, we can
                    // pre-register these behaviors. When moved into WPF, these
                    // behaviors should be unnecessary and this can go away.
                    _registeredKnownTypes = true;

                    // These are the known behaviors in the Toolkit.
                    RegisterControlBehavior(new ButtonBaseBehavior());
                    RegisterControlBehavior(new ToggleButtonBehavior());
                    RegisterControlBehavior(new ListBoxItemBehavior());
                    RegisterControlBehavior(new TextBoxBaseBehavior());
                    RegisterControlBehavior(new ProgressBarBehavior());
                }

                // No VisualStateBehavior has been specified, check the list of registered behaviors.
                VisualStateBehavior behavior = VisualStateBehaviorFactory.Instance.GetHandler(control.GetType());
                if (behavior != null)
                {
                    VisualStateBehavior.SetVisualStateBehavior(control, behavior);
                }
            }
        }

        internal static void RegisterControlBehavior(VisualStateBehavior behavior)
        {
            VisualStateBehaviorFactory.Instance.RegisterHandler(behavior);
        }

        protected override Type GetBaseType(VisualStateBehavior behavior)
        {
            return behavior.TargetType;
        }
    }
}
