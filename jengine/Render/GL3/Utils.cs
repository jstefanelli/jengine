using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace jengine.Render.GL3
{
	public static class Utils
	{
		public const string TAG = "GL3/Utils";

		public static void LogError(string msg)
		{
			ErrorCode e = GL.GetError();
			if (e != ErrorCode.NoError)
			{
				Log.W(TAG, "GL Error " + e.ToString() + " from " + msg);
			}
		}
	}
}