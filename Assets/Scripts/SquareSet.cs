using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PolygonCollider2D))]
public class SquareSet : MonoBehaviour {

    public LayerMask _layer;

    public List<Square> childs;

    public bool test = false;

    private PolygonCollider2D _collider;

    void Awake()
    {
        _collider = GetComponent<PolygonCollider2D>();
        List<Square> childs = new List<Square>();
    }

    // Use this for initialization
    void Start ()
    {
        CalculateCollider();
        
        if(test)
            SearchSuperSquareToAttach();
    }

    private void CalculateCollider()
    {
        Vector2[] vertices = GetVertexCollider();

        Vector2 pivot = Compute2DPolygonCentroid(vertices, vertices.Length);
        _collider.offset = pivot * (-1);

        _collider.SetPath(0, vertices);
    }

    private Vector2[] GetVertexCollider()
    {
        int totalVertices = childs.Count*4;
        Vector2[] vertices = new Vector2[totalVertices];

        //Square[] auxChilds = childs.ToArray();
        childs[0].GetVertex().CopyTo(vertices, 0);

        for (int i = 1; i < childs.Count; i++)
        {
            childs[i].GetVertex().CopyTo(vertices, childs[i-1].GetVertex().Length);
        }

        return vertices;
    }

    private Vector2 Compute2DPolygonCentroid(Vector2[] vertices, int vertexCount)
    {
        Vector2 centroid = new Vector2(0,0);
        float signedArea = 0.0f;
        float x0 = 0.0f; // Current vertex X
        float y0 = 0.0f; // Current vertex Y
        float x1 = 0.0f; // Next vertex X
        float y1 = 0.0f; // Next vertex Y
        float a = 0.0f;  // Partial signed area

        // For all vertices except last
        int i = 0;
        for (i=0; i<vertexCount-1; ++i)
        {
            x0 = vertices[i].x;
            y0 = vertices[i].y;
            x1 = vertices[i + 1].x;
            y1 = vertices[i + 1].y;
            a = x0* y1 - x1* y0;
            signedArea += a;
            centroid.x += (x0 + x1)*a;
            centroid.y += (y0 + y1)*a;
        }

        // Do last vertex separately to avoid performing an expensive
        // modulus operation in each iteration.
        x0 = vertices[i].x;
        y0 = vertices[i].y;
        x1 = vertices[0].x;
        y1 = vertices[0].y;
        a = x0* y1 - x1* y0;
        signedArea += a;
        centroid.x += (x0 + x1)* a;
        centroid.y += (y0 + y1)* a;

        signedArea *= 0.5f;
        centroid.x /= (6.0f* signedArea);
        centroid.y /= (6.0f* signedArea);

        return centroid;
    }

    // Update is called once per frame
    void Update()
    {
       /* if (Input.GetKeyDown(KeyCode.O))
        {
            SearchSuperSquare();
        }*/
    }

    void SearchSuperSquareToAttach()
    {
        Collider2D[] found = Physics2D.OverlapCircleAll(transform.position, 10, _layer);
        //Debug.Log(found.Length);
        foreach (var item in found)
        {
            if (!item.CompareTag(tag))
            {
                SquareSet target = item.GetComponent<SquareSet>() as SquareSet;

                // Search the chils most nearby
                Square[] closestPair = GetClosestPairOfSquares(target);

                //Debug.Log(closestPair[0].gameObject.name + "  " + closestPair[1].gameObject.name);
         
                // Set free the target square
                target.DeleteSquare(closestPair[0]);

                // Attach Square
                closestPair[1].AttachSquares(closestPair[0]);

                // Set new parent
                AddSquare(closestPair[0]);

                // Recalculate Collider
                CalculateCollider();

                //Debug.Log(item.transform.name);     
            }
        }
    }

    Square[] GetClosestPairOfSquares(SquareSet target)
    {        
        float min = 0;
        bool first = true;
        Square[] sol = new Square[2];

        foreach (Square a in target.childs)
        {
            foreach (Square b in childs)
            {
                if (first)
                {
                    first = false;
                    min = target.childs[0].GetDistance(childs[0]);
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
        target.gameObject.transform.SetParent(transform);
        childs.Add(target);
    }

    public void DeleteSquare(Square target)
    {
        target.gameObject.transform.parent = null;
        childs.Remove(target);
    }
}
