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

    Animator enemyAnimator;

    public bool isPunching;

    public GameObject myMesh;

    public GameObject sphereReference;
    public Transform punchSphereLocation;

    public float punchRange;

    public bool isAgentStopped;
    public Transform knockbackRaycastTransform;

    public GameObject deathDummy;
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
       // if (Input.GetKeyDown(KeyCode.Y))
        //{
          // KnockbackDeath();
       // }
        if (isEnemyAlive)
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
                if (isAggressive)
                {
                    navmeshAgentAccess.isStopped = true;
                    isPunching = true;
                    enemyAnimator.SetBool("isPunching", true);
                }
            }
            isAgentStopped = navmeshAgentAccess.isStopped;
            debugFloat = Vector3.Dot(transform.forward, playerCameraTransform.forward);
            debugString = navmeshAgentAccess.remainingDistance.ToString();
            if (isEnemyAlive)
            {
                if (isAggressive)
                {
                    if (HasDirectLineWithPlayer())
                    {
                        canReachPlayer = NavMesh.CalculatePath(transform.position, playerTransformES.position, NavMesh.AllAreas, path);
                        //navmeshAgentAccess.SetPath(path);
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
                        Debug.Log("new 4");
                        canReachPlayer = NavMesh.CalculatePath(transform.position, lastLocation, NavMesh.AllAreas, path);
                        if (canReachPlayer)
                        {
                            navmeshAgentAccess.SetPath(path);
                        }
                        if (Vector3.Distance(transform.position, lastLocation) < 5)
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
                    else if (enemyBehaviourId == 2)
                    {
                        wanderingTime += Time.deltaTime;
                        if (wanderingTime >= wanderingTimeThreshold)
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
    }

    public void DidEnemyHitPlayer()
    {
       GameObject mySphere = Instantiate(sphereReference, punchSphereLocation.position + punchSphereLocation.forward, punchSphereLocation.rotation);
        mySphere.SetActive(true);
        mySphere.transform.localScale = new Vector3(punchRange * 1.5f, punchRange, punchRange * 3.5f);
    }
    public void CanContinueToPunch()
    {
        if(isEnemyAlive && isAggressive)
        {
            if (navmeshAgentAccess.remainingDistance >= navmeshAgentAccess.stoppingDistance)
            {
                Debug.Log("setting to false");
                isPunching = false;
                enemyAnimator.SetBool("isPunching", false);
                navmeshAgentAccess.isStopped = false;
            }
        }
    }
    private void EnemyAnimationCheck()
    {
        if (navmeshAgentAccess.velocity.magnitude <= 0)
        {
            myMesh.GetComponent<Renderer>().material.color = Color.red;
            enemyAnimator.SetBool("isWalking", false);
        }
        else
        {
            myMesh.GetComponent<Renderer>().material.color = Color.white;
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
        if(Physics.Raycast(transform.position, (playerTransformES.position - transform.position), out hitInfo, 35, playerDetectionLayerMask))
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
        if(isEnemyAlive)
        {
            isEnemyAlive = false;
            navmeshAgentAccess.isStopped = true;
            navmeshAgentAccess.enabled = false;
            
            KnockbackDeath();
            transform.position = new Vector3(0, -3000, 0);
            //enemyAnimator.Play("Base Layer.Z0_Death", 0, 0f);
        }
    }

    private void KnockbackDeath()
    {
        //Debug.Log("knockback death 1");
        //StartCoroutine(KnockbackCoroutine(10));
        deathDummy.transform.position = transform.position;
        deathDummy.SetActive(true);
        deathDummy.GetComponent<KnockbackEnemy>().DummySetUp();
    }

    private IEnumerator KnockbackCoroutine(float duration)
    {
        Debug.Log("knockback death 2");

        float timePassed = 0;
        RaycastHit knockbackHitInfo;
        RaycastHit downwardHitInfo;

        Vector3 endPoint;
        if(Physics.Raycast(knockbackRaycastTransform.position, -transform.forward, out knockbackHitInfo, 30, playerDetectionLayerMask))
        {
            Debug.Log("knockback death 3");
            if(Physics.Raycast(knockbackHitInfo.point, Vector3.down, out downwardHitInfo, Mathf.Infinity, playerDetectionLayerMask))
            {
                Debug.Log("knockback death 4");

                endPoint = downwardHitInfo.point + transform.forward * 3.5f;
                endPoint = new Vector3(endPoint.x, downwardHitInfo.point.y + transform.localScale.y * 0.5f, endPoint.z);

                target.position = endPoint;
            }
            else
            {
                Debug.Log("knockback death 5");

                endPoint = knockbackHitInfo.point + transform.forward * 3.5f;
                target.position = endPoint;
            }
        }
        else
        {

            endPoint = knockbackRaycastTransform.position - transform.forward * 7f;
            target.position = endPoint;
            endPoint = new Vector3(endPoint.x, transform.position.y, endPoint.z);

        }
        while (timePassed <= duration)
        {
            transform.position = Vector3.Slerp(transform.position, endPoint, timePassed / duration);
            yield return new WaitForEndOfFrame();
            timePassed += Time.deltaTime;
        }
    }

    private void ResetEnemy()
    {
        Debug.Log("reset");
        enemyAnimator.Play("Base Layer.ZC0_Idle", 0, 0f);
        isPunching = false;
        enemyAnimator.SetBool("isPunching", false);
        enemyAnimator.SetBool("isWalking", false);
        isAggressive = false;
        isEnemyAlive = true;
        transform.position = savedEnemyPosition;
        transform.rotation = savedEnemyRotation;
        navmeshAgentAccess.enabled = true;
        navmeshAgentAccess.isStopped = false;
        deathDummy.SetActive(false);
        navmeshAgentAccess.SetDestination(transform.position);
        if (enemyBehaviourId == 0)
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
