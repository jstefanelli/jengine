using OpenTK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jengine
{
	public static class Log
	{
		public enum ExitCodes
		{
			OK = 0x0, GENERIC_ERROR = 0x1, ASSERTION_FAILED = 0x100
		}

#if DEBUG
		public static int LogLevel = 0;
#else
		public static int LogLevel = 2;
#endif

		public static void V(string tag, string msg)
		{
			if (LogLevel == 0)
				Debug.WriteLine("V [" + tag + "] " + msg);
		}

		public static void G(string tag, string msg)
		{
			if (LogLevel <= 1)
				Debug.WriteLine("G [" + tag + "] " + msg);
		}

		public static void W(string tag, string msg)
		{
			if (LogLevel <= 2)
				Debug.WriteLine("W [" + tag + "] " + msg);
		}

		public static void E(string tag, string msg)
		{
			if (LogLevel <= 3)
				Debug.WriteLine("E [" + tag + "] " + msg);
		}

		public static void C(string tag, string msg)
		{
			if (LogLevel <= 4)
			{
				Debug.WriteLine("C [" + tag + "] " + tag + " REPORTED A CRITICAL MESSAGE");
				Debug.WriteLine("C [" + tag + "] " + msg);
			}
		}

		public static void Assert(bool assertion, string message)
		{
			if (!assertion)
			{
				C("ASSERT", "Assertion failed: " + message);
				System.Windows.Forms.MessageBox.Show("Assertion failed: " + message, "Fatal Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
				Environment.Exit((int)ExitCodes.ASSERTION_FAILED);
			}
#if DEBUG
			else
			{
				V("ASSERT", "Assertion successful (ignore this error message): " + message);
			}
#endif
		}

		public static void DropMatrix(string tag, Matrix4 mat)
		{
			V(tag, "Matrix: ");
			V(tag, "" + mat.M11 + "\t" + mat.M12 + "\t" + mat.M13 + "\t" + mat.M14);
			V(tag, "" + mat.M21 + "\t" + mat.M22 + "\t" + mat.M23 + "\t" + mat.M24);
			V(tag, "" + mat.M31 + "\t" + mat.M32 + "\t" + mat.M43 + "\t" + mat.M34);
			V(tag, "" + mat.M41 + "\t" + mat.M42 + "\t" + mat.M43 + "\t" + mat.M44);
		}
	}
}