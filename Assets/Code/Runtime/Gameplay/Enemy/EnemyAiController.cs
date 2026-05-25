using TopDownRPG.Core;
using UnityEngine;
using UnityEngine.AI;

namespace TopDownRPG.Gameplay
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(EnemyAbilityRunner))]
    public sealed class EnemyAiController : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private EnemyAbilityRunner abilityRunner;
        [SerializeField, Min(0f)] private float attackRange = 1.75f;
        [SerializeField, Min(0f)] private float repathInterval = 0.15f;
        [SerializeField, Min(0f)] private float rotationSpeed = 720f;

        private StateMachine stateMachine;
        private Transform targetTransform;
        private IDamageable targetDamageable;
        private float repathTimer;

        public bool HasTarget => targetTransform != null && targetDamageable != null;
        public bool IsTargetInAttackRange => HasTarget && GetPlanarDistanceToTargetSqr() <= attackRange * attackRange;

        private void Reset()
        {
            agent = GetComponent<NavMeshAgent>();
            abilityRunner = GetComponent<EnemyAbilityRunner>();
        }

        private void Awake()
        {
            if (agent == null)
            {
                agent = GetComponent<NavMeshAgent>();
            }

            if (abilityRunner == null)
            {
                abilityRunner = GetComponent<EnemyAbilityRunner>();
            }

            BuildStateMachine();
        }

        private void OnEnable()
        {
            if (HasTarget)
            {
                stateMachine?.ChangeState<EnemyChaseState>();
                return;
            }

            stateMachine?.ChangeState<EnemyIdleState>();
        }

        private void Update()
        {
            stateMachine?.Tick(Time.deltaTime);
        }

        public void AssignTarget(Transform target, IDamageable damageable)
        {
            targetTransform = target;
            targetDamageable = damageable;
            if (HasTarget)
            {
                stateMachine?.ChangeState<EnemyChaseState>();
                return;
            }

            stateMachine?.ChangeState<EnemyIdleState>();
        }

        public void ClearTarget()
        {
            targetTransform = null;
            targetDamageable = null;
            stateMachine?.ChangeState<EnemyIdleState>();
        }

        public void ResumeMoving()
        {
            if (!CanUseAgent())
            {
                return;
            }

            agent.stoppingDistance = attackRange;
            agent.isStopped = false;
        }

        public void StopMoving()
        {
            if (!CanUseAgent())
            {
                return;
            }

            agent.isStopped = true;
            agent.ResetPath();
        }

        public void ChaseTarget(float deltaSeconds)
        {
            if (!HasTarget || !CanUseAgent())
            {
                StopMoving();
                return;
            }

            repathTimer -= deltaSeconds;
            if (repathTimer > 0f)
            {
                return;
            }

            repathTimer = repathInterval;
            agent.SetDestination(targetTransform.position);
        }

        public void FaceTarget(float deltaSeconds)
        {
            if (!HasTarget)
            {
                return;
            }

            var toTarget = Vector3.ProjectOnPlane(targetTransform.position - transform.position, Vector3.up);
            if (toTarget.sqrMagnitude <= Mathf.Epsilon)
            {
                return;
            }

            var targetRotation = Quaternion.LookRotation(toTarget, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * deltaSeconds);
        }

        public bool TryAttackTarget()
        {
            if (!IsTargetInAttackRange || abilityRunner == null)
            {
                return false;
            }

            return abilityRunner.TryUseAnyReadyAbility(targetDamageable);
        }

        private void BuildStateMachine()
        {
            stateMachine = new StateMachine();
            stateMachine.AddState(new EnemyIdleState(this));
            stateMachine.AddState(new EnemyChaseState(this));
            stateMachine.AddState(new EnemyAttackState(this));

            stateMachine.AddTransition(new StateTransition(typeof(EnemyIdleState), typeof(EnemyChaseState), () => HasTarget));
            stateMachine.AddTransition(new StateTransition(typeof(EnemyChaseState), typeof(EnemyIdleState), () => !HasTarget));
            stateMachine.AddTransition(new StateTransition(typeof(EnemyChaseState), typeof(EnemyAttackState), () => IsTargetInAttackRange));
            stateMachine.AddTransition(new StateTransition(typeof(EnemyAttackState), typeof(EnemyIdleState), () => !HasTarget));
            stateMachine.AddTransition(new StateTransition(typeof(EnemyAttackState), typeof(EnemyChaseState), () => HasTarget && !IsTargetInAttackRange));
            stateMachine.ChangeState<EnemyIdleState>();
        }

        private bool CanUseAgent()
        {
            return agent != null && agent.enabled && agent.isOnNavMesh;
        }

        private float GetPlanarDistanceToTargetSqr()
        {
            var offset = Vector3.ProjectOnPlane(targetTransform.position - transform.position, Vector3.up);
            return offset.sqrMagnitude;
        }
    }
}
