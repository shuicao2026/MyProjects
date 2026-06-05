using YZV25.Dal;
using YZV25.Dto;
using YZV25.Entity;

namespace YZV25.Service
{
    public class LogQueryService
    {
        SqlServerDal _sqlServerDal;
        public LogQueryService(SqlServerDal sqlServerDal)
        {

            this._sqlServerDal = sqlServerDal;
        }


        public async Task<List<AppLogDto>> GetLastLogsAsync(int lastId)
        {
            // 这里可以添加实际的数据库查询逻辑，使用SqlSugar或其他ORM工具
            // 示例返回一个空列表

            List<AppLog> logs = null;
            if (lastId <= 0)
            {
                logs = await this._sqlServerDal.GetDb().Queryable<AppLog>().OrderByDescending(a => a.TimeStamp).Take(10).ToListAsync();

            }
            else
            {
                logs = await this._sqlServerDal.GetDb().Queryable<AppLog>().Where(log => log.Id > lastId).ToListAsync();
            }




            var result = logs.Select(log => new AppLogDto()
            {
                Id = log.Id,
                Message = log.Message,
     
                Exception = log.Exception,
                Level = log.Level,
 
                TimeStamp = log.TimeStamp


            }).ToList();



            return result;
        }


        public async Task<List<AppLogDto>> QueryLogsAsync(string text, int pageIndex, int pageSize)
        {

            var logs = await this._sqlServerDal.GetDb()
                .Queryable<AppLog>()
                .Where(log => log.Message.Contains(text) || log.Exception.Contains(text))
                .OrderByDescending(a => a.TimeStamp)
                .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            var result = logs.Select(log => new AppLogDto()
            {
                Id = log.Id,
                Message = log.Message,
    
                Exception = log.Exception,
                Level = log.Level,
         
                TimeStamp = log.TimeStamp
            }).ToList();
            return result;

        }
    }
}
