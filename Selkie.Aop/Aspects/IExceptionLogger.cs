using System;
using Castle.DynamicProxy;

namespace Selkie.Aop.Aspects
{
    public interface IExceptionLogger
    {
        void LogException(IInvocation invocation,
                          Exception exception);
    }
}