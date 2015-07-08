#region

using System;
using System.Drawing;

#endregion

namespace Fesslersoft.SharedFolderSpy.Controls.UxTabControlCS
{
    internal class GdiMemoryContext : IDisposable
    {
        private readonly int fHeight;
        private readonly int fWidth;

        private readonly Graphics gdiPlusContext;
        private IntPtr fBitmap;
        private IntPtr fDC;
        private IntPtr fStockMonoBmp;

        public GdiMemoryContext(Graphics compatibleTo, int width, int height)
        {
            if (compatibleTo == null || width <= 0 || height <= 0) throw new ArgumentException("Arguments are unacceptable");
            var tmp = compatibleTo.GetHdc();
            var failed = true;
            do
            {
                if ((fDC = NativeMethods.CreateCompatibleDC(tmp)) == IntPtr.Zero) break;
                if ((fBitmap = NativeMethods.CreateCompatibleBitmap(tmp, width, height)) == IntPtr.Zero)
                {
                    NativeMethods.DeleteDC(fDC);
                    break;
                }
                fStockMonoBmp = NativeMethods.SelectObject(fDC, fBitmap);
                if (fStockMonoBmp == IntPtr.Zero)
                {
                    NativeMethods.DeleteObject(fBitmap);
                    NativeMethods.DeleteDC(fDC);
                }
                else failed = false;
            } while (false);
            compatibleTo.ReleaseHdc(tmp);
            if (failed) throw new SystemException("GDI error occured while creating context");

            gdiPlusContext = Graphics.FromHdc(fDC);
            fWidth = width;
            fHeight = height;
        }

        public Graphics Graphics
        {
            get { return gdiPlusContext; }
        }

        public int Width
        {
            get { return fWidth; }
        }

        public int Height
        {
            get { return fHeight; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Releases the unmanaged resources used by the <see cref="T:VSControls.GDIMemoryContext" /> and
        ///     optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     true to release both managed and unmanaged resources; false to release only unmanaged
        ///     resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && gdiPlusContext != null) gdiPlusContext.Dispose();

            NativeMethods.SelectObject(fDC, fStockMonoBmp); //return stock bitmap home
            NativeMethods.DeleteDC(fDC); //deletion of memory context
            fDC = fStockMonoBmp = IntPtr.Zero;
            NativeMethods.DeleteObject(fBitmap); //destruction of created bitmap
            fBitmap = IntPtr.Zero;
        }

        public void DrawContextClipped(Graphics drawTo, Rectangle drawRect)
        {
            do
            {
                if (drawTo == null || fDC == IntPtr.Zero) break;
                var tmpDC = drawTo.GetHdc();
                if (tmpDC == IntPtr.Zero) break;

                NativeMethods.BitBlt(tmpDC, drawRect.Left, drawRect.Top, drawRect.Width, drawRect.Height,
                    fDC, 0, 0, NativeMethods.SRCCOPY);

                drawTo.ReleaseHdc(tmpDC);
            } while (false);
        }

        public void FlipVertical()
        {
            if (fDC != IntPtr.Zero)
                NativeMethods.StretchBlt(fDC, 0, fHeight - 1, fWidth, -fHeight, fDC, 0, 0, fWidth, fHeight, NativeMethods.SRCCOPY);
        }

        public uint GetPixel(int x, int y)
        {
            if (fDC != IntPtr.Zero) return NativeMethods.GetPixel(fDC, x, y);
            throw new ObjectDisposedException(null, "GDI context seems to be disposed.");
        }

        public void SetPixel(int x, int y, uint value)
        {
            if (fDC != IntPtr.Zero) NativeMethods.SetPixel(fDC, x, y, value);
            else throw new ObjectDisposedException(null, "GDI context seems to be disposed.");
        }

        ~GdiMemoryContext()
        {
            Dispose(false);
        }
    }
}