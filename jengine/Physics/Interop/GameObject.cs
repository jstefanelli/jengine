using BulletSharp;
using jengine.Render;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jengine.Physics.Interop
{
	public class GameObject
	{
		public const string TAG = "Physics/Interop/GameObject";

		public IDrawable RenderObject { get; private set; }
		public RigidBody PhysicsObject { get; private set; }

		public Vector3 Position
		{
			get
			{
				return PhysicsObject.WorldTransform.ExtractTranslation();
			}
			set
			{
				Matrix4 m;
				if (PhysicsObject.CollisionFlags == CollisionFlags.KinematicObject)
					m = PhysicsObject.WorldTransform;
				else
					m = PhysicsObject.MotionState.WorldTransform;
				m = m.ClearTranslation();

				Matrix4 m2 = Matrix4.CreateTranslation(value);

				m *= m2;

				PhysicsObject.WorldTransform = m;
				if (PhysicsObject.CollisionFlags == CollisionFlags.KinematicObject)
					PhysicsObject.MotionState.WorldTransform = m;
				RenderObject.Position = value;
			}
		}

		public Quaternion Orientation
		{
			get
			{
				return PhysicsObject.MotionState.WorldTransform.ExtractRotation();
			}
			set
			{
				Matrix4 m = PhysicsObject.MotionState.WorldTransform;
				m = m.ClearRotation();
				Matrix4 m2 = Matrix4.CreateFromQuaternion(value);
				m *= m2;
				PhysicsObject.MotionState.WorldTransform = m;
				RenderObject.Orientation = value;
			}
		}

		public Vector3 Scale
		{
			get
			{
				return RenderObject.Scale;
			}
			set
			{
				RenderObject.Scale = value;
				Log.W(TAG, "Changing the scale of a GameObject does not change the scale for the physics subsystem.");
			}
		}

		public GameObject(RigidBody physicsObject, IDrawable renderObject)
		{
			RenderObject = renderObject;
			PhysicsObject = physicsObject;
			Update();
		}

		public void Update()
		{
			RenderObject.Position = Position;
			RenderObject.Orientation = Orientation;
		}
	}
}