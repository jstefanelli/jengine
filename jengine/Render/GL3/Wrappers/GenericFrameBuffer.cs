using jengine.Render.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace jengine.Render.GL3.Wrappers
{
	public class GenericFrameBuffer : IFrameBuffer
	{
		public const string TAG = "GL3/Wrappers/GenericFrameBuffer";

		public bool GenDepthBuffer { get; set; } = true;

		public int Id { get; private set; }

		public bool Loaded { get; private set; }

		public int Width { get; private set; }

		public int Height { get; private set; }

		public int TextureId { get; private set; }

		private int MultisampleLevel { get; set; } = 0;

		private int DepthBufferId { get; set; }

		public GenericFrameBuffer(int width, int height)
		{
			Id = -1;
			TextureId = -1;
			Width = width;
			Height = height;
			Loaded = false;
		}

		public GenericFrameBuffer() : this(Game.Instance.LoadedSettings.FrameResolutionX, Game.Instance.LoadedSettings.FrameResolutionY)
		{
		}

		public void Bind()
		{
			if (Loaded)
			{
				GL.BindFramebuffer(FramebufferTarget.Framebuffer, Id);
				Utils.LogError(TAG + " Bind 0");
				GL.Viewport(0, 0, Width, Height);
				Utils.LogError(TAG + " Bind 1");
			}
#if DEBUG
			else
				Log.W(TAG, "Trying to bind an unloaded frameBuffer");
#endif
		}

		public void Load(int MultiSampleLevel)
		{
			if (Loaded)
				return;
			Id = GL.GenFramebuffer();
			if (GenDepthBuffer)
				DepthBufferId = GL.GenRenderbuffer();
			TextureId = GL.GenTexture();

			GL.BindFramebuffer(FramebufferTarget.Framebuffer, Id);
			Utils.LogError(TAG + " Load 0");
			if (MultiSampleLevel == 0 || MultiSampleLevel % 2 != 0)
			{
				this.MultisampleLevel = 0;
				byte[] ImageData = new byte[Width * Height * 3];
				GL.BindTexture(TextureTarget.Texture2D, TextureId);
				Utils.LogError(TAG + " Load 1");
				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, Width, Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, ImageData);
				Utils.LogError(TAG + " Load 2");
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
				Utils.LogError(TAG + " Load 3");
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
				Utils.LogError(TAG + " Load 4");
				GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureId, 0);
				Utils.LogError(TAG + " Load 5");
				if (GenDepthBuffer)
				{
					GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, DepthBufferId);
					Utils.LogError(TAG + " Load 6");
					GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent, Width, Height);
					Utils.LogError(TAG + " Load 7");
				}
			}
			else
			{
				this.MultisampleLevel = MultiSampleLevel;
				GL.BindTexture(TextureTarget.Texture2DMultisample, TextureId);
				Utils.LogError(TAG + " Load 7");
				GL.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample, MultisampleLevel, PixelInternalFormat.Rgba8, Width, Height, true);
				Utils.LogError(TAG + " Load 8");
				GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2DMultisample, TextureId, 0);
				Utils.LogError(TAG + " Load 13");
				//GL.TexParameter(TextureTarget.Texture2DMultisample, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
				//Utils.LogError(TAG + " Load 9");
				//GL.TexParameter(TextureTarget.Texture2DMultisample, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
				//Utils.LogError(TAG + " Load 10");
				if (GenDepthBuffer)
				{
					GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, DepthBufferId);
					Utils.LogError(TAG + " Load 11");
					GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, MultisampleLevel, RenderbufferStorage.DepthComponent, Width, Height);
					Utils.LogError(TAG + " Load 12");
				}
			}
			if (GenDepthBuffer)
			{
				GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, DepthBufferId);
				Utils.LogError(TAG + " Load 14");
			}
			GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
			Utils.LogError(TAG + " Load 15");
			if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
			{
				Utils.LogError(TAG + " Load 16");
				Log.V(TAG, "Status: " + GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer).ToString());
				Log.E(TAG, "Initializetion Failed.");
				return;
			}
			else
			{
				Log.V(TAG, "Initialization OK");
			}

			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
			Utils.LogError(TAG + " Load 17");
			Loaded = true;
		}

		public void Load()
		{
			Load(Game.Instance.LoadedSettings.MSAALevel);
		}

		public void Unload()
		{
			if (!Loaded)
				return;
			GL.DeleteFramebuffer(Id);
			if (GenDepthBuffer)
				GL.DeleteRenderbuffer(DepthBufferId);
			GL.DeleteTexture(TextureId);
			Id = -1;
			TextureId = -1;
			Loaded = false;
		}
	}
}