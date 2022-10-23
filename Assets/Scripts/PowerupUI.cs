using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PowerupUI : MonoBehaviour
{
    [SerializeField] private GameObject powerupIcon;
    [SerializeField] private TextMeshProUGUI powerupName;
    [SerializeField] private GameObject activeOverlay;
    [SerializeField] private TextMeshProUGUI powerupTimer;
    
    [SerializeField] private Sprite noPowerupSprite;
    [SerializeField] private Sprite buckshotSprite;
    [SerializeField] private Sprite enhancedSightSprite;
    [SerializeField] private Sprite ghostBoltSprite;
    [SerializeField] private Sprite shieldSprite;
    [SerializeField] private Sprite speedBoostSprite;
    [SerializeField] private Sprite infiniteAmmoSprite;

    public static string BUCKSHOT = "buckshot_pu";
    public static string ENHANCED_SIGHT = "enhanced_sight_pu";
    public static string GHOST_BOLT = "ghost_bolt_pu";
    public static string SHIELD = "invincibility_pu";
    public static string SPEED_BOOST = "speed_boost_pu";
    public static string INFINITE_AMMO = "infinite_ammo_pu";
    private Dictionary<string, PowerupUIItem> POWERUPS = new Dictionary<string, PowerupUIItem>();

    private string currentPowerup = "";
    private float powerupTimeRemaining;
    private bool powerupTimerActive = false;

    private Color goldColor = new Color32(243, 192, 36, 255);

    private AudioManager audioManager;
    public static PowerupUI instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        POWERUPS.Add(BUCKSHOT, new PowerupUIItem("Buckshot", 0, buckshotSprite));
        POWERUPS.Add(ENHANCED_SIGHT, new PowerupUIItem("Enhanced Sight", 10, enhancedSightSprite));
        POWERUPS.Add(GHOST_BOLT, new PowerupUIItem("Ghost Bolt", 0, ghostBoltSprite));
        POWERUPS.Add(SHIELD, new PowerupUIItem("Shield", 7, shieldSprite));
        POWERUPS.Add(SPEED_BOOST, new PowerupUIItem("Speed Boost", 8, speedBoostSprite));
        POWERUPS.Add(INFINITE_AMMO, new PowerupUIItem("Infinite Ammo", 10, infiniteAmmoSprite));

        audioManager = AudioManager.instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (powerupTimerActive)
        {
            if (powerupTimeRemaining > 0)
            {
                powerupTimeRemaining -= Time.deltaTime;
                float seconds = Mathf.FloorToInt(powerupTimeRemaining % 60);

                if(seconds >= 0)
                {
                    powerupTimer.text = seconds.ToString();
                }
               
            }
            else
            {
                disablePowerup();
            }
        }
    }

    public void collectPowerup(string powerup)
    {
        currentPowerup = powerup;
        PowerupUIItem item = POWERUPS[powerup];
        powerupIcon.GetComponent<Image>().sprite = item.getPowerupSprite();
        powerupName.text = item.getPowerupName();
    }

    public void activatePowerup()
    {
        if(currentPowerup != "")
        {
            audioManager.Play("activate_powerup");
            PowerupUIItem item = POWERUPS[currentPowerup];
            activeOverlay.SetActive(true);
            powerupName.color = goldColor;
            powerupTimeRemaining = item.getPowerupDuration();
            if(powerupTimeRemaining > 0)
            {
                powerupTimerActive = true;
                powerupTimer.text = item.getPowerupDuration().ToString();
                powerupTimer.GameObject().SetActive(true);
            }
        }
    }

    public void disablePowerup()
    {
        currentPowerup = "";
        activeOverlay.SetActive(false);
        powerupName.color = Color.white;
        powerupName.text = "No Powerup";
        powerupIcon.GetComponent<Image>().sprite = noPowerupSprite;
        powerupTimerActive = false;
        powerupTimeRemaining = 0f;
        powerupTimer.gameObject.SetActive(false);
    }
}
