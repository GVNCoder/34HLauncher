using System;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Launcher.Styles.Controls
{
    public class ScrollingAnimationBehavior : Behavior<ScrollViewer>
    {
        public ScrollViewer _sv => AssociatedObject;
        public ScrollingAnimationHelper _svh;

        protected override void OnAttached()
        {
            // кріпимо наш обробник події
            //AssociatedObject.ScrollChanged += _scrollChangedHandler;
            AssociatedObject.PreviewMouseWheel += _mouseWheelHandler;

            _svh = new ScrollingAnimationHelper(AssociatedObject);

            base.OnAttached();
        }

        private void _mouseWheelHandler(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            double mouseWheelChange = (double)e.Delta * -1;
            double newVOffset = GetVerticalOffset(AssociatedObject) + (mouseWheelChange / 3);

            AssociatedObject.BeginAnimation(VerticalOffsetProperty, null);

            var animationObj = new DoubleAnimation
            {
                From = AssociatedObject.VerticalOffset,
                To = newVOffset,
                Duration = new Duration(TimeSpan.Parse("0:0:.2"))
            };

            var storyboard = new Storyboard()
            {
                Children = { animationObj }
            };

            Storyboard.SetTarget(animationObj, AssociatedObject);
            Storyboard.SetTargetProperty(animationObj, new PropertyPath(ScrollingAnimationBehavior.VerticalOffsetProperty));

            storyboard.Begin();
        }

        private void _scrollChangedHandler(object sender, ScrollChangedEventArgs e)
        {
            var a = (/*e.VerticalOffset + */e.VerticalChange);
            var b = GetVerticalOffset(AssociatedObject) + e.VerticalChange;

            var animationObj = new DoubleAnimation()
            {
                From = 0,
                To = a,
                Duration = new Duration(TimeSpan.Parse("0:0:.2"))
            };

            var storyboard = new Storyboard()
            {
                Children = { animationObj }
            };

            Storyboard.SetTarget(animationObj, AssociatedObject);
            Storyboard.SetTargetProperty(animationObj, new PropertyPath(ScrollingAnimationBehavior.VerticalOffsetProperty));

            storyboard.Begin();

            e.Handled = true;
        }

        public static DependencyProperty VerticalOffsetProperty =
            DependencyProperty.RegisterAttached("VerticalOffset",
                typeof(double),
                typeof(ScrollingAnimationBehavior),
                new UIPropertyMetadata(0.0, OnVerticalOffsetChanged));

        public static void SetVerticalOffset(FrameworkElement target, double value)
        {
            target.SetValue(VerticalOffsetProperty, value);
        }

        public static double GetVerticalOffset(FrameworkElement target)
        {
            return (double)target.GetValue(VerticalOffsetProperty);
        }

        private static void OnVerticalOffsetChanged(Object target, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewer scrollViewer = target as ScrollViewer;

            if (scrollViewer != null)
            {
                scrollViewer.ScrollToVerticalOffset((double)e.NewValue);
            }
        }
    }

    public class ScrollingAnimationBehaviorDataGrid : Behavior<DataGrid>
    {
        public ScrollViewer associatedObject;

        protected override void OnAttached()
        {
            // кріпимо наш обробник події
            //AssociatedObject.ScrollChanged += _scrollChangedHandler;
            AssociatedObject.PreviewMouseWheel += _mouseWheelHandler;

            associatedObject = GetScrollViewer(AssociatedObject);

            base.OnAttached();
        }

        private void _mouseWheelHandler(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            double mouseWheelChange = (double)e.Delta * -1;
            double newVOffset = GetVerticalOffset(associatedObject) + (mouseWheelChange / 3);

            AssociatedObject.BeginAnimation(VerticalOffsetProperty, null);

            var animationObj = new DoubleAnimation
            {
                From = associatedObject.VerticalOffset,
                To = newVOffset,
                Duration = new Duration(TimeSpan.Parse("0:0:.2"))
            };

            var storyboard = new Storyboard()
            {
                Children = { animationObj }
            };

            Storyboard.SetTarget(animationObj, associatedObject);
            Storyboard.SetTargetProperty(animationObj, new PropertyPath(ScrollingAnimationBehaviorDataGrid.VerticalOffsetProperty));

            storyboard.Begin();
        }

        private void _scrollChangedHandler(object sender, ScrollChangedEventArgs e)
        {
            var a = (/*e.VerticalOffset + */e.VerticalChange);
            var b = GetVerticalOffset(associatedObject) + e.VerticalChange;

            var animationObj = new DoubleAnimation()
            {
                From = 0,
                To = a,
                Duration = new Duration(TimeSpan.Parse("0:0:.2"))
            };

            var storyboard = new Storyboard()
            {
                Children = { animationObj }
            };

            Storyboard.SetTarget(animationObj, associatedObject);
            Storyboard.SetTargetProperty(animationObj, new PropertyPath(ScrollingAnimationBehaviorDataGrid.VerticalOffsetProperty));

            storyboard.Begin();

            e.Handled = true;
        }

        public static DependencyProperty VerticalOffsetProperty =
            DependencyProperty.RegisterAttached("VerticalOffset",
                typeof(double),
                typeof(ScrollingAnimationBehaviorDataGrid),
                new UIPropertyMetadata(0.0, OnVerticalOffsetChanged));

        public static void SetVerticalOffset(FrameworkElement target, double value)
        {
            target.SetValue(VerticalOffsetProperty, value);
        }

        public static double GetVerticalOffset(FrameworkElement target)
        {
            return (double)target.GetValue(VerticalOffsetProperty);
        }

        private static void OnVerticalOffsetChanged(Object target, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewer scrollViewer = target as ScrollViewer;

            if (scrollViewer != null)
            {
                scrollViewer.ScrollToVerticalOffset((double)e.NewValue);
            }
        }

        public static ScrollViewer GetScrollViewer(UIElement element)
        {
            if (element == null) return null;

            ScrollViewer retour = null;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element) && retour == null; i++)
            {
                if (VisualTreeHelper.GetChild(element, i) is ScrollViewer)
                {
                    retour = (ScrollViewer)(VisualTreeHelper.GetChild(element, i));
                }
                else
                {
                    retour = GetScrollViewer(VisualTreeHelper.GetChild(element, i) as UIElement);
                }
            }
            return retour;
        }
    }

    public class ScrollingAnimationHelper : DependencyObject
    {
        public ScrollViewer _sv;

        public ScrollingAnimationHelper(ScrollViewer sv)
        {
            _sv = sv;
        }

        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VerticalOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register("VerticalOffset", typeof(double), typeof(ScrollingAnimationHelper), new PropertyMetadata(PropertyChangedCallback));

        private static void PropertyChangedCallback(Object d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as ScrollingAnimationHelper;
            obj._sv.ScrollToVerticalOffset((double)e.NewValue);
        }
    }

    public static class ScrollAnimationBehavior
    {
        #region Private ScrollViewer for ListBox

        private static ScrollViewer _listBoxScroller = new ScrollViewer();

        #endregion

        #region VerticalOffset Property

        public static DependencyProperty VerticalOffsetProperty =
            DependencyProperty.RegisterAttached("VerticalOffset",
                                                typeof(double),
                                                typeof(ScrollAnimationBehavior),
                                                new UIPropertyMetadata(0.0, OnVerticalOffsetChanged));

        public static void SetVerticalOffset(FrameworkElement target, double value)
        {
            target.SetValue(VerticalOffsetProperty, value);
        }

        public static double GetVerticalOffset(FrameworkElement target)
        {
            return (double)target.GetValue(VerticalOffsetProperty);
        }

        #endregion

        #region TimeDuration Property

        public static DependencyProperty TimeDurationProperty =
            DependencyProperty.RegisterAttached("TimeDuration",
                                                typeof(TimeSpan),
                                                typeof(ScrollAnimationBehavior),
                                                new PropertyMetadata(new TimeSpan(0, 0, 0, 0, 0)));

        public static void SetTimeDuration(FrameworkElement target, TimeSpan value)
        {
            target.SetValue(TimeDurationProperty, value);
        }

        public static TimeSpan GetTimeDuration(FrameworkElement target)
        {
            return (TimeSpan)target.GetValue(TimeDurationProperty);
        }

        #endregion

        #region PointsToScroll Property

        public static DependencyProperty PointsToScrollProperty =
            DependencyProperty.RegisterAttached("PointsToScroll",
                                                typeof(double),
                                                typeof(ScrollAnimationBehavior),
                                                new PropertyMetadata(0.0));

        public static void SetPointsToScroll(FrameworkElement target, double value)
        {
            target.SetValue(PointsToScrollProperty, value);
        }

        public static double GetPointsToScroll(FrameworkElement target)
        {
            return (double)target.GetValue(PointsToScrollProperty);
        }

        #endregion

        #region OnVerticalOffset Changed

        private static void OnVerticalOffsetChanged(Object target, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewer scrollViewer = target as ScrollViewer;

            if (scrollViewer != null)
            {
                scrollViewer.ScrollToVerticalOffset((double)e.NewValue);
            }
        }

        #endregion

        #region IsEnabled Property

        public static DependencyProperty IsEnabledProperty =
                                                DependencyProperty.RegisterAttached("IsEnabled",
                                                typeof(bool),
                                                typeof(ScrollAnimationBehavior),
                                                new UIPropertyMetadata(false, OnIsEnabledChanged));

        public static void SetIsEnabled(FrameworkElement target, bool value)
        {
            target.SetValue(IsEnabledProperty, value);
        }

        public static bool GetIsEnabled(FrameworkElement target)
        {
            return (bool)target.GetValue(IsEnabledProperty);
        }

        #endregion

        #region OnIsEnabledChanged Changed

        private static void OnIsEnabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var target = sender;

            if (target != null && target is ScrollViewer)
            {
                ScrollViewer scroller = target as ScrollViewer;
                scroller.Loaded += new RoutedEventHandler(scrollerLoaded);
            }

            if (target != null && target is ListBox)
            {
                ListBox listbox = target as ListBox;
                listbox.Loaded += new RoutedEventHandler(listboxLoaded);
            }
        }

        #endregion

        #region AnimateScroll Helper

        private static void AnimateScroll(ScrollViewer scrollViewer, double ToValue)
        {
            DoubleAnimation verticalAnimation = new DoubleAnimation();

            verticalAnimation.From = scrollViewer.VerticalOffset;
            verticalAnimation.To = ToValue;
            verticalAnimation.Duration = new Duration(GetTimeDuration(scrollViewer));

            Storyboard storyboard = new Storyboard();

            storyboard.Children.Add(verticalAnimation);
            Storyboard.SetTarget(verticalAnimation, scrollViewer);
            Storyboard.SetTargetProperty(verticalAnimation, new PropertyPath(ScrollAnimationBehavior.VerticalOffsetProperty));
            storyboard.Begin();
        }

        #endregion

        #region NormalizeScrollPos Helper

        private static double NormalizeScrollPos(ScrollViewer scroll, double scrollChange, Orientation o)
        {
            double returnValue = scrollChange;

            if (scrollChange < 0)
            {
                returnValue = 0;
            }

            if (o == Orientation.Vertical && scrollChange > scroll.ScrollableHeight)
            {
                returnValue = scroll.ScrollableHeight;
            }
            else if (o == Orientation.Horizontal && scrollChange > scroll.ScrollableWidth)
            {
                returnValue = scroll.ScrollableWidth;
            }

            return returnValue;
        }

        #endregion

        #region UpdateScrollPosition Helper

        private static void UpdateScrollPosition(object sender)
        {
            ListBox listbox = sender as ListBox;

            if (listbox != null)
            {
                double scrollTo = 0;

                for (int i = 0; i < (listbox.SelectedIndex); i++)
                {
                    ListBoxItem tempItem = listbox.ItemContainerGenerator.ContainerFromItem(listbox.Items[i]) as ListBoxItem;

                    if (tempItem != null)
                    {
                        scrollTo += tempItem.ActualHeight;
                    }
                }

                AnimateScroll(_listBoxScroller, scrollTo);
            }
        }

        #endregion

        #region SetEventHandlersForScrollViewer Helper

        private static void SetEventHandlersForScrollViewer(ScrollViewer scroller)
        {
            scroller.PreviewMouseWheel += new MouseWheelEventHandler(ScrollViewerPreviewMouseWheel);
            scroller.PreviewKeyDown += new KeyEventHandler(ScrollViewerPreviewKeyDown);
        }

        #endregion

        #region scrollerLoaded Event Handler

        private static void scrollerLoaded(object sender, RoutedEventArgs e)
        {
            ScrollViewer scroller = sender as ScrollViewer;

            SetEventHandlersForScrollViewer(scroller);
        }

        #endregion

        #region listboxLoaded Event Handler

        private static void listboxLoaded(object sender, RoutedEventArgs e)
        {
            ListBox listbox = sender as ListBox;

            _listBoxScroller = FindVisualChild<ScrollViewer>(listbox);
            SetEventHandlersForScrollViewer(_listBoxScroller);

            SetTimeDuration(_listBoxScroller, new TimeSpan(0, 0, 0, 0, 200));
            SetPointsToScroll(_listBoxScroller, 16.0);

            listbox.SelectionChanged += new SelectionChangedEventHandler(ListBoxSelectionChanged);
            listbox.Loaded += new RoutedEventHandler(ListBoxLoaded);
            listbox.LayoutUpdated += new EventHandler(ListBoxLayoutUpdated);
        }

        #endregion

        #region ScrollViewerPreviewMouseWheel Event Handler

        private static void ScrollViewerPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            double mouseWheelChange = (double)e.Delta;
            ScrollViewer scroller = (ScrollViewer)sender;
            double newVOffset = GetVerticalOffset(scroller) - (mouseWheelChange / 3);

            if (newVOffset < 0)
            {
                AnimateScroll(scroller, 0);
            }
            else if (newVOffset > scroller.ScrollableHeight)
            {
                AnimateScroll(scroller, scroller.ScrollableHeight);
            }
            else
            {
                AnimateScroll(scroller, newVOffset);
            }

            e.Handled = true;
        }

        #endregion

        #region ScrollViewerPreviewKeyDown Handler

        private static void ScrollViewerPreviewKeyDown(object sender, KeyEventArgs e)
        {
            ScrollViewer scroller = (ScrollViewer)sender;

            Key keyPressed = e.Key;
            double newVerticalPos = GetVerticalOffset(scroller);
            bool isKeyHandled = false;

            if (keyPressed == Key.Down)
            {
                newVerticalPos = NormalizeScrollPos(scroller, (newVerticalPos + GetPointsToScroll(scroller)), Orientation.Vertical);
                isKeyHandled = true;
            }
            else if (keyPressed == Key.PageDown)
            {
                newVerticalPos = NormalizeScrollPos(scroller, (newVerticalPos + scroller.ViewportHeight), Orientation.Vertical);
                isKeyHandled = true;
            }
            else if (keyPressed == Key.Up)
            {
                newVerticalPos = NormalizeScrollPos(scroller, (newVerticalPos - GetPointsToScroll(scroller)), Orientation.Vertical);
                isKeyHandled = true;
            }
            else if (keyPressed == Key.PageUp)
            {
                newVerticalPos = NormalizeScrollPos(scroller, (newVerticalPos - scroller.ViewportHeight), Orientation.Vertical);
                isKeyHandled = true;
            }

            if (newVerticalPos != GetVerticalOffset(scroller))
            {
                AnimateScroll(scroller, newVerticalPos);
            }

            e.Handled = isKeyHandled;
        }

        #endregion

        #region ListBox Event Handlers

        private static void ListBoxLayoutUpdated(object sender, EventArgs e)
        {
            UpdateScrollPosition(sender);
        }

        private static void ListBoxLoaded(object sender, RoutedEventArgs e)
        {
            UpdateScrollPosition(sender);
        }

        private static void ListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateScrollPosition(sender);
        }

        #endregion

        public static childItem FindVisualChild<childItem>(DependencyObject obj)
            where childItem : DependencyObject
        {
            // Iterate through all immediate children
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                if (child != null && child is childItem)
                    return (childItem)child;

                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);

                    if (childOfChild != null)
                        return childOfChild;
                }
            }

            return null;
        }
    }

    public class KineticBehaviour
    {
          #region Friction
 
          /// <summary>
          /// Friction Attached Dependency Property
          /// </summary>
          public static readonly DependencyProperty FrictionProperty =
              DependencyProperty.RegisterAttached("Friction",
              typeof(double), typeof(KineticBehaviour),
                  new FrameworkPropertyMetadata((double)0.95));

          /// <summary>
          /// Gets the Friction property.  This dependency property
          /// indicates ....
          /// </summary>
          public static double GetFriction(DependencyObject d)
         {
              return (double) d.GetValue(FrictionProperty);
          }

          /// <summary>
          /// Sets the Friction property.
          /// </summary>
          public static void SetFriction(DependencyObject d, double value)
          {
              d.SetValue(FrictionProperty, value);
          }

          #endregion

          #region ScrollStartPoint

          /// <summary>
          /// ScrollStartPoint Attached Dependency Property
          /// </summary>
          private static readonly DependencyProperty ScrollStartPointProperty =
              DependencyProperty.RegisterAttached("ScrollStartPoint",
              typeof(Point), typeof(KineticBehaviour),
                  new FrameworkPropertyMetadata((Point)new Point()));

          /// <summary>
          /// Gets the ScrollStartPoint property.
          /// </summary>
          private static Point GetScrollStartPoint(DependencyObject d)
          {
              return (Point) d.GetValue(ScrollStartPointProperty);
          }

          /// <summary>
          /// Sets the ScrollStartPoint property.
          /// </summary>
          private static void SetScrollStartPoint(DependencyObject d,
              Point value)
          {
              d.SetValue(ScrollStartPointProperty, value);
          }

          #endregion

          #region ScrollStartOffset

          /// <summary>
          /// ScrollStartOffset Attached Dependency Property
         /// </summary>
          private static readonly DependencyProperty ScrollStartOffsetProperty =
              DependencyProperty.RegisterAttached("ScrollStartOffset",
              typeof(Point), typeof(KineticBehaviour),
                  new FrameworkPropertyMetadata((Point)new Point()));

         /// <summary>
          /// Gets the ScrollStartOffset property.
          /// </summary>
          private static Point GetScrollStartOffset(DependencyObject d)
         {
             return (Point) d.GetValue(ScrollStartOffsetProperty);
          }

          /// <summary>
         /// Sets the ScrollStartOffset property.
         /// </summary>
          private static void SetScrollStartOffset(DependencyObject d,
              Point value)
         {
              d.SetValue(ScrollStartOffsetProperty, value);
          }

         #endregion

          #region InertiaProcessor

          /// <summary>
          /// InertiaProcessor Attached Dependency Property
          /// </summary>
          private static readonly DependencyProperty InertiaProcessorProperty =
              DependencyProperty.RegisterAttached("InertiaProcessor",
              typeof(InertiaHandler), typeof(KineticBehaviour),
                  new FrameworkPropertyMetadata((InertiaHandler)null));

          /// <summary>
          /// Gets the InertiaProcessor property.
          /// </summary>
          private static InertiaHandler GetInertiaProcessor(DependencyObject d)
          {
              return (InertiaHandler) d.GetValue(InertiaProcessorProperty);
          }

          /// <summary>
          /// Sets the InertiaProcessor property.
          /// </summary>
          private static void SetInertiaProcessor(DependencyObject d,
              InertiaHandler value)
          {
              d.SetValue(InertiaProcessorProperty, value);
          }

          #endregion

          #region HandleKineticScrolling

          /// <summary>
          /// HandleKineticScrolling Attached Dependency Property
          /// </summary>
          public static readonly DependencyProperty
              HandleKineticScrollingProperty =
             DependencyProperty.RegisterAttached("HandleKineticScrolling",
              typeof(bool), typeof(KineticBehaviour),
                  new FrameworkPropertyMetadata((bool)false,
                      new PropertyChangedCallback(
                          OnHandleKineticScrollingChanged)));

         /// <summary>
          /// Gets the HandleKineticScrolling property.
          /// </summary>
         public static bool GetHandleKineticScrolling(DependencyObject d)
          {
              return (bool) d.GetValue(HandleKineticScrollingProperty);
          }

          /// <summary>
          /// Sets the HandleKineticScrolling property.
          /// </summary>
          public static void SetHandleKineticScrolling(DependencyObject d,
              bool value)
          {
              d.SetValue(HandleKineticScrollingProperty, value);
          }

          /// <summary>
          /// Handles changes to the HandleKineticScrolling property.
          /// </summary>
          private static void OnHandleKineticScrollingChanged(Object d,
              DependencyPropertyChangedEventArgs e)
          {
              ScrollViewer scoller = d as ScrollViewer;
              if ((bool) e.NewValue)
              {
                  scoller.PreviewMouseDown += OnPreviewMouseDown;
                  scoller.PreviewMouseMove += OnPreviewMouseMove;
                  scoller.PreviewMouseUp += OnPreviewMouseUp;
                  SetInertiaProcessor(scoller, new InertiaHandler(scoller));
             }
              else
              {
                  scoller.PreviewMouseDown -= OnPreviewMouseDown;
                 scoller.PreviewMouseMove -= OnPreviewMouseMove;
                  scoller.PreviewMouseUp -= OnPreviewMouseUp;
                  var inertia = GetInertiaProcessor(scoller);
                  if (inertia != null)
                      inertia.Dispose();
              }

          }

          #endregion

          #region Mouse Events
          private static void OnPreviewMouseDown(object sender,
             MouseButtonEventArgs e)
         {
              var scrollViewer = (ScrollViewer)sender;
              if (scrollViewer.IsMouseOver)
              {
                  // Save starting point, used later when
                  //determining how much to scroll.
                  SetScrollStartPoint(scrollViewer,
                      e.GetPosition(scrollViewer));
                  SetScrollStartOffset(scrollViewer,
                      new Point(scrollViewer.HorizontalOffset,
                          scrollViewer.VerticalOffset));
                  scrollViewer.CaptureMouse();
              }
          }


          private static void OnPreviewMouseMove(object sender, MouseEventArgs e)
          {
              var scrollViewer = (ScrollViewer)sender;
              if (scrollViewer.IsMouseCaptured)
              {
                  Point currentPoint = e.GetPosition(scrollViewer);

                  var scrollStartPoint = GetScrollStartPoint(scrollViewer);
                  // Determine the new amount to scroll.
                 Point delta = new Point(scrollStartPoint.X -
                     currentPoint.X, scrollStartPoint.Y - currentPoint.Y);

                  var scrollStartOffset = GetScrollStartOffset(scrollViewer);
                 Point scrollTarget = new Point(scrollStartOffset.X +
                      delta.X, scrollStartOffset.Y + delta.Y);

                 var inertiaProcessor = GetInertiaProcessor(scrollViewer);
                  if (inertiaProcessor != null)
                      inertiaProcessor.ScrollTarget = scrollTarget;

                  // Scroll to the new position.
                  scrollViewer.ScrollToHorizontalOffset(scrollTarget.X);
                  scrollViewer.ScrollToVerticalOffset(scrollTarget.Y);
              }
          }

          private static void OnPreviewMouseUp(object sender,
              MouseButtonEventArgs e)
          {
              var scrollViewer = (ScrollViewer)sender;
              if (scrollViewer.IsMouseCaptured)
              {
                  scrollViewer.ReleaseMouseCapture();
              }
          }
         #endregion

          #region Inertia Stuff

          /// <summary>
          /// Handles the inertia
         /// </summary>
          class InertiaHandler : IDisposable
         {
              private Point previousPoint;
              private Vector velocity;
              ScrollViewer scroller;
              DispatcherTimer animationTimer;

              private Point scrollTarget;
             public Point ScrollTarget
              {
                  get { return scrollTarget; }
                  set { scrollTarget = value; }
              }

              public InertiaHandler(ScrollViewer scroller)
              {
                  this.scroller = scroller;
                  animationTimer = new DispatcherTimer();
                  animationTimer.Interval =
                      new TimeSpan(0, 0, 0, 0, 20);
                  animationTimer.Tick +=
                      new EventHandler(HandleWorldTimerTick);
                  animationTimer.Start();
              }

              private void HandleWorldTimerTick(object sender,
                  EventArgs e)
              {
                  if (scroller.IsMouseCaptured)
                  {
                      Point currentPoint = Mouse.GetPosition(scroller);
                      velocity = previousPoint - currentPoint;
                      previousPoint = currentPoint;
                 }
                  else
                  {
                      if (velocity.Length > 1)
                      {
                          scroller.ScrollToHorizontalOffset(
                              ScrollTarget.X);
                          scroller.ScrollToVerticalOffset(
                              ScrollTarget.Y);
                          scrollTarget.X += velocity.X;
                          scrollTarget.Y += velocity.Y;
                          velocity *=
                              KineticBehaviour.GetFriction(scroller);
                      }
                  }
              }

              #region IDisposable Members

             public void Dispose()
              {
                 animationTimer.Stop();
              }

              #endregion
          }

          #endregion
      }
}