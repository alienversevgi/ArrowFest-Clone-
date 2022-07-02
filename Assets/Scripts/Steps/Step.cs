using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using Game.Level;

namespace Game
{
    public class Step : MonoBehaviour
    {
        #region Fields

        [SerializeField] private List<Renderer> _insideRenderers;
        [SerializeField] private List<Renderer> _outsideRenderers;

        private decimal _index;
        private List<TextMeshPro> _texts;
        private List<Chibi> _chibis;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _texts = this.transform.GetChild(0).
                GetComponentsInChildren<TextMeshPro>().ToList();
            _chibis = this.transform.
                GetComponentsInChildren<Chibi>().ToList();
        }

        private void OnTriggerEnter(Collider other)
        {
            ArrowDeckManager arrowDeck = other.GetComponent<ArrowDeckManager>();

            if (arrowDeck != null)
            {
                arrowDeck.OnStepReached(_index);
            }
        }

        #endregion

        #region Public Methods

        public void Initialize(decimal value, Color color)
        {
            _index = value;

            _chibis.ForEach(chibi => chibi.Initialize(false));
            _texts.ForEach(text => text.text = "X" + value);

            float h, s, v;
            Color.RGBToHSV(color, out h, out s, out v);
            v += 0.2f;

            Color outsideColor = Color.HSVToRGB(h, s, v);
            Color insideColor = color;

            MaterialPropertyBlock inside = new MaterialPropertyBlock();
            inside.SetColor("_Color", insideColor);

            MaterialPropertyBlock outside = new MaterialPropertyBlock();
            outside.SetColor("_Color", outsideColor);

            _insideRenderers.ForEach(it => it.SetPropertyBlock(inside));
            _outsideRenderers.ForEach(it => it.SetPropertyBlock(outside));
        }

        #endregion
    }
}
