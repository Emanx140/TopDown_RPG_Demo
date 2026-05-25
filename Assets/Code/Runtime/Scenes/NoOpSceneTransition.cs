using System.Threading;
using Cysharp.Threading.Tasks;

namespace TopDownRPG.Scenes
{
    public sealed class NoOpSceneTransition : ISceneTransition
    {
        public UniTask FadeOutAsync(CancellationToken cancellationToken = default)
        {
            return UniTask.CompletedTask;
        }

        public UniTask FadeInAsync(CancellationToken cancellationToken = default)
        {
            return UniTask.CompletedTask;
        }
    }
}
