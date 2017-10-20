using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jengine.Render.Wrappers;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using jengine.Render.GL3.Wrappers;

namespace jengine.Render.GL3.Shapes
{
	public class Mesh : IDrawable
	{
		public const string TAG = "GL3/Shapes/Mesh";

		public DrawableGroup Parent { get; set; }

		private float[] verts;
		public IBuffer Vertices { get; private set; }

		private float[] norms;
		public IBuffer Normals { get; private set; }

		private float[] texCoords;
		public IBuffer TexCoords { get; private set; }

		public Material Mat { get; private set; }

		public Vector3 Position { get; set; }
		public Quaternion Orientation { get; set; }
		public Vector3 Scale { get; set; }

		public bool Loaded { get; private set; }

		protected int Triangles { get; set; } = 0;

		public Mesh(int triangles, float[] vrts, float[] nrms, float[] txt, Material m)
		{
			Loaded = false;
			Triangles = triangles;
			verts = vrts;
			norms = nrms;
			texCoords = txt;
			Mat = m;
		}

		public void Draw()
		{
			GL.DrawArrays(PrimitiveType.Triangles, 0, Triangles * 3);
		}

		public void Load()
		{
			if (Loaded)
				return;

			Vertices = new GLBuffer();
			Vertices.Load(verts, 3);

			Normals = new GLBuffer();
			Normals.Load(norms, 3);

			TexCoords = new GLBuffer();
			TexCoords.Load(texCoords, 2);
			Log.W(TAG, "Loading generic MESH.");
			Loaded = true;
		}

		public void Unload()
		{
			if (!Loaded)
				return;
			Vertices.Unload();
			Normals.Unload();
			TexCoords.Unload();
			Loaded = false;
		}
	}
}