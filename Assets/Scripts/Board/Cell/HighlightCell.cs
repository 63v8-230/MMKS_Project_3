using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightCell : MonoBehaviour
{
    float t = 1;
    Material m;
    Color c;
    float d;

    // Start is called before the first frame update
    void Start()
    {
        m = gameObject.transform.Find("Model").gameObject.GetComponent<Renderer>().material;
        c = m.color;
        if (c.r > 1 || c.g > 1 || c.b > 1)
            d = (1 / t) * 2;
        else
            d = 1 / t;
    }

    // Update is called once per frame
    void Update()
    {
        t -= Time.deltaTime;
        c.a -= d * Time.deltaTime;
        m.color = c;
        m.SetColor("_EmissionColor", (c * c.a) * 2f);
        if (t < 0)
            Destroy(gameObject);
    }
}
