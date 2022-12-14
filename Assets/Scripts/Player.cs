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
    protected float reloadDuration = 1f;

    public AudioSource boltShootSFX;
    public ParticleSystem blastEffect;
    public Material sphereMaterial;

    public int playerID;
    public string playerName;
    private float forwardInput;
    private float yawInput;
    protected Rigidbody rigidBody;
    public Transform ballista_top;
    protected Vector3 aimDirection;
    public float speed;
    public GameObject powerupIndicator;
    private bool hasPowerup;
    private bool powerupActive;
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

    private bool isInvunrable;
    private float invunrableDuration;

    private bool hasShield;
    private bool buckshot;
    private bool ghostShot;

    private HUD hud;
    private PowerupUI powerupUI;


    // Start is called before the first frame update
    protected void Start()
    {
        hud = HUD.instance;
        ammoUI = AmmoUI.instance;
        audioManager = AudioManager.instance;
        powerupUI = PowerupUI.instance;

        rigidBody = GetComponent<Rigidbody>();
        hasPowerup = false;
        powerupActive = false;
        speed = 5.0f;
        timedPowers = new Dictionary<string, int>(){
            { "enhanced_sight_pu", 10 },
            { "infinite_ammo_pu", 10 },
            { "invincibility_pu", 7 },
            { "speed_boost_pu", 8 } };
        aimDirection = Vector3.zero;
        if (this is Player && !this.GetType().IsSubclassOf(typeof(Player)))
        {
            //Aim Assist
            aimShperes = new GameObject[5];
            for (int i=0; i<5; i++)
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.GetComponent<Renderer>().material = sphereMaterial;
                aimShperes[i] = sphere;
                aimShperes[i].transform.localScale = Vector3.one*0.5f;
                DestroyImmediate(aimShperes[i].GetComponent<Collider>());
            }
        }

        powerupSpawner = PowerupSpawner.instance;

        respawnWait = 0;
        dead = false;

        isInvunrable = true;
        invunrableDuration = 1.5f;

        hasShield = false;
        buckshot = false;
        ghostShot = false;

        rigidBody.centerOfMass = Vector3.zero;
        rigidBody.inertiaTensorRotation = Quaternion.identity;
    }

    private void OnEnable()
    {
        Bolt.OnPlayerHit += CheckHit;
    }

    private void OnDisable()
    {
        Bolt.OnPlayerHit -= CheckHit;
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

    // Update is called once per frame
    void Update()
    {
        if (dead)
        {
            respawnWait += Time.deltaTime;
            return;
        }
        invunrabilityCheck();
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

        if(Input.GetKeyDown(KeyCode.Space) && !HUD.gameIsPaused && hasPowerup)
        {
            powerupUI.activatePowerup();
            powerupIndicator.SetActive(true);
            powerupActive = true;
            if (timedPowers.ContainsKey(power))
            {
                if (power.Equals("speed_boost_pu"))
                {
                    speed = 7.5f;
                }
                else if (power.Equals("invincibility_pu"))
                {
                    hasShield = true;
                    Leaderboard.hasShield = true;
                }
                else if (power.Equals("enhanced_sight_pu"))
                {
                    CameraFollow.enhancedSight = true;
                }
                else if (power.Equals("infinite_ammo_pu"))
                {
                    reloadTime = 0.125f;
                    reloadDuration = 0.5f;
                    AmmoUI.reloadDuration = 0.5f;
                }      
                StartCoroutine(PowerupCountdownRoutine(timedPowers[power]));
            }   
            else
            {
                if(power.Equals("buckshot_pu"))
                {
                    buckshot = true;
                }    
                else if(power.Equals("ghost_bolt_pu"))
                {
                    ghostShot = true;   
                }
            }
        }

        powerupIndicator.transform.position = transform.position;

        Ray ray  = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            aimDirection = hit.point;
        }
    }

    protected void invunrabilityCheck()
    {
        if (isInvunrable)
        {
            if (invunrableDuration > 0) invunrableDuration -= Time.deltaTime;
            else
            {
                isInvunrable = false;
                gameObject.layer = 0;
            }
        }
    }

    protected bool Shoot()
    {
        //Instantiate(bolt, transform.position, Quaternion.Euler(transform.eulerAngles+new Vector3(0, 90, 90))).GetComponent<Bolt>().setPlayer(playerID);
        if (numBolts >= 1 && reload >= reloadDuration)
        {
            GameObject b = Instantiate(bolt, transform.position, Quaternion.Euler(ballista_top.eulerAngles));
            b.GetComponent<Bolt>().setPlayer(playerID);

            if (ghostShot)
            {
                b.GetComponent<Rigidbody>().isKinematic = true;
                ghostShot = false;
                powerupUI.disablePowerup();
                power = "";
                hasPowerup = false;
                powerupActive = false;
                powerupIndicator.SetActive(false);
            }
            else if(buckshot)
            {
                Instantiate(bolt, transform.position, Quaternion.Euler(ballista_top.eulerAngles.x, ballista_top.eulerAngles.y + 25, ballista_top.eulerAngles.z)).GetComponent<Bolt>().setPlayer(playerID);
                Instantiate(bolt, transform.position, Quaternion.Euler(ballista_top.eulerAngles.x, ballista_top.eulerAngles.y - 25, ballista_top.eulerAngles.z)).GetComponent<Bolt>().setPlayer(playerID);
                buckshot = false;
                powerupUI.disablePowerup();
                power = "";
                hasPowerup = false;
                powerupActive = false;
                powerupIndicator.SetActive(false);
            }
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
            if(!hasShield)
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
            else
            {
                hasShield = false;
                powerupUI.disablePowerup();
                power = "";
                hasPowerup = false;
                powerupActive = false;
                powerupIndicator.SetActive(false);
            }
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
        transform.Rotate(0, yawInput * 2, 0, Space.Self);
        rigidBody.velocity = transform.forward * speed * forwardInput;
    }

    public void spawn(Vector3 pos)
    {
        if(playerID == 0 && hud != null)
        {
            hud.hideRespawning();
        }
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
        isInvunrable = true;
        invunrableDuration = 3f;
    }

    //Destroy powerup on collision
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Powerup") && !powerupActive)
        {
            int indexPU = other.ToString().IndexOf("_pu");
            power = other.ToString().Substring(0, indexPU + 3).Trim();

            if (playerID == 0)
            {
                audioManager.Play("pickup_powerup");
                powerupUI.collectPowerup(power);
            }
            hasPowerup = true;
            powerupSpawner.spawnPowerup(other.gameObject.transform.position);
            Destroy(other.gameObject);
        }
    }

    //Create Countdown Routine for powerup
    IEnumerator PowerupCountdownRoutine(int time)
    {
        yield return new WaitForSeconds(time);
        if (power.Equals("speed_boost_pu"))
        {
            speed = 5.0f;
        }
        else if (power.Equals("invincibility_pu"))
        {
            hasShield = false;
        } 
        else if (power.Equals("enhanced_sight_pu"))
        {
            CameraFollow.enhancedSight = false;
        }
        else if (power.Equals("infinite_ammo_pu"))
        {
            reloadTime = 3f;
            reloadDuration = 1f;
            AmmoUI.reloadDuration = 0.5f;
        }
            

        power = "";
        hasPowerup = false;
        powerupActive = false;
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
