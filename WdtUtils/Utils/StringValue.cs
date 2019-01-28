using System;

namespace WdtUtils.Utils
{
    public class StringValue : Attribute
    {
        public StringValue(string value)
        {
            this.Value = value;
        }

        public string Value { get; }
    }
}
