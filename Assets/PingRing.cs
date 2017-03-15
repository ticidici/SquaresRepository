using UnityEngine;
using System.Collections;

public class PingRing : MonoBehaviour
{

    public static PingRing Create(Vector3 pos)
    {
        PingRing pr = new GameObject("Effect").AddComponent<PingRing>();

        pr.transform.position = pos + Vector3.back * 5;//Heights.PingRing;

        return pr;
    }
    
    IEnumerator Start()
    {
        LineRenderer lr = gameObject.AddComponent<LineRenderer>();

        lr.material = Resources.Load<Material>("Effect1");

        int NumVerts = 40;
        lr.SetVertexCount(NumVerts + 1);
        lr.useWorldSpace = true;

        float w = 0.1f;
        lr.SetWidth(w, w);

        // my player is at the 0,0,0, that's why this works
        float DistanceToPlayer = transform.position.magnitude;
        float SpeedOfPing = 3f;

        DistanceToPlayer = 1.5f; // some extra distance so the ping goes "past" us

        for (float radius = 0;
             radius < DistanceToPlayer;
             radius += SpeedOfPing * Time.deltaTime)
        {
            for (int vertno = 0; vertno <= NumVerts; vertno++)
            {
                float angle = (vertno * Mathf.PI * 2) / NumVerts;
                Vector3 pos = transform.position + new Vector3(
                        Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
                lr.SetPosition(vertno, pos);
            }
            yield return null;
        }
        Destroy(gameObject);
    }
}