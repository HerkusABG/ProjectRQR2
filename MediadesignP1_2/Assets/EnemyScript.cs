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

    public bool isAggressive;
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

    //[SerializeField]
    Animator enemyAnimator;

    public bool isPunching;

    public GameObject myMesh;

    public GameObject sphereReference;
    public Transform punchSphereLocation;

    public float punchRange;

    public bool isAgentStopped;
    private void Start()
    {
        enemyAnimator = GetComponentInChildren<Animator>();
        playerCameraTransform = referenceDataAccess.cameraReference;
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
        if (!navmeshAgentAccess.pathPending && navmeshAgentAccess.remainingDistance <= navmeshAgentAccess.stoppingDistance)
        {
            
            Vector3 desiredDirection = navmeshAgentAccess.steeringTarget - transform.position;
            desiredDirection.y = 0;
            if (desiredDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(desiredDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
            Debug.Log("Punching distance");
            if (isAggressive)
            {
                navmeshAgentAccess.isStopped = true;
                isPunching = true;
                enemyAnimator.SetBool("isPunching", true);
            }
        }
        /*else
        {
            isPunching = false;
            enemyAnimator.SetBool("isPunching", false);
        } */
        isAgentStopped = navmeshAgentAccess.isStopped;
        debugFloat = Vector3.Dot(transform.forward, playerCameraTransform.forward);
        debugString = navmeshAgentAccess.remainingDistance.ToString();
        if(isEnemyAlive)
        {
            if (isAggressive)
            {
                if(HasDirectLineWithPlayer())
                {
                    canReachPlayer = NavMesh.CalculatePath(transform.position, playerTransformES.position, NavMesh.AllAreas, path);
                    //navmeshAgentAccess.SetPath(path);
                    if (canReachPlayer)
                    {
                        Debug.Log("new");
                        navmeshAgentAccess.SetPath(path);

                    }
                    else if (NavMesh.CalculatePath(transform.position, referenceDataAccess.playerGroundedLocation, NavMesh.AllAreas, path))
                    {
                        Debug.Log("new 2");
                        navmeshAgentAccess.SetPath(path);
                    }
                    else
                    {
                        Debug.Log("new 3");
                        navmeshAgentAccess.SetDestination(transform.position + (playerTransformES.position - transform.position));
                    }
                    lastLocation = playerTransformES.position;
                }
                else
                {
                    Debug.Log("new 4");
                    canReachPlayer = NavMesh.CalculatePath(transform.position, lastLocation, NavMesh.AllAreas, path);
                    if (canReachPlayer)
                    {
                        Debug.Log("new 5");
                        navmeshAgentAccess.SetPath(path);
                    }
                    else
                    {
                        Debug.Log("new 6");
                    }
                    //navmeshAgentAccess.SetDestination(lastLocation);
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
                    isAggressive = true;
                    navmeshAgentAccess.speed = aggressiveSpeed;
                    navmeshAgentAccess.isStopped = false;
                }
                else if (enemyBehaviourId == 1)
                {
                    canReachPlayer = NavMesh.CalculatePath(transform.position, patrolPointList[patrolIndex].position, NavMesh.AllAreas, path);
                    navmeshAgentAccess.SetPath(path);
                    if (Vector3.Distance(transform.position, patrolPointList[patrolIndex].position) < navmeshAgentAccess.stoppingDistance + 0.5f)
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
                        wanderingTimeThreshold = Random.Range(1f, 5f);
                        wanderingTime = 0;
                        GoToNewSpot();
                    }
                }
            }
            EnemyAnimationCheck();
        }
    }

    public void DidEnemyHitPlayer()
    {
       GameObject mySphere = Instantiate(sphereReference, punchSphereLocation.position + punchSphereLocation.forward, punchSphereLocation.rotation);
        mySphere.SetActive(true);
        mySphere.transform.localScale = new Vector3(punchRange * 1.5f, punchRange, punchRange * 3.5f);
    }
    public void CanContinueToPunch()
    {
       // Debug.Log($"Remaining distance is: {navmeshAgentAccess.remainingDistance}");
       // Debug.Log($"Stopping distance is: {navmeshAgentAccess.stoppingDistance}");

        if (navmeshAgentAccess.remainingDistance >= navmeshAgentAccess.stoppingDistance)
        {
            Debug.Log("setting to false");
            isPunching = false;
            enemyAnimator.SetBool("isPunching", false);
            navmeshAgentAccess.isStopped = false;
        }
    }
    private void EnemyAnimationCheck()
    {
        if (navmeshAgentAccess.velocity.magnitude <= 0)
        {
            myMesh.GetComponent<Renderer>().material.color = Color.red;
            Debug.Log("falsing");
            enemyAnimator.SetBool("isWalking", false);
        }
        else
        {
            myMesh.GetComponent<Renderer>().material.color = Color.white;
            Debug.Log("trueing");
            enemyAnimator.SetBool("isWalking", true);
        }
    }
    private void GoToNewSpot()
    {
        RaycastHit hitInfo;
        Vector3 enemyDirection;
        Vector3 goVectorPos = transform.position;
        enemyDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        if (Physics.Raycast(transform.position, enemyDirection, out hitInfo, 20f, playerDetectionLayerMask))
        {
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
        navmeshAgentAccess.SetDestination(goVectorPos);
    }
    private bool HasDirectLineWithPlayer()
    {
        RaycastHit hitInfo;
        if(Physics.Raycast(transform.position, (playerTransformES.position - transform.position), out hitInfo, Mathf.Infinity, playerDetectionLayerMask))
        {
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
        isPunching = false;
        isAggressive = false;
        isEnemyAlive = true;
        transform.position = savedEnemyPosition;
        transform.rotation = savedEnemyRotation;
        navmeshAgentAccess.isStopped = false;
        
        if(enemyBehaviourId == 0)
        {
            //standstill
            navmeshAgentAccess.isStopped = true;

           // navmeshAgentAccess.SetDestination(transform.position);
        }
        else if(enemyBehaviourId == 1)
        {
            //patrolling
            navmeshAgentAccess.speed = patrolSpeed;
        }
        else if (enemyBehaviourId == 2)
        {
            navmeshAgentAccess.speed = patrolSpeed;
        }
    }

    private void SaveEnemyData()
    {
         savedEnemyPosition = transform.position;
         savedEnemyRotation = transform.rotation;
    }
}
