using System;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class SuperPolygon : MonoBehaviour
{
    private static int ID_COUNT = 0;

    [SerializeField]
    private LayerMask _layerMaskToSearch; // TODO: Investigar mes pq no calgi posarla manualment ja que el this ja te la layer ficada
    private Shape<Polygon> _shape;

    private Rigidbody2D _rb;
    //private BoxCollider2D _collider;

    // Getter/Setter per Test
    public int Id { get; private set; }

    #region Unity Methods
    void Awake()
    {
        _shape = new Shape<Polygon>();
        _rb = GetComponent<Rigidbody2D>();
        //_collider = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        Setup();
    }

    void Update()
    {
        //CheckDistances();
    }

    void FixedUpdate()
    {
        Move();
        PullToUpdate();
    }
    #endregion

    #region Private Methods

    // Temporal, a revisar
    private void Setup()
    {
        Id = ID_COUNT;
        ID_COUNT++;
        name = "SuperPolygon " + ID_COUNT;
        tag = "Player" + ID_COUNT;
    }

    // TODO: Fer que agafi el mes proper. Ara agafa el primer de larray found que no sigui this.
    // depen del tag per fer diferencia, aixo millor passar-ho a la propia Id.
    // Cerca SuperSquares a un radi 2u. Fa Merge.
    private void SearchSuperFormToMerge()
    {
        Collider2D[] found = Physics2D.OverlapCircleAll(transform.position, 2, _layerMaskToSearch);

        Collider2D[] foundSinThis = found.Where(item => !item.CompareTag(tag)).ToArray();

        //Debug.Log("We found" + foundSinThis[0].name + " to merge with.");
        if (foundSinThis.Length == 0)
            return;

        foundSinThis[0].GetComponent<SuperPolygon>().Merge(this);
        /*
        Square[] closestPair = GetClosestPairOfSquares(foundSinThis[0].GetComponent<SuperSquare>());
        Remove(closestPair[1]);
        foundSinThis[0].GetComponent<SuperSquare>().Add(closestPair[1]);*/
    }
    
    // Retornem el parell de Squares amb menor distancia amb target
    // Index 0: es el square del supersquare que vol fer attach
    // Index 1: es l'altre square a que li faran attach
    /*private Square[] GetClosestPairOfSquares(SuperSquare target) // Tot n x n
    {
        float minDistance = target._childrens_test[0].DistanceTo(_childrens_test[0]);
        Square[] squarePair = new Square[2] { target._childrens_test[0], _childrens_test[0] };

        int length = _childrens_test.Count;
        int targetLength = target._childrens_test.Count;

        for (int i = 0; i < targetLength; i++)
        {
            Square targetSquare = target._childrens_test[i];
            for (int j = 0; j < length; j++)
            {
                Square square = _childrens_test[j];
                float distance = square.DistanceTo(targetSquare);

                if (minDistance > distance)
                {
                    minDistance = distance;
                    squarePair[0] = targetSquare;
                    squarePair[1] = square;
                }
            }
        }
        return squarePair;
    }*/
    #endregion

    #region Public Methods
    public void Add(Polygon target)
    {
        if (target == null)
            return;
        else if(IsEmpty())
            target.transform.parent = transform;
        _shape.Add(target);
    }
    
    public void Merge(SuperPolygon target)
    {
        //Debug.Log(target.name + " will merge to " + name);
        int total = target._shape.Count;
        for (int i = 0; i < total; i++)
            Add(target._shape[i]);

        target._shape.Clear();
        PoolManager.ReleaseObject(target.gameObject);
    }

    public void Remove(Polygon target)
    {
        //target.Detach();
        _shape.Remove(target);        

        if (IsEmpty())
            PoolManager.ReleaseObject(this.gameObject);
    }

    public void DetachAll()
    {
        if (_shape.Count < 2)
            return;

        while(!_shape.IsEmpty())
        {
            _shape[0].Detach();
            _shape.Remove(_shape[0]);
        }
        PoolManager.ReleaseObject(this.gameObject);
    }

    // To Base
    public bool IsEmpty()
    {
        return _shape.Count == 0;
    }
    #endregion

    #region Input Related
    public void MagnetInput(Polygon requestingSquare)
    {
        //Debug.Log(name + " " + _shape.Count);
        if (_shape.Count < 2)
        {
            Debug.Log("Attach");
            SearchSuperFormToMerge();
        }
        else
        {
            Debug.Log("Detach");
            DetachAll();
        }
    }

    private void Move()//Once per frame
    {
        //Recorrer diccionario, sumar todo, y luego dividir por número de hijos
        int numberOfChildren = _shape.Count;

        if (numberOfChildren < 1) { return; }
        
        foreach (Polygon entry in _shape) { 
            IControllable a = (IControllable)entry;
            if(a != null)
                _rb.AddForceAtPosition(a.GetForceVector() / numberOfChildren, entry.transform.position, ForceMode2D.Impulse);            
        }
    }
    /* ELIMINAR
    public void MovementInput(Square requestingSquare, Vector2 movementVector)
    {
        _inputsFromchildren[requestingSquare] = movementVector;
        //Debug.Log(movementVector.ToString("F4"));//F4 -> expressió amb 4 decimals
    }*/
    #endregion

    // Tot aixo eren proves alhora de fer reattach, queda en suspensio.
    #region utils
    // Nomes per fer el test
    private void Explosion(Vector3 explosionPos/*, Vector2 dirForceExplosion*/)   // Test
    {
        float radius = 50.0f;
        float power = 200.0f;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(explosionPos, 2, _layerMaskToSearch);
        foreach (Collider2D hit in colliders)
        {
            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();

            if (rb != null)
                rb.AddExplosionForce(power, explosionPos, radius);
        }
        /*
        if (!IsEmpty())            
            _rb.AddForceAtPosition(dirForceExplosion * 6f / _children.Count, explosionPos, ForceMode2D.Impulse);*/
    }

    /* Funcio reaprofitable
public void PushFormation(float pushForce)
{
    //Debug.Log("direction = " + pushDirection + "     force = " + pushForce);

    int numberOfChildren = _childrens_test.Count;

    if (numberOfChildren < 1) { return; }

    Vector2 velocity = -_rb.velocity;
    _rb.velocity = Vector2.zero;//parem la formació per assegurar-nos que les forces sempre fan l'efect desitjat

    foreach (KeyValuePair<Square, Vector2> entry in _inputsFromchildren)
    {
        if (entry.Key == null)
            _inputsFromchildren.Remove(entry.Key);
        else
            _rb.AddForceAtPosition(velocity * pushForce / numberOfChildren, entry.Key.transform.position, ForceMode2D.Impulse);
    }
}*/
    #endregion

    #region WIP
    // Explocio en la posicio del fill que mor. Aqui fa que pogui rotar
    public void ExplodeSquare(int id, Vector2 dirForceExplosion)
    {
        Polygon squareToKill = _shape.FindById(id);//_children.SingleOrDefault(item => item.Id == id);
        squareToKill.GetComponent<Renderer>().material.color = Color.red;
        if (squareToKill)
        { 
            Vector2 position = squareToKill.transform.position;
            Remove(squareToKill);
            //squareToKill.Kill();
            Destroy(squareToKill.gameObject);

            DetachAll();

            //PullTo(gameObject);

            // Tots els fills han de quedar encarant el mes proxims // TODODDODOODODO

            Explosion(position);

            // Han de aproximarse entre ells


            //Explosion(position, -dirForceExplosion);
            //StartCoroutine(ReOrder());
        }
    }

    // Test per al reatach despres duna perdua
    private Vector3 direction;
    private bool isPullingTo = false;
    private bool token = false;
    private GameObject toFollow;
    private SuperPolygon[] cluster; // Substituir per ids de cluster o algo aixi idk, no funcionaria 

    private void CheckDistances() // FALLA
    {
        if (!token || cluster == null)
            return;
        Debug.Log("ENTRO!!!!!!!!!!");
        for (int i = 0; i < cluster.Length; i++)
        {
            if (cluster[i] != null && Vector3.SqrMagnitude(cluster[i].transform.position - transform.position) < 3f)
            {
                //Merge(cluster[i]);
                Debug.Log("Attach");
                cluster[i].isPullingTo = false;
                cluster[i] = null;
            }
        }
    }

    public void PullTo(GameObject target)
    {
        SuperPolygon[] aux = GetNearbySuperSquares(1);

        // Donar token de domini, aques token de domini podria ser el punt intermig entre els SS?
        aux[0].token = true;

        // Els altres l'han de seguir
        for (int i = 1; i < aux.Length; i++)
        {
            // Mirem cap al centre de l'explosio
            //aux[i]._rb.LookAt(aux[0].transform.position);
            aux[i].toFollow = aux[0].gameObject;
            aux[i].isPullingTo = true;
        }

        cluster = aux.Where(item => !item.CompareTag(tag)).ToArray();

        for (int i = 0; i < cluster.Length; i++)
        {
            Debug.Log(cluster[i].name);
        }

        Debug.Log(cluster.Length);

        // Despres qui te el token ha de fer el check de distancia en el Update
        // per tal de fer l'accio d'attach
    }

    private void PullToUpdate()
    {
        // Afegim atraccio
        if (isPullingTo)
        {
            direction = (toFollow.transform.position - transform.position);
            direction.Normalize();
            _rb.LookAt(toFollow.transform.position);
            _rb.AddForce(direction * 4f);
        }
    }

    private SuperPolygon[] GetNearbySuperSquares(float radius)
    {
        Collider2D[] found = Physics2D.OverlapCircleAll(transform.position, radius, _layerMaskToSearch);

        SuperPolygon[] squaresToReturn = Array.ConvertAll(found, item => item.GetComponent<SuperPolygon>());

        return squaresToReturn.Where(item => item != null).ToArray();
    }
    #endregion
}