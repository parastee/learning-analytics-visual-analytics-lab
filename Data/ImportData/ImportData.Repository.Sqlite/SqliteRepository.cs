using System;
using System.Collections.Generic;
using System.Linq;

namespace ImportData.Repository.SQLite
{
    public class SqliteRepository : Repository
    {
        private readonly string fileName;
        private readonly IUnitOfWork externalUOW;


        protected SqliteRepository(IUnitOfWork uow, Action<IRepositoryConfig> configFunc) : base(uow, configFunc)
        {
        }

        protected override string GenerateTableScript()
        {
            var sql = $@"
CREATE TABLE IF NOT EXISTS {TableName} (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    {string.Join(",", Cols.Select(c => $"{c.Key} {c.Value.ToString().Replace("_", " ")}"))}
) ;
";
            return sql;
        }

        protected override string GenerateInsertScript()
        {
            var sql = $@"
INSERT INTO {TableName} ({string.Join(", ", Cols.Select(c => c.Key))})
VALUES(@{string.Join(", @", Cols.Select(c => c.Key))});
";
            return sql;
        }

    }
}
