using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace libjmodel.Structures.Primitives
{
	public class Float2
	{
		public float X { get; set; }
		public float Y { get; set; }

		public Float2()
		{
			X = 0;
			Y = 0;
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(X);
			writer.Write(Y);
		}

		public Float2 Xx
		{
			get
			{
				return new Float2()
				{
					X = X,
					Y = X
				};
			}
		}

		public Float2 Yy
		{
			get
			{
				return new Float2()
				{
					X = Y,
					Y = Y
				};
			}
		}

		public Float2 Yx
		{
			get
			{
				return new Float2()
				{
					X = Y,
					Y = X
				};
			}
		}

		public Float2 Xy
		{
			get
			{
				return new Float2()
				{
					X = X,
					Y = Y
				};
			}
		}

		public static Float2 Load(BinaryReader reader)
		{
			float X = reader.ReadSingle();
			float Y = reader.ReadSingle();
			return new Float2()
			{
				X = X,
				Y = Y
			};
		}

		public override string ToString()
		{
			return "Float2 " + X + "/" + Y;
		}
	}
}