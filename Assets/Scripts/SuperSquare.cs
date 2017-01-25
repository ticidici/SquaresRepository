using UnityEngine;
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
    private void UpdateChild()
    {
        transform.GetChild(0).GetComponent<SquareController>()._playerNumber = _id+1;
        Square currentChild = transform.GetChild(0).GetComponent<Square>();
        currentChild.Id = _id;
        currentChild.tag = tag;
        _children.Add(currentChild);
        _inputsFromchildren.Add(currentChild, Vector2.zero);        
    }
    
    // Cerca SuperSquares a un radi 3u. Fa detach i attach a target.
    private void SearchSuperSquareToAttach()                            // TODO: REV!.
    {
        Collider2D[] found = Physics2D.OverlapCircleAll(transform.position, 3, _layerMaskToSearch);
       
        foreach (var item in found)
        {
            if (!item.CompareTag(tag)) // TODO: canviar el tag per /nom/etc
            {
                SuperSquare target = item.GetComponent<SuperSquare>();
                target.GetComponent<BoxCollider2D>().enabled = false;

                while (!target.IsEmpty()) {
                    // Search the child most nearby.
                    Square[] closestPair = GetClosestPairOfSquares(target);
                    
                    // Set free the target square
                    target.Remove(closestPair[0]);

                    // Attach
                    closestPair[0].AttachTo(closestPair[1]);

                    // Set new parent
                    Add(closestPair[0]);
                }
            }
        }
    }

    // Retornem el parell de Squares amb menor distancia amb target
    // Index 0: es el square del supersquare que vol fer attach
    // Index 1: es l'altre square a que li faran attach
    private Square[] GetClosestPairOfSquares(SuperSquare target)
    {
        float minDistance = target._children[0].GetDistance(_children[0]);        
        Square[] squarePair = new Square[2] { target._children[0], _children[0] };
        
        int length = _children.Count;
        int targetLength = target._children.Count;

        for(int i = 0; i < targetLength; i++)
        {
            Square targetSquare = target._children[i];
            for (int j = 0; j < length; j++)
            {
                Square square = _children[j];
                float distance = square.GetDistance(targetSquare);

                if (minDistance > distance)
                {
                    minDistance = distance;
                    squarePair[0] = targetSquare;
                    squarePair[1] = square;
                }
            }
        }
        return squarePair;         
    }

    // Detach de tots els fills. Es fa una explosio en la posicio de requestingSquare
    private void DetachAllChildren(Vector3 requestingSquarePosition)
    {
        // No cal fer Detach si nomes tenim 1 fill.
        if (_children.Count < 2)
            return;

        // Per cada fill fem el dettach i el borrem de la llista
        foreach (Square item in _children.ToArray())
        {
            item.Detach();
            _children.Remove(item);
            //_inputsFromchildren.Remove(item);     // Implicit
        }
        Explosion(requestingSquarePosition);
    }
    #endregion

    #region Public Methods

    // Add target to the children List and set him this as a parent.
    public void Add(Square target)
    {
        if (target == null)     // Excepcio/Chivato w/e
            return;
        else if (IsEmpty())     // Entrem quan tornem de un detach, que es quan el SuperSquare esta buit.
        {
            transform.position = target.transform.position;  // Actualitzem la posicio del super
            transform.rotation = target.transform.rotation;  // Actualitzem la rotacio del super           

            // Tornem a activar el collider.
            GetComponent<BoxCollider2D>().enabled = true;
        } 

        target.gameObject.transform.parent = transform;
        _children.Add(target);
        //_inputsFromchildren.Add(target, Vector2.zero);    // Implicit alhora de posarlo com a child
    }

    public void Remove(Square target)
    {       
        _children.Remove(target);
        //_inputsFromchildren.Remove(target);               // Implicit alhora de posarlo com a child            
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
    }
        /*    
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
    }*/

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
