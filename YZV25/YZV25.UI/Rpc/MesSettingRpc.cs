using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YZV25UI.Dto;

namespace YZV25.UI.Rpc
{
    public class MesSettingRpc
    {
        private static readonly HttpClient _client = new HttpClient
        {
            BaseAddress = new Uri(GlobalState.RPC_BASEURL)
        };
        public async Task<ReturnDto> Update(MesSettingDto mesSettingDto)
        {
            // 序列化为JSON
            string json = System.Text.Json.JsonSerializer.Serialize(mesSettingDto);

            // 创建HttpContent
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"api/messetting/update", httpContent);

            var content = await response.Content.ReadAsStringAsync();
            ReturnDto returnData = JsonConvert.DeserializeObject<ReturnDto>(content);
            // return JsonConvert.DeserializeObject<List<LogDto>>(content);

            return returnData;

        }

        public async Task<ReturnDataDto<MesSettingDto>> GetSetting()
        {

            var response = await _client.GetAsync($"api/messetting/getsetting");

            var content = await response.Content.ReadAsStringAsync();
            ReturnDataDto<MesSettingDto> returnData = JsonConvert.DeserializeObject<ReturnDataDto<MesSettingDto>>(content);
            // return JsonConvert.DeserializeObject<List<LogDto>>(content);

            return returnData;
        }
    }
}
