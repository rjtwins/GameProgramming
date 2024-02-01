using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flat.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace GameLogic.LogicalEntities
{
    public abstract class WorldEntity<GraphicalType> where GraphicalType : Entity
    {
        Guid id { get; set; }
        int x { get; set; }
        int y { get; set; }

        public virtual Type GetGraphicalType()
        {
            return typeof(GraphicalType);
        }
    }
}
