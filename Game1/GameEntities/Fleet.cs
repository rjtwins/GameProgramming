using Game1.GameLogic;
using Game1.GameLogic.SubSystems;
using Game1.GraphicalEntities;
using Gum.Managers;
using GumRuntime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using RenderingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Game1.Extensions.SpriteBatchExtensions;
using Camera = Game1.Graphics.Camera;

namespace Game1.GameEntities
{
    //Can have stations or ships in it.
    public class Fleet : GameEntity, ICloneable
    {
        public GameEntity SOIEntity { get; set; } = null;
        public virtual Guid SOIGuid
        {
            get => SOIEntity?.Guid ?? Guid.Empty;
            set
            {

            }
        }

        public List<SubGameEntity> Members { get; set; } = new List<SubGameEntity>();
        public List<Order> Orders { get; set; } = new List<Order>();

        public Order CurrentOrder { get; set; }

        private Game Game = GlobalStatic.Game;
        private Camera _camera;

        public bool Military { get; set; } = false;

        public List<(decimal x, decimal y, double time, FleetGhost)> FleetGhosts { get; set; } = new();
        public HashSet<double> FleetGhostTimes { get; set; } = new();

        //For drawing sensor circles.
        public List<(string label, double radius, Sensor sensor)> ActiveSensors { get; set; } = new();
        public List<(string label, double radius, Sensor sensor)> OffSensors { get; set; } = new();
        public List<Sensor> AllSensors => Members.OfType<ShipInstance>().SelectMany(x => x.SubSystems.OfType<Sensor>()).ToList();

        public Vector2 Velocity { get; set; } = Vector2.Zero;
        public Vector2 Tail { get; set; } = Vector2.Zero;

        public double Fuel => Members.Sum(x => x.Fuel);
        public double MaxFuel => Members.Sum(x => x.MaxFuel);

        //public long MaxThrust => GetMaxThrust();
        public long CurrentThrust { get; set; } = 0;
        public float Direction
        {
            get
            {
                return Angle;
            }
            set
            {
                Angle = value;
            }
        }

        public double GetMaxThrust()
        {
            //Something is wrong we cannot have an empty fleet :(
            if (this.Members.Count() == 0)
                return 0;

            //if (this.Members.Any(x => x is Station))
            //    return 0;

            return this.Members
                .OfType<ShipDesign>()
                .Select(x => x.MaxThrust)
                .Max();
        }

        public override void Update(double deltaTime)
        {
            //Debug.WriteLine($"{this.Name} updating DT {deltaTime}");
            UpdateSensor();

            UpdateOrders(deltaTime);

            UpdateMovement(deltaTime);

            this.Members.OfType<ShipInstance>().ToList().ForEach(x => x.Update(deltaTime));

            if (GameState.Focus == this)
                Game.Services.GetService<Camera>().Position = (X, Y);

            if (GameState.SelectedEntities.Contains(this))
                UpdateInfo();
            else
                _infoContainer.Visible = false;
        }

        public void CreateGhost()
        {
            var ghost = this.Clone() as FleetGhost;
            ghost.SOIGuid = SOIGuid;
            var time = (int)GameState.TotalSeconds;
            FleetGhosts.Add((X, Y, time, ghost));
            FleetGhostTimes.Add(time);

            //Keep record of last 120 sec;
            if (FleetGhosts.Count > 60000)
            {
                var toRemove = FleetGhosts[0];
                FleetGhosts.RemoveAt(0);
                FleetGhostTimes.Remove(toRemove.time);
            }
        }

        private void UpdateOrders(double deltaTime)
        {
            if (CurrentOrder == null && Orders.Count() == 0)
                return;

            if (CurrentOrder == null)
            {
                CurrentOrder = Orders[0];
                Orders.RemoveAt(0);
            }

            switch (CurrentOrder.OrderType)
            {
                case OrderType.Stop:
                    break;
                case OrderType.Orbit:
                    break;
                case OrderType.InterceptObject:
                    break;
                case OrderType.Dock:
                    break;
                case OrderType.MoveTo:
                    MoveToPosition(CurrentOrder, deltaTime);
                    break;
                default:
                    break;
            }
        }

