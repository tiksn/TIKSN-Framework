using System;
using System.Reflection;
using System.Threading.Tasks;

namespace TIKSN.Shell
{
    public interface IShellCommandEngine
    {
        void AddAssembly(Assembly assembly);

        void AddType(Type type);

        Task RunAsync();
    }
}
