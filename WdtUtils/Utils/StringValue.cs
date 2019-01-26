using System;

namespace WdtUtils.Utils
{
    public class StringValue : Attribute
    {
        public string Value { get; }

        public StringValue(string value)
        {
            this.Value = value;
        }
    }
}
