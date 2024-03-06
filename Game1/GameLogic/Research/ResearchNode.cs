using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Game1.GameLogic.Research
{
    public class ResearchNode
    {
        //Meta variables
        public string ResearchId { get; set; } = string.Empty;
        public string FriendlyName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public float Cost { get; set; } = 0;

        //UI Stuff
        [JsonIgnore]
        public bool Collapsed { get; set; } = true;
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;

        [JsonIgnore]
        public List<ResearchNode> Requisites => GameState.ResearchNodes.Where(x => RequisiteIds.Contains(x.ResearchId)).ToList();

        public List<string> RequisiteIds { get; set; } = new ();
        public ResearchType ResearchType { get; set; }

        //Instance variables
        public bool Researched { get; set; } = false;
        public float Progress { get; set; } = 0f;
    }
}
