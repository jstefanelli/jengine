using jengine.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jengine.Render;
using OpenTK;
using jengine.Cameras;
using jengine;
using OpenTK.Input;
using BulletSharp;
using jengine.Render.GL3.Scripts;
using jengine.Physics.Interop;
using jengine.Physics;
using jengine.Control.Keyboard;
using jengine.Render.Scripts;

namespace TestApp.Scenes
{
	public class BasicScene : IScene, IKeyboardControlReceiver
	{
		public const string TAG = "App/Scenes/BasicScene";
		private ICamera camera;
		private GameObject floor;
		private GameObject cube;
		private List<IDrawable> drawables;
		private IRenderScript script;
		private World world;
		private DrawableGroup exampleCube;
		private float speed = 0.005f;

		public void Draw(double delta, IRender render)
		{
			script.Draw(drawables, render, camera);
		}

		public void Load(IRender render)
		{
			world = new World();
			floor = Interop.MakeCube(render, new Vector3(10, 1, 10), Quaternion.Identity, new Vector3(0, -1, 0), new Vector4(0, 1, 0, 1), 0);
			floor.RenderObject.Load();
			floor.PhysicsObject.CollisionFlags = CollisionFlags.KinematicObject;
			world.PhysicsWorld.AddRigidBody(floor.PhysicsObject);

			cube = Interop.MakeCube(render, new Vector3(1, 1, 1), Quaternion.Identity, new Vector3(0, 10, 0), new Vector4(1, 0, 0, 1), 100);
			cube.RenderObject.Load();
			cube.RenderObject.Mat.SpecularColor = cube.RenderObject.Mat.DiffuseColor * 0.2f;
			world.PhysicsWorld.AddRigidBody(cube.PhysicsObject);

			camera = new FixedCamera
			{
				Position = new Vector3(0, 3, 5),
				Pitch = Quaternion.FromEulerAngles(new Vector3(0, 0, (0.0f * (float)Math.PI) / 180.0f))
			};
			camera.Load();
			if (!Game.Instance.LoadedSettings.OpenVREnabled)
				script = render.RenderScriptFactory.MakeShadowMapScript();
			else
				script = render.RenderScriptFactory.MakeShadowMapScript_VR();
			script.Load(render);
			exampleCube = render.ShapeFactory.LoadJ3DModel("test.j3d");
			Log.Assert(exampleCube != null, "Failed to Load Example cube");
			drawables = new List<IDrawable>
			{
				floor.RenderObject,
				cube.RenderObject,
			};
			foreach (IDrawable d in exampleCube.Drawables)
			{
				d.Load();
				d.Orientation = Quaternion.Identity;
				drawables.Add(d);
			}
			exampleCube.Position = new Vector3(1f, 0f, -1);
			exampleCube.Orientation = Quaternion.Identity;
			exampleCube.Scale = new Vector3(0.5f, 0.5f, 0.5f);
			Game.Instance.CurrentKeyboardReceiver = this;
		}

		public void Unload()
		{
			floor.RenderObject.Unload();
			cube.RenderObject.Unload();
			script.Unload();
		}

		public void Update(double delta)
		{
			world.Step(delta);
			script.Update();
			floor.Update();
			cube.Update();
			if (Keyboard.GetState().IsKeyDown(Key.W))
			{
				floor.Position = new Vector3(floor.Position.X, floor.Position.Y + speed, floor.Position.Z);
			}
			if (Keyboard.GetState().IsKeyDown(Key.S))
			{
				floor.Position = new Vector3(floor.Position.X, floor.Position.Y - speed, floor.Position.Z);
			}
			if (Keyboard.GetState().IsKeyDown(Key.Escape))
			{
				Game.Instance.Window.Close();
			}
		}

		public void KeyDown(KeyboardKeyEventArgs args)
		{
		}

		public void KeyPress(KeyPressEventArgs args)
		{
		}

		public void KeyUp(KeyboardKeyEventArgs args)
		{
		}
	}
}