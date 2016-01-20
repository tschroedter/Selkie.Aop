namespace Selkie.Aop.Messages
{
    public class ExceptionThrownMessage
    {
        public string Invocation { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }
}