using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentImage : MonoBehaviour
{
    [SerializeField]
    private float _lifetime;
    private float _currentLifetime = 0f;

    private SpriteRenderer _renderer;

    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        _currentLifetime += Time.deltaTime;

        if (_currentLifetime >= _lifetime)
        {
            _currentLifetime = 0f;
            PersistentImageFactory.Instance.DestroyImage(this.gameObject);
        }
        else if (_lifetime != 0f)
        {
            _renderer.size = _renderer.size * (1 - (_currentLifetime / _lifetime) * 0.05f);
        }
    }
}
