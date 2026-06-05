namespace YZV25UI.Dto
{
    public class ReturnDto
    {
        public int code { get; set; }
        public string msg { get; set; }
    }


    public class ReturnDataDto<T> : ReturnDto
    {
        public T data { get; set; }
    }   
}
