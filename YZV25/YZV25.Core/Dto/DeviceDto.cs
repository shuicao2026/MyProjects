using SqlSugar;

namespace YZV25.Dto
{
    public class DeviceDto
    {


        public int Id { get; set; }


        public int? PdaNo { get; set; }


        public string Name { get; set; }

        public string Title { get; set; }


        public string DeviceCode { get; set; }


        public string DeviceType { get; set; }


        public string Workshop { get; set; }


        public string StartSignalPoint { get; set; }


        public string StatusSignalPoint { get; set; }

        public string FinishSignalPoint { get; set; }
    }
}
