using UnityEngine;
using Soulwake.Game.Combat;
using Soulwake.Game.Core;

namespace Soulwake.Game.Enemy
{
    /// <summary>
    /// Minimal enemy brain:
    /// - chases player inside aggro range
    /// - attacks when in melee range
    /// - stops when dead
    /// </summary>
    [RequireComponent(typeof(EnemyMovement2D))]
    [RequireComponent(typeof(EnemyMeleeAttack2D))]
    [RequireComponent(typeof(Damageable2D))]
    public class EnemyController2D : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;
        [SerializeField] private bool autoFindTargetByTag = true;
        [SerializeField] private string playerTag = VerticalSliceConventions.PlayerTag;

        [Header("Detection")]
        [SerializeField] private float aggroRange = 6f;
        [SerializeField] private float loseInterestRange = 8f;

        [Header("Combat Spacing")]
        [SerializeField] private float attackDistancePadding = 0.05f;

        private EnemyMovement2D movement;
        private EnemyMeleeAttack2D meleeAttack;
        private Damageable2D damageable;
        private bool hasAggro;

        private void Awake()
        {
            movement = GetComponent<EnemyMovement2D>();
            meleeAttack = GetComponent<EnemyMeleeAttack2D>();
            damageable = GetComponent<Damageable2D>();
        }

        private void Start()
        {
            TryResolveTarget();
        }

        private void Update()
        {
            if (damageable.IsDead)
            {
                movement.Stop();
                return;
            }

            TryResolveTarget();
            if (target == null)
            {
                movement.Stop();
                return;
            }

            Vector2 toTarget = target.position - transform.position;
            float distance = toTarget.magnitude;

            if (distance <= aggroRange)
            {
                hasAggro = true;
            }
            else if (distance >= loseInterestRange)
            {
                hasAggro = false;
            }

            if (!hasAggro)
            {
                movement.Stop();
                return;
            }

            float attackDistance = meleeAttack.AttackRange + attackDistancePadding;
            if (distance <= attackDistance)
            {
                movement.Stop();
                meleeAttack.TryAttack(toTarget);
                return;
            }

            movement.MoveTowards(target.position);
        }

        private void TryResolveTarget()
        {
            if (target != null || !autoFindTargetByTag)
            {
                return;
            }

            GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObject != null)
            {
                target = playerObject.transform;
            }
        }
    }
}
