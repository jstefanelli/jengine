using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jengine.Cameras;

namespace jengine.Render
{
	public interface IShader
	{
		bool Loaded { get; }

		void Load();

		void Unload();

		void Draw(List<IDrawable> drawables, ICamera camera);
	}
}