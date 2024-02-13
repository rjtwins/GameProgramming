using RenderingLibrary.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameLogic
{
    //Entities that are not on the map directly.
    public abstract class SubGameEntity
    {
        public Guid Guid { get; set; }

        //Crew in men
        public int Crew { get; set; }

        //In metric ton
        public long Mass { get; set; }

        //in kilo liters
        public long Fuel { get; set; }
        public long MaxFuel { get; set; }

        public virtual string ToJson()
        {
            var text = System.Text.Json.JsonSerializer.Serialize(this);
            Debug.WriteLine(text);
            return text;
        }

        public static T FromJson<T>(string json)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }
    }
}
