using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace libjmodel.Structures.Primitives
{
	public class Float3
	{
		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }

		public Float3()
		{
			X = 0;
			Y = 0;
			Z = 0;
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(X);
			writer.Write(Y);
			writer.Write(Z);
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

		public Float3 Xzy
		{
			get
			{
				return new Float3()
				{
					X = this.X,
					Y = this.Z,
					Z = this.Y
				};
			}
		}

		public Float3 Yxz
		{
			get
			{
				return new Float3()
				{
					X = this.Y,
					Y = this.X,
					Z = this.Z
				};
			}
		}

		public Float3 Yzx
		{
			get
			{
				return new Float3()
				{
					X = this.Y,
					Y = this.Z,
					Z = this.X
				};
			}
		}

		public Float3 Zxy
		{
			get
			{
				return new Float3()
				{
					X = this.Z,
					Y = this.X,
					Z = this.Y
				};
			}
		}

		public Float3 Zyx
		{
			get
			{
				return new Float3()
				{
					X = this.Z,
					Y = this.Y,
					Z = this.X
				};
			}
		}

		public static Float3 Load(BinaryReader reader)
		{
			float X = reader.ReadSingle();
			float Y = reader.ReadSingle();
			float Z = reader.ReadSingle();
			return new Float3()
			{
				X = X,
				Y = Y,
				Z = Z
			};
		}

		public override string ToString()
		{
			return "Float3 " + X + "/" + Y + "/" + Z;
		}
	}
}