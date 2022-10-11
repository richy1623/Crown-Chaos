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

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        playerID = numberOfPlayers++;
    }

    // Update is called once per frame
    void Update()
    {
        forwardInput = Input.GetAxis("Vertical");
        yawInput = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Mouse0) && !HUD.gameIsPaused)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        //Instantiate(bolt, transform.position, Quaternion.Euler(transform.eulerAngles+new Vector3(0, 90, 90))).GetComponent<Bolt>().setPlayer(playerID);
        Instantiate(bolt, transform.position, Quaternion.Euler(transform.eulerAngles)).GetComponent<Bolt>().setPlayer(playerID);
    }

    private void FixedUpdate()
    {
        rigidBody.transform.Rotate(0, yawInput * 2, 0, Space.Self);
        rigidBody.velocity = transform.forward * 5 * forwardInput;
    }
}
