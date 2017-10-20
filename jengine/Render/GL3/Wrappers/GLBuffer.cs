using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using jengine.Render.Wrappers;

namespace jengine.Render.GL3.Wrappers
{
	public class GLBuffer : IBuffer
	{
		public const string TAG = "GL3/Wrapper/GLBuffer";

		public int Id { get; private set; } = 0;
		public int Epp { get; set; } = 3;
		public int Stride { get; set; } = 0;
		public int Offset { get; set; } = 0;
		public bool Loaded { get; private set; } = false;

		public GLBuffer()
		{
			Id = -1;
		}

		public void Load(float[] data, int epp)
		{
			Id = GL.GenBuffer();
			Epp = epp;
			Log.V(TAG, "Instantiated buffer: " + Id);
			GL.BindBuffer(BufferTarget.ArrayBuffer, Id);
			GL.BufferData(BufferTarget.ArrayBuffer, data.Length * 4, data, BufferUsageHint.StaticDraw);
			Loaded = true;
		}

		public void Unload()
		{
			GL.DeleteBuffer(Id);
			Log.V(TAG, "Deleted buffer: " + Id);
			Id = -1;
			Loaded = false;
		}

		public void Bind(int loc)
		{
			if (!Loaded)
			{
				return;
			}
			GL.BindBuffer(BufferTarget.ArrayBuffer, Id);
			GL.VertexAttribPointer(loc, Epp, VertexAttribPointerType.Float, false, Stride, Offset);
		}
	}
}