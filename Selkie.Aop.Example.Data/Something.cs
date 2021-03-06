﻿using System;
using System.Diagnostics.CodeAnalysis;
using Castle.Core;
using Selkie.Aop.Aspects;
using Selkie.Windsor;

namespace Selkie.Aop.Example.Data
{
    [ExcludeFromCodeCoverage]
    [Interceptor(typeof( StatusAspect ))]
    [Interceptor(typeof( LogAspect ))]
    [Interceptor(typeof( MessageHandlerAspect ))]
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

        public void DoSomethingThrows(Record record)
        {
            Console.WriteLine("I'm doing something (record) and throw: " + record);
            throw new ArgumentException("Test");
        }

        [Status("**** Doing something (status)... ***")]
        public void DoSomethingStatus(string input)
        {
            Console.WriteLine("I'm doing something (status): " + input);
        }

        [Status("+++ Doing something (status)... +++")]
        public void DoSomethingStatus(Record record)
        {
            Console.WriteLine(record);
        }

        public int Property { get; set; }
    }
}