using System;
using System.Reflection;
using System.Threading.Tasks;

namespace TIKSN.Shell
{
    public interface IShellCommandEngine
    {
        void AddType(Type type);

        void AddAssembly(Assembly assembly);

        Task RunAsync();
    }
}
