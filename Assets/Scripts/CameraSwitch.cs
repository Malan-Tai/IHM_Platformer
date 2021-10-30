using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    [SerializeField]
    private bool _isVertical;

    [SerializeField]
    private Transform _negativeAxisCam;
    [SerializeField]
    private Transform _positiveAxisCam;

    private Vector3 _boxCenter;

    private void Awake()
    {
        _boxCenter = GetComponent<BoxCollider2D>().bounds.center;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Camera cam = _negativeAxisCam.GetComponentInChildren<Camera>();
        if (cam == null) cam = _positiveAxisCam.GetComponentInChildren<Camera>();

        if (_isVertical)
        {
            if (collision.bounds.center.y < _boxCenter.y)
            {
                cam.transform.SetParent(_negativeAxisCam, false);
            }
            else
            {
                cam.transform.SetParent(_positiveAxisCam, false);
            }
        }
        else
        {
            if (collision.bounds.center.x < _boxCenter.x)
            {
                cam.transform.SetParent(_negativeAxisCam, false);
            }
            else
            {
                cam.transform.SetParent(_positiveAxisCam, false);
            }
        }
    }
}
