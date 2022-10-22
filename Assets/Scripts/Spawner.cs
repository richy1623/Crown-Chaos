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
    public int difficulty;

    private Leaderboard leaderboard;
    
    private string[] NAMES = new string[]
    {
        "Henry", "Ariana", "Arthur", "Eleanor", "Baird", "Muriel", "Charles", "Ruth", "Theo"
    };

    private void Awake()
    {
        ballistas = new GameObject[numPlayers];
        if (spawnPoints == null || ballistas.Length > spawnPoints.Length) return;

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
            AI component = (AI)ballistas[i].GetComponent<Player>();
            component.playerName = NAMES[i];
            component.playerID = i;
            component.storeLocations(ballistas);
            component.setDifficulty(difficulty);
            component.spawn(spawnPoints[spawnCounter++]);
            if (spawnCounter >= spawnPoints.Length) spawnCounter = 0;
        }
    }

    private void Start()
    {
        leaderboard = Leaderboard.Instance;
        leaderboard.setLeaderboardItems(ballistas);
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
        if (spawnCounter >= spawnPoints.Length) spawnCounter = 0;
        Collider[] colliders = Physics.OverlapSphere(spawnPoints[spawnCounter], 10);
        return spawnCounter++;
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.tag == "Player") return spawnCounter++;
        }
        spawnCounter++;
        return findFreeSpawnPoint();
    }

    void OnDrawGizmosSelected()
    {
        // Draw a red sphere at the spawn points' position
        Gizmos.color = Color.red;
        for(int i=0;i<spawnPoints.Length;i++)
            Gizmos.DrawSphere(spawnPoints[i], 1);
    }
}
