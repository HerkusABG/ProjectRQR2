using UnityEngine;

public class Restarter : MonoBehaviour
{
    public delegate void RestartAction();
    public event RestartAction restartEvent;
    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            if (restartEvent != null)
            {
                restartEvent();
            }
        }
    }
}
