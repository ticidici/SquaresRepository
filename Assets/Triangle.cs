using UnityEngine;
using System.Collections;
using System;

public class Triangle : Polygon, IControllable
{
    private SquareController _controller;
    public bool IsControllable { get; set; }
    private bool _isMagnetEnabled = true;

    void Awake()
    {
        IsControllable = true;
        _attachPoints = new AttachPoint[3];
        _controller = GetComponent<SquareController>();
        gameObject.layer = LayerMask.NameToLayer("PlayerPolygon");
    }

    // Use this for initialization
    void Start()
    {
        altitude = 0.2886751345948129f;
        Id = ID_COUNT;
        name = "Triangle " + Id;
        ID_COUNT++;
        if (IsControllable)
            _controller.SetPlayerController(ID_COUNT);
        else _controller.enabled = false;

        CurrentSuperSquare = PoolManager.SpawnObject(_superPolygonPrefab, transform.position, Quaternion.identity).GetComponent<SuperPolygon>();
        CurrentSuperSquare.Add(this);

        SetAttachPoints();
        ResetColor();

        ParticleSystem ps = GetComponent<ParticleSystem>();
        var col = ps.colorOverLifetime;
        col.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(new GradientColorKey[] { new GradientColorKey(GetComponent<Renderer>().material.color, 0.0f), new GradientColorKey(GetComponent<Renderer>().material.color, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, .17f), new GradientAlphaKey(0.0f, 1.0f) });
        col.color = grad;

        
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public override void AttachTo(Polygon target) // TODO: Passar-ho a net/Millorar
    {
        // Comprovacio si es possible fer laccio, raycast
        base.AttachTo(target);
        CurrentSuperSquare = transform.parent.GetComponent<SuperPolygon>();

        /*Square aux = (Square)target;
        CurrentSuperSquare = aux.CurrentSuperSquare;
        CurrentSuperSquare.Add(this);

        aux.ResetColor();
        ResetColor();*/

        // Faltaria actualitzar els attachpoints, es util?

        /*
        transform.rotation = target.transform.parent.rotation;

        Vector3 distanceBetweenSquares = target.gameObject.transform.position - transform.position;
        Vector3 dir = distanceBetweenSquares.normalized;

        Vector3 initPoint = transform.position + dir * _length;                     // AttachPoint tenin en compte la rotacio
        Vector3 finalPoint = target.gameObject.transform.position - dir * _length;  // AttachPoint tenin en compte la rotacio

        // Posar els AttachPoints ocupats
        AttachPoint a = GetAttachPointClosestTo(initPoint);
        a.isBusy = true;
        AttachPoint b = target.GetAttachPointClosestTo(finalPoint);
        b.isBusy = true;
        transform.position = b.transform.position;*/
    }

    public override void Detach()
    {
        base.Detach();

        //CurrentSuperSquare = PoolManager.SpawnObject(_superPolygonPrefab, transform.position, Quaternion.identity).GetComponent<SuperPolygon>();
        //CurrentSuperSquare.Add(this);
        //AssignSuperPolygon();
        //ResetColor();
    }

    public override void Kill() // TODO
    {
        base.Detach();
        Destroy(gameObject);
    }

    protected override void SetAttachPoints()
    {
        
        _attachPoints[0] = Instantiate(_AttachPointPrefab, Vector2.zero, Quaternion.identity) as AttachPoint;  // Bottom
        _attachPoints[0].angle = 270;

        _attachPoints[1] = Instantiate(_AttachPointPrefab, Vector2.zero, Quaternion.identity) as AttachPoint;  // Left
        _attachPoints[1].angle = 150;

        _attachPoints[2] = Instantiate(_AttachPointPrefab, Vector2.zero, Quaternion.identity) as AttachPoint;  // Right
        _attachPoints[2].angle = 30;

        foreach (var item in _attachPoints)
        {
            item.gameObject.transform.SetParent(gameObject.transform);
        }

        Vector2 dir = Vector2.right * altitude;

        _attachPoints[0].transform.localPosition = Quaternion.Euler(0, 0, _attachPoints[0].angle) * dir;
        _attachPoints[1].transform.localPosition = Quaternion.Euler(0, 0, _attachPoints[1].angle) * dir;
        _attachPoints[2].transform.localPosition = Quaternion.Euler(0, 0, _attachPoints[2].angle) * dir;


    }

    #region Input Related
    public Vector2 forceVector;
    public void Move(float x, float y)
    {
        if (!IsControllable)
            return;

        forceVector = new Vector2(x, y);
        forceVector.Normalize();
        forceVector.x *= Time.fixedDeltaTime * _xForce;
        forceVector.y *= Time.fixedDeltaTime * _yForce;
        //CurrentSuperSquare.MovementInput(this, forceVector); Ara es el Super que demana el vector de força
    }

    public Vector2 GetForceVector() { return forceVector; }

    public void UseMagnet()
    {
        if (!IsControllable)
            return;

        if (_isMagnetEnabled)
        {
            CurrentSuperSquare.MagnetInput(this);
            StartTimer();
        }
    }

    public bool IsActive() { return true; }

    // Cooldown del magnet
    private void StartTimer()
    {
        _isMagnetEnabled = false;
        //Debug.Log("Magnet disabled");
        StartCoroutine(WaitActiveTime());
    }

    WaitForSeconds wait = new WaitForSeconds(1.5f); // base._magnetWaitTime
    private IEnumerator WaitActiveTime()
    {
        yield return wait;
        _isMagnetEnabled = true;
        //Debug.Log("Magnet enabled");
    }
    #endregion
}
