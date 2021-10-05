using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    [SerializeField]
    private Vector3 _force;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DynamicObject obj = collision.GetComponent<DynamicObject>();
        if (obj != null)
        {
            obj.AddWind(_force);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        DynamicObject obj = collision.GetComponent<DynamicObject>();
        if (obj != null)
        {
            obj.AddWind(-_force);
        }
    }
}
