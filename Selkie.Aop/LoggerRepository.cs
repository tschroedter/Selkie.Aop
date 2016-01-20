using System.Collections.Concurrent;
using Castle.Core.Logging;
using JetBrains.Annotations;
using Selkie.Windsor;

namespace Selkie.Aop
{
    [ProjectComponent(Lifestyle.Singleton)]
    public class LoggerRepository : ILoggerRepository
    {
        private readonly ILoggerFactory m_LoggerFactory;

        private readonly ConcurrentDictionary <string, ILogger> m_Loggers =
            new ConcurrentDictionary <string, ILogger>();

        public LoggerRepository([NotNull] ILoggerFactory loggerFactory)
        {
            m_LoggerFactory = loggerFactory;
        }

        public ILogger Get(string name)
        {
            ILogger logger = m_Loggers.GetOrAdd(name,
                                                m_LoggerFactory.Create(name));

            return logger;
        }
    }
}