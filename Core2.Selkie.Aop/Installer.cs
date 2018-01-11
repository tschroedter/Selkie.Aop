using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Core2.Selkie.Windsor;
using JetBrains.Annotations;

[assembly: InternalsVisibleTo("Core2.Selkie.Aop.Tests")]

namespace Core2.Selkie.Aop
{
    [ExcludeFromCodeCoverage]
    [UsedImplicitly]
    public class Installer : BaseInstaller <Installer>
    {
        public override bool IsAutoDetectAllowedForAssemblyName(string assemblyName)
        {
            return assemblyName.StartsWith("Core2.Selkie.",
                                           StringComparison.InvariantCulture);
        }
    }
}