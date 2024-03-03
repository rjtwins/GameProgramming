using System.Collections.Generic;

namespace Game1.GameLogic.Research
{
    public class ResearchNode
    {
        //Meta variables
        public string ResearchId { get; set; } = string.Empty;
        public string FriendlyName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public List<ResearchNode> Requisites { get; set; } = new ();
        public ResearchType ResearchType { get; set; }

        //Instance variables
        public bool Researched { get; set; } = false;
        public float Progress { get; set; } = 0f;
    }
}
