using System.Reflection;

namespace TIKSN.Shell;

public interface IShellCommandEngine
{
    void AddAssembly(Assembly assembly);

    void AddType(Type type);

    Task RunAsync();
}
