using HslCommunication;
using HslCommunication.Profinet.Melsec;
using NetTaste;
using Newtonsoft.Json.Linq;
using SqlSugar;
using System.Xml.Linq;

namespace YZV25.Core.Utils
{
    public class CMTMcClient
    {
        private MelsecA1ENet melsecA1ENet;

        private readonly string[] faces = new string[] { "A", "B", "C" };


        private readonly Dictionary<string, string> signalPoint = new Dictionary<string, string>() {
            {"A", "D701"},
            {"B", "D721"},
            {"C", "D741"}

        };

        private readonly Dictionary<string, string> facePoint = new Dictionary<string, string>() {
            {"A", "D715"},
            {"B", "D735"},
            {"C", "D755"}
        };

        private readonly Dictionary<string, string> startAddr = new Dictionary<string, string>() {
            {"A", "D701"},
            {"B", "D721"},
            {"C", "D741"}



        };

        private readonly Dictionary<string, Dictionary<int, string>> addressMap = new Dictionary<string, Dictionary<int, string>>() {
            { "A", new Dictionary<int, string>() {
                    { 0, "主工步A" },
                    { 1, "主条码A" },
                    { 2, "设备状态A" },
                    { 3, "设备报警代码A" },
                    { 4, "R1程序号_实际值A" },
                    { 5, "R2程序号_实际值A" },
                    { 6, "R1焊接时间_实际值A" },
                    { 7, "R2焊接时间_实际值A" },
                    { 8, "R1导电嘴焊接数_实际值A" },
                    { 9, "R2导电嘴焊接数_实际值A" },
                    { 10, "R1导电嘴焊接数设定_A" },
                    { 11, "R2导电嘴焊接数设定_A" },
                    { 12, "R1程序号_设定值A" },
                    { 14, "A步" },

               }

            },
            { "B", new Dictionary<int, string>() {
                    { 0, "主工步B" },
                    { 1, "主条码B" },
                    { 2, "设备状态B" },
                    { 3, "设备报警代码B" },
                    { 4, "R1程序号_实际值B" },
                    { 5, "R2程序号_实际值B" },
                    { 6, "R1焊接时间_实际值B" },
                    { 7, "R2焊接时间_实际值B" },
                    { 8, "R1导电嘴焊接数_实际值B" },
                    { 9, "R2导电嘴焊接数_实际值B" },
                    { 10, "R1导电嘴焊接数设定_B" },
                    { 11, "R2导电嘴焊接数设定_B" },
                    { 12, "R1程序号_设定值B" },
                    { 14, "B步" },

                 }

            },
            { "C", new Dictionary<int, string>() {
                    { 0, "主工步C" },
                    { 1, "主条码C" },
                    { 2, "设备状态C" },
                    { 3, "设备报警代码C" },
                    { 4, "R1程序号_实际值C" },
                    { 5, "R2程序号_实际值C" },
                    { 6, "R1焊接时间_实际值C" },
                    { 7, "R2焊接时间_实际值C" },
                    { 8, "R1导电嘴焊接数_实际值C" },
                    { 9, "R2导电嘴焊接数_实际值C" },
                    { 10, "R1导电嘴焊接数设定_C" },
                    { 11, "R2导电嘴焊接数设定_C" },
                    { 12, "R1程序号_设定值C" },
                    { 14, "C步" }


            } }
        };


        Task subTask;

        private CancellationTokenSource? cts = null;


        public CMTMcClient(string ip, int port, string[] faces)
        {
            melsecA1ENet = new MelsecA1ENet(ip, port);

            this.faces = faces;
        }

        public string ReadFacePoint()
        {
            // 通过 MQTT RPC mqttRpcClient 读取信号点，判断是否允许设备继续生产
            foreach (var face in faces)
            {
                OperateResult<short> res = melsecA1ENet.ReadInt16(facePoint[face]);
                if (res.IsSuccess && res.Content == 1)
                {
                    return face;
                }
            }


            return string.Empty;

        }

        public int ReadStatusData()
        {
            OperateResult<short> res = melsecA1ENet.ReadInt16("D700");

            if (!res.IsSuccess)
            {
                return -1;
            }

            //var jsonObj = int.Parse(res.Content.ToString());
            return res.Content;


        }

        public JObject ReadCMTData(string face)
        {

            var jobj = new JObject();


            OperateResult<short[]> res = melsecA1ENet.ReadInt16(startAddr[face], 15);

            if (!res.IsSuccess)
            {

                foreach (var kv in addressMap[face])
                {
                    if (kv.Key >= 0 && kv.Key < res.Content.Length)
                        jobj[kv.Value] = null;
                }
                return null;
            }


            // 通过 MQTT RPC mqttRpcClient
            foreach (var kv in addressMap[face])
            {
                if (kv.Key >= 0 && kv.Key < res.Content.Length)
                    jobj[kv.Value] = res.Content[kv.Key];
            }

            return jobj;


        }

        /// <summary>
        /// 发送启动信号点 11，设备开始生产
        /// </summary>
        /// <param name="singalPoint"></param>
        public void SendStartSignal(string face)
        {
            melsecA1ENet.Write(signalPoint[face], 11);


        }

        /// <summary>
        /// 收到70 完成后，回发71 表示确认已经收到
        /// </summary>
        /// <param name="singalPoint"></param>
        public void SendOKSignal(string face)
        {
            melsecA1ENet.Write(signalPoint[face], 71);
        }

        public async Task UnSubFinishSignal(string face)
        {
            if (cts == null || subTask == null)
            {
                return;
            }


            cts.Cancel();


            await Task.WhenAny([subTask, Task.Delay(TimeSpan.FromSeconds(30))]);

            cts.Dispose();
            cts = null;
            subTask = null;
            return;

        }

        public void SubFinishSignal(string face, Action<string, short> action)
        {

            UnSubFinishSignal(face);

            cts = new CancellationTokenSource();
            var token = cts.Token;


            subTask = Task.Run(async () =>
            {
                using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(1000));

                while (await timer.WaitForNextTickAsync(token))
                {
                    Console.WriteLine($"Tick: {DateTime.Now:HH:mm:ss.fff}");
                    // 这里放定时逻辑
                    token.ThrowIfCancellationRequested(); // 检查是否请求取消

                    try
                    {
                        OperateResult<short> res = melsecA1ENet.ReadInt16(signalPoint[face]);
                        if (!res.IsSuccess)
                        {
                            continue;
                        }
                        action(face, res.Content);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Tick: {DateTime.Now:HH:mm:ss.fff} {ex.Message}");
                    }

                }

            }, token);

        }
    }
}
