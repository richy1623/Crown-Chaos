using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamSwitch : MonoBehaviour
{
    public GameObject mainCam;
    public GameObject powerCam;
    //private Player player;

    // Start is called before the first frame update
    void Start()
    {
        //player = GameObject.Find("ballista_base").GetComponent<Player>();
        mainCam.SetActive(true);
    }

    // Update is called once per frame
    /*void Update()
    {
        if (player.isPowered() && player.powerType().Equals("enhanced_sight_pu"))
        {
            mainCam.SetActive(false);
            powerCam.SetActive(true);
        }
    }*/
    public void eagleView()
    {
        /*if (player.isPowered() && player.powerType().Equals("enhanced_sight_pu"))
        {
            mainCam.SetActive(false);
            powerCam.SetActive(true);
        }*/
        mainCam.SetActive(false);
        powerCam.SetActive(true);
    }

    public void revert()
    {
        mainCam.SetActive(true);
        powerCam.SetActive(false);
    }
}
