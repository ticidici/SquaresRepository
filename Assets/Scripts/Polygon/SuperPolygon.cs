using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class SuperPolygon : MonoBehaviour
{
    [SerializeField]
    private LayerMask _layerMaskToSearch; // TODO: Investigar mes pq no calgi posarla manualment ja que el this ja te la layer ficada
    private List<Polygon> _shape;

    private Rigidbody2D _rb;

    #region Unity Methods
    void Awake()
    {
        _shape = new List<Polygon>();
        _rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
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
    private Polygon GetClosestTo(Polygon target)
    {
        float[] distances = CalculateDistancesTo(target);
        int index = Array.IndexOf(distances, distances.Min());
        return _shape[index];
    }

    private float[] CalculateDistancesTo(Polygon target)
    {
        float[] distances = new float[_shape.Count];

        for (int i = 0; i < _shape.Count; i++)
            distances[i] = _shape[i].DistanceTo(target);

        return distances;
    }

    // TODO: Fer que agafi el mes proper. Ara agafa el primer de larray found que no sigui this.
    // depen del tag per fer diferencia, aixo millor passar-ho a la propia Id.
    // Cerca SuperSquares a un radi 2u. Fa Merge.
    private void SearchSuperFormToMerge() // Search Closest SuperPolygon // TODO
    {
        Collider2D[] found = Physics2D.OverlapCircleAll(transform.position, 2, _layerMaskToSearch);

        Collider2D[] foundSinThis = found.Where(item => item.gameObject != gameObject).ToArray();

        //Debug.Log("We found" + foundSinThis[0].name + " to merge with.");
        if (foundSinThis.Length == 0)
            return;

        foundSinThis[0].GetComponent<SuperPolygon>().Merge(this);
        /*
        Square[] closestPair = GetClosestPairOfSquares(foundSinThis[0].GetComponent<SuperSquare>());
        Remove(closestPair[1]);
        foundSinThis[0].GetComponent<SuperSquare>().Add(closestPair[1]);*/
    }
    #endregion

    #region Public Methods
    public void Add(Polygon target)
    {
        if (target == null)
            return;
        else if (IsEmpty())
            target.transform.parent = transform;
        else
        {
            Polygon closest = GetClosestTo(target);
            target.AttachTo(closest);
        }
        _shape.Add(target);
    }
    
    public void Merge(SuperPolygon target)
    {
        foreach (Polygon current in target._shape)
        {
            Add(current);
        }
        target._shape.Clear();
        PoolManager.ReleaseObject(target.gameObject);
    }

    public void RemovePolygon(Polygon target)
    {
        _shape.Remove(target);
        target.Detach();

        if (IsEmpty())
            PoolManager.ReleaseObject(this.gameObject);
    }

    public void DestroyPolygon(Polygon target)
    {
        _shape.Remove(target);
        target.Kill();

        if (IsEmpty())
            PoolManager.ReleaseObject(this.gameObject);
    }

    public void DetachAll()
    {
        if (_shape.Count < 2)
            return;

        foreach (Polygon current in _shape)
        {
            current.Detach();
        }
        _shape.Clear();
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
    public void ExplodeSquare(int id, Vector2 dirForceExplosion) // TODO: Kill
    {
        Polygon squareToKill = _shape.SingleOrDefault(item => item.Id == id);
        squareToKill.GetComponent<Renderer>().material.color = Color.red;
        if (squareToKill)
        { 
            Vector2 position = squareToKill.transform.position;
            DestroyPolygon(squareToKill);

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