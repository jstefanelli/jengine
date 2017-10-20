using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace jengine.Cameras
{
	public class FixedCamera : ICamera
	{
		public Matrix4 Projection { get; private set; }
		public Matrix4 View { get; private set; }

		public Vector3 Position { get; set; }

		public Quaternion Yaw { get; set; }
		public Quaternion Pitch { get; set; }
		public Quaternion Roll { get; set; }

		public bool Loaded { get; private set; }

		public FixedCamera()
		{
			Position = new Vector3(0, 0, 1);
			Yaw = Quaternion.Identity;
			Pitch = Quaternion.Identity;
			Roll = Quaternion.Identity;
			Loaded = false;
		}

		public void Draw()
		{
		}

		public void Load()
		{
			View = Matrix4.LookAt(new Vector3(0, 0, 0), new Vector3(0, 0, -1), new Vector3(0, 1, 0));
			Resize(Game.Instance.Width, Game.Instance.Height);
			Loaded = true;
		}

		public void Resize(int width, int height)
		{
			Projection = Matrix4.CreatePerspectiveFieldOfView((Game.Instance.LoadedSettings.FOVDegrees * (float)Math.PI) / 180.0f, ((float)width) / height, Game.Instance.LoadedSettings.NearPlane, Game.Instance.LoadedSettings.FarPlane);
		}

		public void Unload()
		{
			Loaded = false;
		}

		public void Update()
		{
		}
	}
}