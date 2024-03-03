using Game1.GameLogic;
using Game1.GameLogic.Research;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;
using GumRuntime;
using Microsoft.Xna.Framework;
using MonoGameGum.GueDeriving;
using RenderingLibrary;
using System;
using System.Collections.Generic;
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

        GraphicalUiElement _researchContainer;
        public Research()
        {
            Instance = this;

            Screen = GlobalStatic.GumProject.Screens.First(x => x.Name == "Research").ToGraphicalUiElement(SystemManagers.Default, true);
            _researchContainer = Screen.GetGraphicalUiElementByName("ResearchContainer");
        }

        public override void Show()
        {
            base.Show();
            UpdateState();
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
                    element.SetProperty("ResearchNameText", lineNode.FriendlyName);
                    element.Tag = lineNode;
                    element.X = cursorX;
                    element.Y = cursorY;
                    _researchContainer.Children.Add(element);
                    cursorX += element.GetAbsoluteWidth() + 20;
                    ResearchNodeElements.Add(element);
                });

                cursorY += 100;
            });

            //Draw dependency lines:
            ResearchNodeElements.ForEach(element =>
            {
                var node = element.Tag as ResearchNode;
                var requisites = node.Requisites;
                var requisiteElements = this.ResearchNodeElements.Where(x => requisites.Contains(x.Tag)).ToList();
                requisiteElements.ForEach(requisiteElement =>
                {
                    var divx = element.GetAbsoluteCenterX() - requisiteElement.GetAbsoluteCenterX();
                    var divy = element.GetAbsoluteCenterY() - requisiteElement.GetAbsoluteCenterY();

                    var angle = Util.AngleBetweenPoints(element.GetPosition(), requisiteElement.GetPosition());
                    var rect = new ColoredRectangleRuntime();

                    rect.X = element.GetAbsoluteCenterX();
                    rect.Y = element.GetAbsoluteCenterY();

                    rect.Width = divx;
                    rect.Height = 1;


                });
            });

            Screen.ResumeLayout(true);
            Screen.UpdateLayout();
        }
    }
}
