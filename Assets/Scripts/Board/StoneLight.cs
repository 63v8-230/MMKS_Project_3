using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoneLight : MonoBehaviour
{
    float t = 0.5f;
    Material m;
    Material r;
    Color cl;

    float f = 3;


    /// <summary>
    /// <para>0: Normal</para>
    /// <para>1: Skill</para>
    /// </summary>
    int kind = -1;

    // Update is called once per frame
    void Update()
    {
        t -= Time.deltaTime;

        switch (kind)
        {
            case 0:
                var c = m.color;
                c -= cl * Time.deltaTime * f;
                m.color = c;
                break;

            case 1:
                var rc = r.color;
                rc.a = t * 3;
                r.color = rc;
                break;
            default:
                break;
        }
        

        

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

        var rt = transform.Find("StoneFire");
        r = rt.GetComponent<Renderer>().material;
        r.color = Color.black * 0;
        var ob = Instantiate(Resources.Load<GameObject>("Models/StoneFlip"));
        ob.transform.SetParent(transform, true);
        ob.transform.localPosition = rt.transform.localPosition;

        f = 2;

        kind = 0;
    }

    public void InitB()
    {
        m = GetComponent<Renderer>().material;
        cl = new Color(0.529f, 0, 0.875f);
        m.color = cl * 2;

        var rt = transform.Find("StoneFire");
        r = rt.GetComponent<Renderer>().material;
        r.color = Color.black * 0;
        var ob = Instantiate(Resources.Load<GameObject>("Models/StoneFlip"));
        ob.transform.SetParent(transform, true);
        ob.transform.localPosition = rt.transform.localPosition;

        f = 4;

        kind = 0;
    }

    public void InitS()
    {
        GetComponent<MeshRenderer>().enabled = false;

        r = transform.Find("StoneFire").GetComponent<Renderer>().material;
        r.color = Color.white * 1.5f;

        kind = 1;
    }
}
