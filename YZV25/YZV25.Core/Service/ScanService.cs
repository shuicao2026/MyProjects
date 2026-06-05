using Dm;
using HslCommunication.MQTT;
using HslCommunication.Profinet.Siemens;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Numerics;
using YZV25.Core.Handler;
using YZV25.Dal;
using YZV25.Dto;
using YZV25.Entity;
using YZV25.Handler;
using YZV25.Handler;
using YZV25.Utils;


namespace YZV25.Service
{
    public class ScanService
    {
        private readonly SqlServerDal _sqlServerDal;
        private readonly IServiceProvider _serviceProvider;

        private readonly MesRpcClient mesRpcClient;

        private readonly MqttClient _mqttClient;

        private readonly MqttSyncClient _mqttSyncClient;

        private readonly IMediator _mediator;

        private readonly ILogger<ScanService> _logger;

        public ScanService(SqlServerDal sqlServerDal,
            MesRpcClient mesRpcClient,
             MqttClient mqttClient,
             MqttSyncClient mqttSyncClient,
             IMediator mediator,
            IServiceProvider serviceProvider,
           ILogger<ScanService> logger)
        {
            this._logger = logger;
            this._sqlServerDal = sqlServerDal;

            this._serviceProvider = serviceProvider;

            this.mesRpcClient = mesRpcClient;

            this._mqttClient = mqttClient;

            this._mediator = mediator;

            this._mqttSyncClient = mqttSyncClient;
        }


        /// <summary>
        /// 处理CNC扫码逻辑
        /// </summary>
        /// <param name="code"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        private async Task<ReturnDto> HandleCNCScanAsync(int pdaNo, string code, Device device)
        {
            SiemensS7Net siemensS7Net = this._serviceProvider.GetRequiredKeyedService<SiemensS7Net>(device.Name);


            try
            {

                if (device.MaskSignal == 0)
                {
                    // 发送设备对应夹具加紧信号
                    var writeRes = await siemensS7Net.WriteAsync(device.StartSignalPoint, true);
                    if (!writeRes.IsSuccess)
                    {

                        this._logger.LogWarning($"条码:{code},PDA:{device.PdaNo},{device.DeviceType}设备:{device.Name},夹具加紧信号写入失败：{writeRes.ErrorCode},{writeRes.Message}");

                        throw new Exception($"夹具加紧信号写入失败:{writeRes.Message}");

                    }
                    this._logger.LogWarning($"条码:{code},PDA:{device.PdaNo},{device.DeviceType}设备:{device.Name},夹具加紧信号写入成功");

                }


                //启动 CNC 监视器
                await _mediator.Publish(new StartMonitorReqeust()
                {
                    Device = device,
                    BarCode = code
                });

            }
            catch (Exception ex)
            {

                this._logger.LogError(ex, $"条码:{code},PDA:{device.PdaNo},{device.DeviceType}设备{device.Name},启动CNC监视器异常:{ex.Message}");
                throw ex;
            }



            return new ReturnDto()
            {
                code = 0
            };

        }




        /// <summary>
        /// 处理FSW扫码逻辑，FSW使用的opcua通讯
        /// </summary>
        /// <param name="code"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        private async Task<ReturnDto> HandleFSWScanAsync(int pdaNo, string code, Device device)
        {
            SiemensS7Net siemensS7Net = this._serviceProvider.GetRequiredKeyedService<SiemensS7Net>(device.Name);


            // 发送设备对应夹具加紧信号
            if (device.MaskSignal == 0)
            {
                var writeRes = await siemensS7Net.WriteAsync(device.StartSignalPoint, true);
                if (!writeRes.IsSuccess)
                {
                    this._logger.LogWarning($"条码:{code},PDA:{device.PdaNo},{device.DeviceType}设备{device.Name},夹具加紧信号写入失败：{writeRes.ErrorCode},{writeRes.Message}");

                    throw new Exception($"夹具加紧信号写入失败:{writeRes.Message}");

                }
                this._logger.LogWarning($"条码:{code},PDA:{device.PdaNo},{device.DeviceType}设备:{device.Name},夹具加紧信号写入成功");

            }


            await _mediator.Publish(new StartMonitorReqeust()
            {
                Device = device,
                BarCode = code
            });

            return new ReturnDto()
            {
                code = 0
            };
        }






