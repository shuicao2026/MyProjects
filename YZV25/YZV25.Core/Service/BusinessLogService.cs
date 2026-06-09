using Newtonsoft.Json;
using System.Runtime.InteropServices;
using YZV25.Core.Dto;
using YZV25.Dal;
using YZV25.Dto;
using YZV25.Entity;
using YZV25.Service;

namespace YZV25.Core.Service
{
    public class BusinessLogService
    {
        SqlServerDal _sqlServerDal;
        ILogger<ScanService> _logger;
        public BusinessLogService(SqlServerDal sqlServerDal, ILogger<ScanService> logger)
        {


            this._sqlServerDal = sqlServerDal;
            this._logger = logger;

        }

        public async Task<List<BusinessLogDto>> GetLastLogsAsync(int lastId)
        {

            List<BusinessLog> logs = null;
            if (lastId <= 0)
            {
                logs = await this._sqlServerDal.GetDb().Queryable<BusinessLog>().OrderByDescending(a => a.OperationTimestamp).Take(50).ToListAsync();

            }
            else
            {
                logs = await this._sqlServerDal.GetDb().Queryable<BusinessLog>().OrderByDescending(a => a.OperationTimestamp).Where(log => log.Id > lastId).Take(50).ToListAsync();
            }




            var result = logs.Select(log => new BusinessLogDto()
            {
                Id = log.Id,
                PdaNo = log.PdaNo,
                Barcode = log.Barcode,
                DeviceName = log.DeviceName,
                DeviceCode = log.DeviceCode,
                MesOperation = log.MesOperation,
                MesReturnContent = log.MesReturnContent,
                MesEnabledStatus = log.MesEnabledStatus,
                CreateTime = log.CreateTime,
                OperationTimestamp = log.OperationTimestamp,
                MesResult = log.MesResult,
                Payload = log.Payload
            }).ToList();

            return result;
        }





        public async Task<int> GetCount(string text)
        {
            return await this._sqlServerDal.GetDb()
                  .Queryable<BusinessLog>()
                  .CountAsync(log => log.Barcode.Contains(text) ||
                  log.DeviceCode.Contains(text) ||
                  log.DeviceName.Contains(text));

        }

        public async Task<List<BusinessLogDto>> QueryLogsAsync(string text, int pageIndex, int pageSize)
        {

            var logs = await this._sqlServerDal.GetDb()
                .Queryable<BusinessLog>()
                .Where(log => log.Barcode.Contains(text) ||
                log.DeviceCode.Contains(text) ||
                log.DeviceName.Contains(text))
                .OrderByDescending(a => a.OperationTimestamp)
                .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            var result = logs.Select(log => new BusinessLogDto()
            {
                Id = log.Id,
                PdaNo = log.PdaNo,
                Barcode = log.Barcode,
                DeviceName = log.DeviceName,
                DeviceCode = log.DeviceCode,
                MesOperation = log.MesOperation,
                MesReturnContent = log.MesReturnContent,
                MesEnabledStatus = log.MesEnabledStatus,
                CreateTime = log.CreateTime,
                OperationTimestamp = log.OperationTimestamp,
                MesResult = log.MesResult,
                Payload = log.Payload
            }).ToList();
            return result;

        }
       
        
        /// <summary>
        /// 记入给mes发送进站数据
        /// </summary>
        /// <param name="pdaNo"></param>
        /// <param name="barCode"></param>
        /// <param name="deviceType"></param>
        /// <param name="deviceName"></param>
        /// <param name="deviceCode"></param>
        /// <param name="mesEnableStatus"></param>
        /// <param name="mesResult"></param>
        /// <param name="mesMessage"></param>
        /// <returns></returns>
        public async Task LogMesEnter(int pdaNo, string barCode, string deviceType, string deviceName, string deviceCode,
            bool mesEnableStatus, bool mesResult, string mesMessage)
        {
            if (mesResult)
            {
                this._logger.LogInformation($"条码:{barCode},PDA:{pdaNo},{deviceType}设备{deviceName}({deviceCode}),MES进站成功");

            }
            else
            {
                this._logger.LogInformation($"条码:{barCode},PDA:{pdaNo},{deviceType}设备{deviceName}({deviceCode}),MES进站失败：{mesMessage}");

            }

            BusinessLog businessLog = new BusinessLog()
            {

                PdaNo = pdaNo,
                Barcode = barCode,
                DeviceCode = deviceCode,
                DeviceName = deviceName,
                MesEnabledStatus = mesEnableStatus,
                MesOperation = "进站",

                MesReturnContent = mesMessage,
                MesResult = mesResult,
                OperationTimestamp = DateTime.Now,

            };
            await this._sqlServerDal.GetDb().Insertable<BusinessLog>(businessLog).IgnoreColumns(it => new { it.CreateTime }).ExecuteCommandAsync();



        }


