﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameLogic
{
    public class GasInfo
    {        
        //Works on base temp.
        public double GHF { get; set; } = 0f;
        public double AGHF { get; set; } = 0f;
    }

    public class BuildingInfo
    {
        //Works on base temp.
        public double IC { get; set; } = 0f;
        public double Pop { get; set; } = 0f;
        public double ICGen { get; set; } = 0f;
        public string FriendlyName { get; set; } = "";
        public Dictionary<Resource, double> BuildCost { get; set; } = new Dictionary<Resource, double>();
    }
}