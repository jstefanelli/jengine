using jengine.Render.GL3.Wrappers;
using jengine.Render.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jengine.Render.Wrappers;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using jengine.Cameras;
using jengine.Render.GL3.Shaders;

namespace jengine.Render.GL3.Scripts
{
	public class ShadowMapScript : IRenderScript
	{
		public interface IProjectionMatrixProvider
		{
			Matrix4 GetProjectionMatrixNearFar(float zNear, float zFar);
		}

		public const string TAG = "GL3/Scripts/ShadowMapScript";
		public const int CASCADE_NUMBER = 3;

		public GenericFrameBuffer FrameBuffer { get; private set; }
		private IFrameBuffer[] shadowMaps;
		public FrameBufferDrawable Drawable { get; private set; }
		private List<IDrawable> drawableList;

		public int FrameBufferSizeX { get; set; } = Game.Instance.LoadedSettings.FrameResolutionX;
		public int FrameBufferSizeY { get; set; } = Game.Instance.LoadedSettings.FrameResolutionX;
		public IProjectionMatrixProvider MatrixProvider { get; set; } = null;

		public int MSAALevel { get; set; } = Game.Instance.LoadedSettings.MSAALevel;

		public bool DrawToWindow { get; set; } = true;
		public bool Loaded { get; set; }
		public Vector3 LightDir { get; set; } = new Vector3(1000.0f, -1000.0f, 1000.0f);

		private float[] CascadeThresholds;
		private Matrix4[] LightMatrices;

		public ShadowMapScript()
		{
			Loaded = false;
			drawableList = new List<IDrawable>();
		}

		public void Load(IRender render)
		{
			if (Loaded)
				return;
			CascadeThresholds = new float[CASCADE_NUMBER] { Game.Instance.LoadedSettings.FarPlane / 5, Game.Instance.LoadedSettings.FarPlane / 2, Game.Instance.LoadedSettings.FarPlane };
			LightMatrices = new Matrix4[CASCADE_NUMBER];
			render.ShaderFactory.Simple2DTextured.Load();
			render.ShaderFactory.ShadowMap.Load();
			shadowMaps = new IFrameBuffer[CASCADE_NUMBER];
			for (int i = 0; i < CASCADE_NUMBER; i++)
			{
				shadowMaps[i] = new ShadowMapFrameBuffer(Game.Instance.LoadedSettings.ShadowMapResolution, Game.Instance.LoadedSettings.ShadowMapResolution);
				shadowMaps[i].Load();
			}
			FrameBuffer = new GenericFrameBuffer(FrameBufferSizeX, FrameBufferSizeY);
			FrameBuffer.Load(MSAALevel);
			Drawable = new FrameBufferDrawable(FrameBuffer);
			Drawable.Load();
			drawableList.Add(Drawable);
			render.ShaderFactory.Phong.Load();
			PhongShader p = (PhongShader)render.ShaderFactory.Phong;
			Texture2D[] frameBufferTextures = new Texture2D[CASCADE_NUMBER] { new Texture2D(((ShadowMapFrameBuffer)shadowMaps[0]).TextureId), new Texture2D(((ShadowMapFrameBuffer)shadowMaps[1]).TextureId), new Texture2D(((ShadowMapFrameBuffer)shadowMaps[2]).TextureId) };
			p.PostLoad(frameBufferTextures, CascadeThresholds);
			Loaded = true;
		}

