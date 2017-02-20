using UnityEngine;
using System.Collections;

public class RuleEnforcer : MonoBehaviour {

    public float _radiusOfAction = 3;
    public float _speed = 3;

    private Square _lockedPlayer;
    private bool _isChasing = false;

	// Update is called once per frame
	void Update () {
        
        if (!_isChasing)
        {
            Collider2D[] squares = Physics2D.OverlapCircleAll(transform.position, _radiusOfAction, 1 << LayerMask.NameToLayer("Square"));
            if (squares.Length < 1) { return; }
            float distanceToSquare = _radiusOfAction;
            foreach (Collider2D collider in squares)
            {
                if ((collider.transform.position - transform.position).magnitude < distanceToSquare)
                {
                    _lockedPlayer = collider.GetComponent<Square>();
                    distanceToSquare = (collider.transform.position - transform.position).magnitude;
                    _isChasing = true;
                    Debug.Log("Closest square: " + _lockedPlayer.name);
                }
            }
        }
        else
        {
            Chase();
        }
	}

    private void Chase()
    {
        if (!_lockedPlayer)
        {
            Unlock();
            return;
        }

        if ((_lockedPlayer.transform.position - transform.position).magnitude < _radiusOfAction * 1.1)
        {
            Debug.Log("Chasing closest square: " + _lockedPlayer.name);
            Vector3 directionToSquare = _lockedPlayer.transform.position - transform.position;
            directionToSquare.Normalize();
            transform.Translate(directionToSquare * _speed * Time.deltaTime);

        }
        else
        {
            Unlock();
            return;
        }
    }

    private void Unlock()
    {
        Debug.Log("Unlocking");
        _isChasing = false;
    }
}
