using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchScript : MonoBehaviour
{
    SphereCollider playerSphereCollider;
    CapsuleCollider playerCapsuleCollider;
    Movement movementAccess;
    AreaCheckScript areaCheckScriptAccess;

    public LayerMask crouchLayers;

    [HideInInspector]
    public bool areWeCrouching;

    float savedUncrounchHeight;

    
    private void Start()
    {
        areWeCrouching = false;
        //savedUncrounchHeight = transform.position.y;
        playerSphereCollider = GetComponent<SphereCollider>();
        playerCapsuleCollider = GetComponent<CapsuleCollider>();
        movementAccess = GetComponent<Movement>();
        areaCheckScriptAccess = GetComponent<AreaCheckScript>();


        RaycastHit startHitInfo;
        if (Physics.Raycast(transform.position, -transform.up, out startHitInfo, 50, crouchLayers))
        {
            // transform.position = startHitInfo.point + new Vector3(0, savedUncrounchHeight + 0.01f, 0);
            savedUncrounchHeight = Vector3.Distance(transform.position, startHitInfo.point);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            if(DeathScript.isAlive)
            {
                if (areWeCrouching)
                {
                    if (areaCheckScriptAccess.CeilingCheck())
                    {
                        //transform.position = new Vector3(transform.position.x, savedUncrounchHeight, transform.position.z);
                        RaycastHit hit;
                        if(Physics.Raycast(transform.position, -transform.up, out hit, 50, crouchLayers))
                        {
                            transform.position = hit.point + new Vector3(0, savedUncrounchHeight + 0.01f, 0);
                        }
                        

                        areWeCrouching = false;
                        //transform.position += new Vector3(0, 1.05f, 0);
                        
                        playerCapsuleCollider.height = 3;
                        playerSphereCollider.center = new Vector3(0, -1.61f, 0);
                        
                        
                        areaCheckScriptAccess.ChangeRaycastSourceLocation(-1.5f);
                        movementAccess.ChangeSpeedLimit(movementAccess.savedSpeedLimit);
                    }
                }
                else
                {
                    areWeCrouching = true;

                    
                    playerCapsuleCollider.height = 1;
                    playerSphereCollider.center = new Vector3(0, -0.5f, 0);

                    areaCheckScriptAccess.ChangeRaycastSourceLocation(-0.5f);
                    movementAccess.ChangeSpeedLimit(20);        
                }
            }
            
        }
    }
}
