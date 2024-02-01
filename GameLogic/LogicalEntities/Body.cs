using Flat.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic.LogicalEntities
{
    public class Body : WorldEntity<CircleEntity>
    {
        public int Mass { get; set; }
        public List<Body> Children { get; set; } = new List<Body>();

        public float Radius = 0f;
    }
}
