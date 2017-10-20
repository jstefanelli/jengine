using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jengine.Render.Wrappers;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using jengine.Render.GL3.Wrappers;
using Valve.VR;
using System.Runtime.InteropServices;
using System.Drawing;
using jengine.Render.Shapes.VR;

namespace jengine.Render.GL3.Shapes
{
	public class RenderModel : IVRTrackedDrawable
	{
		public const string TAG = "GL3_VR/OpenVR/RenderModel";

		public DrawableGroup Parent { get; set; }

		private GLBuffer vertices;
		private GLBuffer normals;
		private GLBuffer texCoords;
		private short[] indexBuffer;

		private float[] vrts;
		private float[] nrms;
		private float[] txtc;

		public IBuffer Vertices => vertices;

		public IBuffer Normals => normals;

		public IBuffer TexCoords => texCoords;

		public Material Mat { get; set; }

		private Vector3 WorldPosition = Vector3.Zero;
		private Quaternion WorldOrientation = Quaternion.Identity;

		public Vector3 Position
		{
			get
			{
				return WorldPosition + TrackedPosition;
			}
			set
			{
				WorldPosition = value;
			}
		}

		public Quaternion Orientation
		{
			get
			{
				return WorldOrientation + TrackedOrientation;
			}
			set
			{
				WorldOrientation = value;
			}
		}

		public Vector3 Scale { get; set; } = new Vector3(1.0f);

		public bool Loaded { get; private set; } = false;
		public Vector3 TrackedPosition { get; set; } = Vector3.Zero;
		public Quaternion TrackedOrientation { get; set; } = Quaternion.Identity;
		public Vector3 TrackedVelocity { get; set; } = Vector3.Zero;
		public Vector3 TrackedAngularVelocity { get; set; } = Vector3.Zero;

		public bool Active { get; set; } = false;

		private RenderModel_t Model;
		private IntPtr ModelPtr;

		public RenderModel(IntPtr modelPtr)
		{
			ModelPtr = modelPtr;
			Model = MarshalRenderModel(modelPtr);
			Prepare();
		}

		private void Prepare()
		{
			RenderModel_Vertex_t[] verts = new RenderModel_Vertex_t[Model.unVertexCount];
			for (int i = 0; i < Model.unVertexCount; i++)
			{
				IntPtr t = new IntPtr(Model.rVertexData.ToInt64() + i * Marshal.SizeOf(typeof(RenderModel_Vertex_t)));
				verts[i] = (RenderModel_Vertex_t)Marshal.PtrToStructure(t, typeof(RenderModel_Vertex_t));
			}

			vrts = new float[verts.Length * 3];
			for (int i = 0; i < verts.Length; i++)
			{
				vrts[i * 3] = verts[i].vPosition.v0;
				vrts[(i * 3) + 1] = verts[i].vPosition.v1;
				vrts[(i * 3) + 2] = verts[i].vPosition.v2;
			}

			nrms = new float[verts.Length * 3];
			for (int i = 0; i < verts.Length; i++)
			{
				nrms[i * 3] = verts[i].vNormal.v0;
				nrms[(i * 3) + 1] = verts[i].vNormal.v1;
				nrms[(i * 3) + 2] = verts[i].vNormal.v2;
			}

			txtc = new float[verts.Length * 2];
			for (int i = 0; i < verts.Length; i++)
			{
				txtc[i * 2] = verts[i].rfTextureCoord0;
				txtc[(i * 2) + 1] = verts[i].rfTextureCoord1;
			}

			int triangleCount = (int)Model.unTriangleCount * 3;
			indexBuffer = new short[triangleCount];
			Marshal.Copy(Model.rIndexData, indexBuffer, 0, indexBuffer.Length);

			IntPtr txt = new IntPtr();
			EVRRenderModelError err = EVRRenderModelError.None;
			do
			{
				err = OpenVR.RenderModels.LoadTexture_Async(Model.diffuseTextureId, ref txt);
			} while (err == EVRRenderModelError.Loading);
			Log.Assert(err == EVRRenderModelError.None, TAG + " Cannot Load Texture: " + err.ToString());
			RenderModel_TextureMap_t texture = MarshalTextureMap(txt);

			Log.V(TAG, "Data: " + (int)texture.unWidth + "x" + (int)texture.unHeight);
			Bitmap b = new Bitmap(texture.unWidth, texture.unHeight, 4 * texture.unWidth, System.Drawing.Imaging.PixelFormat.Format32bppRgb, texture.rubTextureMapData);
			Texture2D myTexture = new Texture2D(b);
			Mat = new Material
			{
				AmbientColor = new Vector4(0.1f, 0.1f, 0.1f, 1.0f),
				DiffuseColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
				DiffuseTexture = myTexture,
				SpecularColor = new Vector4(0.2f, 0.2f, 0.2f, 1.0f)
			};
			OpenVR.RenderModels.FreeRenderModel(ModelPtr);
		}

		public void Draw()
		{
			if (Active && Loaded)
				GL.DrawElements(PrimitiveType.Triangles, indexBuffer.Length, DrawElementsType.UnsignedShort, indexBuffer);
		}

		public void Load()
		{
			vertices = new GLBuffer();
			vertices.Load(vrts, 3);
			vrts = null;

			normals = new GLBuffer();
			normals.Load(nrms, 3);
			nrms = null;

			texCoords = new GLBuffer();
			texCoords.Load(txtc, 2);
			txtc = null;

			Mat.DiffuseTexture?.Load();

			Loaded = true;
		}

		public void Unload()
		{
			if (!Loaded)
				return;
			vertices.Unload();
			normals.Unload();
			texCoords.Unload();
			Mat.DiffuseTexture?.Unload();
		}

		private RenderModel_t MarshalRenderModel(IntPtr pRenderModel)
		{
			if ((Environment.OSVersion.Platform == PlatformID.MacOSX) ||
				(Environment.OSVersion.Platform == PlatformID.Unix))
			{
				var packedModel = (RenderModel_t_Packed)Marshal.PtrToStructure(pRenderModel, typeof(RenderModel_t_Packed));
				RenderModel_t model = new RenderModel_t();
				packedModel.Unpack(ref model);
				return model;
			}
			else
			{
				return (RenderModel_t)Marshal.PtrToStructure(pRenderModel, typeof(RenderModel_t));
			}
		}

		private RenderModel_TextureMap_t MarshalTextureMap(IntPtr pRenderModel)
		{
			if ((Environment.OSVersion.Platform == PlatformID.MacOSX) ||
				(Environment.OSVersion.Platform == PlatformID.Unix))
			{
				var packedModel = (RenderModel_TextureMap_t_Packed)Marshal.PtrToStructure(pRenderModel, typeof(RenderModel_TextureMap_t_Packed));
				RenderModel_TextureMap_t model = new RenderModel_TextureMap_t();
				packedModel.Unpack(ref model);
				return model;
			}
			else
			{
				return (RenderModel_TextureMap_t)Marshal.PtrToStructure(pRenderModel, typeof(RenderModel_TextureMap_t));
			}
		}
	}
}