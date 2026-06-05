using Model.Common;
using Newtonsoft.Json.Linq;
using Opc.Ua;
using OpcUaHelper;

namespace YZV25.Utils
{
    public class FSWOpcUaClient
    {
        private readonly OpcUaClient opcUaClient;
        private readonly string url;
        public FSWOpcUaClient(string url)
        {
            this.url = url;
            opcUaClient = new OpcUaClient();
            opcUaClient.UserIdentity = new UserIdentity("OPCUA", "OPCUA123");
        

        }

        public void Connect()
        {
            opcUaClient.ConnectServer(url).Wait();
        }

        public 搅拌焊 ReadFSWData()
        {
            var entity = new 搅拌焊(); // 目标对象
            try
            {
                // 添加所有的读取的节点，此处的示例是类型不一致的情况
                List<NodeId> nodeIds = new List<NodeId>();

                nodeIds.Add(new NodeId("ns=2;s=/Channel/State/progStatus"));
                nodeIds.Add(new NodeId("ns=2;s=/Bag/State/opMode"));
                nodeIds.Add(new NodeId("ns=2;s=/Channel/Spindle/speedOvr[u1,1]"));
                nodeIds.Add(new NodeId("ns=2;s=/Channel/GeometricAxis/feedRateOvr[u1,1]"));
                nodeIds.Add(new NodeId("ns=2;s=/Channel/Spindle/cmdSpeed"));
                nodeIds.Add(new NodeId("ns=2;s=/Channel/Spindle/actSpeed[u1,1]"));
                nodeIds.Add(new NodeId("ns=2;s=/Plc/DB9031.DBD36:REAL"));
                nodeIds.Add(new NodeId("ns=2;s=/Plc/DB9031.DBD24"));
                nodeIds.Add(new NodeId("ns=2;s=/Plc/DB9031.DBD56:REAL"));
                nodeIds.Add(new NodeId("ns=2;s=/Plc/DB9031.DBW60"));
                nodeIds.Add(new NodeId("ns=2;s=/Channel/GeometricAxis/actToolBasePos[u1,1]"));
                nodeIds.Add(new NodeId("ns=2;s=/Channel/GeometricAxis/actToolBasePos[u1,2]"));
                nodeIds.Add(new NodeId("ns=2;s=/Channel/GeometricAxis/actToolBasePos[u1,3]"));
                nodeIds.Add(new NodeId("ns=2;s=/Channel/GeometricAxis/actToolBasePos[u1,5]"));
                nodeIds.Add(new NodeId("ns=2;s=/Nck/State/numAlarms"));
                nodeIds.Add(new NodeId("ns=2;s=/Nck/SequencedAlarms/textIndex[1]"));
                nodeIds.Add(new NodeId("ns=2;s=/Nck/SequencedAlarms/textIndex[2]"));
                nodeIds.Add(new NodeId("ns=2;s=/Channel/State/oldProgNetTime[1,1]"));
                nodeIds.Add(new NodeId("ns=2;s=/Channel/State/actProgNetTime[1,1]"));
                nodeIds.Add(new NodeId("ns=2;s=/Channel/ChannelDiagnose/operatingTime[1]"));
                nodeIds.Add(new NodeId("ns=2;s=/Channel/Programinfo/progName[u1]"));
                nodeIds.Add(new NodeId("ns=2;s=/Plc/DB9031.DBD36:REAL"));
                nodeIds.Add(new NodeId("ns=2;s=/Plc/DB9003.DBX2.0"));
                nodeIds.Add(new NodeId("ns=2;s=/Plc/DB9030.DBD12:REAL"));
                nodeIds.Add(new NodeId("ns=2;s=/Plc/DB9006.DBX0.0"));
                nodeIds.Add(new NodeId("ns=2;s=/Channel/ProgramInfo/progName[u1]"));

                List<DataValue> dataValues = opcUaClient.ReadNodes(nodeIds.ToArray());


                var props = entity.GetType().GetProperties();

                int i = 0;
                foreach (var prop in props)
                {
                    if (!prop.CanWrite) continue; // 没有setter就跳过

                    object value;
                    if (i == 17 || i == 18)
                    {
                        // 这里假设 dataValues[i].Value 是数组类型（object[]）
                        var arr = dataValues[i].Value as Array;
                        value = arr != null && arr.Length > 0 ? arr.GetValue(0)?.ToString() : string.Empty;
                    }
                    else
                    {
                        value = dataValues[i].Value?.ToString() ?? string.Empty;
                    }

                    prop.SetValue(entity, value); // 把值赋到属性里
                    i++;
                }

            }
            catch (Exception ex)
            {

            }

            return entity;
        }


        public string ReadFSWAlarmData()
        {

            try
            {
                List<NodeId> nodeIds = new List<NodeId>();



                nodeIds.Add(new NodeId("ns=2;s=/Nck/State/numAlarms"));
                nodeIds.Add(new NodeId("ns=2;s=/Nck/SequencedAlarms/textIndex[1]"));
                nodeIds.Add(new NodeId("ns=2;s=/Nck/SequencedAlarms/textIndex[2]"));
                nodeIds.Add(new NodeId("ns=2;s=/Plc/DB9003.DBX2.0"));



                // dataValues按顺序定义的值，每个值里面需要重新判断类型
                List<DataValue> dataValues = opcUaClient.ReadNodes(nodeIds.ToArray());
                //return Boolean.Parse(dataValues[0].Value?.ToString() ?? "false");

                JObject json = new JObject
                {
                    ["报警数"] = dataValues[0].Value?.ToString() ?? string.Empty,
                    ["NC报警"] = dataValues[1].Value?.ToString() ?? string.Empty,
                    ["PLC报警"] = dataValues[2].Value?.ToString() ?? string.Empty,
                    ["急停报警"] = dataValues[3].Value?.ToString() ?? string.Empty
                };

                return json.ToString();

            }
            catch (Exception)
            {

                
            }

         
            return string.Empty;

        }

    }
}
