using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Data
{
    public readonly static Data Instance = new Data();

    public int BOARD_X = 8;
    public int BOARD_Y = 8;
}

public class TitleScript : MonoBehaviour
{
    [SerializeField]
    TMP_InputField x;

    [SerializeField]
    TMP_InputField y;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Data.Instance.BOARD_X = int.Parse(x.text);
            Data.Instance.BOARD_Y = int.Parse(y.text);

            SceneManager.LoadScene("Game");
        }
    }
}
