using System;
using System.Data.Common;
using System.Data.SQLite;

using ImportData.Repository;

namespace ImportData.Repository.SQLite
{
    public class SQLiteUnitOfwork :  IUnitOfWork
    {
        private readonly string fileName;

        public SQLiteUnitOfwork(string fileName)
        {
            this.fileName = fileName;
            CreateUOW();
        }

        public DbConnection Connection { get; private set; }
        public DbTransaction Transaction { get; private set; }

        private void CreateUOW()
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new InvalidOperationException("the file name is not valid");

            Connection = new SQLiteConnection($"Data Source={fileName}");

            Connection.Open();

        }

        void IUnitOfWork.BeginTrans()
        {
            if (!(Transaction is null))
                throw new Exception("Transaction was created");
            Transaction = Connection.BeginTransaction();
        }

        void IUnitOfWork.Commit()
        {
            if (Transaction is null)
                throw new NullReferenceException("Transaction is Null");
            Transaction.Commit();
            DisposeTransaction();
        }

        void IUnitOfWork.RollBack()
        {
            if (Transaction is null)
                throw new NullReferenceException("Transaction is Null");
            Transaction.Rollback();
            DisposeTransaction();
        }

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    DisposeTransaction();
                    Connection?.Dispose();
                    Connection = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        private void DisposeTransaction()
        {
            Transaction?.Dispose();
            Transaction = null;
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Repository() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
    }
}
