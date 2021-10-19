using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentImageFactory : MonoBehaviour
{
    #region Singleton
    private static PersistentImageFactory instance;
    public static PersistentImageFactory Instance { get { return instance; } }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            _availableImages = new Queue<GameObject>();
        }
        else
        {
            Destroy(this);
        }
    }
    #endregion

    [SerializeField]
    private GameObject _imagePrefab;

    private Queue<GameObject> _availableImages;

    public GameObject CreateImage(Vector3 pos, Vector2 spriteSize)
    {
        GameObject image;

        if (_availableImages.Count <= 0)
        {
            image = Instantiate(_imagePrefab, this.transform);
        }
        else
            image = _availableImages.Dequeue();

        image.transform.position = pos;
        SpriteRenderer renderer = image.GetComponent<SpriteRenderer>();
        renderer.size = spriteSize;
        image.SetActive(true);

        return image;
    }

    public void DestroyImage(GameObject image)
    {
        if (!image.activeSelf)
        {
            return;
        }

        _availableImages.Enqueue(image);
        image.SetActive(false);
    }
}
