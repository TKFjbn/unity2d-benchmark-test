using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class copiar : MonoBehaviour
{
    public TMP_Text _textInput;


    // Start is called before the first frame update
    private void awake()
    {
        _textInput = GetComponent<TMP_Text>();
    }

    public void CopiarAlPortapapeles()
    {
        string TextoACopiar = _textInput.text;
        GUIUtility.systemCopyBuffer = TextoACopiar;
        Debug.Log(TextoACopiar);
    }

}
