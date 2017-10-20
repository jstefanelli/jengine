using jengine.Render.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valve.VR;
using jengine.Cameras;
using jengine.Render.GL3.Wrappers;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using jengine.Render.GL3.Scripts.VR;
using System.Runtime.InteropServices;
using jengine.Control.VR;
using jengine.Render.Shapes.VR;

namespace jengine.Render.GL3.Scripts
{
	public class VRShadowMapRenderScript : IRenderScript
	{
		private class EyeProvider : ShadowMapScript.IProjectionMatrixProvider
		{
			public EVREye Eye { get; set; }

			public EyeProvider(EVREye eye)
			{
				Eye = eye;
			}

			public Matrix4 GetProjectionMatrixNearFar(float zNear, float zFar)
			{
				return Game.Instance.VrSettings.GetProjectionMatrixPerEye(Eye, zNear, zFar);
			}
		}

		public const string TAG = "GL3/Scripts/VRShadowMap";
		private ShadowMapScript renderScript;

		private GenericFrameBuffer outputFrameBuffer;
		private VRTextureBounds_t outTextureBounds;
		private Texture_t outTexture;

		private EyeProvider LeftProvider;
		private EyeProvider RightProvider;

		public void Draw(List<IDrawable> drawableas, IRender render, ICamera camera)
		{
			//Log.Assert(Game.Instance.LoadedSettings.OpenVREnabled, TAG + ": Cannot draw with OpenVR support disabled");
			foreach (IVRTrackedDrawable m in Game.Instance.VrRenderModels.Values)
			{
				if (!m.Loaded)
					m.Load();
			}
			VRTempCamera[] cameras = new VRTempCamera[2]
			{
				new VRTempCamera(camera), new VRTempCamera(camera)
			};

			List<IDrawable> newDrawables = new List<IDrawable>();

			cameras[0].Position = camera.Position + Game.Instance.VrSettings.LocalHeadPosition;
			cameras[0].Yaw = Game.Instance.VrSettings.LocalHeadOrientation.Inverted();
			cameras[0].Pitch = Quaternion.Identity;
			cameras[0].Roll = Quaternion.Identity;
			cameras[1].Position = camera.Position + Game.Instance.VrSettings.LocalHeadPosition;
			cameras[1].Yaw = Game.Instance.VrSettings.LocalHeadOrientation.Inverted();
			cameras[1].Pitch = Quaternion.Identity;
			cameras[1].Roll = Quaternion.Identity;

			foreach (IDrawable i in drawableas)
			{
				newDrawables.Add(i);
			}
			foreach (IVRTrackedDrawable m in Game.Instance.VrRenderModels.Values)
			{
				if (m.Active)
				{
					m.Position = camera.Position;
					newDrawables.Add(m);
				}
			}
			cameras[0].View = Game.Instance.VrSettings.ViewMatrices[0];
			cameras[1].View = Game.Instance.VrSettings.ViewMatrices[1];
			cameras[0].Projection = Game.Instance.VrSettings.GetProjectionMatrixPerEye(EVREye.Eye_Left, Game.Instance.LoadedSettings.NearPlane, Game.Instance.LoadedSettings.FarPlane);
			cameras[1].Projection = Game.Instance.VrSettings.GetProjectionMatrixPerEye(EVREye.Eye_Right, Game.Instance.LoadedSettings.NearPlane, Game.Instance.LoadedSettings.FarPlane);

			renderScript.MatrixProvider = LeftProvider;
			renderScript.DrawToWindow = true;
			renderScript.Draw(newDrawables, render, cameras[0]);

			GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, renderScript.FrameBuffer.Id);
			GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, outputFrameBuffer.Id);
			GL.BlitFramebuffer(0, 0, renderScript.FrameBuffer.Width, renderScript.FrameBuffer.Height, 0, 0, outputFrameBuffer.Width, outputFrameBuffer.Height, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Linear);
			GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, 0);
			GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);

			EVRCompositorError e = OpenVR.Compositor.Submit(EVREye.Eye_Left, ref outTexture, ref outTextureBounds, EVRSubmitFlags.Submit_Default);
			if (e != EVRCompositorError.None)
				Log.V(TAG, " Left eye error: " + e.ToString());

			renderScript.MatrixProvider = RightProvider;
			renderScript.DrawToWindow = false;
			renderScript.Draw(newDrawables, render, cameras[1]);

			GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, renderScript.FrameBuffer.Id);
			GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, outputFrameBuffer.Id);
			GL.BlitFramebuffer(0, 0, renderScript.FrameBuffer.Width, renderScript.FrameBuffer.Height, 0, 0, outputFrameBuffer.Width, outputFrameBuffer.Height, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Linear);
			GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, 0);
			GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);

			e = OpenVR.Compositor.Submit(EVREye.Eye_Right, ref outTexture, ref outTextureBounds, EVRSubmitFlags.Submit_Default);
			if (e != EVRCompositorError.None)
				Log.V(TAG, " Right eye error: " + e.ToString());
		}

		public void Load(IRender render)
		{
			Log.Assert(Game.Instance.LoadedSettings.OpenVREnabled, TAG + ": Cannot load with OpenVR support disabled");

			renderScript = (ShadowMapScript)render.RenderScriptFactory.MakeShadowMapScript();

			renderScript.FrameBufferSizeX = Game.Instance.VrSettings.FrameSizeX;
			renderScript.FrameBufferSizeY = Game.Instance.VrSettings.FrameSizeY;
			renderScript.DrawToWindow = true;
			renderScript.Load(render);

			LeftProvider = new EyeProvider(EVREye.Eye_Left);
			RightProvider = new EyeProvider(EVREye.Eye_Right);

			outputFrameBuffer = new GenericFrameBuffer(Game.Instance.VrSettings.FrameSizeX, Game.Instance.VrSettings.FrameSizeY)
			{
				GenDepthBuffer = false
			};
			outputFrameBuffer.Load(0);
			outTexture = new Texture_t
			{
				eType = ETextureType.OpenGL,
				eColorSpace = EColorSpace.Gamma,
				handle = new IntPtr(outputFrameBuffer.TextureId)
			};

			outTextureBounds = new VRTextureBounds_t()
			{
				uMin = 0,
				uMax = 1,
				vMax = 1,
				vMin = 0
			};
		}

		public void Unload()
		{
			renderScript?.Unload();
			outputFrameBuffer?.Unload();
		}

		public void Update()
		{
		}
	}
}