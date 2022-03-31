using System;
using System.Data.Common;

namespace ImportData.Repository
{
    public interface IUnitOfWork:IDisposable
    {
        DbConnection Connection { get; }
        DbTransaction Transaction { get; }
        void BeginTrans();
        void Commit();
        void RollBack();
    }
}
