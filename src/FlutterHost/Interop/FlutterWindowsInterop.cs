using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace FlutterHost.Interop
{
    class FlutterDesktopViewControllerRef : SafeHandle
    {
        public FlutterDesktopViewControllerRef() : base(IntPtr.Zero, true) { }

        public override bool IsInvalid => handle == IntPtr.Zero;

        protected override bool ReleaseHandle()
        {
            // TODO
            return true;
        }
    }

    abstract class FlutterSafeHandle : SafeHandle
    {
        public FlutterSafeHandle() : base(IntPtr.Zero, false)
        {
        }

        public override bool IsInvalid => handle == IntPtr.Zero;

        protected override bool ReleaseHandle()
        {
            return true;
        }
    }

    class FlutterDesktopViewRef : FlutterSafeHandle { }

    class FlutterDesktopPluginRegistrarRef : FlutterSafeHandle { }

    class FlutterDesktopMessageResponseHandle : FlutterSafeHandle { 
    
        public static implicit operator FlutterDesktopMessageResponseHandle(IntPtr handle)
        {
            return new FlutterDesktopMessageResponseHandle { handle = handle };
        }
    }
    
    class FlutterDesktopMessengerRef : FlutterSafeHandle { }

    delegate void FlutterDesktopMessageCallback(/*FlutterDesktopMessengerRef*/IntPtr messenger,
        FlutterDesktopMessage message, IntPtr userData);

    delegate void FlutterDesktopBinaryReply(byte[] data, IntPtr dataSize, IntPtr userData);

    [StructLayout(LayoutKind.Sequential)]
    struct FlutterDesktopEngineProperties
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        public string AssetsPath;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string IcuDataPath;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string AotLibraryPath;

        //[MarshalAs(UnmanagedType.LPStr, SizeParamIndex = 4)]
        //[MarshalAs(UnmanagedType.LPStr)]
        public IntPtr Switches;

        
        public IntPtr SwitchesCount;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FlutterDesktopMessage
    {
        public IntPtr StructSize;

        /// <summary>
        /// The name of the channel used for this message. 
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string Channel;
        
        /// <summary>
        /// The raw message data. 
        /// </summary>        
        public IntPtr Message;
        /// <summary>
        /// The length of |message|. 
        /// </summary>
        public IntPtr MessageSize;
        /// <summary>
        ///   The response handle. If non-null, the receiver of this message must call
        /// FlutterDesktopSendMessageResponse exactly once with this handle.
        /// </summary>
        public /*FlutterDesktopMessageResponseHandle*/IntPtr ResponseHandle;
    }

    static class FlutterWindowsInterop
    {

        public static string ProjectPath
        {
            get;private set;
        }

        public static IntPtr CreateSwitches(string[] switches)
        {
            // TODO: This leaks!!
            var result = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>() * switches.Length);

            for (int i = 0; i < switches.Length; i++)
            {
                var s = Marshal.StringToHGlobalAnsi(switches[i]);
                Marshal.WriteIntPtr(result, i * Marshal.SizeOf<IntPtr>(), s);
            }
            return result;
        }

        [DllImport("kernel32")]
        static extern IntPtr AddDllDirectory([MarshalAs(UnmanagedType.LPWStr)] string directory);
        const uint LOAD_LIBRARY_SEARCH_DEFAULT_DIRS = 0x00001000;

        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool SetDefaultDllDirectories(uint DirectoryFlags);

        [DllImport("flutter_windows")]
        public static extern FlutterDesktopViewControllerRef FlutterDesktopCreateViewController(int width, int height,
            FlutterDesktopEngineProperties engineProperties);

        [DllImport("flutter_windows")]
        public static extern FlutterDesktopViewRef FlutterDesktopGetView(FlutterDesktopViewControllerRef controller);

        [DllImport("flutter_windows")]
        public static extern IntPtr FlutterDesktopViewGetHWND(FlutterDesktopViewRef view);

        [DllImport("flutter_windows")]
        public static extern ulong FlutterDesktopProcessMessages(FlutterDesktopViewControllerRef controller);

        [DllImport("flutter_windows")]
        public static extern FlutterDesktopPluginRegistrarRef FlutterDesktopGetPluginRegistrar(FlutterDesktopViewControllerRef controller,
            [MarshalAs(UnmanagedType.LPStr)] string pluginName);

        [DllImport("flutter_windows")]
        public static extern FlutterDesktopMessengerRef FlutterDesktopRegistrarGetMessenger(FlutterDesktopPluginRegistrarRef registrar);

        [DllImport("flutter_windows")]
        public static extern void FlutterDesktopMessengerSetCallback(FlutterDesktopMessengerRef messenger,
                    [MarshalAs(UnmanagedType.LPStr)]string channel, FlutterDesktopMessageCallback callback, IntPtr user_data);

        [DllImport("flutter_windows")]
        public static extern bool FlutterDesktopMessengerSend(FlutterDesktopMessengerRef messenger,
                    [MarshalAs(UnmanagedType.LPStr)] string channel, byte[] message, IntPtr messageSize);

        [DllImport("flutter_windows")]
        public static extern bool FlutterDesktopMessengerSendWithReply(
                FlutterDesktopMessengerRef messenger,
                [MarshalAs(UnmanagedType.LPStr)] string channel,
                byte[] message,
                IntPtr message_size,
                FlutterDesktopBinaryReply reply,
                IntPtr user_data);

        [DllImport("flutter_windows")]
        public static extern void FlutterDesktopMessengerSendResponse(
                FlutterDesktopMessengerRef messenger,
                FlutterDesktopMessageResponseHandle handle,
                byte[] data,
                IntPtr data_length);

        [DllImport("flutter_windows")]
        public static extern void FlutterDesktopResyncOutputStreams();

        internal static void SetProjectPath(string projectPath)
        {
            SetDefaultDllDirectories(LOAD_LIBRARY_SEARCH_DEFAULT_DIRS);
            AddDllDirectory(projectPath);
            ProjectPath = projectPath;
            
        }
    }
}
