using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YZV25.Dal;
using YZV25.Dto;
using YZV25.Service;
using YZV25.Utils;

namespace YZV25.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PdaController : ControllerBase
    {
        private readonly ScanService _scanService;
        private readonly MesRpcClient _mesRpcClient;

        public PdaController(ScanService scanService, MesRpcClient mesRpcClient)
        {
            this._scanService = scanService;

            this._mesRpcClient = mesRpcClient;


        }
        [HttpPost("scan")]
        public async Task<ReturnDto> Scan(PdaParam pdaDto)
        {

            return await this._scanService.ScanAsync(pdaDto);

        }
        [HttpPost("force")]
        public async Task<ReturnDto> ForceSignOut(PdaForceParam pdaDto)
        {
            return await this._scanService.ForceFinish(pdaDto);

        }



        [HttpGet("query")]
        public async Task<ReturnDataDto<List<PdaQueryDto>>> QueryPdaStatus()
        {
            var result = await this._scanService.GetALLPdaRTInfo();
            return new ReturnDataDto<List<PdaQueryDto>>
            {
                code = 0,
                data = result
            }
            ;
        }
    }
}
