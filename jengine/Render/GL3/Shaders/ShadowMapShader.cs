using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jengine.Cameras;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using jengine.Render.GL3.Wrappers;

namespace jengine.Render.GL3.Shaders
{
	public class ShadowMapShader : IShader
	{
		public const string TAG = "GL3/Shaders/ShadowMap";

		private static ShadowMapShader _Instance = null;

		public static ShadowMapShader Instance
		{
			get
			{
				if (_Instance == null)
					_Instance = new ShadowMapShader();
				return _Instance;
			}
		}

		public int Id { get; private set; }
		public int PosLoc { get; private set; }
		public int MvLoc { get; private set; }
		public int ModelLoc { get; private set; }

		private ShadowMapShader()
		{
			Loaded = false;
		}

		public bool Loaded { get; private set; }

		public void Draw(List<IDrawable> drawables, ICamera camera)
		{
			GL.EnableVertexAttribArray(PosLoc);
			foreach (IDrawable d in drawables)
			{
				if (d.Vertices == null)
				{
					Log.E(TAG, "Drawable has no vertices. Skipping render");
					continue;
				}
				Matrix4 model = MathUtils.GenDrawableModelMatrix(d);
				GL.UniformMatrix4(ModelLoc, false, ref model);
				((GLBuffer)d.Vertices).Bind(PosLoc);
				d.Draw();
			}
			GL.DisableVertexAttribArray(PosLoc);
		}

		public void Load()
		{
			if (Loaded)
				return;
			Id = ShaderUtils.CompileShader(@"DefaultAssets\Shaders\shadowMapv.glsl", @"DefaultAssets\Shaders\shadowMapf.glsl");
			PosLoc = GL.GetAttribLocation(Id, "aPos");
			MvLoc = GL.GetUniformLocation(Id, "uMV");
			ModelLoc = GL.GetUniformLocation(Id, "uModel");
			Loaded = true;
		}

		public void Unload()
		{
			GL.DeleteProgram(Id);
			Id = -1;
			Loaded = false;
		}
	}
}