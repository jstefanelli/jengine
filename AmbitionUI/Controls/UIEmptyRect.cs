using System;
using OpenTK;
using OpenTK.Input;

namespace AmbitionUI.Controls
{
	public class UIEmptyRect : UIControl
	{
		private Vector4 originalColor = new Vector4(0, 0, 1, 1);
		private Vector4 accentColor = new Vector4(1, 0, 0, 1);
		private bool useAccent = false;

		public Vector4 Color
		{
			get
			{
				return originalColor;
			}
			set
			{
				originalColor = value;
			}
		}

		public UIEmptyRect(UISystem system) : this(system, null)
		{
		}

		public UIEmptyRect(UISystem system, UIControl parent) : base(system, parent)
		{
		}

		public override void Draw(int PosX, int PosY)
		{
			base.Draw(PosX, PosY);
			DrawingUtils.DrawRect(PosX + MarginLeft, PosY + MarginBottom, SizeX - MarginLeft - MarginRight, SizeY - MarginTop - MarginBottom, useAccent ? accentColor : originalColor);
		}

		public override void OnEnterFocus()
		{
			useAccent = true;
		}

		public override void OnLeaveFocus()
		{
			useAccent = false;
		}

		public override void OnMouseUp(MouseButtonEventArgs e)
		{
			BaseSystem.FocusControl(this);
		}
	}
}