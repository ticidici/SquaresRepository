using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class SuperPolygon : MonoBehaviour
{
    public int _radiusForMerge = 5; // tmp, poder va a polygon
    public float _forceFactorMerge = 2;
    public float _delayTime = 0.2f;

    private LayerMask _layerMaskToSearch; // TODO: Investigar mes pq no calgi posarla manualment ja que el this ja te la layer ficada
    private List<Polygon> _shape;
    private Rigidbody2D _rb;
    private WaitForSeconds _delayTimeToMerge;
    private LookAt _lookAtScript;

    #region Unity Methods
    void Awake()
    {
        _shape = new List<Polygon>();
        _rb = GetComponent<Rigidbody2D>();
        _lookAtScript = GetComponent<LookAt>();

        _delayTimeToMerge = new WaitForSeconds(_delayTime);
        _layerMaskToSearch = LayerMask.GetMask("PlayerPolygon");
    }
    /*
    if (rotating)
     {
         Vector3 to = new Vector3(20, 20, 20);
         if (Vector3.Distance(transform.eulerAngles, to) > 0.01f)
         {
             transform.eulerAngles = Vector3.Lerp(transform.rotation.eulerAngles, to, Time.deltaTime);
         }
         else
         {
             transform.eulerAngles = to;
             rotating = false;
         }
     }*/

    void FixedUpdate()
    {
        Move();
        if (_rb.rotation > 360)
            _rb.rotation = _rb.rotation % 360;
        /*if (_shape.Count > 1)
        {
            Debug.Log(_shape[0].transform.rotation.eulerAngles + "    " + _shape[1].transform.rotation.eulerAngles);
            Debug.Break();
        }*/
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
    private void SearchSuperPolygonToMerge() // REV: les dues primeres linees son una burrada el que costen.
    {
        // Cerquem els Super proxims en un radi de 2u. TODO: El radi depen del Polygon que fa l'accio
        // Aixo fa que el radi i la posicio venen determinats pel Polygon.
        Collider2D[] found = Physics2D.OverlapCircleAll(transform.position, _radiusForMerge, _layerMaskToSearch);

        // De l'array d'elements trovat, eliminar el this
        Collider2D[] foundWithoutThis = found.Where(item => item.transform.parent.gameObject != gameObject).ToArray();

        if (foundWithoutThis.Length == 0)
            return;
        else if (_shape.Count == 1)
        {
            // Cerca el que es mes proxim
            Collider2D closest = ClosestCollider(foundWithoutThis);

            //Debug.Log(closest.GetComponent<Polygon>().CurrentSuperSquare.name);
            // Fer el Merge sobre el mes proxim
            //closest.GetComponent<SuperPolygon>().Merge(this);
            MergeWithAnimation(closest.GetComponent<Polygon>().CurrentSuperSquare);
        }
    }

    // New System Merge
    private void SearchSuperPolygonToMerge2() // REV: les dues primeres linees son una burrada el que costen.
    {
        // Cerquem els Super proxims en un radi de 2u. TODO: El radi depen del Polygon que fa l'accio
        // Aixo fa que el radi i la posicio venen determinats pel Polygon.
        Collider2D[] found = Physics2D.OverlapCircleAll(transform.position, _radiusForMerge, _layerMaskToSearch);

        // De l'array d'elements trovat, eliminar el this
        Collider2D[] foundWithoutThis = found.Where(item => item.transform.parent.gameObject != gameObject).ToArray();

        if (foundWithoutThis.Length == 0)
            return;
        else if (_shape.Count == 1) // Constrain pq lo del tamany bla bla bla revisar
        {
            // Cerca el que es mes proxim
            Collider2D closestTarget = ClosestCollider(foundWithoutThis);

            // Fer que apuntem a ell
            AttachPoint myClosestAttachPoint = _shape[0].GetAttachPointClosestTo(closestTarget.transform.position); // el primer pq aquesta accio nomes la poden fer els supers amb un fill
            //targetClosestAttachPoint = closestTarget.GetComponent<Polygon>().GetAttachPointClosestTo(myClosestAttachPoint.transform.position);

            //Debug.DrawLine(transform.position, targetClosestAttachPoint.transform.position, Color.yellow);
            //Debug.DrawLine(transform.position, myClosestAttachPoint.transform.position, Color.yellow);

            _lookAtScript.Setup(closestTarget.GetComponent<Polygon>().CurrentSuperSquare.gameObject, myClosestAttachPoint);
            _lookAtScript._isActive = true;

            // Anar cap a ell
            MergeWithAnimation(closestTarget.GetComponent<Polygon>().CurrentSuperSquare);

            // Al fer que el raycast el detecti, agafar vector attachpoint i fer anim
        }
    }

    private IEnumerator PullToUpdate(SuperPolygon target) // Rev
    {
        // Afegim atraccio
        RaycastHit2D hit;
        Vector3 direction;
        bool isPullingTo = true;
        float rayDistance = 0.6f;//0.2886751345948129f + 1f;

        while (isPullingTo)
        {
            if (!target.gameObject.activeSelf)
                isPullingTo = false;
            else
            {
                direction = (target.transform.position - transform.position);
                direction.Normalize();
                _rb.LookAt(target.transform.position);
                _rb.AddForce(direction * _forceFactorMerge);
                yield return null;
                Debug.DrawRay(transform.position, direction * rayDistance, Color.white);
                hit = Physics2D.Raycast(transform.position, direction, rayDistance, _layerMaskToSearch);
                if (hit.collider != null)
                {
                    //Debug.Log(target.gameObject + "  " +  hit.collider.transform.parent.gameObject);
                    if (target.gameObject == hit.collider.transform.parent.gameObject)
                    {
                        _lookAtScript._isActive = false;
                        yield return new WaitForFixedUpdate();
                        yield return _delayTimeToMerge;
                        target.Merge(this);
                        isPullingTo = false;
                    }
                }
            }
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
        else // Aqui es donde debe estar la animacion a la hora de hacer attach!
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

    public void MergeWithAnimation(SuperPolygon target)
    {
        StartCoroutine(PullToUpdate(target));
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
            //Debug.Log("Attach");
            SearchSuperPolygonToMerge2();
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
            if (a != null)
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
        Collider2D[] colliders = Physics2D.OverlapCircleAll(explosionPos, 3, LayerMask.GetMask("Players"));
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
            //if (squareToKill.isLeaf())
            // {
            //   Debug.Log("Extrem");
            Vector3 position = squareToKill.transform.position;
                DestroyPolygon(squareToKill);
             //   return;
           // }

            // Si tenim fills, els separem, fem explosio i merge
            for (int i = 0; i < _shape.Count; i++)
            {
                _shape[i].Detach();
                _shape[i].AssignSuperPolygon();
            }

            Explosion(position);

            
            for (int i = 1; i < _shape.Count; i++)
            {
                _shape[i].CurrentSuperSquare.MergeWithAnimation(_shape[0].CurrentSuperSquare); // En vez de pull es merge
            }

            if (_shape.Count != 0)
            {
                _shape.Clear();
                PoolManager.ReleaseObject(this.gameObject);
            }
        }
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