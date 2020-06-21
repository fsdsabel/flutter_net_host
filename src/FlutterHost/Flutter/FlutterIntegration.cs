using FlutterHost.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System.Windows.Threading;

namespace FlutterHost.Flutter
{

    class FlutterIntegration
    {
        private FlutterDesktopViewControllerRef _controller;
        private Lazy<HwndWrapperControl> _control;
        private DispatcherTimer _runLoopTimer;
        private List<FlutterPlugin> _flutterPlugins = new List<FlutterPlugin>();

        public FlutterIntegration(string projectPath, params FlutterPlugin[] plugins)
        {


            try
            {
                FlutterWindowsInterop.SetProjectPath(projectPath);
                var props = new FlutterDesktopEngineProperties
                {
                    AssetsPath = Path.Combine(FlutterWindowsInterop.ProjectPath, @"data\flutter_assets"),
                    IcuDataPath = Path.Combine(FlutterWindowsInterop.ProjectPath, @"data\icudtl.dat"),
                    AotLibraryPath = Path.Combine(FlutterWindowsInterop.ProjectPath, @"data\app.so"),
                    //Switches = FlutterWindowsInterop.CreateSwitches(new[] { "--observatory-port=49494" }),
                    //SwitchesCount = new IntPtr(1)
                };

#if DEBUG
                var debugSwitches = new[] {
                    "--observatory-port=49494",
                    "--disable-service-auth-codes"
                };
                props.Switches = FlutterWindowsInterop.CreateSwitches(debugSwitches);
                props.SwitchesCount = new IntPtr(debugSwitches.Length);
                NativeMethods.AllocConsole();
                FlutterWindowsInterop.FlutterDesktopResyncOutputStreams();
#endif   

                _controller = FlutterWindowsInterop.FlutterDesktopCreateViewController(1024, 768, props);
                RegisterPlugins(_controller, plugins);

                _control = new Lazy<HwndWrapperControl>(() =>
                {
                    var view = FlutterWindowsInterop.FlutterDesktopGetView(_controller);
                    return new HwndWrapperControl(FlutterWindowsInterop.FlutterDesktopViewGetHWND(view));
                });

                ulong next_flutter_event_time = (ulong)DateTime.Now.Ticks;
                CompositionTarget.Rendering += (s, e) =>
                {
                    /*taken from run_loop.cpp
                     * 
                     * bool processedEvents = false;
                    while(NativeMethods.PeekMessage(out var msg, Control.ChildWnd, (Messages.WM)0, (Messages.WM)0, Messages.PM.REMOVE))
                    {
                        processedEvents = true;
                        NativeMethods.TranslateMessage(ref msg);
                        NativeMethods.DispatchMessage(ref msg);
                        next_flutter_event_time = Math.Min(next_flutter_event_time , 
                            FlutterWindowsInterop.FlutterDesktopProcessMessages(_controller));
                    }
                    if(!processedEvents)
                    {
                        next_flutter_event_time = Math.Min(next_flutter_event_time,
                            FlutterWindowsInterop.FlutterDesktopProcessMessages(_controller));
                    }*/
                    //NativeMethods.PostMessage(Control.ChildWnd, 0, IntPtr.Zero, IntPtr.Zero);
                    FlutterWindowsInterop.FlutterDesktopProcessMessages(_controller);

                };

                /*_runLoopTimer = new DispatcherTimer(DispatcherPriority.Render);
                _runLoopTimer.Interval = 
                
                _runLoopTimer.Tick += (s, e) =>
                  {
                      
                  };*/
                //_runLoopTimer.Start();
            }
            catch(Exception ex)
            {

            }
        }

        private void RegisterPlugins(FlutterDesktopViewControllerRef controller, FlutterPlugin[] plugins)
        {
            foreach (var plugin in plugins)
            {
                var registrar = FlutterWindowsInterop.FlutterDesktopGetPluginRegistrar(controller, plugin.Name);
                var messenger = FlutterWindowsInterop.FlutterDesktopRegistrarGetMessenger(registrar);
                plugin.SetMessenger(messenger);
                _flutterPlugins.Add(plugin);
            }
        }

        public HwndWrapperControl Control => _control.Value;
        
    }
}
