using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] ReferenceData referenceDataAccess;

    NavMeshAgent navmeshAgentAccess;
    Transform playerTransformES;

    bool isAggressive;
    bool isEnemyAlive;

    Vector3 savedEnemyPosition;
    Quaternion savedEnemyRotation;

    private void Start()
    {
        
        navmeshAgentAccess = GetComponent<NavMeshAgent>();
        playerTransformES = referenceDataAccess.playerTransform;
        

        SaveEnemyData();
        ResetEnemy();
    }

    private void Update()
    {
        
        if(isAggressive)
        {
            navmeshAgentAccess.SetDestination(playerTransformES.position);
        }
        else if (Vector3.Distance(playerTransformES.position, transform.position) < 5)
        {
            Debug.Log("enemy angry");
            isAggressive = true;

        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            ResetEnemy();
        }
    }

    public void EnemyDeath()
    {
        isEnemyAlive = false;
        navmeshAgentAccess.isStopped = true;
    }
    private void ResetEnemy()
    {
        isAggressive = false;
        isEnemyAlive = true;
        transform.position = savedEnemyPosition;
        transform.rotation = savedEnemyRotation;
        navmeshAgentAccess.SetDestination(transform.position);
    }

    private void SaveEnemyData()
    {
         savedEnemyPosition = transform.position;
         savedEnemyRotation = transform.rotation;
    }
}
