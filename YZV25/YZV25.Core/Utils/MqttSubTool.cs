using HslCommunication.MQTT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinformApp.Utils
{
    public class MqttSubTool
    {
        protected MqttClient mqttClient;

        protected List<string> topics = new List<string>();

        public MqttSubTool(MqttClient mqttClient)
        {
            this.mqttClient = mqttClient;
            this.mqttClient.OnClientConnected += MqttClient_OnClientConnected;

            this.mqttClient.OnMqttMessageReceived += MqttClient_OnMqttMessageReceived;

            this.mqttClient.OnNetworkError += MqttClient_OnNetworkError;
        }

        private void MqttClient_OnNetworkError(object? sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void MqttClient_OnClientConnected(MqttClient client)
        {

            foreach (var item in topics)
            {
                client.SubscribeMessage(item);
            }
        }

        private void MqttClient_OnMqttMessageReceived(MqttClient client, MqttApplicationMessage message)
        {
            HandleMqttMessageReceived.Invoke(message);
        }


        public Action<MqttApplicationMessage> HandleMqttMessageReceived;


        public  void AddTopic(string topic)
        {
            topics.Add(topic);
        }

        public  void Start()
        {

            mqttClient.ConnectServer();
        }

        public void Stop()
        {

            mqttClient.ConnectClose();

        }


    }


}
