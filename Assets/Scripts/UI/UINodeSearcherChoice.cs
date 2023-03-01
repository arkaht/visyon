using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINodeSearcherChoice : MonoBehaviour
{
    public Text Text { get; private set; }
    public Button Button { get; private set; }

    public string ID;

    void Awake()
    {
        Text = GetComponentInChildren<Text>();
        Button = GetComponent<Button>();
    }
}
