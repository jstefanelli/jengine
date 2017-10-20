using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace libjmodel.Structures.Primitives
{
	public class Float4
	{
		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }
		public float W { get; set; }

		public Float4()
		{
			X = 0;
			Y = 0;
			Z = 0;
			W = 0;
		}

		public void Write(BinaryWriter writer)
		{
			ModelSet.Log("Writing float value to: " + writer.BaseStream.Position);
			writer.Write(X);
			ModelSet.Log("Wrote X as: " + X);
			writer.Write(Y);
			ModelSet.Log("Wrote Y as: " + Y);
			writer.Write(Z);
			ModelSet.Log("Wrote Z as: " + Z);
			writer.Write(W);
			ModelSet.Log("Wrote W as: " + W);
		}

		public Float3 Xyz
		{
			get
			{
				return new Float3()
				{
					X = this.X,
					Y = this.Y,
					Z = this.Z
				};
			}
		}

		public static Float4 Load(BinaryReader reader)
		{
			ModelSet.Log("Loading Float4 from position: " + reader.BaseStream.Position);
			float X = reader.ReadSingle();
			float Y = reader.ReadSingle();
			float Z = reader.ReadSingle();
			float W = reader.ReadSingle();
			return new Float4
			{
				X = X,
				Y = Y,
				Z = Z,
				W = W
			};
		}

		public override string ToString()
		{
			return "Float4 " + X + "/" + Y + "/" + Z + "/" + W;
		}
	}
}