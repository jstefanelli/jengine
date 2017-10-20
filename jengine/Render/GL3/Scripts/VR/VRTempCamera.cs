using jengine.Cameras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace jengine.Render.GL3.Scripts.VR
{
	public class VRTempCamera : ICamera
	{
		public const string TAG = "GL3/Scripts/VR/VRTempCamera";

		private ICamera internalCamera;

		public Matrix4 Projection { get; set; }

		public Matrix4 View { get; set; }

		public Vector3 Position { get; set; }

		public Quaternion Yaw { get; set; }
		public Quaternion Pitch { get; set; }
		public Quaternion Roll { get; set; }

		public bool Loaded { get => internalCamera.Loaded; }

		public void Draw()
		{
			internalCamera.Draw();
		}

		public void Load()
		{
			internalCamera.Load();
		}

		public VRTempCamera(ICamera camera)
		{
			internalCamera = camera;
			Position = internalCamera.Position;
			Yaw = internalCamera.Yaw;
			Pitch = internalCamera.Pitch;
			Roll = internalCamera.Roll;
		}

		public void Resize(int width, int height)
		{
			//Unused
#if DEBUG
			Log.W(TAG, "Not supported in VRTempCamera");
#endif
		}

		public void Unload()
		{
			internalCamera.Unload();
		}

		public void Update()
		{
			internalCamera.Update();
		}
	}
}