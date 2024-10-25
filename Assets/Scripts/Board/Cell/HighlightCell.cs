using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightCell : MonoBehaviour
{
    float t = 2;
    Material m;
    Color c;
    float d;

    // Start is called before the first frame update
    void Start()
    {
        m = gameObject.transform.Find("Model").gameObject.GetComponent<Renderer>().material;
        c = m.color;
        d = 1 / t;
    }

    // Update is called once per frame
    void Update()
    {
        t -= Time.deltaTime;
        c.a -= d * Time.deltaTime;
        Debug.Log(c);
        m.color = c;
        if (t < 0)
            Destroy(gameObject);
    }
}
