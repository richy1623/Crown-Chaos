using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private Image timerFill;
    [SerializeField] private GameObject disabledOverlay;
    [SerializeField] private GameObject staticBolt;
    [SerializeField] private TextMeshProUGUI numBoltsText;

    private float reloadDuration;
    private float remainingDuration;
    private bool reloading;
    private int numBolts;

    public static AmmoUI instance;
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

        reloadDuration = 1f;
        remainingDuration = 0f;
        reloading = false;
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if(reloading)
        {
            if (remainingDuration > 0)
            {
                timerFill.fillAmount = Mathf.InverseLerp(0, reloadDuration, remainingDuration);

            }
            else
            {
                disabledOverlay.SetActive(false);
                staticBolt.SetActive(true);
                reloading = false;
                timerFill.fillAmount = 1;
            }
        }

        numBoltsText.text = "" + numBolts;

        if (Input.GetKeyDown(KeyCode.Mouse0) && !HUD.gameIsPaused && !reloading)
        {
            reload();
        }
    }

    public void reload()
    {
        staticBolt.SetActive(false);
        disabledOverlay.SetActive(true);
        remainingDuration = reloadDuration;
        reloading = true;
    }

    public bool isReloading()
    {
        return reloading;
    }

    public void setRemainingDuration(float duration)
    {
        remainingDuration = duration;
    }

    public void setNumberBolts(int bolts)
    {
        numBolts = bolts;
    }
}
