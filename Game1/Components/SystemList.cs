using Game1.GameEntities;
using Game1.GraphicalEntities;
using Game1.Input;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;
using GumRuntime;
using RenderingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game1.Components
{
    public class SystemList
    {
        public static SystemList Instance { get; private set; }

        public GraphicalUiElement Container { get ; set; }
        public ScrollView ScrollView { get; set; }
        private ComponentSave _bodyStatBlock = ObjectFinder
            .Self
            .GumProjectSave
            .Components
            .First(item => item.Name == "BodyStatBlock");

        bool SimpleMode = false;

        public List<SolarSystem> SolarSystems { get; set; } = new ();
        public List<GameEntity> AllEntities { get; private set; } = new();
        public List<GraphicalUiElement> AllElements { get; private set; } = new();
        public List<GameEntity> ActiveEntities { get; private set; } = new();
        public List<GameEntity> CollapsedEntities { get; private set; } = new();
        public List<GameEntity> FilteredEntities { get; private set; } = new();

        //public Action<GameEntity> SelectionChanged { get; set; }
        //public Action<GameEntity> OnClick { get; set; }
        public delegate void OnClickEventHandler(GameEntity clickedEntity);
        public event OnClickEventHandler OnClick;

        public delegate void SelectionChangedEventHandler(GameEntity selectedEntity);
        public event SelectionChangedEventHandler SelectionChanged;


        public SystemList() 
        {
            if (Instance != null)
                return;

            Instance = this;

            Container = ObjectFinder
            .Self
            .GumProjectSave
            .Components
            .First(item => item.Name == "SystemList")
            .ToGraphicalUiElement(SystemManagers.Default, false);

            ScrollView = new(Container);

            ScrollView.OnClick = (clickedElement) =>
            {
                if (clickedElement == null)
                    return;

                if (!(clickedElement.Tag is GameEntity entity))
                    return;

                if (CollapsedEntities.Contains(entity))
                {
                    CollapsedEntities.Remove(entity);
                    entity.Children.ForEach(x => CollapsedEntities.Remove(x));
                    UpdateState();
                }
                else if (!CollapsedEntities.Contains(entity))
                {
                    CollapsedEntities.Add(entity);
                    entity.GetAllChildren().ForEach(x => CollapsedEntities.Add(x));
                    UpdateState();
                }

                var index = ActiveEntities.IndexOf(entity);
                ScrollView.SetSelectedIndex(index);

                OnClick?.Invoke(entity);
            };

            ScrollView.SelectionChanged = (oldSelection, newSelection) =>
            {
                if (newSelection == null)
                    return;

                if(!(newSelection.Tag is GameEntity entity))
                    return;

                SelectionChanged?.Invoke(entity);
            };
        }

        public void SetSimple()
        {
            AllElements.ForEach(x => x.ApplyState("Simple"));
            SimpleMode = true;
        }

        public void SetDetailed()
        {
            AllElements.ForEach(x => x.ApplyState("Detailed"));
            SimpleMode = false;
        }

        public void AddSystem(SolarSystem system)
        {
            AddSystems(new List<SolarSystem>() { system });
        }

        public void AddSystems(IEnumerable<SolarSystem> systems)
        {
            SolarSystems.AddRange(systems);
            SolarSystems = SolarSystems.DistinctBy(x => x.Guid).ToList();

            //Setup in correct order:
            //O boy what a mess...
            systems.ToList().ForEach(x =>
            {
                AllEntities.Add(x);
                CollapsedEntities.Add(x);
                var element = CreateListItem(x);
                element.ApplyState("Collapsed");
                AllElements.Add(element);
                x.Children.ForEach(y =>
                {
                    AllEntities.Add(y);
                    CollapsedEntities.Add(y);
                    var element = CreateListItem(y);
                    element.ApplyState("Collapsed");
                    AllElements.Add(element);
                    y.Children.ForEach(z =>
                    {
                        AllEntities.Add(z);
                        CollapsedEntities.Add(z);
                        var element = CreateListItem(z);
                        element.ApplyState("Collapsed");
                        AllElements.Add(element);
                        z.Children.ForEach(a =>
                        {
                            AllEntities.Add(a);
                            CollapsedEntities.Add(a);
                            var element = CreateListItem(a);
                            element.ApplyState("Collapsed");
                            AllElements.Add(element);
                        });
                    });
                });
            });

            AllElements.ForEach(x =>
            {
                if (SimpleMode)
                    x.ApplyState("Simple");
                else
                    x.ApplyState("Detailed");
            });

            ScrollView.SetItems(AllElements);

            UpdateState();
        }

        public void CollapseAll()
        {
            CollapsedEntities.Clear();
            CollapsedEntities.AddRange(AllEntities);
            UpdateState();
        }

        public void UpdateState()
        {
            var hiddenByCollapse = CollapsedEntities.SelectMany(x => x.Children).ToList();

            ActiveEntities = AllEntities
                .Except(hiddenByCollapse)
                .Except(FilteredEntities)
                .ToList();

            var elements = ActiveEntities.Select(x =>
            {
                var element = AllElements.FirstOrDefault(y => y.Tag == x);

                if (CollapsedEntities.Contains(x))
                    element.ApplyState("Collapsed");
                else
                    element.ApplyState("Deployed");

                return element;
            }
            ).ToList();

            //FilteredEntities.Clear();

            ScrollView.SetItems(elements);
        }

        public void Update()
        {

        }

        public GraphicalUiElement CreateListItem(GameEntity entity)
        {
            var element = _bodyStatBlock.ToGraphicalUiElement(SystemManagers.Default, false);
            element.SetProperty("HeaderColorVisible", false);

            element.SetProperty("HeaderTextText", entity.Name);

            string entityType = "System";
            string maxPop = string.Empty;
            string atmos = string.Empty;
            string avrTemp = string.Empty;
            string grav = (entity.SurfaceGravity / 9.807).ToString("000.0");
            string distance = string.Empty;
            string diameter = (GlobalStatic.AU * 100).ToString("000.000");
            string rads = string.Empty;

            if (entity is Orbital s)
            {
                entityType = s.SatelliteType.ToString();
                maxPop = "0";
                atmos = s.Atmosphere.AtmosPressure.ToString("#.0");
                avrTemp = s.GetAverageTemp().ToString("000.0");
                diameter = (s.Radius * 2 / 1000).ToString("E1");
                distance = (s.Distance / (double)GlobalStatic.AU).ToString("000.000");
                rads = "0";
            }

            element.SetProperty("P1", entityType);
            element.SetProperty("P2", maxPop);
            element.SetProperty("P3", atmos);
            element.SetProperty("P4", avrTemp);
            element.SetProperty("P5", grav);
            element.SetProperty("P6", distance);
            element.SetProperty("P7", diameter);
            element.SetProperty("P8", rads);
            element.Tag = entity;

            if (entity is SolarSystem)
                element.SetProperty("XOffset", (float)(0));
            if (entity is Star)
                element.SetProperty("XOffset", (float)(10));
            if (entity is Planet)
                element.SetProperty("XOffset", (float)(20));
            if (entity is Moon)
            {
                element.SetProperty("XOffset", (float)(30));
                element.ApplyState("HideButton");
            }

            return element;
        }

        public void FilterNonColonies()
        {
            //Todo do faction check.
            FilteredEntities.AddRange(AllEntities.OfType<Orbital>().Where(x => x.Colony == null));
            FilteredEntities = FilteredEntities.DistinctBy(x => x.Guid).ToList();

            UpdateState();
        }

        public void ClearFilters()
        {
            FilteredEntities.Clear();
            UpdateState();
        }

        internal void Hide()
        {
            Container.Visible = false;
        }

        public void Show()
        {
            Container.Visible = true;
        }
    }
}
