using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PolygonCollider2D))]
public class SuperSquare : MonoBehaviour {

    public LayerMask _layer;
    public bool _isDominant = false;

    private List<Square> _childs;
    private PolygonCollider2D _collider;

    private Vector2 _pivot;

    void Awake()
    {
        _collider = GetComponent<PolygonCollider2D>();
        _childs = new List<Square>();

        //_pivot = transform.position;

        UpdateChilds();
    }

    // Use this for initialization
    void Start ()
    {
        //_collider.Calculate();
        if(_childs.Count != 0)
            CalculateCollider();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O) && _isDominant)
        {
            SearchSuperSquareToAttach();
        }
    }

    private void UpdateChilds() {
        foreach (Transform child in transform)
        {
            Square aux = child.GetComponent<Square>();            
            if(aux != null)
                _childs.Add(aux);            
        }
    }

    void SearchSuperSquareToAttach()
    {
        Collider2D[] found = Physics2D.OverlapCircleAll(transform.position, 1, _layer);
        foreach (var item in found)
        {
            if (!item.CompareTag(tag))
            {
                SuperSquare target = item.GetComponent<SuperSquare>();

                while(!target.IsEmpty()) {
                    // Search the childs most nearby. Until target does not have anyone left.
                    // It just a for.
                    Square[] closestPair = GetClosestPairOfSquares(target);

                    
                    //Debug.Log(closestPair[0].gameObject.name + "  " + closestPair[1].gameObject.name);

                    // Set free the target square
                    target.DeleteSquare(closestPair[0]);

                    // Attach Square
                    closestPair[1].AttachSquare(closestPair[0]);

                    // Set new parent
                    AddSquare(closestPair[0]);

                    Debug.Log("FUSION");
                    // Recalculate Collider
                    CalculateCollider();
                }
            }
        }
    }

    private Square[] GetClosestPairOfSquares(SuperSquare target) // TODO: Millora
    {        
        float min = 0;
        bool first = true;
        Square[] sol = new Square[2];

        foreach (Square a in target._childs)
        {            
            foreach (Square b in _childs)
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
    
    private void CalculateCollider() // REV
    {

        Vector2[] vertices = null;

        if (_childs.Count <=2)
        {
            Debug.Log("Entra");
            vertices = GetVerticesFromChildCollider();//children
        }
        else
        {
            vertices = new Vector2[_collider.GetPath(0).Length + _childs[_childs.Count-1].GetVertices().Length];
            _collider.GetPath(0).CopyTo(vertices,0);
            _childs[_childs.Count - 1].GetVertices().CopyTo(vertices, _collider.GetPath(0).Length);
        }

        Debug.Log(_collider.GetPath(0).Length);
        /*Debug.Log("BEGIN");
        foreach (Vector2 item in vertices)
        {
            Debug.Log(item);
        }*/

        List<Vector2> verticesList = new List<Vector2>();
        verticesList.AddRange(vertices);        
        List<Vector2> toRemove = new List<Vector2>();

        _pivot = Compute2DPolygonCentroid(vertices, vertices.Length); // Ojo

        for (int i = 0; i < verticesList.Count; i++)
        {
            if (_childs.Count == 4 && Vector3.Magnitude(new Vector2(verticesList[i].x, verticesList[i].y) - _pivot)<1)
                toRemove.Add(verticesList[i]);

            for (int j = i+1; j < verticesList.Count; j++)
            {
                if (Vector3.Magnitude(verticesList[i] - verticesList[j]) < 0.2)
                    toRemove.Add(verticesList[j]);        
            }
        }

        for(int i = 0; i < toRemove.Count; i++) 
        {
           verticesList.Remove(toRemove[i]);
        }

        Vector2[] aux2 = verticesList.ToArray();
        

        /*Debug.Log("END:");
        foreach (Vector2 item in aux2)
        {
            Debug.Log(item);
        }*/
        Array.Sort(aux2, Compare);

        
        _collider.offset = transform.position * (-1);
        _collider.SetPath(0, aux2);
        Debug.Log(_collider.GetPath(0).Length);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector3(_pivot.x, _pivot.y), .05f);
    }

    private int Compare(Vector2 v1, Vector2 v2)
    {
        return IsClockwise(v1, v2, _pivot);//transform.position);
    }

    private int IsClockwise(Vector2 first, Vector2 second, Vector2 origin)
    {
        if (first == second)
            return 0;

        Vector2 firstOffset = first - origin;
        Vector2 secondOffset = second - origin;

        float angle1 = Mathf.Atan2(firstOffset.x, firstOffset.y);
        float angle2 = Mathf.Atan2(secondOffset.x, secondOffset.y);

        if (angle1 < angle2)
            return -1;

        if (angle1 > angle2)
            return 1;

        // Check to see which point is closest
        return (firstOffset.sqrMagnitude < secondOffset.sqrMagnitude) ? -1 : 1;
    }

    private Vector2[] GetVerticesFromChildCollider() // For init
    {
        int totalVertices = _childs.Count * 4;
        Vector2[] vertices = new Vector2[totalVertices];

        _childs[0].GetVertices().CopyTo(vertices, 0);

        for (int i = 1; i < _childs.Count; i++)
            _childs[i].GetVertices().CopyTo(vertices, 4 * i);

        return vertices;
    }    

    public void AddSquare(Square target)
    {
        target.gameObject.transform.SetParent(transform);
        _childs.Add(target);
    }

    public void DeleteSquare(Square target)
    {
        target.gameObject.transform.parent = null;
        _childs.Remove(target);
    }

    public bool IsEmpty()
    {
        return _childs.Count == 0;
    }


    private Vector2 Compute2DPolygonCentroid(Vector2[] vertices, int vertexCount)
    {
        Vector2 centroid = new Vector2(0, 0);
        float signedArea = 0.0f;
        float x0 = 0.0f; // Current vertex X
        float y0 = 0.0f; // Current vertex Y
        float x1 = 0.0f; // Next vertex X
        float y1 = 0.0f; // Next vertex Y
        float a = 0.0f;  // Partial signed area

        // For all vertices except last
        int i = 0;
        for (i = 0; i < vertexCount - 1; ++i)
        {
            x0 = vertices[i].x;
            y0 = vertices[i].y;
            x1 = vertices[i + 1].x;
            y1 = vertices[i + 1].y;
            a = x0 * y1 - x1 * y0;
            signedArea += a;
            centroid.x += (x0 + x1) * a;
            centroid.y += (y0 + y1) * a;
        }

        // Do last vertex separately to avoid performing an expensive
        // modulus operation in each iteration.
        x0 = vertices[i].x;
        y0 = vertices[i].y;
        x1 = vertices[0].x;
        y1 = vertices[0].y;
        a = x0 * y1 - x1 * y0;
        signedArea += a;
        centroid.x += (x0 + x1) * a;
        centroid.y += (y0 + y1) * a;

        signedArea *= 0.5f;
        centroid.x /= (6.0f * signedArea);
        centroid.y /= (6.0f * signedArea);

        return centroid;
    }
}
