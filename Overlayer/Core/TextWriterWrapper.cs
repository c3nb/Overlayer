using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace Overlayer.Core
{
    public class TextWriterWrapper : TextWriter
    {
        private readonly TextWriter baseWriter;
        public event Action<object> OnWrite = delegate { };
        public event Action<object, object> OnWrite2 = delegate { };
        public event Action<object, object, object> OnWrite3 = delegate { };
        public event Action<object, object, object, object> OnWrite4 = delegate { };
        public event Action<object> OnWriteLine = delegate { };
        public event Action<object, object> OnWriteLine2 = delegate { };
        public event Action<object, object, object> OnWriteLine3 = delegate { };
        public event Action<object, object, object, object> OnWriteLine4 = delegate { };
        public TextWriter BaseWriter => baseWriter;
        public TextWriterWrapper(TextWriter baseWriter) => this.baseWriter = baseWriter;
        public override IFormatProvider FormatProvider => baseWriter.FormatProvider;
        public override Encoding Encoding => baseWriter.Encoding;
        public override string NewLine { get => baseWriter.NewLine; set => baseWriter.NewLine = value; }
        public override void Close() => baseWriter.Close();
        public override ObjRef CreateObjRef(Type requestedType) => baseWriter.CreateObjRef(requestedType);
        public override bool Equals(object obj) => baseWriter.Equals(obj);
        public override void Flush() => baseWriter.Flush();
        public override Task FlushAsync() => baseWriter.FlushAsync();
        public override int GetHashCode() => baseWriter.GetHashCode();
        public override object InitializeLifetimeService() => baseWriter.InitializeLifetimeService();
        public override string ToString() => baseWriter.ToString();
        public override void Write(char value)
        {
            OnWrite(value);
            baseWriter.Write(value);
        }
        public override void Write(char[] buffer)
        {
            OnWrite(buffer);
            baseWriter.Write(buffer);
        }
        public override void Write(char[] buffer, int index, int count)
        {
            OnWrite3(buffer, index, count);
            baseWriter.Write(buffer, index, count);
        }
        public override void Write(bool value)
        {
            OnWrite(value);
            baseWriter.Write(value);
        }
        public override void Write(int value)
        {
            OnWrite(value);
            baseWriter.Write(value);
        }
        public override void Write(uint value)
        {
            OnWrite(value);
            baseWriter.Write(value);
        }
        public override void Write(long value)
        {
            OnWrite(value);
            baseWriter.Write(value);
        }
        public override void Write(ulong value)
        {
            OnWrite(value);
            baseWriter.Write(value);
        }
        public override void Write(float value)
        {
            OnWrite(value);
            baseWriter.Write(value);
        }
        public override void Write(double value)
        {
            OnWrite(value);
            baseWriter.Write(value);
        }
        public override void Write(decimal value)
        {
            OnWrite(value);
            baseWriter.Write(value);
        }
        public override void Write(string value)
        {
            OnWrite(value);
            baseWriter.Write(value);
        }
        public override void Write(object value)
        {
            OnWrite(value);
            baseWriter.Write(value);
        }
        public override void Write(string format, object arg0)
        {
            OnWrite2(format, arg0);
            baseWriter.Write(format, arg0);
        }
        public override void Write(string format, object arg0, object arg1)
        {
            OnWrite3(format, arg0, arg1);
            baseWriter.Write(format, arg0, arg1);
        }
        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            OnWrite4(format, arg0, arg1, arg2);
            baseWriter.Write(format, arg0, arg1, arg2);
        }
        public override void Write(string format, params object[] arg)
        {
            OnWrite2(format, arg);
            baseWriter.Write(format, arg);
        }
        public override Task WriteAsync(char value)
        {
            OnWrite(value);
            return baseWriter.WriteAsync(value);
        }
        public override Task WriteAsync(string value)
        {
            OnWrite(value);
            return baseWriter.WriteAsync(value);
        }
        public override Task WriteAsync(char[] buffer, int index, int count)
        {
            OnWrite3(buffer, index, count);
            return baseWriter.WriteAsync(buffer, index, count);
        }
        public override void WriteLine()
        {
            OnWriteLine("");
            baseWriter.WriteLine();
        }
        public override void WriteLine(char value)
        {
            OnWriteLine(value);
            baseWriter.WriteLine(value);
        }
        public override void WriteLine(char[] buffer)
        {
            OnWriteLine(buffer);
            baseWriter.WriteLine(buffer);
        }
        public override void WriteLine(char[] buffer, int index, int count)
        {
            OnWriteLine3(buffer, index, count);
            baseWriter.WriteLine(buffer, index, count);
        }
        public override void WriteLine(bool value)
        {
            OnWriteLine(value);
            baseWriter.WriteLine(value);
        }
        public override void WriteLine(int value)
        {
            OnWriteLine(value);
            baseWriter.WriteLine(value);
        }
        public override void WriteLine(uint value)
        {
            OnWriteLine(value);
            baseWriter.WriteLine(value);
        }
        public override void WriteLine(long value)
        {
            OnWriteLine(value);
            baseWriter.WriteLine(value);
        }
        public override void WriteLine(ulong value)
        {
            OnWriteLine(value);
            baseWriter.WriteLine(value);
        }
        public override void WriteLine(float value)
        {
            OnWriteLine(value);
            baseWriter.WriteLine(value);
        }
        public override void WriteLine(double value)
        {
            OnWriteLine(value);
            baseWriter.WriteLine(value);
        }
        public override void WriteLine(decimal value)
        {
            OnWriteLine(value);
            baseWriter.WriteLine(value);
        }
        public override void WriteLine(string value)
        {
            OnWriteLine(value);
            baseWriter.WriteLine(value);
        }
        public override void WriteLine(object value)
        {
            OnWriteLine(value);
            baseWriter.WriteLine(value);
        }
        public override void WriteLine(string format, object arg0)
        {
            OnWriteLine2(format, arg0);
            baseWriter.WriteLine(format, arg0);
        }
        public override void WriteLine(string format, object arg0, object arg1)
        {
            OnWriteLine3(format, arg0, arg1);
            baseWriter.WriteLine(format, arg0, arg1);
        }
        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            OnWriteLine4(format, arg0, arg1, arg2);
            baseWriter.WriteLine(format, arg0, arg1, arg2);
        }
        public override void WriteLine(string format, params object[] arg)
        {
            OnWriteLine2(format, arg);
            baseWriter.WriteLine(format, arg);
        }
        public override Task WriteLineAsync(char value)
        {
            OnWriteLine(value);
            return baseWriter.WriteLineAsync(value);
        }
        public override Task WriteLineAsync(string value)
        {
            OnWriteLine(value);
            return baseWriter.WriteLineAsync(value);
        }
        public override Task WriteLineAsync(char[] buffer, int index, int count)
        {
            OnWriteLine3(buffer, index, count);
            return baseWriter.WriteLineAsync(buffer, index, count);
        }
        public override Task WriteLineAsync()
        {
            OnWriteLine("");
            return baseWriter.WriteLineAsync();
        }
        protected override void Dispose(bool disposing) => baseWriter.Dispose();
    }
}
