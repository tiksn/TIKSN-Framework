using System.Threading.Tasks;

namespace TIKSN.Shell
{
	public interface IShellCommand
	{
		Task ExecuteAsync();
	}
}