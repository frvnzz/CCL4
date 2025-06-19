using UnityEngine;

public class Explosive : MonoBehaviour
{
    public float explosionRadius = 5f;
    public int explosionDamage = 50;
    public ParticleSystem explosionEffect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Explode()
    {
        // Play explosion effect

        ParticleSystem explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        explosion.Play();
        AkUnitySoundEngine.PostEvent("Play_barrel_explosion", gameObject);
        // Destroy(explosion.gameObject, explosion.main.duration);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                var enemy = hitCollider.GetComponent<AIController>();
                if (enemy != null)
                {
                    enemy.TakeDamage(explosionDamage);
                }
            }
        }
        Debug.Log("Explosion triggered!");

        Destroy(gameObject);
    }
}
