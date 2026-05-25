using System.Threading;
using Cysharp.Threading.Tasks;

namespace TopDownRPG.Scenes
{
    public interface ISceneTransition
    {
        UniTask FadeOutAsync(CancellationToken cancellationToken = default);
        UniTask FadeInAsync(CancellationToken cancellationToken = default);
    }
}
