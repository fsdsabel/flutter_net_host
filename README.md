# Prototype Embedding Flutter in .NET 

This is a prototype to proof embedding flutter to .NET. This is far from production ready but proofs that it's possible to do.

## Setup

- You need to setup flutter SDK for Windows Desktop development
- Visual Studio 2019 16.6 or higher
- Visual Studio Code with flutter plugin (Attaching Flutter debugger only works with this)


## Building

1. Build flutter app in debug mode with
```cmd
cd src\flutter_app
flutter run
```
2. Terminate app
3. Build and run `src\FlutterHost.sln` with Debug configuration
4. Open folder `src\flutter_app` in Visual Studio Code and start debugging configuration `Flutter: Attach to Device` (hit `F5`)
5. You should be able to edit Dart sources with automatic reloading

## Takeaways

1. It's possible to embed Flutter to .NET (Core and full)
2. Live editing flutter code is possible
3. Exchanging simple method calls between both worlds is possible (Click WPF-Button in WPF and +-Button in Flutter). More work would be needed for more complex scenarios.
4. Development workflow is a bit cumbersome. It could be much better if `flutter build windows` would build the .NET project. This might be possible by tinkering with the `src\flutter_app\windows` directory.
