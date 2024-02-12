using Autofac.Features.Metadata;
using Game1.GameLogic;
using Game1.GraphicalEntities;
using Game1.Graphics;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;
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
        public Vector2 Tail { get; set; } = Vector2.Zero;

        public long Fuel => Members.Select(x => x.Fuel).Sum();
        public long MaxFuel => Members.Select(x => x.MaxFuel).Sum();

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

        public long GetMaxThrust()
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

        public override void Update(decimal deltaTime)
        {
            UpdateOrders(deltaTime);

            UpdateMovement(deltaTime);

            if (GameState.Focus == this)
                Game.Services.GetService<Camera>().Position = (X, Y);
        }

        private void UpdateOrders(decimal deltaTime)
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

        private void UpdateMovement(decimal timePassed)
        {
            //var vx =(float)(this.GetMaxThrust() * Math.Sin(Direction));
            //var vy =(float)(this.GetMaxThrust() * Math.Cos(Direction));

            //this.Velocity += new Vector2(vx, vy);

            this.X += (decimal)Velocity.X * timePassed;
            this.Y += (decimal)Velocity.Y * timePassed;

            Tail = GameState.GameSpeed / (float)timePassed * -1 * Velocity;
        }

        private void MoveToPosition(Order order, decimal deltaTime)
        {
            var pos = order.Position;
            var divx = pos.x - this.X;
            var divy = pos.y - this.Y;
            var div = (decimal)Math.Sqrt((double)((double)divx * (double)divx + (double)divy * (double)divy));

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
            this.GraphicalEntity = entity;

            return entity;
        }

        public void DrawTargetLine(SpriteBatch spriteBatch)
        {
            var color = Color.Red;
            if (!GameState.SelectedEntities.Contains(this))
            {
                _container.Visible = false;
                color = Color.Red * 0.5f;
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
            color.A = 150;
            var windowPos = Util.WindowPosition((X, Y));
            var vector = windowPos + (Tail) * (float)camera.Zoom;
            spriteBatch.DrawLine(windowPos, vector, color, 1f);
        }

        private void InitInfo()
        {

            _container = new RectangleRuntime();
            _container.Color = Color.Red * 0.5f;
            //_container.SetProperty("Red", 255);
            //_container.SetProperty("Green", 255);
            //_container.SetProperty("Blue", 255);
            //_container.SetProperty("Alpha", 150);

            _container.Visible = false;
            _container.Width = 1f;
            _container.Height = 1f;
            _container.WidthUnits = Gum.DataTypes.DimensionUnitType.RelativeToChildren;
            _container.HeightUnits = Gum.DataTypes.DimensionUnitType.RelativeToChildren;
            _container.ChildrenLayout = Gum.Managers.ChildrenLayout.TopToBottomStack;

            _container.AddToManagers(SystemManagers.Default, null);

            var _subContainer = new ContainerRuntime();
            _subContainer.Width = 1f;
            _subContainer.Height = 1f;
            _subContainer.WidthUnits = Gum.DataTypes.DimensionUnitType.RelativeToChildren;
            _subContainer.HeightUnits = Gum.DataTypes.DimensionUnitType.RelativeToChildren;
            _subContainer.ChildrenLayout = Gum.Managers.ChildrenLayout.TopToBottomStack;

            _container.Children.Add(_subContainer);
        }

        private void UpdateInfo()
        {
            var pos = Util.WindowPosition((X, Y));

            _container.X = pos.X;
            _container.Y = pos.Y + 20;
            _container.Visible = true;

            var rect = _container.Children[0] as GraphicalUiElement;
            rect.Children.Clear();
            rect.Children.Add(Util.GetTextRuntime($"ID: {Name} ", 255, 0, 0, 200));
            rect.Children.Add(Util.GetTextRuntime($"~{Velocity.Length()} km/s ", 255, 0, 0, 200));
            //rect.Children.Add(Util.GetTextRuntime($"({X}, {Y}) ", 255, 0, 0, 150));
        }
    }
}
