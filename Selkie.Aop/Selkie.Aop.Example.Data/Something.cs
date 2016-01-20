using System;
using Castle.Core;
using Selkie.Windsor;

namespace Selkie.Aop.Example.Data
{
    [Interceptor(typeof(LogAspect))] // todo replace with name
    [ProjectComponent(Lifestyle.Transient)]
    public class Something : ISomething
    {
        public int Augment(int input)
        {
            return input + 1;
        }

        public void DoSomething(string input)
        {
            Console.WriteLine("I'm doing something: " + input);
        }

        public void DoSomething(Record record)
        {
            Console.WriteLine(record);
        }

        public int Property
        {
            get;
            set;
        }
    }
}