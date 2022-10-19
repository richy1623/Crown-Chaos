using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public Vector3 pos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    bool checkValid()
    {
        Collider[] hitColliders = Physics.OverlapSphere(pos, 10);
        foreach (Collider hitInfo in hitColliders){

        }
        return true;
    }
}
