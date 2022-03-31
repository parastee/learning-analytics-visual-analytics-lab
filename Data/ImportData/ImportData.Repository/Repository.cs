using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;

namespace ImportData.Repository
{
    public abstract  class Repository : IRepositoryConfig
    {
        
        private readonly IUnitOfWork externalUOW;

        protected string TableName { get; private set; }
        protected Dictionary<string, DataType> Cols { get; private set; } = new Dictionary<string, DataType>();

        public DbConnection Connection => externalUOW?.Connection ;
        public DbTransaction Transaction => externalUOW?.Transaction ;

        private Repository(Action<IRepositoryConfig> configFunc)
        {
            config = configFunc;
            TableName = this.GetType().Name.Replace(typeof(Repository).Name, "");
        }

        protected Repository(IUnitOfWork uow, Action<IRepositoryConfig> configFunc) : this(configFunc)
        {
            this.externalUOW = uow;

            config(this);
            CreateTable();
        }

        private string tableScript;
        protected virtual void CreateTable()
        {
            Connection.Execute(tableScript??(tableScript=GenerateTableScript()), Transaction);
        }

        private string insertScript;
        protected virtual void Insert(dynamic obj)
        {
            Connection.Execute(insertScript ?? (insertScript = GenerateInsertScript()),param: (object)obj,transaction: Transaction);
        }

        protected abstract string GenerateTableScript();

        protected abstract string GenerateInsertScript();


        #region IRepositoryConfig Support

        Action<IRepositoryConfig> config { get; }

        public virtual IRepositoryConfig AddCol(string name, DataType Type)
        {
            Cols.Add(name, Type);
            return this;
        }
        public virtual IRepositoryConfig TryAddCol(string name, DataType Type)
        {
            if(!Cols.ContainsKey(name))
                AddCol(name, Type);
            return this;
        }


        public virtual IRepositoryConfig SetTableName(string name)
        {
            TableName = name;
            return this;
        }

        #endregion

        
    }



}
