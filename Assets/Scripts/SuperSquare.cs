using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Faltaria saber quina forma te --> attachPoints

public class SuperSquare : MonoBehaviour {
        
    public int numChilds = 0;                   // Nomes per el test, indica quants fills tenim
    public int _idPlayer;                       // Test Input

    private Dictionary<Square, Vector2> _inputsFromchildren;

    [SerializeField]
    private LayerMask _layerMaskToSearch;       // Investigar mes pq no calgi posarla manualment ja que el this ja te la layer ficada
    private List<Square> _children;             // Llista de fills
    //private ControllerPlayer _controller;

    // Getter/Setter per Test
    public int IdPlayer                         // Test Input
    {
        get
        {
            return _idPlayer;
        }
        set
        {
            _idPlayer = value;
        }
    }

    #region Unity Methods
    void Awake()
    {
        _children = new List<Square>();
        //_controller = GetComponent<ControllerPlayer>();
        _inputsFromchildren = new Dictionary<Square, Vector2>();

        // Set la llista de fills
        UpdateChilds();
    }

    void Start()
    {
        //_controller.id = _idPlayer;
    }

    void Update()
    {
        // Actualitzacio del nombre de fills
        numChilds = _children.Count;

        
        // Attach others squares
        if (tag.Equals("Player1") && (Input.GetKeyDown(KeyCode.O) || Input.GetKeyDown(KeyCode.Joystick1Button2)))        
            SearchSuperSquareToAttach();
        
        // Detach all children
        if (tag.Equals("Player1") &&  (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Joystick1Button1)))
            DetachAllChildren(); // Test
            
        Move();
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
            Debug.Log(hit.name);
            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
            
            if (rb != null)
                rb.AddExplosionForce(power,explosionPos,radius);
        }
    }

    // Set color
    public void SetColor(Color color)
    {
        _children[0].Color = color;
    }
    #endregion

    #region Private Methods

    private void Move()//Once per frame
    {
        //Recorrer diccionario, sumar todo, y luego dividir por número de hijos

        int numberOfChildren = _children.Count;

        if (numberOfChildren < 1) { return; }

      //  Vector2 movement = Vector2.right * _MovementInputValueX * _speed * Time.deltaTime;
        //movement += Vector2.up * _MovementInputValueY * _speed * Time.deltaTime;

        

        Vector2 movement = Vector2.zero;
        foreach (KeyValuePair<Square, Vector2> childInputPair in _inputsFromchildren)
        {
            movement += childInputPair.Value;
        }

        GetComponent<Rigidbody2D>().AddForceAtPosition(movement, transform.position + new Vector3(0, 0.1f, 0), ForceMode2D.Impulse);

        //transform.Translate(AddedVelocity / numberOfChildren);//Revisar
    }

    // Actualitzem la llista de fills i guardem la referencia del primer fill.
    private void UpdateChilds() {
        foreach (Transform child in transform)
        {
            Square aux = child.GetComponent<Square>();
            if (aux != null) {
                _children.Add(aux);
                _inputsFromchildren.Add(aux, Vector2.zero);
            }          
        }
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

    #region Public Methods
    // Add target to the children List and set him this as a parent.
    public void AddSquare(Square target)
    {
        target.gameObject.transform.parent = transform;
        _children.Add(target);
        _inputsFromchildren.Add(target, Vector2.zero);
    }

    public void DeleteSquare(Square target)
    {
        target.gameObject.transform.parent = null;        
        _children.Remove(target);
        _inputsFromchildren.Remove(target);
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
            SearchSuperSquareToAttach();
        }
        else
        {
            //Detach
        }
    }

    public void MovementInput(Square requestingSquare, Vector2 movementVector)
    {
        _inputsFromchildren[requestingSquare] = movementVector;
        //Debug.Log(movementVector.ToString("F4"));//F4 -> expressió amb 4 decimals
    }
    #endregion
}
