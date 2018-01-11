using Castle.DynamicProxy;
using JetBrains.Annotations;

namespace Core2.Selkie.Aop
{
    public interface IInvocationToTextConverter
    {
        string Convert([NotNull] IInvocation invocation);
    }
}