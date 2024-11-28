using UnityEngine;
using UnityEngine.AI;


public class UnitMovement : MonoBehaviour
{
    public Camera camera;
    public NavMeshAgent agent;
    public LayerMask ground;

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
                agent.SetDestination(hit.point);
            }
        }
    }
}
