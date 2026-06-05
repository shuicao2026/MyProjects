using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YZV25.Dto;
using YZV25.Service;

namespace YZV25.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        LogQueryService logQueryService;
        ILogger<LogController> logger;
        public LogController(LogQueryService logQueryService,
            ILogger<LogController> logger)
        {
            this.logQueryService = logQueryService;
            this.logger = logger;
        }

        [HttpGet("GetRealTimeLog")]
        public async Task<ReturnDataDto<List<AppLogDto>>> GetRealTimeLog(int lastId)
        {

            //logger.LogInformation("查询实时日志");
            var res = await logQueryService.GetLastLogsAsync(lastId);

            return new ReturnDataDto<List<AppLogDto>>()
            {
                data = res
            };
        }

        [HttpGet("QueryLogs")]
        public async Task<ReturnDataDto<List<AppLogDto>>> QueryLogs(string text, int pageIndex, int pageSize)
        {
            var res = await logQueryService.QueryLogsAsync(text, pageIndex, pageSize);
            return new ReturnDataDto<List<AppLogDto>>()
            {
                data = res
            };




        }
    }
}
