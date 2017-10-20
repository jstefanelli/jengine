using jengine.Render.Wrappers;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jengine.Render
{
	public interface IDrawable
	{
		DrawableGroup Parent { get; set; }

		IBuffer Vertices { get; }
		IBuffer Normals { get; }
		IBuffer TexCoords { get; }

		Material Mat { get; }
		Vector3 Position { get; set; }
		Quaternion Orientation { get; set; }
		Vector3 Scale { get; set; }

		bool Loaded { get; }

		void Load();

		void Draw();

		void Unload();
	}
}