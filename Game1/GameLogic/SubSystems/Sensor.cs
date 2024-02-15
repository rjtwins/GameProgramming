using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameLogic.SubSystems
{
    public class Sensor : SubSystemBase
    {
        public override SubSystemType SubsystemType => SubSystemType.Sensor;
        public SensorType SensorType { get; set; } = SensorType.Active;
        public double Range {  get; set; }
        public double Resolution { get; set; }

        public double GetResolution(double range)
        {
            return (range / Range) * Resolution;
        }

        public (int r, int d)[] GetResolutionTable()
        {
            var steps = 8;
            var table = new (int, int)[steps];
            var stepSize = Range / steps;

            for (int i = 0; i < steps; i++)
            {
                table[i] = ((int)GetResolution(stepSize * (i + 1)), (int)(stepSize * (i + 1)));
            }

            return table;
        }

        public override string Report()
        {
            var reportString = base.Report();

            reportString += GetRangeTable();

            return reportString;
        }

        public string GetRangeTable()
        {
            string sensorString = $"SENSOR: {Resolution} M3 @ {Range} KM\n\n";

            var t = GetResolutionTable();
            //var top =    "10KM " + string.Join("---", t.Select(x => $"{x.d.ToString("0.0E+0")}")) + "\n";
            var top = "10K KM " + string.Join("---", t.Select(x => $"{(x.d / 10000).ToString("000000")}")) + "\n";
            var middle = "++++++ " + string.Join("|-----", t.Select(x => $"---")) + "\n";
            var bot = "M3     " + string.Join("---", t.Select(x => $"{x.r.ToString("000000")}")) + "\n\n";

            sensorString += top;
            sensorString += middle;
            sensorString += bot;

            return sensorString;
        }
    }
}
