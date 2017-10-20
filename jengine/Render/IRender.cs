using jengine.Render.Scripts;
using jengine.Render.Shapes;
using jengine.Render.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jengine.Render
{
	public interface IRender
	{
		bool Loaded { get; }

		void Draw(double deltaSec);

		void Update();

		void Resize(int width, int height);

		void Load();

		void Unload();

		void ResetFrameBuffer();

		IShaderFactory ShaderFactory { get; }

		IShapeFactory ShapeFactory { get; }

		IRenderScriptFactory RenderScriptFactory { get; }
	}
}