using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Game1.GameLogic
{
    public abstract class AnimatableBase
    {
        public bool IsActiveEntity { get; set; } = true;

        public void Animate()
        {
            Task.Factory.StartNew(() =>
            {
                var id = Guid.NewGuid();
                GameEngine.Workers[id] = false;

                while (IsActiveEntity)
                {
                    while (!GameEngine.Synced)
                    {
                        Thread.Yield();
                    }

                    this.Update(GameEngine.TimeSenseLastUpdate);

                    GameEngine.Workers[id] = true;
                }
            });
        }

        public abstract void Update(double deltaTime);
    }
}
