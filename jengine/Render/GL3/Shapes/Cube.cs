using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jengine.Render.Wrappers;
using OpenTK;
using jengine.Render.GL3.Wrappers;
using OpenTK.Graphics.OpenGL;

namespace jengine.Render.GL3.Shapes
{
	public class Cube : IDrawable
	{
		public DrawableGroup Parent { get; set; }

		public const string TAG = "GL3/Shapes/Cube";
		private static GLBuffer pts;
		private static GLBuffer norms;
		private static GLBuffer texCoords;
		private static bool staticLoaded = false;

		private static void StaticLoad()
		{
			Log.V(TAG, "StaticLoaded");
			float[] points = new float[]
			{// 0, 2, 1
				-0.5f, 0.5f, -0.5f,
				0.5f, -0.5f, -0.5f,
				-0.5f, -0.5f, -0.5f,

                //2, 0, 3
				0.5f, -0.5f, -0.5f,
				-0.5f, 0.5f, -0.5f,
				0.5f, 0.5f, -0.5f,

                //4, 5, 6
				-0.5f, 0.5f, 0.5f,
				-0.5f, -0.5f, 0.5f,
				0.5f, -0.5f, 0.5f,

                //4, 6, 7
				-0.5f, 0.5f, 0.5f,
				0.5f, -0.5f, 0.5f,
				0.5f, 0.5f, 0.5f,

                //0, 5, 4
				-0.5f, 0.5f, -0.5f,
				-0.5f, -0.5f, 0.5f,
				-0.5f, 0.5f, 0.5f,

                //0, 1, 5
				-0.5f, 0.5f, -0.5f,
				-0.5f, -0.5f, -0.5f,
				-0.5f, -0.5f, 0.5f,

                //3, 6, 2
				0.5f, 0.5f, -0.5f,
				0.5f, -0.5f, 0.5f,
				0.5f, -0.5f, -0.5f,

                //3, 7, 6
				0.5f, 0.5f, -0.5f,
				0.5f, 0.5f, 0.5f,
				0.5f, -0.5f, 0.5f,

                //0, 4, 3
				-0.5f, 0.5f, -0.5f,
				-0.5f, 0.5f, 0.5f,
				0.5f, 0.5f, -0.5f,

                //3, 4, 7
				0.5f, 0.5f, -0.5f,
				-0.5f, 0.5f, 0.5f,
				0.5f, 0.5f, 0.5f,

                //1, 2, 5
				-0.5f, -0.5f, -0.5f,
				0.5f, -0.5f, -0.5f,
				-0.5f, -0.5f, 0.5f,

                //2, 5, 6
				0.5f, -0.5f, -0.5f,
				0.5f, -0.5f, 0.5f,
				-0.5f, -0.5f, 0.5f,
			};
			float[] normals = new float[]
			{// 0, 2, 1
				0, 0, -1,
				0, -0, -1f,
				0, -0, -1f,

                //2, 0, 3
				0, 0, -1,
				0, 0, -1,
				0, 0, -1,

                //4, 5, 6
				0, 0, 1,
				0, 0, 1,
				0, 0, 1,

                //4, 6, 7
				0, 0, 1,
				0, 0, 1,
				0, 0, 1,

                //0, 5, 4
				-1, 0, -0,
				-1, -0, 0,
				-1, 0, 0,

                //0, 1, 5
				-1, 0, -0,
				-1, -0, -0,
				-1, -0, 0,

                //3, 6, 2
				1, 0, -0,
				1, -0, 0,
				1, -0, -0,

                //3, 7, 6
				1, 0, -0,
				1, 0, 0,
				1, -0, 0,

                //0, 4, 3
				-0, 1, -0,
				-0, 1, 0,
				0, 1, -0,

                //3, 4, 7
				0, 1, -0,
				-0, 1, 0,
				0, 1, 0,

                //1, 2, 5
				-0, -1, -0,
				0, -1, -0,
				-0, -1, 0,

                //2, 5, 6
				0, -1, -0,
				0, -1, 0,
				-0, -1, 0,
			};
			pts = new GLBuffer();
			pts.Load(points, 3);
			norms = new GLBuffer();
			norms.Load(normals, 3);
			staticLoaded = true;
		}

		private static void StaticUnload()
		{
			if (!staticLoaded)
				return;
			Log.V(TAG, "StaticUnloaded");
			pts.Unload();
			norms.Unload();
			staticLoaded = false;
		}

		public IBuffer Vertices => pts;

		public IBuffer Normals => norms;

		public IBuffer TexCoords => texCoords;

		public Material Mat { get; private set; }

		public Vector3 Position { get; set; }

		public Quaternion Orientation { get; set; }

		public Vector3 Scale { get; set; }

		public bool Loaded { get; private set; }

		public void Load()
		{
			if (!staticLoaded)
			{
				StaticLoad();
			}
			Loaded = true;
		}

		public void Draw()
		{
			GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
			Utils.LogError(TAG + " Draw 0");
		}

		public void Unload()
		{
		}

		public Cube(Vector3 size, Vector4 color)
		{
			Loaded = false;
			Scale = size;
			Mat = new Material
			{
				DiffuseColor = color,
				AmbientColor = color / 10
			};

			Position = new Vector3(0, 0, 0);

			Orientation = Quaternion.Identity;
		}
	}
}