using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using CircleArena.Helpers;
using CircleArena.Models.Events;

namespace CircleArena.ViewModels
{
    public class ArenaViewModel : INotifyPropertyChanged
    {
        private const int CircleSize = 50;

        private ObservableCollection<Ellipse> _circles;
        private List<IArenaEvent> _events;
        private IArenaEvent _lastEvent;
        private int _eventIndex;
        private bool _hasUndoActions;
        private bool _hasRedoActions;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged<T>([CallerMemberName]string caller = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(caller));
            }
        }

        /// <summary>
        /// The circles present on the arena
        /// </summary>
        public ObservableCollection<Ellipse> Circles
        {
            get
            {
                if (_circles == null)
                {
                    _circles = new ObservableCollection<Ellipse>();
                }
                return _circles;
            }
        }

        /// <summary>
        /// The arena events that are queued in the stack
        /// </summary>
        public List<IArenaEvent> Events
        {
            get
            {
                if (_events == null)
                {
                    _events = new List<IArenaEvent>();
                    _eventIndex = 0;
                }
                return _events;
            }
        }

        /// <summary>
        /// The last event that occurred on the arena
        /// </summary>
        public IArenaEvent LastEvent
        {
            get { return _lastEvent; }
            set
            {
                if (_lastEvent == value) return;
                _lastEvent = value;

                // Clear out the old event list that exceeds our current position in the queue.
                while (_eventIndex < Events.Count-1)
                {
                    Events.RemoveAt(_eventIndex+1);
                }

                // Add the new event and make sure we're at the top of the list.
                Events.Add(_lastEvent);
                _eventIndex = Events.Count-1;

                UpdateButtonProperties();
            }
        }

        /// <summary>
        /// Bound to whether there are available undo actions in the event stack.
        /// </summary>
        public bool HasUndoActions
        {
            get { return _hasUndoActions; }
            set
            {
                _hasUndoActions = value;
                OnPropertyChanged<bool>();
            }
        }

        /// <summary>
        /// Bound to whether there are available redo actions in the event stack.
        /// </summary>
        public bool HasRedoActions
        {
            get { return _hasRedoActions; }
            set
            {
                _hasRedoActions = value;
                OnPropertyChanged<bool>();
            }
        }

        public ICommand AddCircleButtonCommand { get; set; }
        public ICommand UndoButtonCommand { get; set; }
        public ICommand RedoButtonCommand { get; set; }

        // TODO: Make these actually fit the window size
        public float CanvasWidth { get; set; } = 772;
        public float CanvasHeight { get; set; } = 374;

        /// <summary>
        /// The view model assigned to the MainWindow arena
        /// </summary>
        public ArenaViewModel()
        {
            AddCircleButtonCommand = new RelayCommand(AddCircle);
            UndoButtonCommand = new RelayCommand(Undo);
            RedoButtonCommand = new RelayCommand(Redo);
        }

        /// <summary>
        /// Add a circle to the arena. This will trigger a new user event to the stack
        /// </summary>
        /// <param name="obj"></param>
        public void AddCircle(object obj)
        {
            var point = PointExtensions.GetRandomPointInRect(
                new Rect(0, 
                0, 
                CanvasWidth - CircleSize,
                CanvasHeight - CircleSize));

            var color = ColorExtensions.GetRandomColor();
            var circle = new Ellipse()
            {
                Stroke = new SolidColorBrush(color),
                StrokeThickness = CircleSize/20d,
                Fill = new SolidColorBrush(ColorExtensions.GetTransluecientColorFromColor(color)),
                Width = CircleSize,
                Height = CircleSize,
                Margin = new Thickness(point.X, point.Y, 0, 0)
            };

            Circles.Add(circle);
            LastEvent = new ArenaEventCreate() {Target = circle};
        }

        /// <summary>
        /// An event action to remove a circle from the arena.
        /// </summary>
        /// <param name="e"></param>
        private void RemoveCircleFromEvent(IArenaEvent e)
        {
            Circles.Remove(e.Target);
        }

        /// <summary>
        /// An event action to create a circle to the arena
        /// </summary>
        /// <param name="e"></param>
        private void AddCircleFromEvent(IArenaEvent e)
        {
            if (Circles.Contains(e.Target)) return;
            Circles.Add(e.Target);
        }

        /// <summary>
        /// An event action to undo a move command on a circle within the arena.
        /// </summary>
        /// <param name="e"></param>
        private void UndoMoveCircleFromEvent(ArenaEventMove e)
        {
            e.Target.Margin = new Thickness(e.PreviousMarginLeft, e.PreviousMarginTop, 0, 0);
        }

        /// <summary>
        /// An event action to redo a move command on a circle within the arena
        /// </summary>
        /// <param name="e"></param>
        private void RedoMoveCircleFromEvent(ArenaEventMove e)
        {
            e.Target.Margin = new Thickness(e.NewMarginLeft, e.NewMarginTop, 0, 0);
        }

        public void Undo(object obj)
        {
            if (Events.Count == 0 || _eventIndex < 0 || _eventIndex > Events.Count-1) return;

            // Get event at event index
            var e = Events[_eventIndex];

            switch (e.Type)
            {
                case ArenaEventType.Create:
                    RemoveCircleFromEvent(e);
                    break;
                case ArenaEventType.Move:
                    UndoMoveCircleFromEvent(e as ArenaEventMove);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (_eventIndex >= 0) _eventIndex--;
            UpdateButtonProperties();
        }

        public void Redo(object obj)
        {
            if (Events.Count == 0 || _eventIndex > Events.Count - 1) return;

            if (_eventIndex < Events.Count - 1) _eventIndex++;

            // Get event at event index
            var e = Events[_eventIndex];

            switch (e.Type)
            {
                case ArenaEventType.Create:
                    AddCircleFromEvent(e);
                    break;
                case ArenaEventType.Move:
                    RedoMoveCircleFromEvent(e as ArenaEventMove);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            UpdateButtonProperties();
        }

        private void UpdateButtonProperties()
        {
            HasUndoActions = Events.Count != 0 && _eventIndex >= 0;
            HasRedoActions = Events.Count != 0 && _eventIndex < Events.Count - 1;
        }
    }
}
