using Game1.Extensions;
using Game1.GameLogic.SubSystems;
using Game1.Input;
using Gum.Managers;
using Gum.Wireframe;
using GumRuntime;
using RenderingLibrary;
using System.Linq;

namespace Game1.ScreenModels
{
    public class ShipDesign : ScreenBase
    {
        public static ShipDesign Instance { get; private set; }

        public GameLogic.ShipDesign ActiveDesign
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
        private GameLogic.ShipDesign _activeDesign { get; set; }

        private GraphicalUiElement ShipStatsScrollView,
            ShipStatsList,
            ComponentsScrollView,
            ComponentList,
            DesignListContainer,
            DesignList,
            InstalledComponentsScrollView,
            InstalledComponentsList,
            ShipName;

        private TextBox _shipNameTextBox { get; set; }

        private ScrollView _shipStatsScrollView, _designListContainer, _componentsScrollView, _installedComponentsScrollView;

        public ShipDesign()
        {
            Instance = this;
            Screen = GlobalStatic.GumProject.Screens.First(x => x.Name == "ShipDesigner").ToGraphicalUiElement(SystemManagers.Default, addToManagers: true);
            ShipStatsScrollView = Screen
                .GetGraphicalUiElementByName("ShipStats.ContainerInstance2")
                .GetGraphicalUiElementByName("ScrollView");

            ShipStatsList = Screen.GetGraphicalUiElementByName("ShipStats", "ContainerInstance2", "ScrollView", "ShipStatBlock");

            ComponentsScrollView = Screen.GetGraphicalUiElementByName("Components", "CenterMainContainer", "CenterInfoContainer", "ComponentListOutline", "ComponentListScrollContainer");
            ComponentList = ComponentsScrollView.GetGraphicalUiElementByName("ComponentList");

            DesignListContainer = Screen.GetGraphicalUiElementByName("Top", "DesignListContainer");
            DesignList = Screen.GetGraphicalUiElementByName("Top", "DesignListContainer", "DesignList");
            //.GetGraphicalUiElementByName("DesignList");

            InstalledComponentsScrollView = Screen.GetGraphicalUiElementByName("Installed", "InstalledComponentsScrollView");
            InstalledComponentsList = Screen.GetGraphicalUiElementByName("Installed", "InstalledComponentsScrollView", "InstalledComponentsList");
            ShipName = Screen.GetGraphicalUiElementByName("ShipStats", "ContainerInstance2", "Name1");
            _shipNameTextBox = new TextBox(ShipName);
            _shipNameTextBox.OnTextChanged += _shipNameTextBox_OnTextChanged;

            //_shipStatsScrollView = new(ShipStatsScrollView, InstalledComponentsList);
            //_componentsScrollView = new(ComponentsScrollView, ComponentList);
            //_designListContainer = new(DesignListContainer, DesignList, true);
            //_installedComponentsScrollView = new(ShipStatsScrollView, ShipStatsList);

            //UIScrollEventHandler.Instance.ScrollViews.Add(_shipStatsScrollView);
            //UIScrollEventHandler.Instance.ScrollViews.Add(_componentsScrollView);
            //UIScrollEventHandler.Instance.ScrollViews.Add(_designListContainer);
            //UIScrollEventHandler.Instance.ScrollViews.Add(_installedComponentsScrollView);

            Screen.UpdateLayout();
            Screen.RemoveFromManagers();
        }

        private void _shipNameTextBox_OnTextChanged(string newText)
        {
            ActiveDesign.Name = newText;
            GetShipDesigns();
            ShipChanged();
        }

        public override void Show()
        {
            base.Show();

            //var textbox = new TextBox();
            //textbox.Top = 250;
            //textbox.Left = 1500;
            //textbox.Width = 200;
            //textbox.Background = new SolidBrush(new Color(15, 15, 15) * 0);
            //textbox.Text = "HERE";
            //GlobalStatic.MyraPanel.Widgets.Add(textbox);


            GetShipDesigns();
            GetComponents();
            SubSystemChanged();
        }

