using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DeathScript : MonoBehaviour
{
    public int deathForce;

    public ReferenceData referenceDataAccess;

    private Rigidbody playerRigidbody;
    private Quaternion savedPlayerRotQuaternion;
    private Vector3 savedPlayerPosVector;

    Movement movementAccess;
    UiScript uiScriptAccess;
    AreaCheckScript areaCheckScriptAccess;

    //[SerializeField]
    public Restarter restarterAccess;

    [HideInInspector]
    public static bool isAlive;

    void Start()
    {
        restarterAccess.restartEvent += ResetPlayerInvocation;
        isAlive = true;
        playerRigidbody = GetComponent<Rigidbody>();
        movementAccess = GetComponent<Movement>();
        areaCheckScriptAccess = GetComponent<AreaCheckScript>();
        savedPlayerRotQuaternion = transform.rotation;
        savedPlayerPosVector = transform.position;
        uiScriptAccess = referenceDataAccess.uiScriptReference;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            KillPlayer(new Vector3(10, 0, 10));
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 10)
        {
            KillPlayer(collision.gameObject.transform.position);
        }
    }
    public void KillPlayer(Vector3 deathPosition)
    {
        isAlive = false;
        playerRigidbody.freezeRotation = false;

        Vector3 deathDirection = (transform.position - deathPosition);

        playerRigidbody.AddForce(deathDirection * deathForce, ForceMode.Impulse);

        TriggerComponentDeath();
    }
    public void KillPlayerSilent()
    {
        isAlive = false;
        playerRigidbody.freezeRotation = false;

        TriggerComponentDeath();
    }

    
    private void ResetPlayerInvocation()
    {
        uiScriptAccess.ToggleDeathScreen(true);
        Invoke("ResetPlayer", 0.1f);
    }

    private void ResetPlayer()
    {
        //movementAccess.enabled = true;
        isAlive = true;
        playerRigidbody.freezeRotation = true;
        uiScriptAccess.ToggleDeathScreen(false);
        transform.rotation = savedPlayerRotQuaternion;
        transform.position = savedPlayerPosVector;

        ResetComponents();
    }

    private void TriggerComponentDeath()
    {
        areaCheckScriptAccess.AreaCheckDeath();
    }
    private void ResetComponents()
    {
        areaCheckScriptAccess.AreaCheckReset();
    }
}
