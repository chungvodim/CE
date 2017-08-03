using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGP.Utils.Repository.EntityFramework
{
    public class Transaction : ITransaction
    {
        private System.Data.Entity.DbContextTransaction _dbContextTransaction;

        protected internal Transaction(System.Data.Entity.DbContextTransaction dbContextTransaction)
        {
            // TODO: Complete member initialization
            this._dbContextTransaction = dbContextTransaction;
        }

        #region ITransaction Members

        public void Commit()
        {
            _dbContextTransaction.Commit();
        }

        public void Rollback()
        {
            _dbContextTransaction.Rollback();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _dbContextTransaction.Dispose();
        }

        #endregion
    }
}
