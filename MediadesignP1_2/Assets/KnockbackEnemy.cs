using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class KnockbackEnemy : MonoBehaviour
{

    Rigidbody myRigidbody;
    Animator myAnimator;
    public float force;
    bool isGrounded;
    public LayerMask dummyLayerMask;
    float fallTimer;
    public float gravity;
    public float gravityDelayValue;
    public bool debugBool;
    [SerializeField]
    ReferenceData referenceDataAccess;
    [SerializeField]
    ParticleSystem bloodParticleSystem;
    Coroutine bleedingCoroutine;
    public void DummySetUp()
    {
        if(bleedingCoroutine != null)
        {
            StopCoroutine(bleedingCoroutine);
        }
        if (!myRigidbody)
        {
            myRigidbody = GetComponent<Rigidbody>();
            myAnimator = GetComponentInChildren<Animator>();
        }
        KnockBackInstantiation();
    }
    /*void Update()
    {
        if(Input.GetKeyDown(KeyCode.Y))
        {
            KnockBackInstantiation();
        }
    } */

    private void FixedUpdate()
    {
        if (!GroundCheck())
        {
            fallTimer = fallTimer + Time.fixedDeltaTime;
            float newGravity = -Mathf.Pow(gravity, fallTimer - gravityDelayValue);
            myRigidbody.linearVelocity += new Vector3(0, newGravity, 0);
            myRigidbody.linearVelocity = new Vector3(myRigidbody.linearVelocity.x, Mathf.Clamp(myRigidbody.linearVelocity.y, -180, 1000), myRigidbody.linearVelocity.z);
        }
        else
        {
            fallTimer = 0;
        } 
    }
    private void KnockBackInstantiation()
    {
        bloodParticleSystem.Emit(250);
        bleedingCoroutine = StartCoroutine(TuneOutBleeding());
        Vector3 rot = Quaternion.LookRotation(referenceDataAccess.playerTransform.position - transform.position).eulerAngles;
        rot.x = rot.z = 0;
        transform.rotation = Quaternion.Euler(rot);
        myAnimator.Play("Base Layer.Z0_Death", 0, 0f);
        myRigidbody.AddForce((-transform.forward * 2.5f + transform.up * 0.5f) * force);
    }
    private IEnumerator TuneOutBleeding()
    {
        Debug.Log("ZIZI");
        var emissionVar = bloodParticleSystem.emission;
        emissionVar.rateOverTime = 45;
        yield return new WaitForSeconds(3);
        for (int i = 45; i > 0; i--)
        {
            emissionVar.rateOverTime = i;
            yield return new WaitForSeconds(0.1f);
        }
        StopCoroutine(bleedingCoroutine);
    }
    
    private bool GroundCheck()
    {
        Debug.DrawRay(transform.position - new Vector3(0, transform.localScale.y * 0.5f, 0), -transform.up, Color.green);
        if(Physics.Raycast(transform.position - new Vector3(0, transform.localScale.y * 0.5f, 0), -transform.up, 1, dummyLayerMask))
        {
            debugBool = true;
            return true;
        }
        else
        {
            debugBool = false;
            return false;
        }
    }
}
