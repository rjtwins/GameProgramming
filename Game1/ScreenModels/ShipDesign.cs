using Gum.Wireframe;
using GumRuntime;
using MonoGame.Extended.Screens;
using RenderingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.ScreenModels
{
    public class ShipDesign
    {
        public static ShipDesign Instance { get; private set; }
        public GraphicalUiElement Screen { get; set; }

        public bool Active = false;

        public ShipDesign()
        {
            Instance = this;
            Screen = GlobalStatic.GumProject.Screens.First(x => x.Name == "ShipDesigner").ToGraphicalUiElement(SystemManagers.Default, addToManagers: true);
            Screen.RemoveFromManagers();
        }
    }
}
