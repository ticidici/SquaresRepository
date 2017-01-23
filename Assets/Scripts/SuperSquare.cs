using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Faltaria saber quina forma te --> attachPoints

public class SuperSquare : MonoBehaviour {
    
    public bool _isDominant = false;            // Nomes per el test indica que es el que es controlat pel jugador.
    public int numChilds = 0;                   // Nomes per el test, indica quants fills tenim

    [SerializeField]
    private LayerMask _layerMaskToSearch;       // Investigar mes pq no calgi posarla manualment ja que el this ja te la layer ficada
    private List<Square> _children;             // Llista de fills
    private Square _firstChild;                 // Referencia al primer fill amb el començem.

    #region Unity Methods
    void Awake()
    {
        _children = new List<Square>();
        
        // Set la llista de fills
        UpdateChilds();        
    }

    void Update()
    {
        // Actualitzacio del nombre de fills
        numChilds = _children.Count;

        // Attach others squares
        if ((Input.GetKeyDown(KeyCode.O) || Input.GetKeyDown(KeyCode.Joystick1Button2)) && _isDominant)        
            SearchSuperSquareToAttach();
        
        // Detach all children
        if ((Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Joystick1Button1)) && _isDominant)
            DetachAllChildren(); // Test
    }
    #endregion

    #region Just for test1
    // Nomes per fer el test
    private void DetachAllChildren() // Prova de dettach de tots el fills
    {
        if (_children.Count < 2)          // Si no te almenys 2 fills. Hauria d'alertar
            return;

        foreach (Square item in _children.ToArray())  // Per cada fill fem el dettach i el borrem de la llista
        {
            item.Detach();
            _children.Remove(item);
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
    #endregion

    #region Private Methods

    // Actualitzem la llista de fills i guardem la referencia del primer fill.
    private void UpdateChilds() {
        foreach (Transform child in transform)
        {
            Square aux = child.GetComponent<Square>();            
            if(aux != null)
                _children.Add(aux);            
        }

        _firstChild = _children[0];
    }
    
    private void SearchSuperSquareToAttach() // TODO: Canviar nom
    {
        Collider2D[] found = Physics2D.OverlapCircleAll(transform.position, 3, _layerMaskToSearch);
       
        foreach (var item in found)
        {
            if (!item.CompareTag(tag)) // TODO: canviar el tag per /nom/etc
            {
                SuperSquare target = item.GetComponent<SuperSquare>();

                while(!target.IsEmpty()) {
                    // Search the childs most nearby. Until target does not have anyone left.
                    Square[] closestPair = GetClosestPairOfSquares(target);
                    
                    // Set free the target square
                    target.DeleteSquare(closestPair[0]);

                    // Attach
                    closestPair[0].AttachTo(closestPair[1]);

                    // Set new parent
                    AddSquare(closestPair[0]);
                }
            }
        }
    }

    // Retornem el parell de Squares amb menor distancia amb target
    private Square[] GetClosestPairOfSquares(SuperSquare target) // TODO: Millorar
    {        
        float min = 0;
        bool first = true;
        Square[] sol = new Square[2];

        foreach (Square a in target._children.ToArray())
        {            
            foreach (Square b in _children.ToArray())
            {
                if (first)
                {
                    first = false;
                    min = target._children[0].GetDistance(_children[0]);
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

    // Pre: el child no te cap pare
    // Pos: Posicionem el SuperSquare en la posicio del fill original i l'emparentem.
    private void Reset(Square child) // Es cridat per el Square quan es queda orfan.
    {
        transform.position = child.transform.position; // Actualitzem la posicio del super
        AddSquare(child);
        GetComponent<BoxCollider2D>().enabled = true;
    }
    #endregion

    #region Private Methods
    // Add target to the children List and set him this as a parent.
    public void AddSquare(Square target)
    {
        target.gameObject.transform.parent = transform;
        _children.Add(target);
    }

    public void DeleteSquare(Square target)
    {
        target.gameObject.transform.parent = null;
        _children.Remove(target);

        if (_children.Count == 0)
        {
            _children.Clear();            
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    public bool IsEmpty()
    {
        return _children.Count == 0;
    }
    #endregion
}
