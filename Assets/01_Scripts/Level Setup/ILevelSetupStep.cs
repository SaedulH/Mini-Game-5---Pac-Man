using System.Threading.Tasks;

namespace CoreSystem
{
    public interface ILevelSetupStep
    {
        int Weight { get; }
        Task Run(LevelContext context);
    }
}

