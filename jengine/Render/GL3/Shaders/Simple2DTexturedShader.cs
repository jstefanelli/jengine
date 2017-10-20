using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jengine.Cameras;
using OpenTK.Graphics.OpenGL;
using jengine.Render.GL3.Wrappers;

namespace jengine.Render.GL3.Shaders
{
	public class Simple2DTexturedShader : IShader
	{
		public const string TAG = "GL3/Shaders/Simple2D";

		private static Simple2DTexturedShader _instance = null;

		public static Simple2DTexturedShader Instance
		{
			get
			{
				if (_instance == null)
					_instance = new Simple2DTexturedShader();
				return _instance;
			}
		}

		public int Id { get; private set; }
		public bool Loaded { get; private set; }
		public int PosLoc { get; private set; }
		public int TexCoordLoc { get; private set; }
		public int TxtLoc { get; private set; }
		public int TxtMsLoc { get; private set; }
		public int MsLoc { get; private set; }
		public int MSAALevel { get; set; } = Game.Instance.LoadedSettings.MSAALevel;

		private Simple2DTexturedShader()
		{
			Loaded = false;
			Id = -1;
			Log.V(TAG, "Instantiated");
		}

		public void Draw(List<IDrawable> drawables, ICamera camera)
		{
			GL.UseProgram(Id);
			Utils.LogError(TAG + "Draw 0");
			int MSAALevel = this.MSAALevel;
			GL.Uniform1(MsLoc, MSAALevel);
			Utils.LogError(TAG + "Draw 1");
			if (MSAALevel != 0)
			{
				GL.Uniform1(TxtLoc, 1);
				Utils.LogError(TAG + "Draw 2");
				GL.Uniform1(TxtMsLoc, 0);
				Utils.LogError(TAG + "Draw 3");
			}
			else
			{
				GL.Uniform1(TxtLoc, 0);
				Utils.LogError(TAG + "Draw 4");
				GL.Uniform1(TxtMsLoc, 1);
				Utils.LogError(TAG + "Draw 5");
			}
			GL.EnableVertexAttribArray(PosLoc);
			Utils.LogError(TAG + "Draw 6");
			GL.EnableVertexAttribArray(TexCoordLoc);
			Utils.LogError(TAG + "Draw 7");
			foreach (IDrawable d in drawables)
			{
				if (d == null)
					return;
				if (d.Mat == null || d.Mat.DiffuseTexture == null)
				{
					Log.E(TAG, "Trying to render a drawable without texture");
					continue;
				}
				if (d.Vertices == null || d.TexCoords == null)
				{
					Log.E(TAG, "Trying to render a drawable without vertices and/or texCoords");
					continue;
				}
				GL.ActiveTexture(TextureUnit.Texture0);
				Utils.LogError(TAG + "Draw 8");
				if (MSAALevel != 0)
				{
					GL.BindTexture(TextureTarget.Texture2DMultisample, ((Texture2D)d.Mat.DiffuseTexture).Id);
					Utils.LogError(TAG + "Draw 9");
				}
				else
				{
					GL.BindTexture(TextureTarget.Texture2D, ((Texture2D)d.Mat.DiffuseTexture).Id);
					Utils.LogError(TAG + "Draw 10");
				}
				((GLBuffer)d.Vertices).Bind(PosLoc);
				Utils.LogError(TAG + "Draw 11");
				((GLBuffer)d.TexCoords).Bind(TexCoordLoc);
				Utils.LogError(TAG + "Draw 12");
				d.Draw();
			}
			GL.DisableVertexAttribArray(PosLoc);
			Utils.LogError(TAG + "Draw 13");
			GL.DisableVertexAttribArray(TexCoordLoc);
			Utils.LogError(TAG + "Draw 14");
		}

		public void Load()
		{
			if (Loaded)
				return;
			Id = ShaderUtils.CompileShader(@"DefaultAssets\Shaders\simple2DTexv.glsl", @"DefaultAssets\Shaders\simple2DTexf.glsl");
			Log.V(TAG, "Generated program: " + Id);
			PosLoc = GL.GetAttribLocation(Id, "aPos");
			Log.V(TAG, "PosLoc: " + PosLoc);
			TexCoordLoc = GL.GetAttribLocation(Id, "aTexCoord");
			Log.V(TAG, "TexCoordLoc: " + TexCoordLoc);
			TxtLoc = GL.GetUniformLocation(Id, "uTxt");
			Log.V(TAG, "TxtLoc: " + TxtLoc);
			TxtMsLoc = GL.GetUniformLocation(Id, "uTxtMs");
			Log.V(TAG, "TxtMsLoc: " + TxtMsLoc);
			MsLoc = GL.GetUniformLocation(Id, "uMs");
			Log.V(TAG, "MsLoc: " + MsLoc);
			Loaded = true;
		}

		public void Unload()
		{
		}
	}
}