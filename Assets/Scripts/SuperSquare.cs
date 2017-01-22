using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Faltaria saber quina forma te --> attachPoints

public class SuperSquare : MonoBehaviour {
    
    public bool _isDominant = false;
    public int numChilds = 0;                   // Variable de Test

    [SerializeField]
    private LayerMask _layerMaskToSearch;       // Investigar mes pq no calgi posarla manualment ja que el this ja te la layer ficada

    private List<Square> _childs;
    private Square _firstChild;

    void Awake()
    {
        _childs = new List<Square>();
        
        UpdateChilds();        
    }

    void Update()
    {
        numChilds = _childs.Count;
        if (Input.GetKeyDown(KeyCode.O) && _isDominant)
        {
            SearchSuperSquareToAttach();
        }

        if (Input.GetKeyDown(KeyCode.P) && _isDominant)
        {
            DetachAllChildren(); // Test
        }
    }


    // Nomes per fer el test
    private void DetachAllChildren() // Prova de dettach de tots el fills
    {
        if (_childs.Count < 2)          // Si no te almenys 2 fills. Hauria d'alertar
            return;

        foreach (Square item in _childs.ToArray())  // Per cada fill fem el dettach i el borrem de la llista
        {
            item.Detach();
            _childs.Remove(item);
        }

        Explosion();
    }

    // Nomes per fer el test
    private void Explosion()   // Test
    {
        float radius = 5.0f;
        float power = 1000.0f;
        Vector3 explosionPos = transform.position;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(explosionPos, 2, _layerMaskToSearch);
        foreach (Collider2D hit in colliders)
        {
            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
            
            if (rb != null)
                rb.AddExplosionForce(power,explosionPos,radius);
        }
    }

    private void UpdateChilds() {
        foreach (Transform child in transform)
        {
            Square aux = child.GetComponent<Square>();            
            if(aux != null)
                _childs.Add(aux);            
        }

        _firstChild = _childs[0];
    }

    private void SearchSuperSquareToAttach() // Canviar nom
    {
        Collider2D[] found = Physics2D.OverlapCircleAll(transform.position, 3, _layerMaskToSearch);
       
        foreach (var item in found)
        {
            if (!item.CompareTag(tag)) // tag/nom/etc
            {
                SuperSquare target = item.GetComponent<SuperSquare>();

                while(!target.IsEmpty()) {
                    // Search the childs most nearby. Until target does not have anyone left.
                    Square[] closestPair = GetClosestPairOfSquares(target);

                    // Set free the target square
                    target.DeleteSquare(closestPair[0]);

                    // Attach Square
                    closestPair[0].AttachTo(closestPair[1]);

                    // Set new parent
                    AddSquare(closestPair[0]);
                }
            }
        }
    }

    private Square[] GetClosestPairOfSquares(SuperSquare target) // TODO: Millora
    {        
        float min = 0;
        bool first = true;
        Square[] sol = new Square[2];

        foreach (Square a in target._childs.ToArray())
        {            
            foreach (Square b in _childs.ToArray())
            {
                if (first)
                {
                    first = false;
                    min = target._childs[0].GetDistance(_childs[0]);
                    sol[0] = a;
                    sol[1] = b;
                }
                else
                {
                    float aux = a.GetDistance(b);
                    if (min > aux)
                    {
                        min = aux;
                        sol[0] = a;
                        sol[1] = b;
                    }
                }
            }
        }
        return sol;
    }

    public void AddSquare(Square target)
    {
        target.gameObject.transform.parent = transform;
        _childs.Add(target);
    }

    public void DeleteSquare(Square target)
    {
        target.gameObject.transform.parent = null;
        _childs.Remove(target);

        if (_childs.Count == 0)
        {
            _childs.Clear();            
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    public bool IsEmpty()
    {
        return _childs.Count == 0;
    }

    private void Reset() // Es cridat per el Square quan es queda orfan.
    {
        transform.position = _firstChild.transform.position; // Actualitzem la posicio del super
        AddSquare(_firstChild);
        GetComponent<BoxCollider2D>().enabled = true;
    }
}
