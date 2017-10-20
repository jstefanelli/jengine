using libjmodel;
using libjmodel.Structures;
using libjmodel.Structures.Primitives;
using obj2j3d.Formats.Obj;
using System;
using System.Collections.Generic;
using System.IO;

namespace obj2j3d
{
	internal class Program
	{
		private static string InputFilePath = null;
		private static string OutputFilePath = null;

		private static void Main(string[] args)
		{
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i] == "-i" || args[i] == "--input")
				{
					i++;
					InputFilePath = args[i];
					continue;
				}
				if (args[i] == "-o" || args[i] == "--output")
				{
					i++;
					OutputFilePath = args[i];
					continue;
				}
				if (args[i] == "-v")
				{
					Console.WriteLine("JEngine .obj to .j3d converter.");
					Console.WriteLine("Options:\n-i\t--input\t\t\tInput file path\n-o\t--output\t\tOutput File Path");
					return;
				}
			}
			if (InputFilePath == null)
			{
				Console.WriteLine("Error: No input file specified.");
				return;
			}
			if (!File.Exists(InputFilePath))
			{
				Console.WriteLine("Error: File not found.");
				return;
			}
			if (OutputFilePath == null)
			{
				FileInfo InputInfo = new FileInfo(InputFilePath);
				string InputExt = InputInfo.Extension;
				string OutputName = InputInfo.FullName.Substring(0, InputInfo.FullName.IndexOf(InputExt));
				OutputFilePath = OutputName + ".j3d";
				Console.WriteLine("Computed file path: " + OutputFilePath);
			}
			Console.WriteLine("Parsing...");
			ObjParser p = ObjParser.ParseObj(InputFilePath);
			Console.WriteLine("Converting...");
			ModelSet m = Convert(p);
			Console.WriteLine("Saving....");
			m.SaveToFile(OutputFilePath);
		}

		public static ModelSet Convert(ObjParser obj)
		{
			ModelSet m = new ModelSet();
			string lastMaterialname = "";
			bool firstIteartion = true;
			Model lastPart = new Model();
			List<Model> parts = new List<Model>();
			List<Vertex> CurrenVertices = new List<Vertex>();
			foreach (ObjParser.Face f in obj.Faces)
			{
				if (lastMaterialname != f.MaterialName || firstIteartion)
				{
					if (!firstIteartion)
					{
						lastPart.Vertices = CurrenVertices.ToArray();
						parts.Add(lastPart);
					}
					lastMaterialname = f.MaterialName;
					firstIteartion = false;
					lastPart = new Model();
					lastPart.Mat = obj.Materials[f.MaterialName];
					CurrenVertices = new List<Vertex>();
				}
				int vertexNum = f.Vertices.Length;
				int texCoordNum = (f.TexCoords != null) ? f.TexCoords.Length : 0;
				int normalNum = (f.Normals != null) ? f.Normals.Length : 0;
				for (int i = 0; i < vertexNum; i++)
				{
					Vertex v = new Vertex();
					v.Position = f.Vertices[i];
					if (texCoordNum == vertexNum)
						v.TexCoords = f.TexCoords[i];
					else
						v.TexCoords = new Float2() { X = 0, Y = 0 };
					if (normalNum == vertexNum)
						v.Normal = f.Normals[i];
					else
						v.Normal = new Float3() { X = 0, Y = 0, Z = 0 };
					CurrenVertices.Add(v);
				}
			}
			try
			{
				lastPart.Vertices = CurrenVertices.ToArray();
				parts.Add(lastPart);
			}
			catch (Exception)
			{
			}
			m.Parts = parts.ToArray();
			return m;
		}
	}
}