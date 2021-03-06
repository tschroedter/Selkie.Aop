﻿using System;
using System.Diagnostics.CodeAnalysis;
using Castle.Windsor;

namespace Selkie.Aop.Example
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static void Main()
        {
            var container = new WindsorContainer();
            var installer = new Installer();

            var example = new Example(container,
                                      installer);

            example.Main();

            Console.ReadLine();

            example.Dispose();

            Environment.Exit(0);
        }
    }
}