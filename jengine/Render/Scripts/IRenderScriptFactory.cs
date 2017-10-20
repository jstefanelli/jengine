using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jengine.Render.Scripts
{
	public interface IRenderScriptFactory
	{
		IRenderScript MakeShadowMapScript();

		IRenderScript MakeShadowMapScript_VR();
	}
}