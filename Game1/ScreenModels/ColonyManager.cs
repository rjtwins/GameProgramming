using Game1.Components;
using Game1.GameEntities;
using Gum.Wireframe;
using GumRuntime;
using RenderingLibrary;
using System;
using System.Linq;

namespace Game1.ScreenModels
{
    public class ColonyManager : ScreenBase
    {
        public static ColonyManager Instance { get; private set; }
        private GraphicalUiElement _systemListContainer;
        private GameEntity SelectedEntity
        {
            get
            {
                return _selectedEntity;
            }
            set
            {
                _selectedEntity = value;
                UpdateState();
            }
        }

        private GameEntity _selectedEntity {  get; set; }

        public ColonyManager()
        {
            Instance = this;
            Screen = GlobalStatic.GumProject.Screens.First(x => x.Name == "ColonyManager").ToGraphicalUiElement(SystemManagers.Default, true);
            _systemListContainer = Screen.GetGraphicalUiElementByName("ColonyManagerTopContainer", "SystemListOutline", "SystemListContainer");

            SystemList.Instance.SelectionChanged += SystemList_SelectionChanged;
            SystemList.Instance.OnClick += SystemList_OnClick;
        }

        private void SystemList_OnClick(GameEntity clickedEntity)
        {
            SelectedEntity = clickedEntity;
        }

        private void SystemList_SelectionChanged(GameEntity selectedEntity)
        {
            SelectedEntity = selectedEntity;
        }

        private void UpdateState()
        {
            if (SelectedEntity == null)
                return;
            if(!(SelectedEntity is Orbital o))
                return;
            if (o.Colony == null)
                return;

            var c = o.Colony;
            var value = string.Empty;

            Screen.SetProperty("SpeciesText", "Human");

            value = ((c.AgriPop / c.Population) * 100).ToString("#.0") + "% of Total Pop";
            Screen.SetProperty("SustainPercText", value);

            Screen.SetProperty("TotalPopText", c.Population.ToString("#.0"));
            Screen.SetProperty("SustainPopText", c.AgriPop.ToString("#.0"));
            Screen.SetProperty("ServicesPopText", 0.ToString("#.0"));
            Screen.SetProperty("ManufacturingPopText", 0.ToString("#.0"));
            Screen.SetProperty("ExtractionPopText", 0.ToString("#.0"));
            Screen.SetProperty("GrowthRateText", 0.ToString("#.0"));
            Screen.SetProperty("GrowthRateText", "100/1000");

            if (c.PopulationSupported == int.MaxValue)
                value = "No Maximum";
            else
                value = c.PopulationSupported.ToString("#.0");
            Screen.SetProperty("PopSupportedText", value);

            Screen.SetProperty("BPDayText", c.BGGenerationDay.ToString("#.0"));
            Screen.SetProperty("WaterText", c.ResourceStockpiles[GameLogic.Resource.Water].ToString("#.0"));
            Screen.SetProperty("FissilesText", c.ResourceStockpiles[GameLogic.Resource.Fissiles].ToString("#.0"));
            Screen.SetProperty("VolitilesText", c.ResourceStockpiles[GameLogic.Resource.Volatiles].ToString("#.0"));
            Screen.SetProperty("FusablesText", c.ResourceStockpiles[GameLogic.Resource.Fusibles].ToString("#.0"));
            Screen.SetProperty("MetalsText", c.ResourceStockpiles[GameLogic.Resource.Metals].ToString("#.0"));
            Screen.SetProperty("RareMetalsText", c.ResourceStockpiles[GameLogic.Resource.RareMetals].ToString("#.0"));
        }

        public override void Show()
        {
            _systemListContainer.Children.Add(SystemList.Instance.Container);
            SystemList.Instance.SetSimple();
            SystemList.Instance.FilterNonColonies();
            SystemList.Instance.Show();
            base.Show();
        }

        public override void Hide()
        {
            _systemListContainer.Children.Clear();
            SystemList.Instance.Hide();
            base.Hide();
        }
    }
}
