using Game1.Extensions;
using Game1.Input;
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
    internal class MainMenu : ScreenBase
    {
        public static MainMenu Instance { get; private set; }

        public GraphicalUiElement NewGameButton { get; private set; }
        public GraphicalUiElement LoadButton { get; private set; }

        public GraphicalUiElement TestTextBox { get; private set; }

        public MainMenu()
        {
            Screen = GlobalStatic.GumProject.Screens.First(x => x.Name == "MainMenu").ToGraphicalUiElement(SystemManagers.Default, true);

            NewGameButton = Screen.GetGraphicalUiElementByName("ButtonContainer", "NewGameButton");
            LoadButton = Screen.GetGraphicalUiElementByName("ButtonContainer", "LoadButton");
            TestTextBox = Screen.GetGraphicalUiElementByName("TestTextBox");

            var TextBox = new TextBox(TestTextBox);

            new InteractiveGUE(NewGameButton).OnClick = () => 
            {
                MainMenu.Instance.Hide();
                Main.Instance.ShowTopBar();
                Main.Instance.Show();
            };

            new InteractiveGUE(LoadButton).OnClick = () =>
            {
                return;
            };

            Instance = this;
        }
    }
}
