using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jengine.Render.Wrappers
{
	public interface ITexture2D
	{
		bool Loaded { get; }

		void Load();

		void Unload();
	}
}