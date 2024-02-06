using Game1.GraphicalEntities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameEntities
{
    public class Star : GameEntity
    {
        public Star()
        {
            Color = Color.Yellow;
        }

        public override GameGraphicalEntity GenerateGraphicalEntity(Game game)
        {
            var entity = new CircleEntity(game);
            entity.GameEntity = this;
            this.GraphicalEntity = entity;
            return entity;
        }
    }
}
