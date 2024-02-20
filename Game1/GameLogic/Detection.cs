using Game1.GameEntities;
using Game1.GameLogic.SubSystems;
using System;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace Game1.GameLogic
{
    public static class Detection
    {
        public static void Update()
        {
            var sensingFleets = GameState
                .GameEntities.OfType<Fleet>()
                .Where(x => x.AllSensors.Any(y => y.IsOn))
                .ToList();

            sensingFleets.ForEach(x => HandleFleet(x));

            GameState.Factions.ForEach(x =>
            {
                Debug.WriteLine($"Faction {x.Name} has contacts: {string.Join("-", x.SensorContacts.Select(y => y.ContactGhost.Name))}");
            });
        }

        public static void HandleFleet(Fleet fleet)
        {
            var time = (int)GameState.TotalSeconds;
            
            fleet.ActiveSensors.ForEach(x =>
            {

                //KM radius/(km/s)
                //If we are active we have twice the travel time and have to start farther back in time.
                var sec = x.sensor.SensorType == SensorType.Active ? x.radius / 300000d * 2 : x.radius / 300000d;
                int start = (int)(x.sensor.SensorType == SensorType.Active ? x.radius / 300000d : 0);
                sec = Math.Max(sec, 1);

                var hostileFleets = GameState.GameEntities.OfType<Fleet>()
                .Where(y => y.Faction != fleet.Faction).ToList();

                for (int i = start; i < sec; i++)
                {
                    var emissionTime = time - i;

                    //For the range of the sensor starting at 0 km and going back into time.
                    var ghosts = hostileFleets
                    .Select(y => y.GetClosestGhost(emissionTime))
                    .Where(y => y != null)
                    .Where(y => y.SOIGuid == fleet.SOIEntity.Guid)
                    .ToList();

                    ghosts.ForEach(y => {
                            if(GetDetection(x.sensor, fleet, y, emissionTime, out SensorContact contact))
                                fleet.Faction.SensorContacts.Add(contact);
                        });
                };
            });
        }

        public static bool GetDetection(Sensor sensor, Fleet fleet, FleetGhost fleetGhost, double emissionTime, out SensorContact contact)
        {
            contact = null;
            var distance = Util.Distance((fleet.X, fleet.Y), (fleetGhost.X, fleetGhost.Y));
            var res = sensor.Resolution * (double)(distance / (decimal)sensor.Range);
            var detected = false;
            var members = fleet.Members.OfType<ShipInstance>();

            switch (sensor.SensorType)
            {
                case SensorType.Active:
                case SensorType.Optical:
                    detected = res >= members.Sum(x => x.Volume);
                    break;
                case SensorType.EM:
                    detected = res >= members.Sum(x => x.CurrentEMSig);
                    break;
                case SensorType.IR:
                    detected = res >= members.Sum(x => x.CurrentThermalSig);
                    break;
                default:
                    break;
            }

            if (!detected)
                return false;

            contact = new SensorContact()
            {
                Contact = fleetGhost,
                ContactType = sensor.SensorType,
                ContactGhost = fleetGhost,
                GameTime = emissionTime,
                Label = "FUCK",
            };

            return detected;
        }
    }
}
