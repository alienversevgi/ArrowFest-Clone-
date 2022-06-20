using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class Step : MonoBehaviour
{
    public decimal Index;
    public List<Material> insideMaterials;
    public List<Material> outsideMaterials;

    private List<TextMeshPro> texts;

    public bool isReached;

    private void Awake()
    {
        insideMaterials = this.transform.GetChild(0).
            GetComponentsInChildren<Renderer>().
            Select(renderer => renderer.material).ToList();

        outsideMaterials = this.transform.GetChild(1).
            GetComponentsInChildren<Renderer>().
            Select(renderer => renderer.material).ToList();

        texts = this.transform.GetChild(0).
            GetComponentsInChildren<TextMeshPro>().ToList();
    }

    public void Initialize(decimal value, Color color)
    {
        Index = value;
        texts.ForEach(text => text.text = "X" + value);
        float h, s, v;
        Color.RGBToHSV(color, out h, out s, out v);
        v += 0.2f;

        Color insideColor = Color.HSVToRGB(h, s, v);
        Color outsideColor = color;

        insideMaterials.ForEach(material => material.SetColor("_Color", insideColor));
        outsideMaterials.ForEach(material => material.SetColor("_Color", outsideColor));
    }

    private void OnTriggerEnter(Collider other)
    {
        ArrowDeckManager arrowDeck = other.GetComponent<ArrowDeckManager>();
        
        if (arrowDeck != null)
        {
            //GameEventManager.Instance.OnStepReached.Raise();
            arrowDeck.OnStepReached(Index);
        }
    }
}