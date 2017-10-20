using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jengine.Cameras
{
	public interface ICamera
	{
		Matrix4 Projection { get; }
		Matrix4 View { get; }

		Vector3 Position { get; set; }
		Quaternion Yaw { get; set; }
		Quaternion Pitch { get; set; }
		Quaternion Roll { get; set; }

		bool Loaded { get; }

		void Update();

		void Draw();

		void Load();

		void Unload();

		void Resize(int width, int height);
	}
}