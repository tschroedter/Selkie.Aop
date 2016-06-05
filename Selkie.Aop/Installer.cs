using System.Diagnostics.CodeAnalysis;
using Selkie.Windsor;

namespace Selkie.Aop
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