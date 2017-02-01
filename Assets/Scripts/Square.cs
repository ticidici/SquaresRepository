using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SquareController))]
public class Square : MonoBehaviour
{
    public AttachPoint _AttachPointPrefab;
    public float _length = 1f;    
    public float _xSpeed = 7.1f;
    public float _ySpeed = 3.8f;
    public float _magnetWaitTime = 1.5f;

    private AttachPoint[] _attachPoints;
    private GameObject _firstParent;
    private SuperSquare _currentSuperSquare;
    private bool _isMagnetEnabled = true;
    private int _id;

    public int Id
    {
        set
        {
            _id = value;
        }
    }

    #region Unity Methods
    void Awake()
    {        
        _attachPoints = new AttachPoint[4];
        _firstParent = transform.parent.gameObject;
        _currentSuperSquare = _firstParent.GetComponent<SuperSquare>();  
    }

    void Start()
    {
        SetAttachPoints();
        ResetColor();
        GetComponent<ParticleSystem>().startColor = GetComponent<Renderer>().material.color;
    }

    // Al fer colisio amb un altre collider, canviem el color a blanc
    void OnCollisionEnter2D()
    {
        GetComponent<Renderer>().material.color = Color.white;
    }

    // Al sortir d'una colisio, canviem al color original.
    void OnCollisionExit2D()
    {
        ResetColor();
    }
    #endregion

    #region Private Methods

    // Fem el calcul dels attach points i els fiquem com a fills per despres guardar-los a _attachPoints
    private void SetAttachPoints()
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

    // Retorna el AttachPoint mes proper al point donat
    private AttachPoint GetAttachPointClosestTo(Vector3 point)
    {
        AttachPoint closest = _attachPoints[0];
        float minimumDistance = Vector3.Distance(point,closest.transform.position);

        for (int i = 1; i < _attachPoints.Length; i++)
        {
            float currentDistance = Vector3.Distance(point, _attachPoints[i].transform.position);
            if (minimumDistance > currentDistance)
            {
                minimumDistance = currentDistance;
                closest = _attachPoints[i];
            }
        }
        return closest;
    }

    // Canviem el color al que correspont per _id
    private void ResetColor()
    {
        GetComponent<Renderer>().material.color = TestManager.instance._colors[_id];
    }

    // Cooldown del magnet
    private void StartTimer()
    {
        _isMagnetEnabled = false;
        Debug.Log("Magnet disabled");
        StartCoroutine(WaitActiveTime());
    }

    private IEnumerator WaitActiveTime()
    {
        yield return new WaitForSeconds(_magnetWaitTime);
        _isMagnetEnabled = true;
        Debug.Log("Magnet enabled");
    }

    #endregion

    #region Public Methods

    public void Detach() // Poder hi ha algun bug per no desactivar els colliders? De moment no hi ha res sospitos
    {
        foreach (AttachPoint item in _attachPoints)
        {
            item.isBusy = false;
        }
        transform.parent = gameObject.transform.root;
        _firstParent.SendMessage("Add", this);
        ResetColor();
    }

    public void AttachTo(Square target) // TODO: Passar-ho a net/Millorar
    {
        //target.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        //GetComponent<BoxCollider2D>().enabled = false;

        ResetColor();
        target.ResetColor();
        
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
        transform.position = b.transform.position;

        //target.gameObject.GetComponent<BoxCollider2D>().enabled = true;
        //GetComponent<BoxCollider2D>().enabled = true;
    }

    public float GetDistance(Square target)
    {
        return Vector3.SqrMagnitude(target.transform.position - transform.position);
    }

    public void Move(float x, float y)
    {
        Vector2 velocityVector = new Vector2(x, y);
        velocityVector.Normalize();
        velocityVector.x *= Time.deltaTime * _xSpeed;
        velocityVector.y *= Time.deltaTime * _ySpeed;
        //Debug.Log(""+velocityVector.x + "  " + velocityVector.y);
        //GetComponentInParent<SuperSquare>().MovementInput(this, velocityVector);//Canviar la manera de pillar referencia
        _currentSuperSquare.MovementInput(this, velocityVector);
    }

    public void UseMagnet()
    {
        if (_isMagnetEnabled)
        {
            //GetComponentInParent<SuperSquare>().MagnetInput(this);//Canviar la manera de pillar referencia
            _currentSuperSquare.MagnetInput(this);
            StartTimer();
        }
    }

    //<<<<<<<<<< - Mil proves aquí
    public void KillThisSquare()
    {
        _currentSuperSquare.DetachAllChildren(transform.position);
        //_currentSuperSquare.Remove(this);
        //transform.parent = gameObject.transform.root;
        Debug.Log("I'm being killed");

        //Mandarle al supersquare el mensaje de juntar a todos menos él
        //_currentSuperSquare.AttachOthers(this);
        Destroy(gameObject);
        //StartCoroutine(Explode());
    }

    public IEnumerator Explode()
    {
        yield return new WaitForSeconds(0.03f);//TODO: Fer una animació d'explosió de veritat
        Destroy(gameObject);
    }

    public void SetSuperSquare(SuperSquare parent)
    {
        _currentSuperSquare = parent;
    }
    #endregion
}
