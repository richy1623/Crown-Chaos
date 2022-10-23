using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PowerupSpawner : MonoBehaviour
{
    [SerializeField] private Transform forestSpawners;
    [SerializeField] private Transform farmSpawners;
    [SerializeField] private Transform villageSpawners;

    [SerializeField] private GameObject[] powerups;

    private const int NUM_POWERUPS = 8;

    private Dictionary<string, float> cumulativeProbs;
    private string FARM_KEY = "farm";
    private string FOREST_KEY = "forest";
    private string VILLAGE_KEY = "vilalge";

    private Dictionary<string, List<Transform>> transforms;
    
    private List<Transform> forestSpawnerTransforms;
    private List<Transform> farmSpawnerTransforms;
    private List<Transform> villageSpawnerTransforms;

    public static PowerupSpawner instance;

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

        cumulativeProbs = new Dictionary<string, float>();
        
        cumulativeProbs[VILLAGE_KEY] = 0.3f;
        cumulativeProbs[FARM_KEY] = 0.65f;
        cumulativeProbs[FOREST_KEY] = 1f;

        forestSpawnerTransforms = new List<Transform>(forestSpawners.GetComponentsInChildren<Transform>());
        farmSpawnerTransforms = new List<Transform>(farmSpawners.GetComponentsInChildren<Transform>());
        villageSpawnerTransforms = new List<Transform>(villageSpawners.GetComponentsInChildren<Transform>());

        forestSpawnerTransforms.RemoveAt(0);
        farmSpawnerTransforms.RemoveAt(0);
        villageSpawnerTransforms.RemoveAt(0);

        transforms = new Dictionary<string, List<Transform>>();

        transforms[FOREST_KEY] = forestSpawnerTransforms;
        transforms[FARM_KEY] = farmSpawnerTransforms;
        transforms[VILLAGE_KEY] = villageSpawnerTransforms;

        for (int i = 0; i < NUM_POWERUPS; i++)
        {
            spawnPowerup();
        }
    }
    public void spawnPowerup(Vector3 prevPos = default(Vector3))
    {
        string area = VILLAGE_KEY;
        double areaRand = Random.value;
        foreach(KeyValuePair<string, float> kvp in cumulativeProbs)
        {
            if (kvp.Value > areaRand)
            {
                area = kvp.Key;
                break;
            }
        }

        System.Random rand = new System.Random();
        int spawnerIndex = rand.Next(0, transforms[area].Count);
        while (!spawnIsClear(transforms[area][spawnerIndex].position) || transforms[area][spawnerIndex].position == prevPos)
        {
            areaRand = Random.value;
            foreach (KeyValuePair<string, float> kvp in cumulativeProbs)
            {
                if (kvp.Value > areaRand)
                {
                    area = kvp.Key;
                    break;
                }
            }
            spawnerIndex = rand.Next(0, transforms[area].Count);
        }

        int powerupIndex = rand.Next(0, powerups.Length);
        Instantiate(powerups[powerupIndex], transforms[area][spawnerIndex].position, Quaternion.identity);
    }

    private bool spawnIsClear(Vector3 position)
    {
        Collider[] hitColliders = Physics.OverlapSphere(position, 2f);
        foreach(Collider collider in hitColliders)
        {
            if (collider.gameObject.tag == "Powerup")
            {
                return false;
            }
        }
        return true;
    }

}