        /// <summary>
        /// 处理CMT pda 扫码
        /// </summary>
        /// <param name="code"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        private async Task<ReturnDto> HandleCMTScanAsync(int pdaNo, string code, Device device)
        {


            CMTMqttRpcClient mqttRpcClient = this._serviceProvider.GetRequiredKeyedService<CMTMqttRpcClient>(device.Name);


            //CMT设备不通过夹具控制，device.MaskSignal 对CMT设备无效
            //1 读取CMT设备的人工装件位面

            var face = mqttRpcClient.ReadFacePoint();

            if (string.IsNullOrEmpty(face))
            {
                this._logger.LogWarning($"条码:{code},CMT设备 {device.Name} 未检测到有效的人工装件位面。");

                throw new Exception($"CMT设备 {device.Name} 未检测到有效的人工装件位面。");

            }
            var startSignalPoint = $"{face}面工步";

            var finishSignalPoint = $"{face}面工步";

            var statusSignalPoint = $"设备状态{face}";

            //2.获取装机位对应的设备编号

            JObject jobj = JObject.Parse(device.Face);

            var deviceCode = jobj[face]?["DeviceCode"]?.ToString() ?? "";


            if (string.IsNullOrEmpty(deviceCode))
            {

                throw new Exception($"CMT设备 {device.Name} {face}人工装件位面未配置设备编号。");
            }

            device.DeviceCode = deviceCode;
            //3.发送MES入站请求
            await SendMesEneter(pdaNo, code, device);

            //4.发送信号点，启动设备继续生产
            mqttRpcClient.SendStartSignal(startSignalPoint);

            _logger.LogInformation($"条码:{code},CMT设备 {device.Name} 人工装件位面 {face}，已发送生产信号点 {startSignalPoint}，启动监视器。");

            await _mediator.Publish(new StartCMTMonitorReqeust

            {
                DeviceCode = deviceCode,
                Device = device,
                BarCode = code,
                Face = face,
                FinishSignalPoint = finishSignalPoint,
                StatusSignalPoint = statusSignalPoint
            });

            return new ReturnDto()
            {
                code = 0
            }
            ; ;

        }


        /// <summary>
        /// 更新扫码PDA的条码信息
        /// </summary>
        /// <param name="pdaNo"></param>
        /// <param name="barcode"></param>
        /// <returns></returns>
        private async Task UpdatePdaCode(int pdaNo, string barcode)
        {

            await this._sqlServerDal.GetDb().Updateable<Pda>()
                    .SetColumns(p => p.Barcode, barcode)
                    .SetColumns(p => p.UpdateTime, DateTime.Now)
                    .Where(p => p.No == pdaNo)
                    .ExecuteCommandAsync();
            this._logger.LogInformation($"PDA:{pdaNo},清除条码信息：{barcode}");


        }

        private async Task SendMesEneter(int pdaNo, string code, Device device)
        {

            //1 .发送mes入站请求
            var mesResp = mesRpcClient.PostEnterStation(code, new DeviceInfo()
            {
                DeviceCode = device.DeviceCode,
                DeviceType = device.DeviceType,
                Name = device.Name,
            });

            // mes出错 或是 mes返回false 则退出
            if (mesResp == null || !mesResp.Success)
            {
                this._logger.LogError($"条码:{code},PDA:{device.PdaNo},{device.DeviceType}设备{device.Name}");

                throw new Exception($"MES错误:{(mesResp != null ? mesResp.Message : "")}");

            }

            //2 MES成功，更新pda条码信息
            await this.UpdatePdaCode(pdaNo, code);
        }



