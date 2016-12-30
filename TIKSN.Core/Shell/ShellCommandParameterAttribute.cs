using System;

namespace TIKSN.Shell
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ShellCommandParameterAttribute : Attribute
    {
        public ShellCommandParameterAttribute(string parameterNameKey)
        {
            ParameterNameKey = new Guid(parameterNameKey);
        }

        public Guid ParameterNameKey { get; private set; }

        public bool Mandatory { get; set; }

        public int Position { get; set; }
    }
}
