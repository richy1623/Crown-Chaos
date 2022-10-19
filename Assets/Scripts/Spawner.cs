using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public int numPlayers;

    private GameObject[] ballistas;

    public Vector3[] spawnPoints;
    private int spawnCounter;

    public GameObject player;
    public GameObject ai;

    // Start is called before the first frame update
    void Start()
    {
        ballistas = new GameObject[numPlayers];

        //Set spawn Points
        spawnCounter = 0;
        //TODO Shuffle

        //Create Balistas
        //TODO set to player    
        ballistas[0] = Instantiate(ai);
        ballistas[0].GetComponent<Player>().spawn(spawnPoints[spawnCounter++]);
        for (int i = 1; i < numPlayers; i++)
        {
            ballistas[i] = Instantiate(ai);
            ballistas[i].GetComponent<Player>().spawn(spawnPoints[spawnCounter++]);
            if (spawnCounter >= spawnPoints.Length) spawnCounter = 0;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void respawn(int id)
    {
        for (int i = 0; i < numPlayers; i++)
        {
            Player player = ballistas[i].GetComponent<Player>();
            if (player.playerID == id)
            {
                player.spawn(spawnPoints[findFreeSpawnPoint()]);
            }
        }
    }

    private int findFreeSpawnPoint()
    {
        //TODO: find a good point
        if (spawnCounter >= spawnPoints.Length) spawnCounter = 0;
        return spawnCounter++;
    }

    void OnDrawGizmosSelected()
    {
        // Draw a red sphere at the spawn points' position
        Gizmos.color = Color.red;
        for(int i=0;i<spawnPoints.Length;i++)
            Gizmos.DrawSphere(spawnPoints[i], 1);
    }
}
