using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameEntities
{
    public class SolarSystem : BodyBase
    {
        public double Radius = GlobalStatic.SYSTEMSIZE;

        public List<Planet> Planets { get; set; } = new();
        public List<Star> Stars { get; set; } = new();
        public SolarSystem() 
        {
            
        }
    }
}
