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
    AreaCheckScript areaCheckAccess;
    Transform playerTransformES;
    Transform playerCameraTransform;

    bool isAggressive;
    bool isEnemyAlive;
    public bool canReachPlayer;

    Vector3 savedEnemyPosition;
    Quaternion savedEnemyRotation;

    private NavMeshPath path;

    float rayTrueDistance = 0;
    //0 - standing still
    //1 - patrolling
    public int enemyBehaviourId;

    int patrolIndex;
    int patrolSpeed = 4;
    int aggressiveSpeed = 10;
    public List<Transform> patrolPointList;

    public float debugFloat;
    public string debugString;

    float wanderingTime = 0;
    float wanderingTimeThreshold = 1f;

    public LayerMask playerDetectionLayerMask;

    public Transform target;
    Vector3 lastLocation;
    private void Start()
    {
        playerCameraTransform = referenceDataAccess.cameraReference;
        patrolIndex = 0;
        restarterAccess.restartEvent += ResetEnemy;
        path = new NavMeshPath();

        navmeshAgentAccess = GetComponent<NavMeshAgent>();
        playerTransformES = referenceDataAccess.playerTransform;
        if(enemyBehaviourId == 1)
        {
            navmeshAgentAccess.speed = patrolSpeed;
        }
        SaveEnemyData();
        ResetEnemy();
    }

    private void Update()
    {
        if (!navmeshAgentAccess.pathPending && navmeshAgentAccess.remainingDistance <= navmeshAgentAccess.stoppingDistance)
        {
            Vector3 desiredDirection = navmeshAgentAccess.steeringTarget - transform.position;
            desiredDirection.y = 0;
            if (desiredDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(desiredDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }
        debugFloat = Vector3.Dot(transform.forward, playerCameraTransform.forward);
        if(isEnemyAlive)
        {
            if (isAggressive)
            {
                if(HasDirectLineWithPlayer())
                {
                    canReachPlayer = NavMesh.CalculatePath(transform.position, playerTransformES.position, NavMesh.AllAreas, path);
                    navmeshAgentAccess.SetPath(path);
                    if (canReachPlayer)
                    {
                        navmeshAgentAccess.SetPath(path);
                    }
                    else if (NavMesh.CalculatePath(transform.position, referenceDataAccess.playerGroundedLocation, NavMesh.AllAreas, path))
                    {
                        navmeshAgentAccess.SetPath(path);
                    }
                    else
                    {
                        navmeshAgentAccess.SetDestination(transform.position + (playerTransformES.position - transform.position));
                    }
                    lastLocation = playerTransformES.position;
                }
                else
                {
                    navmeshAgentAccess.SetDestination(lastLocation);
                    if(Vector3.Distance(transform.position, lastLocation) < 5)
                    {
                        if (!HasDirectLineWithPlayer())
                        {
                            isAggressive = false;
                            enemyBehaviourId = 2;
                        }
                    }
                }
            }
            else
            {
                if (Vector3.Distance(playerTransformES.position, transform.position) < 18 && Vector3.Dot(transform.forward, playerCameraTransform.forward) <= 0.3f && HasDirectLineWithPlayer())
                {
                    Debug.Log("enemy angry");
                    isAggressive = true;
                    navmeshAgentAccess.speed = aggressiveSpeed;
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
                else if(enemyBehaviourId == 2)
                {
                    wanderingTime += Time.deltaTime;
                    if(wanderingTime >= wanderingTimeThreshold)
                    {
                        wanderingTimeThreshold = Random.Range(1, 3f);
                        wanderingTime = 0;
                        GoToNewSpot();
                    }
                }
            }
        }
    }
    private void UniqueDrawRay()
    {

    }
    private void GoToNewSpot()
    {
        Debug.Log("Go to new spot");
        RaycastHit hitInfo;
        Vector3 enemyDirection;
        Vector3 goVectorPos = transform.position;
        enemyDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        if (Physics.Raycast(transform.position, enemyDirection, out hitInfo, 20f, playerDetectionLayerMask))
        {
            Debug.Log("console");
            Debug.DrawRay(transform.position, enemyDirection * 5, Color.white, 5);
            goVectorPos = new Vector3(hitInfo.point.x, transform.position.y, hitInfo.point.z);
            rayTrueDistance = Vector3.Distance(transform.position, goVectorPos);
            target.position = hitInfo.point;
        }
        else
        {
            goVectorPos = transform.position + enemyDirection * 20;
            //goVectorPos = new Vector3(hitInfo.point.x, transform.position.y, hitInfo.point.z);
            Debug.DrawRay(transform.position, enemyDirection * 5, Color.red, 5);
           
        }
       /* do
        {
            
        } while (rayTrueDistance < 2.5f); */
        
        
        navmeshAgentAccess.SetDestination(goVectorPos);
    }
    private bool HasDirectLineWithPlayer()
    {
        RaycastHit hitInfo;
        if(Physics.Raycast(transform.position, (playerTransformES.position - transform.position), out hitInfo, Mathf.Infinity, playerDetectionLayerMask))
        {
            Debug.Log(hitInfo.transform.gameObject);
            if(hitInfo.transform.GetComponent<Movement>())
            {
                return true;

            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
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
        if (enemyBehaviourId == 1)
        {
            //patrolling
            navmeshAgentAccess.speed = patrolSpeed;
        }
        else
        {
            //standstill
            navmeshAgentAccess.SetDestination(transform.position);
        }
    }

    private void SaveEnemyData()
    {
         savedEnemyPosition = transform.position;
         savedEnemyRotation = transform.rotation;
    }
}
