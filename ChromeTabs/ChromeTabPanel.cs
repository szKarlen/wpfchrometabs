﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Media.Animation;
using System.Threading;

namespace ChromeTabs
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ChromiumTabs"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ChromiumTabs;assembly=ChromiumTabs"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:ChromiumTabPanel/>
    ///
    /// </summary>
    [ToolboxItem(false)]
    public class ChromeTabPanel : Panel
    {
        public bool IsReorderingTabs
        {
            get { return (bool)GetValue(IsReorderingTabsProperty); }
            private set { SetValue(IsReorderingTabsPropertyKey, value); }
        }

        internal static readonly DependencyPropertyKey IsReorderingTabsPropertyKey = DependencyProperty.RegisterReadOnly("IsReorderingTabs", typeof(bool), typeof(ChromeTabPanel), new PropertyMetadata(false, IsReorderingChangedCallback));

        private static void IsReorderingChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var chromeTabPanel = dependencyObject as ChromeTabPanel;
            if (chromeTabPanel != null)
            {
                chromeTabPanel.ParentTabControl.SetValue(ChromeTabPanel.IsReorderingTabsPropertyKey, dependencyPropertyChangedEventArgs.NewValue);
            }
        }

        // Using a DependencyProperty as the backing store for IsReorderingTabs.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsReorderingTabsProperty = IsReorderingTabsPropertyKey.DependencyProperty;

        public double TabsOverlap
        {
            get { return (double)GetValue(TabsOverlapProperty); }
            set { SetValue(TabsOverlapProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Overlap.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TabsOverlapProperty =
            DependencyProperty.Register("TabsOverlap", typeof(double), typeof(ChromeTabPanel), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public double MaxTabWidth
        {
            get { return (double)GetValue(MaxTabWidthProperty); }
            set { SetValue(MaxTabWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxTabWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxTabWidthProperty =
            DependencyProperty.Register("MaxTabWidth", typeof(double), typeof(ChromeTabPanel), new PropertyMetadata(180.0));

        public double MinTabWidth
        {
            get { return (double)GetValue(MinTabWidthProperty); }
            set { SetValue(MinTabWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinTabWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinTabWidthProperty =
            DependencyProperty.Register("MinTabWidth", typeof(double), typeof(ChromeTabPanel), new PropertyMetadata(40.0));

        static ChromeTabPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChromeTabPanel), new FrameworkPropertyMetadata(typeof(ChromeTabPanel)));
        }

        public ChromeTabPanel()
        {
            this.defaultMeasureHeight = 30.0;
        }

        protected override int VisualChildrenCount
        {
            get { return base.VisualChildrenCount; }
        }

        protected override Visual GetVisualChild(int index)
        {
            if(index == this.VisualChildrenCount - 1)
            {
                return base.GetVisualChild(index);
            }
            if(index < this.VisualChildrenCount - 1)
            {
                return base.GetVisualChild(index);
            }
            throw new IndexOutOfRangeException("Not enough visual children in the ChromeTabPanel.");
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            Point start = new Point(0, Math.Round(this.finalSize.Height));
            Point end = new Point(this.finalSize.Width, Math.Round(this.finalSize.Height));
            Color penColor = (Color)ColorConverter.ConvertFromString("#FF999999");
            Brush brush = new SolidColorBrush(penColor);
            Pen pen = new Pen(brush, .5);
            dc.DrawLine(pen, start, end);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double activeWidth = finalSize.Width - this.Margin.Left - this.Margin.Right;
            this.currentTabWidth = Math.Min(Math.Max((activeWidth + (this.Children.Count - 1) * this.TabsOverlap)/ this.Children.Count, this.MinTabWidth), this.MaxTabWidth);
            //ParentTabControl.SetCanAddTab(this.currentTabWidth > this.minTabWidth);
            this.finalSize = finalSize;
            double offset = Margin.Left;
            foreach (UIElement element in this.Children)
            {
                double thickness = 0.0;
                ChromeTabItem item = ItemsControl.ContainerFromElement(this.ParentTabControl, element) as ChromeTabItem;
                thickness = item.Margin.Bottom;
                element.Arrange(new Rect(offset, 0, this.currentTabWidth, finalSize.Height - thickness));
                offset += this.currentTabWidth - this.TabsOverlap;
            }
            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double activeWidth = double.IsPositiveInfinity(availableSize.Width) ? 500 : availableSize.Width - this.Margin.Left - this.Margin.Right;
            this.currentTabWidth = Math.Min(Math.Max((activeWidth + (this.Children.Count - 1) * this.TabsOverlap) / this.Children.Count, this.MinTabWidth), this.MaxTabWidth);
            //ParentTabControl.SetCanAddTab(this.currentTabWidth > this.minTabWidth);
            double height = double.IsPositiveInfinity(availableSize.Height) ? this.defaultMeasureHeight : availableSize.Height;
            Size resultSize = new Size(0, availableSize.Height);
            foreach (UIElement child in this.Children)
            {
                ChromeTabItem item = ItemsControl.ContainerFromElement(this.ParentTabControl, child) as ChromeTabItem;
                Size tabSize = new Size(this.currentTabWidth, height - item.Margin.Bottom);
                child.Measure(tabSize);
                resultSize.Width += child.DesiredSize.Width - this.TabsOverlap;
            }
            return resultSize;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            this.SetTabItemsOnTabs();
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
            this.SetTabItemsOnTabs();
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            this.slideIntervals = null;
            
            this.downPoint = e.GetPosition(this);
            HitTestResult result = VisualTreeHelper.HitTest(this, this.downPoint);
            if(result == null) { return; }
            DependencyObject source = result.VisualHit;
            while(source != null && !this.Children.Contains(source as UIElement))
            {
                source = VisualTreeHelper.GetParent(source);
            }
            if(source == null) { return; }
            draggedTab = source as ChromeTabItem;
            if(draggedTab != null && this.Children.Count > 1)
            {
                Canvas.SetZIndex(draggedTab, 1000);
            }
            else if(draggedTab != null && this.Children.Count == 1)
            {
                this.draggingWindow = true;
                Window.GetWindow(this).DragMove();
            }
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);
            if(this.draggedTab == null || this.draggingWindow) { return; }
            Point nowPoint = e.GetPosition(this);
            Thickness margin = new Thickness(nowPoint.X - this.downPoint.X, 0, this.downPoint.X - nowPoint.X, 0);
            this.draggedTab.Margin = margin;
            IsReorderingTabs = true;
            if(margin.Left != 0)
            {
                int guardValue = Interlocked.Increment(ref this.captureGuard);
                if(guardValue == 1)
                {
                    this.originalIndex = this.draggedTab.Index;
                    this.slideIndex = this.originalIndex + 1;
                    this.slideIntervals = new List<double>();
                    this.slideIntervals.Add(double.NegativeInfinity);
                    for(int i = 1; i <= this.Children.Count; i += 1)
                    {
                        var diff = i - this.slideIndex;
                        var sign = diff == 0 ? 0 : diff / Math.Abs(diff);
                        var bound = Math.Min(1, Math.Abs(diff)) * ((sign * this.currentTabWidth / 3) + ((Math.Abs(diff) < 2) ? 0 : (diff - sign) * (this.currentTabWidth - this.TabsOverlap)));
                        this.slideIntervals.Add(bound);
                    }
                    this.slideIntervals.Add(double.PositiveInfinity);
                    this.CaptureMouse();
                }
                else
                {
                    int changed = 0;
                    if(margin.Left < this.slideIntervals[this.slideIndex - 1])
                    {
                        SwapSlideInterval(this.slideIndex - 1);
                        this.slideIndex -= 1;
                        changed = 1;
                    }
                    else if(margin.Left > this.slideIntervals[this.slideIndex + 1])
                    {
                        SwapSlideInterval(this.slideIndex + 1);
                        this.slideIndex += 1;
                        changed = -1;
                    }
                    if(changed != 0)
                    {
                        var rightedOriginalIndex = this.originalIndex + 1;
                        var diff = 1;
                        if(changed > 0 && this.slideIndex >= rightedOriginalIndex)
                        {
                            changed = 0;
                            diff = 0;
                        }
                        else if(changed < 0 && this.slideIndex <= rightedOriginalIndex)
                        {
                            changed = 0;
                            diff = 2;
                        }
                        ChromeTabItem shiftedTab = this.Children[this.slideIndex - diff] as ChromeTabItem;
                        if(shiftedTab != this.draggedTab)
                        {
                            StickyReanimate(shiftedTab, changed * (this.currentTabWidth - this.TabsOverlap), .01);
                        }
                    }
                }
            }
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);
            this.draggingWindow = false;
            if(this.IsMouseCaptured && this.slideIntervals != null)
            {
                Mouse.Capture(null);

                double offset = 0;
                if(this.slideIndex < this.originalIndex + 1)
                {
                    offset = this.slideIntervals[this.slideIndex + 1] - 2 * this.currentTabWidth / 3 + this.TabsOverlap;
                }
                else if(this.slideIndex > this.originalIndex + 1)
                {
                    offset = this.slideIntervals[this.slideIndex - 1] + 2 * this.currentTabWidth / 3 - this.TabsOverlap;
                }
                Console.WriteLine(offset);
                Action completed = () =>
                {
                    if(this.draggedTab != null)
                    {
                        ParentTabControl.ChangeSelectedItem(this.draggedTab);
                        this.draggedTab.Margin = new Thickness(offset, 0, -offset, 0);
                        this.draggedTab = null;
                        this.captureGuard = 0;
                        ParentTabControl.MoveTab(this.originalIndex, this.slideIndex - 1);
                        IsReorderingTabs = false;
                    }
                };
                Reanimate(this.draggedTab, offset, .1, completed);
            }
            else
            {
                if(this.draggedTab != null)
                {
                    ParentTabControl.ChangeSelectedItem(this.draggedTab);
                    this.draggedTab.Margin = new Thickness(0);
                }
                this.draggedTab = null;
                this.captureGuard = 0;
            }
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            this.parent = null;
        }

        private ChromeTabControl ParentTabControl
        {
            get
            {
                if(this.parent == null)
                {
                    DependencyObject parent = this;
                    while (parent != null && !(parent is ChromeTabControl))
                    {
                        parent = VisualTreeHelper.GetParent(parent);
                    }
                    this.parent = parent as ChromeTabControl;
                }
                return this.parent;
            }
        }

        private static void StickyReanimate(ChromeTabItem tab, double left, double duration)
        {
            Action completed = () =>
            {
                tab.Margin = new Thickness(left, 0, -left, 0);
            };
            Reanimate(tab, left, duration, completed);
        }

        private static void Reanimate(ChromeTabItem tab, double left, double duration, Action completed)
        {
            if(tab == null)
            {
                return;
            }
            Thickness offset = new Thickness(left, 0, -left, 0);
            ThicknessAnimation moveBackAnimation = new ThicknessAnimation(tab.Margin, offset, new Duration(TimeSpan.FromSeconds(duration)));
            Storyboard.SetTarget(moveBackAnimation, tab);
            Storyboard.SetTargetProperty(moveBackAnimation, new PropertyPath(FrameworkElement.MarginProperty));
            Storyboard sb = new Storyboard();
            sb.Children.Add(moveBackAnimation);
            sb.Completed += (o, ea) =>
            {
                sb.Remove();
                if(completed != null)
                {
                    completed();
                }
            };
            sb.Begin();
        }

        private void SetTabItemsOnTabs()
        {
            for(int i = 0; i < this.Children.Count; i += 1)
            {
                DependencyObject depObj = this.Children[i] as DependencyObject;
                if(depObj == null)
                {
                    continue;
                }
                ChromeTabItem item = ItemsControl.ContainerFromElement(this.ParentTabControl, depObj) as ChromeTabItem;
                if(item != null)
                {
                    depObj.SetValue(System.Windows.Shell.WindowChrome.IsHitTestVisibleInChromeProperty, true);
                    KeyboardNavigation.SetTabIndex(item, i);
                }
            }
        }

        private void SwapSlideInterval(int index)
        {
            this.slideIntervals[this.slideIndex] = this.slideIntervals[index];
            this.slideIntervals[index] = 0;
        }

        private bool draggingWindow;
        private Size finalSize;
        private double defaultMeasureHeight;
        private double currentTabWidth;
        private int captureGuard;
        private int originalIndex;
        private int slideIndex;
        private List<double> slideIntervals;
        private ChromeTabItem draggedTab;
        private Point downPoint;
        private ChromeTabControl parent;
    }
}
