using Game1.GraphicalEntities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameEntities
{
    internal class Moon : BodyBase
    {
        public override GameGraphicalEntity GenerateGraphicalEntity(Game game)
        {
            var entity = new CircleEntity(game);
            entity.GameEntity = this;
            return entity;
        }
    }
}
