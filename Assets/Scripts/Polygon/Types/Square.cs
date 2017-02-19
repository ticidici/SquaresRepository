using UnityEngine;
using System.Collections;

//Attachable passarla a interface , Done
//Polygon Passar-la a abstracte // No crearem un polygon no especific!

public class Square : Polygon, IControllable
{

    private SquareController _controller;
    public bool IsControllable { get; set; }
    private bool _isMagnetEnabled = true;

    void Awake()
    {
        IsControllable = true;
        _attachPoints = new AttachPoint[4];
        _controller = GetComponent<SquareController>();
    }

	// Use this for initialization
	void Start () {
        Id = ID_COUNT;
        name = "Square " + Id;
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
    void Update () {
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
        _attachPoints[0] = Instantiate(_AttachPointPrefab, new Vector2(transform.position.x - _length, transform.position.y), Quaternion.identity) as AttachPoint;  // Left
        _attachPoints[1] = Instantiate(_AttachPointPrefab, new Vector2(transform.position.x + _length, transform.position.y), Quaternion.identity) as AttachPoint;  // Right
        _attachPoints[2] = Instantiate(_AttachPointPrefab, new Vector2(transform.position.x, transform.position.y + _length), Quaternion.identity) as AttachPoint;  // Top
        _attachPoints[3] = Instantiate(_AttachPointPrefab, new Vector2(transform.position.x, transform.position.y - _length), Quaternion.identity) as AttachPoint;  // Bottom

        foreach (var item in _attachPoints)
        {
            item.gameObject.transform.SetParent(gameObject.transform);
        }
    }

    #region Input Related
    public Vector2 forceVector;
    public void Move(float x, float y)
    {
        if (!IsControllable)
            return;
        forceVector = new Vector2(x, y);
        forceVector.Normalize();
        forceVector.x *= Time.deltaTime * _xForce;
        forceVector.y *= Time.deltaTime * _yForce;
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

    private IEnumerator WaitActiveTime()
    {
        yield return new WaitForSeconds(_magnetWaitTime);
        _isMagnetEnabled = true;
        //Debug.Log("Magnet enabled");
    }
    #endregion
}
