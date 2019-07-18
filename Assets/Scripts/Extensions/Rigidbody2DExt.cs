using UnityEngine;
using UnityEngine.Audio;

public static class Rigidbody2DExt
{

    public static void AddExplosionForce(this Rigidbody2D rb, float explosionForce, Vector2 explosionPosition, float explosionRadius, float upwardsModifier = 0.0F, ForceMode2D mode = ForceMode2D.Force)
    {
        var explosionDir = rb.position - explosionPosition;
        var explosionDistance = explosionDir.magnitude;

        // Normalize without computing magnitude again
        if (upwardsModifier == 0)
            explosionDir /= explosionDistance;
        else
        {
            // From Rigidbody.AddExplosionForce doc:
            // If you pass a non-zero value for the upwardsModifier parameter, the direction
            // will be modified by subtracting that value from the Y component of the centre point.
            explosionDir.y += upwardsModifier;
            explosionDir.Normalize();
        }

        explosionDistance = Mathf.Min(.99f, explosionDistance);

        rb.AddForce(Mathf.Lerp(0, explosionForce, (explosionRadius - explosionDistance)) * explosionDir, mode);
    }

    public static AudioSource AddAS(this Rigidbody2D rb)
    {
        AudioSource AS = rb.gameObject.AddComponent<AudioSource>();
        AS.playOnAwake = false;
        return AS;
    }
}