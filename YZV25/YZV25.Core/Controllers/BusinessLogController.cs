using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YZV25.Core.Dto;
using YZV25.Core.Service;
using YZV25.Dto;
using YZV25.Service;

namespace YZV25.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessLogController : ControllerBase
    {
        private readonly BusinessLogService _businessLogService;

        public BusinessLogController(BusinessLogService businessLogService)
        {
            _businessLogService = businessLogService;
        }
        [HttpGet("QueryLogs")]
        public async Task<ReturnDataDto<List<BusinessLogDto>>> QueryLogs(string text, int pageIndex, int pageSize)
        {
            var res = await _businessLogService.QueryLogsAsync(text, pageIndex, pageSize);

            var cnt = await _businessLogService.GetCount(text);

            return new ReturnDataDto<List<BusinessLogDto>>()
            {
                Count = cnt,
                data = res
            };

        }


        [HttpGet("GetRealTimeLog")]
        public async Task<ReturnDataDto<List<BusinessLogDto>>> GetRealTimeLog(int lastId)
        {

            //logger.LogInformation("查询实时日志");
            var res = await _businessLogService.GetLastLogsAsync(lastId);

            return new ReturnDataDto<List<BusinessLogDto>>()
            {
                data = res
            };
        }
    }
}