        /// <summary>
        /// 扫码业务处理
        /// </summary>
        /// <param name="pdaDto"></param>
        /// <returns></returns>
        public async Task<ReturnDto> ScanAsync(PdaParam pdaDto)
        {

            var pda = await this._sqlServerDal.GetDb().Queryable<Pda>().FirstAsync(p => p.No == pdaDto.id);

            if (pda == null)
            {
                _logger.LogError($"无法找到PDA信息，PDA:{pdaDto.id}");
                return new ReturnDto()
                {
                    code = 1,
                    msg = "PDA配置错误,找不到PDA信息"
                };
            }


            if ((pda.Barcode != null && pda.Barcode != "0") &&
                string.Equals(pda.Barcode, pdaDto.code)

                )
            {
                _logger.LogWarning($"条码:{pdaDto.code},PDA:{pdaDto.id} 已经扫描过。");
                return new ReturnDto()
                {
                    code = 2,
                    msg = $"重复扫码:{pdaDto.code},请先等待或者点击强制完成"
                }; ;
            }



            if ((pda.Barcode != null && pda.Barcode != "0") && !string.Equals(pda.Barcode, pdaDto.code))
            {
                _logger.LogWarning($"PDA:{pdaDto.id} 扫描的条码与上次扫描的不同，更新PDA表条码信息。旧条码: {pda.Barcode}, 新条码: {pdaDto.code}");
                // Here you would typically update the 
                return new ReturnDto()
                {
                    code = 3,
                    msg = $"目前有条码未完成,请先等待或者点击强制完成"
                }; ;
            }

            var device = await this._sqlServerDal.GetDb().Queryable<Device>().FirstAsync(d => d.PdaNo == pdaDto.id);

            if (device == null)
            {

                _logger.LogWarning($"无法找到PDA关联的设备信息，PDA:{pdaDto.id}");
                return new ReturnDto()
                {
                    code = 4,
                    msg = $"PDA配置错误,找不到关联的设备信息"
                };
            }
            ReturnDto result = null;


            try
            {





                switch (device.DeviceType)
                {
                    case "CNC":
                        {
                            //发送MES入站请求
                            await SendMesEneter(pdaDto.id, pdaDto.code, device);
                            result = await HandleCNCScanAsync(pdaDto.id, pdaDto.code, device);
                        }

                        break;
                    case "FSW":
                        {
                            //发送MES入站请求
                            await SendMesEneter(pdaDto.id, pdaDto.code, device);
                            result = await HandleFSWScanAsync(pdaDto.id, pdaDto.code, device);
                        }

                        break;
                    case "CMT":
                        result = await HandleCMTScanAsync(pdaDto.id, pdaDto.code, device);
                        break;

                }


                return new ReturnDto()
                {
                    code = 0,
                    msg = $"操作成功"
                };

            }
            catch (Exception ex)
            {


                this._logger.LogError(ex, $"条码:{pdaDto.code},PDA:{pdaDto.id},设备{device.Name}, 扫码异常:{ex.Message}");

                return new ReturnDto()
                {
                    code = -1,
                    msg = $"操作出错:{ex.Message}"
                };
            }




        }


        /// <summary>
        /// 获取pda 扫描状态
        /// </summary>
        /// <returns></returns>
        public async Task<List<PdaQueryDto>> GetALLPdaRTInfo()
        {
            var result = await this._sqlServerDal.GetDb().SqlQueryable<PdaQueryDto>("SELECT " +
                "pda.id Id," +
                "no,barcode Barcode," +
                "update_time UpdateTime," +
                "device.device_code DeviceCode," +
                "device_type DeviceType," +
                "device.name DeviceName," +
                "device.title DeviceTitle" +
                " FROM [dbo].[pda] join device on pda.[no]=device.pda_no order by pda.id").ToListAsync();
            return result;
        }

