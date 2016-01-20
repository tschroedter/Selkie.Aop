using Castle.Core.Logging;
using JetBrains.Annotations;

namespace Selkie.Aop
{
    public interface ILoggerRepository
    {
        ILogger Get([NotNull] string name);
    }
}