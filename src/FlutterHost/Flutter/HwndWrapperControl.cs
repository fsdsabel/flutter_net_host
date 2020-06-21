using System;
using System.Windows.Forms;
using FlutterHost.Interop;


namespace FlutterHost.Flutter
{
    public class HwndWrapperControl : Control
    {
        private readonly IntPtr _childWnd;

        public HwndWrapperControl(IntPtr childWnd, bool insertToWindow)
        {

            CreateControl();
            _childWnd = childWnd;
            if (insertToWindow)
            {
                InsertToWindow();
            }
        }

        public HwndWrapperControl(IntPtr childWnd) : this(childWnd, true)
        {
        }

        public void InsertToWindow()
        {
            NativeMethods.SetParent(_childWnd, Handle);
            NativeMethods.MoveWindow(_childWnd, 0, 0, Width, Height, true);
        }

        public IntPtr ChildWnd
        {
            get { return _childWnd; }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            NativeMethods.SetFocus(_childWnd);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            NativeMethods.MoveWindow(_childWnd, 0, 0, Width, Height, true);            
        }

    }
}
