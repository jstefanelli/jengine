using jengine.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jengine.Render;
using AmbitionUI;
using AmbitionUI.Controls;
using AmbitionUI.Controls.Containers;
using jengine;
using OpenTK;

namespace J3XEdit
{
	internal class EditorMainScene : IScene
	{
		private UISystem UI;

		public void Draw(double delta, IRender render)
		{
			UI.Draw();
		}

		public void Load(IRender render)
		{
			UI = UISystem.InjectSystem(Game.Instance.Window);
			UIStackControl stack = new UIStackControl(UI)
			{
				Orientation = StackOrientation.Vertical,
				HorizontalSizePolicy = SizeMode.Expand,
				VerticalSizePolicy = SizeMode.Expand,
			};
			UI.RootControl = stack;
			UIEmptyRect rect = new UIEmptyRect(UI, stack)
			{
				HorizontalSizePolicy = SizeMode.Expand,
				VerticalSizePolicy = SizeMode.KeepSize,
				MaxSizeY = 50,
				MinSizeY = 30,
				MarginLeft = 5,
				MarginRight = 5,
				MarginBottom = 5,
				MarginTop = 5,
				Color = new Vector4(0, 1, 0, 1)
			};
			stack.AddControl(rect);
		}

		public void Unload()
		{
		}

		public void Update(double delta)
		{
		}
	}
}