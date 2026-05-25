using System.Collections.Generic;
using UnityEngine;

namespace TopDownRPG.Audio
{
    internal class AudioEmitter
    {
        public AudioSource Source;
        public AudioClip Clip;
        public AudioChannel Channel;
        public float Volume;
    }

    internal class AudioEmitterPool
    {
        private readonly Transform parent;
        private readonly int maxPoolSize;
        private readonly Stack<AudioEmitter> pooledEmitters = new();
        private readonly List<AudioEmitter> activeEmitters = new();
        private readonly Dictionary<AudioChannel, int> activeCountsByChannel = new();
        private readonly Dictionary<AudioClip, int> activeCountsByClip = new();

        public AudioEmitterPool(Transform parent, int initialPoolSize, int maxPoolSize)
        {
            this.parent = parent;
            this.maxPoolSize = maxPoolSize;

            for (var i = 0; i < initialPoolSize; i++)
            {
                pooledEmitters.Push(CreateEmitter());
            }
        }

        public IReadOnlyList<AudioEmitter> ActiveEmitters => activeEmitters;

        public int GetActiveCount(AudioClip clip)
        {
            return clip != null && activeCountsByClip.TryGetValue(clip, out var count) ? count : 0;
        }

        public int GetActiveCount(AudioChannel channel)
        {
            return activeCountsByChannel.TryGetValue(channel, out var count) ? count : 0;
        }

        public void MarkStarted(AudioEmitter emitter)
        {
            if (emitter.Clip != null)
            {
                activeCountsByClip.TryGetValue(emitter.Clip, out var clipCount);
                activeCountsByClip[emitter.Clip] = clipCount + 1;
            }

            activeCountsByChannel.TryGetValue(emitter.Channel, out var channelCount);
            activeCountsByChannel[emitter.Channel] = channelCount + 1;
        }

        public AudioEmitter GetEmitter()
        {
            if (pooledEmitters.Count > 0)
            {
                return Activate(pooledEmitters.Pop());
            }

            return Activate(activeEmitters.Count + pooledEmitters.Count < maxPoolSize ? CreateEmitter() : StealOldestEmitter());
        }

        public void ReleaseCompletedEmitters()
        {
            for (var i = activeEmitters.Count - 1; i >= 0; i--)
            {
                if (!activeEmitters[i].Source.isPlaying)
                {
                    ReleaseEmitterAt(i);
                }
            }
        }

        public void StopChannel(AudioChannel channel)
        {
            for (var i = activeEmitters.Count - 1; i >= 0; i--)
            {
                if (activeEmitters[i].Channel != channel)
                {
                    continue;
                }

                activeEmitters[i].Source.Stop();
                ReleaseEmitterAt(i);
            }
        }

        private AudioEmitter Activate(AudioEmitter emitter)
        {
            emitter.Source.gameObject.SetActive(true);
            activeEmitters.Add(emitter);
            return emitter;
        }

        private AudioEmitter CreateEmitter()
        {
            var emitterObject = new GameObject("Audio Emitter");
            emitterObject.transform.SetParent(parent);
            emitterObject.SetActive(false);

            var source = emitterObject.AddComponent<AudioSource>();
            source.playOnAwake = false;

            return new AudioEmitter
            {
                Source = source,
                Channel = AudioChannel.Master,
                Volume = 1f
            };
        }

        private AudioEmitter StealOldestEmitter()
        {
            var emitter = activeEmitters[0];
            activeEmitters.RemoveAt(0);
            emitter.Source.Stop();
            DecrementActiveCounts(emitter);
            return emitter;
        }

        private void ReleaseEmitterAt(int index)
        {
            var emitter = activeEmitters[index];
            activeEmitters.RemoveAt(index);
            DecrementActiveCounts(emitter);

            emitter.Source.clip = null;
            emitter.Source.outputAudioMixerGroup = null;
            emitter.Clip = null;
            emitter.Channel = AudioChannel.Master;
            emitter.Volume = 1f;
            emitter.Source.gameObject.SetActive(false);
            emitter.Source.transform.SetParent(parent);

            pooledEmitters.Push(emitter);
        }

        private void DecrementActiveCounts(AudioEmitter emitter)
        {
            if (emitter.Clip != null && activeCountsByClip.TryGetValue(emitter.Clip, out var clipCount))
            {
                if (clipCount <= 1)
                {
                    activeCountsByClip.Remove(emitter.Clip);
                }
                else
                {
                    activeCountsByClip[emitter.Clip] = clipCount - 1;
                }
            }

            if (!activeCountsByChannel.TryGetValue(emitter.Channel, out var channelCount))
            {
                return;
            }

            if (channelCount <= 1)
            {
                activeCountsByChannel.Remove(emitter.Channel);
            }
            else
            {
                activeCountsByChannel[emitter.Channel] = channelCount - 1;
            }
        }
    }
}
