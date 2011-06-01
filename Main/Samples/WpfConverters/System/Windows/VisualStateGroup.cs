// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace System.Windows
{
    /// <summary>
    ///     A group of mutually exclusive visual states.
    /// </summary>
    [ContentProperty("States")] 
    [RuntimeNameProperty("Name")]
    public class VisualStateGroup : DependencyObject 
    {
        /// <summary>
        ///     The Name of the VisualStateGroup.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        ///     VisualStates in the group.
        /// </summary>
        public IList States
        {
            get
            {
                if (_states == null)
                {
                    _states = new FreezableCollection<VisualState>();
                }

                return _states;
            }
        }

        /// <summary>
        ///     Transitions between VisualStates in the group.
        /// </summary>
        public IList Transitions
        {
            get
            {
                if (_transitions == null)
                {
                    _transitions = new FreezableCollection<VisualTransition>();
                }

                return _transitions;
            }
        }

        /// <summary>
        ///     VisualState that is currently applied.
        /// </summary>
        internal VisualState CurrentState 
        { 
            get; set; 
        }

        internal VisualState GetState(string stateName)
        {
            for (int stateIndex = 0; stateIndex < States.Count; ++stateIndex)
            {
                VisualState state = (VisualState)States[stateIndex];
                if (state.Name == stateName)
                {
                    return state;
                }
            }

            return null;
        }

        internal Collection<Storyboard> CurrentStoryboards
        {
            get
            {
                if (_currentStoryboards == null)
                {
                    _currentStoryboards = new Collection<Storyboard>();
                }

                return _currentStoryboards;
            }
        }

        internal void StartNewThenStopOld(FrameworkElement element, params Storyboard[] newStoryboards)
        {
            // Start the new Storyboards
            for (int index = 0; index < newStoryboards.Length; ++index)
            {
                if (newStoryboards[index] == null)
                {
                    continue;
                }

                newStoryboards[index].Begin(element, HandoffBehavior.SnapshotAndReplace, true);

                // Silverlight had an issue where initially, a checked CheckBox would not show the check mark
                // until the second frame. They chose to do a Seek(0) at this point, which this line
                // is supposed to mimic. It does not seem to be equivalent, though, and WPF ends up
                // with some odd animation behavior. I haven't seen the CheckBox issue on WPF, so
                // commenting this out for now.
                // newStoryboards[index].SeekAlignedToLastTick(element, TimeSpan.Zero, TimeSeekOrigin.BeginTime);
            }

            // Stop the old Storyboards
            for (int index = 0; index < CurrentStoryboards.Count; ++index)
            {
                if (CurrentStoryboards[index] == null)
                {
                    continue;
                }

                CurrentStoryboards[index].Stop(element);
            }

            // Hold on to the running Storyboards
            CurrentStoryboards.Clear();
            for (int index = 0; index < newStoryboards.Length; ++index)
            {
                CurrentStoryboards.Add(newStoryboards[index]);
            }
        }

        internal void RaiseCurrentStateChanging(FrameworkElement element, VisualState oldState, VisualState newState, Control control)
        {
            if (CurrentStateChanging != null)
            {
                CurrentStateChanging(element, new VisualStateChangedEventArgs(oldState, newState, control));
            }
        }

        internal void RaiseCurrentStateChanged(FrameworkElement element, VisualState oldState, VisualState newState, Control control)
        {
            if (CurrentStateChanged != null)
            {
                CurrentStateChanged(element, new VisualStateChangedEventArgs(oldState, newState, control));
            }
        }

        /// <summary>
        ///     Raised when transition begins
        /// </summary>
        public event EventHandler<VisualStateChangedEventArgs> CurrentStateChanged;

        /// <summary>
        ///     Raised when transition ends and new state storyboard begins.
        /// </summary>
        public event EventHandler<VisualStateChangedEventArgs> CurrentStateChanging;

        private Collection<Storyboard> _currentStoryboards;
        private FreezableCollection<VisualState> _states;
        private FreezableCollection<VisualTransition> _transitions;
    } 
}
