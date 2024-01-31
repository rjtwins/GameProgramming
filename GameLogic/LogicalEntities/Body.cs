using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic.LogicalEntities
{
    public class Body : WorldEntity
    {
        public int Mass { get; set; }
        public List<Body> Children { get; set; } = new List<Body>();
    }
}
