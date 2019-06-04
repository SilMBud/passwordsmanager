using System;

namespace PAWSwords.Core.Master
{
    public class Key : IDisposable
    {
        public Key(byte[] value)
        {
            Value = value;
        }

        public byte[] Value { get; private set; }

        public void Dispose()
        {
            Helpers.ClearData(Value);
            Value = null;
        }
    }
}
