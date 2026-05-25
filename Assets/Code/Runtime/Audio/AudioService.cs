using System;
using System.Collections.Generic;
using TopDownRPG.Infrastructure;
using UnityEngine;
using UnityEngine.Audio;
using VContainer;

namespace TopDownRPG.Audio
{
    public class AudioService : MonoBehaviour, IAudioService
    {
        [Serializable]
        private class ChannelMixerRoute
        {
            [field: SerializeField] public AudioChannel Channel { get; private set; }
            [field: SerializeField] public AudioMixerGroup MixerGroup { get; private set; }
        }

        [Serializable]
        private class ChannelLimit
        {
            [field: SerializeField] public AudioChannel Channel { get; private set; }
            [field: SerializeField, Min(1)] public int MaxSimultaneous { get; private set; } = 8;
        }

        [SerializeField, Min(1)] private int initialPoolSize = 8;
        [SerializeField, Min(1)] private int maxPoolSize = 24;
        [SerializeField, Min(1)] private int defaultMaxSimultaneousPerClip = 4;
        [SerializeField] private List<ChannelMixerRoute> mixerRoutes = new();
        [SerializeField] private List<ChannelLimit> channelLimits = new();

        private readonly Dictionary<AudioChannel, AudioMixerGroup> mixerGroupsByChannel = new();
        private readonly Dictionary<AudioChannel, int> limitsByChannel = new();
        private readonly Dictionary<AudioChannel, float> channelVolumes = new();
        private readonly HashSet<AudioChannel> mutedChannels = new();

        private IGameLogger logger;
        private AudioEmitterPool emitterPool;

        [Inject]
        public void Construct(IGameLogger logger)
        {
            this.logger = logger;
        }

        private void Awake()
        {
            ValidatePoolSettings();
            BuildMixerLookup();
            BuildLimitLookup();
            emitterPool = new AudioEmitterPool(transform, initialPoolSize, maxPoolSize);
        }

        private void Update()
        {
            emitterPool?.ReleaseCompletedEmitters();
        }

        private void OnValidate()
        {
            ValidatePoolSettings();
        }

        public void PlaySound(AudioClip clip, AudioChannel channel = AudioChannel.Master, float volume = 1f, float pitch = 1f)
        {
            Play(clip, transform.position, channel, volume, pitch, 0f);
        }

        public void PlaySoundAt(AudioClip clip, Vector3 position, AudioChannel channel = AudioChannel.Master, float volume = 1f, float pitch = 1f)
        {
            Play(clip, position, channel, volume, pitch, 1f);
        }

        public void StopChannel(AudioChannel channel)
        {
            emitterPool?.StopChannel(channel);
        }

        public void SetChannelVolume(AudioChannel channel, float volume)
        {
            channelVolumes[channel] = Mathf.Clamp01(volume);
            RefreshActiveVolumes(channel);
        }

        public void SetChannelMuted(AudioChannel channel, bool isMuted)
        {
            if (isMuted)
            {
                mutedChannels.Add(channel);
            }
            else
            {
                mutedChannels.Remove(channel);
            }

            RefreshActiveVolumes(channel);
        }

        private void Play(AudioClip clip, Vector3 position, AudioChannel channel, float volume, float pitch, float spatialBlend)
        {
            if (clip == null)
            {
                logger?.Warning("Audio playback ignored because no clip was provided.");
                return;
            }

            EnsureEmitterPool();
            emitterPool.ReleaseCompletedEmitters();

            if (IsClipLimitReached(clip) || IsChannelLimitReached(channel))
            {
                return;
            }

            var emitter = emitterPool.GetEmitter();

            emitter.Clip = clip;
            emitter.Channel = channel;
            emitter.Volume = Mathf.Clamp01(volume);

            var source = emitter.Source;

            source.transform.position = position;
            source.clip = clip;
            source.outputAudioMixerGroup = GetMixerGroup(channel);
            source.volume = GetEffectiveVolume(channel, emitter.Volume);
            source.pitch = Mathf.Max(0.01f, pitch);
            source.spatialBlend = Mathf.Clamp01(spatialBlend);
            source.loop = false;
            source.Play();

            emitterPool.MarkStarted(emitter);
        }

        private void BuildMixerLookup()
        {
            mixerGroupsByChannel.Clear();

            foreach (var route in mixerRoutes)
            {
                if (route?.MixerGroup == null)
                {
                    continue;
                }

                if (mixerGroupsByChannel.ContainsKey(route.Channel))
                {
                    LogWarning($"Duplicate mixer route for audio channel '{route.Channel}' will use the last configured mixer group.");
                }

                mixerGroupsByChannel[route.Channel] = route.MixerGroup;
            }
        }

        private void BuildLimitLookup()
        {
            limitsByChannel.Clear();

            foreach (var limit in channelLimits)
            {
                if (limit == null)
                {
                    continue;
                }

                if (limitsByChannel.ContainsKey(limit.Channel))
                {
                    LogWarning($"Duplicate simultaneous limit for audio channel '{limit.Channel}' will use the last configured value.");
                }

                limitsByChannel[limit.Channel] = limit.MaxSimultaneous;
            }
        }

        private bool IsClipLimitReached(AudioClip clip)
        {
            return emitterPool.GetActiveCount(clip) >= defaultMaxSimultaneousPerClip;
        }

        private bool IsChannelLimitReached(AudioChannel channel)
        {
            var maxSimultaneous = limitsByChannel.TryGetValue(channel, out var value) ? value : 0;
            if (maxSimultaneous <= 0)
            {
                return false;
            }

            return emitterPool.GetActiveCount(channel) >= maxSimultaneous;
        }

        private AudioMixerGroup GetMixerGroup(AudioChannel channel)
        {
            if (mixerGroupsByChannel.TryGetValue(channel, out var channelGroup))
            {
                return channelGroup;
            }

            mixerGroupsByChannel.TryGetValue(AudioChannel.Master, out var masterGroup);
            return masterGroup;
        }

        private void RefreshActiveVolumes(AudioChannel channel)
        {
            if (emitterPool == null)
            {
                return;
            }

            foreach (var emitter in emitterPool.ActiveEmitters)
            {
                if (channel == AudioChannel.Master || emitter.Channel == channel)
                {
                    emitter.Source.volume = GetEffectiveVolume(emitter.Channel, emitter.Volume);
                }
            }
        }

        private float GetEffectiveVolume(AudioChannel channel, float volume)
        {
            if (mutedChannels.Contains(channel) || mutedChannels.Contains(AudioChannel.Master))
            {
                return 0f;
            }

            var channelVolume = channelVolumes.TryGetValue(channel, out var storedChannelVolume) ? storedChannelVolume : 1f;
            var masterVolume = channelVolumes.TryGetValue(AudioChannel.Master, out var storedMasterVolume) ? storedMasterVolume : 1f;

            return Mathf.Clamp01(volume) * channelVolume * masterVolume;
        }

        private void ValidatePoolSettings()
        {
            if (maxPoolSize < initialPoolSize)
            {
                maxPoolSize = initialPoolSize;
            }
        }

        private void EnsureEmitterPool()
        {
            if (emitterPool == null)
            {
                emitterPool = new AudioEmitterPool(transform, initialPoolSize, maxPoolSize);
            }
        }

        private void LogWarning(string message)
        {
            if (logger != null)
            {
                logger.Warning(message);
                return;
            }

            Debug.LogWarning(message, this);
        }
    }
}
