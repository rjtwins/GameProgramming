using Game1.Extensions;
using Game1.GameLogic;
using Game1.GameLogic.SubSystems;
using Game1.Input;
using Gum.Managers;
using Gum.Wireframe;
using GumRuntime;
using MonoGame.Extended.Screens;
using RenderingLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.PortableExecutable;
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

        public SubSystemBase ActiveSubSystem
        {
            get
            {
                return _activeSubSystem;
            }
            set
            {
                _activeSubSystem = value;
                SubSystemChanged();
            }
        }
        private SubSystemBase _activeSubSystem { get; set; }


        private GraphicalUiElement ShipStatsScrollView,
            ComponentsScrollView,
            ComponentList,
            DesignListContainer,
            DesignList,
            InstalledComponentsScrollView,
            InstalledComponentsList;

        public ShipDesign()
        {
            Instance = this;
            Screen = GlobalStatic.GumProject.Screens.First(x => x.Name == "ShipDesigner").ToGraphicalUiElement(SystemManagers.Default, addToManagers: true);
            ShipStatsScrollView = Screen
                .GetGraphicalUiElementByName("ShipStats.ContainerInstance2")
                .GetGraphicalUiElementByName("ScrollView");

            ComponentsScrollView = Screen.GetGraphicalUiElementByName("Components", "CenterMainContainer", "CenterInfoContainer", "ComponentListOutline", "ComponentListScrollContainer");
            ComponentList = ComponentsScrollView.GetGraphicalUiElementByName("ComponentList");

            DesignListContainer = Screen.GetGraphicalUiElementByName("Top", "DesignListContainer");
            DesignList = Screen.GetGraphicalUiElementByName("Top", "DesignListContainer", "DesignList");
            //.GetGraphicalUiElementByName("DesignList");

            InstalledComponentsScrollView = Screen.GetGraphicalUiElementByName("Installed", "InstalledComponentsScrollView");
            InstalledComponentsList = Screen.GetGraphicalUiElementByName("Installed", "InstalledComponentsScrollView", "InstalledComponentsList");

            UIScrollEventHandler.Instance.ScrollElements.Add((ShipStatsScrollView, false));
            UIScrollEventHandler.Instance.ScrollElements.Add((ComponentsScrollView, false));
            UIScrollEventHandler.Instance.ScrollElements.Add((DesignListContainer, true));
            UIScrollEventHandler.Instance.ScrollElements.Add((InstalledComponentsScrollView, false));

            Screen.UpdateLayout();
            Screen.RemoveFromManagers();
        }

        public override void Show()
        {
            base.Show();

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
            DesignList.Children.Clear();

            foreach (Ship design in GameState.ShipDesigns)
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

                //interactiveGUE.OnClick = () =>
                //{
                //    ActiveSubSystem = subSystem;
                //};

                interactiveGUE.OnClick = () =>
                {
                    AddSubSystemToDesign(subSystem);
                    SubSystemChanged();
                };
            }

            ComponentList.UpdateLayout();
        }

        public void SubSystemChanged()
        {
            var addedSystems = ActiveDesign.SubSystems
                .GroupBy(x => x.GetType())
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

                interactiveGUE.OnClick = () =>
                {
                    ActiveSubSystem = subSystem.instance;
                };

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
            Screen.SetProperty("SelectedName", ActiveDesign.Name);
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
                    sensorString += $"SENSOR: {x.Resolution} M3 @ {x.Range} KM\n\n";

                    var t = x.GetResolutionTable();
                    //var top =    "10KM " + string.Join("---", t.Select(x => $"{x.d.ToString("0.0E+0")}")) + "\n";
                    var top =    "10K KM " + string.Join("---", t.Select(x => $"{(x.d / 10000).ToString("000000")}")) + "\n";
                    var middle = "++++++ " + string.Join("|-----", t.Select(x => $"---")) + "\n";
                    var bot =    "M3     " + string.Join("---", t.Select(x => $"{x.r.ToString("000000")}")) + "\n\n";

                    sensorString += top;
                    sensorString += middle;
                    sensorString += bot;
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
