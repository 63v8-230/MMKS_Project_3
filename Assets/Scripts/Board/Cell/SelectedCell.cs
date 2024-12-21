using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCell : MonoBehaviour
{
    [SerializeField]
    private GameObject model;

    private Renderer rend;

    private float changeValue = 0.2922f;

    // Start is called before the first frame update
    void Start()
    {
        rend = model.GetComponent<Renderer>();

        var c = rend.material.color;
        c.a = 0;
        rend.material.color = c;
    }

    // Update is called once per frame
    void Update()
    {
        var c = rend.material.color;
        c.a += changeValue * Time.deltaTime;
        if(c.a >= 0.3f)
        {
            changeValue = -0.2922f;
        }else if(c.a <= 0.1)
        {
            changeValue = 0.2922f;
        }
        rend.material.color = c;

        rend.material.SetColor("_EmissionColor", (c * c.a)*2f);
    }
}
