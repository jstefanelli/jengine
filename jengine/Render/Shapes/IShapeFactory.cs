using jengine.Render.Shapes.VR;
using jengine.Render.Wrappers;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jengine.Render.Shapes
{
	public interface IShapeFactory
	{
		IDrawable MakeCube(Vector3 size, Vector4 color);

		IDrawable MakePhongCube(Vector3 size, ITexture2D diffuse, ITexture2D ambient, ITexture2D specular);

		IDrawable MakePhongCube(Vector3 size, Vector4 diffuse, Vector4 ambient, Vector4 specular);

		IVRTrackedDrawable MakeRenderModel(uint id);

		DrawableGroup LoadJ3DModel(string path);
	}
}