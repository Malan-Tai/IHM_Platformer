using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    private float _distanceToMove;
    private float _currentDistance;

    [SerializeField]
    private Vector3 _speed;
    private Vector3 _prevDelta;

    void LateUpdate()
    {
        Vector3 delta = _speed * Time.deltaTime;
        this.transform.position += delta;
        _currentDistance += delta.magnitude;
        if (_currentDistance >= _distanceToMove)
        {
            _currentDistance = 0f;
            _speed *= -1;
        }
        _prevDelta = delta;
    }

    public Vector3 GetDelta()
    {
        return _prevDelta;
    }
}
