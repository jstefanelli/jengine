using jengine.Cameras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using jengine.Render.GL3.Wrappers;
using OpenTK;
using jengine.Render.GL3.Scripts;

namespace jengine.Render.GL3.Shaders
{
	public class PhongShader : IShader
	{
		//Texture Unit organization:
		/*
		 * 0: Diffuse Texture
		 * 1: Ambient Texture
		 * 2: Specular Texture
		 * 3: Normal Map
		 * 4: Shadow Map Cascade 0
		 * 5: Shadow Map Cascade 1
		 * 6: Shadow Map Cascade 2
		 */

		public const string TAG = "GL3/Shaders/Phong";

		private static PhongShader _Instance = null;

		public static PhongShader Instance
		{
			get
			{
				if (_Instance == null)
					_Instance = new PhongShader();
				return _Instance;
			}
		}

		public int Id { get; private set; }

		//Vertex Shader

		private int aPosLoc;
		private int aTexCoordLoc;
		private int aNormLoc;

		private int uProjLoc;
		private int uViewLoc;
		private int uCameraLoc;
		private int uModelLoc;
		private int uNormalLoc;
		private int uLightMatrix0Loc;
		private int uLightMatrix1Loc;
		private int uLightMatrix2Loc;

		//Fragment Shader

		private int uLightPosLoc;
		private int uShadowMapsLoc;
		private int cascThresholdLoc;

		//Material (in Fragment Shader)

		private int uMat_diffLoc;
		private int uMat_ambLoc;
		private int uMat_specLoc;

		private int uMat_diffTxtLoc;
		private int uMat_ambTxtLoc;
		private int uMat_specTxtLoc;

		private int uMat_hasDiffLoc;
		private int uMat_hasAmbLoc;
		private int uMat_hasSpecLoc;

		private int uMat_normTxtLoc;

		private int uMat_hasNormLoc;

		private PhongShader()
		{
			Loaded = false;
			postLoaded = false;
		}

		public Vector3 LightPosition { get; set; } = new Vector3(-1000, -1000, -10000);

		private Matrix4[] lightMatrices;
		private Texture2D[] shadowMaps;
		private float[] cascadeThresholds;

		private int[] shadowMapTextureIds = new int[ShadowMapScript.CASCADE_NUMBER] { 4, 5, 6 };

		public void PostLoad(Texture2D[] shadowMaps, float[] cascadeThresholds)
		{
			this.shadowMaps = shadowMaps;
			this.cascadeThresholds = cascadeThresholds;
			postLoaded = false;
			valid = true;
		}

		public bool Loaded { get; private set; }
		private bool postLoaded;
		private bool valid;

		private void LoadMaterial(Material m)
		{
			GL.Uniform4(uMat_diffLoc, m.DiffuseColor);
			if (m.DiffuseTexture == null)
			{
				GL.Uniform1(uMat_hasDiffLoc, 0);
			}
			else
			{
				Texture2D t = (Texture2D)m.DiffuseTexture;
				GL.Uniform1(uMat_hasDiffLoc, 1);
				GL.ActiveTexture(TextureUnit.Texture0);
				GL.BindTexture(TextureTarget.Texture2D, t.Id);
				GL.Uniform1(uMat_diffTxtLoc, 0);
			}

			GL.Uniform4(uMat_ambLoc, m.AmbientColor);
			if (m.AmbientTexture == null)
			{
				GL.Uniform1(uMat_hasAmbLoc, 0);
			}
			else
			{
				Texture2D t = (Texture2D)m.AmbientTexture;
				GL.Uniform1(uMat_hasAmbLoc, 1);
				GL.ActiveTexture(TextureUnit.Texture1);
				GL.BindTexture(TextureTarget.Texture2D, t.Id);
				GL.Uniform1(uMat_ambTxtLoc, 1);
			}

			GL.Uniform4(uMat_specLoc, m.SpecularColor);
			if (m.SpecularTexture == null)
			{
				GL.Uniform1(uMat_hasSpecLoc, 0);
			}
			else
			{
				Texture2D t = (Texture2D)m.SpecularTexture;
				GL.Uniform1(uMat_hasSpecLoc, 1);
				GL.ActiveTexture(TextureUnit.Texture2);
				GL.BindTexture(TextureTarget.Texture2D, t.Id);
				GL.Uniform1(uMat_specTxtLoc, 2);
			}

			GL.Uniform1(uMat_hasNormLoc, 0);
		}

