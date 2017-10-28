using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenTK;
using OpenTK.Input;

namespace AmbitionUI.Controls.Containers
{
	public enum StackOrientation
	{
		Horizontal, Vertical
	}

	internal class StackControlInfo
	{
		public UIControl c;
		public int PosX;
		public int PosY;
	}

	public class UIStackControl : UIContainer
	{
		private List<StackControlInfo> Controls { get; set; }
		public StackOrientation Orientation { get; set; } = StackOrientation.Vertical;
		public bool UseRowSize { get; set; } = false;
		public int RowSize { get; set; } = 50;
		public bool DynamicRowSize { get; set; } = false;

		public UIStackControl(UISystem system) : this(system, null)
		{
		}

		public UIStackControl(UISystem system, UIControl parent) : base(system, parent)
		{
			Controls = new List<StackControlInfo>();
		}

		public override void AddControl(UIControl c)
		{
			if (c == null)
				return;
			Controls.Add(new StackControlInfo()
			{
				c = c,
				PosX = 0,
				PosY = 0
			});
			Resize(SizeX, SizeY);
		}

		public override void Draw(int PosX, int PosY)
		{
			base.Draw(PosX, PosY);
			DrawingUtils.DrawRect(PosX + MarginLeft, PosY + MarginTop, SizeX - MarginLeft - MarginRight, SizeY - MarginTop - MarginBottom, new Vector4(0.2f, 0.2f, 0.2f, 0.5f));
			foreach (StackControlInfo info in Controls)
			{
				UIControl c = info.c;
				c.Draw(PosX + info.PosX, PosY + info.PosY - c.SizeY);
			}
		}

		public override void DropControl(UIControl c)
		{
			if (c == null)
				return;
			for (int i = 0; i < Controls.Count; i++)
			{
				UIControl ic = Controls[i].c;
				if (ic == c)
				{
					Controls.RemoveAt(i);
					break;
				}
			}
		}

		public override void OnEnterFocus()
		{
			//Unused
		}

		public override void OnLeaveFocus()
		{
			//Unused
		}

		public override void OnMouseMove(MouseMoveEventArgs e)
		{
			int posX = e.X;
			int posY = BaseSystem.NativeWindow.Height - e.Y;
			foreach (StackControlInfo info in Controls)
			{
				if (posX >= LastPosX + info.PosX && posX <= LastPosX + info.PosX + info.c.SizeX &&
					posY >= LastPosY + info.PosY - info.c.SizeY && posY <= LastPosY + info.PosY)
				{
					info.c.OnMouseMove(e);
					break;
				}
			}
		}

		public override void OnMouseUp(MouseButtonEventArgs e)
		{
			Console.WriteLine("MouseUp: " + e.X + " " + e.Y);
			int posX = e.X;
			int posY = BaseSystem.NativeWindow.Height - e.Y;
			foreach (StackControlInfo info in Controls)
			{
				if (posX >= LastPosX + info.PosX && posX <= LastPosX + info.PosX + info.c.SizeX &&
					posY >= LastPosY + info.PosY - info.c.SizeY && posY <= LastPosY + info.PosY)
				{
					info.c.OnMouseUp(e);
					Console.WriteLine("Passing MouseUp");
					return;
				}
			}
			BaseSystem.FocusControl(null);
		}

		public override void Resize(int width, int height)
		{
			base.Resize(width, height);

			if (Controls.Count == 0)
				return;

			if (UseRowSize && DynamicRowSize)
				if (Orientation == StackOrientation.Vertical)
					RowSize = (SizeY - MarginTop - MarginBottom) / Controls.Count;
				else
					RowSize = (SizeX - MarginLeft - MarginRight) / Controls.Count;

			if (UseRowSize)
			{
				if (Orientation == StackOrientation.Vertical)
					foreach (StackControlInfo info in Controls)
					{
						UIControl c = info.c;
						c.Resize(width - MarginLeft - MarginRight, RowSize);
					}
				else
					foreach (StackControlInfo info in Controls)
					{
						UIControl c = info.c;
						c.Resize(RowSize, height - c.MarginTop - c.MarginBottom);
					}
			}
			else
			{
				if (Orientation == StackOrientation.Vertical)
				{
					int AvailableSpace = SizeY - MarginTop - MarginBottom;
					int IdealMinSpace = 0;
					foreach (StackControlInfo info in Controls)
					{
						UIControl c = info.c;
						if (c.VerticalSizePolicy == SizeMode.Expand)
							IdealMinSpace += 100000;
						else
							IdealMinSpace += c.MaxSizeY;
					}
					if (IdealMinSpace > AvailableSpace)
					{
						UIControl HighestPriorityControl = Controls[0].c;
						foreach (StackControlInfo info in Controls)
						{
							UIControl c = info.c;
							if (HighestPriorityControl == null || c.SizePriority > HighestPriorityControl.SizePriority)
								HighestPriorityControl = c;
						}
						foreach (StackControlInfo info in Controls)
						{
							UIControl c = info.c;
							if (c != HighestPriorityControl)
								c.Resize(width - MarginLeft - MarginRight, c.MinSizeY);
							else
								continue;
							AvailableSpace -= c.SizeY;
						}
						if (AvailableSpace < HighestPriorityControl.SizeY)
						{
							Debug.Print("WELP (Vertical).\n");
							return;
						}
						HighestPriorityControl.Resize(width - MarginLeft - MarginRight, AvailableSpace);
					}
					else
					{
						foreach (StackControlInfo info in Controls)
						{
							UIControl c = info.c;
							c.Resize(width - MarginLeft - MarginTop, c.MaxSizeY);
						}
					}
				}
				else
				{
					int AvailableSpace = SizeX - MarginLeft - MarginRight;
					int IdealMinSpace = 0;
					foreach (StackControlInfo info in Controls)
					{
						UIControl c = info.c;
						if (c.HorizontalSizePolicy == SizeMode.Expand)
							IdealMinSpace += 100000;
						else
							IdealMinSpace += c.MaxSizeX;
					}
					if (IdealMinSpace > AvailableSpace)
					{
						UIControl HighestPriorityControl = Controls[0].c;
						foreach (StackControlInfo info in Controls)
						{
							UIControl c = info.c;
							if (HighestPriorityControl == null || c.SizePriority > HighestPriorityControl.SizePriority)
								HighestPriorityControl = c;
						}
						foreach (StackControlInfo info in Controls)
						{
							UIControl c = info.c;
							if (c != HighestPriorityControl)
								c.Resize(c.MinSizeX, height - MarginTop - MarginBottom);
							else
								continue;
							AvailableSpace -= c.SizeX;
						}
						if (AvailableSpace < HighestPriorityControl.SizeY)
						{
							Debug.Print("WELP (Vertical).\n");
							return;
						}
						HighestPriorityControl.Resize(AvailableSpace, height - MarginTop - MarginBottom);
					}
					else
					{
						foreach (StackControlInfo info in Controls)
						{
							UIControl c = info.c;
							c.Resize(c.MaxSizeX, height - MarginTop - MarginBottom);
						}
					}
				}
			}
			int actualX = MarginLeft;
			int actualY = SizeY - MarginTop;
			for (int i = 0; i < Controls.Count; i++)
			{
				UIControl c = Controls[i].c;
				Controls[i].PosX = actualX;
				Controls[i].PosY = actualY;
				if (Orientation == StackOrientation.Vertical)
					actualY -= c.SizeY;
				else
					actualX += c.SizeX;
			}
		}
	}
}