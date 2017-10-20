using jengine.Cameras;
using jengine.Render.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jengine.Render.Scripts
{
	public interface IRenderScript
	{
		void Draw(List<IDrawable> drawableas, IRender render, ICamera camera);

		void Load(IRender render);

		void Unload();

		void Update();
	}
}