		public void Prepare(Matrix4[] lightMatrices)
		{
			this.lightMatrices = lightMatrices;
		}

		private bool PrepareShadowMaps()
		{
			if (shadowMaps == null || lightMatrices == null || cascadeThresholds == null)
			{
				Log.E(TAG, "Trying to render without having called PostLoad()");
				valid = false;
				return false;
			}
			if (shadowMaps.Count() != ShadowMapScript.CASCADE_NUMBER || lightMatrices.Count() != ShadowMapScript.CASCADE_NUMBER || cascadeThresholds.Count() != ShadowMapScript.CASCADE_NUMBER)
			{
				Log.E(TAG, "The values passed to PostLoad() are incorrect");
				valid = false;
				return false;
			}
			GL.Uniform1(cascThresholdLoc, 3, cascadeThresholds);
			GL.Uniform1(uShadowMapsLoc, 3, shadowMapTextureIds);
			GL.ActiveTexture(TextureUnit.Texture4);
			GL.BindTexture(TextureTarget.Texture2D, shadowMaps[0].Id);
			GL.ActiveTexture(TextureUnit.Texture5);
			GL.BindTexture(TextureTarget.Texture2D, shadowMaps[1].Id);
			GL.ActiveTexture(TextureUnit.Texture6);
			GL.BindTexture(TextureTarget.Texture2D, shadowMaps[2].Id);
			valid = true;
			postLoaded = true;
			return true;
		}

		private bool PrepareLightMatrices()
		{
			if (lightMatrices == null)
			{
				Log.E(TAG, "Trying to render without having called Preapare()");
				return false;
			}
			if (lightMatrices.Count() != ShadowMapScript.CASCADE_NUMBER)
			{
				Log.E(TAG, "the values passed to Preapre() are incorrect");
				return false;
			}
			GL.UniformMatrix4(uLightMatrix0Loc, false, ref lightMatrices[0]);
			GL.UniformMatrix4(uLightMatrix1Loc, false, ref lightMatrices[1]);
			GL.UniformMatrix4(uLightMatrix2Loc, false, ref lightMatrices[2]);
			return true;
		}

		public void Draw(List<IDrawable> drawables, ICamera camera)
		{
			if (!valid)
			{
				Log.V(TAG, "Shader is in invalid state.");
				return;
			}
			GL.UseProgram(Id);
			if (!postLoaded)
			{
				if (!PrepareShadowMaps())
				{
					Log.E(TAG, "Cannot load shadowMap data. Setting shader to invalid state");
#if DEBUG
					Log.W(TAG, "Run PostLoad() with valid values to reset state.");
#endif
					return;
				}
			}

			if (!PrepareLightMatrices())
			{
				Log.E(TAG, "Wrong Light Matrix Array passed to shader. Skipping render");
				return;
			}
			Matrix4 proj = camera.Projection;
			Matrix4 view = camera.View;
			Matrix4 cameraModel = MathUtils.GenCameraModel(camera);
			GL.UniformMatrix4(uProjLoc, false, ref proj);
			GL.UniformMatrix4(uViewLoc, false, ref view);
			GL.UniformMatrix4(uCameraLoc, false, ref cameraModel);
			GL.Uniform3(uLightPosLoc, Vector3.TransformPosition(-LightPosition, cameraModel));

			GL.EnableVertexAttribArray(aPosLoc);
			GL.EnableVertexAttribArray(aNormLoc);

			foreach (IDrawable d in drawables)
			{
				Matrix4 model = MathUtils.GenDrawableModelMatrix(d);
				Matrix4 normal = Matrix4.Identity;
				normal *= model;
				normal *= cameraModel;
				normal.Invert();
				normal.Transpose();
				GL.UniformMatrix4(uNormalLoc, false, ref normal);
				GL.UniformMatrix4(uModelLoc, false, ref model);
				if (d.Mat == null)
				{
					Log.W(TAG, "Drawable missing material. Rendering with default");
					LoadMaterial(new Material()); //TODO: Optimize default material
				}
				else
					LoadMaterial(d.Mat);

				if (d.Vertices == null)
				{
					Log.E(TAG, "Cannot render drawable. Missing Vertices");
					continue;
				}
				((GLBuffer)d.Vertices).Bind(aPosLoc);
				if (d.Normals == null)
				{
					Log.E(TAG, "Cannot render drawable. Missing Normals");
					continue;
				}
				((GLBuffer)d.Normals).Bind(aNormLoc);
				if (d.TexCoords != null)
				{
					GL.EnableVertexAttribArray(aTexCoordLoc);
					((GLBuffer)d.TexCoords).Bind(aTexCoordLoc);
				}
				d.Draw();
				if (d.TexCoords != null)
					GL.DisableVertexAttribArray(aTexCoordLoc);
			}

			GL.DisableVertexAttribArray(aNormLoc);
			GL.DisableVertexAttribArray(aPosLoc);
		}

