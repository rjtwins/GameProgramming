using Game1.GraphicalEntities;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameEntities
{
    public abstract class Orbital : GameEntity
    {
        public virtual decimal CurrentTime { get; set; } = 31536000000;
        public virtual decimal Distance => GetDistance();
        public virtual decimal Inclination { get; set; } = 0M;

        //Simplified
        private decimal GetDistance()
        {
            return this.LocalX;
        }

        protected (decimal x, decimal y) GetPosition()
        {
            return Util.InclinedOrbit(Parent.Mass, Mass, Math.Abs((double)Distance*1000), (double)CurrentTime, (double)Inclination);
        }

        public override void Update(decimal deltaTime)
        {
            CurrentTime += deltaTime;
            var pos = GetPosition();
            X = Parent.X + pos.x/1000;
            Y = Parent.Y + pos.y/1000;
        }

        public override GameGraphicalEntity GenerateGraphicalEntity()
        {
            var entity = new CircleEntity();
            entity.GameEntity = this;
            this.GraphicalEntity = entity;
            return entity;
        }
    }
}
