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
        "Henry", "Ariana", "Arthur", "Eleanor", "Baird", "Muriel", "Charles", "Ruth", "Theo", "Archie"
    };

    private void Awake()
    {
        ballistas = new GameObject[numPlayers];
        if (spawnPoints == null || ballistas.Length > spawnPoints.Length) return;

        //Set spawn Points
        spawnCounter = 0;
        //TODO Shuffle
        Shuffle<Vector3>(spawnPoints);

        int d = PlayerPrefs.GetInt(OptionsMenu.DIFFICULTY_KEY) + 1;

        //Create Balistas
        //TODO set to player    
        ballistas[0] = player;
        ballistas[0].GetComponent<Player>().spawn(spawnPoints[spawnCounter++]);
        ballistas[0].GetComponent<Player>().playerName = "You";
        for (int i = 1; i < numPlayers; i++)
        {
            ballistas[i] = Instantiate(ai);
            AI component = (AI)ballistas[i].GetComponent<Player>();
            component.playerName = NAMES[i];
            component.playerID = i;
            component.storeLocations(ballistas);
            component.setDifficulty(d);
            component.spawn(spawnPoints[spawnCounter++]);
            if (spawnCounter >= spawnPoints.Length) spawnCounter = 0;
        }
    }

    private void Start()
    {
        leaderboard = Leaderboard.Instance;
        leaderboard.setLeaderboardItems(ballistas);
    }

    private void Update()
    {
        for (int i = 0; i < numPlayers; i++)
        {
            Player component = (Player)ballistas[i].GetComponent<Player>();
            if (component.dead && component.respawnWait>=Player.RESPAWN_TIME) respawn(component.playerID);
        }
    }

    private void respawn(int id)
    {
        if (id == 0)
        {
            Player player = ballistas[id].GetComponent<Player>();
            player.spawn(spawnPoints[findFreeSpawnPoint()]);
        }
        else
        {
            AI ai = ballistas[id].GetComponent<AI>();
            ai.spawn(spawnPoints[findFreeSpawnPoint()]);
        }
    }

    private int findFreeSpawnPoint()
    {
        spawnCounter++;
        if (spawnCounter >= spawnPoints.Length) spawnCounter = 0;
        Collider[] colliders = Physics.OverlapSphere(spawnPoints[spawnCounter], 12, 1<<0);
        //return spawnCounter++;
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.tag == "Player") return findFreeSpawnPoint();
        }
        return spawnCounter;
    }

    void OnDrawGizmosSelected()
    {
        // Draw a red sphere at the spawn points' position
        Gizmos.color = Color.red;
        for (int i = 0; i < spawnPoints.Length; i++)
            Gizmos.DrawSphere(spawnPoints[i], 1);
        //Gizmos.color = Color.green;
        //for (int i = 0; i < spawnPoints.Length; i++)
            //Gizmos.DrawSphere(spawnPoints[i], 12);
    }

    private void Shuffle<T>(T[] ls)
    {
        for (int i = 0; i < ls.Length; i++)
        {
            int j = Random.Range(0, ls.Length);
            T temp = ls[j];
            ls[j] = ls[i];
            ls[i] = temp;
        }
    }
}
