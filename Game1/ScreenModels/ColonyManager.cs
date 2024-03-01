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
using RenderingLibrary;
using System;
using System.Linq;

namespace Game1.ScreenModels
{
    public class ColonyManager : ScreenBase
    {
        public static ColonyManager Instance { get; private set; }
        private GraphicalUiElement
            _systemListContainer,
            _buildQueueList,
            _buildingContainer,
            _upButton,
            _downButton,
            _updateButton,
            _createButton,
            _pauseButton,
            _cancelButton,
            _amountPlus,
            _amountMinus,
            _amountContinuous,
            _allocationPlus,
            _allocationMinus;

        private ScrollView _buildQueue { get; set; }
        private ScrollView _buildingList { get; set; }
        private BuildingQueueItem _selectedBuildQueueItem { get; set; }

        private GameEntity SelectedEntity
        {
            get
            {
                return _selectedEntity;
            }
            set
            {
                _selectedEntity = value;
                _selectedBuildQueueItem = null;
                _buildQueue.Items.Clear();
                _buildQueue.Container.Visible = false;
                UpdateState();
            }
        }
        private GameEntity _selectedEntity {  get; set; }
        private ComponentSave _BuildQueueItem = ObjectFinder
            .Self
            .GumProjectSave
            .Components
            .First(item => item.Name == "BuildingQueueItem");

        private ComponentSave _BuildInfoItem = ObjectFinder
            .Self
            .GumProjectSave
            .Components
            .First(item => item.Name == "BuildingBuildItem");

        private int _editorAmount { get; set; } = 0;
        private int _editorAllocation { get; set; } = 0;
        private bool _continuous { get; set; } = false;

        public ColonyManager()
        {
            Instance = this;
            Screen = GlobalStatic.GumProject.Screens.First(x => x.Name == "ColonyManager").ToGraphicalUiElement(SystemManagers.Default, true);
            _systemListContainer = Screen.GetGraphicalUiElementByName("ColonyManagerTopContainer", "SystemListOutline", "SystemListContainer");

            _buildingContainer = Screen.GetGraphicalUiElementByName("ColonyManagerTopContainer", "RectangleInstance", "RectangleInstance1", "ColonyBuildingOptions");
            _buildingList = new ScrollView(_buildingContainer);
            _buildingList.KeyboardEnabled = false;

            _buildQueueList = Screen.GetGraphicalUiElementByName("ColonyManagerTopContainer", "RectangleInstance", "BuildQueueList");
            _buildQueue = new ScrollView(_buildQueueList);
            _buildQueue.KeyboardEnabled = false;

            SystemList.Instance.SelectionChanged += SystemList_SelectionChanged;
            SystemList.Instance.OnClick += SystemList_OnClick;
            _buildQueue.OnClick += _buildQueue_OnClick; ;
            _buildQueue.SelectionChanged += _buildQueue_SelectionChanged;

            SetupEditor();
        }

