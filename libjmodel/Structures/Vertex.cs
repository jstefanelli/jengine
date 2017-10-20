using libjmodel.Structures.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace libjmodel.Structures
{
	public struct Vertex
	{
		//.j3d File Structure (protocol version 1)
		/* Vertex Definition:
		 *	Position: 12 bytes, Float3
		 *	Normal: 12 bytes, Float3
		 *	TexCoord: 8 bytes, Float2
		 */

		public Float3 Position { get; set; }
		public Float3 Normal { get; set; }
		public Float2 TexCoords { get; set; }

		public void Write(BinaryWriter writer)
		{
			Position.Write(writer);
			Normal.Write(writer);
			TexCoords.Write(writer);
		}

		public static Vertex Load(BinaryReader reader)
		{
			ModelSet.Log("Loading vertex");
			Float3 pos = Float3.Load(reader);
			ModelSet.Log("Pos: " + pos);
			Float3 norm = Float3.Load(reader);
			ModelSet.Log("Norm: " + norm);
			Float2 txtCoord = Float2.Load(reader);
			ModelSet.Log("TxtCoord: " + txtCoord);
			return new Vertex()
			{
				Position = pos,
				Normal = norm,
				TexCoords = txtCoord
			};
		}
	}
}