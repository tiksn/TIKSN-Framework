using System;

namespace TIKSN.Shell
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ShellCommandAttribute : Attribute
    {
        public ShellCommandAttribute(string commandNameKey)
        {
            CommandNameKey = new Guid(commandNameKey);
        }

        public Guid CommandNameKey { get; private set; }
    }
}
