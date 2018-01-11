using System;
using System.Diagnostics.CodeAnalysis;
using Core2.Selkie.Windsor;

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