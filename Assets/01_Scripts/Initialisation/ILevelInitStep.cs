using System.Threading.Tasks;

namespace CoreSystem
{
    public interface ILevelInitStep
    {
        int Weight { get; }
        Task Run(GameManager gameManager,LevelContext context);
    }
}

