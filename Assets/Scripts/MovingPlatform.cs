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

    // Update is called once per frame
    void Update()
    {
        Vector3 delta = _speed * Time.deltaTime;
        this.transform.position += delta;
        _currentDistance += delta.magnitude;
        if (_currentDistance >= _distanceToMove)
        {
            _currentDistance = 0f;
            _speed *= -1;
        }
    }

    public Vector3 GetSpeed()
    {
        return _speed;
    }
}
