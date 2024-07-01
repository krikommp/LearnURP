using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SceneButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buttonTextComponent;
    
    public void SetButtonText(string text)
    {
        buttonTextComponent.text = text;
    }
}
