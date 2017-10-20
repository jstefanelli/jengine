using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using jengine.Render.Shapes;
using jengine.Render.Wrappers;
using OpenTK;
using jengine.Render.GL3.Shapes;
using jengine.Render.GL3.Wrappers;
using jengine.Render.GL3.Shaders;
using jengine.Render.Scripts;

namespace jengine.Render.GL3
{
	public class GL3Render : IRender
	{
		private static string ExtensionString = "";

		public bool Loaded { get; private set; } = false;

		public void Draw(double deltaSec)
		{
		}

		public void Load()
		{
			if (Loaded)
				return;
			GL.Enable(EnableCap.DepthTest);
			GL.DepthFunc(DepthFunction.Lequal);

			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

			Loaded = true;
		}

		public void Resize(int width, int height)
		{
			GL.Viewport(0, 0, width, height);
		}

		public void Unload()
		{
		}

		public void Update()
		{
		}

		public static bool QueryExtension(string name)
		{
			if (ExtensionString.Length == 0)
				ExtensionString = GL.GetString(StringName.Extensions);
			return ExtensionString.Contains(name);
		}

		public void ResetFrameBuffer()
		{
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
			Resize(Game.Instance.Width, Game.Instance.Height);
		}

		public IShaderFactory ShaderFactory => GL3.ShaderFactory.Instance;

		public IShapeFactory ShapeFactory => GL3.ShapeFactory.Instance;

		public IRenderScriptFactory RenderScriptFactory => GL3.RenderScriptFactory.Instance;
	}
}