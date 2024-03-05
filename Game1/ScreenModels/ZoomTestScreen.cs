using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;
using GumRuntime;
using Microsoft.Xna.Framework;
using MonoGameGum.GueDeriving;
using RenderingLibrary;
using RenderingLibrary.Graphics;
using System;
using System.Linq;
using System.Timers;

namespace Game1.ScreenModels
{
    public class ZoomTestScreen : ScreenBase
    {
        public static ZoomTestScreen Instance { get; private set; }

        private Layer _layer;

        private GraphicalUiElement container;

        private ComponentSave _researchNodeSave = ObjectFinder
                .Self
                .GumProjectSave
                .Components
                .First(item => item.Name == "ResearchNode");

        public ZoomTestScreen()
        {
            Instance = this;

            _layer = SystemManagers.Default.Renderer.AddLayer();
            var layerCameraSettings = new LayerCameraSettings();
            layerCameraSettings.Zoom = 1;
            _layer.LayerCameraSettings = layerCameraSettings;

            Screen = GlobalStatic.GumProject.Screens.First(x => x.Name == "ZoomTestScreen").ToGraphicalUiElement(SystemManagers.Default, true);
            container = Screen.GetGraphicalUiElementByName("ContainerInstance");
            container.MoveToLayer(_layer);
        }

        public override void Show()
        {
            var rectRuntime = new RectangleRuntime();
            rectRuntime.Width = 100;
            rectRuntime.Height = 100;
            rectRuntime.X = 250;
            rectRuntime.Y = 250;
            rectRuntime.Color = Color.Red;
            rectRuntime.AddToManagers(SystemManagers.Default, _layer);
            rectRuntime.MoveToLayer(_layer);

            var testElement = _researchNodeSave.ToGraphicalUiElement(SystemManagers.Default, true);
            testElement.MoveToLayer(_layer);

            base.Show();

            var timer = new Timer(250);
            timer.AutoReset = true;
            timer.Elapsed += SetZoom;
            timer.Start();
        }

        private void SetZoom(object sender, ElapsedEventArgs e)
        {
            _layer.LayerCameraSettings.Zoom -= 0.01f;
        }
    }
}
