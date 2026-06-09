using HslCommunication.MQTT;
using MediatR;
using NetTaste;
using System.Text;
using WinformApp.Utils;
using YZV25.Core.Handler;
using YZV25.Dal;
using YZV25.Dto;
using YZV25.Entity;

namespace YZV25.Core.Monitor
{
    public abstract class BaseMonitor : IMonitor
    {
        protected readonly object _lockObj = new object();

        protected readonly string devicetype;
        protected readonly string deviceName;
        protected readonly ILogger logger;

        protected readonly IMediator mediator;

        //完成信号地址点
        protected string finishedSignalPoint;

        //设备状态，或是报警信号地址点
        protected string statusSignalPoint;
        protected Device device;
        protected string deviceCode;
        protected string code;
        protected bool _isRunning = false;

        private readonly MqttSubTool mqttSubTool;
        private CancellationTokenSource? cts = null;

        private readonly Action<string, int, string, string, string, string, string, CancellationToken> taskAction;
        private Task _task;
        private readonly Action<string, int, string, string, string, string, string, CancellationToken> warnAction;
        private Task _warnTask;
        private int times = 2; //发送两次 加工工程数据


        protected BaseMonitor(string devicetype, string deviceName, string[] signalPoints, MqttClient mqttClient,
            IMediator mediator,
            ILogger logger)
        {
            this.mediator = mediator;
            this.deviceName = deviceName;
            this.logger = logger;

            this.devicetype = devicetype;

            taskAction = async (string barcode, int pdaNo, string deviceCode, string deviceName, string deviceType, string finishedSignalPoint, string statusSignalPoint, CancellationToken token) =>
            {


                if (token == null) return;

                int i = 0;
                do
                {
                    try
                    {
                        token.ThrowIfCancellationRequested();

                        if (this.device == null || string.IsNullOrEmpty(code) || string.IsNullOrEmpty(deviceCode))
                        {
                            return;
                        }


                        //logger.LogInformation($"{devicetype}设备{deviceName}({deviceCode}),条码:{code},加工中，开始发送MES数据");
                        var result = HandleWorkingStatus(code, new DeviceInfo
                        {
                            DeviceCode = deviceCode ?? "",
                            DeviceType = devicetype,
                            Name = deviceName,
                            PdaNo = device.PdaNo,
                        });
                        logger.LogInformation(
                            $"{devicetype}设备{deviceName}({deviceCode}),条码:{code},加工中，结束发送MES数据:{result}");

                        //TODO: 这里可以添加监控逻辑，比如定时获取设备状态，或者根据特定条件触发数据发送等情况
                        //await Task.Delay(TimeSpan.FromMinutes(2), cts.Token);
                        await Task.Delay(TimeSpan.FromSeconds(90), token);


                        if (times != -1 && device.MaskSignal == 0)
                        {
                            i++;
                        }
                    }
                    catch (TaskCanceledException ex)
                    {
                        Console.WriteLine($"任务被取消: {ex.Message}");
                        // 这里可以处理取消逻辑
                    }
                    catch (OperationCanceledException ex)
                    {
                        Console.WriteLine($"操作被取消: {ex.Message}");
                        // OperationCanceledException 是 TaskCanceledException 的基类
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"操作异常被取消: {ex.Message}");
                    }
                } while (this._isRunning && !token.IsCancellationRequested && (times == -1 || i < times));


                Console.WriteLine("taskAction exit");
            };
            warnAction = async (string barcode, int pdaNo, string deviceCode, string deviceName, string deviceType, string finishedSignalPoint, string statusSignalPoint, CancellationToken token) =>
            {


                if (token == null) return;

                do
                {
                    try
                    {


                        token.ThrowIfCancellationRequested();

                        if (string.IsNullOrEmpty(barcode) || string.IsNullOrEmpty(deviceCode))
                        {
                            return;
                        }
                        var result = HandleAlarmStatus(barcode, new DeviceInfo
                        {
                            Barcode = barcode,
                            DeviceCode = deviceCode ?? "", //device.DeviceCode,
                            DeviceType = deviceType,
                            Name = deviceName,
                            PdaNo = device.PdaNo,
                        });
                        logger.LogInformation($"{devicetype}设备:{deviceName}({deviceCode}),条码:{barcode},状态数据，发送MES数据:{result}");

                        await Task.Delay(5000, token); // 每5秒检查一次预警条件
                    }
                    catch (TaskCanceledException ex)
                    {
                        logger.LogInformation($"{devicetype}设备:{deviceName}({deviceCode}),条码:{barcode},状态数据，发送MES数据,任务被取消: {ex.Message}");                        // 这里可以处理取消逻辑
                    }
                    catch (OperationCanceledException ex)
                    {
                        logger.LogInformation($"{devicetype}设备:{deviceName}({deviceCode}),条码:{barcode},状态数据，发送MES数据,操作被取消: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"{devicetype}设备:{deviceName}({deviceCode}),条码:{barcode},状态数据，发送MES数据,操作异常: {ex.Message}");
                    }
                } while (this._isRunning && !token.IsCancellationRequested);

                Console.WriteLine("warnAction exit");
            };

            //mqtt订阅工具，订阅CNC设备关联的夹具加工完成消息
            this.mqttSubTool = new MqttSubTool(mqttClient);

            if (signalPoints != null && signalPoints.Length > 0)
            {
                foreach (var item in signalPoints)
                {
                    this.mqttSubTool.AddTopic($"{deviceName}/{item}");
                }

                this.mqttSubTool.HandleMqttMessageReceived = HandleMqttMessageReceived;
                this.mqttSubTool.Start();
            }

        }

