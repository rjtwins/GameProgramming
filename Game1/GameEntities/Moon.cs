using Game1.GraphicalEntities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameEntities
{
    public class Moon : Orbital
    {
        public Moon() 
        {
            Color = Color.Gray;
        }

        public override GameGraphicalEntity GenerateGraphicalEntity()
        {
            var entity = base.GenerateGraphicalEntity();
            entity.MinSize = 0.1f;
            return entity;
        }
    }
}
