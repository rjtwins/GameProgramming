using Game1.Input;
using Gum.Managers;
using Gum.Wireframe;
using GumRuntime;
using RenderingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.ScreenModels
{
    public class Main : ScreenBase
    {
        public static Main Instance { get; private set; }
        public GraphicalUiElement Speed, Year, Day, Time, TopBar, ShipDesignButton;

        public Main()
        {
            TopBar = ObjectFinder.Self.GumProjectSave.Components.First(x => x.Name == "TopBar").ToGraphicalUiElement(SystemManagers.Default, true);

            Screen = GlobalStatic.GumProject.Screens.First(x => x.Name == "Main").ToGraphicalUiElement(SystemManagers.Default, true);
            Speed = TopBar.GetGraphicalUiElementByName("ContainerInstance.GameSpeed");
            var gameDateContainer = TopBar.GetGraphicalUiElementByName("ContainerInstance.GameDateContainer");
            Year = gameDateContainer.GetGraphicalUiElementByName("Year");
            Day = gameDateContainer.GetGraphicalUiElementByName("Day");
            Time = gameDateContainer.GetGraphicalUiElementByName("Time");
            ShipDesignButton = TopBar.GetGraphicalUiElementByName("ContainerInstance.Button1");

            //Speed.AddToManagers(SystemManagers.Default, null);
            //Year.AddToManagers(SystemManagers.Default, null);
            //Day.AddToManagers(SystemManagers.Default, null);
            //Time.AddToManagers(SystemManagers.Default, null);

            UIClickEventHandler.Instance.AddElement(ShipDesignButton, () => {
                if (Main.Instance.Active)
                {
                    Main.Instance.Active = false;
                    Main.Instance.Screen.RemoveFromManagers();
                    ShipDesign.Instance.Screen.AddToManagers(SystemManagers.Default, null);
                    ShipDesign.Instance.Screen.Visible = true;
                }
                else
                {
                    Main.Instance.Active = true;
                    Main.Instance.Screen.AddToManagers(SystemManagers.Default, null);
                    ShipDesign.Instance.Screen.RemoveFromManagers();
                    ShipDesign.Instance.Screen.Visible = false;
                }
            });

            Instance = this;
            Main.Instance.Active = true;
        }

        public void Update()
        {
            var dateSpan = TimeSpan.FromSeconds(GameState.TotalSeconds);
            var speedSpan = TimeSpan.FromSeconds(GameState.GameSpeed);
            var speedString = speedSpan.TotalSeconds + " s/s";

            Year.SetProperty("Text", $"YEAR: {((int)Math.Floor((double)dateSpan.Days / 365)).ToString("00000")}");
            Day.SetProperty("Text", $"DAY: {(dateSpan.Days % 365).ToString("000")}");
            Time.SetProperty("Text", $"{dateSpan.Hours.ToString("00")}:{dateSpan.Minutes.ToString("00")}:{dateSpan.Seconds.ToString("00")}");
            Speed.SetProperty("Text", speedString);
        }

        public override void UpdateResolution()
        {
            base.UpdateResolution();
            //TopBar.Width = GlobalStatic.Width;
            TopBar.UpdateLayout();
            //TopBar.UpdateWidth(GlobalStatic.Width, false);
        }

    }
}
