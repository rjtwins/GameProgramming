using System;
using System.Collections.Generic;
using System.Linq;

namespace Game1.GameLogic
{
    public class Atmos
    {
        public AtmosType AtmosType {  get; set; }

        public Dictionary<Gas, float> Gases = new();
        
        //In surface coverage.
        public float LiquidWater { get; set; } = 0f;
        //In surface coverage.
        public float Ice { get; set; } = 0f;
        
        public bool Frozen { get; set; } = false;

        public float AtmosPressure => Gases.Sum(x => x.Value);

        public Atmos()
        {
            //Setup
            Enum.GetValues<Gas>().ToList().ForEach(x => Gases[x] = 0f);
        }

        internal string Report()
        {
            if (Gases.Sum(x => x.Value) == 0)
                return string.Empty;

            var gases = Gases.Where(x => x.Value > 0f).ToList();
            string part1 = string.Join(",", gases.Select(x => x.Key.ToString()));
            //string part2 = string.Join(",", gases.Select(x => x.Value.ToString("00.0")));
            string part3 = AtmosPressure.ToString("000.0");

            return $"{part1} - {part3}";
        }
    }
}
