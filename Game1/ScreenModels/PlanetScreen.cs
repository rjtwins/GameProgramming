using Game1.Components;
using Game1.Extensions;
using Game1.GameEntities;
using Game1.GameLogic;
using Game1.Input;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;
using GumRuntime;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using RenderingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Game1.ScreenModels
{
    public class PlanetScreen : ScreenBase
    {
        public static PlanetScreen Instance { get; private set; }
        public List<SolarSystem> ListedSystems { get; private set; } = new();
        public SystemList SystemList { get; private set; }
        public GameEntity SelectedBody
        {
            get
            {
                return _selectedBody;
            }
            set
            {
                if (_selectedBody?.Guid == value.Guid)
                    return;

                _selectedBody = value;
                UpdateSelectedBody();
            }
        }
        private GameEntity _selectedBody { get; set; }

        private GraphicalUiElement
            _expandButton,
            _collapseButton,
            _systemListContainer,
            _bodyTextBoxFilter;

        private CheckBox _checkBoxAsteroid,
            _checkTerrestrial,
            _checkBoxGas,
            _checkBoxMines,
            _checkBoxColony,
            _checkBoxCivFleet,
            _checkBoxStation,
            _checkBoxMillFleet;

        private TextBox _bodyFilterTextBox;

        public PlanetScreen()
        {
            Instance = this;

            Screen = GlobalStatic.GumProject.Screens.First(x => x.Name == "Planets").ToGraphicalUiElement(SystemManagers.Default, true);
            _systemListContainer = Screen.GetGraphicalUiElementByName("PlanetsOutline", "SystemListContainer");
            
            SystemList = SystemList.Instance;

            SystemList.OnClick += (entity) =>
            {
                SelectedBody = entity;
            };

            SystemList.SelectionChanged += (selectedEntity) =>
            {
                SelectedBody = selectedEntity;
            };

            _expandButton = Screen.GetGraphicalUiElementByName("ExpandButton");
            _collapseButton = Screen.GetGraphicalUiElementByName("CollapseButton");
            _bodyTextBoxFilter = Screen.GetGraphicalUiElementByName("FilterPanel", "Container", "BodyNameFilter");

            new InteractiveGUE(_expandButton).OnClick = () =>
            {
            };

            new InteractiveGUE(_collapseButton).OnClick = () =>
            {
                SystemList.CollapseAll();
            };

            _bodyFilterTextBox = new TextBox(_bodyTextBoxFilter);
            _bodyFilterTextBox.OnTextChanged += _bodyFilterTextBox_OnTextChanged;

            #region Checkboxes
            var element = Screen.GetGraphicalUiElementByName("FilterPanel", "Container", "CheckBoxAsteroid");
            _checkBoxAsteroid = new CheckBox(element);
            _checkBoxAsteroid.OnChange = (bool newState) => { if (newState) SystemList.SetSimple(); else SystemList.SetDetailed(); };

            element = Screen.GetGraphicalUiElementByName("FilterPanel", "Container", "CheckTerrestrial");
            _checkTerrestrial = new CheckBox(element);
            //_checkTerrestrial.OnChange = (bool newState) => UpdateState();

            element = Screen.GetGraphicalUiElementByName("FilterPanel", "Container", "CheckBoxGas");
            _checkBoxGas = new CheckBox(element);
            //_checkBoxGas.OnChange = (bool newState) => UpdateState();

            element = Screen.GetGraphicalUiElementByName("FilterPanel", "Container", "CheckBoxMines");
            _checkBoxMines = new CheckBox(element);
            //_checkBoxMines.OnChange = (bool newState) => UpdateState();

            element = Screen.GetGraphicalUiElementByName("FilterPanel", "Container", "CheckBoxColony");
            _checkBoxColony = new CheckBox(element);
            //_checkBoxColony.OnChange = (bool newState) => UpdateState();

            element = Screen.GetGraphicalUiElementByName("FilterPanel", "Container", "CheckBoxCivFleet");
            _checkBoxCivFleet = new CheckBox(element);
            //_checkBoxCivFleet.OnChange = (bool newState) => UpdateState();

            element = Screen.GetGraphicalUiElementByName("FilterPanel", "Container", "CheckBoxStation");
            _checkBoxStation = new CheckBox(element);
            //_checkBoxStation.OnChange = (bool newState) => UpdateState();

            element = Screen.GetGraphicalUiElementByName("FilterPanel", "Container", "CheckBoxMillFleet");
            _checkBoxMillFleet = new CheckBox(element);
            //_checkBoxMillFleet.OnChange = (bool newState) => UpdateState();
            #endregion

            Screen.RemoveFromManagers();
        }

        private void _bodyFilterTextBox_OnTextChanged(string newText)
        {
            SystemList.FilterBodyByText(newText);
        }

        public override void Update(double deltaTime)
        {
            if ( _timeSinceLastUpdate > 5 ) 
            {
                UpdateDetails();
                _timeSinceLastUpdate = 0;
            }
        }

        private bool PassFilter(GameEntity e)
        {
            if (!_checkTerrestrial.Checked && e is Orbital s && s.SatelliteType == BodyType.Terrestrial)
                return false;

            return true;
        }

        public void UpdateSelectedBody()
        {
            if (_selectedBody == null)
                return;

            GlobalStatic.Game.Services.GetService<Graphics.Camera>().Position = (_selectedBody.X, _selectedBody.Y);
            GameState.Focus = _selectedBody;

            Screen.SuspendLayout(true);

            UpdateOverview();
            UpdateDetails();

            GlobalStatic.UISemaphore.WaitOne();
            Screen.ResumeLayout(true);
            GlobalStatic.UISemaphore.Release();
        }

        private void UpdateOverview()
        {
            if (_selectedBody is Orbital o && !(_selectedBody is Star))
            {
                var parent = o.Parent;
                if (o.Parent is SolarSystem s)
                    parent = s.Children.FirstOrDefault();

                Screen.SetProperty("DistanceStickVisible", true);
                Screen.SetProperty("PlanetVisible", true);
                Screen.SetProperty("PlanetDistanceVisible", true);
                Screen.SetProperty("PlanetNameVisible", true);
                Screen.SetProperty("PlanetRadiusVisible", true);
                Screen.SetProperty("PlanetRadiusStickVisible", true);

                Screen.SetProperty("PlanetNameText", SelectedBody.Name);
                Screen.SetProperty("ParentNameText", parent.Name);

                var distance = $"{(o.Distance / (float)GlobalStatic.AU).ToString("0.0")} <-> {((o.SemiMajorAxis / 1000) / (float)GlobalStatic.AU).ToString("0.000")} AU";
                Screen.SetProperty("PlanetDistanceText", distance);

                Screen.SetProperty("PlanetRadiusText", (o.Radius * 2).ToString("E1") + " KM");
                Screen.SetProperty("ParentRadiusText", (parent.Radius * 2).ToString("E1") + " KM");
            }
            else if (_selectedBody is Star s)
            {
                Screen.SetProperty("DistanceStickVisible", false);
                Screen.SetProperty("PlanetVisible", false);
                Screen.SetProperty("PlanetDistanceVisible", false);
                Screen.SetProperty("PlanetNameVisible", false);
                Screen.SetProperty("PlanetRadiusVisible", false);
                Screen.SetProperty("PlanetRadiusStickVisible", false);

                Screen.SetProperty("ParentNameText", s.Name);
                Screen.SetProperty("ParentRadiusText", (s.Radius * 2).ToString("E1") + " KM");
            }
        }

        private void UpdateDetails()
        {
            Screen.SuspendLayout(true);
            //reset:
            //Atmos:
            Enum.GetValues<Gas>().ToList().ForEach(gas =>
            {
                Screen.SetProperty($"{gas}PercText", "-");
                Screen.SetProperty($"{gas}AtmText", "-");
            });

            //Res:
            Enum.GetValues<Resource>().ToList().ForEach(resource =>
            {
                Screen.SetProperty($"{resource}YieldText", "-");
                Screen.SetProperty($"{resource}AmountText", "-");
            });

            Screen.SetProperty("TectonicsText", "-");

            if (!(_selectedBody is Orbital o))
                return;

            o.Atmosphere.Gases.ToList().ForEach(pair =>
            {
                var atm = pair.Value.ToString("#.0");
                atm = atm == "0" || atm == "NaN" || atm == ".0" ? "-" : atm;

                var perc = (pair.Value / o.Atmosphere.AtmosPressure * 100).ToString("00.0");
                perc = perc == "0" || perc == "00.0" || perc == "NaN" ? "-" : perc;

                Screen.SetProperty($"{pair.Key}AtmText", atm);
                Screen.SetProperty($"{pair.Key}PercText", perc);
            });
            o.Resources.ToList().ForEach(pair =>
            {
                var amount = pair.Value.amount.ToString("#.0");
                amount = amount == "0" || amount == "NaN" || amount == ".0" ? "-" : amount;

                var yield = (pair.Value.access * 100).ToString("00.0");
                yield = yield == "0" || yield == "00.0" || yield == "NaN" ? "-" : yield;

                Screen.SetProperty($"{pair.Key}AmountText", amount);
                Screen.SetProperty($"{pair.Key}YieldText", yield);
            });

            Screen.SetProperty("BaseTempText", o.BaseSurfaceTemp.ToString("000.0"));
            Screen.SetProperty("TectonicsText", o.CoreType.ToString());

            //Screen.SetProperty("BaseTempText", o.BaseSurfaceTemp.ToString("#.0"));
            Screen.ResumeLayout(true);
        }

        public override void Show()
        {
            _systemListContainer.Children.Add(SystemList.Container);

            SystemList.Instance.ClearFilters();
            SystemList.Show();
            SystemList.SetDetailed();

            base.Show();
        }

        public override void Hide()
        {
            _systemListContainer.Children.Remove(SystemList.Container);

            SystemList.Hide();
            base.Hide();
        }
    }
}
