using Game1.GameEntities;
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
    public class PlanetScreen : ScreenBase
    {
        public static PlanetScreen Instance { get; private set; }

        public List<Planet> ListedPlanets { get; set; } = new List<Planet>();
        public List<SolarSystem> ListedSystems { get; set; } = new List<SolarSystem>();

        public Planet SelectedPlanet { get; set; }
        public Star SelectedStar {  get; set; }

        private GraphicalUiElement _scrollContainer, _systemList;
        private bool _filled = false;

        public PlanetScreen()
        {
            Instance = this;
            ListedPlanets = GameState.Planets.ToArray().ToList(); //make a copy of the list;
            ListedSystems = GameState.SolarSystems.ToArray().ToList(); 

            Screen = GlobalStatic.GumProject.Screens.First(x => x.Name == "Planets").ToGraphicalUiElement(SystemManagers.Default, true);
            _scrollContainer = Screen.GetGraphicalUiElementByName("PlanetsOutline", "SystemScrollContainer");
            _systemList = Screen.GetGraphicalUiElementByName("PlanetsOutline", "SystemScrollContainer", "SystemList");

            UIScrollEventHandler.Instance.ScrollElements.Add((_scrollContainer, false));

            //FillList();

            Screen.UpdateLayout();
            Screen.RemoveFromManagers();
        }

        public override void Show()
        {            
            if(!_filled)
                FillList();

            _filled = true;

            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }

        public void FillList()
        {
            ListedSystems.ForEach(x =>
            {
                var starElement = ObjectFinder
                    .Self
                    .GumProjectSave
                    .Components
                    .First(item => item.Name == "StatBlockComponent2")
                    .ToGraphicalUiElement(SystemManagers.Default, false);

                starElement.SetProperty("HeaderTextText", x.Name);

                var list = starElement.GetGraphicalUiElementByName("SubStatBlocks");

                x.Planets.ForEach(y =>
                {
                    var planetElement = ObjectFinder
                        .Self
                        .GumProjectSave
                        .Components
                        .First(item => item.Name == "StatBlockComponent")
                        .ToGraphicalUiElement(SystemManagers.Default, false);

                    planetElement.SetProperty("HeaderTextText", y.Name);
                    planetElement.SetProperty("BodyTextText", "Placeholder");

                    list.Children.Add(planetElement);
                });

                _systemList.Children.Add(starElement);
            });
        }
    }
}
