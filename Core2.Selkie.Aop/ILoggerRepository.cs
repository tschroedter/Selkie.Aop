using Castle.Core.Logging;
using JetBrains.Annotations;

namespace Core2.Selkie.Aop
{
    public interface ILoggerRepository // todo check if I can use ISelkieLogger instead of repository
    {
        ILogger Get([NotNull] string name);
    }
}