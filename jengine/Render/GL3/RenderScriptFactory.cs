using jengine.Render.GL3.Scripts;
using jengine.Render.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jengine.Render.GL3
{
	public class RenderScriptFactory : IRenderScriptFactory
	{
		private static RenderScriptFactory _instance = null;

		public static RenderScriptFactory Instance
		{
			get
			{
				if (_instance == null)
					_instance = new RenderScriptFactory();
				return _instance;
			}
		}

		public IRenderScript MakeShadowMapScript()
		{
			return new ShadowMapScript();
		}

		public IRenderScript MakeShadowMapScript_VR()
		{
			return new VRShadowMapRenderScript();
		}
	}
}