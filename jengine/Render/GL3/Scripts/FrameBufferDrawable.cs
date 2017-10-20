using jengine.Render.GL3.Wrappers;
using jengine.Render.Wrappers;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jengine.Render.GL3.Scripts
{
	public class FrameBufferDrawable : IDrawable
	{
		public DrawableGroup Parent { get; set; }

		public const string TAG = "GL3/Scripts/ShadowMapScript/FrameBufferDrawable";

		private IFrameBuffer frameBuffer;

		private GLBuffer FrameBufferPoints;
		private GLBuffer FrameBufferTexCoords;

		public IBuffer Vertices => FrameBufferPoints;

		public IBuffer Normals => null;

		public IBuffer TexCoords => FrameBufferTexCoords;

		public Material Mat { get; private set; }

		public Vector3 Position { get; set; }
		public Quaternion Orientation { get; set; }
		public Vector3 Scale { get; set; }

		public bool Loaded { get; private set; }

		public FrameBufferDrawable(IFrameBuffer buffer)
		{
			frameBuffer = buffer;
			Mat = new Material()
			{
				DiffuseTexture = new Texture2D(((GenericFrameBuffer)frameBuffer).TextureId)
			};
			Utils.LogError(TAG + "Init 0");
		}

		public void Draw()
		{
			if (!Loaded)
			{
				Log.E(TAG, "Not Loaded");
				return;
			}
			GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
			Utils.LogError(TAG + " Draw 0");
		}

		public void Load()
		{
			if (Loaded)
				return;
			Log.V(TAG, "Loading...");
			frameBuffer.Load();
			float[] framePoints = new float[]
			{
					-1.0f, 1.0f,
					1.0f, 1.0f,
					-1.0f, -1.0f,

					1.0f, 1.0f,
					1.0f, -1.0f,
					-1.0f, -1.0f
			};

			FrameBufferPoints = new GLBuffer();
			FrameBufferPoints.Load(framePoints, 2);

			float[] frameTex = new float[]
			{
					0.0f, 1.0f,
					1f, 1f,
					0f, 0f,

					1f, 1f,
					1f, 0f,
					0f, 0f
			};

			FrameBufferTexCoords = new GLBuffer();
			FrameBufferTexCoords.Load(frameTex, 2);

			Loaded = true;
		}

		public void Unload()
		{
			Log.V(TAG, "Unloading");
			FrameBufferPoints.Unload();
			FrameBufferTexCoords.Unload();
		}
	}
}