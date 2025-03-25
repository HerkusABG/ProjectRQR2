using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyNavigation : MonoBehaviour
{
    [SerializeField] ReferenceData referenceDataAccess;
    public string word;

    Transform playerTransformForEnemy;

    void Update()
    {
        if(Vector3.Distance(transform.position, playerTransformForEnemy.position) < 3)
        {

        }
    }
}
