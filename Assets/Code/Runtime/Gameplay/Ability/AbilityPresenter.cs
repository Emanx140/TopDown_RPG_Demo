using TopDownRPG.Audio;
using UnityEngine;
using VContainer;

namespace TopDownRPG.Gameplay
{
    public class AbilityPresenter : MonoBehaviour
    {
        private IAudioService audioService;

        [Inject]
        public void Construct(IAudioService audioService)
        {
            this.audioService = audioService;
        }

        public void PlayCast(AbilitySo ability, Vector3 position)
        {
            PlayCast(ability, position, Quaternion.identity);
        }

        public void PlayCast(AbilitySo ability, Vector3 position, Quaternion rotation)
        {
            if (ability == null)
            {
                return;
            }

            PlaySound(ability, position);
            SpawnVfx(ability, position, rotation);
        }

        private void PlaySound(AbilitySo ability, Vector3 position)
        {
            if (ability.AudioClip == null || audioService == null)
            {
                return;
            }

            audioService.PlaySoundAt(
                ability.AudioClip,
                position,
                AudioChannel.Abilities);
        }

        private static void SpawnVfx(AbilitySo ability, Vector3 position, Quaternion rotation)
        {
            if (ability.VfxPrefab == null)
            {
                return;
            }

            Instantiate(ability.VfxPrefab, position, rotation);
        }
    }
}
