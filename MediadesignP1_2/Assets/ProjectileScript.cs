using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    //[HideInInspector] 
    public Transform projectileTarget;
    public float projectileSpeed;

    public float debugFloat;

    [HideInInspector] public int projectileYcoordinate;
    bool shouldBeMoving;
    bool valuableHit;


    public float timeFloat;
    Vector3 startPosition;
    Vector3 target;
    float timeToReachTarget;

    private void Awake()
    {
        shouldBeMoving = false;
    }

    private void Update()
    {
        if(shouldBeMoving)
        {
            timeFloat += Time.deltaTime / timeToReachTarget;
            timeFloat = Mathf.Clamp(timeFloat, 0, 1);
            Vector3 startPositionNulled = new Vector3(startPosition.x, 2, startPosition.z);
            Vector3 targetPositionNulled = new Vector3(target.x, 2, target.z);
            transform.position = Vector3.Lerp(startPositionNulled, targetPositionNulled, timeFloat);
            //float restrictedFloat;
            transform.position = new Vector3(transform.position.x, timeFloat * 5 , transform.position.z);
        }
    }

    public void SetDestination(Vector3 destination, bool arWeHittingValuable)
    {
        valuableHit = arWeHittingValuable;
        float distanceToGoal = Vector3.Distance(transform.position, destination);
        timeToReachTarget = (distanceToGoal * 0.3f) / 10;
        shouldBeMoving = true;
        timeFloat = 0;
        startPosition = transform.position;
        //timeToReachTarget = time;
        target = destination;

        Debug.Log(destination + " target");
        Debug.Log(transform.position + "locator");
    }

    private void OnTriggerEnter(Collider other)
    {
        // if(valuableHit)
        // {
            Debug.Log("HHH");
            Invoke("DestroyProjectile", 0.3f);
            shouldBeMoving = false;
       // }
    }

    private void DestroyProjectile()
    {
        Destroy(gameObject);
    }
    /*
          public void ProjectileVoid(Vector3 targetLocation)
    {
        float distanceVector = Vector3.Distance(transform.position, targetLocation);
        distanceVector *= 0.5f;
    }
    */
}
