using System.Windows.Shapes;

namespace CircleArena.Models.Events
{
    public class ArenaEventMove : IArenaEvent
    {
        public Ellipse Target { get; set; }
        public ArenaEventType Type => ArenaEventType.Move;
        public double PreviousMarginLeft { get; set; }
        public double PreviousMarginTop { get; set; }
        public double NewMarginLeft { get; set; }
        public double NewMarginTop { get; set; }
    }
}