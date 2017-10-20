using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jengine.Render.Shapes.VR
{
	public interface IVRTrackedDrawable : IDrawable
	{
		Vector3 TrackedPosition { get; set; }
		Quaternion TrackedOrientation { get; set; }
		Vector3 TrackedVelocity { get; set; }
		Vector3 TrackedAngularVelocity { get; set; }
		bool Active { get; set; }
	}
}