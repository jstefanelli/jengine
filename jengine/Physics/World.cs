using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;
using OpenTK;

namespace jengine.Physics
{
	public class World
	{
		private BroadphaseInterface broadphase;
		private DefaultCollisionConfiguration collisionConfiguration;
		private CollisionDispatcher dispatcher;
		private SequentialImpulseConstraintSolver solver;
		public DiscreteDynamicsWorld PhysicsWorld { get; private set; }

		public World()
		{
			broadphase = new DbvtBroadphase();
			collisionConfiguration = new DefaultCollisionConfiguration();
			dispatcher = new CollisionDispatcher(collisionConfiguration);
			GImpactCollisionAlgorithm.RegisterAlgorithm(dispatcher);
			solver = new SequentialImpulseConstraintSolver();
			PhysicsWorld = new DiscreteDynamicsWorld(dispatcher, broadphase, solver, collisionConfiguration)
			{
				Gravity = new Vector3(0, -9.81f, 0)
			};
		}

		public void Step(double deltaSec)
		{
			PhysicsWorld.StepSimulation((float)deltaSec, 10);
		}
	}
}