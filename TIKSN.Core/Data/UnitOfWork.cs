using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace TIKSN.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext dbContext;

        public UnitOfWork(DbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Task CompleteAsync()
        {
            return dbContext.SaveChangesAsync();
        }
    }
}