        /// <summary>
        /// 计入给mes发送出站数据
        /// </summary>
        /// <param name="pdaNo"></param>
        /// <param name="barCode"></param>
        /// <param name="deviceType"></param>
        /// <param name="deviceName"></param>
        /// <param name="deviceCode"></param>
        /// <param name="mesEnableStatus"></param>
        /// <param name="mesResult"></param>
        /// <param name="mesMessage"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task LogMesExit(int pdaNo, string barCode, string deviceType, string deviceName, string deviceCode,
            bool mesEnableStatus, bool mesResult, string mesMessage, object data)
        {
            var dataJson = JsonConvert.SerializeObject(data);
            if (mesResult)
            {
                this._logger.LogInformation($"条码:{barCode},PDA:{pdaNo},{deviceType}设备{deviceName}({deviceCode}),MES出站数据:{dataJson},MES返回成功");
            }
            else
            {
                this._logger.LogInformation($"条码:{barCode},PDA:{pdaNo},{deviceType}设备{deviceName}({deviceCode}),MES出站数据:{dataJson},MES返回错误:{mesMessage}");
            }
            BusinessLog businessLog = new BusinessLog()
            {
                PdaNo = pdaNo,
                Barcode = barCode,
                DeviceCode = deviceCode,
                DeviceName = deviceName,
                MesEnabledStatus = mesEnableStatus,
                MesOperation = "出站",
                MesReturnContent = mesMessage,
                MesResult = mesResult,
                OperationTimestamp = DateTime.Now,
                Payload = dataJson
            };
            await this._sqlServerDal.GetDb().Insertable<BusinessLog>(businessLog).IgnoreColumns(it => new { it.CreateTime }).ExecuteCommandAsync();
        }




        /// <summary>
        /// 记入给mes发送加工数据
        /// </summary>
        /// <param name="pdaNo"></param>
        /// <param name="barCode"></param>
        /// <param name="deviceType"></param>
        /// <param name="deviceName"></param>
        /// <param name="deviceCode"></param>
        /// <param name="mesEnableStatus"></param>
        /// <param name="mesResult"></param>
        /// <param name="mesMessage"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task LogMesWorking(int pdaNo, string barCode,
            string deviceType, string deviceName, string deviceCode,
            bool mesEnableStatus, bool mesResult, string mesMessage, object data)
        {
            var dataJson = JsonConvert.SerializeObject(data);
            if (mesResult)
            {

                this._logger.LogInformation($"条码:{barCode},PDA:{pdaNo},{deviceType}设备{deviceName}({deviceCode}),发送MES加工中数据:{dataJson},MES返回成功");
            }
            else
            {
                this._logger.LogInformation($"条码:{barCode},PDA:{pdaNo},{deviceType}设备{deviceName}({deviceCode}),发送MES加工中数据:{dataJson}，MES返回失败:{mesMessage}");
            }
            BusinessLog businessLog = new BusinessLog()
            {
                PdaNo = pdaNo,
                Barcode = barCode,
                DeviceCode = deviceCode,
                DeviceName = deviceName,
                MesEnabledStatus = mesEnableStatus,
                MesOperation = "加工数据",
                MesReturnContent = mesMessage,
                MesResult = mesResult,
                OperationTimestamp = DateTime.Now,
                Payload = dataJson
            };
            await this._sqlServerDal.GetDb().Insertable<BusinessLog>(businessLog).IgnoreColumns(it => new { it.CreateTime }).ExecuteCommandAsync();
        }


        /// <summary>
        /// 计入给mes发送设备状态数据
        /// </summary>
        /// <param name="pdaNo"></param>
        /// <param name="barCode"></param>
        /// <param name="deviceType"></param>
        /// <param name="deviceName"></param>
        /// <param name="deviceCode"></param>
        /// <param name="mesEnableStatus"></param>
        /// <param name="mesResult"></param>
        /// <param name="mesMessage"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task LogMesDeviceStatus(int pdaNo, string barCode,
            string deviceType, string deviceName, string deviceCode,
            bool mesEnableStatus, bool mesResult, string mesMessage, object data)
        {
            if (mesResult)
            {
                this._logger.LogInformation($"条码:{barCode},PDA:{pdaNo},{deviceType}设备{deviceName}({deviceCode}),发送MES状态数据:{data},MES返回成功");
            }
            else
            {
                this._logger.LogInformation($"条码:{barCode},PDA:{pdaNo},{deviceType}设备{deviceName}({deviceCode}),发送MES状态数据:{data},MES返回失败:{mesMessage}");
            }

            BusinessLog businessLog = new BusinessLog()
            {
                PdaNo = pdaNo,
                Barcode = barCode,
                DeviceCode = deviceCode,
                DeviceName = deviceName,
                MesEnabledStatus = mesEnableStatus,
                MesOperation = "状态数据",
                MesReturnContent = mesMessage,
                MesResult = mesResult,
                OperationTimestamp = DateTime.Now,
                Payload = data?.ToString() ?? ""
            };
            var ret = await this._sqlServerDal.GetDb().Insertable<BusinessLog>(businessLog)
                  .IgnoreColumns(it => new { it.CreateTime }).ExecuteCommandAsync();

        }
    }
}
