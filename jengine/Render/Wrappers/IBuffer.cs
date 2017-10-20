using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jengine.Render.Wrappers
{
	public interface IBuffer
	{
		bool Loaded { get; }

		void Load(float[] data, int epp);

		void Unload();
	}
}