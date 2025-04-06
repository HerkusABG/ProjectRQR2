using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    public float sensX;
    public float sensY;
    public Transform orientation;

    float xRotation;
    float yRotation;

    public float multiplier;

    [SerializeField]
    Movement movementAccess;

    Coroutine bounceCoroutine;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        multiplier = 1f;
        //StartCoroutine(MovementCoroutine(3f, transform.localPosition + new Vector3(0, 10, 0)));
    }


    public Transform target;

    public float speed;
    Quaternion towardsQuaternion;
    public bool coroutineRunning;
    private void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.fixedDeltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.fixedDeltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);




        if (movementAccess.horizontalInputFloat == 0)
        {
            target.localEulerAngles = new Vector3(target.localEulerAngles.x, target.localEulerAngles.y, 0);
        }
        else if (movementAccess.horizontalInputFloat == 1)
        {
            target.localEulerAngles = new Vector3(target.localEulerAngles.x, target.localEulerAngles.y, -2.5f);
        }
        else if (movementAccess.horizontalInputFloat == -1)
        {
            target.localEulerAngles = new Vector3(target.localEulerAngles.x, target.localEulerAngles.y, 2.5f);
        }
        target.rotation = Quaternion.Euler(xRotation, yRotation, target.localEulerAngles.z);
        towardsQuaternion = Quaternion.RotateTowards(transform.localRotation, target.localRotation, speed * Time.deltaTime);


        transform.rotation = Quaternion.Euler(xRotation, yRotation, towardsQuaternion.eulerAngles.z);
    }
    public void CameraBounce()
    {
        if(bounceCoroutine != null)
        {
            StopCoroutine(bounceCoroutine);
            transform.localPosition = new Vector3(0, 0, 0);
        }
        //if (!coroutineRunning)
        //{
            bounceCoroutine = StartCoroutine(CameraBounceCoroutine());
        //}
    }
    public IEnumerator CameraBounceCoroutine()
    {
        coroutineRunning = true;
        StartCoroutine(MovementCoroutine(0.05f, transform.localPosition + new Vector3(0, -0.4f, 0), false));
        //transform.localPosition += new Vector3(0, -0.4f, 0);
        yield return new WaitForSeconds(0.05f);
        yield return new WaitForEndOfFrame();
        StartCoroutine(MovementCoroutine(0.15f, transform.localPosition + new Vector3(0, 0.4f, 0), true));
        
    }
    private IEnumerator MovementCoroutine(float duration, Vector3 endPos, bool shouldDisableBool)
    {
        float myTime = 0;
        float fraction = myTime / duration;
        Vector3 startPos = transform.localPosition;
        while (myTime < duration)
        {
            myTime += Time.deltaTime;
            fraction = myTime / duration;
            yield return new WaitForEndOfFrame();
            transform.localPosition = Vector3.Lerp(startPos, endPos, fraction);
        }
        transform.localPosition = Vector3.Lerp(startPos, endPos, 1);
        if(shouldDisableBool)
        {
            coroutineRunning = false;
        }
    }

}