        public void SetupEditor()
        {
            var panel = Screen.GetGraphicalUiElementByName("ColonyManagerTopContainer", "RectangleInstance", "RectangleInstance3", "ContainerInstance12");
            _upButton = panel.GetGraphicalUiElementByName("QueueUpButton");
            _downButton = panel.GetGraphicalUiElementByName("QueueDownButton");

            _updateButton = panel.GetGraphicalUiElementByName("QueueUpdateButton");
            _createButton = panel.GetGraphicalUiElementByName("QueueCreateButton");

            _pauseButton = panel.GetGraphicalUiElementByName("QueuePauseButton");
            _cancelButton = panel.GetGraphicalUiElementByName("QueueCancelButton");

            _amountPlus = panel.GetGraphicalUiElementByName("QueueAmountPlus");
            _amountMinus = panel.GetGraphicalUiElementByName("QueueAmountMinus");
            _amountContinuous = panel.GetGraphicalUiElementByName("QueueAmountContinuous");

            _allocationPlus = panel.GetGraphicalUiElementByName("QueueAllocationPlus");
            _allocationMinus = panel.GetGraphicalUiElementByName("QueueAllocationMinus");


            //buttons:
            new InteractiveGUE(_upButton).OnClick = () =>
            {
                if (!(SelectedEntity is Orbital o))
                    return;

                if (_selectedBuildQueueItem == null)
                    return;

                var oldIndex = o.Colony.BuildingQueue.IndexOf(_selectedBuildQueueItem);

                if(oldIndex == 0) 
                    return;

                o.Colony.BuildingQueue.Move(_selectedBuildQueueItem, oldIndex - 1);
                _buildQueue.SetSelectedIndex(oldIndex - 1);
                UpdateState();
            };

            new InteractiveGUE(_downButton).OnClick = () =>
            {
                if (!(SelectedEntity is Orbital o))
                    return;
                if (_selectedBuildQueueItem == null)
                    return;

                var oldIndex = o.Colony.BuildingQueue.IndexOf(_selectedBuildQueueItem);

                if (oldIndex == o.Colony.BuildingQueue.Count - 1)
                    return;

                o.Colony.BuildingQueue.Move(_selectedBuildQueueItem, oldIndex + 1);
                _buildQueue.SetSelectedIndex(oldIndex + 1);
                UpdateState();
            };

            new InteractiveGUE(_updateButton).OnClick = () =>
            {
                if (!(SelectedEntity is Orbital o))
                    return;
                if (_selectedBuildQueueItem == null)
                    return;

                _selectedBuildQueueItem.Amount = _editorAmount;
                _selectedBuildQueueItem.Inf = _continuous;
                _selectedBuildQueueItem.Allocation = ((float)_editorAllocation / 100f);

                UpdateState();
            };

            new InteractiveGUE(_pauseButton).OnClick = () =>
            {
                if (!(SelectedEntity is Orbital o))
                    return;
                if (_selectedBuildQueueItem == null)
                    return;

                _selectedBuildQueueItem.Paused = !_selectedBuildQueueItem.Paused;
                UpdateState();
            };

            new InteractiveGUE(_cancelButton).OnClick = () => {

                if (!(SelectedEntity is Orbital o))
                    return;
                if (_selectedBuildQueueItem == null)
                    return;

                o.Colony.BuildingQueue.Remove(_selectedBuildQueueItem);
                UpdateState();
            };

            new InteractiveGUE(_amountPlus).OnClick = () => {
                _editorAmount += FlatKeyboard.Instance.IsKeyDown(Keys.LeftShift) ? 5 : 1; ;
                _continuous = false;

                Screen.SetProperty("QueueAmountText", _editorAmount.ToString());
            };

            new InteractiveGUE(_amountMinus).OnClick = () => {
                _editorAmount -= FlatKeyboard.Instance.IsKeyDown(Keys.LeftShift) ? 5 : 1;
                _editorAmount = Math.Max(0, _editorAmount);
                _continuous = false;

                Screen.SetProperty("QueueAmountText", _editorAmount.ToString());
            };

            new InteractiveGUE(_amountContinuous).OnClick = () => {
                _continuous = !_continuous;

                Screen.SetProperty("QueueAmountText", "C");
            };

            new InteractiveGUE(_allocationPlus).OnClick = () => {
                _editorAllocation += FlatKeyboard.Instance.IsKeyDown(Keys.LeftShift) ? 20 : 5;
                _editorAllocation = Math.Min(100, _editorAllocation);

                Screen.SetProperty("QueueAllocationText", $"{_editorAllocation}%");
            };

            new InteractiveGUE(_allocationMinus).OnClick = () => {
                _editorAllocation -= FlatKeyboard.Instance.IsKeyDown(Keys.LeftShift) ? 20 : 5;
                _editorAllocation = Math.Max(0, _editorAllocation);

                Screen.SetProperty("QueueAllocationText", $"{_editorAllocation}%");
            };

        }

        private void _buildQueue_SelectionChanged(GraphicalUiElement oldSelection, GraphicalUiElement newSelection)
        {
            if (!(newSelection?.Tag is BuildingQueueItem item))
                return;

            _selectedBuildQueueItem = item;
        }

        private void _buildQueue_OnClick(GraphicalUiElement clickedEntity)
        {
            if (!(clickedEntity?.Tag is BuildingQueueItem item))
                return;

            _buildQueue.SetSelected(clickedEntity);

            _selectedBuildQueueItem = item;
        }

        private void SystemList_OnClick(GameEntity clickedEntity)
        {
            SelectedEntity = clickedEntity;
        }

        private void SystemList_SelectionChanged(GameEntity selectedEntity)
        {
            SelectedEntity = selectedEntity;
        }

