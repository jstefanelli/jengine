using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace AmbitionUI.Render
{
	public abstract class UIShader
	{
		protected UISystem BaseSystem { get; set; }
		public int MatrixPosition { get; protected set; }
		public int Id { get; protected set; }
		public bool Loaded { get; protected set; } = false;

		public abstract void Load();

		public abstract void Unload();

		protected UIShader(UISystem system)
		{
			BaseSystem = system;
		}
	}

	public static class ShaderUtils
	{
		public static int CompileShader(string vsFile, string fsFile)
		{
			if (!File.Exists(vsFile))
			{
				Console.WriteLine("Cannot find vertex shader file");
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
				Console.WriteLine("Failed to compile vertex shader: ");
				Console.WriteLine("\n" + log);
				throw new Exception("Failed to compile vertex shader: " + log);
			}

			if (!File.Exists(fsFile))
			{
				Console.WriteLine("Cannot find fragment shader file");
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
				Console.WriteLine("Failed to compile fragment shader: ");
				Console.WriteLine("\n" + log);
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
				Console.WriteLine("Failed to link program: ");
				Console.WriteLine("\n" + log);
				throw new Exception("Failed to link program: " + log);
			}
			GL.DeleteShader(vId);
			GL.DeleteShader(fId);

			return Id;
		}
	}
}