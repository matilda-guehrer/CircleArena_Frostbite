using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using CircleArena.Models.Events;

namespace CircleArena.Controls
{
    /// <summary>
    /// The circle arena that allows <see cref="Ellipse"/> objects to be dragged around.
    /// </summary>
    public class ArenaCanvas : Canvas
    {
        // Some notes:
        // * This should've been made to be more generic to the shape. It's hard coded to be an ellipse, but it doesn't need to be.
        // * Possible improvements can be made to allow shapes to be dragged outside of the canvas bounds.

        private Ellipse _currentDragObject;
        private bool _isCurrentlyDragging => _currentDragObject != null;
        private double? _originalMarginLeft = null;
        private double? _originalMarginTop = null;

        /// <summary>
        /// A circle arena that allows <see cref="Ellipse"/> objects to be dragged around.
        /// </summary>
        public ArenaCanvas()
        {
            _currentDragObject = null;
            this.Background = Brushes.Ivory;
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            StartDrag(e);
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);
            Drag(e);
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);
            EndDrag(e);
        }

        private void StartDrag(MouseButtonEventArgs e)
        {
            // If we're already dragging an object, make sure we trigger a drop first
            if (_isCurrentlyDragging) EndDrag(e);

            // Ensure we drop our current object
            _currentDragObject = null;

            // Walk up the visual tree from the element that was clicked, 
            // looking for an element that is a direct child of the Canvas.
            _currentDragObject = this.FindCircle(e.Source as DependencyObject);

            if (_currentDragObject == null) return;

            // Capture the original margin points so we can add to our event at the drop
            _originalMarginLeft = _currentDragObject.Margin.Left;
            _originalMarginTop = _currentDragObject.Margin.Top;

            // Set the Handled flag so that a control being dragged 
            // does not react to the mouse input.
            e.Handled = true;
        }

        private void Drag(MouseEventArgs e)
        {
            // If no element is being dragged, there is nothing to do.
            if (!_isCurrentlyDragging) return;

            // Get the position of the mouse cursor, relative to the Canvas.
            Point cursorLocation = e.GetPosition(this);

            var element = _currentDragObject;

            var left = cursorLocation.X - (element.ActualWidth / 2);
            var top = cursorLocation.Y - (element.ActualHeight / 2);

            // Bind the control to the canvas space
            left = Math.Max(0, Math.Min(left, this.ActualWidth - element.ActualWidth));
            top = Math.Max(0, Math.Min(top, this.ActualHeight - element.ActualHeight));

            element.Margin = new Thickness(left, top, 0, 0);
        }

        private void EndDrag(MouseButtonEventArgs e)
        {
            // If we're not dragging anything, do nothing.
            if (!_isCurrentlyDragging) return;

            // Update our last action
            LastMoveAction = new ArenaEventMove()
            {
                Target = _currentDragObject,
                NewMarginLeft = _currentDragObject.Margin.Left,
                NewMarginTop = _currentDragObject.Margin.Top,
                PreviousMarginLeft = _originalMarginLeft.GetValueOrDefault(),
                PreviousMarginTop = _originalMarginTop.GetValueOrDefault()
            };

            // Remove our dragged objects
            _currentDragObject = null;
        }

        private Ellipse FindCircle(DependencyObject obj)
        {
            while (obj != null)
            {
                // If the current object is a UIElement which is a child of the
                // Canvas, exit the loop and return it.
                var element = obj as Ellipse;
                if (element != null && Children.Contains(element))
                    break;
                
                obj = VisualTreeHelper.GetParent(obj);
            }
            return obj as Ellipse;
        }

        public static DependencyProperty LastMoveActionProperty =
            DependencyProperty.Register(nameof(LastMoveAction), typeof(ArenaEventMove),
                typeof(ArenaCanvas));

        public ArenaEventMove LastMoveAction
        {
            get => (ArenaEventMove)GetValue(LastMoveActionProperty);
            set => SetValue(LastMoveActionProperty, value);
        }
    }
}
