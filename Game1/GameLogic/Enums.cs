using Game1.GameEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameLogic
{
    public enum OrderType
    {
        Stop,
        MoveTo,
        InterceptObject,
        Orbit,
        Dock
    }

    public class Order
    {
        public Guid Guid { get; set; }
        public OrderType OrderType { get; set; }
        public string Label { get; set; }
        public (decimal x, decimal y) Position {  get; set; }
        public GameEntity Owner {  get; set; }
        public GameEntity Target { get; set; }
    }
}
