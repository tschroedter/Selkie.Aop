using System;

namespace Core2.Selkie.Aop.Aspects
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class StatusAttribute
        : Attribute,
          IStatusInfo
    {
        public StatusAttribute(string text)
        {
            Text = text;
        }

        public string Text { get; private set; }
    }
}