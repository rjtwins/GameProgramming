namespace Game1.GameLogic
{
    public class BuildingQueueItem
    {
        public ColonyBuilding ColonyBuilding { get; set; }
        public double Amount { get; set; }
        public double Allocation { get; set; }
        public double Progress { get; set; }

        public bool Inf { get; set; } = false;
        public bool PauseOnCompletion { get; set; } = false;
        public bool Paused = false;

        /// <summary>
        /// Time to completion in days given an allocated amount of ic per day.
        /// </summary>
        /// <param name="ic"></param>
        /// <returns></returns>
        public double TimeToCompletion(double ic)
        {
            var totalIC = Amount * GameState.BuildingInfo[ColonyBuilding].IC;
            return (totalIC - Progress ) / ( ic * Allocation );
        }
    }
}