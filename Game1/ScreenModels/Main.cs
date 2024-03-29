﻿using Game1.Extensions;
using Gum.Managers;
using Gum.Wireframe;
using GumRuntime;
using RenderingLibrary;
using System;
using System.Linq;

namespace Game1.ScreenModels
{
    public class Main : ScreenBase
    {
        public static Main Instance { get; private set; }
        public GraphicalUiElement Speed, Year, Day, Time, TopBar,
            ShipDesignButton,
            SystemListButton,
            ColonyManagerButton,
            ResearchButton;

        public Main()
        {
            var topBarLayer = SystemManagers.Default.Renderer.MainLayer;
            TopBar = ObjectFinder.Self.GumProjectSave.Components.First(x => x.Name == "TopBar").ToGraphicalUiElement(SystemManagers.Default, true);
            //TopBar.AddToManagers(SystemManagers.Default, topBarLayer);
            TopBar.Z = float.MaxValue;

            Screen = GlobalStatic.GumProject.Screens.First(x => x.Name == "Main").ToGraphicalUiElement(SystemManagers.Default, true);
            Speed = TopBar.GetGraphicalUiElementByName("ContainerInstance.GameSpeed");
            var gameDateContainer = TopBar.GetGraphicalUiElementByName("ContainerInstance.GameDateContainer");
            Year = gameDateContainer.GetGraphicalUiElementByName("Year");
            Day = gameDateContainer.GetGraphicalUiElementByName("Day");
            Time = gameDateContainer.GetGraphicalUiElementByName("Time");
            ShipDesignButton = TopBar.GetGraphicalUiElementByName("ContainerInstance.Button1");
            SystemListButton = TopBar.GetGraphicalUiElementByName("ContainerInstance.Button2");
            ColonyManagerButton = TopBar.GetGraphicalUiElementByName("ContainerInstance.Button3");
            ResearchButton = TopBar.GetGraphicalUiElementByName("ContainerInstance.Button4");

            new InteractiveGUE(ShipDesignButton).OnClick = () =>
            {
                if (ShipDesign.Instance.Active)
                {
                    ScreenManager.Screens.ForEach(x => x.Hide());
                    Main.Instance.Show();
                    return;
                }

                ScreenManager.Screens.ForEach(x => x.Hide());
                ShipDesign.Instance.Show();
            };

            new InteractiveGUE(SystemListButton).OnClick = () =>
            {
                if (PlanetScreen.Instance.Active)
                {
                    ScreenManager.Screens.ForEach(x => x.Hide());
                    Main.Instance.Show();
                    return;
                }

                ScreenManager.Screens.ForEach(x => x.Hide());
                PlanetScreen.Instance.Show();

            };

            new InteractiveGUE(ColonyManagerButton).OnClick = () =>
            {
                if (ColonyManager.Instance.Active)
                {
                    ScreenManager.Screens.ForEach(x => x.Hide());
                    Main.Instance.Show();
                    return;
                }

                ScreenManager.Screens.ForEach(x => x.Hide());
                ColonyManager.Instance.Show();

            };

            new InteractiveGUE(ResearchButton).OnClick = () =>
            {
                if (Research.Instance.Active)
                {
                    ScreenManager.Screens.ForEach(x => x.Hide());
                    Main.Instance.Show();
                    return;
                }

                ScreenManager.Screens.ForEach(x => x.Hide());
                Research.Instance.Show();
            };


            Instance = this;
            Main.Instance.Active = true;
        }

        public override void Update(double deltaTime)
        {
            var dateSpan = TimeSpan.FromSeconds(GameState.TotalSeconds);
            var speedSpan = TimeSpan.FromSeconds(GameState.GameSpeed);
            var speedString = speedSpan.TotalSeconds + " s/s";

            Year.SetProperty("Text", $"YEAR: {((int)Math.Floor((double)dateSpan.Days / 365)).ToString("00000")}");
            Day.SetProperty("Text", $"DAY: {(dateSpan.Days % 365).ToString("000")}");
            Time.SetProperty("Text", $"{dateSpan.Hours.ToString("00")}:{dateSpan.Minutes.ToString("00")}:{dateSpan.Seconds.ToString("00")}");
            Speed.SetProperty("Text", $"{Util.ConvertSpeed(GameState.GameSpeed)}/S");
            TopBar.SetProperty("SimResText", $"{Util.ConvertSpeed((int)GameEngine.TimeSinceLastUpdate)}");
        }

        public override void UpdateResolution()
        {
            base.UpdateResolution();
            //TopBar.Width = GlobalStatic.Width;
            TopBar.UpdateLayout();
            //TopBar.UpdateWidth(GlobalStatic.Width, false);
        }

        public void HideTopBar()
        {
            TopBar.Visible = false;
            TopBar.RemoveFromManagers();
        }

        public void ShowTopBar()
        {
            TopBar.Visible = true;
            TopBar.AddToManagers(SystemManagers.Default, null);
        }
    }
}
