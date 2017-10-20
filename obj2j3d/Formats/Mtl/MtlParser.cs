using libjmodel;
using libjmodel.Structures;
using libjmodel.Structures.Primitives;
using obj2j3d.Formats.Obj;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace obj2j3d.Formats.Mtl
{
	public class MtlParser
	{
		public static Regex newMtlRegex = new Regex("newmtl (.+)", RegexOptions.IgnoreCase);

		public static Regex kaRegex = new Regex("ka " + ObjParser.FloatDefRgx + " " + ObjParser.FloatDefRgx + " " + ObjParser.FloatDefRgx, RegexOptions.IgnoreCase);
		public static Regex ksRegex = new Regex("ks " + ObjParser.FloatDefRgx + " " + ObjParser.FloatDefRgx + " " + ObjParser.FloatDefRgx, RegexOptions.IgnoreCase);
		public static Regex kdRegex = new Regex("kd " + ObjParser.FloatDefRgx + " " + ObjParser.FloatDefRgx + " " + ObjParser.FloatDefRgx, RegexOptions.IgnoreCase);

		public static Regex map_kaRegex = new Regex("map_ka (.+)", RegexOptions.IgnoreCase);
		public static Regex map_ksRegex = new Regex("map_ks (.+)", RegexOptions.IgnoreCase);
		public static Regex map_kdRegex = new Regex("map_kd (.+)", RegexOptions.IgnoreCase);

		public static Dictionary<string, Material> ParseMtl(string path)
		{
			Dictionary<string, Material> materials = new Dictionary<string, Material>();

			string currentMaterialname = "";
			Material currentMaterial = new Material();
			foreach (string line in File.ReadLines(path))
			{
				if (newMtlRegex.IsMatch(line))
				{
					currentMaterialname = newMtlRegex.Match(line).Groups[1].Value;
					currentMaterial = new Material();
					materials.Add(currentMaterialname, currentMaterial);
					continue;
				}
				if (kaRegex.IsMatch(line))
				{
					Match m = kaRegex.Match(line);
					materials[currentMaterialname].AmbientColor = new Float4()
					{
						X = ObjParser.ObjFloat(m.Groups[1].Value),
						Y = ObjParser.ObjFloat(m.Groups[2].Value),
						Z = ObjParser.ObjFloat(m.Groups[3].Value),
						W = 1.0f
					};
					foreach (Capture c in m.Groups)
					{
						ModelSet.Log(c.Value);
					}
					ModelSet.Log("Found Ambient: " + materials[currentMaterialname].AmbientColor + " from " + line);
					continue;
				}
				if (kdRegex.IsMatch(line))
				{
					Match m = kdRegex.Match(line);
					materials[currentMaterialname].DiffuseColor = new Float4()
					{
						X = ObjParser.ObjFloat(m.Groups[1].Value),
						Y = ObjParser.ObjFloat(m.Groups[2].Value),
						Z = ObjParser.ObjFloat(m.Groups[3].Value)
					};
					continue;
				}

				if (ksRegex.IsMatch(line))
				{
					Match m = ksRegex.Match(line);
					materials[currentMaterialname].SpecularColor = new Float4()
					{
						X = ObjParser.ObjFloat(m.Groups[1].Value),
						Y = ObjParser.ObjFloat(m.Groups[2].Value),
						Z = ObjParser.ObjFloat(m.Groups[3].Value)
					};
					continue;
				}
				if (map_kaRegex.IsMatch(line))
				{
					Match m = map_kaRegex.Match(line);
					materials[currentMaterialname].AmbientMapPath = m.Groups[1].Value;
					continue;
				}
				if (map_kdRegex.IsMatch(line))
				{
					Match m = map_kdRegex.Match(line);
					materials[currentMaterialname].DiffuseMapPath = m.Groups[1].Value;
					continue;
				}
				if (map_ksRegex.IsMatch(line))
				{
					Match m = map_ksRegex.Match(line);
					materials[currentMaterialname].SpecularMapPath = m.Groups[1].Value;
					continue;
				}
			}

			return materials;
		}
	}
}