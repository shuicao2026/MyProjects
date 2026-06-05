using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YZV25.Core.Dto;
using YZV25.Dto;
using YZV25.Service;

namespace YZV25.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        DeviceService deviceService;
        public DeviceController(DeviceService deviceService)
        {

            this.deviceService = deviceService;
        }
        [HttpGet("getall")]
        public async Task<ReturnDataDto<List<DeviceDto>>> GetDevices()
        {
            var devices = await this.deviceService.GetAllDevices();

            return new ReturnDataDto<List<DeviceDto>>()
            {
                code = 0,

                data = devices
            };


        }

    }
}
