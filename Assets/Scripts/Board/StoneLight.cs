using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneLight : MonoBehaviour
{
    float t = 0.5f;
    Material m;
    Color cl;

    float f = 3;

    // Update is called once per frame
    void Update()
    {
        t -= Time.deltaTime;
        var c = m.color;
        c -= cl * Time.deltaTime * f;
        //c.a -= Time.deltaTime * 2;
        m.color = c;

        if (t <= 0)
        {

            Destroy(gameObject);
        }

    }

    public void InitW()
    {
        m = GetComponent<Renderer>().material;
        m.color = Color.white * 2;

        cl = Color.white;

        f = 2;
        Debug.Log(f);
    }

    public void InitB()
    {
        m = GetComponent<Renderer>().material;
        cl = new Color(0.529f, 0, 0.875f);
        m.color = cl * 2;

        f = 4;
        Debug.Log(f);
    }
}
