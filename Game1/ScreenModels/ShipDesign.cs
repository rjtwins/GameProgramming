using Game1.GameLogic;
using Game1.Input;
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
    public class ShipDesign : ScreenBase
    {
        public static ShipDesign Instance { get; private set; }

        public Ship ActiveDesign
        {
            get
            {
                return _activeDesign;
            }
            set
            {
                _activeDesign = value;
                ShipChanged();
            }
        }

        private Ship _activeDesign { get; set; }

        private GraphicalUiElement ShipStatBlock, 
            ScrollView, 
            ShipBasic,
            ShipCrew, 
            ShipProp, 
            ShipSensor, 
            ShipHold, 
            ShipEnergy, 
            ShipMisc;

        public ShipDesign()
        {
            Instance = this;
            Screen = GlobalStatic.GumProject.Screens.First(x => x.Name == "ShipDesigner").ToGraphicalUiElement(SystemManagers.Default, addToManagers: true);
            ScrollView = Screen
                .GetGraphicalUiElementByName("Right.ContainerInstance2")
                .GetGraphicalUiElementByName("ScrollView");

            ShipStatBlock = ScrollView.GetGraphicalUiElementByName("ShipStatBlock");

            ShipBasic = ShipStatBlock
                .GetGraphicalUiElementByName("Basic")
                .GetGraphicalUiElementByName("BodyText");

            ShipCrew = ShipStatBlock
                .GetGraphicalUiElementByName("Crew")
                .GetGraphicalUiElementByName("BodyText");

            ShipProp = ShipStatBlock
                .GetGraphicalUiElementByName("Prop")
                .GetGraphicalUiElementByName("BodyText");

            ShipSensor = ShipStatBlock
                .GetGraphicalUiElementByName("Sensor")
                .GetGraphicalUiElementByName("BodyText");

            ShipHold = ShipStatBlock
                .GetGraphicalUiElementByName("Hold")
                .GetGraphicalUiElementByName("BodyText");

            ShipEnergy = ShipStatBlock
                .GetGraphicalUiElementByName("Energy")
                .GetGraphicalUiElementByName("BodyText");

            ShipMisc = ShipStatBlock
                .GetGraphicalUiElementByName("Misc")
                .GetGraphicalUiElementByName("BodyText");

            UIScrollEventHandler.Instance.ScrollElements.Add(ScrollView);

            Screen.RemoveFromManagers();
        }

        public void ShipChanged()
        {
            string basicString = string.Empty;
            basicString += $"TONNAGE: {ActiveDesign.Mass}\n";
            basicString += $"FUEL: {ActiveDesign.MaxFuel}\n";
            basicString += $"MASS: {ActiveDesign.Mass}\n";
            basicString += $"MASS: {ActiveDesign.Mass}\n";
            basicString += $"MASS: {ActiveDesign.Mass}\n";
            basicString += $"MASS: {ActiveDesign.Mass}\n";
            basicString += $"MASS: {ActiveDesign.Mass}\n";
            basicString += $"MASS: {ActiveDesign.Mass}\n";
            this.ShipBasic.SetProperty("Text", basicString);
            ShipBasic.UpdateLayout();
        }

        public void DesignUpdated()
        {           
            ShipChanged();
        }
    }
}
