using Autofac.Features.Metadata;
using Game1.GameLogic;
using Game1.GraphicalEntities;
using Game1.Graphics;
using Gum.Managers;
using GumRuntime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGameGum.GueDeriving;
using Newtonsoft.Json;
using RenderingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camera = Game1.Graphics.Camera;

namespace Game1.GameEntities
{
    //Can have stations or ships in it.
    public class Fleet : GameEntity
    {
        public GameEntity SOIEntity = null;
        public List<SubGameEntity> Members { get; set; } = new List<SubGameEntity>();
        public List<Order> Orders { get; set; } = new List<Order>();
        public Order CurrentOrder { get; set; }

        private Game Game = GlobalStatic.Game;

        public Vector2 Velocity { get; set; } = Vector2.Zero;

        public long Fuel => Members.Select(x => x.Fuel).Sum();
        public long MaxFuel => Members.Select(x => x.MaxFuel).Sum();
        public long MaxAcceleration => GetMaxAcceleration();

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

        private long GetMaxThrust()
        {
            //Something is wrong we cannot have an empty fleet :(
            if (this.Members.Count() == 0)
                return 0;

            //if (this.Members.Any(x => x is Station))
            //    return 0;

            return this.Members
                .Where(x => x is Ship)
                .Cast<Ship>()
                .Select(x => x.MaxThrust)
                .Max();
        }

        private long GetMaxAcceleration()
        {
            return this.Members.Where(x => x is Ship)
                .Cast<Ship>()
                .Select(x => x.Mass / x.Thrust)
                .Min();
        }

        public override void Update(double timePassed)
        {
            UpdateOrders();

            UpdateMovement(timePassed);
        }

        private void UpdateOrders()
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
                    MoveToPosition(CurrentOrder);
                    break;
                default:
                    break;
            }
        }

        private void UpdateMovement(double timePassed)
        {
            this.X += Velocity.X * timePassed;
            this.Y += Velocity.Y * timePassed;
        }

        private void MoveToPosition(Order order)
        {
            var pos = order.Position;
            var divx = pos.x - this.X;
            var divy = pos.y - this.Y;
            var div = Math.Sqrt(Math.Pow(divx, 2) + Math.Pow(divy, 2));

            //Order completed
            if (div < 1000)
            {
                CurrentOrder = null;

                if (Orders.Count() == 0)
                    return;

                this.CurrentOrder = this.Orders[0];
                this.Orders.RemoveAt(0);

                return;
            }

            var angle = Util.AngleBetweenPoints(new double[] { pos.x, pos.y }, new double[] { this.X, this.Y }) + Math.PI;
            this.Direction = (float)angle;
            var velocity_x = GetMaxThrust() * Math.Cos(angle);
            var velocity_y = GetMaxThrust() * Math.Sin(angle);
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
            Radius = 5d;
            Color = Color.Red;
            entity.GameEntity = this;
            entity.FixedSize = true;
            this.GraphicalEntity = entity;

            return entity;
        }

        public void DrawTargetLine(SpriteBatch spriteBatch)
        {
            var color = Color.Red;
            if (!GameState.SelectedEntities.Contains(this))
            {
                _container.Visible = false;
                color = Color.DarkGray;
            }
            else
            {
                UpdateInfo();
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
            var windowPos = Util.WindowPosition((X, Y));
            var vector = windowPos + (Velocity * -1) * (float)camera.Zoom;
            spriteBatch.DrawLine(windowPos, vector, color, 1f);
        }

        private void InitInfo()
        {
            //_container = new ContainerRuntime();
            //_container.ChildrenLayout = Gum.Managers.ChildrenLayout.TopToBottomStack;
            //_container.WidthUnits = Gum.DataTypes.DimensionUnitType.RelativeToChildren;
            //_container.HeightUnits = Gum.DataTypes.DimensionUnitType.RelativeToChildren;
            //_container.Height = 1f;
            //_container.Width = 1f;
            //_container.AddToManagers();

            var componentSave = ObjectFinder.Self.GumProjectSave.Components
                .First(item => item.Name == "RectComponent");

            _container = componentSave.ToGraphicalUiElement(SystemManagers.Default, addToManagers: true);
            //_container.Visible = false;
            _container.Width = 1f;
            _container.Height = 1f;
            _container.WidthUnits = Gum.DataTypes.DimensionUnitType.RelativeToChildren;
            _container.HeightUnits = Gum.DataTypes.DimensionUnitType.RelativeToChildren;
            _container.ChildrenLayout = Gum.Managers.ChildrenLayout.TopToBottomStack;
            _container.SetProperty("Red", 255);
            _container.SetProperty("Green", 0);
            _container.SetProperty("Blue", 0);

            //_container.Children.Add(rect);
        }

        private void UpdateInfo()
        {
            var rect = _container;
            rect.Children.Clear();
            var pos = Util.WindowPosition((X, Y + 20));
            _container.X = pos.X;
            _container.Y = pos.Y;
            var text = new TextRuntime();
            text.Text = $"({X}, {Y})";
            rect.Children.Add(text);
            _container.Visible = true;
        }
    }
}
