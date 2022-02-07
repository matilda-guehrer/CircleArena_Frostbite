using System.Windows.Shapes;

namespace CircleArena.Models.Events
{
    /// <summary>
    /// An Arena event type is defined by user action.
    /// </summary>
    public enum ArenaEventType
    {
        Create,
        Move
    }

    /// <summary>
    /// An Arena event is used to represent a user action within the arena.
    /// </summary>
    public interface IArenaEvent
    {
        /// <summary>
        /// The Target object of the event
        /// </summary>
        Ellipse Target { get; set; }

        /// <summary>
        /// The Event type that occurred.
        /// </summary>
        ArenaEventType Type { get; }
    }
}