        public override void Hide()
        {
            base.Hide();
            this.DesignList.Children.ToList().OfType<GraphicalUiElement>().ToList().ForEach(x =>
            {
                InteractiveGUE.UnRegister(x);
                x.RemoveFromManagers();
                x.Visible = false;
            });

            this.ComponentList.Children.ToList().OfType<GraphicalUiElement>().ToList().ForEach(x =>
            {
                InteractiveGUE.UnRegister(x);
                x.RemoveFromManagers();
                x.Visible = false;
            });

            this.InstalledComponentsList.Children.ToList().OfType<GraphicalUiElement>().ToList().ForEach(x =>
            {
                InteractiveGUE.UnRegister(x);
                x.RemoveFromManagers();
                x.Visible = false;
            });
        }

        public void GetShipDesigns()
        {
            DesignList.Children.OfType<GraphicalUiElement>().ToList().ForEach(x =>
            {
                InteractiveGUE.UnRegister(x);
            });

            DesignList.Children.Clear();
            var orderedDesigns = GameState.ShipDesigns
                .OrderByDescending(x => x.ShipClass != "-")
                .ThenBy(x => x.ShipClass)
                .ThenBy(x => x.Name)
                .ThenByDescending(x => x.WettMass);

            foreach (GameLogic.ShipDesign design in orderedDesigns)
            {
                var element = ObjectFinder
                    .Self
                    .GumProjectSave
                    .Components
                    .First(item => item.Name == "DesignItem")
                    .ToGraphicalUiElement(SystemManagers.Default, false);

                element.SetProperty("DesignText", $"{design.Name} Class {design.ShipClass}");
                //element.SetProperty("HeaderTextText", );

                DesignList.Children.Add(element);

                var interactive = new InteractiveGUE(element);

                interactive.OnClick = () =>
                {
                    this.ActiveDesign = design;
                };

                element.UpdateLayout();
            }

            var newButton = ObjectFinder
                .Self
                .GumProjectSave
                .Components
                .First(item => item.Name == "DesignItem")
                .ToGraphicalUiElement(SystemManagers.Default, false);

            newButton.SetProperty("DesignText", $"NEW DESIGN");

            DesignList.Children.Add(newButton);

            new InteractiveGUE(newButton)
            {
                OnClick = () =>
                {
                    GameLogic.ShipDesign ship = new();
                    ship.Name = $"Design {GameState.ShipDesigns.Count + 1}";
                    ship.ShipClass = "-";
                    GameState.ShipDesigns.Add(ship);
                    ActiveDesign = ship;
                    GetShipDesigns();
                    InteractiveGUE.UnRegister(newButton);
                }
            };

            newButton.UpdateLayout();

            DesignList.UpdateLayout();
        }

        public void GetComponents()
        {
            ComponentList.Children.Clear();

            foreach (var subSystem in GameState.SubSystems)
            {
                var element = ObjectFinder
                    .Self
                    .GumProjectSave
                    .Components
                    .First(item => item.Name == "StatBlockComponent")
                    .ToGraphicalUiElement(SystemManagers.Default, false);

                element.SetProperty("BodyTextText", subSystem.Report() );
                element.SetProperty("HeaderTextText", subSystem.Name);

                ComponentList.Children.Add(element);
                element.UpdateLayout();

                InteractiveGUE interactiveGUE = new InteractiveGUE(element);

                interactiveGUE.OnClick = () =>
                {
                    AddSubSystemToDesign(subSystem);
                    SubSystemChanged();
                };

                element.Tag = _componentsScrollView;
            }

            ComponentList.UpdateLayout();
        }

        public void SubSystemChanged()
        {
            var addedSystems = ActiveDesign.SubSystems
                .GroupBy(x => x.DesignGuid)
                .Select(x => new
                {
                    count = x.ToList().Count,
                    instance = x.ToList().First()
                })
                .ToList();

            InstalledComponentsList.Children.OfType<GraphicalUiElement>()
                .ToList()
                .ForEach(x =>
            {
                InteractiveGUE.UnRegister(x);
                x.RemoveFromManagers();
                x.Visible = false;
            });

            InstalledComponentsList.Children.Clear();


            foreach (var subSystem in addedSystems)
            {
                var element = ObjectFinder
                    .Self
                    .GumProjectSave
                    .Components
                    .First(item => item.Name == "StatBlockComponent")
                    .ToGraphicalUiElement(SystemManagers.Default, false);

                element.SetProperty("BodyTextText", subSystem.instance.Report());
                element.SetProperty("HeaderTextText", $"{subSystem.instance.Name} * {subSystem.count}");

                InstalledComponentsList.Children.Add(element);
                element.UpdateLayout();

                InteractiveGUE interactiveGUE = new InteractiveGUE(element);

                interactiveGUE.OnRightClick = () =>
                {
                    RemoveSubSystemFromDesign(subSystem.instance);
                    SubSystemChanged();

                    element.Visible = false;
                    element.RemoveFromManagers();
                    InteractiveGUE.UnRegister(element);
                };
            }
        }

