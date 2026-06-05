using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using System.Configuration;
using System.Text.Json.Nodes;
using YZV25.Core.Dto;
using YZV25.Dal;
using YZV25.Dto;
using YZV25.Entity;
using static System.Collections.Specialized.BitVector32;
namespace YZV25.Service
{
    public class DeviceService
    {
        private readonly SqlServerDal sqlServerDal;
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment _environment;

        public DeviceService(SqlServerDal sqlServerDal, IConfiguration configuration, IWebHostEnvironment environment)
        {
            this.sqlServerDal = sqlServerDal;
            this.configuration = configuration;
            this._environment = environment;
        }

        public async Task<List<DeviceDto>> GetAllDevices()
        {

            return await this.sqlServerDal.GetDb().Queryable<Device>().OrderBy(d=>d.PdaNo).Select(d => new DeviceDto()
            {
                Id = d.Id,
                Name = d.Name,
                PdaNo = d.PdaNo,
                DeviceType = d.DeviceType,
                DeviceCode = d.DeviceCode,
                Title = d.Title,
                Workshop = d.Workshop,
                StartSignalPoint = d.StartSignalPoint,
                StatusSignalPoint = d.StatusSignalPoint,
                FinishSignalPoint = d.FinishSignalPoint


            }).ToListAsync();


        }



    }
}
