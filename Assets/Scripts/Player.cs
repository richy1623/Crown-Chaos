using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static int numberOfPlayers = 0;

    public GameObject bolt;

    public int playerID;
    private float forwardInput;
    private float yawInput;
    protected Rigidbody rigidBody;
    public Transform ballista_top;
    protected Vector3 aimDirection;
    public float speed;
    public GameObject powerupIndicator;
    private bool hasPowerup;
    private string power;
    enum botlPowers {buckshot_pu, ghost_bolt_pu};
    Dictionary<string, int> timedPowers;


    // Start is called before the first frame update
    protected void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        playerID = numberOfPlayers++;
        hasPowerup = false;
        speed = 5.0f;
        timedPowers = new Dictionary<string, int>(){
            { "enhanced_sight_pu", 10 },
            { "infinite_ammo_pu", 10 },
            { "invincibility_pu", 7 },
            { "speed_boost_pu", 8 } };
        aimDirection = Vector3.zero;
        /*if(this is Player and typeof(this).IsSubclassOf(typeof(Player))
        {
            print("true");
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        aim();
        forwardInput = Input.GetAxis("Vertical");
        yawInput = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Mouse0) && !HUD.gameIsPaused)
        {
            Shoot();
            AudioManager.instance.Play("bolt_fire");
        }
        powerupIndicator.transform.position = transform.position;

        Ray ray  = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            aimDirection = hit.point;
        }
    }

    protected void Shoot()
    {
        //Instantiate(bolt, transform.position, Quaternion.Euler(transform.eulerAngles+new Vector3(0, 90, 90))).GetComponent<Bolt>().setPlayer(playerID);
        Instantiate(bolt, transform.position, Quaternion.Euler(ballista_top.eulerAngles)).GetComponent<Bolt>().setPlayer(playerID);
    }

    private void FixedUpdate()
    {
        rigidBody.transform.Rotate(0, yawInput * 2, 0, Space.Self);
        rigidBody.velocity = transform.forward * speed * forwardInput;
    }

    public void spawn(Vector3 pos)
    {
        pos.y = 0.5f;
        transform.position = pos;
        transform.rotation = Quaternion.Euler(0, 270 - Mathf.Atan2(pos.z, pos.x) * Mathf.Rad2Deg, 0);
    }

    //Destroy powerup on collision
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Powerup"))
        {
            hasPowerup = true;
            powerupIndicator.SetActive(true);
            Destroy(other.gameObject);
            int indexPU = other.ToString().IndexOf("_pu");
            power = other.ToString().Substring(0, indexPU + 3).Trim();
            Debug.Log(timedPowers.ContainsKey(power));
            Debug.Log(timedPowers[power]);
            if (timedPowers.ContainsKey(power))
                if (power.Equals("speed_boost_pu"))
                    speed = 7.5f;
                StartCoroutine(PowerupCountdownRoutine(timedPowers[power]));
        }
    }

    //Create Countdown Routine for powerup
    IEnumerator PowerupCountdownRoutine(int time)
    {
        yield return new WaitForSeconds(time);
        if (power.Equals("speed_boost_pu"))
            speed = 5.0f;
        hasPowerup = false;
    }

    //private void OnCollisionEnter(Collision collision) { }

    protected void aim()
    {
        Vector3 targetDirection = aimDirection - ballista_top.position;
        targetDirection.y = 0;
        targetDirection = targetDirection.normalized;
        // The step size is equal to speed times frame time.
        float singleStep = 4 * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(ballista_top.forward, targetDirection, singleStep, 0.0f);

        // Draw a ray pointing at our target in
        //Debug.DrawRay(transform.position, newDirection, Color.red);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        ballista_top.rotation = Quaternion.LookRotation(newDirection);
    }

    void aimAssist()
    {
        Ray ray = new Ray(transform.position, ballista_top.rotation.eulerAngles);
        RaycastHit raycastHit;
        Physics.Raycast(ray, out raycastHit, 10f);
        for (float i = 1; i <= raycastHit.distance; i += raycastHit.distance / 10)
        {
            GameObject mySphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            mySphere.transform.position = transform.position + ballista_top.rotation.eulerAngles * i;
            mySphere.transform.localScale = Vector3.one;
        }
    }
}
