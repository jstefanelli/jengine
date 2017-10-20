using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using System.IO;
using OpenTK;
using jengine.Render.GL3.Wrappers;
using jengine.Render.Wrappers;
using jengine.Cameras;

namespace jengine.Render.GL3.Shaders
{
	public class Simple3DShader : IShader
	{
		private static Simple3DShader _instance = null;

		public static Simple3DShader Instance
		{
			get
			{
				if (_instance == null)
					_instance = new Simple3DShader();
				return _instance;
			}
		}

		public const string TAG = "GL3/Shaders/Simple3D";

		public int Id { get; private set; }
		public bool Loaded { get; private set; }
		public int MVPLoc { get; private set; }
		public int PosLoc { get; private set; }
		public int ColLoc { get; private set; }

		private Simple3DShader()
		{
			Log.V(TAG, "Instantiated new Simple3D");
			Loaded = false;
		}

		public void Draw(List<IDrawable> drawables, ICamera camera)
		{
			if (camera == null)
			{
				Log.E(TAG, "Skipping render. Missing camera");
				return;
			}
			if (!Loaded)
			{
				Log.E(TAG, "Shader is not loaded");
#if DEBUG
				Load();
#else
				return;
#endif
			}

			Matrix4 cameraMvp = Matrix4.Mult(MathUtils.GenCameraModel(camera), camera.View);
			cameraMvp = Matrix4.Mult(cameraMvp, camera.Projection);
			GL.UseProgram(Id);
			Utils.LogError(TAG + "/Draw0");
			GL.EnableVertexAttribArray(PosLoc);
			Utils.LogError(TAG + "/Draw1");
			foreach (IDrawable d in drawables)
			{
				GL.Uniform4(ColLoc, d.Mat.DiffuseColor);
				Utils.LogError(TAG + "/Draw2");
				Matrix4 model = Matrix4.Mult(MathUtils.GenDrawableModelMatrix(d), cameraMvp);
				GL.UniformMatrix4(MVPLoc, false, ref model);
				Utils.LogError(TAG + "/Draw3");
				IBuffer PosBuffer = d.Vertices;
				if (PosBuffer == null)
				{
					Log.E(TAG, "Cannot draw Drawable " + d.ToString() + ". No Vertex Buffer");
					continue;
				}

				((GLBuffer)PosBuffer).Bind(PosLoc);
				Utils.LogError(TAG + "/Draw4");
				d.Draw();
				Utils.LogError(TAG + "/Draw5");
			}
			GL.DisableVertexAttribArray(PosLoc);
			Utils.LogError(TAG + "/Draw6");
		}

		public void Load()
		{
			Id = ShaderUtils.CompileShader(@"DefaultAssets\Shaders\simple3Dv.glsl", @"DefaultAssets\Shaders\simple3Df.glsl");
			Log.V(TAG, "Generated program: " + Id);
			MVPLoc = GL.GetUniformLocation(Id, "uMVP");
			Log.V(TAG, "MVPLoc: " + MVPLoc);
			PosLoc = GL.GetAttribLocation(Id, "aPos");
			Log.V(TAG, "PosLoc: " + PosLoc);
			ColLoc = GL.GetUniformLocation(Id, "uCol");
			Log.V(TAG, "ColLoc: " + ColLoc);
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