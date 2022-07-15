using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelScript : MonoBehaviour
{
    [SerializeField]
    private Text _text;

    public void ShowAndSetText(string text)
    {
        gameObject.SetActive(true);
        _text.text = text;
    }
}
