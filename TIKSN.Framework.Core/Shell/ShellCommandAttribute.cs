namespace TIKSN.Shell;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ShellCommandAttribute : ShellAttributeBaseAttribute
{
    public ShellCommandAttribute(string nameKey) : base(nameKey)
    {
    }
}
