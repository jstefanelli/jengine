using jengine.Render.Wrappers;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jengine.Render
{
	public class Material
	{
		public Material()
		{
			AmbientColor = new Vector4(0.1f, 0.1f, 0.1f, 1);
			DiffuseColor = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);
			SpecularColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
		}

		public Vector4 DiffuseColor { get; set; }
		public Vector4 SpecularColor { get; set; }
		public Vector4 AmbientColor { get; set; }

		public ITexture2D DiffuseTexture { get; set; } = null;
		public ITexture2D SpecularTexture { get; set; } = null;
		public ITexture2D AmbientTexture { get; set; } = null;
		public ITexture2D NormalMap { get; set; } = null;
	}
}