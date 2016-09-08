using System.Threading.Tasks;

namespace TIKSN.Data
{
    public interface IUnitOfWork
    {
        Task CompleteAsync();
    }
}