using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class loginfo : MonoBehaviour
{

    public TextMeshProUGUI lblAuto;
    public TMP_InputField logText;
    private string texto = "";
    private int ultimoNum;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var total = lblAuto.text;
        var totalInt = int.Parse(total);

        texto = logText.text;

        if (totalInt > 0 && totalInt % 50 == 0)
        {
            if (totalInt != ultimoNum)
            {
                Debug.Log(texto);
                ultimoNum = totalInt;
            }
        }


    }
}
