using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCell : MonoBehaviour
{
    [SerializeField]
    private GameObject model;

    private Renderer rend;

    private float changeValue = 0.3922f;

    // Start is called before the first frame update
    void Start()
    {
        rend = model.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        var c = rend.material.color;
        c.a += changeValue * Time.deltaTime;
        if(c.a >= 0.39f)
        {
            changeValue = -0.3922f;
        }else if(c.a <= 0)
        {
            changeValue = 0.3922f;
        }
        rend.material.color = c;
    }
}
