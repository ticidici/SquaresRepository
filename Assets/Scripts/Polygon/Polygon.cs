using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(SquareController))]
public abstract class Polygon : MonoBehaviour, IAttachable
{
    public AttachPoint _AttachPointPrefab;
    public GameObject _superPolygonPrefab;

    // Parametres generals
    public float _xForce = 20f;
    public float _yForce = 16f;
    public float _magnetWaitTime = 1.5f;

    public int Id { get; set; }
    public SuperPolygon CurrentSuperSquare { get; set; }

    public static int ID_COUNT = 0;//ho he fet public per poder-ho resetejar
    protected float _length = 1f;
    public  AttachPoint[] _attachPoints; // protected
    public float altitude; // protected

    protected GameManager _gameManager;

    #region Protected Methods

    protected abstract void SetAttachPoints();

    protected void OnEnable()
    {
        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager != null)
            _gameManager.AddPlayerToGame();//¡¡Pressuposa que tots els polygons són jugadors, canviar si cal!!
    }

    protected void OnDisable()
    {
        if (_gameManager != null)
            _gameManager.PolygonKilled();
    }

    //TODO Donar punts per temps transcorregut a FixedUpdate per exemple
    #endregion

    #region Public Methods

    public AttachPoint GetAttachPointClosestTo(Vector3 point) // Mirar scope per determinar public o private
    {
        AttachPoint closest = _attachPoints[0];

        float minimumDistance = Vector3.Distance(point, closest.transform.position);

        for (int i = 1; i < _attachPoints.Length; i++)
        {
            float currentDistance = Vector3.Distance(point, _attachPoints[i].transform.position);
            if (currentDistance < minimumDistance)
            {
                minimumDistance = currentDistance;
                closest = _attachPoints[i];
            }
        }
        return closest;
    }

    public bool isLeaf() // Mirar-ho amb mes deteniment
    {
        int totalBusy = 0;
        foreach (AttachPoint item in _attachPoints)
        {
            if (item.isBusy)
                totalBusy++;
        }
        return (totalBusy == 1);
    }

    public void AssignSuperPolygon()
    {
        CurrentSuperSquare = PoolManager.SpawnObject(_superPolygonPrefab, transform.position, Quaternion.identity).GetComponent<SuperPolygon>();
        CurrentSuperSquare.Add(this);
    }

    public virtual void AttachTo(Polygon target)
    {
        AttachPoint myClosestAttachPoint = GetAttachPointClosestTo(target.transform.position);
        AttachPoint targetClosestAttachPoint = target.GetComponent<Polygon>().GetAttachPointClosestTo(myClosestAttachPoint.transform.position); // Falta veure que no estigui ocupat!!!!

        transform.parent = target.transform.parent;

       // Debug.DrawLine(target.transform.localPosition,  (Quaternion.AngleAxis(targetClosestAttachPoint.angle + target.transform.localEulerAngles.z, Vector3.forward) * (Vector3.right *(altitude+target.altitude))) + target.transform.localPosition , Color.red, 150);

        transform.localPosition = (Quaternion.AngleAxis(targetClosestAttachPoint.angle + target.transform.localEulerAngles.z, Vector3.forward) * (Vector3.right * (altitude + target.altitude))) + target.transform.localPosition;

        //rad = ((Quaternion.AngleAxis(targetClosestAttachPoint.angle + target.transform.localEulerAngles.z, Vector3.forward) * (Vector3.right * (altitude + target.altitude))) + target.transform.localPosition).magnitude;
        //init = Vector3.zero;



        float AngleRad = Mathf.Atan2(target.transform.localPosition.y - transform.localPosition.y, target.transform.localPosition.x - transform.localPosition.x);
        // Get Angle in Degrees
        float AngleDeg = Mathf.Rad2Deg * AngleRad - myClosestAttachPoint.angle; // si es 180 estan alineats
       // Debug.Log(Mathf.Rad2Deg * AngleRad);
        //transform.RotateAround(myClosestAttachPoint.transform.position,Vector3.forward, AngleDeg);
        transform.localEulerAngles = new Vector3(0, 0, AngleDeg);
        

    }
    //Vector3 init;
    //float rad;
    /*
    void OnDrawGizmos()
    {
        UnityEditor.Handles.DrawWireDisc(init, Vector3.back,rad );
    }*/

    public virtual void Detach()
    {
        // Esto no sera nesecario ya que ira por trigger de los attachPoints
        /*foreach (AttachPoint item in _attachPoints)
        {
            item.isBusy = false;
        }*/

        transform.parent = transform.parent.parent;
    }

    public abstract void Kill();

    public float DistanceTo(Polygon target)
    {
        return Vector3.SqrMagnitude(transform.position - target.transform.position);
    }
    #endregion

    #region WIP
    // Canviem el color al que correspont per _id
    protected void ResetColor()
    {
        GetComponent<Renderer>().material.color = TestManager.instance._colors[Id%4];
    }

    #endregion
}