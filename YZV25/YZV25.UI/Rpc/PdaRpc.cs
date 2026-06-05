using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YZV25.Dto;
using YZV25UI.Dto;

namespace YZV25.UI.Rpc
{
    public class PdaRpc
    {
        private static readonly HttpClient _client = new HttpClient
        {
            BaseAddress = new Uri(GlobalState.RPC_BASEURL)
        };
        public PdaRpc() { }


        public async Task<ReturnDataDto<List<PdaQueryDto>>> GetRealTimePda()
        {

            var response = await _client.GetAsync($"api/pda/query");

            var content = await response.Content.ReadAsStringAsync();
            ReturnDataDto<List<PdaQueryDto>> returnData = JsonConvert.DeserializeObject<ReturnDataDto<List<PdaQueryDto>>>(content);
            // return JsonConvert.DeserializeObject<List<LogDto>>(content);

            return returnData;
        }
    }
}
