using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jengine.Cameras;

namespace jengine.Render.GL3.Shaders
{
	public static class MathUtils
	{
		public static Matrix4 GenCameraModel(ICamera camera)
		{
			if (camera.Loaded)
				camera.Load();
			Matrix4 model = Matrix4.CreateTranslation(-camera.Position);
			model *= Matrix4.CreateFromQuaternion(camera.Yaw);
			model *= Matrix4.CreateFromQuaternion(camera.Pitch);
			model *= Matrix4.CreateFromQuaternion(camera.Roll);
			return model;
		}

		public static Matrix4 GenDrawableModelMatrix(IDrawable d)
		{
			Vector3 effectivePosition;
			Quaternion effectiveRotation;
			Vector3 effectiveScale;
			if (d.Parent == null)
			{
				effectivePosition = d.Position;
				effectiveRotation = d.Orientation;
				effectiveScale = d.Scale;
			}
			else
			{
				effectivePosition = d.Parent.Position + (d.Parent.Orientation * (d.Position * d.Parent.Scale));
				effectiveRotation = d.Parent.Orientation * d.Orientation;
				effectiveScale = d.Parent.Scale * d.Scale;
			}
			Matrix4 model = Matrix4.CreateScale(effectiveScale);
			model *= Matrix4.CreateFromQuaternion(effectiveRotation);
			model *= Matrix4.CreateTranslation(effectivePosition);
			return model;
		}
	}
}