        private void HandleMqttMessageReceived(MqttApplicationMessage message)
        {
            var paylod = this.PraseMqttPayload(message.Payload);

            if (!this._isRunning)
            {
                logger.LogInformation(
                    $"{devicetype}设备:{deviceName}({deviceCode}),未在运行，忽略该消息 Topic:{message.Topic},值:{paylod}");

                return;
            }

            if (this.device == null || string.IsNullOrEmpty(code) || string.IsNullOrEmpty(deviceCode))
            {
                logger.LogInformation(
                    $"{devicetype}设备:{deviceName}({deviceCode}),条码:{code},设备信息为空，忽略该消息  Topic:{message.Topic},值:{paylod}");
                return;
            }


            if (!string.Equals(message.Topic, $"{deviceName}/{this.finishedSignalPoint}"))
            {
                logger.LogWarning(
                    $"{devicetype}设备:{deviceName}({deviceCode}),条码:{code},收到的信号Topic:{message.Topic},与singalPoint:{deviceName}/{this.finishedSignalPoint}  不匹配，忽略该消息");
                return;
            }

            //处理CNC设备发送的加工完成消息，通知mes系统
            if (!CheckFinished(paylod))
            {
                logger.LogInformation(
                    $"{devicetype}设备:{deviceName}({deviceCode}),条码:{code},的信号:{message.Topic},值为{paylod}，未完成加工，忽略该消息");
                return;
            }


            //通过网关获取获取CNC设备信息，并发送给mes系统 出站数据
            logger.LogInformation(
                $"{devicetype}设备:{deviceName}({deviceCode}),条码:{code},的信号:{message.Topic},值为{paylod},加工完成，开始发送MES数据");


            var result = HandleFinishedStatus(code, new DeviceInfo
            {
                DeviceCode = deviceCode,
                DeviceType = devicetype,
                Name = deviceName,
                PdaNo = device.PdaNo,
            }, paylod);
            logger.LogInformation(
                $"{devicetype}设备:{deviceName}({deviceCode}),条码:{code},的信号:{message.Topic},值为{paylod},加工完成，完成发送MES数据:{result}");

            // 通知处理完成，进行后续处理，比如更新数据库状态，或者触发其他业务逻辑等
            if (result)
            {
                this.mediator.Publish(new CompleteProcesRequest()
                {
                    Code = code,
                    Device = device
                });
            }

            //停止监控任务
            Task.Run(async () => { await this.Stop(); });
        }


        /// <summary>
        /// 启动任务
        /// </summary>
        /// <param name="monitorContext"></param>
        private void StartTask(MonitorContext monitorContext)
        {
            this.cts = new CancellationTokenSource();

            this._isRunning = true;

            this._task = Task.Run(() =>
            {
                if (cts == null
                || device == null)
                {
                    return;
                }


                taskAction(code, device.PdaNo, deviceCode, deviceName, devicetype, finishedSignalPoint, statusSignalPoint, cts.Token);

            }, cts.Token);
            this._warnTask = Task.Run(() =>
            {

                if (cts == null
                || device == null)
                {
                    return;
                }

                warnAction(code, device.PdaNo, deviceCode, deviceName, devicetype, finishedSignalPoint, statusSignalPoint, cts.Token);

            }, cts.Token);


            //如果device为屏蔽了夹具信号，则启动一个定时任务，对cmt无效
            if (this.device.MaskSignal == 1 && this.device.DeviceType != "CMT")
            {
                Task.Run(async () =>
                {
                    var localCts = this.cts;

                    if (localCts == null) return;

                    localCts.Token.ThrowIfCancellationRequested();

                    await Task.Delay(TimeSpan.FromSeconds(3 * 60), localCts.Token);


                    var result = HandleFinishedStatus(code, new DeviceInfo
                    {
                        DeviceCode = deviceCode,
                        DeviceType = devicetype,
                        Name = deviceName
                    }, null);


                    // 通知处理完成，进行后续处理，比如更新数据库状态，或者触发其他业务逻辑等
                    if (result)
                    {
                        await this.mediator.Publish(new CompleteProcesRequest()
                        {
                            Code = code,
                            Device = device
                        });
                    }

                    await this.Stop();
                }, cts.Token);
            }
        }

