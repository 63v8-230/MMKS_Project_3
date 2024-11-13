using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    SpriteRenderer render;
    RectTransform canvas;
    Vector2 prevSize = Vector2.zero;

    public float factor = 1;

    // Start is called before the first frame update
    void Start()
    {
        render = transform.Find("Back").GetComponent<SpriteRenderer>();
        canvas = GameObject.Find("Canvas").GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(prevSize != canvas.rect.size || true)
        {
            var currentSize = render.size;
            currentSize.x = canvas.rect.size.x * factor;
            currentSize.y = canvas.rect.size.y * factor;
            render.size = currentSize;
        }
        prevSize = canvas.rect.size;
    }
}
