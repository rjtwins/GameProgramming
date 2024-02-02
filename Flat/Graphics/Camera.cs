using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Flat.Graphics
{
    public sealed class Camera
    {
        public readonly static float MinZ = 1f;
        public readonly static float MaxZ = float.MaxValue;

        public readonly static float MinZoom = 1f;
        public float MaxZoom = int.MaxValue;

        private Vector2 position;
        private float baseZ;
        private float z;

        private float aspectRatio;
        private float fieldOfView;

        private Matrix view;
        private Matrix proj;

        public float Zoom { get; set; }

        public Vector2 Position
        {
            get { return this.position; }
        }

        public float BaseZ
        {
            get { return this.baseZ; }
        }

        public float Z
        {
            get { return this.z; }
        }

        public Matrix View
        {
            get { return this.view; }
        }

        public Matrix Projection
        {
            get { return this.proj; }
        }

        public Camera(Screen screen)
        {
            if(screen is null)
            {
                throw new ArgumentNullException("screen");
            }

            this.aspectRatio = (float)screen.Width / screen.Height;
            this.fieldOfView = MathHelper.PiOver2;

            this.position = new Vector2(0, 0);
            this.baseZ = this.GetZFromHeight(screen.Height);
            this.z = this.baseZ;
            this.MaxZoom = baseZ;

            this.UpdateMatrices();

            this.Zoom = 1;
        }

        public void UpdateMatrices()
        {
            this.view = Matrix.CreateLookAt(new Vector3(this.position.X, this.position.Y, this.z), new Vector3(this.position.X, this.position.Y, 0), Vector3.Up);
            this.proj = Matrix.CreatePerspectiveFieldOfView(this.fieldOfView, this.aspectRatio, Camera.MinZ, Camera.MaxZ);
        }

        public float GetZFromHeight(float height)
        {
            return (0.5f * height) / MathF.Tan(0.5f * this.fieldOfView);
        }

        public float GetHeightFromZ()
        {
            return this.z * MathF.Tan(0.5f * this.fieldOfView) * 2f;
        }

        //public void MoveZ(float amount)
        //{
        //    this.z += amount;
        //    this.z = Util.Clamp(this.z, Camera.MinZ, Camera.MaxZ);
        //}

        public void ResetZ()
        {
            this.z = this.baseZ;
        }

        public void Move(Vector2 amount)
        {
            this.position += amount;
        }

        public void MoveTo(Vector2 position)
        {
            this.position = position;
        }

        public void IncZoom()
        {
            //this.Zoom *= 1.2f;
            //this.Zoom = Util.Clamp(this.Zoom, MinZoom, MaxZoom);
            //this.z = this.baseZ / this.Zoom;

            this.z *= 0.8f;
            this.z = Math.Max(this.z, 100);

            this.Zoom = this.baseZ / this.z;
            //System.Diagnostics.Debug.WriteLine(this.Zoom);
            System.Diagnostics.Debug.WriteLine(this.z);
        }

        public void DecZoom()
        {
            //this.Zoom *= 0.8f;
            //this.Zoom = Util.Clamp(this.Zoom, MinZoom, MaxZoom);
            //this.z = this.baseZ / this.Zoom;

            this.z *= 1.2f;
            this.Zoom = this.baseZ / this.z;

            //System.Diagnostics.Debug.WriteLine(this.Zoom);
        }

        public void IncZoomToPos(Vector2 pos)
        {
            IncZoom();
            this.Move(pos * 0.1f);
        }

        public void DecZoomToPos(Vector2 pos)
        {
            DecZoom();
            this.Move(pos * -0.1f);
        }



        public void SetZoom(int amount)
        {
            this.Zoom = amount;
            this.Zoom = Util.Clamp(this.Zoom, MinZoom, MaxZoom);
            this.z = this.baseZ / this.Zoom;
        }

        public void GetExtents(out float width, out float height)
        {
            height = this.GetHeightFromZ();
            width = height * this.aspectRatio;
        }

        public void GetExtents(out float left, out float right, out float bottom, out float top)
        {
            this.GetExtents(out float width, out float height);

            left = this.position.X - width * 0.5f;
            right = left + width;
            bottom = this.position.Y - height * 0.5f;
            top = bottom + height;
        }

        public void GetExtents(out Vector2 min, out Vector2 max)
        {
            this.GetExtents(out float left, out float right, out float bottom, out float top);
            min = new Vector2(left, bottom);
            max = new Vector2(right, top);
        }

    }
}
