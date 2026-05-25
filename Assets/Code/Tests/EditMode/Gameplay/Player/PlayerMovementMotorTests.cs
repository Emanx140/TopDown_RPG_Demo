using NUnit.Framework;
using TopDownRPG.Gameplay;
using UnityEngine;

namespace TopDownRPG.Tests.Gameplay.Player
{
    public sealed class PlayerMovementMotorTests
    {
        [Test]
        public void Tick_WithMoveInput_ReturnsHorizontalVelocity()
        {
            var motor = new PlayerMovementMotor(-30f, -2f);

            var frame = motor.Tick(Vector2.right, false, true, 4f, 8f, 0.1f);

            Assert.That(frame.Velocity.x, Is.EqualTo(4f));
            Assert.That(frame.Velocity.z, Is.Zero);
        }

        [Test]
        public void Tick_WhenSprinting_UsesSprintSpeed()
        {
            var motor = new PlayerMovementMotor(-30f, -2f);

            var frame = motor.Tick(Vector2.up, true, true, 4f, 8f, 0.1f);

            Assert.That(frame.Velocity.z, Is.EqualTo(8f));
        }

        [Test]
        public void Tick_WhenIdle_ReturnsNoFacingDirection()
        {
            var motor = new PlayerMovementMotor(-30f, -2f);

            var frame = motor.Tick(Vector2.zero, false, true, 4f, 8f, 0.1f);

            Assert.That(frame.FacingDirection.HasValue, Is.False);
        }

        [Test]
        public void Tick_WhenInputExceedsMagnitude_ClampsMovement()
        {
            var motor = new PlayerMovementMotor(-30f, -2f);

            var frame = motor.Tick(new Vector2(2f, 0f), false, true, 4f, 8f, 0.1f);

            Assert.That(frame.Velocity.x, Is.EqualTo(4f));
        }

        [Test]
        public void Tick_WhenAirborne_AppliesGravity()
        {
            var motor = new PlayerMovementMotor(-30f, -2f);

            var frame = motor.Tick(Vector2.zero, false, false, 4f, 8f, 0.5f);

            Assert.That(frame.Velocity.y, Is.EqualTo(-15f));
        }
    }
}