        private void UpdateMovement(double timePassed)
        {
            //var vx =(float)(this.GetMaxThrust() * Math.Sin(Direction));
            //var vy =(float)(this.GetMaxThrust() * Math.Cos(Direction));

            //this.Velocity += new Vector2(vx, vy);

            this.X += (decimal)(Velocity.X * timePassed);
            this.Y += (decimal)(Velocity.Y * timePassed);

            Tail = GameState.GameSpeed / (float)timePassed * -1 * Velocity;
        }

        private void MoveToPosition(Order order, double deltaTime)
        {
            var pos = order.Position;
            var divx = pos.x - this.X;
            var divy = pos.y - this.Y;
            var div = Math.Sqrt((double)((double)divx * (double)divx + (double)divy * (double)divy));

            bool orderCompleted = false;
            orderCompleted = div < 100;
            orderCompleted = div - GetMaxThrust() * deltaTime < 100;
            //Order completed
            if (orderCompleted)
            {
                CurrentOrder = null;
                Velocity = Vector2.Zero;
                X = pos.x; Y = pos.y;

                if (Orders.Count() == 0)
                    return;

                this.CurrentOrder = this.Orders[0];
                this.Orders.RemoveAt(0);

                return;
            }

            var angle = Util.AngleBetweenPoints(new decimal[] { pos.x, pos.y }, new decimal[] { this.X, this.Y }) + (decimal)Math.PI;
            this.Direction = (float)angle;
            var velocity_x = GetMaxThrust() * Math.Cos((double)angle);
            var velocity_y = GetMaxThrust() * Math.Sin((double)angle);
            Velocity = new Vector2((float)velocity_x, (float)velocity_y);
        }

        public override GameGraphicalEntity GenerateGraphicalEntity()
        {
            InitInfo();

            var entity = new CircleEntity();
            //Vector2[] vectors = new Vector2[4];
            //vectors[0] = new Vector2(50, 0);
            //vectors[0] = new Vector2(0, 50);
            //vectors[0] = new Vector2(0, -50);
            //vectors[0] = new Vector2(50, 0);

            //var entity = new PolyEntity(vectors);
            entity.LineWidth = 1f;
            Radius = 5M;
            Color = Color.Red;
            entity.ShouldDrawLabel = false;
            entity.GameEntity = this;
            entity.FixedSize = true;
            GraphicalEntity = entity;
            _camera = Game.Services.GetService<Camera>();

            return entity;
        }

        public void DrawTargetLine(SpriteBatch spriteBatch)
        {
            var pos = Util.WindowPosToGumPos(Util.WindowPosition((X, Y)));
            _infoContainer.X = pos.X;
            _infoContainer.Y = pos.Y + 20;

            var color = Color.Red;
            if (!GameState.SelectedEntities.Contains(this))
            {
                if (_infoContainer == null)
                    return;

                _infoContainer.Visible = false;
                color = Color.Red * 0.5f;
            }

            if (CurrentOrder == null || (CurrentOrder?.Position.x == 0 && this.CurrentOrder?.Position.y == 0))
                return;

            var windowPos = Util.WindowPosition(this.CurrentOrder.Position);
            spriteBatch.DrawLine(Util.WindowPosition(GraphicalEntity.Position), windowPos, color, 1f);

            var oldOrder = this.CurrentOrder;
            Orders.ForEach(x =>
            {
                var prevPos = Util.WindowPosition(oldOrder.Position);
                var newPos = Util.WindowPosition(x.Position);
                spriteBatch.DrawLine(prevPos, newPos, color, 1f);
                oldOrder = x;
            });
        }

        public void DrawVelocityVector(SpriteBatch spriteBatch)
        {
            var camera = Game.Services.GetService<Camera>();
            var color = Color.Green;
            color.A = 150;
            var windowPos = Util.WindowPosition((X, Y));
            var vector = windowPos + (Tail) * (float)camera.Zoom;
            spriteBatch.DrawLine(windowPos, vector, color, 1f);
        }

