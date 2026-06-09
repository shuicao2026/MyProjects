namespace YZV25.Dto
{
    public class ReturnDto
    {
        public int code { get; set; }
        public string msg { get; set; }
    }


    public class ReturnDataDto<T> : ReturnDto
    {
        public int Count { get; set; }
        public T data { get; set; }
    }   
}
