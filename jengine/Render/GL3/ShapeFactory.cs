using jengine.Render.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jengine.Render.Wrappers;
using OpenTK;
using jengine.Render.GL3.Shapes;
using jengine.Render.Shapes.VR;
using Valve.VR;
using libjmodel;
using libjmodel.Structures;
using libjmodel.Structures.Primitives;
using jengine.Render.GL3.Wrappers;

namespace jengine.Render.GL3
{
	public class ShapeFactory : IShapeFactory
	{
		private class MTLLoader
		{
		}

		private class OBJLoader
		{
			//TODO
			private string Path { get; set; }

			public OBJLoader(string path)
			{
				Path = path;
			}
		}

		public const string TAG = "GL3/ShapeFactory";

		private static ShapeFactory _Instance = null;

		public static ShapeFactory Instance
		{
			get
			{
				if (_Instance == null)
					_Instance = new ShapeFactory();
				return _Instance;
			}
		}

		private ShapeFactory()
		{
		}

		public IDrawable MakeCube(Vector3 size, Vector4 color)
		{
			return new Cube(size, color);
		}

		public IDrawable MakePhongCube(Vector3 size, ITexture2D diffuse, ITexture2D ambient, ITexture2D specular)
		{
			return null;
		}

		public IDrawable MakePhongCube(Vector3 size, Vector4 diffuse, Vector4 ambient, Vector4 specular)
		{
			return null;
		}

		public IVRTrackedDrawable MakeRenderModel(uint id)
		{
			Log.Assert(Game.Instance.LoadedSettings.OpenVREnabled, TAG + " Cannot Load RenderModel with OpenVR disabled.");
			StringBuilder builder = new StringBuilder(256);
			ETrackedPropertyError err = ETrackedPropertyError.TrackedProp_Success;
			Game.Instance.Vr.GetStringTrackedDeviceProperty(id, ETrackedDeviceProperty.Prop_RenderModelName_String, builder, 256, ref err);
			IntPtr model = new IntPtr();
			Log.V(TAG, "New Deivce RenderModelName: " + builder.ToString());
			if (builder.ToString().Trim().Length == 0)
				return null;
			EVRRenderModelError er = EVRRenderModelError.None;
			do
			{
				er = OpenVR.RenderModels.LoadRenderModel_Async(builder.ToString(), ref model);
			} while (er == EVRRenderModelError.Loading);
			Log.Assert(er == EVRRenderModelError.None, TAG + " Render Model Load Error: " + er.ToString());
			return new RenderModel(model);
		}

		public DrawableGroup LoadJ3DModel(string modelPath)
		{
			List<Mesh> parts = new List<Mesh>();
			//ModelSet.DoLog = true;
			int x = ModelSet.LoadFromFile(modelPath, out ModelSet m);
			if (x != 0)
			{
				Log.E(TAG, "Failed to Load J3D Model. Internal Library Error: " + x);
				return null;
			}
			else
			{
				foreach (Model mod in m.Parts)
				{
					Log.V(TAG, "Found part in J3D");
					float[] vertices = new float[mod.Vertices.Length * 3];
					float[] texCoords = new float[mod.Vertices.Length * 2];
					float[] normals = new float[mod.Vertices.Length * 3];
					Log.V(TAG, "Vertices Length: " + mod.Vertices.Length);
					for (int i = 0; i < mod.Vertices.Length; i++)
					{
						Vertex v = mod.Vertices[i];
						vertices[(i * 3)] = v.Position.X;
						vertices[(i * 3) + 1] = v.Position.Y;
						vertices[(i * 3) + 2] = v.Position.Z;
						texCoords[(i * 2)] = v.TexCoords.X;
						texCoords[(i * 2) + 1] = v.TexCoords.Y;
						normals[(i * 3)] = v.Normal.X;
						normals[(i * 3) + 1] = v.Normal.Y;
						normals[(i * 3) + 2] = v.Normal.Z;
					}
					Material mat = new Material()
					{
						AmbientColor = new Vector4(mod.Mat.AmbientColor.X, mod.Mat.AmbientColor.Y, mod.Mat.AmbientColor.Z, mod.Mat.AmbientColor.W),
						DiffuseColor = new Vector4(mod.Mat.DiffuseColor.X, mod.Mat.DiffuseColor.Y, mod.Mat.DiffuseColor.Z, mod.Mat.DiffuseColor.W),
						SpecularColor = new Vector4(mod.Mat.DiffuseColor.X, mod.Mat.DiffuseColor.Y, mod.Mat.DiffuseColor.Z, mod.Mat.DiffuseColor.W)
					};
					if (!mod.Mat.DiffuseMapPath.Equals(string.Empty))
						mat.DiffuseTexture = new Texture2D(mod.Mat.DiffuseMapPath);
					if (!mod.Mat.AmbientMapPath.Equals(string.Empty))
						mat.AmbientTexture = new Texture2D(mod.Mat.AmbientMapPath);
					if (!mod.Mat.SpecularMapPath.Equals(string.Empty))
						mat.SpecularTexture = new Texture2D(mod.Mat.SpecularMapPath);
					Mesh mesh = new Mesh(mod.Vertices.Length / 3, vertices, normals, texCoords, mat)
					{
						Position = new Vector3(mod.OffsetPosition.X, mod.OffsetPosition.Y, mod.OffsetPosition.Z),
						Orientation = new Quaternion(mod.OffsetOrientationQuaternion.X, mod.OffsetOrientationQuaternion.Y, mod.OffsetOrientationQuaternion.Z, mod.OffsetOrientationQuaternion.Z),
						Scale = new Vector3(mod.OffsetScale.X, mod.OffsetScale.Y, mod.OffsetScale.Z)
					};
					parts.Add(mesh);
				}
				return new DrawableGroup(parts.ToArray());
			}
		}
	}
}