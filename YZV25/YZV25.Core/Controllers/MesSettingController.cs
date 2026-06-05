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
    public class MesSettingController : ControllerBase
    {

        MesSettingService mesSettingService;
        public MesSettingController(MesSettingService mesSettingService)
        {

            this.mesSettingService = mesSettingService;
        }
        [HttpPost("update")]
        public async Task<ReturnDto> UpdateMESSetting(MesSettingDto mesSettingDto)
        {
            await this.mesSettingService.Update(mesSettingDto);

            return new ReturnDto()
            {
                code = 0,
            };
        }

        [HttpGet("getsetting")]
        public async Task<ReturnDataDto<MesSettingDto>> GetSetting()
        {


            var setting = await this.mesSettingService.GetSetting();

            return new ReturnDataDto<MesSettingDto>()
            {
                code = 0,
                data = setting

            };
        }
    }
}
