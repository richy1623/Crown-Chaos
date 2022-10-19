using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static int numberOfPlayers = 0;

    public GameObject bolt;

    public int playerID;
    protected float forwardInput;
    protected float yawInput;
    protected float speed;
    protected Rigidbody rigidBody;

    // Start is called before the first frame update
    protected void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        playerID = numberOfPlayers++;
        speed = 5;
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

    protected void Shoot()
    {
        //Instantiate(bolt, transform.position, Quaternion.Euler(transform.eulerAngles+new Vector3(0, 90, 90))).GetComponent<Bolt>().setPlayer(playerID);
        Instantiate(bolt, transform.position, Quaternion.Euler(transform.eulerAngles)).GetComponent<Bolt>().setPlayer(playerID);
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
}
