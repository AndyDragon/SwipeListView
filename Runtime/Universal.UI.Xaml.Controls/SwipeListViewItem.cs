﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Universal.UI.Xaml.Controls
{
    public class SwipeListViewItem : ListViewItem
    {
        private TranslateTransform ContentDragTransform;
        private TranslateTransform LeftTransform;
        private TranslateTransform RightTransform;

        private Border LeftContainer;
        private Border RightContainer;

        private Grid DragBackground;

        private Border DragContainer;

        public SwipeListViewItem()
        {
            DefaultStyleKey = typeof(SwipeListViewItem);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ContentDragTransform = (TranslateTransform)GetTemplateChild("ContentDragTransform");
            LeftTransform = (TranslateTransform)GetTemplateChild("LeftTransform");
            RightTransform = (TranslateTransform)GetTemplateChild("RightTransform");

            LeftContainer = (Border)GetTemplateChild("LeftContainer");
            RightContainer = (Border)GetTemplateChild("RightContainer");

            DragBackground = (Grid)GetTemplateChild("DragBackground");
            DragContainer = (Border)GetTemplateChild("DragContainer");
        }

        internal void ResetSwipe()
        {
            if (DragBackground != null)
            {
                DragBackground.Background = null;

                ContentDragTransform.X = 0;
                LeftTransform.X = -(LeftContainer.ActualWidth + 20);
                RightTransform.X = (RightContainer.ActualWidth + 20);
            }
        }

        private SwipeListDirection _direction = SwipeListDirection.None;

        protected override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs e)
        {
            var target = ((ActualWidth / 5) * 1);

            if (_direction == SwipeListDirection.None)
            {
                _direction = e.Delta.Translation.X > 0 
                    ? SwipeListDirection.Left 
                    : SwipeListDirection.Right;

                DragBackground.Background = _direction == SwipeListDirection.Left 
                    ? LeftBackground 
                    : RightBackground;

                LeftTransform.X = -(LeftContainer.ActualWidth + 20);
                RightTransform.X = (RightContainer.ActualWidth + 20);

                if (_direction == SwipeListDirection.Left && LeftBehavior != SwipeListBehavior.Disabled)
                {
                    DragBackground.Background = LeftBackground;

                    LeftContainer.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    RightContainer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
                else if (_direction == SwipeListDirection.Right && RightBehavior != SwipeListBehavior.Disabled)
                {
                    DragBackground.Background = RightBackground;

                    LeftContainer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    RightContainer.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else
                {
                    e.Complete();
                    return;
                }
            }

            if (_direction == SwipeListDirection.Left)
            {
                var area1 = LeftBehavior == SwipeListBehavior.Collapse ? 1.7 : 2.5;
                var area2 = LeftBehavior == SwipeListBehavior.Collapse ? 2 : 3;

                ContentDragTransform.X = Math.Max(0, Math.Min(e.Cumulative.Translation.X, ActualWidth));

                if (ContentDragTransform.X < target * area1)
                {
                    LeftTransform.X += (e.Delta.Translation.X / 1.5);
                }
                else if (ContentDragTransform.X >= target * area1 && ContentDragTransform.X < target * area2)
                {
                    LeftTransform.X += (e.Delta.Translation.X * 3);
                }
                else
                {
                    LeftTransform.X = Math.Max(0, Math.Min(e.Cumulative.Translation.X, ActualWidth)) - LeftContainer.ActualWidth;
                }

                if (ContentDragTransform.X == 0 && e.Delta.Translation.X < 0)
                {
                    _direction = SwipeListDirection.None;
                }
            }
            else if (_direction == SwipeListDirection.Right)
            {
                var area1 = RightBehavior == SwipeListBehavior.Collapse ? 1.7 : 2.5;
                var area2 = RightBehavior == SwipeListBehavior.Collapse ? 2 : 3;

                ContentDragTransform.X = Math.Max(-ActualWidth, Math.Min(e.Cumulative.Translation.X, 0));

                if (ContentDragTransform.X > -(target * area1))
                {
                    RightTransform.X += (e.Delta.Translation.X / 1.5);
                }
                else if (ContentDragTransform.X <= -(target * area1) && ContentDragTransform.X > -(target * area2))
                {
                    RightTransform.X += (e.Delta.Translation.X * 3);
                }
                else
                {
                    RightTransform.X = Math.Max(-ActualWidth, Math.Min(e.Cumulative.Translation.X, 0)) + RightContainer.ActualWidth;
                }

                if (ContentDragTransform.X == 0 && e.Delta.Translation.X > 0)
                {
                    _direction = SwipeListDirection.None;
                }
            }

            //e.Handled = true;
            //base.OnManipulationDelta(e);
        }

        protected override void OnManipulationCompleted(ManipulationCompletedRoutedEventArgs e)
        {
            var target = (ActualWidth / 5) * 2;
            if ((_direction == SwipeListDirection.Left && LeftBehavior == SwipeListBehavior.Expand) ||
                (_direction == SwipeListDirection.Right && RightBehavior == SwipeListBehavior.Expand))
            {
                target = (ActualWidth / 5) * 3;
            }

            Storyboard currentAnim;

            if (_direction == SwipeListDirection.Left && ContentDragTransform.X >= target)
            {
                if (LeftBehavior == SwipeListBehavior.Collapse)
                    currentAnim = CollapseAnimation(SwipeListDirection.Left, true);
                else
                    currentAnim = ExpandAnimation(SwipeListDirection.Left);
            }
            else if (_direction == SwipeListDirection.Right && ContentDragTransform.X <= -target)
            {
                if (RightBehavior == SwipeListBehavior.Collapse)
                    currentAnim = CollapseAnimation(SwipeListDirection.Right, true);
                else
                    currentAnim = ExpandAnimation(SwipeListDirection.Right);
            }
            else
            {
                currentAnim = CollapseAnimation(SwipeListDirection.Left, false);
            }

            currentAnim.Begin();
            _direction = SwipeListDirection.None;

            //e.Handled = true;
            //base.OnManipulationCompleted(e);
        }

        private Storyboard CollapseAnimation(SwipeListDirection direction, bool raise)
        {
            var animDrag = CreateDouble(0, 300, ContentDragTransform, "TranslateTransform.X", new ExponentialEase { EasingMode = EasingMode.EaseOut });
            var animLeft = CreateDouble(-(LeftContainer.ActualWidth + 20), 300, LeftTransform, "TranslateTransform.X", new ExponentialEase { EasingMode = EasingMode.EaseOut });
            var animRight = CreateDouble((RightContainer.ActualWidth + 20), 300, RightTransform, "TranslateTransform.X", new ExponentialEase { EasingMode = EasingMode.EaseOut });

            var currentAnim = new Storyboard();
            currentAnim.Children.Add(animDrag);
            currentAnim.Children.Add(animLeft);
            currentAnim.Children.Add(animRight);

            currentAnim.Completed += (s, args) =>
            {
                DragBackground.Background = null;

                ContentDragTransform.X = 0;
                LeftTransform.X = -(LeftContainer.ActualWidth + 20);
                RightTransform.X = (RightContainer.ActualWidth + 20);

                Grid.SetColumn(DragBackground, 1);
                Grid.SetColumnSpan(DragBackground, 1);

            };

            if (raise)
            {
                if (ItemSwipe != null)
                    ItemSwipe(this, new ItemSwipeEventArgs(Content, direction));
            }

            return currentAnim;
        }

        private Storyboard ExpandAnimation(SwipeListDirection direction)
        {
            var currentAnim = new Storyboard();
            if (direction == SwipeListDirection.Left)
            {
                var animDrag = CreateDouble(ActualWidth + 100, 300, ContentDragTransform, "TranslateTransform.X", new ExponentialEase { EasingMode = EasingMode.EaseOut });
                var animLeft = CreateDouble(ActualWidth + 100, 300, LeftTransform, "TranslateTransform.X", new ExponentialEase { EasingMode = EasingMode.EaseIn });
                var animRight = CreateDouble(ActualWidth + 100, 300, RightTransform, "TranslateTransform.X", new ExponentialEase { EasingMode = EasingMode.EaseIn });

                currentAnim.Children.Add(animDrag);
                currentAnim.Children.Add(animLeft);
                currentAnim.Children.Add(animRight);
            }
            else if (direction == SwipeListDirection.Right)
            {
                var animDrag = CreateDouble(-ActualWidth - 100, 300, ContentDragTransform, "TranslateTransform.X", new ExponentialEase { EasingMode = EasingMode.EaseOut });
                var animLeft = CreateDouble(-ActualWidth - 100, 300, LeftTransform, "TranslateTransform.X", new ExponentialEase { EasingMode = EasingMode.EaseIn });
                var animRight = CreateDouble(-ActualWidth - 100, 300, RightTransform, "TranslateTransform.X", new ExponentialEase { EasingMode = EasingMode.EaseIn });

                currentAnim.Children.Add(animDrag);
                currentAnim.Children.Add(animLeft);
                currentAnim.Children.Add(animRight);
            }

            currentAnim.Completed += (s, args) =>
            {
                if (ItemSwipe != null)
                    ItemSwipe(this, new ItemSwipeEventArgs(Content, direction));
            };

            return currentAnim;
        }

        private DoubleAnimation CreateDouble(double to, int duration, DependencyObject target, string path, EasingFunctionBase easing)
        {
            var anim = new DoubleAnimation();
            anim.To = to;
            anim.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
            anim.EasingFunction = easing;

            Storyboard.SetTarget(anim, target);
            Storyboard.SetTargetProperty(anim, path);

            return anim;
        }

        public event ItemSwipeEventHandler ItemSwipe;

        #region LeftContentTemplate
        public DataTemplate LeftContentTemplate
        {
            get { return (DataTemplate)GetValue(LeftContentTemplateProperty); }
            set { SetValue(LeftContentTemplateProperty, value); }
        }

        public static readonly DependencyProperty LeftContentTemplateProperty =
            DependencyProperty.Register("LeftContentTemplate", typeof(DataTemplate), typeof(SwipeListViewItem), new PropertyMetadata(null));
        #endregion

        #region LeftBackground
        public Brush LeftBackground
        {
            get { return (Brush)GetValue(LeftBackgroundProperty); }
            set { SetValue(LeftBackgroundProperty, value); }
        }

        public static readonly DependencyProperty LeftBackgroundProperty =
            DependencyProperty.Register("LeftBackground", typeof(Brush), typeof(SwipeListViewItem), new PropertyMetadata(null));
        #endregion

        #region LeftBehavior
        public SwipeListBehavior LeftBehavior
        {
            get { return (SwipeListBehavior)GetValue(LeftBehaviorProperty); }
            set { SetValue(LeftBehaviorProperty, value); }
        }

        public static readonly DependencyProperty LeftBehaviorProperty =
            DependencyProperty.Register("LeftBehavior", typeof(SwipeListBehavior), typeof(SwipeListViewItem), new PropertyMetadata(SwipeListBehavior.Collapse));
        #endregion

        #region RightContentTemplate
        public DataTemplate RightContentTemplate
        {
            get { return (DataTemplate)GetValue(RightContentTemplateProperty); }
            set { SetValue(RightContentTemplateProperty, value); }
        }

        public static readonly DependencyProperty RightContentTemplateProperty =
            DependencyProperty.Register("RightContentTemplate", typeof(DataTemplate), typeof(SwipeListViewItem), new PropertyMetadata(null));
        #endregion

        #region RightBackground
        public Brush RightBackground
        {
            get { return (Brush)GetValue(RightBackgroundProperty); }
            set { SetValue(RightBackgroundProperty, value); }
        }

        public static readonly DependencyProperty RightBackgroundProperty =
            DependencyProperty.Register("RightBackground", typeof(Brush), typeof(SwipeListViewItem), new PropertyMetadata(null));
        #endregion

        #region RightBehavior
        public SwipeListBehavior RightBehavior
        {
            get { return (SwipeListBehavior)GetValue(RightBehaviorProperty); }
            set { SetValue(RightBehaviorProperty, value); }
        }

        public static readonly DependencyProperty RightBehaviorProperty =
            DependencyProperty.Register("RightBehavior", typeof(SwipeListBehavior), typeof(SwipeListViewItem), new PropertyMetadata(SwipeListBehavior.Expand));
        #endregion
    }
}
