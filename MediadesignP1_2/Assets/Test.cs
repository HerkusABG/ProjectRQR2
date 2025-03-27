using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Test : MonoBehaviour
{

    [SerializeField]
    Movement movementAccess;

    public Transform target;

    // Angular speed in degrees per sec.
    public float speed;
    private void Update()
    {
        /* Vector3 targetDirection = (transform.position + new Vector3(0, 0, movementAccess.horizontalInputFloat * -1 * 4)) - transform.position;
         //Quaternion endQuat = Quaternion.Euler(0, 0, movementAccess.horizontalInputFloat * -1 * 15);

         //Vector3 newDirection = Vector3.RotateTowards(movementAccess.transform.forward, targetDirection, 2f * Time.deltaTime, 0.0f);
         Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, 2f * Time.deltaTime, 0.0f);
         Debug.DrawRay(transform.position, newDirection, Color.red);
         transform.rotation = Quaternion.LookRotation(newDirection); */
        // RotateZAxis();
        /* if(Input.GetKeyDown(KeyCode.D))
         {
             StartCoroutine(CameraRotationCoroutine(3, transform.eulerAngles, 15));
         } */
        if(movementAccess.horizontalInputFloat == 0)
        {
            target.localEulerAngles = new Vector3(target.localEulerAngles.x, target.localEulerAngles.y, 0);
        }
        else if (movementAccess.horizontalInputFloat == 1)
        {
            target.localEulerAngles = new Vector3(target.localEulerAngles.x, target.localEulerAngles.y, -15);
        }
        else if (movementAccess.horizontalInputFloat == -1)
        {
            target.localEulerAngles = new Vector3(target.localEulerAngles.x, target.localEulerAngles.y, 15);
        }
        //var step = speed * Time.deltaTime;
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, target.localRotation, speed * Time.deltaTime);
    }


    private IEnumerator CameraRotationCoroutine(float duration, Vector3 startRot, float degreeInput)
    {
        Debug.Log(startRot.z);
        Debug.Log(degreeInput);
        if(startRot.z != degreeInput)
        {
            Debug.Log("rotating");
            Vector3 endRot = startRot + new Vector3(0, 0, degreeInput);
            float myTime = 0;
            float fraction = myTime / duration;
            while (myTime < duration)
            {
                transform.eulerAngles = Vector3.Lerp(startRot, endRot, fraction);
                myTime += Time.deltaTime;
                fraction = myTime / duration;
                yield return new WaitForEndOfFrame();
            }
            transform.eulerAngles = Vector3.Lerp(startRot, endRot, 1);

        }
        //transform.localEulerAngles = new Vector3(0, 0, -15);


    }
}
