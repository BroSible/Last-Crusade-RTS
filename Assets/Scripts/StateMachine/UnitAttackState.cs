using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitAttackState : StateMachineBehaviour
{
    NavMeshAgent agent;
    AttackController attackController;

    public float stopAttackingDistance; // Это будет извлекаться из базы данных

    public float attackRate;
    public float attackTimer;

    // Добавляем ссылку на базу данных юнитов
    [SerializeField] private UnitDatabaseSO unitDatabase;

    public int unitIndex;  // Индекс юнита для поиска в базе данных

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.GetComponent<NavMeshAgent>();
        attackController = animator.GetComponent<AttackController>();

        // Проверяем, есть ли база данных и если в ней есть юниты
        if (unitDatabase != null && unitDatabase.units.Count > 0)
        {
            // Проверяем, если индекс в пределах допустимого диапазона
            if (unitIndex >= 0 && unitIndex < unitDatabase.units.Count)
            {
                // Извлекаем юнита по индексу
                UnitData unitData = unitDatabase.units[unitIndex];
                stopAttackingDistance = unitData.stopAttackingDistance;  // Извлекаем stopAttackingDistance из базы данных
            }
            else
            {
                Debug.LogWarning($"Invalid unit index: {unitIndex}. The index is out of bounds.");
            }
        }
        else
        {
            Debug.LogWarning("UnitDatabase is not assigned or empty.");
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (attackController.targetToAttack != null)
        {
            LookAtTarget();

            if (attackTimer <= 0)
            {
                Attack();
                attackTimer = 1f / attackRate;
            }
            else
            {
                attackTimer -= Time.deltaTime;
            }

            Collider targetCollider = attackController.targetToAttack.GetComponent<Collider>();
            Vector3 closestPoint = targetCollider.ClosestPoint(animator.transform.position);
            float distanceFromTarget = Vector3.Distance(closestPoint, animator.transform.position);

            if (distanceFromTarget > stopAttackingDistance || attackController.targetToAttack == null)
            {
                agent.SetDestination(animator.transform.position);

                // Переход в состояние "следования", если атака невозможна
                animator.SetBool("isAttacking", false);
            }
        }
        else
        {
            // Переход в состояние "следования", если нет цели
            animator.SetBool("isAttacking", false);
        }
    }

    private void Attack()
    {
        SoundManager.Instance.PlayAttackSound();

        var damageable = attackController.targetToAttack.GetComponent<IDamageable>();
        if (damageable != null)
        {
            var damageToInflict = attackController.unitDamage;  // Используем damage из AttackController для нанесения урона
            damageable.TakeDamage(damageToInflict);
        }
    }

    private void LookAtTarget()
    {
        Vector3 direction = attackController.targetToAttack.position - agent.transform.position;
        agent.transform.rotation = Quaternion.LookRotation(direction);

        var yRotation = agent.transform.eulerAngles.y;
        agent.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Логика при выходе из состояния, если требуется
    }
}
