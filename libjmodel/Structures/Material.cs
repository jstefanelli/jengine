using libjmodel.Structures.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace libjmodel.Structures
{
	//.j3d File Structure (protocol version 1):
	/*
	 * Material Definition (variable):
	 *			DiffuseColor: 16 bytes, Float4
	 *			SpecularColor: 16 bytes, Float4
	 *			AmbientColor: 16 bytes, Float4
	 *			DiffusePath: variable, ASCII string
	 *			SpecularPath: variable, ASCII string
	 *			AmbientPath: variable, ASCII string
	 *			NormalPath: variable, ASCII string
	 */

	public class Material
	{
		public string DiffuseMapPath { get; set; } = "";
		public string SpecularMapPath { get; set; } = "";
		public string AmbientMapPath { get; set; } = "";

		public Float4 DiffuseColor { get; set; } = new Float4();
		public Float4 SpecularColor { get; set; } = new Float4();
		public Float4 AmbientColor { get; set; } = new Float4();

		public string NormalMapPath { get; set; } = "";

		public void Write(BinaryWriter writer)
		{
			DiffuseColor.Write(writer);
			SpecularColor.Write(writer);
			AmbientColor.Write(writer);
			if (!DiffuseMapPath.Equals(string.Empty))
			{
				writer.Write(Encoding.ASCII.GetBytes(DiffuseMapPath));
				writer.Write((byte)0);
			}
			else
			{
				writer.Write((byte)0);
			}
			if (!SpecularMapPath.Equals(string.Empty))
			{
				writer.Write(Encoding.ASCII.GetBytes(SpecularMapPath));
				writer.Write((byte)0);
			}
			else
			{
				writer.Write((byte)0);
			}
			if (!AmbientMapPath.Equals(string.Empty))
			{
				writer.Write(Encoding.ASCII.GetBytes(AmbientMapPath));
				writer.Write((byte)0);
			}
			else
			{
				writer.Write((byte)0);
			}
			if (!NormalMapPath.Equals(string.Empty))
			{
				writer.Write(Encoding.ASCII.GetBytes(NormalMapPath));
				writer.Write((byte)0);
			}
			else
			{
				writer.Write((byte)0);
			}
		}

		public static Material Load(BinaryReader reader)
		{
			ModelSet.Log("Loading material");
			Float4 diff = Float4.Load(reader);
			ModelSet.Log("Diff: " + diff);
			Float4 spec = Float4.Load(reader);
			ModelSet.Log("Spec: " + spec);
			Float4 amb = Float4.Load(reader);
			ModelSet.Log("Amb: " + amb);
			string diffMap = Utils.LoadASCIIString(reader);
			ModelSet.Log("DiffMap: '" + diffMap + "'");
			string specMap = Utils.LoadASCIIString(reader);
			ModelSet.Log("SpecMap: '" + specMap + "'");
			string ambMap = Utils.LoadASCIIString(reader);
			ModelSet.Log("AmbMap: '" + ambMap + "'");
			string normMap = Utils.LoadASCIIString(reader);
			ModelSet.Log("NormMap: '" + normMap + "'");
			return new Material
			{
				DiffuseColor = diff,
				SpecularColor = spec,
				AmbientColor = amb,
				DiffuseMapPath = diffMap,
				SpecularMapPath = specMap,
				AmbientMapPath = ambMap,
				NormalMapPath = normMap
			};
		}
	}
}