using JetBrains.Annotations;

namespace Selkie.Aop.Messages
{
    [UsedImplicitly]
    public class ExceptionThrownMessage
    {
        [UsedImplicitly]
        public ExceptionInformation Exception { get; set; }

        [UsedImplicitly]
        public ExceptionInformation[] InnerExceptions { get; set; }
    }
}