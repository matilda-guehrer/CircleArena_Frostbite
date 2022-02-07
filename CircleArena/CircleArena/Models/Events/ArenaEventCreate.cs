using System.Windows.Shapes;

namespace CircleArena.Models.Events
{
    public class ArenaEventCreate : IArenaEvent
    {
        public Ellipse Target { get; set; }
        public ArenaEventType Type => ArenaEventType.Create;
    }
}