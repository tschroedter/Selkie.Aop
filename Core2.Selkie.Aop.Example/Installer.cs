using System;
using System.Diagnostics.CodeAnalysis;
using Castle.MicroKernel.Registration;
using Core2.Selkie.Windsor;

namespace Core2.Selkie.Aop.Example
{
    [ExcludeFromCodeCoverage]
    public class Installer
        : BasicConsoleInstaller,
          IWindsorInstaller
    {
        public override bool IsAutoDetectAllowedForAssemblyName(string assemblyName)
        {
            return assemblyName.StartsWith("Core2.Selkie.",
                                           StringComparison.InvariantCulture);
        }
    }
}