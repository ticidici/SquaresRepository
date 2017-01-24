using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SuperSquare : MonoBehaviour {
       
    private int _id;                            // Test Input

    [SerializeField]
    private LayerMask _layerMaskToSearch;       // Investigar mes pq no calgi posarla manualment ja que el this ja te la layer ficada
    private List<Square> _children;             // Llista de fills 

    // INPUT
    private Dictionary<Square, Vector2> _inputsFromchildren;

    // Getter/Setter per Test
    public int Id                               // Test Input
    {
        get
        {
            return _id;
        }
        set
        {
            _id = value;
        }
    }

    #region Unity Methods
    void Awake()
    {
        _children = new List<Square>();
        _inputsFromchildren = new Dictionary<Square, Vector2>();
    }

    void Start()
    {
        UpdateChild();
    }

    void Update()
    {
        // Attach others squares
        /*if (tag.Equals("Player1") && (Input.GetKeyDown(KeyCode.O) || Input.GetKeyDown(KeyCode.Joystick1Button2)))        
            SearchSuperSquareToAttach();
        
        // Detach all children
        if (tag.Equals("Player1") &&  (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Joystick1Button1)))
            DetachAllChildren(); // Test
            */
    }

    void FixedUpdate()
    {
        Move();
    }
    #endregion

    #region Private Methods

    // Actualitzem el primer fill que tenim.
    // Es garantitza que sempre hi haura nomes un fill d'inici.
    private void UpdateChild() {

        transform.GetChild(0).GetComponent<SquareController>()._playerNumber = _id+1;
        Square currentChild = transform.GetChild(0).GetComponent<Square>();
        currentChild.Id = _id;
        currentChild.tag = tag;
        _children.Add(currentChild);
        _inputsFromchildren.Add(currentChild, Vector2.zero);        
    }
    
    // Cerca SuperSquares a un radi 3. Fa detach i attach a target.
    // El attach es presuposa sempre viable.
    private void SearchSuperSquareToAttach() // TODO: Canviar nom
    {
        Collider2D[] found = Physics2D.OverlapCircleAll(transform.position, 3, _layerMaskToSearch);
       
        foreach (var item in found)
        {
            if (!item.CompareTag(tag)) // TODO: canviar el tag per /nom/etc
            {
                SuperSquare target = item.GetComponent<SuperSquare>();

                while (!target.IsEmpty()) {
                    // Search the child most nearby.
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
    // Index 0: es qui els square del supersquare que vol fer attach
    // Index 1: es l'altre square a que li faran attach
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

    // Posiblement canviar el nom, no es un reset reset
    // Pre: el child no te cap pare
    // Pos: Posicionem el SuperSquare en la posicio del fill original i l'emparentem.
    private void Reset(Square child) // Es cridat per el Square quan es queda orfan.
    {
        transform.position = child.transform.position;  // Actualitzem la posicio del super
        transform.rotation = child.transform.rotation;  // Actualitzem la rotacio del super
        AddSquare(child);
        GetComponent<BoxCollider2D>().enabled = true;
    }

    // Detach de tots els fills fent una explosio en la posicio de requestingSquare
    private void DetachAllChildren(Vector3 requestingSquarePosition)                // TODO: Millora, detach de tots menys el nostre!
    {
        // No cal fer Detach si nomes tenim 1 fill.
        if (_children.Count < 2)
            return;

        foreach (Square item in _children.ToArray())  // Per cada fill fem el dettach i el borrem de la llista
        {
            item.Detach();
            _children.Remove(item);
            //_inputsFromchildren.Remove(item);
        }
        Explosion(requestingSquarePosition);
    }
    #endregion

    #region Public Methods

    // Add target to the children List and set him this as a parent.
    public void AddSquare(Square target)
    {
        target.gameObject.transform.parent = transform;
        _children.Add(target);
        //_inputsFromchildren.Add(target, Vector2.zero);    // Implicit alhora de posarlo com a child
    }

    public void DeleteSquare(Square target)
    {       
        _children.Remove(target);
        //_inputsFromchildren.Remove(target);               // Implicit alhora de posarlo com a child

        if (_children.Count == 0)
            GetComponent<BoxCollider2D>().enabled = false;        
    }

    public bool IsEmpty()
    {
        return _children.Count == 0;
    }

    public void MagnetInput(Square requestingSquare)
    {
        //Debug.Log("magnet input");

        if (_children.Count < 2)
        {
            Debug.Log("Attach");
            SearchSuperSquareToAttach();
        }
        else
        {
            Debug.Log("Detach");
            DetachAllChildren(requestingSquare.transform.position);
        }
    }
    #endregion

    #region Input Related
    /*
    // Versio Nico. La molongui
    private void Move()//Once per frame
    {
        //Recorrer diccionario, sumar todo, y luego dividir por número de hijos

        int numberOfChildren = _children.Count;

        if (numberOfChildren < 1) { return; }

        Vector2 movement = Vector2.zero;
        foreach (KeyValuePair<Square, Vector2> childInputPair in _inputsFromchildren)
        {
            movement += childInputPair.Value;
        }

        GetComponent<Rigidbody2D>().AddForceAtPosition(movement / numberOfChildren, transform.position + new Vector3(0, 0.1f, 0), ForceMode2D.Impulse);
    }*/
            
    // Versio Tomas amb velocitat i translate. Es curios xd        
    private void Move()//Once per frame 
    {
        //Recorrer diccionario, sumar todo, y luego dividir por número de hijos

        int numberOfChildren = _children.Count;

        if (numberOfChildren < 1) { return; }

        Vector2 AddedVelocity = Vector2.zero;
        foreach (KeyValuePair<Square, Vector2> childInputPair in _inputsFromchildren)
        {
            AddedVelocity += childInputPair.Value;
        }

        transform.Translate(AddedVelocity / numberOfChildren);//Revisar
    }

    public void MovementInput(Square requestingSquare, Vector2 movementVector)
    {
        _inputsFromchildren[requestingSquare] = movementVector;
        //Debug.Log(movementVector.ToString("F4"));//F4 -> expressió amb 4 decimals
    }
    #endregion

    #region utils
    // Nomes per fer el test
    private void Explosion(Vector3 explosionPos)   // Test
    {
        float radius = 5.0f;
        float power = 1000.0f;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(explosionPos, 2, _layerMaskToSearch);
        foreach (Collider2D hit in colliders)
        {
            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();

            if (rb != null)
                rb.AddExplosionForce(power, explosionPos, radius);
        }
    }
    #endregion
}
