using System;

namespace Selkie.Aop.Example.Data
{
    public interface ISomething
    {
        int Augment(Int32 input);
        void DoSomething(String input);
        int Property
        {
            get;
            set;
        }

        void DoSomething(Record record);
    }
}