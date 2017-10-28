using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace AmbitionUI.Render.Shaders
{
	public class SHPlainColor : UIShader
	{
		public int uPositionLoc { get; protected set; }
		public int uSizeLoc { get; protected set; }
		public int uColorLoc { get; protected set; }

		public int aPoints { get; protected set; }

		private int RectangleBuffer;

		public SHPlainColor(UISystem system) : base(system)
		{
		}

		public override void Load()
		{
			if (Loaded)
				return;
			Id = ShaderUtils.CompileShader("AmbitionUI/MinAssets/Shaders/PlainColorV.glsl", "AmbitionUI/MinAssets/Shaders/PlainColorF.glsl");

			uPositionLoc = GL.GetUniformLocation(Id, "uPosition");
			uSizeLoc = GL.GetUniformLocation(Id, "uSize");
			uColorLoc = GL.GetUniformLocation(Id, "uColor");
			MatrixPosition = GL.GetUniformLocation(Id, "uProjection");

			aPoints = GL.GetAttribLocation(Id, "aPoints");

			float[] rectPoints = new float[]
			{
				0.0f, 0.0f,
				0.0f, 1.0f,
				1.0f, 1.0f,

				1.0f, 1.0f,
				1.0f, 0.0f,
				0.0f, 0.0f
			};

			RectangleBuffer = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, RectangleBuffer);
			GL.BufferData(BufferTarget.ArrayBuffer, rectPoints.Length * sizeof(float), rectPoints, BufferUsageHint.StaticDraw);

			Console.WriteLine("Loaded with ids: " + Id + " " + RectangleBuffer);

			Loaded = true;
		}

		public void DrawRectangle(int PosX, int PosY, int SizeX, int SizeY, Vector4 color)
		{
			if (!Loaded)
				Load();
			GL.UseProgram(Id);
			Matrix4 matrix = BaseSystem.ProjectionMatrix;
			GL.UniformMatrix4(MatrixPosition, false, ref matrix);

			GL.Uniform2(uPositionLoc, new Vector2(PosX, PosY));
			GL.Uniform2(uSizeLoc, new Vector2(SizeX, SizeY));
			GL.Uniform4(uColorLoc, color);

			GL.EnableVertexAttribArray(aPoints);

			GL.BindBuffer(BufferTarget.ArrayBuffer, RectangleBuffer);
			GL.VertexAttribPointer(aPoints, 2, VertexAttribPointerType.Float, false, 0, 0);

			GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

			GL.DisableVertexAttribArray(aPoints);
		}

		public override void Unload()
		{
			if (!Loaded)
				return;
			GL.DeleteProgram(Id);
		}
	}
}