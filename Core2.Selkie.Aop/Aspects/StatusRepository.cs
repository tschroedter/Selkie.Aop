using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core2.Selkie.Windsor;
using JetBrains.Annotations;

namespace Core2.Selkie.Aop.Aspects
{
    [ProjectComponent(Lifestyle.Singleton)]
    [UsedImplicitly]
    public class StatusRepository : IStatusRepository
    {
        private readonly ConcurrentDictionary <string, string> m_Dictionary =
            new ConcurrentDictionary <string, string>();

        public string Get(MethodInfo methodInfo)
        {
            string name = methodInfo.Name;
            // ReSharper disable once PossibleNullReferenceException
            string type = methodInfo.DeclaringType.FullName;
            string parameters = GetParametersTypes(methodInfo);
            string returnType = methodInfo.ReturnType.FullName;
            string id = $"{type}.{name}({parameters}):{returnType}";

            string text = m_Dictionary.GetOrAdd(id,
                                                Create(methodInfo));

            return text;
        }

        private static string GetParametersTypes(MethodInfo methodInfo)
        {
            ParameterInfo[] parameterInfos = methodInfo.GetParameters();

            IEnumerable <string> types = parameterInfos.Select(x => x.ParameterType.ToString());

            return string.Join(",",
                               types);
        }

        private string Create([NotNull] MethodInfo methodInfo)
        {
            IEnumerable <StatusAttribute> attributes = methodInfo.GetCustomAttributes <StatusAttribute>();
            IEnumerable <string> texts = attributes.Select(x => x.Text);

            string text = string.Join(",",
                                      texts);

            return text;
        }
    }
}