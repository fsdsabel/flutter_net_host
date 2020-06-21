import 'package:flutter/services.dart';

const MethodChannel _channel = MethodChannel('plugins.test.com/csharp_interop');


class TestPlugin {
  Function _onAlert;

  TestPlugin() {
    _channel.setMethodCallHandler(callHandler);
  }

  Future<dynamic> callHandler(dynamic arg) {    
    _onAlert(arg.arguments["message"]);
    return Future.value(null);
  }

  void setAlertHandler({Function onAlert}) {
    _onAlert = onAlert;
  }

  Future<String> getStringFromCSharp(String prefix) async {
    final Map<String, Object> args = <String, Object>{
      'prefix': prefix
    };
    return await _channel.invokeMethod<String>("addPrefix", args);
  }

}