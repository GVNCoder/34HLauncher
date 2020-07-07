using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Launcher.Styles.Controls
{
    //public class ScrollViewerOffsetMediator : FrameworkElement
    //{
    //    /// <summary>
    //    /// ScrollViewer instance to forward Offset changes on to.
    //    /// </summary>
    //    public ScrollViewer ScrollViewer
    //    {
    //        get { return (ScrollViewer)GetValue(ScrollViewerProperty); }
    //        set { SetValue(ScrollViewerProperty, value); }
    //    }
    //    public static readonly DependencyProperty ScrollViewerProperty =
    //        DependencyProperty.Register(
    //            "ScrollViewer",
    //            typeof(ScrollViewer),
    //            typeof(ScrollViewerOffsetMediator),
    //            new PropertyMetadata(OnScrollViewerChanged));
    //    private static void OnScrollViewerChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    //    {
    //        var mediator = (ScrollViewerOffsetMediator)o;
    //        var scrollViewer = (ScrollViewer)(e.NewValue);
    //        if (null != scrollViewer)
    //        {
    //            scrollViewer.ScrollToVerticalOffset(mediator.VerticalOffset);
    //        }
    //    }

    //    /// <summary>
    //    /// VerticalOffset property to forward to the ScrollViewer.
    //    /// </summary>
    //    public double VerticalOffset
    //    {
    //        get { return (double)GetValue(VerticalOffsetProperty); }
    //        set { SetValue(VerticalOffsetProperty, value); }
    //    }
    //    public static readonly DependencyProperty VerticalOffsetProperty =
    //        DependencyProperty.Register(
    //            "VerticalOffset",
    //            typeof(double),
    //            typeof(ScrollViewerOffsetMediator),
    //            new PropertyMetadata(OnVerticalOffsetChanged));
    //    public static void OnVerticalOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    //    {
    //        var mediator = (ScrollViewerOffsetMediator)o;
    //        if (null != mediator.ScrollViewer)
    //        {
    //            mediator.ScrollViewer.ScrollToVerticalOffset((double)(e.NewValue));
    //        }
    //    }

    //    /// <summary>
    //    /// Multiplier for ScrollableHeight property to forward to the ScrollViewer.
    //    /// </summary>
    //    /// <remarks>
    //    /// 0.0 means "scrolled to top"; 1.0 means "scrolled to bottom".
    //    /// </remarks>
    //    public double ScrollableHeightMultiplier
    //    {
    //        get { return (double)GetValue(ScrollableHeightMultiplierProperty); }
    //        set { SetValue(ScrollableHeightMultiplierProperty, value); }
    //    }
    //    public static readonly DependencyProperty ScrollableHeightMultiplierProperty =
    //        DependencyProperty.Register(
    //            "ScrollableHeightMultiplier",
    //            typeof(double),
    //            typeof(ScrollViewerOffsetMediator),
    //            new PropertyMetadata(OnScrollableHeightMultiplierChanged));
    //    public static void OnScrollableHeightMultiplierChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    //    {
    //        var mediator = (ScrollViewerOffsetMediator)o;
    //        var scrollViewer = mediator.ScrollViewer;
    //        if (null != scrollViewer)
    //        {
    //            scrollViewer.ScrollToVerticalOffset((double)(e.NewValue) * scrollViewer.ScrollableHeight);
    //        }
    //    }
    //}

    //public class AnimatableScrollView : ScrollViewer
    //{
    //    public AnimatableScrollView()
    //    {
    //        //ScrollChanged += OnScrollChanged;
    //        //PreviewMouseWheel += OnPreviewMouseWheel;
    //        //SizeChanged += OnSizeChanged;
    //    }

    //    //private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    //    //{
    //    //    if (IsLoaded)
    //    //        UpadeUpdateAnimator();
    //    //}

    //    //private void UpadeUpdateAnimator()
    //    //{
    //    //    ScrollToEnd();
    //    //}

    //    //private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
    //    //{
    //    //    e.Handled = true;
    //    //    int Division_Unit = 5;
    //    //    ScrollToVerticalOffset((VerticalOffset + ((e.Delta * -1) / Division_Unit)));
    //    //}

    //    private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
    //    {
    //        PowerEase _ease = new PowerEase {EasingMode = EasingMode.EaseOut, Power = 8};

    //        DoubleAnimation scrollAnimation = new DoubleAnimation(VerticalOffset, TimeSpan.FromSeconds(2));
    //        scrollAnimation.EasingFunction = _ease;

    //        BeginAnimation(AnimatableScrollView.VerticalOffsetAnimationValueProperty, scrollAnimation);
    //    }



    //    public double HorizontalOffsetAnimationValue
    //    {
    //        get { return (double) GetValue(HorizontalOffsetAnimationValueProperty); }
    //        set { SetValue(HorizontalOffsetAnimationValueProperty, value); }
    //    }

    //    public static readonly DependencyProperty HorizontalOffsetAnimationValueProperty =
    //        DependencyProperty.Register("HorizontalOffsetAnimationValue", typeof(double), typeof(AnimatableScrollView),
    //            new PropertyMetadata(OnHorizontalChanged));

    //    public double VerticalOffsetAnimationValue
    //    {
    //        get { return (double) GetValue(VerticalOffsetAnimationValueProperty); }
    //        set { SetValue(VerticalOffsetAnimationValueProperty, value); }
    //    }

    //    public static readonly DependencyProperty VerticalOffsetAnimationValueProperty =
    //        DependencyProperty.Register("VerticalOffsetAnimationValue", typeof(double), typeof(AnimatableScrollView),
    //            new PropertyMetadata(OnVerticalChanged));

    //    private static void OnVerticalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        if (d is AnimatableScrollView fixer)
    //            fixer.ScrollToVerticalOffset((double) e.NewValue);
    //    }
    //    private static void OnHorizontalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        if (d is AnimatableScrollView fixer)
    //            fixer.ScrollToHorizontalOffset((double) e.NewValue);
    //    }
    //}
}