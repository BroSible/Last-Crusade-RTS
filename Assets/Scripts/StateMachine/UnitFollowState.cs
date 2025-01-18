using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitFollowState : StateMachineBehaviour
{
    AttackController attackController;
    NavMeshAgent agent;

    // Будет извлечен из базы данных
    public float attackingDistance;

    public float rotationSpeed = 5.0f;

    // Ссылка на базу данных юнитов
    [SerializeField] private UnitDatabaseSO unitDatabase;

    public int unitIndex;  // Индекс юнита для поиска в базе данных

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackController = animator.transform.GetComponent<AttackController>();
        agent = animator.transform.GetComponent<NavMeshAgent>();

        // Проверяем, есть ли база данных и если в ней есть юниты
        if (unitDatabase != null && unitDatabase.units.Count > 0)
        {
            // Проверяем, если индекс в пределах допустимого диапазона
            if (unitIndex >= 0 && unitIndex < unitDatabase.units.Count)
            {
                // Извлекаем юнита по индексу
                UnitData unitData = unitDatabase.units[unitIndex];
                attackingDistance = unitData.attackingDistance;  // Извлекаем attackingDistance из базы данных
                Debug.Log($"Found unit at index {unitIndex} with attackingDistance: {attackingDistance}");
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

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (attackController.targetToAttack == null)
        {
            animator.SetBool("isFollow", false);
        }
        else
        {
            if (animator.transform.GetComponent<UnitMovement>().isCommandedToMove == false)
            {
                // Получение коллайдера цели
                Collider targetCollider = attackController.targetToAttack.GetComponent<Collider>();
                if (targetCollider != null)
                {
                    // Вычисляем ближайшую точку на границе коллайдера цели
                    Vector3 closestPoint = targetCollider.ClosestPoint(animator.transform.position);
                    agent.SetDestination(closestPoint);

                    // Ограничиваем вращение по оси Y
                    Vector3 direction = closestPoint - animator.transform.position;
                    direction.y = 0; // Игнорируем ось Y

                    if (direction != Vector3.zero)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(direction);
                        animator.transform.rotation = Quaternion.Slerp(animator.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                    }

                    // Переход к атаке, если юнит достаточно близко к цели
                    float distanceFromTarget = Vector3.Distance(animator.transform.position, closestPoint);
                    if (distanceFromTarget <= attackingDistance)
                    {
                        agent.SetDestination(animator.transform.position);
                        animator.SetBool("isAttacking", true);
                        Debug.Log("Can attack");
                    }
                }
            }
        }
    }
}
