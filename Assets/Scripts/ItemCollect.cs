using UnityEngine;
using UnityEditor.UI;
using TMPro;

public class ItemCollect : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public ScriptableObject itemData;
    public Canvas canvas;
    public TextMeshProUGUI TextMeshPro;


    void Start()
    {
        canvas.enabled = false;
        TextMeshPro.text = "Oi, testando";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowCanvas()
    {
        canvas.enabled = true;
    }
}
