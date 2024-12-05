using UnityEngine;
using UnityEngine.AI;


public class UnitMovement : MonoBehaviour
{
    public Camera camera;
    public NavMeshAgent agent;
    public LayerMask ground;

    public bool isCommandedToMove;

    void Start()
    {
        camera = Camera.main;
        agent = GetComponent<NavMeshAgent>();
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
                agent.SetDestination(hit.point);
            }

            // Agent reached destination
            if(agent.hasPath == false || agent.remainingDistance <= agent.stoppingDistance)
            {
                isCommandedToMove = false;  
            }
        }
    }
}
