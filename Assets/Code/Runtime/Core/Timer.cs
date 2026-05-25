using System;
using UnityEngine;

namespace TopDownRPG.Core
{
    public class Timer
    {
        public const float PermanentDurationThreshold = 10000f;

        private readonly float durationSeconds;
        private readonly float tickEverySeconds;

        private float elapsedSeconds;
        private float tickElapsedSeconds;

        public Timer(float durationSeconds, float tickEverySeconds = 0f)
        {
            this.durationSeconds = Mathf.Max(0f, durationSeconds);
            this.tickEverySeconds = Mathf.Max(0f, tickEverySeconds);
            IsCompleted = IsInstant;
        }

        public event Action Ticked;
        public event Action Completed;

        public float ElapsedSeconds => elapsedSeconds;
        public float DurationSeconds => durationSeconds;
        public float TickEverySeconds => tickEverySeconds;
        public float RemainingSeconds => IsPermanent ? float.PositiveInfinity : Mathf.Max(0f, durationSeconds - elapsedSeconds);
        public bool IsInstant => durationSeconds <= 0f;
        public bool IsPermanent => durationSeconds > PermanentDurationThreshold;
        public bool UsesTicks => tickEverySeconds > 0f;
        public bool IsCompleted { get; private set; }
        public bool IsRunning => !IsCompleted;

        public void Reset()
        {
            elapsedSeconds = 0f;
            tickElapsedSeconds = 0f;
            IsCompleted = IsInstant;
        }

        public void Complete()
        {
            if (IsCompleted)
            {
                return;
            }

            if (!IsPermanent)
            {
                elapsedSeconds = durationSeconds;
            }

            IsCompleted = true;
            Completed?.Invoke();
        }

        public void Tick(float deltaSeconds)
        {
            if (deltaSeconds <= 0f || IsCompleted)
            {
                return;
            }

            var tickDeltaSeconds = IsPermanent ? deltaSeconds : Mathf.Min(deltaSeconds, RemainingSeconds);
            elapsedSeconds += tickDeltaSeconds;
            TickInterval(tickDeltaSeconds);

            if (!IsPermanent && elapsedSeconds >= durationSeconds)
            {
                Complete();
            }
        }

        private void TickInterval(float deltaSeconds)
        {
            if (!UsesTicks)
            {
                return;
            }

            tickElapsedSeconds += deltaSeconds;

            while (tickElapsedSeconds >= tickEverySeconds && !IsCompleted)
            {
                tickElapsedSeconds -= tickEverySeconds;
                Ticked?.Invoke();
            }
        }
    }
}
