#region License
/*
MIT License
Copyright � 2006 The Mono.Xna Team

All rights reserved.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
#endregion License

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using Microsoft.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.Content
{
    internal class Texture2DReader : ContentTypeReader<Texture2D>
    {
        internal Texture2DReader()
        {
            // Do nothing
        }

		public static string Normalize(string FileName)
		{
			if (File.Exists(FileName))
				return FileName;
			
			// Check the file extension
			if (!string.IsNullOrEmpty(Path.GetExtension(FileName)))
			{
				return null;
			}
			
			// Concat the file name with valid extensions
			if (File.Exists(FileName+".xnb"))
				return FileName+".xnb";
			if (File.Exists(FileName+".jpg"))
				return FileName+".jpg";
			if (File.Exists(FileName+".bmp"))
				return FileName+".bmp";
			if (File.Exists(FileName+".jpeg"))
				return FileName+".jpeg";
			if (File.Exists(FileName+".png"))
				return FileName+".png";
			if (File.Exists(FileName+".gif"))
				return FileName+".gif";
			if (File.Exists(FileName+".pict"))
				return FileName+".pict";
			if (File.Exists(FileName+".tga"))
				return FileName+".tga";
			
			return null;
		}
		
		protected internal override Texture2D Read(ContentReader reader, Texture2D existingInstance)
		{
			Texture2D texture = null;
			
			SurfaceFormat surfaceFormat;
			if (reader.version < 5) {
				SurfaceFormat_Legacy legacyFormat = (SurfaceFormat_Legacy)reader.ReadInt32 ();
				switch(legacyFormat) {
				case SurfaceFormat_Legacy.Dxt1:
					surfaceFormat = SurfaceFormat.Dxt1;
					break;
				case SurfaceFormat_Legacy.Dxt3:
					surfaceFormat = SurfaceFormat.Dxt3;
					break;
				case SurfaceFormat_Legacy.Dxt5:
					surfaceFormat = SurfaceFormat.Dxt5;
					break;
				case SurfaceFormat_Legacy.Color:
					surfaceFormat = SurfaceFormat.Color;
					break;
				case SurfaceFormat_Legacy.Bgra4444:
					surfaceFormat = SurfaceFormat.Bgra4444;
					break;
				case SurfaceFormat_Legacy.Bgra5551:
					surfaceFormat = SurfaceFormat.Bgra5551;
					break;
				case SurfaceFormat_Legacy.Alpha8:
					surfaceFormat = SurfaceFormat.Alpha8;
					break;
				default:
					throw new NotImplementedException();
				}
			} else {
				surfaceFormat = (SurfaceFormat)reader.ReadInt32 ();
			}
			int width = reader.ReadInt32 ();
			int height = reader.ReadInt32 ();
			int levelCount = reader.ReadInt32 ();

			int imageLength = reader.ReadInt32 ();
			
			byte[] imageBytes = reader.ReadBytes (imageLength);
			IntPtr ptr = Marshal.AllocHGlobal (imageBytes.Length);
			try
			{
				Marshal.Copy (imageBytes, 0, ptr, imageBytes.Length);
				ESTexture2D temp = new ESTexture2D(ptr, imageBytes.Length, surfaceFormat, width, height, new Size (width, height), All.Linear);
				texture = new Texture2D (new ESImage (temp));
			}
			finally
			{
				Marshal.FreeHGlobal (ptr);
			}
			
			return texture;
		}
    }
}