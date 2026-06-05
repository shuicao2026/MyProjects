using Dm;
using Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Ocsp;
using System.Dynamic;
using System.Net;
using System.Reflection;
using System.Text;
using YZV25.Dto;
using YZV25.Entity;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace YZV25.Utils
{
    public class MesRpcClient
    {
        private readonly static Dictionary<string, int> dicDevices = new Dictionary<string, int>()
        {
            {"CNC",40 },
            {"FSW",20 },
            {"CMT",10 }
        };

        string hostname;
        ILogger<MesRpcClient> logger;
        IConfiguration configuration;
        public MesRpcClient(string hostname, IConfiguration configuration, ILogger<MesRpcClient> logger)
        {
            this.hostname = hostname;
            this.logger = logger;
            this.configuration = configuration;
        }
        #region  处理普通类型
        public string ParseActualValueFromObject<T>(T data) where T : class, new()
        {
            JArray jsonArray = new JArray();

            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties(
                BindingFlags.Public | BindingFlags.Instance
            );

            foreach (PropertyInfo property in properties)
            {
                var jobj = new JObject();
                jobj["TagName"] = property.Name;
                jobj["Val"] = property.GetValue(data)?.ToString() ?? "";
                jsonArray.Add(jobj);
            }

            return jsonArray.ToString();
        }
        #endregion

        #region 处理 JObject
        public string ParseActualValueFromJObject(JObject jObject)
        {
            JArray jsonArray = new JArray();

            if (jObject != null)
            {
                foreach (var property in jObject.Properties())
                {
                    if (property.Name.StartsWith("__"))
                    {
                        continue;
                    }
                    var jobj = new JObject();
                    jobj["TagName"] = property.Name;
                    jobj["Val"] = property.Value?.ToString() ?? "";
                    jsonArray.Add(jobj);
                    //Console.WriteLine("TagName: " + property.Name + ", Val: " + property.Value);
                }
            }

            return jsonArray.ToString();
        }
        #endregion

        #region  处理 ExpandoObject

        public string ParseActualValueFromExpandoObject(ExpandoObject expandoObject)
        {
            JArray jsonArray = new JArray();

            if (expandoObject != null)
            {
                var dictionary = (IDictionary<string, object>)expandoObject;
                foreach (var kvp in dictionary)
                {
                    var jobj = new JObject();
                    jobj["TagName"] = kvp.Key;
                    jobj["Val"] = kvp.Value?.ToString() ?? "";
                    jsonArray.Add(jobj);
                }
            }

            return jsonArray.ToString();
        }
        #endregion



        public string ParseActualValue<T>(T data) where T : class, new()
        {
            if (data is JObject jObject)
            {
                return ParseActualValueFromJObject(jObject);
            }
            else if (data is ExpandoObject expandoObject)
            {
                return ParseActualValueFromExpandoObject(expandoObject);
            }
            else
            {
                return ParseActualValueFromObject(data);
            }
        }


        private bool IsEnable()
        {
            return Convert.ToBoolean(this.configuration["MES:Enable"]);

        }
        private bool IsBlock(string deviceName, string deviceType)
        {


            var myArray = this.configuration.GetSection("MES:Block")?.Get<string[]>();

            return !IsEnable() || (myArray!=null&&myArray.Contains(deviceName));

        }

        /// <summary>
        /// 封装进站
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="barCode"></param>
        /// <param name="device"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private MesDto<Value> GenEnterStation (string barCode, DeviceInfo device)
        {
            // Implementation for GenEnterStation
            MesDto<Value> mesDto = new MesDto<Value>();
            mesDto.ApiType = "ScadaController";
            mesDto.Method = "ScadaUploadData";

            Context context = new Context();
            context.Ticket = "";
            context.InvOrgId = 1;
            mesDto.Context = context;
    


            Parameter<Value> parameters = new Parameter<Value>();

            Value value = new Value();

            value.invOrg = 1;
            value.deviceCode = device.DeviceCode;
            value.deviceType = dicDevices[device.DeviceType];
            value.sn = barCode;
            value.moveType = 0;  //0 入站标志
            value.dataGenTime = DateTime.Now;
            value.isFirstProcess = false;

            value.actualValue = "";

            parameters.Value = value;

            mesDto.Parameters = new List<Parameter<Value>>() { parameters };
            return mesDto;



        }


        /// <summary>
        /// 封装出站
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="barCode"></param>
        /// <param name="device"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private MesDto<Value> GenOutStation<T>(string barCode, DeviceInfo device, T data) where T : class, new()
        {
            // Implementation for GenEnterStation
            MesDto<Value> mesDto = new MesDto<Value>();
            mesDto.ApiType = "ScadaController";
            mesDto.Method = "ScadaUploadData";

            Context context = new Context();
            context.Ticket = "";
            context.InvOrgId = 1;
            mesDto.Context = context;
     

            Parameter<Value> parameters = new Parameter<Value>();

            Value value = new Value();

            value.invOrg = 1;
            value.deviceCode = device.DeviceCode;
            value.deviceType = dicDevices[device.DeviceType];
            value.sn = barCode;
            value.moveType = 1; //1 出站标志
            value.dataGenTime = DateTime.Now;
            value.isFirstProcess = false;

            value.actualValue = ParseActualValue(data);

            parameters.Value = value;

            mesDto.Parameters = new List<Parameter<Value>>() { parameters };
            return mesDto;



        }


        /// <summary>
        /// 封装处理过程请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="barCode"></param>
        /// <param name="device"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private MesDto<Value> GenMesHandling<T>(string barCode, DeviceInfo device, T data)
 where T : class, new()
        {
            // Implementation for GenEnterStation
            MesDto<Value> mesDto = new MesDto<Value>();
            mesDto.ApiType = "ScadaController";
            mesDto.Method = "ScadaUploadData";

            Context context = new Context();
            context.Ticket = "";
            context.InvOrgId = 1;
            mesDto.Context = context;


            Parameter<Value> parameters = new Parameter<Value>();

            Value value = new Value();

            value.invOrg = 1;
            value.deviceCode = device.DeviceCode;
            value.deviceType = dicDevices[device.DeviceType];
            value.sn = barCode;
            value.moveType = 99; //99 处理过程标志
            value.dataGenTime = DateTime.Now;
            value.isFirstProcess = false;
            if (data != null)
            {
                value.actualValue = ParseActualValue(data);
            }
            else
            {
                value.actualValue = string.Empty;
            }

            parameters.Value = value;

            mesDto.Parameters = new List<Parameter<Value>>() { parameters };
            return mesDto;



        }


        /// <summary>
        /// 封装处理设备状态信息
        /// </summary>
        /// <param name="barCode"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        private MesDto<ValueStatus> GenDeviceStatus(string barCode, DeviceStatusInfo device)
        {
            MesDto<ValueStatus> mesDto = new MesDto<ValueStatus>();
            mesDto.ApiType = "ScadaController";
            mesDto.Method = "ScadaDeviceStatusUpload";


            Context context = new Context();
            context.Ticket = "";
            context.InvOrgId = 1;
            mesDto.Context = context;


            Parameter<ValueStatus> parameters = new Parameter<ValueStatus>();

            ValueStatus value = new ValueStatus();

            value.invOrg = 1;
            value.deviceCode = device.DeviceCode;
            value.runningState = device.RunningState;
            value.faultConditions = device.FaultConditions;
            value.isOnline = device.IsOnline;
            value.deviceType = dicDevices[device.DeviceType];
            value.stateGenTime = DateTime.Now;
            parameters.Value = value;
            mesDto.Parameters = new List<Parameter<ValueStatus>>() { parameters };
            return mesDto;

        }

        /// <summary>
        /// 发送入站请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="barCode"></param>
        /// <param name="device"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public MesResponse PostEnterStation(string barCode, DeviceInfo device)
        {

            if (IsBlock(device.Name, device.DeviceType))
            {
                return new MesResponse()
                {
                    Success = true,
                };
            }

            try
            {
                string url = "api/dataportal/invoke?ApiType=ScadaController";
                var mesDto = GenEnterStation(barCode, device);

                string jsonString = JsonConvert.SerializeObject(mesDto);

                //return JsonConvert.DeserializeObject<MesResponse>(Post(url, jsonString));

                var resp = JsonConvert.DeserializeObject<MesResponse>(Post(url, jsonString));
                this.logger.LogInformation($"发送 MES 进站数据:{jsonString}，PostEnterStation 响应：{JsonConvert.SerializeObject(resp)}");
                return resp;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"发送 MES  进站数据失败，PostEnterStation 请求异常：{ex.Message}");
                //throw;
            }

            return new MesResponse()
            {
                Success = false
            };
        }

        /// <summary>
        ///  发送出站请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="barCode"></param>
        /// <param name="device"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public MesResponse PostOutStation<T>(string barCode, DeviceInfo device, T data) where T : class, new()
        {

            if (IsBlock(device.Name, device.DeviceType))
            {
                this.logger.LogInformation($" MES已屏蔽,设备:{device.Name} ");
                return new MesResponse()
                {
                    Success = true,
                };
            }


            try
            {
                string url = "api/dataportal/invoke?ApiType=ScadaController";
                var mesDto = GenOutStation(barCode, device, data);
                string jsonString = JsonConvert.SerializeObject(mesDto);
                var resp = JsonConvert.DeserializeObject<MesResponse>(Post(url, jsonString));
                this.logger.LogInformation($"发送 MES 出站数据:{jsonString}，PostOutStation 响应：{JsonConvert.SerializeObject(resp)}");
                return resp;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"发送 MES 出站数据失败，PostOutStation 请求异常：{ex.Message}");

            }
            return new MesResponse()
            {
                Success = false
            };


        }


        /// <summary>
        /// 发送处理过程请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="barCode"></param>
        /// <param name="device"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public MesResponse PostHandling<T>(string barCode, DeviceInfo device, T data) where T : class, new()
        {

            if (IsBlock(device.Name, device.DeviceType))
            {
                this.logger.LogInformation($" MES已屏蔽,设备:{device.Name} ");
                return new MesResponse()
                {
                    Success = true,
                };
            }


            try
            {
                string url = "api/dataportal/invoke?ApiType=ScadaController";
                var mesDto = GenMesHandling(barCode, device, data);
                string jsonString = JsonConvert.SerializeObject(mesDto);
                //return JsonConvert.DeserializeObject<MesResponse>(Post(url, jsonString));

                var resp = JsonConvert.DeserializeObject<MesResponse>(Post(url, jsonString));
                this.logger.LogInformation($"发送 MES 加工中数据:{jsonString}，PostHandling 响应：{JsonConvert.SerializeObject(resp)}");
                return resp;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"发送 MES 加工中数据失败，PostHandling 请求异常：{ex.Message}");
                //throw;
            }

            return new MesResponse()
            {
                Success = false
            };

        }

        /// <summary>
        /// 发送设备状态
        /// </summary>
        /// <param name="barCode"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        public MesResponse PostDeviceStatus(string barCode, DeviceStatusInfo device)
        {
            if (IsBlock(device.Name, device.DeviceType))
            {
                this.logger.LogInformation($" MES已屏蔽,设备:{device.Name} ");
                return new MesResponse()
                {
                    Success = true,
                };
            }


            try
            {
                string url = "api/dataportal/invoke?ApiType=ScadaController";
                var mesDto = GenDeviceStatus(barCode, device);
                string jsonString = JsonConvert.SerializeObject(mesDto);

                var resp = JsonConvert.DeserializeObject<MesResponse>(Post(url, jsonString));
                this.logger.LogInformation($"发送 MES 设备状态数据:{jsonString}，PostDeviceStatus 响应：{JsonConvert.SerializeObject(resp)}");
                return resp;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"发送 MES 设备状态数据失败，PostDeviceStatus 请求异常：{ex.Message}");
                //throw;
            }

            return new MesResponse()
            {
                Success = false
            };


        }
        public string Post(string url, string data)
        {
            string moveTypeDesc = "未知";
            string stationName = string.Empty;
            const int maxAttempts = 4; // 1 次初试 + 3 次重试
            const int retryDelayMs = 200;
            url = hostname + url;//"api/dataportal/invoke?ApiType=ScadaController";
            /// "http://101.226.8.125:2030/api/dataportal/invoke?ApiType=ScadaController";
            string returnXml = "";
            var logId1 = Guid.NewGuid().ToString();  // 全过程使用同一个 logId 便于追踪
            string logId = logId1.Substring(logId1.Length - 5, 5);  // 全过程使用同一个 logId 便于追踪


            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    byte[] buf = Encoding.UTF8.GetBytes(data);

                    //LYNLog.Log.WriteLog($"{stationName}[{logId}] 第 {attempt} 次【{moveTypeDesc}】|【请求MES】：{data}", "运行", "MES接口");

                    request.Method = "POST";
                    request.ContentLength = buf.Length;
                    request.ContentType = "application/json";
                    request.MaximumAutomaticRedirections = 1;
                    request.AllowAutoRedirect = true;

                    using (Stream stream = request.GetRequestStream())
                    {
                        stream.Write(buf, 0, buf.Length);
                    }

                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        returnXml = reader.ReadToEnd();
                    }

                    //    LYNLog.Log.WriteLog($"{stationName}[{logId}] 第 {attempt} 次【{moveTypeDesc}】|【 MES返回】：{returnXml}", "运行", "MES接口");

                    if (!string.IsNullOrWhiteSpace(returnXml))
                        break; // 成功返回，退出循环
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, $"{stationName}[{logId}] 第 {attempt} 次{moveTypeDesc}| MES请求异常：{ex.Message}");
                    // LYNLog.Log.WriteLog($"{stationName}[{logId}] 第 {attempt} 次{moveTypeDesc}| MES请求异常：{ex.Message}", "异常", "MES接口");

                    if (attempt < maxAttempts)
                    {
                        //  LYNLog.Log.WriteLog($"{stationName}[{logId}] 准备第 {attempt + 1} 次重试...", "运行", "MES接口");
                        Thread.Sleep(retryDelayMs);
                    }
                    else
                    {
                        throw ex; // 最后一次尝试失败，抛出异常
                        // LYNLog.Log.WriteLog($"{stationName}[{logId}] 已重试 {maxAttempts} 次仍失败，放弃。", "异常", "MES接口");
                    }
                }
            }

            return returnXml;
        }



    }
}
