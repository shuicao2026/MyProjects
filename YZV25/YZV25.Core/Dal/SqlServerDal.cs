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
    public class SqlServerDal : BaseDal
    {
        public SqlServerDal(IConfiguration configuration) : base(configuration)
        {

            this.db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = configuration.GetConnectionString("DefaultConnection"),
                DbType = SqlSugar.DbType.SqlServer,
                IsAutoCloseConnection = true
            });

        }


    }
}
