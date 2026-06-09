using Dm;
using HslCommunication.MQTT;
using HslCommunication.Profinet.Siemens;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Numerics;
using YZV25.Core.Handler;
using YZV25.Core.Service;
using YZV25.Core.Utils;
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
        private readonly BusinessLogService _businessLogService;

        public ScanService(SqlServerDal sqlServerDal,
            MesRpcClient mesRpcClient,
             MqttClient mqttClient,
             MqttSyncClient mqttSyncClient,
             IMediator mediator,
            IServiceProvider serviceProvider,
            BusinessLogService businessLogService,
           ILogger<ScanService> logger)
        {
            this._logger = logger;
            this._sqlServerDal = sqlServerDal;

            this._serviceProvider = serviceProvider;

            this.mesRpcClient = mesRpcClient;

            this._mqttClient = mqttClient;

            this._mediator = mediator;

            this._mqttSyncClient = mqttSyncClient;
            this._businessLogService = businessLogService;
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
                this._logger.LogWarning($"条码:{code},CMT设备{device.Name} 未检测到有效的人工装件位面。");

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
            var mesResp = await SendMesEnter(pdaNo, code, face, device);
            if (mesResp == null || !mesResp.Success)
            {
                throw new Exception($"MES错误:{(mesResp != null ? mesResp.Message : "")}");

            }

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
            };


        }

        private async Task<ReturnDto> HandleCMTMCScanAsync(int pdaNo, string code, Device device)
        {


            CMTMcClient mqttRpcClient = this._serviceProvider.GetRequiredKeyedService<CMTMcClient>(device.Name);


            //CMT设备不通过夹具控制，device.MaskSignal 对CMT设备无效
            //1 读取CMT设备的人工装件位面

            var face = mqttRpcClient.ReadFacePoint();

            if (string.IsNullOrEmpty(face))
            {
                this._logger.LogWarning($"条码:{code},CMT设备{device.Name} 未检测到有效的人工装件位面。");

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
            var mesResp = await SendMesEnter(pdaNo, code, face, device);
            if (mesResp == null || !mesResp.Success)
            {
                throw new Exception($"MES错误:{(mesResp != null ? mesResp.Message : "")}");

            }

            //4.发送信号点，启动设备继续生产
            mqttRpcClient.SendStartSignal(face);

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
            };


        }

        /// <summary>
        /// 更新扫码PDA的条码信息
        /// </summary>
        /// <param name="pdaNo"></param>
        /// <param name="barcode"></param>
        /// <returns></returns>
        private async Task UpdatePdaCode(int pdaNo, string barcode, string face, bool? isBlockMes)
        {

            await this._sqlServerDal.GetDb().Updateable<Pda>()
                    .SetColumns(p => p.Barcode, barcode)
                    .SetColumns(p => p.Face, face)
                    .SetColumns(p => p.MesStatus, isBlockMes.HasValue ? (isBlockMes.Value ? 2 : 1) : 0)
                    .SetColumns(p => p.UpdateTime, DateTime.Now)
                    .Where(p => p.No == pdaNo)
                    .ExecuteCommandAsync();
            this._logger.LogInformation($"PDA:{pdaNo},清除条码信息：{barcode}");


        }

        /// <summary>
        /// 发送pda MES进站
        /// </summary>
        /// <param name="pdaNo"></param>
        /// <param name="code"></param>
        /// <param name="face"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        private async Task<MesResponse> SendMesEnter(int pdaNo, string code, string face, Device device)
        {

            //1 .发送mes入站请求
            var mesResp = mesRpcClient.PostEnterStation(code, new DeviceInfo()
            {
                DeviceCode = device.DeviceCode,
                DeviceType = device.DeviceType,
                Name = device.Name,
            });
            var isBlockMes = mesRpcClient.IsBlock(device.Name, device.DeviceType);
            // 记录MES业务日志
            await this._businessLogService.LogMesEnter(pdaNo, code, device.DeviceType, device.Name, device.DeviceCode,
                 !isBlockMes, mesResp?.Success ?? false, mesResp?.Message);

            // mes出错 或是 mes返回false 则退出
            if (mesResp != null && mesResp.Success)
            {

                //2 MES成功，更新pda条码信息
                await this.UpdatePdaCode(pdaNo, code, face, isBlockMes);


            }

            return mesResp;

        }

        private async Task<MesResponse> SendMesForceExit(int pdaNo, string code, string deviceCode, string deviceName, string deviceType)
        {
            var mesResp = mesRpcClient.PostOutStation(code, new DeviceInfo()
            {
                DeviceCode = deviceCode,
                DeviceType = deviceType,
                Name = deviceName,
            }, new JObject());

            await this._businessLogService.LogMesExit(pdaNo, code,
                deviceType, deviceName, deviceCode,
                  mesRpcClient.IsEnable(),
                  mesResp?.Success ?? false,
                  mesResp?.Message ?? "", null);

            return mesResp;

            //if (mesResp == null || !mesResp.Success)
            //{
            //    //this._logger.LogError($"条码:{code},PDA:{device.PdaNo},{device.DeviceType}设备{device.Name},发送MES出站请求失败：{mesResp?.Message}");
            //    throw new Exception($"MES错误:{(mesResp != null ? mesResp.Message : "")}");
            //}


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
                            var mesResp = await SendMesEnter(pdaDto.id, pdaDto.code, pdaDto.face, device);
                            if (mesResp == null || !mesResp.Success)
                            {
                                throw new Exception($"MES错误:{(mesResp != null ? mesResp.Message : "")}");

                            }
                            result = await HandleCNCScanAsync(pdaDto.id, pdaDto.code, device);
                        }

                        break;
                    case "FSW":
                        {
                            //发送MES入站请求
                            var mesResp = await SendMesEnter(pdaDto.id, pdaDto.code, pdaDto.face, device);
                            if (mesResp == null || !mesResp.Success)
                            {
                                throw new Exception($"MES错误:{(mesResp != null ? mesResp.Message : "")}");
                            }
                            ;
                            result = await HandleFSWScanAsync(pdaDto.id, pdaDto.code, device);
                        }

                        break;
                    case "CMT":
                        switch (device.Name)
                        {
                            //cmt2 是西门子的可以用网关监听
                            case "cmt2":
                                {
                                    result = await HandleCMTScanAsync(pdaDto.id, pdaDto.code, device);
                                }
                                break;
                            default:
                                {
                                    //cmt1,cmt3 三菱无法使用网关

                                    result = await HandleCMTMCScanAsync(pdaDto.id, pdaDto.code, device);
                                }
                                break;

                        }

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
                "pda.face ," +
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
        public async Task<ReturnDto> ForceFinish(PdaForceParam pdaDto)
        {

            var pda = await this._sqlServerDal.GetDb().Queryable<Pda>().FirstAsync(pda => pda.No == pdaDto.id);

            if (!string.Equals(pda.Password, pdaDto.pwd))
            {
                return new ReturnDto()
                {
                    code = -1,
                    msg = "配置错误，无法找到对应的pda！"
                };
            }


            var device = await this._sqlServerDal.GetDb().Queryable<Device>().FirstAsync(d => d.PdaNo == pdaDto.id);


            if (device == null)
            {
                return new ReturnDto()
                {
                    code = -1,
                    msg = "配置错误，无法找到pda对应的设备！"
                };
            }


            if (device.DeviceType == "CMT" && //设备工作台面主要针对cmt设备，因为cmt有多个台面，每个台面分别对应不同设备编号
                !string.IsNullOrEmpty(pda.Face) && //不为空的情况下才判断相等
                !string.Equals(pda.Face, pdaDto.face, StringComparison.OrdinalIgnoreCase))
            {
                return new ReturnDto()
                {
                    code = -1,
                    msg = $"操作错误，CMT工作台面选择错误，应为{pda.Face}！"
                };

            }

            string Barcode = pda.Barcode;
            string deviceCode = device.DeviceCode;



            //CMT 要通过工作台面来区分设备编号，因为CMT有多个工作台面，每个工作台面对应不同的设备编号，所以强制完成时需要根据工作台面来获取正确的设备编号，以便发送给MES
            if (device.DeviceType == "CMT")
            {
                JObject jobj = JObject.Parse(device.Face);

                deviceCode = jobj[pdaDto.face.ToUpper()]?["DeviceCode"]?.ToString() ?? "";

            }


            if (pda.MesStatus == 1 &&
                !string.IsNullOrEmpty(Barcode) &&
                !string.Equals(Barcode, "0"))
            {


                //发送强制出站给MES

                var mesResp = await this.SendMesForceExit(pdaDto.id, Barcode, deviceCode, device.Name, device.DeviceType);


                if (mesResp == null || !mesResp.Success)
                {
                    return new ReturnDto()
                    {
                        code = -1,
                        msg = $"MES错误，{mesResp.Message}！"
                    };

                }

            }

            //清空对应PDA的条码
            await UpdatePdaCode(pdaDto.id, "0", "", null);

            //复位信号
            switch (device.DeviceType)
            {
                //CMT 不通过夹具控制，通过对应工作面的步控制
                case "CMT":
                    {
                        //var cmtMqttRpcClient = this._serviceProvider.GetRequiredKeyedService<CMTMqttRpcClient>(device.Name);
                        //var startSignalPoint = $"{pdaDto.face.ToUpper()}面工步";

                        ////写完成信号
                        //cmtMqttRpcClient.SendOKSingnal(startSignalPoint);
                    }
                    break;
                //CNC,FSW 通过夹具控制，需要将夹具信号复位
                default:
                    {
                        //未屏蔽信号
                        if (device.MaskSignal == 0)
                        {
                            SiemensS7Net siemensS7Net = this._serviceProvider.GetRequiredKeyedService<SiemensS7Net>(device.Name);
                            var writeRes = await siemensS7Net.WriteAsync(device.StartSignalPoint, false);
                            if (!writeRes.IsSuccess)
                            {
                                this._logger.LogWarning($"条码:{Barcode},PDA:{device.PdaNo},{device.DeviceType}设备{device.Name}({device.DeviceCode}) ,扫码完成信号:{device.StartSignalPoint},复位失败：{writeRes.ErrorCode},{writeRes.Message}");

                            }
                            else
                            {
                                this._logger.LogInformation($"条码:{Barcode},PDA:{device.PdaNo},{device.DeviceType}设备{device.Name}({device.DeviceCode}),扫码完成信号:{device.StartSignalPoint},复位");

                            }
                        }

                    }
                    break;

            }

            //停止 监视器
            await _mediator.Publish(new StopMonitorRequest()
            {
                PdaNo = pdaDto.id,
                Face = pdaDto.face,
                Device = device,

            });

            return new ReturnDto()
            {
                code = 0,
                msg = $"操作成功！"
            };

        }

        /// <summary>
        /// 完成加工进程
        /// </summary>
        /// <param name="barcode"></param>
        /// <param name="device"></param>
        /// <returns></returns>

        public async Task FinishProcess(string barcode, Device device)
        {
            await UpdatePdaCode(device.PdaNo, "0", "", null);
            //复位开启信号 一般就是对应devic表的start_singnal_point，CNC FSW 通过夹具控制，CMT通过对应工作面的步控制
            //CMT设备不通过夹具控制，device.MaskSignal 对CMT设备无效,
            if (device.DeviceType != "CMT" && device.MaskSignal == 0)
            {
                SiemensS7Net siemensS7Net = this._serviceProvider.GetRequiredKeyedService<SiemensS7Net>(device.Name);
                var writeRes = await siemensS7Net.WriteAsync(device.StartSignalPoint, false);
                if (!writeRes.IsSuccess)
                {
                    this._logger.LogWarning($"条码:{barcode},PDA:{device.PdaNo},{device.DeviceType}设备{device.Name}({device.DeviceCode}),扫码完成信号:{device.StartSignalPoint},复位失败：{writeRes.ErrorCode},{writeRes.Message}");
                    return;
                }

                this._logger.LogInformation($"条码:{barcode},PDA:{device.PdaNo},{device.DeviceType}设备{device.Name}({device.DeviceCode}),扫码完成信号:{device.StartSignalPoint},复位");


            }



        }

    }
}
