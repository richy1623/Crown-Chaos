using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : Player
{
    public int difficulty;

    public Map map;

    Vector3 currentWaypoint;

    Vector3 target;
    Vector3[] path;
    int targetIndex;
    private bool reqPath;

    bool moving;
    bool turning;
    float direction;

    float distance;

    private float originY;
    public float mindistance;

    private GameObject[] ballistas;

    public float aimRock;
    int aimDir;

    new void Start()
    {
        Application.targetFrameRate = 60;
        base.Start();
        map = GameObject.Find("Map").GetComponent<Map>();
        originY = 0.249f;
        mindistance = 0.05f;
        reqPath = false;
        target = map.getTarget();
        //waypoint = transform.position;
        moving = false;
        turning = true;
        //getPath();
        aimRock = 0;
        aimDir = 1;
        direction = 1f;

        //TODO REmove
        show = false;
    }

    public void spawn(Vector3 pos)
    {
        base.spawn(pos);
        mindistance = 0.05f;
        reqPath = false;
        moving = false;
        turning = true;
        aimRock = 0;
        aimDir = 1;
        direction = 1f;
        //if (map!=null) target = map.getTarget();
    }

    public void storeLocations(GameObject[] ballistas){
        this.ballistas = ballistas;
    }

    void Update()
    {
        if (dead)
        {
            respawnWait += Time.deltaTime;
            return;
        }
        invunrabilityCheck();
        scout();
        aim();
        Reload();
        if (!moving)
        {
            getPath();
            return;
        }
        //if (path == null) return;
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
                target = map.getTarget();
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
        reqPath = false;
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
            //TODO move into update loop
            moving = false;
            //print(playerID);
            //getPath();
        }
    }
    public bool show;
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

        Gizmos.color = Color.yellow;
        //if (!show) return;
        Gizmos.DrawSphere(target, 1);
    }

    public void getPath()
    {
        if (!reqPath)
        {
            //print(playerID + " req path");
            reqPath = true;
            PathProvider.RequestPath(transform.position, target, OnPathFound);
        }
    }

    private void FixedUpdate()
    {
        if (dead) return;
        //rigidBody.transform.Rotate(0, yawInput * 2, 0, Space.Self);
        if (!turning && moving)
        rigidBody.position = Vector3.MoveTowards(transform.position, currentWaypoint, 5 * Time.deltaTime * direction);
    }

    private new void aim()
    {
        Vector3 targetDirection = transform.forward;
        //targetDirection.y = 0;
        targetDirection = targetDirection.normalized;
        // The step size is equal to speed times frame time.
        float singleStep = 90/difficulty * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(ballista_top.forward, targetDirection, singleStep, 1.0f);
        newDirection = Quaternion.Euler(0, aimRock*45, 0) * newDirection;
        aimRock += aimDir*Time.deltaTime/2f;
        if (aimRock*aimDir >= 1) aimDir *= -1;

        // Draw a ray pointing at our target in
        //Debug.DrawRay(transform.position, newDirection*difficulty*15, Color.blue);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        ballista_top.rotation = Quaternion.LookRotation(newDirection);
    }

    private void scout()
    {
        
        //if (difficulty <= 1) return;
        int sight = 15;
        if (difficulty == 2) sight = 30;
        if (difficulty == 3) sight = 45;
        /*if (ballistas == null)
        {
            print("EMPTY");
            return;
        }
        float dist = Vector3.Magnitude(target - transform.position);
        bool found = false;
        foreach (GameObject o in ballistas)
        {
            if (o == this) return;
            float newDist = Vector3.Magnitude(o.transform.position - transform.position);
            if (newDist<dist-5f && newDist < sight)
            {
                dist = newDist;
                target = o.transform.position;
                found = true;
            }
        }
        if (found)
        {
            getPath();
        }*/

        if (checkShoot(sight, difficulty, ballista_top.position, ballista_top.forward))
        {
            if(Shoot())
                StartCoroutine(ReverseAfterShoot(1f));
        }
        
    }

    bool checkShoot(float sight, int bounces, Vector3 pos, Vector3 dir)
    {
        Ray ray = new Ray(pos, dir);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, sight, 1 << 0))
        {
            if (hit.collider.tag == "Player" && hit.collider.gameObject!=gameObject)
            {
                return true;
            }
            Debug.DrawLine(pos, hit.point, Color.red);
            if (bounces > 0)
            {
                Vector3 incomingVec = hit.point - pos;

                // Use the point's normal to calculate the reflection vector.
                Vector3 reflectVec = Vector3.Reflect(incomingVec, hit.normal);

                // Draw lines to show the incoming "beam" and the reflection.
                //Debug.DrawRay(hit.point, reflectVec, Color.green);
                return checkShoot(sight-hit.distance, bounces-1, hit.point, reflectVec);
            }
        }
        return false;
    }

    public void setDifficulty(int d)
    {
        difficulty = d;
    }

    private IEnumerator ReverseAfterShoot(float waitTime)
    {
        mindistance = -1f;
        direction = (1-difficulty)*(0.25f);
        yield return new WaitForSeconds(waitTime);
        direction = 1f;
        mindistance = 0.05f;
    }
}