        /// <summary>
        /// 启动监听
        /// </summary>
        /// <param name="monitorContext"></param>
        public virtual void Start(MonitorContext monitorContext)
        {
            if (CheckStarted(monitorContext))
            {
                return;
            }

            lock (_lockObj)
            {
                if (CheckStarted(monitorContext))
                {
                    return;
                }

                BeforeStart(monitorContext);

                StartTask(monitorContext);

                logger.LogInformation($"{devicetype}设备:{deviceName},条码:{code}，启动监听");
            }
        }

        public virtual async Task Stop()
        {
            if (this._isRunning == false || this.cts == null || this.cts.IsCancellationRequested)
            {
                return;
            }

            this._isRunning = false;


            if (cts != null)
            {
                cts?.Cancel();
                var timeoutTask = Task.Delay(TimeSpan.FromSeconds(30));

                var waitTask = Task.WhenAll(new Task[] { _task, _warnTask });
                await Task.WhenAny(waitTask, timeoutTask);
                cts.Dispose();
                cts = null;

                _task = null;
                _warnTask = null;
            }

            logger.LogInformation($"{devicetype}设备:{deviceName},条码:{code}，停止监听");

            this.code = string.Empty;
            this.deviceCode = string.Empty;
            this.finishedSignalPoint = string.Empty;
            this.statusSignalPoint = string.Empty;
            this.device = null;
        }


        /// <summary>
        /// 检查是否以及开始监听
        /// </summary>
        /// <param name="monitorContext"></param>
        /// <returns></returns>
        protected bool CheckStarted(MonitorContext monitorContext)
        {
            if (this._isRunning && string.Equals(this.code, monitorContext.BarCode))
            {
                logger.LogInformation($"{devicetype}设备:{deviceName},已经启动,条码:{code}。");
                return true;
            }

            if (this._isRunning && !string.Equals(this.code, monitorContext.BarCode))
            {
                logger.LogWarning($"{devicetype}设备:{deviceName},条码冲突,前次条码:{code}，传入条码:{monitorContext.BarCode}。");
                return true;
            }

            return false;
        }


        /// <summary>
        /// 监听开始前
        /// </summary>
        /// <param name="monitorContext"></param>
        protected abstract void BeforeStart(MonitorContext monitorContext);


        /// <summary>
        /// 解析MQTT消息载荷，转换成具体的数值或者对象，供后续处理使用
        /// </summary>
        /// <param name="Payload"></param>
        /// <returns></returns>
        protected abstract object PraseMqttPayload(byte[] Payload);


        /// <summary>
        /// 判断MQTT消息载荷是否满足加工完成的条件，不同设备可能有不同的判断逻辑，比如数值达到某个阈值，或者状态变为某个特定值等
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        protected abstract bool CheckFinished(object payload);


        /// <summary>
        /// 加工时处理信息
        /// </summary>
        protected abstract bool HandleWorkingStatus(string barcode, DeviceInfo deviceInfo);

        /// <summary>
        /// 完成时处理信息
        /// </summary>
        protected abstract bool HandleFinishedStatus(string barcode, DeviceInfo deviceInfo, object payload);


        /// <summary>
        /// 报警时处理信息
        /// </summary>
        protected abstract bool HandleAlarmStatus(string barcode, DeviceInfo deviceInfo);


        /// <summary>
        /// 发送MES数据
        /// <param name="step"></param>
        /// <returns></returns>
        protected abstract bool SendMesData(string barcode, DeviceInfo deviceInfo, string status);

        /// <summary>
        /// 发送MES状态数据
        /// </summary>
        /// <returns></returns>
        protected abstract bool SendMesStatusData(DeviceStatusInfo deviceInfo);
    }


    public class MonitorContext
    {
        string deviceCode;
        string barcode;
        string startSigPoint;
        string finishSigPoint;
        string statusSigPoint;
        string face;
        Device device;

        /// <summary>
        /// 当前条码
        /// </summary>
        public string BarCode
        {
            get => barcode;
            set => barcode = value;
        }

        /// <summary>
        /// 启动信号网关中点位名称，优先级高于 device中的设置
        /// </summary>
        public string StartSigPoint
        {
            get => startSigPoint;
            set => startSigPoint = value;
        }

        /// <summary>
        /// 完成信号网关中点位名称，优先级高于 device中的设置
        /// </summary>
        public string FinishSigPoint
        {
            get => finishSigPoint;
            set => finishSigPoint = value;
        }

        /// <summary>
        /// 报警信号地址点
        /// </summary>
        public string StatusSigPoint
        {
            get => statusSigPoint;
            set => statusSigPoint = value;
        }

        /// <summary>
        /// 设备
        /// </summary>
        public Device Device
        {
            get => device;
            set => device = value;
        }

        /// <summary>
        /// 工作面 A面 B面，优先级高于 device中的设置
        /// </summary>
        public string Face
        {
            get => face;
            set => face = value;
        }

        /// <summary>
        /// 设备编号 ，优先级高于 device中的设置
        /// </summary>
        public string DeviceCode
        {
            get => deviceCode;
            set => deviceCode = value;
        }
    }

    /// <summary>
    /// 监听器接口
    /// </summary>
    public interface IMonitor
    {
        void Start(MonitorContext monitorContext);
        Task Stop();
    }
}
