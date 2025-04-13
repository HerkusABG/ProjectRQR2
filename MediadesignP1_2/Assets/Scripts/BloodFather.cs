using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodFather : MonoBehaviour
{
    public List<GameObject> kraujoDecals;
    public List<GameObject> deletableDecals;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void SpawnSomeBlood(Transform decalLocation, int a)
    {
        if(a == 0)
        {
            GameObject kraujoKlonas = Instantiate(kraujoDecals[Random.Range(0, kraujoDecals.Count)], decalLocation.position, Quaternion.Euler(0, Random.Range(0, 360), 0), transform);
            deletableDecals.Add(kraujoKlonas);
        }
        else
        {
            GameObject kraujoKlonas = Instantiate(kraujoDecals[Random.Range(0, kraujoDecals.Count)], decalLocation.position + new Vector3(0, .08f, 0), Quaternion.Euler(0, Random.Range(0, 360), 0), transform);
            deletableDecals.Add(kraujoKlonas);
        }
        //checkpointManagerAccess.roomObjectsAccess[checkpointManagerAccess.currentRoomInt].lygioDaiktai.Add(kraujoKlonas);

    }
}
