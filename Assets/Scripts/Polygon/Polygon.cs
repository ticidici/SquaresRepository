using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(SquareController))]
public abstract class Polygon : MonoBehaviour, IAttachable
{
    public AttachPoint _AttachPointPrefab;
    public GameObject _superPolygonPrefab;

    // Parametres generals
    public float _xForce = 7.1f;
    public float _yForce = 3.8f;
    public float _magnetWaitTime = 1.5f;

    public int Id { get; set; }
    public SuperPolygon CurrentSuperSquare { get; set; }

    protected static int ID_COUNT = 0;
    protected float _length = 1f;
    protected AttachPoint[] _attachPoints;

    #region Protected Methods

    protected abstract void SetAttachPoints();

    #endregion

    #region Public Methods

    private AttachPoint GetAttachPointClosestTo(Vector3 point) // Mirar scope per determinar public o private
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
        //if (target == this) Mirar com fer-ho millor
        //return;
        transform.rotation = target.transform.rotation;
        AttachPoint aux = target.GetAttachPointClosestTo(transform.position);
        transform.position = aux.transform.position;
        transform.parent = target.transform.parent;
        aux.isBusy = true;
    }

    public virtual void Detach()
    {
        // Esto no sera nesecario ya que ira por trigger de los attachPoints
        foreach (AttachPoint item in _attachPoints)
        {
            item.isBusy = false;
        }

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