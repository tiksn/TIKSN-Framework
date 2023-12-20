namespace TIKSN.Shell;

[AttributeUsage(AttributeTargets.Property)]
public sealed class ShellCommandParameterAttribute : ShellAttributeBase
{
    public ShellCommandParameterAttribute(int nameKey) : base(nameKey)
    {
    }

    public ShellCommandParameterAttribute(string nameKey) : base(nameKey)
    {
    }

    public bool Mandatory { get; set; }
    public int Position { get; set; }
}
