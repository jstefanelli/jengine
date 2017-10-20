using libjmodel.Structures.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace libjmodel.Structures
{
	//.j3d File Structure (protocol version 1)
	/*
	 *
	 * Part Definition (variable):
	 *	OffsetPosition: 12 bytes, Float3
	 *	OffsetOrientation: 16 bytes, Float4
	 *	OffsetScale: 12 bytes, Float3
	 *	Vertex Count: 4 bytes, signed integer
	 *	Vertex Array [variable]:
	 *	{
	 *		See vertex definition
	 *	}
	 */

	public class Model
	{
		public Float3 OffsetPosition { get; set; } = new Float3() { X = 0, Y = 0, Z = 0 };
		public Float4 OffsetOrientationQuaternion { get; set; } = new Float4() { X = 0, Y = 0, Z = 0, W = 1 };
		public Float3 OffsetScale { get; set; } = new Float3() { X = 1, Y = 1, Z = 1 };
		public Material Mat { get; set; }
		public Vertex[] Vertices { get; set; }

		public void Write(BinaryWriter writer)
		{
			Mat.Write(writer);
			OffsetPosition.Write(writer);
			OffsetOrientationQuaternion.Write(writer);
			OffsetScale.Write(writer);
			writer.Write(Vertices.Length);
			foreach (Vertex v in Vertices)
			{
				v.Write(writer);
			}
		}

		public static Model Load(BinaryReader reader)
		{
			Model m = new Model();
			ModelSet.Log("Loading model.");
			m.Mat = Material.Load(reader);
			m.OffsetPosition = Float3.Load(reader);
			ModelSet.Log("Position: " + m.OffsetPosition);
			m.OffsetOrientationQuaternion = Float4.Load(reader);
			ModelSet.Log("Orientation: " + m.OffsetOrientationQuaternion);
			m.OffsetScale = Float3.Load(reader);
			ModelSet.Log("Scale: " + m.OffsetScale);
			int vertices = reader.ReadInt32();
			m.Vertices = new Vertex[vertices];
			for (int i = 0; i < vertices; i++)
			{
				m.Vertices[i] = Vertex.Load(reader);
			}
			return m;
		}
	}
}