using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valve.VR;

namespace jengine.Control.VR
{
	public static class opentk_interop
	{
		public static Matrix4 ConvertTK(HmdMatrix44_t mat)
		{
			return new Matrix4(mat.m0, mat.m4, mat.m8, mat.m12, mat.m1, mat.m5, mat.m9, mat.m13, mat.m2, mat.m6, mat.m10, mat.m14, mat.m3, mat.m7, mat.m11, mat.m15);
		}

		public static HmdMatrix44_t ConvertVR(Matrix4 mat)
		{
			HmdMatrix44_t m = new HmdMatrix44_t
			{
				m0 = mat.M11,
				m1 = mat.M21,
				m2 = mat.M31,
				m3 = mat.M41,
				m4 = mat.M12,
				m5 = mat.M22,
				m6 = mat.M32,
				m7 = mat.M42,
				m8 = mat.M13,
				m9 = mat.M23,
				m10 = mat.M33,
				m11 = mat.M43,
				m12 = mat.M14,
				m13 = mat.M24,
				m14 = mat.M34,
				m15 = mat.M44
			};

			return m;
		}

		public static Matrix4 ConvertTK(HmdMatrix34_t mat)
		{
			return new Matrix4(mat.m0, mat.m4, mat.m8, 0.0f,
				mat.m1, mat.m5, mat.m9, 0.0f,
				mat.m2, mat.m6, mat.m10, 0.0f,
				mat.m3, mat.m7, mat.m11, 1.0f);
		}

		public static Vector3 ConvertTK(HmdVector3_t v)
		{
			return new Vector3(v.v0, v.v1, v.v2);
		}
	}
}