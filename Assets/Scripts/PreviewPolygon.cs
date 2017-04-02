using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class PreviewPolygon : MonoBehaviour {

    private enum Shape
    {
        Square,
        Triangle,
        NumberOfShapes
    };

    public float _xForce = 17f;
    public float _yForce = 16f;

    private InputDevice _thisController;
    private Color _thisColor;
    private Shape _thisShape = Shape.Square;
    private GameObject _squareChild;
    private GameObject _triangleChild;
    private SpriteRenderer _squareRenderer;
    private SpriteRenderer _triangleRenderer;
    private Rigidbody2D _rigidbody;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        foreach (Transform child in transform)
        {
            if (child.name == "Square")
            {
                _squareChild = child.gameObject;
            }
            else if (child.name == "Triangle")
            {
                _triangleChild = child.gameObject;
            }
        }
        _squareRenderer = _squareChild.GetComponent<SpriteRenderer>();
        _triangleRenderer = _triangleChild.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (_thisController == null) { return; }
        MovementInputBindings();
    }

    public void AssignController(InputDevice device)
    {
        _thisController = device;
    }

    public void ChangeShape()
    {
        _triangleChild.SetActive(false);
        _squareChild.SetActive(false);
        _thisShape++;
        if (_thisShape >= Shape.NumberOfShapes)
        {
            _thisShape = 0;
        }
        if (_thisShape == Shape.Square)
        {
            _squareChild.SetActive(true);
        }
        else if (_thisShape == Shape.Triangle)
        {
            _triangleChild.SetActive(true);
        }
        else
        {
            Debug.LogError("Out of range shape", this);
        }
    }

    public Color AssignColor(Color color)
    {
        Color aux = _thisColor;
        _thisColor = color;
        _triangleRenderer.color = _thisColor;
        _squareRenderer.color = _thisColor;
        return aux;
    }

    public Color ReturnColor()
    {
        return _thisColor;
    }

    void MovementInputBindings()
    {
        float x = 0;
        float y = 0;
        x = _thisController.LeftStickX;
        y = _thisController.LeftStickY;

        Vector2 forceVector = new Vector2(x, y);
        forceVector.Normalize();
        forceVector.x *= Time.fixedDeltaTime * _xForce;
        forceVector.y *= Time.fixedDeltaTime * _yForce;
        _rigidbody.AddForceAtPosition(forceVector, transform.position, ForceMode2D.Impulse);
    }
}
