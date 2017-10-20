using jengine.Render.GL3.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jengine.Render.GL3
{
	public class ShaderFactory : IShaderFactory
	{
		private static ShaderFactory _Instance = null;

		public static ShaderFactory Instance
		{
			get
			{
				if (_Instance == null)
					_Instance = new ShaderFactory();
				return _Instance;
			}
		}

		private ShaderFactory()
		{
		}

		public IShader Simple3D => Simple3DShader.Instance;

		public IShader Phong => PhongShader.Instance;

		public IShader TexturedPhong => null;

		public IShader Textured2D => null;

		public IShader Simple2D => null;

		public IShader Simple2DTextured => Simple2DTexturedShader.Instance;

		public IShader ShadowMap => ShadowMapShader.Instance;
	}
}