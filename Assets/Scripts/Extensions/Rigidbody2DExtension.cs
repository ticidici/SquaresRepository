using UnityEngine;
using System.Collections;

public static class Rigidbody2DExtension
{
    public static void AddExplosionForce(this Rigidbody2D body, float explosionForce, Vector3 explosionPosition, float explosionRadius)
    {
        var dir = (body.transform.position - explosionPosition);
        float wearoff = 1 - (dir.magnitude / explosionRadius);
        body.AddForce(dir.normalized * explosionForce * wearoff);
    }

    public static void LookAt(this Rigidbody2D body, Vector3 targetPosition)
    {        
        float AngleRad = Mathf.Atan2(targetPosition.y - body.position.y, targetPosition.x - body.position.x);
        // Get Angle in Degrees
        float AngleDeg = Mathf.Rad2Deg * AngleRad - 90;
        // Rotate Object
        body.MoveRotation(AngleDeg);
    }
}
