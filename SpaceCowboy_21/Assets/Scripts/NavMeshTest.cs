using Pathfinding;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshTest : MonoBehaviour
{
    //public Transform target;
    Vector2 targetPos;

    public float speed = 200f;
    public float nextWaypointDistance = 3f;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        //agent = GetComponent<NavMeshAgent>();
        //agent.updateRotation = false;
        //agent.updateUpAxis = false;

        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        
    }

    //Path�� ���� ����Ѵ�. path����� �Ϸ�Ǹ� OnPathComplete�� �ݹ�. Path ��� ���̸� �Է��� ���� �ʴ´�. 
    void UpdatePath()
    {
        if(seeker.IsDone())
            seeker.StartPath(rb.position, targetPos, OnPathComplete);
    }

    //����� ������ path�� ������ ������ path�� �����ϰ� ���� waypoint�� 0���� �ʱ�ȭ�Ѵ�. 
    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    
    void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            UpdatePath();
        }

        if (path == null) return; 

        //waypoint�� ���������� �Ѿ��
        if(currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }


        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        rb.AddForce(force);


        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if(distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

}
