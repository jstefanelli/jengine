using jengine.Render.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace jengine.Render.GL3.Wrappers
{
	public class ShadowMapFrameBuffer : IFrameBuffer
	{
		private const string TAG = "GL3/Wrappers/ShadowMapFrameBuffer";

		public bool Loaded { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }
		public int Id { get; private set; }
		public int TextureId { get; private set; }

		public ShadowMapFrameBuffer(int Width, int Height)
		{
			this.Width = Width;
			this.Height = Height;
			Log.V(TAG, "Instanced");
		}

		public ShadowMapFrameBuffer() : this(Game.Instance.LoadedSettings.ShadowMapResolution, Game.Instance.LoadedSettings.ShadowMapResolution)
		{
		}

		public void Bind()
		{
			if (!Loaded)
			{
				Log.W(TAG, "Trying to bind an unloaded framebuffer");
				return;
			}
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, Id);
			Utils.LogError(TAG + " Bind 0");
			GL.Viewport(0, 0, Width, Height);
			Utils.LogError(TAG + " Bind 1");
		}

		public void Load()
		{
			if (Loaded)
			{
				Log.E(TAG, "Called Load() when already loaded");
				return;
			}
			Id = GL.GenFramebuffer();
			Utils.LogError(TAG + " Load 0");
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, Id);
			Utils.LogError(TAG + " Load 1");
			float[] ImageData = new float[Width * Height];

			TextureId = GL.GenTexture();
			Utils.LogError(TAG + " Load 2");
			GL.BindTexture(TextureTarget.Texture2D, TextureId);
			Utils.LogError(TAG + " Load 3");

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent, Width, Height, 0, PixelFormat.DepthComponent, PixelType.Float, ImageData);
			Utils.LogError(TAG + " Load 4");
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			Utils.LogError(TAG + " Load 5");
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			Utils.LogError(TAG + " Load 6");
			float[] borders = new float[] { 1f, 1f, 1f, 1f };
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
			Utils.LogError(TAG + " Load 7");
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
			Utils.LogError(TAG + " Load 8");
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, borders);
			Utils.LogError(TAG + " Load 9");

			GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureId, 0);
			Utils.LogError(TAG + " Load 10");
			GL.DrawBuffer(DrawBufferMode.None);
			Utils.LogError(TAG + " Load 11");

			if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
			{
				Utils.LogError(TAG + " Load 12");
				Log.V(TAG, "Initializetion Failed.");
			}
			else
			{
				Log.V(TAG, "Initialization OK. Id: " + Id);
			}
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
			Utils.LogError(TAG + " Load 13");
			Loaded = true;
		}

		public void Unload()
		{
			GL.DeleteTexture(TextureId);
			GL.DeleteFramebuffer(Id);
			Loaded = false;
		}
	}
}