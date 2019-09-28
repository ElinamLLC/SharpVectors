//
// Based on codes from 
//      https://jefuri.wordpress.com/2010/09/15/gridexpander-for-wpf/
//

using System;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;

namespace WpfTestSvgControl
{
    /// <summary>
    /// Specifies different collapse modes of a GridExpander.
    /// </summary>
    public enum GridExpanderDirection
    {
        /// <summary>
        /// The GridExpander cannot be collapsed or expanded.
        /// </summary>
        None = 0,
        /// <summary>
        /// The column (or row) to the right (or below) the
        /// splitter's column, will be collapsed.
        /// </summary>
        Next = 1,
        /// <summary>
        /// The column (or row) to the left (or above) the
        /// splitter's column, will be collapsed.
        /// </summary>
        Previous = 2
    }

    /// <summary>
    /// An updated version of the standard GridExpander control that includes a centered handle
    /// which allows complete collapsing and expanding of the appropriate grid column or row.
    /// </summary>
    [TemplatePart(Name = ElementHandleName, Type = typeof(ToggleButton))]
    [TemplatePart(Name = ElementTemplateName, Type = typeof(FrameworkElement))]
    [TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Focused", GroupName = "FocusStates")]
    [TemplateVisualState(Name = "Unfocused", GroupName = "FocusStates")]
    public class GridExpander : GridSplitter
    {
        #region Private Fields

        /// <summary>
        /// An enumeration that specifies the direction the GridExpander will
        /// be collapased (Rows or Columns).
        /// </summary>
        private enum GridCollapseOrientation
        {
            Auto,
            Columns,
            Rows
        }

        private const string ElementHandleName = "ExpanderHandle";
        private const string ElementTemplateName = "TheTemplate";
        private const string ElementGridExpanderBackground = "GridExpanderBackground";

        private ToggleButton _expanderButton;
        private Rectangle _elementGridExpanderBackground;

        private RowDefinition AnimatingRow;
        private ColumnDefinition AnimatingColumn;

        private GridCollapseOrientation _gridCollapseDirection = GridCollapseOrientation.Auto;
        private GridLength _savedGridLength;
        private double _savedActualValue;
        private double _animationTimeMillis = 200;

        #endregion

        #region Dependency properties

        /// <summary>
        /// Identifies the Direction dependency property
        /// </summary>
        public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register(
            "Direction", typeof(GridExpanderDirection), typeof(GridExpander),
            new PropertyMetadata(GridExpanderDirection.Next, new PropertyChangedCallback(OnDirectionPropertyChanged)));

        /// <summary>
        /// Identifies the HandleStyle dependency property
        /// </summary>
        public static readonly DependencyProperty HandleStyleProperty = DependencyProperty.Register(
            "HandleStyle", typeof(Style), typeof(GridExpander), null);

        /// <summary>
        /// Identifies the IsAnimated dependency property
        /// </summary>
        public static readonly DependencyProperty IsAnimatedProperty = DependencyProperty.Register(
            "IsAnimated", typeof(bool), typeof(GridExpander), null);

        /// <summary>
        /// Identifies the IsCollapsed dependency property
        /// </summary>
        public static readonly DependencyProperty IsCollapsedProperty = DependencyProperty.Register(
            "IsCollapsed", typeof(bool), typeof(GridExpander),
            new PropertyMetadata(new PropertyChangedCallback(OnIsCollapsedPropertyChanged)));

        private static readonly DependencyProperty RowHeightAnimationProperty = DependencyProperty.Register(
            "RowHeightAnimation", typeof(double), typeof(GridExpander),
            new PropertyMetadata(new PropertyChangedCallback(RowHeightAnimationChanged)));

        private static readonly DependencyProperty ColWidthAnimationProperty = DependencyProperty.Register(
            "ColWidthAnimation", typeof(double), typeof(GridExpander),
            new PropertyMetadata(new PropertyChangedCallback(ColWidthAnimationChanged)));

        #endregion

        #region Public Events

        // Define Collapsed and Expanded evenets
        public event EventHandler<EventArgs> Collapsed;
        public event EventHandler<EventArgs> Expanded;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// Initializes a new instance of the GridExpander class,
        /// which inherits from System.Windows.Controls.GridExpander.
        /// </summary>
        public GridExpander()
        {
            // Set default values
            //DefaultStyleKey = typeof(GridExpander);

            VisualStateManager.GoToState(this, "Checked", false);

            //Direction = GridExpanderDirection.None;
            this.IsAnimated = true;
            this.LayoutUpdated += delegate
            {
                _gridCollapseDirection = GetCollapseDirection();
            };

            // All GridExpander visual states are handled by the parent GridSplitter class.

            this.Direction = GridExpanderDirection.Next;
        }

        static GridExpander()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridExpander),
                new FrameworkPropertyMetadata(typeof(GridExpander)));
        }

        #endregion

        #region Public and Private Properties

        /// <summary>
        /// Gets or sets a value that indicates the direction in which the row/colum
        /// will be located that is to be expanded and collapsed.
        /// </summary>
        public GridExpanderDirection Direction
        {
            get { return (GridExpanderDirection)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        ///<summary>
        /// Gets or sets the style that customizes the appearance of the vertical handle
        /// that is used to expand and collapse the GridExpander.
        /// </summary>
        public Style HandleStyle
        {
            get { return (Style)GetValue(HandleStyleProperty); }
            set { SetValue(HandleStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value that indicates if the collapse and
        /// expanding actions should be animated.
        /// </summary>
        public bool IsAnimated
        {
            get { return (bool)GetValue(IsAnimatedProperty); }
            set { SetValue(IsAnimatedProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value that indicates if the target column is
        /// currently collapsed.
        /// </summary>
        public bool IsCollapsed
        {
            get { return (bool)GetValue(IsCollapsedProperty); }
            set { SetValue(IsCollapsedProperty, value); }
        }

        private double RowHeightAnimation
        {
            get { return (double)GetValue(RowHeightAnimationProperty); }
            set { SetValue(RowHeightAnimationProperty, value); }
        }

        private double ColWidthAnimation
        {
            get { return (double)GetValue(ColWidthAnimationProperty); }
            set { SetValue(ColWidthAnimationProperty, value); }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method is called when the tempalte should be applied to the control.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _expanderButton = GetTemplateChild(ElementHandleName) as ToggleButton;
            _elementGridExpanderBackground = GetTemplateChild(ElementGridExpanderBackground) as Rectangle;

            // Wire up the Checked and Unchecked events of the VerticalGridExpanderHandle.
            if (_expanderButton != null)
            {
                _expanderButton.Checked += GridExpanderButton_Checked;
                _expanderButton.Unchecked += OnExpanderButtonUnchecked;
            }

            // Set default direction since we don't have all the components layed out yet.
            _gridCollapseDirection = GridCollapseOrientation.Auto;

            // Directely call these events so design-time view updates appropriately
            OnDirectionChanged(Direction);
            OnIsCollapsedChanged(IsCollapsed);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Handles the property change event of the IsCollapsed property.
        /// </summary>
        /// <param name="isCollapsed">The new value for the IsCollapsed property.</param>
        protected virtual void OnIsCollapsedChanged(bool isCollapsed)
        {
            _expanderButton.IsChecked = isCollapsed;
        }

        /// <summary>
        /// Handles the property change event of the Direction property.
        /// </summary>
        /// <param name="direction">The new value for the Direction property.</param>
        protected virtual void OnDirectionChanged(GridExpanderDirection direction)
        {
            if (_expanderButton == null)
            {
                // There is no expander button so don't attempt to modify it
                return;
            }

            // TODO: Use triggers for setting visibility conditionally instead of doing it here
            if (direction == GridExpanderDirection.None)
            {
                // Hide the handles if the Direction is set to None.
                _expanderButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Ensure the handle is Visible.
                _expanderButton.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Raises the Collapsed event.
        /// </summary>
        /// <param name="e">Contains event arguments.</param>
        protected virtual void OnCollapsed(EventArgs e)
        {
            this.Collapsed?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the Expanded event.
        /// </summary>
        /// <param name="e">Contains event arguments.</param>
        protected virtual void OnExpanded(EventArgs e)
        {
            this.Expanded?.Invoke(this, e);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Collapses the target ColumnDefinition or RowDefinition.
        /// </summary>
        private void Collapse()
        {
            Grid parentGrid = base.Parent as Grid;
            int splitterIndex = int.MinValue;

            if (_gridCollapseDirection == GridCollapseOrientation.Rows)
            {
                // Get the index of the row containing the splitter
                splitterIndex = (int)base.GetValue(Grid.RowProperty);

                // Determing the curent Direction
                if (this.Direction == GridExpanderDirection.Next)
                {
                    // Save the next rows Height information
                    _savedGridLength = parentGrid.RowDefinitions[splitterIndex + 1].Height;
                    _savedActualValue = parentGrid.RowDefinitions[splitterIndex + 1].ActualHeight;

                    // Collapse the next row
                    if (IsAnimated)
                        AnimateCollapse(parentGrid.RowDefinitions[splitterIndex + 1]);
                    else
                        parentGrid.RowDefinitions[splitterIndex + 1].SetValue(RowDefinition.HeightProperty, new GridLength(0));
                }
                else
                {
                    // Save the previous row's Height information
                    _savedGridLength = parentGrid.RowDefinitions[splitterIndex - 1].Height;
                    _savedActualValue = parentGrid.RowDefinitions[splitterIndex - 1].ActualHeight;

                    // Collapse the previous row
                    if (IsAnimated)
                        AnimateCollapse(parentGrid.RowDefinitions[splitterIndex - 1]);
                    else
                        parentGrid.RowDefinitions[splitterIndex - 1].SetValue(RowDefinition.HeightProperty, new GridLength(0));
                }
            }
            else
            {
                // Get the index of the column containing the splitter
                splitterIndex = (int)base.GetValue(Grid.ColumnProperty);

                // Determing the curent Direction
                if (this.Direction == GridExpanderDirection.Next)
                {
                    // Save the next column's Width information
                    _savedGridLength = parentGrid.ColumnDefinitions[splitterIndex + 1].Width;
                    _savedActualValue = parentGrid.ColumnDefinitions[splitterIndex + 1].ActualWidth;

                    // Collapse the next column
                    if (IsAnimated)
                        AnimateCollapse(parentGrid.ColumnDefinitions[splitterIndex + 1]);
                    else
                        parentGrid.ColumnDefinitions[splitterIndex + 1].SetValue(ColumnDefinition.WidthProperty, new GridLength(0));
                }
                else
                {
                    // Save the previous column's Width information
                    _savedGridLength = parentGrid.ColumnDefinitions[splitterIndex - 1].Width;
                    _savedActualValue = parentGrid.ColumnDefinitions[splitterIndex - 1].ActualWidth;

                    // Collapse the previous column
                    if (IsAnimated)
                        AnimateCollapse(parentGrid.ColumnDefinitions[splitterIndex - 1]);
                    else
                        parentGrid.ColumnDefinitions[splitterIndex - 1].SetValue(ColumnDefinition.WidthProperty, new GridLength(0));
                }
            }

        }

        /// <summary>
        /// Expands the target ColumnDefinition or RowDefinition.
        /// </summary>
        private void Expand()
        {
            Grid parentGrid = base.Parent as Grid;
            int splitterIndex = int.MinValue;

            if (_gridCollapseDirection == GridCollapseOrientation.Rows)
            {
                // Get the index of the row containing the splitter
                splitterIndex = (int)this.GetValue(Grid.RowProperty);

                // Determine the curent Direction
                if (this.Direction == GridExpanderDirection.Next)
                {
                    // Expand the next row
                    if (IsAnimated)
                        AnimateExpand(parentGrid.RowDefinitions[splitterIndex + 1]);
                    else
                        parentGrid.RowDefinitions[splitterIndex + 1].SetValue(RowDefinition.HeightProperty, _savedGridLength);
                }
                else
                {
                    // Expand the previous row
                    if (IsAnimated)
                        AnimateExpand(parentGrid.RowDefinitions[splitterIndex - 1]);
                    else
                        parentGrid.RowDefinitions[splitterIndex - 1].SetValue(RowDefinition.HeightProperty, _savedGridLength);
                }
            }
            else
            {
                // Get the index of the column containing the splitter
                splitterIndex = (int)this.GetValue(Grid.ColumnProperty);

                // Determine the curent Direction
                if (this.Direction == GridExpanderDirection.Next)
                {
                    // Expand the next column
                    if (IsAnimated)
                        AnimateExpand(parentGrid.ColumnDefinitions[splitterIndex + 1]);
                    else
                        parentGrid.ColumnDefinitions[splitterIndex + 1].SetValue(ColumnDefinition.WidthProperty, _savedGridLength);
                }
                else
                {
                    // Expand the previous column
                    if (IsAnimated)
                        AnimateExpand(parentGrid.ColumnDefinitions[splitterIndex - 1]);
                    else
                        parentGrid.ColumnDefinitions[splitterIndex - 1].SetValue(ColumnDefinition.WidthProperty, _savedGridLength);
                }
            }
        }

        /// <summary>
        /// Determine the collapse direction based on the horizontal and vertical alignments
        /// </summary>
        private GridCollapseOrientation GetCollapseDirection()
        {
            if (base.HorizontalAlignment != HorizontalAlignment.Stretch)
            {
                return GridCollapseOrientation.Columns;
            }

            if ((base.VerticalAlignment == VerticalAlignment.Stretch) && (base.ActualWidth <= base.ActualHeight))
            {
                return GridCollapseOrientation.Columns;
            }

            return GridCollapseOrientation.Rows;
        }

        /// <summary>
        /// Handles the Checked event of either the Vertical or Horizontal
        /// GridExpanderHandle ToggleButton.
        /// </summary>
        /// <param name="sender">An instance of the ToggleButton that fired the event.</param>
        /// <param name="e">Contains event arguments for the routed event that fired.</param>
        private void GridExpanderButton_Checked(object sender, RoutedEventArgs e)
        {
            if (IsCollapsed != true)
            {
                // In our case, Checked = Collapsed.  Which means we want everything
                // ready to be expanded.
                Collapse();

                IsCollapsed = true;

                // Deactivate the background so the splitter can not be dragged.
                _elementGridExpanderBackground.IsHitTestVisible = false;
                //_elementGridExpanderBackground.Opacity = 0.5;

                // Raise the Collapsed event.
                OnCollapsed(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Handles the Unchecked event of either the Vertical or Horizontal
        /// GridExpanderHandle ToggleButton.
        /// </summary>
        /// <param name="sender">An instance of the ToggleButton that fired the event.</param>
        /// <param name="e">Contains event arguments for the routed event that fired.</param>
        private void OnExpanderButtonUnchecked(object sender, RoutedEventArgs e)
        {
            if (IsCollapsed != false)
            {
                // In our case, Unchecked = Expanded.  Which means we want everything
                // ready to be collapsed.
                Expand();

                IsCollapsed = false;

                // Activate the background so the splitter can be dragged again.
                _elementGridExpanderBackground.IsHitTestVisible = true;
                //_elementGridExpanderBackground.Opacity = 1;

                // Raise the Expanded event.
                OnExpanded(EventArgs.Empty);
            }
        }

        /// <summary>
        /// The IsCollapsed property porperty changed handler.
        /// </summary>
        /// <param name="d">GridExpander that changed IsCollapsed.</param>
        /// <param name="e">An instance of DependencyPropertyChangedEventArgs.</param>
        private static void OnIsCollapsedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GridExpander s = d as GridExpander;

            bool value = (bool)e.NewValue;
            s.OnIsCollapsedChanged(value);
        }

        /// <summary>
        /// The DirectionProperty property changed handler.
        /// </summary>
        /// <param name="d">GridExpander that changed IsCollapsed.</param>
        /// <param name="e">An instance of DependencyPropertyChangedEventArgs.</param>
        private static void OnDirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GridExpander s = d as GridExpander;

            GridExpanderDirection value = (GridExpanderDirection)e.NewValue;
            s.OnDirectionChanged(value);
        }

        private static void RowHeightAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as GridExpander).AnimatingRow.Height = new GridLength((double)e.NewValue);
        }

        private static void ColWidthAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as GridExpander).AnimatingColumn.Width = new GridLength((double)e.NewValue);
        }

        /// <summary>
        /// Uses DoubleAnimation and a StoryBoard to animated the collapsing
        /// of the specificed ColumnDefinition or RowDefinition.
        /// </summary>
        /// <param name="definition">The RowDefinition or ColumnDefintition that will be collapsed.</param>
        private void AnimateCollapse(object definition)
        {
            double currentValue;

            // Setup the animation and StoryBoard
            DoubleAnimation gridLengthAnimation = new DoubleAnimation()
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(_animationTimeMillis))
            };
            Storyboard sb = new Storyboard();

            // Add the animation to the StoryBoard
            sb.Children.Add(gridLengthAnimation);

            if (_gridCollapseDirection == GridCollapseOrientation.Rows)
            {
                // Specify the target RowDefinition and property (Height) that will be altered by the animation.
                this.AnimatingRow = (RowDefinition)definition;
                Storyboard.SetTarget(gridLengthAnimation, this);
                Storyboard.SetTargetProperty(gridLengthAnimation, new PropertyPath("RowHeightAnimation"));

                currentValue = AnimatingRow.ActualHeight;
            }
            else
            {
                // Specify the target ColumnDefinition and property (Width) that will be altered by the animation.
                this.AnimatingColumn = (ColumnDefinition)definition;
                Storyboard.SetTarget(gridLengthAnimation, this);
                Storyboard.SetTargetProperty(gridLengthAnimation, new PropertyPath("ColWidthAnimation"));

                currentValue = AnimatingColumn.ActualWidth;
            }

            gridLengthAnimation.From = currentValue;
            gridLengthAnimation.To = 0;

            // Start the StoryBoard.
            sb.Begin();
        }

        /// <summary>
        /// Uses DoubleAnimation and a StoryBoard to animate the expansion
        /// of the specificed ColumnDefinition or RowDefinition.
        /// </summary>
        /// <param name="definition">The RowDefinition or ColumnDefintition that will be expanded.</param>
        private void AnimateExpand(object definition)
        {
            double currentValue;

            // Setup the animation and StoryBoard
            DoubleAnimation gridLengthAnimation = new DoubleAnimation()
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(_animationTimeMillis))
            };
            Storyboard sb = new Storyboard();

            // Add the animation to the StoryBoard
            sb.Children.Add(gridLengthAnimation);

            if (_gridCollapseDirection == GridCollapseOrientation.Rows)
            {
                // Specify the target RowDefinition and property (Height) that will be altered by the animation.
                this.AnimatingRow = (RowDefinition)definition;
                Storyboard.SetTarget(gridLengthAnimation, this);
                Storyboard.SetTargetProperty(gridLengthAnimation, new PropertyPath("RowHeightAnimation"));

                currentValue = AnimatingRow.ActualHeight;
            }
            else
            {
                // Specify the target ColumnDefinition and property (Width) that will be altered by the animation.
                this.AnimatingColumn = (ColumnDefinition)definition;
                Storyboard.SetTarget(gridLengthAnimation, this);
                Storyboard.SetTargetProperty(gridLengthAnimation, new PropertyPath("ColWidthAnimation"));

                currentValue = AnimatingColumn.ActualWidth;
            }
            gridLengthAnimation.From = currentValue;
            gridLengthAnimation.To = _savedActualValue;

            // Start the StoryBoard.
            sb.Begin();
        }

        #endregion
    }
}
