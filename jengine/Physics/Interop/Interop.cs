using jengine.Render;
using OpenTK;
using BulletSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jengine.Physics.Interop
{
	public static class Interop
	{
		public static GameObject MakeCube(IRender render, Vector3 size, Quaternion orientation, Vector3 position, Vector4 color, float mass)
		{
			IDrawable d = render.ShapeFactory.MakeCube(size, color);
			BoxShape s = new BoxShape(size / 2);
			Matrix4 m = Matrix4.CreateScale(new Vector3(1));
			m *= Matrix4.CreateFromQuaternion(orientation);
			m *= Matrix4.CreateTranslation(position);
			DefaultMotionState motionState = new DefaultMotionState(m);
			s.CalculateLocalInertia(mass, out Vector3 inertia);
			RigidBodyConstructionInfo i = new RigidBodyConstructionInfo(mass, motionState, s, inertia);
			RigidBody b = new RigidBody(i)
			{
				ActivationState = ActivationState.DisableDeactivation
			};
			return new GameObject(b, d);
		}
	}
}