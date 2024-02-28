using Game1.Extensions;
using Gum.Wireframe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Input
{
    public class CheckBox
    {
        public bool Checked
        {
            get
            {
                return _checked;
            }
            set
            {
                _checked = value;
                if (_checked)
                    Element.ApplyState("Checked");
                else
                    Element.ApplyState("UnChecked");

                OnChange?.Invoke(_checked);
            }
        }
        private bool _checked {  get; set; }
        public GraphicalUiElement Element { get; set; }
        private InteractiveGUE _interactiveGUE {  get; set; }

        public Action<bool> OnChange { get; set; }
        
        public CheckBox(GraphicalUiElement element, bool initial = false) 
        {
            Element = element;
            Checked = initial;

            _interactiveGUE = new InteractiveGUE(Element);
            _interactiveGUE.OnClick = () =>
            {
                Toggle();
            };
        }

        public void Toggle()
        {
            this.Checked = !this.Checked;
        }

        public static CheckBox CreateFrom(GraphicalUiElement Element, bool initial = false)
        {
            return new(Element, initial);
        }
    }
}
