using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Game1.GameLogic.Research
{
    public class ResearchManager : AnimatableBase
    {
        public static ResearchManager Instance = new ResearchManager();

        public ResearchManager() 
        {
            Instance = this;
        }

        /// <summary>
        /// Updates all factions in the game state.
        /// </summary>
        /// <param name="deltaTime">The time that has passed since the last update.</param>
        public override void Update(double deltaTime)
        {
            foreach (var faction in GameState.Factions)
            {
                UpdateFaction(faction, deltaTime);
            }
        }


        /// <summary>
        /// Updates the faction based on the specified time interval.
        /// </summary>
        /// <param name="faction">The faction to update.</param>
        /// <param name="deltaTime">The time interval since the last update.</param>
        public void UpdateFaction(Faction faction, double deltaTime)
        {
            var current = faction.ResearchQueue.FirstOrDefault();
            if (current == null)
                return;

            // Get colonies belonging to the faction
            var colonies = GameState.Colonies.Where(x => x.Faction.Guid == faction.Guid).ToList();

            // Check if the faction has a research academy
            if (!colonies.Any(x => x.CurrentBuildings[ColonyBuilding.ResearchAcademy] == 1))
                return;

            // Convert deltaTime to days
            var daysPassed = deltaTime / 86400;

            // Calculate the points generated based on the number of research outposts and time passed
            var pointsGenerated = (colonies.Sum(x => x.CurrentBuildings[ColonyBuilding.ResearchOutpost]) + 10) * daysPassed;

            while(pointsGenerated > 0 && current != null)
            {
                var remaining = current.Cost - current.Progress;
                if(remaining < pointsGenerated)
                {
                    current.Progress = current.Cost;
                    current.Researched = true;
                    pointsGenerated -= remaining;

                    faction.ResearchQueue.Remove(current);
                    current = faction.ResearchQueue.FirstOrDefault();
                }
                else
                {
                    current.Progress += (float)pointsGenerated;
                    pointsGenerated = 0;
                }
            }
        }
    }
}
