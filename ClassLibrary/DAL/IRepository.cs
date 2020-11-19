using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer.DAL
{
    public interface IRepository<TEntity> where TEntity : class
    {
        void Delete(TEntity entityToDelete);
        void Delete(object id);
        TEntity GetByID(object id);
        void Insert(TEntity entity);
        void Update(TEntity entityToUpdate);
        void Commit();
    }
}