		private void GenerateLightMatrix(Matrix4 view, int cascade)
		{
			//Refreshing LightMatrix
			//Get the 8 corners of the viewport in worldspace
			float FarPlane = Game.Instance.LoadedSettings.FarPlane;

			//Get the renderer's projection matrix
			Matrix4 proj = (MatrixProvider != null ? MatrixProvider.GetProjectionMatrixNearFar((cascade == 0) ? Game.Instance.LoadedSettings.NearPlane : CascadeThresholds[cascade - 1], CascadeThresholds[cascade]) : Matrix4.CreatePerspectiveFieldOfView((float)Math.PI * Game.Instance.LoadedSettings.FOVDegrees / 180.0f, Game.Instance.Width / (float)Game.Instance.Height, (cascade == 0) ? Game.Instance.LoadedSettings.NearPlane : CascadeThresholds[cascade - 1], CascadeThresholds[cascade]));
			Matrix4 projView = view * proj;

			//Invert it
			Matrix4 invProjView = Matrix4.Invert(projView);

			//Corner position in viewport space
			Vector4[] corners = new Vector4[8]{ new Vector4(-1.0f, -1.0f, 1.0f, 1.0f), new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(-1.0f, 1.0f, 1.0f, 1.0f), new Vector4(-1.0f, 1.0f, -1.0f, 1.0f),
						new Vector4(1.0f, -1.0f, 1.0f, 1.0f), new Vector4(1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f), new Vector4(1.0f, 1.0f, -1.0f, 1.0f) };

			//Get the worldSpace positions of the corners
			for (int i = 0; i < 8; i++)
			{
				corners[i] = corners[i] * invProjView;
				corners[i] /= corners[i].W;
			}

			//calculate centeroid
			Vector3 centeroid = new Vector3();
			foreach (Vector4 v in corners)
				centeroid += v.Xyz;
			centeroid /= 8;

			//Normalize LightDirection
			Vector3 myLightDir = LightDir;
			myLightDir.Normalize();

			//Calculate Radius
			float radius = (corners[0].Xyz - corners[6].Xyz).Length / 2;

			float texelPerUnit = ((ShadowMapFrameBuffer)shadowMaps[cascade]).Width / (radius * 2);

			//Clamp centeroid in texel-size movemennts
			Matrix4 scalarMatrix = Matrix4.CreateScale(texelPerUnit);
			Matrix4 simpleViewMatrx = Matrix4.LookAt(Vector3.Zero, -myLightDir, new Vector3(0, 1, 0));

			simpleViewMatrx = simpleViewMatrx * scalarMatrix;
			Vector4 centeroidTmp = (new Vector4(centeroid, 0) * simpleViewMatrx);
			centeroidTmp.X = (float)Math.Round(centeroidTmp.X);
			centeroidTmp.Y = (float)Math.Round(centeroidTmp.Y);
			simpleViewMatrx.Invert();
			centeroid = (centeroidTmp * simpleViewMatrx).Xyz;

			Vector3 eye = centeroid - (myLightDir * radius);

			Matrix4 newLightViewMatrx = Matrix4.LookAt(eye, centeroid, new Vector3(0, 1, 0));

			Matrix4 LightProjection = Matrix4.CreateOrthographicOffCenter(-radius, radius, -radius, radius, -radius * 3, radius * 3);
			LightMatrices[cascade] = newLightViewMatrx * LightProjection;
		}

		public void Draw(List<IDrawable> drawables, IRender render, ICamera camera)
		{
			if (!Loaded)
			{
#if DEBUG
				Log.E(TAG, "Trying to render an unloaded ShadowMapScript.");
				Log.W(TAG, "Current configuration (Debug) will load the script in Draw(). THIS WILL NOT HAPPEN IN PRODUCTION (Release)");
				Load(render);
#else
				Log.E(TAG, "Trying to render an unloaded ShadowMapScript. Skipping render.");
				return;
#endif
			}
			for (int i = 0; i < CASCADE_NUMBER; i++)
			{
				GenerateLightMatrix(camera.View, i);
				shadowMaps[i].Bind();
				GL.Clear(ClearBufferMask.DepthBufferBit);
				ShadowMapShader s = (ShadowMapShader)render.ShaderFactory.ShadowMap;
				GL.UseProgram(s.Id);
				GL.UniformMatrix4(s.MvLoc, false, ref LightMatrices[i]);
				s.Draw(drawables, null);
			}

			FrameBuffer.Bind();
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			PhongShader phong = (PhongShader)render.ShaderFactory.Phong;
			phong.LightPosition = this.LightDir;
			phong.Prepare(LightMatrices);
			phong.Draw(drawables, camera);
			render.ResetFrameBuffer();
			if (DrawToWindow)
			{
				((Simple2DTexturedShader)render.ShaderFactory.Simple2DTextured).MSAALevel = MSAALevel;
				render.ShaderFactory.Simple2DTextured.Draw(drawableList, null);
			}
		}

		public void Unload()
		{
			Loaded = false;
			Drawable.Unload();
			FrameBuffer.Unload();
		}

		public void Update()
		{
		}
	}
}