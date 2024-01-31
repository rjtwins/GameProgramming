using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic.LogicalEntities
{
    public class WorldEntity
    {
        Guid id { get; set; }
        int x { get; set; }
        int y { get; set; }
    }
}
