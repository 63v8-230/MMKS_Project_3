using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WebGL_MouseTest : MonoBehaviour
{
    public TextMeshProUGUI tx;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var p = Input.mousePosition;
        var t = Input.touches;
        bool c = false;
        if (t.Length > 0 || Input.GetMouseButtonDown(0))
            c = true;

        tx.text = $"{p.x:F2} / {p.y:F2}\n{c}";
    }
}
