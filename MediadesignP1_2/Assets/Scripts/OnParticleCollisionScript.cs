using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class OnParticleCollisionScript : MonoBehaviour
{
    public ParticleSystem part;
    public List<ParticleCollisionEvent> collisionEvents;
    public ParticleSystem anotherParticleSystem;
    public GameObject temporaryGameObject;
    public GameObject prefab;
    void Start()
    {
        part = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);

        for(int i = 0; i < numCollisionEvents; i++)
        {
            GenerateParticle(other, collisionEvents[i].intersection, -collisionEvents[i].normal);
        }
    }

    public int particleCount;
    public void GenerateParticle(GameObject collidedWith, Vector3 position, Vector3 normal)
	{
		//var bloodGO = Instantiate(prefab);
		//bloodGO.transform.position = position;
		//bloodGO.transform.forward = normal;

		//gavimas particle
		anotherParticleSystem.Emit(1);
        Particle[] particles = new Particle[anotherParticleSystem.particleCount];
        particleCount = anotherParticleSystem.GetParticles(particles);

        //keiciam konkretu paskutini particle
        var lastParticle = particles[particles.Length - 1];
        lastParticle.position = position;

        temporaryGameObject.transform.forward = normal;
        lastParticle.rotation3D = -1 * temporaryGameObject.transform.eulerAngles;

        particles[particles.Length - 1] = lastParticle;

        //priskyriomas visu particle
        anotherParticleSystem.SetParticles(particles, particleCount);
    }
}
