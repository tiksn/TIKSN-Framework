using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace TIKSN.Data
{
    public class EntityUnitOfWork : IUnitOfWork
    {
        private readonly DbContext dbContext;

        public EntityUnitOfWork(DbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Task CompleteAsync()
        {
            return dbContext.SaveChangesAsync();
        }
    }
}