        /// <summary>
        /// 强制完成
        /// </summary>
        /// <param name="pdaDto"></param>
        /// <returns></returns>
        public async Task<int> ForceFinish(PdaForceParam pdaDto)
        {

            var pda = await this._sqlServerDal.GetDb().Queryable<Pda>().FirstAsync(pda => pda.No == pdaDto.id);

            if (!string.Equals(pda.Password, pdaDto.pwd))
            {
                return -1;
            }


            var device = await this._sqlServerDal.GetDb().Queryable<Device>().FirstAsync(d => d.PdaNo == pdaDto.id);


            if (device == null)
            {
                return -2;
            }

            var result = 0;
            string Barcode = pdaDto.code;
            if (string.IsNullOrEmpty(Barcode))
            {
                Barcode = pda.Barcode;
            }

            if (!string.IsNullOrEmpty(Barcode) &&
                !string.Equals(Barcode, "0"))
            {

                string deviceCode = device.DeviceCode;

                if (device.DeviceType == "CMT")
                {
                    JObject jobj1 = JObject.Parse(device.Face);

                    deviceCode = jobj1[pdaDto.face.ToUpper()]?["DeviceCode"]?.ToString() ?? "";

                }


                //发送强制出站给MES
                JObject jobj = new JObject();
                var mesResp = mesRpcClient.PostOutStation(Barcode, new DeviceInfo()
                {
                    DeviceCode = deviceCode,
                    DeviceType = device.DeviceType,
                    Name = device.Name,

                }, jobj);

                if (mesResp == null)
                {
                    result = 1;

                }
                else
                {
                    result = mesResp.Success ? 0 : 1;
                }
                if (result == 1)
                {
                    return result;
                }
            }


            //CMT设备不通过夹具控制，device.MaskSignal 对CMT设备无效
            if (device.DeviceType != "CMT" && device.MaskSignal == 0)
            {
                SiemensS7Net siemensS7Net = this._serviceProvider.GetRequiredKeyedService<SiemensS7Net>(device.Name);
                var writeRes = await siemensS7Net.WriteAsync(device.StartSignalPoint, false);
                if (!writeRes.IsSuccess)
                {
                    this._logger.LogWarning($"条码:{Barcode},PDA:{device.PdaNo},{device.DeviceType}设备{device.Name} ,扫码完成信号:{device.StartSignalPoint},复位失败：{writeRes.ErrorCode},{writeRes.Message}");

                }
                else
                {
                    this._logger.LogInformation($"条码:{Barcode},PDA:{device.PdaNo},{device.DeviceType}设备{device.Name},扫码完成信号:{device.StartSignalPoint},复位");

                }


            }


            // 如果pda上传的条码为空或者和当前pda的条码一致则停止监视器并清空条码
            if (string.IsNullOrEmpty(pdaDto.code) || string.Equals(pdaDto.code, pda.Barcode))
            {

                //清空对应PDA的条码
                await UpdatePdaCode(pdaDto.id, "0");

                //停止 CNC 监视器
                await _mediator.Publish(new StopMonitorRequest()
                {
                    PdaNo = pdaDto.id,
                    Face = pdaDto.face,
                    Device = device,

                });



            }

            return result;

        }

        /// <summary>
        /// 完成加工进程
        /// </summary>
        /// <param name="barcode"></param>
        /// <param name="device"></param>
        /// <returns></returns>

        public async Task FinishProcess(string barcode, Device device)
        {



            await UpdatePdaCode(device.PdaNo, "0");

            //CMT设备不通过夹具控制，device.MaskSignal 对CMT设备无效
            if (device.DeviceType != "CMT" && device.MaskSignal == 0)
            {
                SiemensS7Net siemensS7Net = this._serviceProvider.GetRequiredKeyedService<SiemensS7Net>(device.Name);
                var writeRes = await siemensS7Net.WriteAsync(device.StartSignalPoint, false);
                if (!writeRes.IsSuccess)
                {
                    this._logger.LogWarning($"条码:{barcode},PDA:{device.PdaNo},{device.DeviceType}设备{device.Name} ,扫码完成信号:{device.StartSignalPoint},复位失败：{writeRes.ErrorCode},{writeRes.Message}");
                    return;
                }

                this._logger.LogInformation($"条码:{barcode},PDA:{device.PdaNo},{device.DeviceType}设备{device.Name},扫码完成信号:{device.StartSignalPoint},复位");


            }



        }
   
    }
}
