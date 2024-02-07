using Game1.GameLogic;
using Game1.GraphicalEntities;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameEntities
{
    //Can have stations or ships in it.
    public class Fleet : GameEntity
    {
        public GameEntity SOIEntity = null;
        public List<SubGameEntity> Members { get; set; } = new List<SubGameEntity>();
        public List<Order> Orders { get; set; } = new List<Order>();
        public Order CurrentOrder { get; set; }

        public Vector2 Velocity { get; set; } = Vector2.Zero;

        public long Fuel => Members.Select(x => x.Fuel).Sum();
        public long MaxFuel => Members.Select(x => x.MaxFuel).Sum();
        public long MaxAcceleration => GetMaxAcceleration();

        //public long MaxThrust => GetMaxThrust();
        public long CurrentThrust { get; set; } = 0;
        public float Direction
        {
            get
            {
                return Angle;
            }
            set
            {
                Angle = value;
            }
        }

        private long GetMaxThrust()
        {
            if (this.Members.Any(x => x is Station))
                return 0;

            return this.Members
                .Where(x => x is Ship)
                .Cast<Ship>()
                .Select(x => x.MaxThrust)
                .Max();
        }

        private long GetMaxAcceleration()
        {
            return this.Members.Where(x => x is Ship)
                .Cast<Ship>()
                .Select(x => x.Mass / x.Thrust)
                .Min();
        }

        public override void Update(float timePassed)
        {
            switch (CurrentOrder.OrderType)
            {
                case OrderType.Stop:
                    break;
                case OrderType.Orbit:
                    break;
                case OrderType.InterceptObject:
                    break;
                case OrderType.Dock:
                    break;
                case OrderType.MoveTo:
                    break;
                default:
                    break;
            }
        }

        public override GameGraphicalEntity GenerateGraphicalEntity(Game game)
        {
            var entity = new DotEntity(game);
            entity.GameEntity = this;
            this.GraphicalEntity = entity;

            return entity;
        }
    }
}
