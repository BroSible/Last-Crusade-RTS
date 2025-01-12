using System.Collections;
using UnityEngine;
using UnityEngine.AI;


public class UnitMovement : MonoBehaviour
{
    public Camera camera;
    public NavMeshAgent agent;
    public LayerMask ground;
    AttackController attackController;
    public bool isCommandedToMove;

    DirectionIndicator directionIndicator;

    void Start()
    {
        camera = Camera.main;
        agent = GetComponent<NavMeshAgent>();
        directionIndicator = GetComponent<DirectionIndicator>();
        attackController = GetComponent<AttackController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
            {
                isCommandedToMove = true;
                StartCoroutine(NoCommand());
                agent.SetDestination(hit.point);
                directionIndicator.DrawLine(hit);
            }
        }

        // if(agent.hasPath == false || agent.remainingDistance <= agent.stoppingDistance)
        // {
        //     isCommandedToMove = false;
        // }
    }

    IEnumerator NoCommand()
    {
        yield return new WaitForSeconds(1);
        isCommandedToMove = false;
    }
}
