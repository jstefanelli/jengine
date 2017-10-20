using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using System.IO;

namespace jengine.Render.GL3.Shaders
{
	public static class ShaderUtils
	{
		public static string TAG = "GL31/Shaders/ShaderUtils";

		public static int CompileShader(string vsFile, string fsFile)
		{
			if (!File.Exists(vsFile))
			{
				Log.C(TAG, "Cannot find vertex shader file");
				throw new FileNotFoundException("Cannot file vertex shader file: " + vsFile);
			}
			String vShaderSource = File.ReadAllText(vsFile);
			int vId = GL.CreateShader(ShaderType.VertexShader);
			GL.ShaderSource(vId, vShaderSource);
			GL.CompileShader(vId);
			GL.GetShader(vId, ShaderParameter.CompileStatus, out int param);
			if (param == 0)
			{
				string log = GL.GetShaderInfoLog(vId);
				Log.C(TAG, "Failed to compile vertex shader: ");
				Log.E(TAG, "\n" + log);
				throw new Exception("Failed to compile vertex shader: " + log);
			}

			if (!File.Exists(fsFile))
			{
				Log.C(TAG, "Cannot find fragment shader file");
				throw new FileNotFoundException("Cannot file fragment shader file: " + fsFile);
			}
			String fShaderSource = File.ReadAllText(fsFile);
			int fId = GL.CreateShader(ShaderType.FragmentShader);
			GL.ShaderSource(fId, fShaderSource);
			GL.CompileShader(fId);
			GL.GetShader(fId, ShaderParameter.CompileStatus, out int fparam);
			if (fparam == 0)
			{
				string log = GL.GetShaderInfoLog(fId);
				Log.C(TAG, "Failed to compile fragment shader: ");
				Log.E(TAG, "\n" + log);
				throw new Exception("Failed to compile fragment shader: " + log);
			}

			int Id = GL.CreateProgram();
			GL.AttachShader(Id, vId);
			GL.AttachShader(Id, fId);
			GL.LinkProgram(Id);
			GL.GetProgram(Id, GetProgramParameterName.LinkStatus, out int pparam);
			if (pparam == 0)
			{
				string log = GL.GetProgramInfoLog(Id);
				Log.C(TAG, "Failed to link program: ");
				Log.E(TAG, "\n" + log);
				throw new Exception("Failed to link program: " + log);
			}
			GL.DeleteShader(vId);
			GL.DeleteShader(fId);

			return Id;
		}
	}
}