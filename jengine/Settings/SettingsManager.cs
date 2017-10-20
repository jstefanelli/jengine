using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace jengine.Settings
{
	public class SettingsManager
	{
		public const string TAG = "SettingsMnagaer";

		public const string WindowResX_pattern = @"^[wW]indow[rR]esX +(\d+)";
		public const string WindowResY_pattern = @"^[wW]indow[rR]esY +(\d+)";
		public const string FrameResX_pattern = @"^[fF]rame[Rr]esX +(\d+)";
		public const string FrameResY_pattern = @"^[fF]rame[Rr]esY +(\d+)";
		public const string ShadowMapRes_pattern = @"^[sS]hadow[mM]ap[rR]es +(\d+)";
		public const string MSAA_pattern = @"^[Mm][Ss][Aa][aA][Ll]evel +(\d+)";
		public const string Anisotropy_pattern = @"^[Aa]nisotropy[lL]evel +(\d+)";
		public const string VSync_pattern = @"^[vV][Ss]ync +([tT]rue|[fF]alse)";
		public const string Fullscreen_pattern = @"^[Ff]ull[sS]creen +([tT]rue|[fF]alse)";
		public const string FPS_pattern = @"^[Ff][Pp][Ss] +(\d+)";
		public const string FarPlane_pattern = @"^[fF]ar[pP]lane +([\d.\-eEf,]+)";
		public const string NearPlane_pattern = @"^[nN]ear[pP]lane +([\d.\-eEf,]+)";
		public const string FOV_pattern = @"^[fF][oO][vV] +([\d.\-eEf,]+)";
		public const string OpenVREnabled_pattern = @"^[Oo]pen[Vv][Rr][Ee]nabled +([tT]rue|[fF]alse)";
		public const string OpenVRDispLighthouses_pattern = @"^[Oo]pen[Vv][Rr][Dd]isp[Ll]ight[hH]ouse +([tT]rue|[fF]alse)";

		public int WindowResolutionX { get; set; } = 1280;
		public int WindowResolutionY { get; set; } = 720;

		public int FrameResolutionX { get; set; } = 1280;
		public int FrameResolutionY { get; set; } = 720;

		public int ShadowMapResolution { get; set; } = 2048;

		public int MSAALevel { get; set; } = 0;
		public int AnisotropyLevel { get; set; } = 0;

		public bool VSync { get; set; } = true;
		public bool Fullscreen { get; set; } = false;

		public float FarPlane { get; set; } = 100.0f;
		public float NearPlane { get; set; } = 0.1f;

		public float FOVDegrees { get; set; } = 80.0f;

		public int RefreshRate { get; set; } = 60;

		#region OpenVR

		public bool OpenVREnabled { get; set; } = false;
		public bool OpenVRDisplayLighthouses { get; set; } = false;

		#endregion OpenVR

		private SettingsManager()
		{
		}

		public void SaveToFIle(string path)
		{
			FileStream f = new FileStream(path, FileMode.Create);
			if (!f.CanWrite)
			{
				throw new IOException("Cannot write to file: " + path);
			}
			StreamWriter w = new StreamWriter(f);
			w.WriteLine("[jengine settings]");
			w.WriteLine("WindowResX " + WindowResolutionX);
			w.WriteLine("WindowResY " + WindowResolutionY);
			w.WriteLine("FrameResX " + FrameResolutionX);
			w.WriteLine("FrameResY " + FrameResolutionY);
			w.WriteLine("ShadowMapRes " + ShadowMapResolution);
			w.WriteLine("FarPlane " + FarPlane);
			w.WriteLine("NearPlane " + NearPlane);
			w.WriteLine("FOV " + FOVDegrees);
			w.WriteLine("MSAALevel " + MSAALevel);
			w.WriteLine("AnisotropyLevel " + AnisotropyLevel);
			w.WriteLine("VSync " + VSync);
			w.WriteLine("FullScreen " + Fullscreen);
			w.WriteLine("FPS " + RefreshRate);
			w.WriteLine();
			w.WriteLine("[OpenVR settings]");
			w.WriteLine("OpenVREnabled " + OpenVREnabled);
			w.WriteLine("OpenVRDispLighthouse " + OpenVRDisplayLighthouses);
			w.Close();
		}

		public static SettingsManager LoadFromFile(string path)
		{
			SettingsManager s = new SettingsManager();
			FileStream fs = new FileStream(path, FileMode.Open);
			if (!fs.CanRead)
			{
				throw new IOException("Cannot read from file: " + path);
			}
			StreamReader st = new StreamReader(fs);
			string line;
			while ((line = st.ReadLine()) != null)
			{
				if (Regex.IsMatch(line, WindowResX_pattern))
				{
					s.WindowResolutionX = Int32.Parse(Regex.Match(line, WindowResX_pattern).Groups[1].Value);
					continue;
				}
				if (Regex.IsMatch(line, WindowResY_pattern))
				{
					s.WindowResolutionY = Int32.Parse(Regex.Match(line, WindowResY_pattern).Groups[1].Value);
					continue;
				}
				if (Regex.IsMatch(line, FrameResX_pattern))
				{
					s.FrameResolutionX = Int32.Parse(Regex.Match(line, FrameResX_pattern).Groups[1].Value);
					continue;
				}
				if (Regex.IsMatch(line, FrameResY_pattern))
				{
					s.FrameResolutionY = Int32.Parse(Regex.Match(line, FrameResY_pattern).Groups[1].Value);
					continue;
				}
				if (Regex.IsMatch(line, ShadowMapRes_pattern))
				{
					s.ShadowMapResolution = Int32.Parse(Regex.Match(line, ShadowMapRes_pattern).Groups[1].Value);
					continue;
				}
				if (Regex.IsMatch(line, MSAA_pattern))
				{
					s.MSAALevel = Int32.Parse(Regex.Match(line, MSAA_pattern).Groups[1].Value);
					continue;
				}
				if (Regex.IsMatch(line, Anisotropy_pattern))
				{
					s.AnisotropyLevel = Int32.Parse(Regex.Match(line, Anisotropy_pattern).Groups[1].Value);
					continue;
				}
				if (Regex.IsMatch(line, VSync_pattern))
				{
					s.VSync = Boolean.Parse(Regex.Match(line, VSync_pattern).Groups[1].Value);
					continue;
				}
				if (Regex.IsMatch(line, Fullscreen_pattern))
				{
					s.Fullscreen = Boolean.Parse(Regex.Match(line, Fullscreen_pattern).Groups[1].Value);
					continue;
				}
				if (Regex.IsMatch(line, FPS_pattern))
				{
					s.RefreshRate = Int32.Parse(Regex.Match(line, FPS_pattern).Groups[1].Value);
					continue;
				}
				if (Regex.IsMatch(line, FarPlane_pattern))
				{
					s.FarPlane = float.Parse(Regex.Match(line, FarPlane_pattern).Groups[1].Value);
					continue;
				}
				if (Regex.IsMatch(line, NearPlane_pattern))
				{
					s.NearPlane = float.Parse(Regex.Match(line, NearPlane_pattern).Groups[1].Value);
					continue;
				}
				if (Regex.IsMatch(line, FOV_pattern))
				{
					s.FOVDegrees = float.Parse(Regex.Match(line, FOV_pattern).Groups[1].Value);
					continue;
				}
				if (Regex.IsMatch(line, OpenVREnabled_pattern))
				{
					s.OpenVREnabled = Boolean.Parse(Regex.Match(line, OpenVREnabled_pattern).Groups[1].Value);
					continue;
				}
				if (Regex.IsMatch(line, OpenVRDispLighthouses_pattern))
				{
					s.OpenVRDisplayLighthouses = Boolean.Parse(Regex.Match(line, OpenVRDispLighthouses_pattern).Groups[1].Value);
					continue;
				}
			}
			st.Close();
			return s;
		}

		public static SettingsManager Default => new SettingsManager();
	}
}