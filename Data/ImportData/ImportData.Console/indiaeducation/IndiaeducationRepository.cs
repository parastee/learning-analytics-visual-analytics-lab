using ImportData.Repository.SQLite;
using System;

namespace ImportData.Repository.Indiaeducation
{
    public class IndiaeducationRepository: SqliteRepository
    {
        public IndiaeducationRepository(IUnitOfWork uow) :
            base(uow, config => config.SetTableName("Indiaeducation")
                .AddCol("UniId", DataType.TEXT_NOT_NUll_UNIQUE)
                .AddCol("UniName", DataType.TEXT_NOT_NUll_UNIQUE)
                .AddCol("Rank", DataType.TEXT_NUll)
                .AddCol("Acronym", DataType.TEXT_NUll)
                .AddCol("Founded", DataType.TEXT_NUll))
        { }


        public void InsertUni(object obj)
        {
            Insert(obj);
        }
    }
   
}
