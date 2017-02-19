using System;
using UnityEngine;
using System.Linq;
using System.Collections;
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
    }

    void FixedUpdate()
    {
        Move();
    }
    #endregion

    #region Private Methods
    private Polygon GetClosestTo(Polygon target) 
    {
        float[] distances = CalculateDistancesTo(target);
        int index = Array.IndexOf(distances, distances.Min());
        return _shape[index];
    }

    private Collider2D ClosestCollider(Collider2D[] array) 
    {
        int index = 0;
        float minDistance = Vector3.Distance(transform.position, array[index].transform.position);
        Collider2D closest = array[index];

        for (int i = 1; i < array.Length; i++)
        {
            float distance = Vector3.Distance(transform.position, array[i].transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = array[i];
            }
        }
        return closest;
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
    private void SearchSuperPolygonToMerge()
    {
        // Cerquem els Super proxims en un radi de 2u. TODO: El radi depen del Polygon que fa l'accio
        // Aixo fa que el radi i la posicio venen determinats pel Polygon.
        Collider2D[] found = Physics2D.OverlapCircleAll(transform.position, 2, _layerMaskToSearch);

        // De l'array d'elements trovat, eliminar el this
        Collider2D[] foundWithoutThis = found.Where(item => item.gameObject != gameObject).ToArray();

        if (foundWithoutThis.Length == 0)
            return;
        else if (_shape.Count == 1)
        {
            // Cerca el que es mes proxim
            Collider2D closest = ClosestCollider(foundWithoutThis);

            // Fer el Merge sobre el mes proxim
            closest.GetComponent<SuperPolygon>().Merge(this);
        }
    }
    #endregion

    #region Public Methods
    public void Add(Polygon target)
    {
        if (target == null)
            return;
        else if (IsEmpty())
        {
            transform.rotation = target.transform.rotation;
            target.CurrentSuperSquare = this;
            target.transform.parent = transform;
            target.transform.localRotation = Quaternion.identity;
        }
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
        target.AssignSuperPolygon();

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
            current.AssignSuperPolygon();
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
            SearchSuperPolygonToMerge();
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
        float power = 250.0f;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(explosionPos, 3, _layerMaskToSearch);
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
    // El reattach, encara li queda per afinar i retocar pero ja esta aqui.
    public void ExplodeSquare(int id, Vector2 dirForceExplosion) // TODO: Kill
    {
        Polygon squareToKill = _shape.SingleOrDefault(item => item.Id == id);
        squareToKill.GetComponent<Renderer>().material.color = Color.red;
        if (squareToKill)
        {
            Vector2 position = squareToKill.transform.position;
            DestroyPolygon(squareToKill);

            Debug.Log(_shape.Count);
            for (int i = 0; i < _shape.Count; i++)
            {
                _shape[i].Detach();
                _shape[i].AssignSuperPolygon();
                if(i != 0 )                
                    _shape[i].CurrentSuperSquare.PullTo(_shape[0]);
            }

            Explosion(transform.position);

            _shape.Clear();
            PoolManager.ReleaseObject(this.gameObject);
        }
    }

    // Test per al reatach despres duna perdua
    public void PullTo(Polygon target)
    {
        // Mirem cap al centre de l'explosio
        //_rb.LookAt(target.transform.position);
        StartCoroutine(PullToUpdate(target));
    }

    private IEnumerator PullToUpdate(Polygon toFollow)
    {
        // Afegim atraccio
        Vector3 direction;
        bool isPullingTo = true;

        while (isPullingTo)
        {
            direction = (toFollow.transform.position - transform.position);
            direction.Normalize();
            _rb.LookAt(toFollow.transform.position);
            _rb.AddForce(direction * 2); // TODO: Fer atribut el 2
            yield return null;
            if (Vector3.SqrMagnitude(transform.position - toFollow.transform.position) < 1.1f)
                isPullingTo = false;
        }
        toFollow.CurrentSuperSquare.Merge(this);
    }


    // Funcio que no te sentit d'existir
    private SuperPolygon[] GetNearbySuperSquares(float radius)
    {
        Collider2D[] found = Physics2D.OverlapCircleAll(transform.position, radius, _layerMaskToSearch);

        SuperPolygon[] squaresToReturn = Array.ConvertAll(found, item => item.GetComponent<SuperPolygon>());

        return squaresToReturn.Where(item => item != null).ToArray();
    }
    #endregion
}