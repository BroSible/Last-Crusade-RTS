using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    public Transform targetToAttack;
    public int unitDamage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && targetToAttack == null)
        {
            targetToAttack = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy") && targetToAttack != null)
        {
            targetToAttack = null;
        }
    }

    private void OnDrawGizmos()
    {
        // Follow Distance / Area
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 10f * 0.3f);

        // Attack Distance / Area
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1.5f);

        // Stop Attack Distance / Area
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 2f);
    }
}
