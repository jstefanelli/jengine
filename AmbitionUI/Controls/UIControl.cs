using System;
using OpenTK.Input;

namespace AmbitionUI.Controls
{
    public enum AnchorMode{
        TopLeft, CenterLeft, BottomLeft, TopCenter, Center, BottomCenter, TopRight, CenterRight, BottomRight
    }

    public enum SizeMode{
        KeepSize, Expand
    }

    public abstract class UIControl
    {
        protected UISystem BaseSystem { get; private set; }
        protected UIControl Parent { get; set; }

        public SizeMode HorizontalSizePolicy { get; set; } = SizeMode.Expand;
        public SizeMode VerticalSizePolicy { get; set; } = SizeMode.Expand;

        public int SizeX { get; protected set; } = 0;
        public int SizeY { get; protected set; } = 0;

        public int MaxSizeX { get; set; } = 0;
        public int MaxSizeY { get; set; } = 0;

        public int MinSizeX { get; set; } = 0;
        public int MinSizeY { get; set; } = 0;

		public int LastPosX { get; protected set; } = 0;
		public int LastPosY { get; protected set; } = 0;

        public int MarginTop { get; set; } = 0;
        public int MarginBottom { get; set; } = 0;
        public int MarginLeft { get; set; } = 0;
        public int MarginRight { get; set; } = 0;

        public int SizePriority { get; set; } = 0;

		public abstract void OnEnterFocus();
        public abstract void OnLeaveFocus();

		public virtual void Draw(int PosX, int PosY){
			LastPosX = PosX;
			LastPosY = PosY;	
		}
		
        public virtual void Update(){
            
        }

        public virtual void OnMouseDown(MouseButtonEventArgs e){
            
        }

        public virtual void OnMouseUp(MouseButtonEventArgs e){
            
        }

        public virtual void OnMouseMove(MouseMoveEventArgs e){
            
        }

        
        public virtual void Resize(int width, int height)
        {
            SizeX = (width < MinSizeX) ? MinSizeX : (HorizontalSizePolicy == SizeMode.Expand) ? width : (width > MaxSizeX) ? MaxSizeX : width;
            SizeY = (height < MinSizeY) ? MinSizeY : (VerticalSizePolicy == SizeMode.Expand) ? height : (height > MaxSizeY) ? MaxSizeY : height;
        }

        protected UIControl(UISystem system){
            BaseSystem = system;
        }

        protected UIControl(UISystem system, UIControl parent) : this(system){
            Parent = parent;
        }
    }
}
