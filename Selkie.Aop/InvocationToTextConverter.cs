using System.Linq;
using System.Text;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Selkie.Windsor;
using Selkie.Windsor.Extensions;

namespace Selkie.Aop
{
    [ProjectComponent(Lifestyle.Transient)]
    public class InvocationToTextConverter : IInvocationToTextConverter
    {
        public string Convert(IInvocation invocation)
        {
            string arguments = ConvertArgumentsToString(invocation.Arguments);

            string called = "{0}.{1}({2})".Inject(invocation.TargetType.Name,
                                                  invocation.Method.Name,
                                                  arguments);

            return called;
        }

        internal string ConvertArgumentsToString([NotNull] object[] arguments)
        {
            var builder = new StringBuilder(100);

            foreach ( object argument in arguments )
            {
                string argumentDescription = argument == null
                                                 ? "null"
                                                 : DumpObject(argument);

                builder.Append(argumentDescription).Append(",");
            }

            if ( arguments.Any() )
            {
                builder.Length--;
            }

            return builder.ToString();
        }

        private string DumpObject(object argument)
        {
            string json = JsonConvert.SerializeObject(argument);

            return json;
        }
    }
}