        public void ShipChanged()
        {
            Screen.SetProperty("SelectedClass", ActiveDesign.ShipClass);
            _shipNameTextBox.SetText(ActiveDesign.Name, true);
            //Screen.SetProperty("SelectedName", ActiveDesign.Name);
            Screen.SetProperty("SelectedFullName", $"{ActiveDesign.Name} Class {ActiveDesign.ShipClass}");

            string basicString = string.Empty;
            basicString += $"TONNAGE DRY:\t{ActiveDesign.HullMass} TON\n";
            basicString += $"TONNAGE WETT:\t{ActiveDesign.WettMass} TON\n";
            basicString += $"TONNAGE ARMORED:\t{ActiveDesign.ArmorMass} TON\n";
            basicString += $"TONNAGE ARMOR:\t{ActiveDesign.ArmorMass - ActiveDesign.WettMass} TON\n";
            basicString += $"VOLUME:\t{(int)ActiveDesign.Volume} M3\n";

            basicString += $"FUEL:\t{ActiveDesign.MaxFuel} TON\n";
            Screen.SetProperty("BasicText", basicString);

            string crewString = string.Empty;
            crewString += $"REQUIRED:\t{ActiveDesign.CrewRequired}\n";
            crewString += $"BERTHS:\t{ActiveDesign.CrewCapacity}\n";
            Screen.SetProperty("CrewText", crewString);

            string energyString = string.Empty;
            energyString += $"GENERATION:\t{ActiveDesign.EnergyGeneration} KW\n";
            energyString += $"REQUIRED:\t{ActiveDesign.EnergyRequired} KW\n";
            energyString += $"MAX STORES:\t{ActiveDesign.EnergyMaxStorage} KJ\n";
            energyString += $"STORES GRACE:\t{ActiveDesign.TimeOnStores} S\n";
            Screen.SetProperty("EnergyText", energyString);

            string propString = string.Empty;
            propString += $"THRUST:\t{ActiveDesign.MaxThrust} KN\n";
            propString += $"SPEED:\t{ActiveDesign.MaxThrust / ActiveDesign.Mass} KN/TON\n";
            propString += $"ENGINE MASS:\t{ActiveDesign.SubSystems.OfType<Engine>().Sum(x => x.Mass)} TON\n";
            propString += $"ENGINE RATIO:\t{(int)((ActiveDesign.SubSystems.OfType<Engine>().Sum(x => x.Mass) / ActiveDesign.ArmorMass) * 100)} %\n";
            Screen.SetProperty("PropText", propString);

            string sensorString = string.Empty;
            ActiveDesign.SubSystems.OfType<Sensor>()
                .ToList()
                .GroupBy(x => (x.Range, x.Resolution))
                .OrderBy(x => x.Key.Range).ThenBy(x => x.Key.Resolution)
                .Select(x => x.ToList().First())
                .ToList()
                .ForEach(x =>
                {
                    //sensorString += $"SENSOR: {x.Resolution} M3 @ {x.Range} KM\n\n";
                    sensorString += x.GetRangeTable();
                });

            Screen.SetProperty("SensorText", sensorString);

            SubSystemChanged();
        }

        public void AddSubSystemToDesign(SubSystemBase subSystem)
        {
            ActiveDesign.SubSystems.Add(subSystem);
            ShipChanged();
        }

        public void RemoveSubSystemFromDesign(SubSystemBase subSystem)
        {
            ActiveDesign.SubSystems.Remove(subSystem);
            ShipChanged();
        }
    }
}
