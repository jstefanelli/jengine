using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using AmbitionUI.Render.Shaders;

namespace AmbitionUI
{
	public static class DrawingUtils
	{
		private static SHPlainColor PlainColorShader;

		public static void Load(UISystem s)
		{
			PlainColorShader = new SHPlainColor(s);
			PlainColorShader.Load();
		}

		public static void DrawRect(int PosX, int PosY, int SizeX, int SizeY, Vector4 color)
		{
			PlainColorShader.DrawRectangle(PosX, PosY, SizeX, SizeY, color);
		}
	}
}