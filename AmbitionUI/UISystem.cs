using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing;
using AmbitionUI.Controls;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace AmbitionUI
{
	public enum SystemMode
	{
		OwnWindow, Injected
	}

	public class UISystem
	{
		public class SystemDrawArea
		{
			public bool Fill { get; set; } = true;
			public Rectangle Area { get; set; }

			public SystemDrawArea()
			{
				Fill = true;
			}

			public SystemDrawArea(Rectangle area)
			{
				Area = area;
				Fill = false;
			}
		}

		public GameWindow NativeWindow { get; private set; }
		public SystemMode Mode { get; private set; } = SystemMode.OwnWindow;
		public SystemDrawArea DrawArea { get; set; }
		public UIControl RootControl { get; set; }
		public UIControl FocusedControl { get; private set; }
		public Matrix4 ProjectionMatrix { get; private set; }

		private bool firstUpdate = true;

		public static UISystem InjectSystem(GameWindow w)
		{
			UISystem s = new UISystem(w);
			s.Mode = SystemMode.Injected;
			s.DrawArea = new SystemDrawArea();
			return s;
		}

		public static UISystem CreateWindow(int width, int height, string title = "AmbitionUI")
		{
			GameWindow w = new GameWindow(width, height, new GraphicsMode(new ColorFormat(32), 16, 0), title);
			UISystem s = new UISystem(w)
			{
				Mode = SystemMode.OwnWindow,
				DrawArea = new SystemDrawArea()
			};
			w.RenderFrame += s.NativeWindow_RenderFrame;
			w.Load += s.NativeWindow_Load;
			return s;
		}

		public void Run()
		{
			if (Mode == SystemMode.OwnWindow)
				NativeWindow.Run(30, 60);
		}

		private UISystem(GameWindow w)
		{
			this.NativeWindow = w;
			NativeWindow.UpdateFrame += NativeWindow_UpdateFrame;
			NativeWindow.MouseMove += NativeWindow_MouseMove;
			NativeWindow.MouseDown += NativeWindow_MouseDown;
			NativeWindow.MouseUp += NativeWindow_MouseUp;
			NativeWindow.Resize += NativeWindow_Resize;
		}

		private void Setup()
		{
			DrawingUtils.Load(this);
		}

		private void Resize()
		{
			Console.WriteLine("Resize");
			Console.WriteLine("Size: " + NativeWindow.Width + "x" + NativeWindow.Height);
			if (Mode == SystemMode.OwnWindow)
				GL.Viewport(0, 0, NativeWindow.Width, NativeWindow.Height);

			if (Mode == SystemMode.OwnWindow || DrawArea.Fill)
			{
				ProjectionMatrix = Matrix4.CreateOrthographicOffCenter(0, NativeWindow.Width, 0, NativeWindow.Height, 0, 4);
				RootControl.Resize(NativeWindow.Width, NativeWindow.Height);
			}
			else
			{
				ProjectionMatrix = Matrix4.CreateOrthographicOffCenter(DrawArea.Area.Left, DrawArea.Area.Right, DrawArea.Area.Bottom, DrawArea.Area.Top, 0, 4);
				RootControl.Resize(DrawArea.Area.Width, DrawArea.Area.Height);
			}
		}

		private void NativeWindow_Load(object sender, EventArgs args)
		{
			Console.WriteLine("Load");
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

			GL.Enable(EnableCap.DepthTest);
			GL.DepthFunc(DepthFunction.Lequal);

			GL.Disable(EnableCap.Lighting);

			Resize();
		}

		private void NativeWindow_Resize(object sender, EventArgs e)
		{
			Resize();
		}

		public void Draw()
		{
			if (Mode == SystemMode.OwnWindow)
			{
				GL.ClearColor(0.9f, 0.9f, 0.9f, 0);
				GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			}
			else
			{
				GL.UseProgram(0);
			}

			GL.GetInteger(GetPName.Viewport, Viewport);

			if (DrawArea.Fill)
			{
				GL.Viewport(0, 0, NativeWindow.Width, NativeWindow.Height);
			}
			else
			{
				GL.Viewport(DrawArea.Area);
			}

			RootControl.Draw(0, 0);

			if (Mode == SystemMode.OwnWindow)
			{
				NativeWindow.SwapBuffers();
			}
			else
			{
				GL.Viewport(Viewport[0], Viewport[1], Viewport[2], Viewport[3]);
			}
		}

		private int[] Viewport = new int[4];

		private void NativeWindow_RenderFrame(object sender, FrameEventArgs args)
		{
			Draw();
		}

		private void NativeWindow_UpdateFrame(object sender, FrameEventArgs e)
		{
			if (firstUpdate && Mode == SystemMode.Injected)
			{
				Setup();
				Resize();
				firstUpdate = false;
			}
			RootControl?.Update();
		}

		private void NativeWindow_MouseMove(object sender, OpenTK.Input.MouseMoveEventArgs e)
		{
			RootControl?.OnMouseMove(e);
		}

		private void NativeWindow_MouseDown(object sender, OpenTK.Input.MouseButtonEventArgs e)
		{
			RootControl?.OnMouseDown(e);
		}

		private void NativeWindow_MouseUp(object sender, OpenTK.Input.MouseButtonEventArgs e)
		{
			RootControl?.OnMouseUp(e);
		}

		public void FocusControl(UIControl c)
		{
			if (FocusedControl != null)
			{
				FocusedControl.OnLeaveFocus();
				Console.WriteLine("Chaning Focus");
			}
			if (c == null)
				return;
			FocusedControl = c;
			FocusedControl.OnEnterFocus();
		}
	}
}