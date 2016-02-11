using System;
using System.Drawing;
using System.Drawing.Text;

namespace CryptoMessenger.Stuff
{
	/// <summary>
	/// Contains methods to create fonts.
	/// </summary>
	static class SimpleFontFactory
	{
		internal class NativeMethods
		{
			[System.Runtime.InteropServices.DllImport("gdi32.dll")]
			internal static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont,
			   IntPtr pdv, [System.Runtime.InteropServices.In] ref uint pcFonts);
		}

		/// <summary>
		/// Create font from ttf file.
		/// </summary>
		/// <param name="fonts">font collection (needed to exist when app running).</param>
		/// <param name="fontData">byte data of font (data from ttf file).</param>
		/// <param name="fontSize">size of font.</param>
		/// <returns>font of needed size.</returns>
		public static Font CreateFont(PrivateFontCollection fonts, byte[] fontData, float fontSize)
		{
			IntPtr fontPtr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(fontData.Length);
			System.Runtime.InteropServices.Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
		
			fonts.AddMemoryFont(fontPtr, fontData.Length);
			uint dummy = 0;
			NativeMethods.AddFontMemResourceEx(fontPtr, (uint)fontData.Length, IntPtr.Zero, ref dummy);
			System.Runtime.InteropServices.Marshal.FreeCoTaskMem(fontPtr);

			return new Font(fonts.Families[0], fontSize);
		}
	}
}
