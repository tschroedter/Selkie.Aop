using System.Diagnostics.CodeAnalysis;
using Selkie.Windsor;

namespace Selkie.Aop
{
    [ExcludeFromCodeCoverage]
    //ncrunch: no coverage start
    public class Installer : BaseInstaller <Installer>
    {
        public override string GetPrefixOfDllsToInstall()
        {
            return "Selkie.";
        }
    }
}