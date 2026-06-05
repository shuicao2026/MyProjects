using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YZV25.UI;
using YZV25UI.Dto;

namespace YZV25UI.Rpc
{
    public class LogQueryRpc
    {
        private static readonly HttpClient _client = new HttpClient
        {
            BaseAddress = new Uri(GlobalState.RPC_BASEURL)
        };
        public LogQueryRpc() { }


        public async Task<ReturnDataDto<List<LogDto>>> GetRealTimeLog(int lastId)
        {

            var response = await _client.GetAsync($"api/log/GetRealTimeLog?lastId={lastId}");

          var content = await response.Content.ReadAsStringAsync();
            ReturnDataDto<List<LogDto>> returnData = JsonConvert.DeserializeObject<ReturnDataDto<List<LogDto>>>(content);
           // return JsonConvert.DeserializeObject<List<LogDto>>(content);

            return returnData;
        }



        public async Task<ReturnDataDto<List<LogDto>>> QueryLogs(string text, int pageIndex, int pageSize)
        {

            var response = await _client.GetAsync($"api/log/QueryLogs?text={text}&pageIndex={pageIndex}&pageSize={pageSize}");

            var content = await response.Content.ReadAsStringAsync();
            ReturnDataDto<List<LogDto>> returnData = JsonConvert.DeserializeObject<ReturnDataDto<List<LogDto>>>(content);
            // return JsonConvert.DeserializeObject<List<LogDto>>(content);

            return returnData;
        }

    }
}
