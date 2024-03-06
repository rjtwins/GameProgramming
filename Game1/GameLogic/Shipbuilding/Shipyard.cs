using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameLogic.Shipbuilding
{
    public class Shipyard
    {
        public Colony Colony { get; set; }

        public string Name { get; set; } = string.Empty;
        public Guid Guid { get; } = Guid.NewGuid();
        public List<Slipway> Slipways { get; set; } = new();

        public Shipyard(Colony colony)
        {
            if (Slipways.Count == 0)
                Slipways.Add(new Slipway(this));

            Colony = colony;
        }

        public void Update(double timePassed)
        {
            Slipways.ForEach(x => x.Update(timePassed));
        }
    }
}
