using System.Diagnostics.CodeAnalysis;
using Selkie.Windsor;

namespace Selkie.Aop.Example.Data
{
    [ExcludeFromCodeCoverage]
    public class Installer : BaseInstaller <Installer>
    {
        public override string GetPrefixOfDllsToInstall()
        {
            return "Selkie.";
        }
    }
}