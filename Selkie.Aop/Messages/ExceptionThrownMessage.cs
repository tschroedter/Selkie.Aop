namespace Selkie.Aop.Messages
{
    public class ExceptionThrownMessage
    {
        public ExceptionInformation Exception { get; set; }
        public ExceptionInformation[] InnerExceptions { get; set; }
    }
}