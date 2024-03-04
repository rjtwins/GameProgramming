using Game1.Extensions;
using Game1.GameLogic;
using Game1.GameLogic.Research;
using Game1.Input;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;
using GumRuntime;
using Microsoft.Xna.Framework;
using MonoGameGum.GueDeriving;
using RenderingLibrary;
using RenderingLibrary.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Game1.ScreenModels
{
    public class Research : ScreenBase
    {
        public static Research Instance { get; private set; }

        private ComponentSave _researchNodeSave = ObjectFinder
            .Self
            .GumProjectSave
            .Components
            .First(item => item.Name == "ResearchNode");

        public Vector2 CursorStart { get; set; } = new Vector2(50f, 50f);
        public List<GraphicalUiElement> ResearchNodeElements { get; private set; } = new();
        public Dictionary<ResearchNode, List<GraphicalUiElement>> RequisiteLines { get; private set; } = new();
        GraphicalUiElement _researchContainer { get; set; }
        Layer _layer { get; set; } 

        public Research()
        {
            Instance = this;

            _layer = SystemManagers.Default.Renderer.AddLayer();
            var layerCameraSettings = new LayerCameraSettings();
            layerCameraSettings.Zoom = 1;
            layerCameraSettings.IsInScreenSpace = true;

            _layer.LayerCameraSettings = layerCameraSettings;
            Screen = GlobalStatic.GumProject.Screens.First(x => x.Name == "Research").ToGraphicalUiElement(SystemManagers.Default, true);
            _researchContainer = Screen.GetGraphicalUiElementByName("ResearchContainer");
            _researchContainer.MoveToLayer(_layer);
        }

        public override void Show()
        {
            base.Show();
            UpdateState();
            _researchContainer.Visible = true;
        }

        public override void Hide()
        {
            _researchContainer.Visible = false;
            base.Hide();
        }

        public override void Update(double deltaTime)
        {
            if (!Active)
                return;

            if (FlatMouse.Instance.IsMiddleButtonDown())
            {
                GameState.Focus = null;

                //var initPos = _researchContainer.GetPosition();
                var camera = SystemManagers.Default.Renderer.Camera;

                var divx = FlatMouse.Instance.MouseMovement().X;
                var divy = FlatMouse.Instance.MouseMovement().Y;

                var x = (divx * -1f * (1f / camera.Zoom));
                var y = (divy * (1f / camera.Zoom));

                Pan(x, y);

                //_camera.Position = (_camera.Position.x + x, _camera.Position.y + y);

                //Debug.WriteLine((divx, divy));
                //Debug.WriteLine($"init: {initPos}\nNew:{_camera.Position}");
            }

            Zoom();

            base.Update(deltaTime);
        }

        public void Pan(float divx, float divy)
        {
            var newx = Math.Max(_researchContainer.X + divx, -1920);
            var newy = Math.Max(_researchContainer.Y + divy, -1080);
            _researchContainer.X = newx;
            _researchContainer.Y = newy;
        }

        public void Zoom()
        {
            //Zoom:
            if (FlatMouse.Instance.ScrolledUp())
            {
                var zoom = _layer.LayerCameraSettings.Zoom.Value;
                zoom = MathF.Min(zoom * 1.2f, 2f);
                _layer.LayerCameraSettings.Zoom = zoom;
                Debug.WriteLine(_layer.LayerCameraSettings.Zoom);
            }

            if (FlatMouse.Instance.ScrolledDown())
            {
                var zoom = _layer.LayerCameraSettings.Zoom.Value;
                zoom = MathF.Max(zoom * 0.8f, 0.1f);

                _layer.LayerCameraSettings.Zoom = zoom;
                Debug.WriteLine(_layer.LayerCameraSettings.Zoom);
            }
        }

        public void UpdateState()
        {
            var nodes = GameState.ResearchNodes;
            var lines = Enum.GetValues<ResearchType>().ToList();

            float cursorX = CursorStart.X;
            float cursorY = CursorStart.Y;

            Screen.SuspendLayout(true);

            lines.ForEach(line =>
            {
                var lineNodes = nodes.Where(x => x.ResearchType == line).ToList();
                lineNodes.ForEach(lineNode =>
                {
                    var element = _researchNodeSave.ToGraphicalUiElement(SystemManagers.Default, true);
                    //element.MoveToLayer(_layer);
                    element.SetProperty("ResearchNameText", lineNode.FriendlyName);
                    element.Tag = lineNode;
                    element.X = cursorX;
                    element.Y = cursorY;
                    _researchContainer.Children.Add(element);
                    cursorX += element.GetAbsoluteWidth() + 20;
                    ResearchNodeElements.Add(element);
                });

                cursorX = CursorStart.X;
                cursorY += 100;
            });

            //Draw dependency lines:
            ResearchNodeElements.ForEach(element =>
            {
                var node = element.Tag as ResearchNode;
                var requisites = node.Requisites;
                var requisiteElements = this.ResearchNodeElements.Where(x => requisites.Contains(x.Tag)).ToList();

                List<GraphicalUiElement> requisiteLines = new();

                requisiteElements.ForEach(requisiteElement =>
                {
                    var pos1 = new System.Numerics.Vector2(element.X + (element.GetAbsoluteWidth() / 2), element.Y + (element.GetAbsoluteHeight() / 2));
                    var pos2 = new System.Numerics.Vector2(requisiteElement.X + (requisiteElement.GetAbsoluteWidth() / 2), requisiteElement.Y + (requisiteElement.GetAbsoluteHeight() / 2));

                    var polygon = new PolygonRuntime();
                    polygon.SetPoints(new System.Numerics.Vector2[]
                    {
                        pos1,
                        pos2,
                    });

                    polygon.Color = Color.Red;
                    polygon.IsDotted = false;

                    _researchContainer.Children.Add(polygon);
                    requisiteLines.Add(polygon);

                    //var left = (new List<GraphicalUiElement>() { element, requisiteElement }).MinBy(x => x.GetAbsoluteCenterX());
                    //var right = (new List<GraphicalUiElement>() { element, requisiteElement }).Where(x => x != left).First();

                    //var divx = right.GetAbsoluteCenterX() - left.GetAbsoluteCenterX();
                    //var divy = right.GetAbsoluteCenterY() - left.GetAbsoluteCenterY();

                    //var pos1 = new Vector2(left.GetAbsoluteCenterX(), left.GetAbsoluteCenterY());
                    //var pos2 = new Vector2(right.GetAbsoluteCenterX(), right.GetAbsoluteCenterY());

                    //float angle = Util.AngleBetweenPoints(pos1, pos2) * -1;
                    //float angleDeg = (float)(angle * (180.0f / (float)Math.PI));
                    //var rect = new ColoredRectangleRuntime();

                    //rect.X = left.X + (left.GetAbsoluteWidth() / 2);
                    //rect.Y = left.Y + (left.GetAbsoluteHeight() / 2) + 1;

                    //rect.Width = Math.Max(Math.Abs(divx), Math.Abs(divy));
                    //rect.Height = 2;

                    //rect.Color = Color.Red;

                    //rect.SetProperty("Rotation", angleDeg);
                    //_researchContainer.Children.Add(rect);

                    //requisiteLines.Add(rect);
                });

                RequisiteLines[node] = requisiteLines;
            });

            //SetupClickEvents
            ResearchNodeElements.ForEach(element =>
            {
                new InteractiveGUE(element).OnClick = () =>
                {
                    if (!(element.Tag is ResearchNode node))
                        return;

                    RequisiteLines.ToList().ForEach(pair =>
                    {
                        pair.Value.ForEach(line =>
                        {
                            line.SetProperty("Color", System.Drawing.Color.Red);
                        });
                    });

                    var lines = RequisiteLines[node];
                    lines.ForEach(line =>
                    {
                        line.SetProperty("Color", System.Drawing.Color.Green);
                    });
                };
            });

            Screen.ResumeLayout(true);
            Screen.UpdateLayout();
        }
    }
}
