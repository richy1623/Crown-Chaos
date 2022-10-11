using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bolt : MonoBehaviour
{
    public static event Action OnPlayerHit;

    private static int MAX_BOUNCES=10;

    private int bounces;
    private float speed;
    private int playerID;

    [SerializeField] private AudioSource wallHitSFX;
    //private float y;


    // Start is called before the first frame update
    void Start()
    {
        bounces = MAX_BOUNCES;
        speed = 10;
        //y = transform.position.y;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        transform.Translate(Vector3.forward * speed * Time.fixedDeltaTime);
        //transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }

    /*private void OnTriggerEnter(Collider other)
    {
        print("Collide");
        if (other.gameObject.tag == "Wall")
        {
            RaycastHit info;

            Bounce(other.gameObject);
        }
    }*/

    void OnCollisionEnter(Collision collision)
    {
        print("Collide");
        if (collision.gameObject.tag == "Wall")
        {
            wallHitSFX.Play();
            Bounce(collision.contacts[0].normal);
            //Debug.DrawRay(collision.contacts[0].point, collision.contacts[0].normal);
        }else if (collision.gameObject.tag == "Player")
        {
            HitPlayer(collision.gameObject);
        }
        
    }

    private void Bounce(Vector3 normal)
    {
        //print("Bounce "+ bounces);
        Vector3 v = Vector3.Reflect(transform.forward, normal);
        transform.rotation = Quaternion.FromToRotation(Vector3.forward, v);
        //float rot = 90 - Mathf.Atan2(v.z, v.x) * Mathf.Rad2Deg;
        //transform.eulerAngles = new Vector3(90, rot, 0);
        Move();
        bounces--;
        if (bounces <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void HitPlayer(GameObject playerObject)
    {
        Player player = playerObject.GetComponent<Player>();
        if (player.playerID == playerID && bounces == MAX_BOUNCES)  return;
        OnPlayerHit?.Invoke();
        print("hit player "+ player.playerID);
        Destroy(gameObject);
    }

    public void setPlayer(int id)
    {
        playerID = id;
    }
}
