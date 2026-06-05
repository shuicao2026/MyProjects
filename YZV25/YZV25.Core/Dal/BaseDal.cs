using Microsoft.Extensions.Configuration;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace YZV25.Dal

{
    public abstract class BaseDal
    {
        protected SqlSugarClient db;

        public BaseDal(IConfiguration configuration)
        {

        }

        public virtual void BeginTran()
        {
            db.BeginTran();
        }
        public virtual void CommitTran()
        {
            db.CommitTran();
        }

        public virtual void RollbackTran()
        {
            db.RollbackTran();
        }


        public SqlSugarClient GetDb()
        {
            return db;
        }

        public virtual long Count<T>(Expression<Func<T, bool>> expression)
        {

            return db.Queryable<T>().Count(expression);


        }

        public virtual T Get<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            return db.Queryable<T>().First(expression);

        }

        public virtual T Insert<T>(T entity) where T : class, new()
        {

            //db.Insertable(entity).ExecuteCommand();
            return db.Insertable(entity).ExecuteReturnEntity();
        }

        public virtual int Insert<T>(List<T> entities) where T : class, new()
        {
            return db.Insertable(entities).ExecuteCommand();
        }


        public virtual int Update<T>(T entity,  Expression<Func<T, bool>> expression) where T : class, new()
        {

            return db.Updateable(entity).Where(expression).ExecuteCommand();


        }

        public virtual int Update<T>(T entity, Expression<Func<T, object>> columns, Expression<Func<T, bool>> expression) where T : class, new()
        {

            return db.Updateable(entity).UpdateColumns(columns).Where(expression).ExecuteCommand();


        }

        public virtual int Update<T>(Expression<Func<T, T>> columns, Expression<Func<T, bool>> expression) where T : class, new()
        {
           return db.Updateable<T>().SetColumns(columns).Where(expression).ExecuteCommand();

            //return db.Updateable()(columns).Where(expression).ExecuteCommand();


        }
    }
}