        public void DrawSensors(SpriteBatch spriteBatch)
        {            
            ActiveSensors.ForEach(x =>
            {
                var pos = Util.WindowPosition((X, Y));
                spriteBatch.DrawString(GlobalStatic.MainFont, x.label, new Vector2(pos.X, pos.Y - (float)x.radius * (float)_camera.Zoom - 30), Color.White);
                spriteBatch.DrawDashedCircle(pos, x.radius * (double)_camera.Zoom, 100, Color.Red);
            });

            OffSensors.ForEach(x =>
            {
                var pos = Util.WindowPosition((X, Y));
                spriteBatch.DrawDottedCircle(pos, x.radius * (double)_camera.Zoom, 100, Color.Red);
            });
        }

        private void InitInfo()
        {
            _infoContainer = ObjectFinder.Self.GumProjectSave.Components.First(x => x.Name == "FleetInfo").ToGraphicalUiElement(SystemManagers.Default, true);
            _infoContainer.Visible = false;
        }

        private void UpdateInfo()
        {
            var orderText = CurrentOrder?.Label ?? CurrentOrder?.OrderType.ToString();
            _infoContainer.Visible = true;
            _infoContainer.SetProperty("CivMillText", "");
            _infoContainer.SetProperty("NameText", $"{Name}");
            _infoContainer.SetProperty("SpeedText", $"{Velocity.Length()}km/s");
            _infoContainer.SetProperty("CargoText", $"0t");
            _infoContainer.SetProperty("FuelText", $"FUEL: {(int)(Fuel/MaxFuel * 100)}% ... todo burn time.");
            _infoContainer.SetProperty("OrderText", $"{orderText}");
            _infoContainer.SetProperty("StoresText", $"TODO");
        }

        private void UpdateSensor()
        {
            //Get all unique sensor instances that are on.
            var sensors = this.Members
                .OfType<ShipInstance>()
                .SelectMany(x => x.SubSystems.OfType<Sensor>().Where(x => x.IsOn).ToList())
                .GroupBy(x => x.DesignGuid)
                .Select(x => x.FirstOrDefault())
                .Where(x => x != null)
                .ToList();

            var sensors2 = this.Members
                .OfType<ShipInstance>()
                .SelectMany(x => x.SubSystems.OfType<Sensor>().Where(x => !x.IsOn).ToList())
                .GroupBy(x => x.DesignGuid)
                .Select(x => x.FirstOrDefault())
                .Where(x => x != null)
                .ToList();


            ActiveSensors = sensors.Select(x => ($"{x.Name}:{x.Resolution}@{x.Range}", x.Range, x)).ToList();
            OffSensors = sensors2.Select(x => ($"{x.Name}:{x.Resolution}@{x.Range}", x.Range, x)).ToList();
        }

        public object Clone()
        {
            var clone = new FleetGhost()
            {
                Name = Name,
                X = X,
                Y = Y,
                Velocity = Velocity
            };

            clone.Members = Members.OfType<ShipDesign>().
                Select(x => x.Clone())
                .OfType<SubGameEntity>()
                .ToList();

            return clone;
        }

        public FleetGhost GetClosestGhost(double time)
        {
            lock (FleetGhosts)
            {
                lock (FleetGhostTimes)
                {
                    var fleetGhosts = FleetGhosts.ToArray();
                    var stop = FleetGhostTimes.Count;
                    double[] ghostTimes = new double[stop];
                    FleetGhostTimes.CopyTo(ghostTimes, 0, stop);

                    if (fleetGhosts.Length == 0 || ghostTimes.Length == 0)
                        return null;

                    if (ghostTimes.Contains(time))
                        return fleetGhosts.FirstOrDefault(x => x.time == time).Item4;

                    time = Util.FindClosestInteger(ghostTimes.ToList(), time);
                    var fleetGhost = fleetGhosts.FirstOrDefault(x => x.time == time);

                    return fleetGhost.Item4;
                }
            }
        }
    }
}
