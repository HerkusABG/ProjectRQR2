using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellScript : MonoBehaviour
{
    bool isSpellReady;

    
    public Transform spellTargetTransform;
    Transform projectileDestinationReference;
    [HideInInspector] public Vector3 mouseLocVector;

    [SerializeField] GameObject projectileReference;
    private void Start()
    {
        
    }
    private void Update()
    {
        RaycastHit wizardHit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out wizardHit, 100))
        {
            //Debug.Log("hit!!");
            mouseLocVector = wizardHit.point;
            if (wizardHit.transform.gameObject.layer == 7)
            {
                Debug.Log("njnkd");
                spellTargetTransform.position = new Vector3(wizardHit.transform.position.x, spellTargetTransform.position.y, wizardHit.transform.position.z);
                projectileDestinationReference = wizardHit.transform;
            }
            else
            {
                projectileDestinationReference = null;
                spellTargetTransform.position = mouseLocVector;
                //targetTransformReference = null;
            }
        }
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            GameObject clonedProjectile = Instantiate(projectileReference, transform.position, Quaternion.identity);
            clonedProjectile.SetActive(true);
            if (projectileDestinationReference == null)
            {
                clonedProjectile.GetComponent<ProjectileScript>().SetDestination(spellTargetTransform.position, false);//+ new Vector3 (0, spellTargetTransform.transform.position.y, 0), false);
            }
            else
            {
                clonedProjectile.GetComponent<ProjectileScript>().SetDestination(projectileDestinationReference.position, true);
            }
        }
    }
}
