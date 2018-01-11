using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Core2.Selkie.Windsor;

[assembly: InternalsVisibleTo("Core2.Selkie.Aop.Tests")]

namespace Core2.Selkie.Aop
{
    [ExcludeFromCodeCoverage]
    public class Installer : BaseInstaller <Installer>
    {
        public override bool IsAutoDetectAllowedForAssemblyName(string assemblyName)
        {
            return assemblyName.StartsWith("Core2.Selkie.",
                                           StringComparison.InvariantCulture);
        }
    }
}