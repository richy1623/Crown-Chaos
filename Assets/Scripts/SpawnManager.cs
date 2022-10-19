using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    //powerup prefabs
    public GameObject speed;
    public GameObject ghost;
    public GameObject invincible;
    public GameObject buckshot;
    public GameObject ammo;
    public GameObject sight;
    
    // Start is called before the first frame update
    void Start()
    {
        //forest dimensions.
        float forest_spawnPosx = Random.Range(-65, -27);
        float forest_spawnPosZ = Random.Range(-29, 34);

        //village dimensions - not sure of this one
        float village_spawnPosX = Random.Range(-30,70);
        float village_spawnPosZ = Random.Range(-29,34);

        //farm dimensions - not sure of this one
        float farm_spawnPosX = Random.Range(60,90);
        float farm_spawnPosZ = Random.Range(-29,29);
        
        //spawn point
        Vector3 randomPosForest = new Vector3(forest_spawnPosx,-0.4f,forest_spawnPosZ);
        Vector3 randomPosVillage = new Vector3(village_spawnPosX,-0.4f,village_spawnPosZ);
        Vector3 randomPosFarm = new Vector3(farm_spawnPosX,-0.4f,farm_spawnPosZ);

        //spawn powerups
        Instantiate(sight, randomPosForest, sight.transform.rotation);
        Instantiate(ghost, randomPosForest, ghost.transform.rotation);
        Instantiate(buckshot, randomPosVillage, buckshot.transform.rotation);
        Instantiate(ammo, randomPosVillage, ammo.transform.rotation);
        Instantiate(speed, randomPosFarm, speed.transform.rotation);
        Instantiate(invincible, randomPosFarm, invincible.transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
