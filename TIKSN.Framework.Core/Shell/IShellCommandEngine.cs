using System.Reflection;

namespace TIKSN.Shell;

public interface IShellCommandEngine
{
    public void AddAssembly(Assembly assembly);

    public void AddType(Type type);

    public Task RunAsync();
}
