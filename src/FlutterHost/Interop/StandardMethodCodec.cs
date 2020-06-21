using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace FlutterHost.Interop
{
    public sealed class MethodCall
    {
        public MethodCall(string method, object arguments)
        {
            Debug.Assert(method != null, "Parameter method must not be null.");
            Method = method;
            Arguments = arguments;
        }

        public string Method { get; }

        public object Arguments { get; }

        public T CastedArguments<T>()
        {
            return ((T)(Arguments));
        }
                
        public T Argument<T>(string key)
        {
            if ((this.Arguments == null))
            {
                return default;
            }
            else if ((this.Arguments is IDictionary))
            {
                return ((T)(((IDictionary)(this.Arguments))[key]));
            }
            /*else if ((this._arguments is JSONObject))
            {
                return ((T)(((JSONObject)(this.arguments)).opt(key)));
            }*/
            else
            {
                throw new InvalidCastException();
            }

        }

        public bool HasArgument(string key)
        {
            if ((this.Arguments == null))
            {
                return false;
            }
            else if ((this.Arguments is IDictionary))
            {
                return ((IDictionary)(this.Arguments)).Contains(key);
            }
            /*else if ((this.arguments is JSONObject))
            {
                return ((JSONObject)(this.arguments)).has(key);
            }*/
            else
            {
                throw new InvalidCastException();
            }

        }
    }

    public interface IMethodCodec
    {

        byte[] EncodeMethodCall(MethodCall methodCall);

        MethodCall DecodeMethodCall(byte[] methodCall);

        byte[] EncodeSuccessEnvelope(object result);

        byte[] EncodeErrorEnvelope(string errorCode, string errorMessage, object errorDetails);

        object DecodeEnvelope(byte[] envelope);
    }


    public sealed class StandardMethodCodec : IMethodCodec
    {

        public static StandardMethodCodec Instance = new StandardMethodCodec(StandardMessageCodec.Instance);

        private StandardMessageCodec _messageCodec;

        public StandardMethodCodec(StandardMessageCodec messageCodec)
        {
            _messageCodec = messageCodec;
        }

        public byte[] EncodeMethodCall(MethodCall methodCall)
        {
            var stream = new MemoryStream();
            _messageCodec.WriteValue(stream, methodCall.Method);
            _messageCodec.WriteValue(stream, methodCall.Arguments);
            
            return stream.ToArray();
        }

        public MethodCall DecodeMethodCall(byte[] methodCall)
        {
            //methodCall.order(ByteOrder.nativeOrder());
            using (var reader = new BinaryReader(new MemoryStream(methodCall)))
            {
                object method = _messageCodec.ReadValue(reader);
                object arguments = _messageCodec.ReadValue(reader);
                if (method is string && reader.BaseStream.Position == reader.BaseStream.Length)
                {
                    return new MethodCall(((string)(method)), arguments);
                }

                throw new ArgumentException("Method call corrupted");
            }
        }

        public byte[] EncodeSuccessEnvelope(object result)
        {
            var stream = new MemoryStream();
            stream.WriteByte(0);
            _messageCodec.WriteValue(stream, result);
            
            return stream.ToArray();
        }

        public byte[] EncodeErrorEnvelope(string errorCode, string errorMessage, object errorDetails)
        {
            var stream = new MemoryStream();
            stream.WriteByte(1);
            _messageCodec.WriteValue(stream, errorCode);
            _messageCodec.WriteValue(stream, errorMessage);
            if ((errorDetails is Exception ex))
            {
                _messageCodec.WriteValue(stream, ex.StackTrace);
            }
            else
            {
                _messageCodec.WriteValue(stream, errorDetails);
            }

            
            return stream.ToArray();
        }

        public object DecodeEnvelope(byte[] envelope)
        {
            // envelope.order(ByteOrder.nativeOrder());
            using (var reader = new BinaryReader(new MemoryStream(envelope)))
            {
                byte flag = reader.ReadByte();
                if (flag > 1)
                {
                    throw new ArgumentException("Envelope corrupted");
                }
                if (flag == 0)
                {
                    object result = _messageCodec.ReadValue(reader);
                    if (reader.BaseStream.Position == reader.BaseStream.Length)
                    {
                        return result;
                    }

                    //  Falls through intentionally.
                }
                object code = _messageCodec.ReadValue(reader);
                object message = _messageCodec.ReadValue(reader);
                object details = _messageCodec.ReadValue(reader);

                if (code is string
                      && (message == null || message is string)
                      && reader.BaseStream.Position >= reader.BaseStream.Length)
                {
                    throw new FlutterException((string)code, (string)message, details);
                }
                throw new ArgumentException("Envelope corrupted");
            }
        }

    }

    [Serializable]
    internal class FlutterException : Exception
    {
        private string code;
        private string message;
        private object details;

        public FlutterException()
        {
        }

        public FlutterException(string message) : base(message)
        {
        }

        public FlutterException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public FlutterException(string code, string message, object details)
        {
            this.code = code;
            this.message = message;
            this.details = details;
        }

        protected FlutterException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
