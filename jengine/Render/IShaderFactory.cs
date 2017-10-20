using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jengine.Render
{
	public interface IShaderFactory
	{
		IShader Simple3D { get; }
		IShader Phong { get; }
		IShader TexturedPhong { get; }
		IShader Textured2D { get; }
		IShader Simple2D { get; }
		IShader Simple2DTextured { get; }
		IShader ShadowMap { get; }
	}
}