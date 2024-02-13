using Game1.GraphicalEntities;
using Game1.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameEntities
{
    public abstract class Orbital : GameEntity
    {
        //public virtual decimal CurrentTime { get; set; }
        public virtual float Distance { get; set; }
        public virtual float Eccentricity { get; set; } = 0f;
        public virtual float Period { get; set; } = 0f;
        public double TimeSeed = 0f;// (double)new Random().NextInt64(0, long.MaxValue);
        public double AngleSeed = (2 * Math.PI) / new Random().NextDouble();

        public double SemiMajorAxis { get; set; }
        public double AngularVelocity { get; set; }

        public Vector2[] OrbitalPoints = new Vector2[0];

        public Orbital()
        {

        }

        public void Init()
        {
            SemiMajorAxis = (Distance * 1000) / (1 + Eccentricity);

            // Calculate the gravitational parameter (μ)
            double mu = Util.G * (Mass + Parent.Mass);

            // Calculate the period of the orbit
            Period = (float)(2 * Math.PI * Math.Sqrt(Math.Pow(SemiMajorAxis, 3d) / mu));

            AngularVelocity = 2 * Math.PI / Period;

            CalculateOrbitPoints();
        }

        public void CalcPos()
        {
            SolarSystem system = GetParentSystem();
            if (!system.GraphicalEntity.InView())
            {
                return;
            }

            var pos = GlobalCoordinatesAtTime(GameState.TotalSeconds);

            this.X = pos.x;
            this.Y = pos.y;

            if (GameState.Focus == this)
                GraphicalEntity.Game.Services.GetService<Camera>().Position = (X, Y);
        }

        public override void Update(decimal deltaTime)
        {
            
        }

        public virtual void UpdateInView()
        {
            SolarSystem system = GetParentSystem();
            if (!system.GraphicalEntity.InView())
            {
                GraphicalEntity.IsInView = false;
                return;
            }

            GraphicalEntity.IsInView = GraphicalEntity.InView();
        }

        public override GameGraphicalEntity GenerateGraphicalEntity()
        {
            var entity = new CircleEntity();
            entity.GameEntity = this;
            this.GraphicalEntity = entity;
            return entity;
        }

        public virtual SolarSystem GetParentSystem()
        {
            GameEntity current = this;
            while (!(current is SolarSystem) && current != null)
                current = current.Parent as GameEntity;

            if(current is SolarSystem solarSystem)
                return solarSystem;

            return null;
        }

        //public (double x, double y) LocalCoordinatesAtTime(double time)
        //{
        //    // Calculate the current angle
        //    double currentAngle = AngularVelocity * time;
        //    currentAngle %= 2 * Math.PI;

        //    double r = SemiMajorAxis * (1 - Eccentricity * Eccentricity) / (1 + Eccentricity * Math.Cos(currentAngle));
        //    // Calculate the x and y coordinates of object A
        //    //double x = r * Math.Cos(currentAngle);
        //    //double y = r * Math.Sin(currentAngle);

        //    double x = r * Math.Cos(currentAngle + AngleSeed); // Offset by AngleSeed degrees to shift the orbit
        //    double y = r * Math.Sin(currentAngle + AngleSeed); // Offset by AngleSeed degrees to shift the orbit

        //    return (x / 1000, y / 1000);
        //}

        public (double x, double y) LocalCoordinatesAtTime(double time)
        {
            // Calculate the current angle using the initial angle and angular velocity
            double currentAngle = AngularVelocity * time;
            currentAngle %= 2 * Math.PI;

            // Compute the distance from the center (r) based on the semi-major axis
            double r = SemiMajorAxis * (1 - Eccentricity * Eccentricity) / (1 + Eccentricity * Math.Cos(currentAngle));

            // Calculate the x and y coordinates
            double x = r * Math.Cos(currentAngle + AngleSeed);
            double y = r * Math.Sin(currentAngle + AngleSeed);

            x += Math.Cos(AngleSeed) * (SemiMajorAxis + Distance * 500) * Eccentricity;
            y += Math.Sin(AngleSeed) * (SemiMajorAxis + Distance * 500) * Eccentricity;

            // Convert coordinates to kilometers (assuming SemiMajorAxis is in meters)
            return (x / 1000, y / 1000);
        }

        public (double x, double y) LocalCircularCoordinatesAtTime(double time)
        {
            double currentAngle = AngularVelocity * time;
            double x = Distance * 1000 * Math.Cos(currentAngle + AngleSeed);
            double y = Distance * 1000 * Math.Sin(currentAngle + AngleSeed);

            return (x, y);
        }

        public (decimal x, decimal y) GlobalCoordinatesAtTime(double time)
        {
            var local = LocalCoordinatesAtTime(time);
            var parentGlobal = this.Parent is Orbital o ? o.GlobalCoordinatesAtTime(time) : (Parent.X, Parent.Y);

            return (parentGlobal.Item1 + (decimal)local.x, parentGlobal.Item2 + (decimal)local.y);
        }

        public void CalculateOrbitPoints(int numSegments = 250)
        {
            OrbitalPoints = new Vector2[numSegments];

            // Calculate time span for one orbit
            double timeIncrement = Period / numSegments;

            for (int i = 0; i < numSegments; i++)
            {
                double time = i * timeIncrement;
                (double x, double y) = LocalCoordinatesAtTime(time);
                OrbitalPoints[i] = new Vector2((float)x, (float)y);
            }
        }
    }
}
