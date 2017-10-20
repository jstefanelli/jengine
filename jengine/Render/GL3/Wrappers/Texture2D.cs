using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using jengine.Render.Wrappers;

namespace jengine.Render.GL3.Wrappers
{
	public class Texture2D : ITexture2D
	{
		public const string TAG = "GL3/Wrappers/Texture2D";

		public int Id { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }
		public int Bpp { get; private set; }
		public string Path { get; protected set; }
		public bool Loaded { get; private set; }
		private Bitmap BitMap;

		public Texture2D(int Id)
		{
			Loaded = true;
			this.Id = Id;
			Width = -1;
			Height = -1;
			Bpp = 32;
			BitMap = null;
		}

		public Texture2D(string path)
		{
			Loaded = false;
			Path = path;

			if (!File.Exists(path))
			{
				Log.C(TAG, "Trying to load non-existent file");
				throw new FileNotFoundException("Texture2D: Referenced file does not exist");
			}

			Id = -1;
			Width = -1;
			Height = -1;
			Bpp = -1;

			BitMap = new Bitmap(path);
			Width = BitMap.Width;
			Height = BitMap.Height;
			Bpp = 4;
		}

		public Texture2D(Bitmap b)
		{
			Loaded = false;
			Id = -1;
			BitMap = b;
			Width = b.Width;
			Height = b.Height;
			Bpp = 4;
		}

		/// <summary>
		/// Equivalent of <see cref="Load(bool)"/> with doAnisotropy = true
		/// </summary>
		public void Load()
		{
			Load(true);
		}

		/// <summary>
		/// Loads the Texture from the file
		/// </summary>
		/// <param name="doAnisotropy">Apply anisotropic filtering (does not override Game setting AnisotropyLevel when true)</param>
		public void Load(bool doAnisotropy)
		{
			if (Loaded)
				return;
			Id = GL.GenTexture();

			GL.BindTexture(TextureTarget.Texture2D, Id);
			Log.V(TAG, "Created new Texture with Id: " + Id);
			int MinFilter = (int)TextureMinFilter.LinearMipmapNearest;
			int MagFilter = (int)TextureMagFilter.Linear;
			int Wrap_S = (int)TextureWrapMode.ClampToEdge;
			int Wrap_T = (int)TextureWrapMode.ClampToEdge;

			if (doAnisotropy && GL.GetString(StringName.Extensions).Contains("GL_EXT_texture_filter_anisotropic"))
			{
				GL.GetFloat((GetPName)ExtTextureFilterAnisotropic.MaxTextureMaxAnisotropyExt, out float MaxAniso);
				Log.V(TAG, "Max Anisotropic Level: " + MaxAniso);
				float myAniso = Game.Instance.LoadedSettings.AnisotropyLevel;
				Log.V(TAG, "My Anisotropic Level: " + myAniso);
				if (myAniso > MaxAniso)
				{
					myAniso = MaxAniso;
					Game.Instance.LoadedSettings.AnisotropyLevel = (int)MaxAniso;
				}
				GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt, myAniso);
			}
			GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, ref MinFilter);
			GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, ref MagFilter);
			GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, ref Wrap_S);
			GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, ref Wrap_T);
			ErrorCode Error = GL.GetError();
			while (Error != ErrorCode.NoError)
			{
				Error = GL.GetError();
				Log.E(TAG, "GL_ERROR: " + Error.ToString());
			}
			BitmapData data = BitMap.LockBits(new System.Drawing.Rectangle(0, 0, BitMap.Width, BitMap.Height),
			ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
				OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
			BitMap.UnlockBits(data);
			Error = GL.GetError();
			GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
			Loaded = true;
		}

		public void Unload()
		{
			GL.DeleteTexture(Id);
			BitMap = null;
			Id = -1;
			Loaded = false;
		}
	}
}