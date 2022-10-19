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
    private Rigidbody rigidBody;
    public float speed;
    public GameObject powerupIndicator;
    private bool hasPowerup;
    private string power;
    enum botlPowers {buckshot_pu, ghost_bolt_pu};
    Dictionary<string, int> timedPowers;


    // Start is called before the first frame update
    void Start()
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
    }

    // Update is called once per frame
    void Update()
    {
        forwardInput = Input.GetAxis("Vertical");
        yawInput = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Mouse0) && !HUD.gameIsPaused)
        {
            Shoot();
            AudioManager.instance.Play("bolt_fire");
        }
        powerupIndicator.transform.position = transform.position;
    }

    private void Shoot()
    {
        //Instantiate(bolt, transform.position, Quaternion.Euler(transform.eulerAngles+new Vector3(0, 90, 90))).GetComponent<Bolt>().setPlayer(playerID);
        Instantiate(bolt, transform.position, Quaternion.Euler(transform.eulerAngles)).GetComponent<Bolt>().setPlayer(playerID);
    }

    private void FixedUpdate()
    {
        rigidBody.transform.Rotate(0, yawInput * 2, 0, Space.Self);
        rigidBody.velocity = transform.forward * speed * forwardInput;
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
        powerupIndicator.SetActive(false);
    }

    //private void OnCollisionEnter(Collision collision) { }
}
