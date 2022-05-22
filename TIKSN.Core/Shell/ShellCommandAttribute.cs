using System;

namespace TIKSN.Shell
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ShellCommandAttribute : ShellAttributeBase
    {
        public ShellCommandAttribute(int nameKey) : base(nameKey)
        {
        }

        public ShellCommandAttribute(string nameKey) : base(nameKey)
        {
        }
    }
}
