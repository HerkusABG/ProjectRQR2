using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDoorScript : MonoBehaviour
{
    public Vector3 savedDoorPos, savedDoorRot;
    public Rigidbody rigidbodyAccess;
    private void Start()
    {
        rigidbodyAccess = GetComponent<Rigidbody>();
        savedDoorPos = transform.position;
        savedDoorRot = transform.eulerAngles;
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            rigidbodyAccess.isKinematic = true;
            rigidbodyAccess.useGravity = false;
            transform.position = savedDoorPos;
            transform.eulerAngles = savedDoorRot;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Leg") )//&& GetComponent<BetterEnemyCollision>().arNumuse == false)
        {
            rigidbodyAccess.isKinematic = false;
            rigidbodyAccess.useGravity = true;
            rigidbodyAccess.AddForce(-other.transform.forward * 500);
        }
    }

}
