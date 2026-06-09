
using HslCommunication.MQTT;
using HslCommunication.Profinet.Siemens;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using Serilog.Sinks.SystemConsole.Themes;
using SqlSugar;
using System.Reflection;
using YZV25.Core.Monitor;
using YZV25.Core.Service;
using YZV25.Core.Utils;
using YZV25.Dal;
using YZV25.Dto;
using YZV25.Monitor;
using YZV25.Service;
using YZV25.Utils;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace YZV25
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.


            //日志配置
            builder.Host.UseSerilog((ctx, cfg) =>
            {

                var columnOptions = new ColumnOptions();

                // 从 Store 中移除 MessageTemplate（以及可选的 Properties）
                columnOptions.Store.Remove(StandardColumn.MessageTemplate);
                columnOptions.Store.Remove(StandardColumn.Properties); // 可选，Properties 那列(xml/json)也常删

                cfg
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                 .WriteTo.Console(
                    theme: AnsiConsoleTheme.Code,
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} <s:{SourceContext}>{NewLine}{Exception}"
                 )
                .WriteTo.MSSqlServer(
                    connectionString: ctx.Configuration.GetConnectionString("DefaultConnection"),
                    sinkOptions: new MSSqlServerSinkOptions
                    {

                        TableName = "AppLogs",
                        AutoCreateSqlTable = true,
                        BatchPostingLimit = 50,
                        BatchPeriod = TimeSpan.FromSeconds(5)

                    },
                    columnOptions: columnOptions);
            });




            // MQTT客户端，假设是一个MQTT客户端，可以根据实际情况进行调整
            builder.Services.AddTransient<MqttClient>(provider =>
            {

                return new MqttClient(new MqttConnectionOptions()
                {
                    IpAddress = "127.0.0.1",
                    Port = 1521,
                    Credentials = new MqttCredential("admin", "123456"),   // 设置了用户名和密码
                    ConnectTimeout = 2000
                });
            });

            // MQTT同步客户端，假设是一个MQTT客户端，可以根据实际情况进行调整
            builder.Services.AddTransient<MqttSyncClient>(provider =>
            {

                return new MqttSyncClient(new MqttConnectionOptions()
                {
                    IpAddress = "127.0.0.1",
                    Port = 1521,
                    Credentials = new MqttCredential("admin", "123456"),   // 设置了用户名和密码
                    ConnectTimeout = 2000
                });
            });

            //mes客户端，假设是一个HTTP客户端，可以根据实际情况进行调整
            builder.Services.AddTransient<MesRpcClient>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<MesRpcClient>>();
                var config = provider.GetRequiredService<IConfiguration>();
                ;
                //"http://101.226.8.125:2030/

                return new MesRpcClient(config["MES:Hostname"]!.ToString(), config, logger);
            });



            //数据访问层，
            builder.Services.AddTransient<SqlServerDal>();


            //fanuc客户端，用于读取cnc设备的信息
            builder.Services.AddKeyedTransient<CNCFanucClient>("cnc1", (provider, key) => new CNCFanucClient("192.168.106.70", 8193));
            builder.Services.AddKeyedTransient<CNCFanucClient>("cnc2", (provider, key) => new CNCFanucClient("192.168.106.80", 8193));
            builder.Services.AddKeyedTransient<CNCFanucClient>("cnc3", (provider, key) => new CNCFanucClient("192.168.106.90", 8193));
            builder.Services.AddKeyedTransient<CNCFanucClient>("cnc4", (provider, key) => new CNCFanucClient("192.168.106.100", 8193));
            builder.Services.AddKeyedTransient<CNCFanucClient>("cnc5", (provider, key) => new CNCFanucClient("192.168.106.110", 8193));
            builder.Services.AddKeyedTransient<CNCFanucClient>("cnc6", (provider, key) => new CNCFanucClient("192.168.106.120", 8193));
            builder.Services.AddKeyedSingleton<CNCFanucClient>("cnc7", (provider, key) => new CNCFanucClient("192.168.106.130", 8193));
            builder.Services.AddKeyedSingleton<CNCFanucClient>("cnc8", (provider, key) => new CNCFanucClient("192.168.106.140", 8193));
            builder.Services.AddKeyedSingleton<CNCFanucClient>("cnc9", (provider, key) => new CNCFanucClient("192.168.106.150", 8193));
            //cnc夹具 s7客户端
            builder.Services.AddKeyedSingleton<SiemensS7Net>("cnc1", (provider, key) => new SiemensS7Net(SiemensPLCS.S1200, "192.168.106.75"));
            builder.Services.AddKeyedSingleton<SiemensS7Net>("cnc2", (provider, key) => new SiemensS7Net(SiemensPLCS.S1200, "192.168.106.85"));
            builder.Services.AddKeyedSingleton<SiemensS7Net>("cnc3", (provider, key) => new SiemensS7Net(SiemensPLCS.S1200, "192.168.106.95"));
            builder.Services.AddKeyedSingleton<SiemensS7Net>("cnc4", (provider, key) => new SiemensS7Net(SiemensPLCS.S1200, "192.168.106.105"));
            builder.Services.AddKeyedSingleton<SiemensS7Net>("cnc5", (provider, key) => new SiemensS7Net(SiemensPLCS.S1200, "192.168.106.115"));
            builder.Services.AddKeyedSingleton<SiemensS7Net>("cnc6", (provider, key) => new SiemensS7Net(SiemensPLCS.S1200, "192.168.106.125"));
            builder.Services.AddKeyedSingleton<SiemensS7Net>("cnc7", (provider, key) => new SiemensS7Net(SiemensPLCS.S1200, "192.168.106.135"));
            builder.Services.AddKeyedSingleton<SiemensS7Net>("cnc8", (provider, key) => new SiemensS7Net(SiemensPLCS.S1200, "192.168.106.145"));
            builder.Services.AddKeyedSingleton<SiemensS7Net>("cnc9", (provider, key) => new SiemensS7Net(SiemensPLCS.S1200, "192.168.106.155"));



            //fsw opcua客户端，用于读取fsw设备的信息
            builder.Services.AddKeyedSingleton<FSWOpcUaClient>("fsw1", (provider, key) => new FSWOpcUaClient("opc.tcp://192.168.106.160:4840"));
            //fsw夹具  s7客户端
            builder.Services.AddKeyedSingleton<SiemensS7Net>("fsw1", (provider, key) => new SiemensS7Net(SiemensPLCS.S1200, "192.168.106.165"));

            //cmt mqtt客户端 用于读取cmt设备数据

            builder.Services.AddKeyedSingleton<CMTMqttRpcClient>("cmt2", (provider, key) =>
            {

                return new CMTMqttRpcClient("cmt2", "127.0.0.1", 1521, new string[] { "A", "B" });
            });


            builder.Services.AddKeyedTransient<CMTMcClient>("cmt1", (provider, key) =>
            {
                return new CMTMcClient("192.168.106.10", 5559, ["A", "B"]);
            });

            builder.Services.AddKeyedTransient<CMTMcClient>("cmt3", (provider, key) =>
            {
                return new CMTMcClient("192.168.106.50", 5559, ["A", "B","C"]);
            });

            builder.Services.AddKeyedSingleton<IMonitor>("cmt1", (provider, key) =>
            {
                var mesRpcClient = provider.GetRequiredService<MesRpcClient>();
                var mqttClient = provider.GetRequiredService<MqttClient>();
                var cmtMqttRpcClient = provider.GetRequiredKeyedService<CMTMcClient>("cmt1");
                var logger = provider.GetRequiredService<ILogger<CMTMonitor>>();
                var mediator = provider.GetRequiredService<IMediator>();
                return new CMTMcMonitor(
                    "cmt1", new string[] { }, mesRpcClient, mqttClient, cmtMqttRpcClient, mediator, logger
                    );

            });

            builder.Services.AddKeyedSingleton<IMonitor>("cmt3", (provider, key) =>
            {
                var mesRpcClient = provider.GetRequiredService<MesRpcClient>();
                var mqttClient = provider.GetRequiredService<MqttClient>();
                var cmtMqttRpcClient = provider.GetRequiredKeyedService<CMTMcClient>("cmt3");
                var logger = provider.GetRequiredService<ILogger<CMTMonitor>>();
                var mediator = provider.GetRequiredService<IMediator>();
                return new CMTMcMonitor(
                    "cmt1", new string[] {}, mesRpcClient, mqttClient, cmtMqttRpcClient, mediator, logger
                    );

            });

            //CMT 监听器 1台 西门子
            builder.Services.AddKeyedSingleton<IMonitor>("cmt2", (provider, key) =>
            {
                var mesRpcClient = provider.GetRequiredService<MesRpcClient>();
                var mqttClient = provider.GetRequiredService<MqttClient>();
                var cmtMqttRpcClient = provider.GetRequiredKeyedService<CMTMqttRpcClient>("cmt2");
                var logger = provider.GetRequiredService<ILogger<CMTMonitor>>();
                var mediator = provider.GetRequiredService<IMediator>();
                return new CMTMonitor(
                    "cmt2", new string[] {
                    "B面工步","A面工步"
                    }, mesRpcClient, mqttClient, cmtMqttRpcClient, mediator, logger
                    );

            });


            //CNC 监听器 9台
            builder.Services.AddKeyedSingleton<IMonitor>("cnc1", (provider, key) =>
            {
                var mesRpcClient = provider.GetRequiredService<MesRpcClient>();
                var mqttClient = provider.GetRequiredService<MqttClient>();
                var cncFanucClient = provider.GetRequiredKeyedService<CNCFanucClient>("cnc1");
                var logger = provider.GetRequiredService<ILogger<CNCMonitor>>();
                var mediator = provider.GetRequiredService<IMediator>();
                return new CNCMonitor(
                    "cnc1", new string[]
                    {
                        "加工完成"
                    }, mesRpcClient, mqttClient, cncFanucClient, mediator, logger
                    );
            });
            builder.Services.AddKeyedSingleton<IMonitor>("cnc2", (provider, key) =>
            {
                var mesRpcClient = provider.GetRequiredService<MesRpcClient>();
                var mqttClient = provider.GetRequiredService<MqttClient>();
                var cncFanucClient = provider.GetRequiredKeyedService<CNCFanucClient>("cnc2");
                var logger = provider.GetRequiredService<ILogger<CNCMonitor>>();
                var mediator = provider.GetRequiredService<IMediator>();
                return new CNCMonitor(
                    "cnc2",
                      new string[]
                    {
                        "加工完成"
                    }, mesRpcClient, mqttClient, cncFanucClient, mediator, logger
                    );
            });
            builder.Services.AddKeyedSingleton<IMonitor>("cnc3", (provider, key) =>
            {
                var mesRpcClient = provider.GetRequiredService<MesRpcClient>();
                var mqttClient = provider.GetRequiredService<MqttClient>();
                var cncFanucClient = provider.GetRequiredKeyedService<CNCFanucClient>("cnc3");
                var logger = provider.GetRequiredService<ILogger<CNCMonitor>>();
                var mediator = provider.GetRequiredService<IMediator>();
                return new CNCMonitor(
                    "cnc3",
                      new string[]
                    {
                        "加工完成"
                    }, mesRpcClient, mqttClient, cncFanucClient, mediator, logger
                    );
            });
            builder.Services.AddKeyedSingleton<IMonitor>("cnc4", (provider, key) =>
            {
                var mesRpcClient = provider.GetRequiredService<MesRpcClient>();
                var mqttClient = provider.GetRequiredService<MqttClient>();
                var cncFanucClient = provider.GetRequiredKeyedService<CNCFanucClient>("cnc4");
                var logger = provider.GetRequiredService<ILogger<CNCMonitor>>();
                var mediator = provider.GetRequiredService<IMediator>();
                return new CNCMonitor(
                    "cnc4",
                      new string[]
                    {
                        "加工完成"
                    }, mesRpcClient, mqttClient, cncFanucClient, mediator, logger
                    );
            });
            builder.Services.AddKeyedSingleton<IMonitor>("cnc5", (provider, key) =>
            {
                var mesRpcClient = provider.GetRequiredService<MesRpcClient>();
                var mqttClient = provider.GetRequiredService<MqttClient>();
                var cncFanucClient = provider.GetRequiredKeyedService<CNCFanucClient>("cnc5");
                var logger = provider.GetRequiredService<ILogger<CNCMonitor>>();
                var mediator = provider.GetRequiredService<IMediator>();
                return new CNCMonitor(
                    "cnc5",
                      new string[]
                    {
                        "加工完成"
                    }, mesRpcClient, mqttClient, cncFanucClient, mediator, logger
                    );
            });
            builder.Services.AddKeyedSingleton<IMonitor>("cnc6", (provider, key) =>
            {
                var mesRpcClient = provider.GetRequiredService<MesRpcClient>();
                var mqttClient = provider.GetRequiredService<MqttClient>();
                var cncFanucClient = provider.GetRequiredKeyedService<CNCFanucClient>("cnc6");
                var logger = provider.GetRequiredService<ILogger<CNCMonitor>>();
                var mediator = provider.GetRequiredService<IMediator>();
                return new CNCMonitor(
                    "cnc6",
                      new string[]
                    {
                        "加工完成"
                    }, mesRpcClient, mqttClient, cncFanucClient, mediator, logger
                    );
            });
            builder.Services.AddKeyedSingleton<IMonitor>("cnc7", (provider, key) =>
            {
                var mesRpcClient = provider.GetRequiredService<MesRpcClient>();
                var mqttClient = provider.GetRequiredService<MqttClient>();
                var cncFanucClient = provider.GetRequiredKeyedService<CNCFanucClient>("cnc7");
                var logger = provider.GetRequiredService<ILogger<CNCMonitor>>();
                var mediator = provider.GetRequiredService<IMediator>();

                return new CNCMonitor(
                    "cnc7",
                      new string[]
                    {
                        "加工完成"
                    }, mesRpcClient, mqttClient, cncFanucClient, mediator, logger
                    );
            });
            builder.Services.AddKeyedSingleton<IMonitor>("cnc8", (provider, key) =>
            {
                var mesRpcClient = provider.GetRequiredService<MesRpcClient>();
                var mqttClient = provider.GetRequiredService<MqttClient>();
                var cncFanucClient = provider.GetRequiredKeyedService<CNCFanucClient>("cnc8");
                var logger = provider.GetRequiredService<ILogger<CNCMonitor>>();
                var mediator = provider.GetRequiredService<IMediator>();
                return new CNCMonitor(
                    "cnc8",
                      new string[]
                    {
                        "加工完成"
                    }, mesRpcClient, mqttClient, cncFanucClient, mediator, logger
                    );
            });
            builder.Services.AddKeyedSingleton<IMonitor>("cnc9", (provider, key) =>
            {
                var mesRpcClient = provider.GetRequiredService<MesRpcClient>();
                var mqttClient = provider.GetRequiredService<MqttClient>();
                var cncFanucClient = provider.GetRequiredKeyedService<CNCFanucClient>("cnc9");
                var logger = provider.GetRequiredService<ILogger<CNCMonitor>>();
                var mediator = provider.GetRequiredService<IMediator>();
                return new CNCMonitor(
                    "cnc9",
                      new string[]
                    {
                        "加工完成"
                    }, mesRpcClient, mqttClient, cncFanucClient, mediator, logger
                    );
            });

            // FSW 监听器 1台 
            builder.Services.AddKeyedSingleton<IMonitor>("fsw1", (provider, key) =>
            {

                var mesRpcClient = provider.GetRequiredService<MesRpcClient>();
                var mqttClient = provider.GetRequiredService<MqttClient>();
                var opcUaClient = provider.GetRequiredKeyedService<FSWOpcUaClient>("fsw1");
                var logger = provider.GetRequiredService<ILogger<FSWMonitor>>();
                var mediator = provider.GetRequiredService<IMediator>();
                return new FSWMonitor(
                    "fsw1",
                    new string[] {
                    "加工完成"
                    }, mesRpcClient, mqttClient, opcUaClient, mediator, logger
                    );

            });



            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            builder.Services.AddScoped<LogQueryService>();
            builder.Services.AddScoped<ScanService>();
            builder.Services.AddScoped<DeviceService>();
            builder.Services.AddScoped<BusinessLogService>();
            builder.Services.AddScoped<MesSettingService>();

            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.WebHost.UseUrls("http://0.0.0.0:6060");


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();


        }
    }
}