        public override void Update(double deltaTime)
        {
            if (_timeSinceLastUpdate > 2)
            {
                UpdateState();
                _timeSinceLastUpdate = 0;
            }

            base.Update(deltaTime);
        }

        private void UpdateState()
        {
            //_buildQueue.Items.ForEach(x => x.Element.RemoveFromManagers());
            //_buildQueue.Items.Clear();
            //_buildQueue.Container.Visible = false;

            UpdateBuildingList();

            if (SelectedEntity == null)
                return;
            if(!(SelectedEntity is Orbital o))
                return;
            if (o.Colony == null)
                return;

            var c = o.Colony;
            var value = string.Empty;

            Screen.SetProperty("SpeciesText", "Human");

            value = ((c.AgriPop / c.Population) * 100).ToString("#.0") + "% of Pop";
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

            Screen.SetProperty("BPDayText", c.ICGenerationDay.ToString("#.0"));
            Screen.SetProperty("BasicMetalsText", c.ResourceStockpiles[GameLogic.Resource.BasicMetals].ToString("#.0"));
            Screen.SetProperty("LowDensityElementsText", c.ResourceStockpiles[GameLogic.Resource.LowDensityElements].ToString("#.0"));
            Screen.SetProperty("RareMetalsText", c.ResourceStockpiles[GameLogic.Resource.RareMetals].ToString("#.0"));
            Screen.SetProperty("NobleElementsText", c.ResourceStockpiles[GameLogic.Resource.NobelElements].ToString("#.0"));
            Screen.SetProperty("HeavyMetalsText", c.ResourceStockpiles[GameLogic.Resource.HeavyMetals].ToString("#.0"));
            Screen.SetProperty("FissileElementsText", c.ResourceStockpiles[GameLogic.Resource.FissileElements].ToString("#.0"));
            Screen.SetProperty("FusibleElementsText", c.ResourceStockpiles[GameLogic.Resource.FusibleElements].ToString("#.0"));
            Screen.SetProperty("ExoticMaterialsText", c.ResourceStockpiles[GameLogic.Resource.ExoticMaterials].ToString("#.0"));

            //Build Queue:
            _buildQueue.Container.Visible = true;
            int selectedIndex = _buildQueue.GetSelectedIndex();

            var buildQueueElements = c.BuildingQueue.Select(x =>
            {
                var element = _BuildQueueItem.ToGraphicalUiElement(SystemManagers.Default, false);

                element.SetProperty("AllocationText", $"{(x.Allocation * 100).ToString("#.0")}%");
                element.SetProperty("NameText", GameState.BuildingInfo[x.ColonyBuilding].FriendlyName);

                var amountText = x.Inf ? "Continues" : x.Amount.ToString();
                element.SetProperty("AmountText", amountText);

                var ic = GameState.BuildingInfo[x.ColonyBuilding].IC;
                var totalIc = ic * x.Amount;

                element.SetProperty("ICText", $"{ic} - {totalIc}");
                element.SetProperty("ProgressCurrentText", $"{(x.Progress / ic * 100).ToString("#.0")}%");

                var completionText = x.Inf ? "-" : $"{x.TimeToCompletion(c.ICGenerationDay).ToString("#.0")} days";
                element.SetProperty("CompletionTimeText", completionText);

                var checkBoxElement = element.GetGraphicalUiElementByName("PauseOnCompletion");
                CheckBox checkBox = new CheckBox(checkBoxElement, x.PauseOnCompletion);
                checkBox.OnChange = (bool newState) => { x.PauseOnCompletion = newState; };

                element.Tag = x;

                return element;
            }).ToList();

            _buildQueue.SetItems(buildQueueElements);
            _buildQueue.SetSelectedIndex(selectedIndex);
        }

        public void UpdateBuildingList()
        {
            _buildingList.SetItems(new());
            _buildingList.Container.Visible = true;

            var elements = GameState.BuildingInfo.OrderBy(x => x.Value.FriendlyName).Select(x =>
            {
                var element = _BuildInfoItem.ToGraphicalUiElement(SystemManagers.Default, false);
                element.SetProperty("NameText", x.Value.FriendlyName);
                element.SetProperty("ICText", x.Value.IC);

                return element;
            }).ToList();

            _buildingList.SetItems(elements);
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
