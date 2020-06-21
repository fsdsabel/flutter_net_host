using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace FlutterHost.Interop
{
    public class NativeMethods
    {
        private const string USER32DLL = "user32.dll";
        private const string KERNEL32DLL = "kernel32.dll";
        private const string COMCTL32DLL = "comctl32.dll";
        private const string VERSIONDLL = "version.dll";

        public delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
            public RECT(Rectangle src)
            {
                left = src.Left;
                top = src.Top;
                right = src.Right;
                bottom = src.Bottom;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPOS
        {
            public IntPtr Hwnd;
            public IntPtr HwndInsertAfter;
            public int X;
            public int Y;
            public int Cx;
            public int Cy;
            public SWP Flags;
        }

        public struct SIZE
        {
            public int width;
            public int height;
        }

        public struct POINT
        {
            public int x;
            public int y;
        }

        [Flags]
        public enum SWP
        {
            SWP_NOSIZE = 0x0001,
            SWP_NOMOVE = 0x0002,
            SWP_NOZORDER = 0x0004,
            SWP_NOREDRAW = 0x0008,
            SWP_NOACTIVATE = 0x0010,
            SWP_FRAMECHANGED = 0x0020,
            SWP_SHOWWINDOW = 0x0040,
            SWP_HIDEWINDOW = 0x0080,
            SWP_NOCOPYBITS = 0x0100,
            SWP_NOOWNERZORDER = 0x0200,
            SWP_NOSENDCHANGING = 0x0400,
            SWP_DRAWFRAME = SWP_FRAMECHANGED,
            SWP_NOREPOSITION = SWP_NOOWNERZORDER,
            SWP_DEFERERASE = 0x2000,
            SWP_ASYNCWINDOWPOS = 0x4000
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct VS_FIXEDFILEINFO
        {
            public uint dwSignature;
            public uint dwStrucVersion;
            public uint dwFileVersionMS;
            public uint dwFileVersionLS;
            public uint dwProductVersionMS;
            public uint dwProductVersionLS;
            public uint dwFileFlagsMask;
            public uint dwFileFlags;
            public uint dwFileOS;
            public uint dwFileType;
            public uint dwFileSubtype;
            public uint dwFileDateMS;
            public uint dwFileDateLS;
        };

        [StructLayout(LayoutKind.Sequential)]
        public class BITMAPINFO
        {
            public int biSize;
            public int biWidth;
            public int biHeight;
            public short biPlanes;
            public short biBitCount;
            public int biCompression;
            public int biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public int biClrUsed;
            public int biClrImportant;
            public int colors;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MSG
        {
            public IntPtr hwnd;
            public uint message;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public POINT pt;
        }

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateDIBSection(IntPtr hdc, IntPtr hdr, uint colors, ref IntPtr pBits, IntPtr hFile, uint offset);

        [DllImport("gdi32.dll")]
        public static extern int SelectClipRgn(IntPtr hdc, IntPtr hrgn);

        [DllImport("gdi32.dll")]
        public static extern bool SetViewportOrgEx(IntPtr hdc, int X, int Y, out POINT lpPoint);

        [DllImport("gdi32.dll")]
        public static extern bool ScaleViewportExtEx(IntPtr hdc, int Xnum, int Xdenom, int Ynum, int Ydenom, out SIZE lpSize);

        [DllImport("gdi32.dll")]
        public static extern bool SetViewportExtEx(IntPtr hdc, int nXExtent, int nYExtent, out SIZE lpSize);
        [DllImport("gdi32.dll")]
        public static extern bool SetWindowExtEx(IntPtr hdc, int nXExtent, int nYExtent, out SIZE lpSize);

        [DllImport("gdi32.dll")]
        public static extern int SetMapMode(IntPtr hdc, int fnMapMode);

        [DllImport(USER32DLL, SetLastError = true)]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport(USER32DLL, SetLastError = true)]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, WndProcDelegate newProc);

        [DllImport(USER32DLL, SetLastError = true)]
        public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport(USER32DLL, CharSet = CharSet.Auto)]
        public static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport(USER32DLL)]
        public static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport(KERNEL32DLL, CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string modName);

        [DllImport(USER32DLL)]
        public static extern IntPtr CreateWindowEx(uint dwExStyle, string lpClassName,
            string lpWindowName, uint dwStyle, int x, int y, int nWidth, int nHeight,
            IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

        [DllImport(USER32DLL)]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth,
           int nHeight, bool bRepaint);

        [DllImport(USER32DLL)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport(USER32DLL)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SWP uFlags);

        [DllImport(USER32DLL)]
        public static extern int UpdateWindow(IntPtr hWnd);

        [DllImport(USER32DLL)]
        public extern static void InvalidateRect(IntPtr handle, RECT rect, bool erase);


        [DllImport(USER32DLL)]
        public extern static void InvalidateRect(IntPtr handle, IntPtr rect, bool erase);

        [DllImport(USER32DLL)]
        public extern static bool GetUpdateRect(IntPtr handle, out RECT rect, bool erase);

        [DllImport(USER32DLL)]
        public static extern bool IntersectRect(out RECT lprcDest, ref RECT src1, ref RECT src2);

        [DllImport(USER32DLL)]
        public extern static IntPtr SendMessage(IntPtr hWnd, Messages.WM msg, IntPtr wParam, IntPtr lParam);

        [DllImport(USER32DLL)]
        public extern static IntPtr PostMessage(IntPtr hWnd, Messages.WM msg, IntPtr wParam, IntPtr lParam);

        [DllImport(USER32DLL)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetMessage(out MSG lpMsg, IntPtr hWnd, Messages.WM wMsgFilterMin, Messages.WM wMsgFilterMax);

        [DllImport(USER32DLL)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PeekMessage(out MSG lpMsg, IntPtr hWnd, Messages.WM wMsgFilterMin, Messages.WM wMsgFilterMax, Messages.PM wRemoveMsg);

        [DllImport(USER32DLL, CharSet = CharSet.Auto)]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport(COMCTL32DLL)]
        public static extern bool ScrollDC(IntPtr hDC, int dx, int dy, IntPtr lprcScroll,
           IntPtr lprcClip, IntPtr hrgnUpdate, out RECT lprcUpdate);

        [DllImport(COMCTL32DLL)]
        public static extern bool ScrollDC(IntPtr hDC, int dx, int dy, ref RECT lprcScroll,
           ref RECT lprcClip, IntPtr hrgnUpdate, out RECT lprcUpdate);

        [DllImport(USER32DLL, CharSet = CharSet.Auto)]
        public static extern IntPtr WindowFromPoint(Point point);

        [DllImport(USER32DLL, CharSet = CharSet.Auto)]
        public static extern bool GetWindowRect(IntPtr hWnd, out Rectangle r);

        [DllImport(USER32DLL)]
        public extern static IntPtr GetDC(IntPtr hWnd);

        [DllImport(USER32DLL)]
        public extern static int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport(USER32DLL, SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport(USER32DLL)]
        public static extern IntPtr FindWindowEx(IntPtr hWndParent, IntPtr hWndChild, string lpClassName, string lpWindowName);

        [DllImport(USER32DLL, SetLastError = true)]
        public static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        [DllImport(USER32DLL, SetLastError = true)]
        public static extern int GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport(USER32DLL, SetLastError = true)]
        public static extern IntPtr SetCapture(IntPtr hWnd);

        [DllImport(USER32DLL, SetLastError = true)]
        public static extern int ReleaseCapture();

        [DllImport(USER32DLL)]
        public static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

        [DllImport(KERNEL32DLL)]
        public static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport(VERSIONDLL, SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetFileVersionInfoSize(string lptstrFilename, out int lpdwHandle);

        [DllImport(VERSIONDLL)]
        public static extern int GetFileVersionInfo(string lptstrFilename, int dwHandle, int dwLen, IntPtr lpData);

        [DllImport(VERSIONDLL)]
        public static extern int VerQueryValue(IntPtr pBlock, string lpSubBlock, out IntPtr lplpBuffer, out int puLen);

        [DllImport(USER32DLL)]
        public static extern int ToAscii(int uVirtKey,
            int uScanCode,
            byte[] lpbKeyState,
            byte[] lpChar,
            int uFlags);

        [DllImport(USER32DLL)]
        public static extern int GetKeyboardState(byte[] pbKeyState);

        [DllImport(USER32DLL)]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        [DllImport(USER32DLL)]
        public static extern int RedrawWindow(IntPtr hWnd, [In] ref RECT lprcUpdate,
           IntPtr hrgnUpdate, uint flags);

        [DllImport(USER32DLL)]
        public static extern int RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate,
           IntPtr hrgnUpdate, uint flags);

        [DllImport(USER32DLL)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport(USER32DLL)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport(USER32DLL)]
        public static extern bool ValidateRect(IntPtr hWnd, ref RECT lpRect);

        [DllImport(USER32DLL)]
        public static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport(USER32DLL)]
        public static extern IntPtr GetFocus();

        [DllImport(USER32DLL)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int num);

        [DllImport(KERNEL32DLL)]
        public static extern int GetCurrentThreadId();

        [DllImport(KERNEL32DLL)]
        public static extern void ExitProcess(int exitCode);

        [DllImport(KERNEL32DLL, SetLastError = true, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateEvent(IntPtr lpEventAttributes, [In, MarshalAs(UnmanagedType.Bool)] bool bManualReset, [In, MarshalAs(UnmanagedType.Bool)] bool bIntialState, [In, MarshalAs(UnmanagedType.BStr)] string lpName);

        [DllImport(KERNEL32DLL, SetLastError = true)]
        public static extern int WaitForSingleObject(IntPtr handle, uint wait);

        [DllImport(KERNEL32DLL, SetLastError = true, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport(KERNEL32DLL)]
        public static extern bool SetEvent(IntPtr hEvent);


        [DllImport(KERNEL32DLL)]
        public static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetDiskFreeSpaceEx(string lpDirectoryName,
                                                    out ulong lpFreeBytesAvailable,
                                                    out ulong lpTotalNumberOfBytes,
                                                    out ulong lpTotalNumberOfFreeBytes);

        [DllImport(USER32DLL)]
        public static extern IntPtr BeginPaint(IntPtr hwnd, out PAINTSTRUCT lpPaint);

        [StructLayout(LayoutKind.Sequential)]
        public struct PAINTSTRUCT
        {
            public IntPtr hdc;
            public bool fErase;
            public RECT rcPaint;
            public bool fRestore;
            public bool fIncUpdate;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] rgbReserved;
        }

        [DllImport("user32.dll")]
        public static extern bool EndPaint(IntPtr hWnd, [In] ref PAINTSTRUCT lpPaint);

        [DllImport(USER32DLL)]
        public static extern bool TranslateMessage([In] ref MSG msg);


        [DllImport(USER32DLL)]
        public static extern bool DispatchMessage([In] ref MSG msg);

        [DllImport("shlwapi.dll")]
        public static extern IntPtr StrFormatByteSize64(long qdw, StringBuilder pszBuf, uint cchBuf);


        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);



    }
}
