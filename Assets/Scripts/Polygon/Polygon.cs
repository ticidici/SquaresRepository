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
    protected float altitude;
    protected AttachPoint[] _attachPoints;

    protected GameManager _gameManager;

    #region Protected Methods
    protected abstract void SetAttachPoints();

    protected virtual void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager != null)
        {
            _gameManager.AddPlayerToGame(Id);//¡¡Pressuposa que tots els polygons són jugadors, canviar si cal!!
        }
    }

    protected virtual void FixedUpdate()
    {
        //ScoreManager.AddToScore(1, Id);
    }
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
        transform.localPosition = (Quaternion.AngleAxis(targetClosestAttachPoint.angle+target.transform.localEulerAngles.z, Vector3.forward) * (Vector3.right*(altitude+target.altitude))) + target.transform.localPosition;

        float AngleRad = Mathf.Atan2(target.transform.localPosition.y - transform.localPosition.y, target.transform.localPosition.x - transform.localPosition.x);
        float AngleDeg = Mathf.Rad2Deg * AngleRad - myClosestAttachPoint.angle;
        transform.localEulerAngles = new Vector3(0, 0, AngleDeg);
    }

    public virtual void Detach()
    {
        transform.parent = transform.parent.parent;
    }

    public virtual void Kill()
    {
        GameObject ps = Resources.Load<GameObject>("ExplosionPolygon");
        ParticleSystem ps1 = ps.GetComponent<ParticleSystem>();
        var col = ps1.colorOverLifetime;
        col.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(new GradientColorKey[] { new GradientColorKey(TestManager.instance._colors[Id % 4], 0.0f), new GradientColorKey(TestManager.instance._colors[Id % 4], 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, .17f), new GradientAlphaKey(0.0f, 1.0f) });
        col.color = grad;
        Instantiate(ps, transform.position, Quaternion.identity);

        if (_gameManager != null)
        {
            _gameManager.PolygonKilled(Id);
        }
    }

    public float DistanceTo(Polygon target)
    {
        return Vector3.SqrMagnitude(transform.position - target.transform.position);
    }

    public void ReceivePoints(int pointsReceived)
    {
        ScoreManager.AddToScore(pointsReceived, Id);
    }
    #endregion

    #region WIP
    // Canviem el color al que correspont per _id
    protected void ResetColor()
    {
        GetComponent<Renderer>().material.color = TestManager.instance._colors[Id%4];
    }
    #endregion

    public abstract void TestFeedBackMagnetDetach();

    public abstract void TestFeedBackMagnetAttach();

    public abstract void TestFeedBackMagnetRDY();
}