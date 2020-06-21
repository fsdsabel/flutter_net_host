using System;

namespace FlutterHost.Flutter
{
    [AttributeUsage(AttributeTargets.Method)]
    public class FlutterMethodAttribute : Attribute
    {
        public FlutterMethodAttribute(string methodName)
        {
            MethodName = methodName;
        }

        public string MethodName { get; }
    }
}
