using System.Collections.Concurrent;
using Castle.Core.Logging;
using Core2.Selkie.Windsor;
using JetBrains.Annotations;

namespace Core2.Selkie.Aop
{
    [ProjectComponent(Lifestyle.Singleton)]
    [UsedImplicitly]
    public class LoggerRepository : ILoggerRepository
    {
        public LoggerRepository([NotNull] ILoggerFactory loggerFactory)
        {
            m_LoggerFactory = loggerFactory;
        }

        private readonly ILoggerFactory m_LoggerFactory;

        private readonly ConcurrentDictionary <string, ILogger> m_Loggers =
            new ConcurrentDictionary <string, ILogger>();

        public ILogger Get(string name)
        {
            ILogger logger = m_Loggers.GetOrAdd(name,
                                                m_LoggerFactory.Create(name));

            return logger;
        }
    }
}