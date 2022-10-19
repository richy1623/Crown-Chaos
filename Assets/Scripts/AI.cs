using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : Player
{
    public Map map;

    Vector3 currentWaypoint;

    Vector3 target;
    Vector3[] path;
    int targetIndex;

    bool moving;
    bool turning;

    float distance;

    private float originY;
    public float mindistance;

    new void Start()
    {
        base.Start();
        map = GameObject.Find("Map").GetComponent<Map>();
        originY = 0.249f;
        mindistance = 0.05f;
        target = map.getTarget();
        //waypoint = transform.position;
        forwardInput = 1;
        moving = false;
        turning = true;
        getPath();
    }

    void Update()
    {
        if (path == null) return;
        if (turning)
        {
            look();
            return;
        }
        Vector3 targetDirection = currentWaypoint - transform.position;
        targetDirection.y = 0;
        distance = Vector3.Magnitude(targetDirection);

        if (distance < mindistance)
        {
            targetIndex++;
            if (targetIndex >= path.Length)
            {
                moving = false;
                return;
            }
            currentWaypoint = path[targetIndex];
            currentWaypoint.y = originY;
            mindistance = 0.05f;
            turning = true;
        }
        else
        {
            mindistance += 0.01f;
        }
    }

    void look()
    {
        Vector3 targetDirection = currentWaypoint - transform.position;
        targetDirection.y = 0;
        targetDirection = targetDirection.normalized;
        // The step size is equal to speed times frame time.
        float singleStep = 10 * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
        if (Mathf.Abs(Vector3.SignedAngle(targetDirection, transform.forward, Vector3.up)) < mindistance)
        {
            turning = false;
            mindistance = 0.05f;
        }
        else
        {
            mindistance += 0.01f;
        }

        // Draw a ray pointing at our target in
        //Debug.DrawRay(transform.position, newDirection, Color.red);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        transform.rotation = Quaternion.LookRotation(newDirection);
    }


    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            targetIndex = 0;
            moving = true;
            currentWaypoint = path[0];
            currentWaypoint.y = originY;
        }
        else
        {
            moving = false;
            getPath();
        }
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }

    public void getPath()
    {
        PathProvider.RequestPath(transform.position, target, OnPathFound);
    }

    private void FixedUpdate()
    {
        //rigidBody.transform.Rotate(0, yawInput * 2, 0, Space.Self);
        if (!turning && moving)
        rigidBody.position = Vector3.MoveTowards(transform.position, currentWaypoint, 5 * Time.deltaTime);
    }
}
