using UnityEngine;
using System.Collections;

public class ExampleClass : MonoBehaviour
{
    public float floatHeight;
    public float liftForce;
    public float damping;
    public Rigidbody2D rb2D;

    public LayerMask _layerMaskToSearch;
    void Start()
    {
        _layerMaskToSearch = LayerMask.GetMask("Players");
    }

    void FixedUpdate()
    {
        Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z) * Vector2.up, Color.white);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z) * Vector2.up, 100, _layerMaskToSearch);
        if (hit.collider != null)
        {
            Debug.Log(hit.collider.name);
            Debug.Log(hit.distance);
            Debug.Log(hit.collider.gameObject.layer);
        }
    }
    /*
    void FixedUpdate()
    {
        Ray mRay = new Ray(transform.position, Vector3.up); // Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z) *
        Debug.DrawRay(transform.position, Vector3.up, Color.white);
        RaycastHit hit;

        if (Physics.Raycast(mRay, out hit, _layerMaskToSearch))
        {
            Debug.Log(hit.collider.name);
            Debug.Log(hit.distance);
            Debug.Log(hit.collider.gameObject.layer);
        }
    }*/
}