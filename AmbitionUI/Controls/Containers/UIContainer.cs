using System;
using System.Collections.Generic;

namespace AmbitionUI.Controls.Containers
{
    public abstract class UIContainer : UIControl
    {
        

        public UIContainer(UISystem system) : this(system, null)
        {
            
        }

        public UIContainer(UISystem system, UIControl parent) : base(system, parent)
        {
            
        }

        public abstract void AddControl(UIControl c);
        public abstract void DropControl(UIControl c);
    }
}
