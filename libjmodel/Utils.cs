using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace libjmodel
{
	public static class Utils
	{
		private const int BUFFER_SIZE = 1024;

		public static String LoadASCIIString(BinaryReader reader)
		{
			byte[] buffer = new byte[BUFFER_SIZE];
			int i = 0;
			while (i < BUFFER_SIZE)
			{
				buffer[i] = reader.ReadByte();
				if (buffer[i] == 0)
				{
					break;
				}
				i++;
			}
			return Encoding.ASCII.GetString(buffer, 0, i);
		}
	}
}