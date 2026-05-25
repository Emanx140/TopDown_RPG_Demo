using NUnit.Framework;
using TopDownRPG.Core;

namespace TopDownRPG.Tests.Core
{
    public sealed class TimerTests
    {
        [Test]
        public void Constructor_WithPositiveDuration_StartsRunning()
        {
            var timer = new Timer(2f);

            Assert.That(timer.IsRunning, Is.True);
            Assert.That(timer.RemainingSeconds, Is.EqualTo(2f));
        }

        [Test]
        public void Tick_WhenDurationReached_CompletesAndClampsRemainingTime()
        {
            var completedCount = 0;
            var timer = new Timer(1f);
            timer.Completed += () => completedCount++;

            timer.Tick(1.5f);

            Assert.That(timer.IsCompleted, Is.True);
            Assert.That(timer.RemainingSeconds, Is.Zero);
            Assert.That(completedCount, Is.EqualTo(1));
        }

        [Test]
        public void Tick_WithInterval_RaisesTickedForEachElapsedInterval()
        {
            var tickCount = 0;
            var timer = new Timer(1f, 0.25f);
            timer.Ticked += () => tickCount++;

            timer.Tick(0.75f);

            Assert.That(tickCount, Is.EqualTo(3));
        }

        [Test]
        public void Reset_AfterCompletion_RestartsTimer()
        {
            var timer = new Timer(1f);
            timer.Tick(1f);

            timer.Reset();

            Assert.That(timer.IsRunning, Is.True);
            Assert.That(timer.ElapsedSeconds, Is.Zero);
            Assert.That(timer.RemainingSeconds, Is.EqualTo(1f));
        }
    }
}
