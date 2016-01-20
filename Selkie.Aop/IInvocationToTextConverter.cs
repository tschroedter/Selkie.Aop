using Castle.DynamicProxy;
using JetBrains.Annotations;

namespace Selkie.Aop
{
    public interface IInvocationToTextConverter
    {
        string Convert([NotNull] IInvocation invocation);
    }
}