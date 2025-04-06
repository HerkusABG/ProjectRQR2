using UnityEngine;

public class EnemyAnimationLink : MonoBehaviour
{
    EnemyScript enemyScriptAccess;

    private void Start()
    {
        enemyScriptAccess = GetComponentInParent<EnemyScript>();
    }
    public void PunchEvent()
    {
        enemyScriptAccess.DidEnemyHitPlayer();
    }

    public void ShouldRepeatPunchVoid()
    {
        enemyScriptAccess.CanContinueToPunch();
    }
}
