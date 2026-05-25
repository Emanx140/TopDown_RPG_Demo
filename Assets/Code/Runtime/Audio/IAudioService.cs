using UnityEngine;

namespace TopDownRPG.Audio
{
    public interface IAudioService
    {
        void PlaySound(AudioClip clip, AudioChannel channel = AudioChannel.Master, float volume = 1f, float pitch = 1f);
        void PlaySoundAt(AudioClip clip, Vector3 position, AudioChannel channel = AudioChannel.Master, float volume = 1f, float pitch = 1f);
        void StopChannel(AudioChannel channel);
        void SetChannelVolume(AudioChannel channel, float volume);
        void SetChannelMuted(AudioChannel channel, bool isMuted);
    }
}
