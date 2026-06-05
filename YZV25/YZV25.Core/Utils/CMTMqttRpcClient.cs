using HslCommunication;
using HslCommunication.MQTT;
using Newtonsoft.Json.Linq;

namespace YZV25.Utils
{
    public class CMTMqttRpcClient
    {

        private readonly MqttSyncClient mqttRpcClient;

        private readonly string name;

        private readonly string[] faces ;
        public CMTMqttRpcClient(string name, string ip, int port, string[] faces)
        {
            this.name = name;
            this.mqttRpcClient = new MqttSyncClient(new MqttConnectionOptions()
            {

                IpAddress = ip,
                Port = port,
                Credentials = new MqttCredential("admin", "123456"),   // 设置了用户名和密码
                ConnectTimeout = 2000
            });
            this.faces = faces;         

        }

        /// <summary>
        /// 读取CMT设备的工作面位  A面 B面 
        /// </summary>
        /// <returns></returns>
        public string ReadFacePoint()
        {
            // 通过 MQTT RPC mqttRpcClient 读取信号点，判断是否允许设备继续生产
            foreach (var face in faces)
            {
                bool read = mqttRpcClient.ReadRpc<bool>("Edge/DeviceData", new { data = $"{name}/{face}面在人工装件位" }).Content;
                if (read)
                {
                    return face;
                }
            }

            //bool readA = mqttRpcClient.ReadRpc<bool>("Edge/DeviceData", new { data = $"{name}/A面在人工装件位" }).Content;

            //bool readB = mqttRpcClient.ReadRpc<bool>("Edge/DeviceData", new { data = $"{name}/B面在人工装件位" }).Content;

            //if (readA && !readB)
            //{
            //    return "A";
            //}
            //if (readB && !readA)
            //{
            //    return "B";
            //}
            return string.Empty;

            //JObject jsonObj = JObject.Parse(read.Content.ToString());
            //string facePointValue = jsonObj["value"].ToString();
            //return facePointValue;
        }

        /// <summary>
        /// 发送启动信号点 11，设备开始生产
        /// </summary>
        /// <param name="singalPoint"></param>
        public void SendStartSignal(string singalPoint)
        {
            // 通过 MQTT RPC mqttRpcClient 发送信号点，允许设备继续生产
            OperateResult send = mqttRpcClient.ReadRpc<JObject>("Edge/WriteData", new { data = $"{name}/{singalPoint}", value = 11 });
        }

        /// <summary>
        /// 收到70 完成后，回发71 表示确认已经收到
        /// </summary>
        /// <param name="singalPoint"></param>
        public void SendOKSingnal(string singalPoint)
        {

            OperateResult send = mqttRpcClient.ReadRpc<JObject>("Edge/WriteData", new { data = $"{name}/{singalPoint}", value = 71 });
        }

        public JObject ReadCMTData()
        {
           

            // 通过 MQTT RPC mqttRpcClient
            OperateResult<JObject> read = mqttRpcClient.ReadRpc<JObject>("Edge/DeviceData", new { data = $"{name}" });
            if (!read.IsSuccess)
            {
                return null;

            }
            JObject jsonObj = JObject.Parse(read.Content.ToString());
            return jsonObj;
        }

        public int ReadStatusData(string address)
        {
            OperateResult<int> read = mqttRpcClient.ReadRpc<int>("Edge/DeviceData", new { data = $"{address}" });

            if(!read.IsSuccess)
            {
                return -1;
            }

            var jsonObj = int.Parse(read.Content.ToString());
            return jsonObj;


        }
    }
}
