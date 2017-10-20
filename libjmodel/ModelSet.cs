using libjmodel.Structures;
using libjmodel.Structures.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace libjmodel
{
	public class ModelSet
	{
		//.j3d File structure (protocol version 1):
		/*
		 * J3D: 4 bytes, ASCII (NULL counts)
		 * Version: 4 bytes, signed integer (1)
		 * PartsNumber: 4 byes, signed integer
		 * Parts Array [varaible]:
		 * {
		 *		Material Definition (variable):
		 *			DiffuseColor: 16 bytes, Float4
		 *			SpecularColor: 16 bytes, Float4
		 *			AmbientColor: 16 bytes, Float4
		 *			DiffusePath: variable, ASCII string
		 *			SpecularPath: variable, ASCII string
		 *			AmbientPath: variable, ASCII string
		 *			NormalPath: variable, ASCII string
		 *		Part Definition (variable):
		 *			OffsetPosition: 12 bytes, Float3
		 *			OffsetOrientation: 16 bytes, Float4
		 *			OffsetScale: 12 bytes, Float3
		 *			Vertex Count: 4 bytes, signed integer
		 *			Vertex Array [variable]:
		 *			{
		 *				Position: 12 bytes, Float3
		 *				Normal: 12 bytes, Float3
		 *				TexCoord: 8 bytes, Float2
		 *			}
		 * }
		 */

		public const int ProtocolVersion = 1;

		public Model[] Parts { get; set; }

		public bool SaveToFile(string path)
		{
			try
			{
				BinaryWriter writer = new BinaryWriter(new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write));
				writer.Write(Encoding.ASCII.GetBytes("J3D"));
				writer.Write((byte)0);
				writer.Write(ProtocolVersion);
				writer.Write(Parts.Length);
				foreach (Model m in Parts)
				{
					m.Write(writer);
				}
				writer.Close();
				return true;
			}
			catch (IOException)
			{
				return false;
			}
		}

		public static bool DoLog { get; set; } = false;

		public static void Log(string msg)
		{
#if DEBUG
			if (DoLog)
				Debug.WriteLine(msg);
#endif
		}

		/// <summary>
		/// Loads a ModelSet from a binary file written respecting the .j3d specifications (no file name checks are applied)
		/// </summary>
		/// <param name="path">The path of the file to load</param>
		/// <param name="model">Output value (null on failure)</param>
		/// <returns>True on succes. False on failure (<paramref name="model"/> is null)</returns>
		public static int LoadFromFile(string path, out ModelSet model)
		{
			Log("Trying to load: " + path);
			BinaryReader r;
			try
			{
				r = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read));
			}
			catch (IOException)
			{
				model = null;
				return -1;
			}
			string s = Utils.LoadASCIIString(r);
			Log("Magic String: " + s);
			if (!s.Equals("J3D"))
			{
				model = null;
				return -2;
			}
			int v = r.ReadInt32();
			Log("Protocol version: " + v);
			if (v != ProtocolVersion)
			{
				model = null;
				return -3;
			}
			model = new ModelSet();
			int parts = r.ReadInt32();
			Log("Parts: " + parts);
			model.Parts = new Model[parts];
			for (int i = 0; i < parts; i++)
			{
				model.Parts[i] = Model.Load(r);
			}
			return 0;
		}
	}
}