using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YZV25UI.Dto;

namespace YZV25.UI.Rpc
{
    public  class DeviceRpc
    {
        private static readonly HttpClient _client = new HttpClient
        {
            BaseAddress = new Uri(GlobalState.RPC_BASEURL)
        };

        public async Task<ReturnDataDto<List<DeviceDto>>> GetAllDevices()
        {
            var response = await _client.GetAsync($"api/device/getall");

            var content = await response.Content.ReadAsStringAsync();
            ReturnDataDto<List<DeviceDto>> returnData = JsonConvert.DeserializeObject<ReturnDataDto<List<DeviceDto>>>(content);

            return returnData;

        }

    }
}