		public void Load()
		{
			if (Loaded)
				return;
			Log.Assert(ShadowMapScript.CASCADE_NUMBER == 3, "GL3/Scripts/ShadowMapScript.CASCADE_NUMBER == 3 failed. The built-in shaders support only 3-level shadow map cascades");
			Id = ShaderUtils.CompileShader(@"DefaultAssets\Shaders\phongv.glsl", @"DefaultAssets\Shaders\phongf.glsl");
			//Vertex Shader

			aPosLoc = GL.GetAttribLocation(Id, "aPos");
			Utils.LogError(TAG + " Load 0");
			aTexCoordLoc = GL.GetAttribLocation(Id, "aTexCoord");
			Utils.LogError(TAG + " Load 1");
			aNormLoc = GL.GetAttribLocation(Id, "aNorm");
			Utils.LogError(TAG + " Load 2");

			uProjLoc = GL.GetUniformLocation(Id, "uProj");
			uViewLoc = GL.GetUniformLocation(Id, "uView");
			uCameraLoc = GL.GetUniformLocation(Id, "uCamera");
			uModelLoc = GL.GetUniformLocation(Id, "uModel");
			uNormalLoc = GL.GetUniformLocation(Id, "uNormal");

			uLightMatrix0Loc = GL.GetUniformLocation(Id, "uLightMatrix0");
			uLightMatrix1Loc = GL.GetUniformLocation(Id, "uLightMatrix1");
			uLightMatrix2Loc = GL.GetUniformLocation(Id, "uLightMatrix2");

			//Fragment Shader

			uLightPosLoc = GL.GetUniformLocation(Id, "uLightPos");
			uShadowMapsLoc = GL.GetUniformLocation(Id, "uShadowMaps");
			cascThresholdLoc = GL.GetUniformLocation(Id, "cascThreshold");

			//Material

			uMat_diffLoc = GL.GetUniformLocation(Id, "uMat.diff");
			uMat_ambLoc = GL.GetUniformLocation(Id, "uMat.amb");
			uMat_specLoc = GL.GetUniformLocation(Id, "uMat.spec");

			uMat_diffTxtLoc = GL.GetUniformLocation(Id, "uMat.diffTxt");
			uMat_ambTxtLoc = GL.GetUniformLocation(Id, "uMat.ambTxt");
			uMat_specTxtLoc = GL.GetUniformLocation(Id, "uMat.specTxt");

			uMat_hasDiffLoc = GL.GetUniformLocation(Id, "uMat.hasDiff");
			uMat_hasAmbLoc = GL.GetUniformLocation(Id, "uMat.hasAmb");
			uMat_hasSpecLoc = GL.GetUniformLocation(Id, "uMat.hasSpec");

			uMat_normTxtLoc = GL.GetUniformLocation(Id, "uMat.normTxt");
			uMat_hasNormLoc = GL.GetUniformLocation(Id, "uMat.hasNorm");

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