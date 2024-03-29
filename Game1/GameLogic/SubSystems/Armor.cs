﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameLogic.SubSystems
{
    internal class Armor : SubSystemBase
    {
        public override SubSystemType SubsystemType => SubSystemType.Armor;
        public string ArmorTypeName { get; set; }
        public ArmorType ArmorType { get; set; }

        //Weight per ton/m3
        public double Weight { get; set; } = 100;

        //Dept in m
        public double Depth { get; set; } = 0.001;

        public override string Report()
        {
            var reportString = string.Empty;

            reportString += $"TYPE: {ArmorType}\n";
            reportString += $"WEIGHT: {(Weight/1000).ToString("0.000")} KG/CM3\n";
            reportString += $"AMOUNT: {(Depth/1000).ToString("00000")} CM\n";

            return reportString;
        }
    }
}
