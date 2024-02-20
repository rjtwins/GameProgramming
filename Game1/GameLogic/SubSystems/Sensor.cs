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
            var top =    "10K KM " + string.Join("---", t.Select(x => $"{(x.d / 10000).ToString("000000")}")) + "\n";
            var middle = "++++++ " + string.Join("|-----", t.Select(x => $"---")) + "\n";
            var bot = "{0}" + string.Join("---", t.Select(x => $"{x.r.ToString("000000")}")) + "\n\n";

            sensorString += top;
            sensorString += middle;
            sensorString += bot;

            switch (SensorType)
            {
                case SensorType.Active:
                    break;
                case SensorType.Optical:
                    sensorString = $"[Color=yellow]{sensorString}[/Color]";
                    sensorString = string.Format(sensorString, "M3     ");
                    break;
                case SensorType.EM:
                    sensorString = $"[Color=cyan]{sensorString}[/Color]";
                    sensorString = string.Format(sensorString, "KW     ");
                    break;
                case SensorType.IR:
                    sensorString = $"[Color=red]{sensorString}[/Color]";
                    sensorString = string.Format(sensorString, "KW     ");
                    break;
                default:
                    break;
            }

            return sensorString;
        }
    }
}
