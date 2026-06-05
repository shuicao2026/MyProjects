using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YZV25.Core.Dto;
using YZV25.Dal;

namespace YZV25.Core.Service
{
    public class MesSettingService
    {
        private readonly SqlServerDal sqlServerDal;
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment _environment;

        public MesSettingService(SqlServerDal sqlServerDal, IConfiguration configuration, IWebHostEnvironment environment)
        {
            this.sqlServerDal = sqlServerDal;
            this.configuration = configuration;
            this._environment = environment;
        }


        public async Task<MesSettingDto> GetSetting()
        {

            return new MesSettingDto()
            {
                Block = this.configuration["MES:Block"] == null ? null : JsonConvert.DeserializeObject<string[]>(this.configuration["MES:Block"].ToString()),

                Enable = Convert.ToBoolean(this.configuration["MES:Enable"]),
                Hostname = this.configuration["MES:Hostname"]

            };
        }


        public async Task Update(MesSettingDto mesSettingDto)
        {
            //修改内存部分
            if (!string.IsNullOrEmpty(mesSettingDto.Hostname) && !string.Equals(Convert.ToString(this.configuration["MES:Hostname"]), mesSettingDto.Hostname))
            {
                this.configuration["MES:Hostname"] = mesSettingDto.Hostname;
            }
            if (mesSettingDto.Enable != null)
            {
                this.configuration["MES:Enable"] = mesSettingDto.Enable.ToString();
            }

            if (mesSettingDto.Block != null)
            {
                this.configuration["MES:Block"] = JsonConvert.SerializeObject(mesSettingDto.Block);
            }



            //修改后写入文件
            var fileName = _environment.IsDevelopment()
              ? "appsettings.Development.json"
              : "appsettings.json";

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);


            var json = await File.ReadAllTextAsync(filePath);
            var jsonObject = JsonConvert.DeserializeObject<JObject>(json);

            jsonObject!["MES"]!["Hostname"] = this.configuration["MES:Hostname"];
            jsonObject!["MES"]!["Enable"] = this.configuration["MES:Enable"];
            jsonObject!["MES"]!["Block"] = this.configuration["MES:Block"];


            var updatedJson = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
            await File.WriteAllTextAsync(filePath, updatedJson);

            // 重新加载配置
            if (configuration is IConfigurationRoot configRoot)
            {
                configRoot.Reload();
            }


            return;






        }

    }
}
