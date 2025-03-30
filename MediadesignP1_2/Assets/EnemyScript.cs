using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    [SerializeField]
    ReferenceData referenceDataAccess;

    [SerializeField]
    Restarter restarterAccess;

    NavMeshAgent navmeshAgentAccess;
    Transform playerTransformES;

    bool isAggressive;
    bool isEnemyAlive;
    bool canReachPlayer;

    Vector3 savedEnemyPosition;
    Quaternion savedEnemyRotation;

    private NavMeshPath path;

    public int enemyBehaviourId;

    int patrolIndex;
    public List<Transform> patrolPointList;

    private void Start()
    {
        patrolIndex = 0;
        restarterAccess.restartEvent += ResetEnemy;
        path = new NavMeshPath();

        navmeshAgentAccess = GetComponent<NavMeshAgent>();
        playerTransformES = referenceDataAccess.playerTransform;
        
        SaveEnemyData();
        ResetEnemy();
    }

    private void Update()
    {      
        if(isAggressive)
        {
            canReachPlayer = NavMesh.CalculatePath(transform.position, playerTransformES.position, NavMesh.AllAreas, path);
            
            if (canReachPlayer)
            {
                navmeshAgentAccess.SetPath(path);
            }
            else
            {
                navmeshAgentAccess.SetDestination(transform.position + (playerTransformES.position - transform.position));
            }
        }
        else
        {
            if (Vector3.Distance(playerTransformES.position, transform.position) < 10)
            {
                Debug.Log("enemy angry");
                isAggressive = true;
            }
            else if (enemyBehaviourId == 1)
            {
                canReachPlayer = NavMesh.CalculatePath(transform.position, patrolPointList[patrolIndex].position, NavMesh.AllAreas, path);
                navmeshAgentAccess.SetPath(path);
                if (Vector3.Distance(transform.position, patrolPointList[patrolIndex].position) < 3)
                {
                    if (patrolIndex + 1 >= patrolPointList.Count)
                    {
                        patrolIndex = 0;
                    }
                    else
                    {
                        patrolIndex++;
                    }
                }
            }
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
        navmeshAgentAccess.isStopped = false;
        navmeshAgentAccess.SetDestination(transform.position);
    }

    private void SaveEnemyData()
    {
         savedEnemyPosition = transform.position;
         savedEnemyRotation = transform.rotation;
    }
}
