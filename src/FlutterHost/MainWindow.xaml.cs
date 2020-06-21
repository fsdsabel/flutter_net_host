using FlutterHost.Flutter;
using FlutterHost.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FlutterHost
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TestPlugin _testPlugin;
        private FlutterIntegration _flutter;

        public MainWindow()
        {
            InitializeComponent();

#if DEBUG
            var type = "Debug";            
#else
            var type = "Release";
#endif

            var project = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(GetType().Assembly.Location), 
                $@"..\..\..\..\flutter_app\build\windows\x64\{type}\Runner");

            _testPlugin = new TestPlugin();
            _flutter = new FlutterIntegration(project, _testPlugin);
                        
            FlutterHost.Child = _flutter.Control;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _testPlugin.CallSomeFlutterMethod("Hi flutter!");
        }
    }

    class TestPlugin : FlutterPlugin
    {
        public override string Name => "CSharpInterop";
        protected override string ChannelName => "plugins.test.com/csharp_interop";

        [FlutterMethod("addPrefix")]
        public string AddPrefix(string prefix)
        {
            return "Hello from C#: " + prefix;
        }

        public void CallSomeFlutterMethod(string message)
        {
            CallFlutterMethod(new object[] { message });
        }
    }

}
