using System.Threading;
using Cysharp.Threading.Tasks;

namespace TopDownRPG.Scenes
{
    public interface ISceneLoader
    {
        bool IsLoading { get; }

        UniTask LoadAsync(SceneReference scene, CancellationToken cancellationToken = default);
        UniTask LoadAdditiveAsync(SceneReference scene, CancellationToken cancellationToken = default);
    }
}
