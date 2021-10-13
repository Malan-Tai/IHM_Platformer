using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeBehavior : MonoBehaviour
{
    [SerializeField]
    private float _shakeDuration = 0f;
    [SerializeField]
    private float _shakeMagnitude = 0.7f;
    [SerializeField]
    private float _dampingSpeed = 1.0f;
    private Vector3 initialPosition;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (_shakeDuration > 0)
        {
            //transform.localPosition = initialPosition + Random.insideUnitSphere * _shakeMagnitude;
            transform.localPosition = initialPosition + (new Vector3(0, Random.Range(-0.2f, 0.2f), 0) )* _shakeMagnitude;

            _shakeDuration -= Time.deltaTime * _dampingSpeed;
        }
        else
        {
            _shakeDuration = 0f;
            transform.localPosition = initialPosition;
        }
    }
    public void TriggerShake()
    {
        _shakeDuration = 0.5f;
    }
}
