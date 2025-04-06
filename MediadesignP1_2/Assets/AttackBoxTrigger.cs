using UnityEngine;
using UnityEditor;

public class AttackBoxTrigger : MonoBehaviour
{
    private void Awake()
    {
        Invoke("DestroyMe", 0.1f);
    }
    private void OnTriggerEnter(Collider other)
    {
        EditorApplication.isPaused = true;
        //Debug.Log(other.transform.root.gameObject.name);
    }
    private void DestroyMe()
    {
        Destroy(gameObject);
    }
}
