namespace TIKSN.Shell;

[AttributeUsage(AttributeTargets.Property)]
public sealed class ShellCommandParameterAttribute : ShellAttributeBaseAttribute
{
    public ShellCommandParameterAttribute(string nameKey) : base(nameKey)
    {
    }

    public bool Mandatory { get; set; }
    public int Position { get; set; }
}
