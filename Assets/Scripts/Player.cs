using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static event Action<int> LoadPlayer;

    public static int numberOfPlayers = 0;
    public float reloadTime = 3f;

    public GameObject bolt;
    public float numBolts;
    protected float reload;

    public AudioSource boltShootSFX;
    public ParticleSystem blastEffect;

    public int playerID;
    public string playerName;
    private float forwardInput;
    private float yawInput;
    protected Rigidbody rigidBody;
    public Transform ballista_top;
    protected Vector3 aimDirection;
    public float speed;
    public GameObject powerupIndicator;
    private CamSwitch cam;
    private bool hasPowerup;
    private string power;
    enum botlPowers {buckshot_pu, ghost_bolt_pu};
    Dictionary<string, int> timedPowers;

    private GameObject[] aimShperes;
    private AudioManager audioManager;

    private PowerupSpawner powerupSpawner;
    private AmmoUI ammoUI;

    public static float RESPAWN_TIME = 5;
    public bool dead;
    public float respawnWait;

    private bool isinvulnerable;
    private float invulnerableDuration;


    // Start is called before the first frame update
    protected void Start()
    {
        ammoUI = AmmoUI.instance;
        audioManager = AudioManager.instance;
        rigidBody = GetComponent<Rigidbody>();
        playerID = numberOfPlayers++;
        speed = 5.0f;
        timedPowers = new Dictionary<string, int>(){
            { "enhanced_sight_pu", 10 },
            { "infinite_ammo_pu", 10 },
            { "invincibility_pu", 7 },
            { "speed_boost_pu", 8 } };
        aimDirection = Vector3.zero;
        cam = GameObject.Find("Camera Manager").GetComponent<CamSwitch>();
        if (this is Player && !this.GetType().IsSubclassOf(typeof(Player)))
        {
            //Aim Assist
            aimShperes = new GameObject[5];
            for (int i=0; i<5; i++)
            {
                aimShperes[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                aimShperes[i].transform.localScale = Vector3.one*0.5f;
                DestroyImmediate(aimShperes[i].GetComponent<Collider>());
            }
        }

        powerupSpawner = PowerupSpawner.instance;

        respawnWait = 0;
        dead = false;

        hasPowerup = true;
        power = "invincibility_pu";
        StartCoroutine(PowerupCountdownRoutine(3));

        //isinvulnerable = true;
        //invulnerableDuration = 1.5f;
    }

    private void OnEnable()
    {
        Bolt.OnPlayerHit += CheckHit;
    }

    public bool isPowered()
    {
        if (hasPowerup) return true;
        else return false;
    }

    public string powerType()
    {
        string copy = (string)power.Clone();
        return copy;
    }

    private void OnDisable()
    {
        Bolt.OnPlayerHit -= CheckHit;
    }

    // Update is called once per frame
    void Update()
    {
        if (dead)
        {
            respawnWait += Time.deltaTime;
            return;
        }
        //invulnerabilityCheck();
        Reload();
        updateAmmoUI();
        aim();
        aimAssist();
        forwardInput = Input.GetAxis("Vertical");
        yawInput = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Mouse0) && !HUD.gameIsPaused)
        {
            Shoot();
        }

        powerupIndicator.transform.position = transform.position;

        Ray ray  = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            aimDirection = hit.point;
        }
    }

    protected void invulnerabilityCheck()
    {
        if (isinvulnerable)
        {
            if (invulnerableDuration > 0) invulnerableDuration -= Time.deltaTime;
            else
            {
                isinvulnerable = false;
                gameObject.layer = 0;
            }
        }
    }

    protected bool Shoot()
    {
        //Instantiate(bolt, transform.position, Quaternion.Euler(transform.eulerAngles+new Vector3(0, 90, 90))).GetComponent<Bolt>().setPlayer(playerID);
        if (numBolts >= 1 && reload >=1)
        {
            Instantiate(bolt, transform.position, Quaternion.Euler(ballista_top.eulerAngles)).GetComponent<Bolt>().setPlayer(playerID);
            boltShootSFX.Play();
            numBolts--;
            reload = 0;
            return true;
        }
        return false;
    }

    void CheckHit(int shooter, int hit)
    {
        if (hit == playerID)
        {
            //if (this.GetType().IsSubclassOf(typeof(Player))) Destroy(gameObject);
            Instantiate(blastEffect, gameObject.transform.position, gameObject.transform.rotation).Play();
            dead = true;
            gameObject.layer = 7;
            rigidBody.velocity = Vector3.zero;
            GetComponent<Renderer>().enabled = false;
            if (aimShperes != null) foreach (GameObject o in aimShperes) o.GetComponent<Renderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
            yawInput = 0;
            forwardInput = 0;
        }
    }

    protected void Reload()
    {
        numBolts += 1/reloadTime * Time.deltaTime;
        if (numBolts > 5) numBolts = 5;
        reload += Time.deltaTime;
    }

    private void updateAmmoUI()
    {
        float remainingDuration = 1f - reload;
        ammoUI.setRemainingDuration(remainingDuration);

        int bolts = (int) Math.Floor(numBolts);
        ammoUI.setNumberBolts(bolts);
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
        numBolts = 5f;
        reload = 1f;
        dead = false;
        respawnWait = 0;
        GetComponent<Renderer>().enabled = true;
        if (aimShperes != null) foreach (GameObject o in aimShperes) o.GetComponent<Renderer>().enabled = true;
        GetComponent<Collider>().enabled = true;
        //isinvulnerable = true;
        //invulnerableDuration = 3f;
        hasPowerup = true;
        power = "invincibility_pu";
        StartCoroutine(PowerupCountdownRoutine(3));
    }

    //Destroy powerup on collision
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Powerup"))
        {
            if (playerID == 0)
            {
                audioManager.Play("pickup_powerup");
            }
            hasPowerup = true;
            powerupIndicator.SetActive(true);
            powerupSpawner.spawnPowerup(other.gameObject.transform.position);
            Destroy(other.gameObject);
            int indexPU = other.ToString().IndexOf("_pu");
            power = other.ToString().Substring(0, indexPU + 3).Trim();
            Debug.Log("In the powers dictionary" + timedPowers.ContainsKey(power));
            //Debug.Log(timedPowers[power]);
            if (timedPowers.ContainsKey(power))
            {
                if (power.Equals("speed_boost_pu"))
                    speed = 7.5f;
                else if (power.Equals("invincibility_pu"))
                    gameObject.layer = 7;
                else if (power.Equals("enhanced_sight_pu"))
                    cam.eagleView();
                StartCoroutine(PowerupCountdownRoutine(timedPowers[power]));
            }
        }
    }

    //Create Countdown Routine for powerup
    IEnumerator PowerupCountdownRoutine(int time)
    {
        Debug.Log("Time: "+time);
        yield return new WaitForSeconds(time);
        if (power.Equals("speed_boost_pu"))
            speed = 5.0f;
        else if (power.Equals("invincibility_pu"))
            gameObject.layer = 0;
        else if (power.Equals("enhanced_sight_pu"))
            cam.revert();
        power = "";
        hasPowerup = false;
        powerupIndicator.SetActive(false);
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
        Ray ray = new Ray(transform.position, ballista_top.forward);
        RaycastHit raycastHit;
        Physics.Raycast(ray, out raycastHit);
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
        //int orbs = (int) Mathf.Clamp(raycastHit.distance/4, 3, 10);
        float distance = raycastHit.distance / 5;
        for (int i = 0; i < 5; i++)
        {
            aimShperes[i].transform.position = ray.origin + ray.direction * distance * (i+1);
        }
    }

    public void setName(string name)
    {
        playerName = name;
    }

    public string getName()
    {
        return playerName;
    }

}
