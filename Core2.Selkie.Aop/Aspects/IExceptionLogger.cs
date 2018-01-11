using System;
using Castle.DynamicProxy;

namespace Core2.Selkie.Aop.Aspects
{
    public interface IExceptionLogger
    {
        void LogException(IInvocation invocation,
                          Exception exception);
    }
}