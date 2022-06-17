using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Checkers
{
    public class ChipComponent : BaseClickComponent
    {
        public override void OnPointerEnter(PointerEventData eventData)
        {
            GetPair();
            Highlight = HighlightCondition.Highlighted;
            if (Pair != null) Pair.Highlight = HighlightCondition.Highlighted;
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            Highlight = HighlightCondition.NotHighlighted;
            if (Pair != null) Pair.Highlight = HighlightCondition.NotHighlighted;
        }

        private void OnEnable()
        {
            switch (GetColor)
            {
                case ColorType.White:
                    AddAdditionalMaterial(Resources.Load<Material>("Materials/WhiteChipMaterialHighlighted"), 1);
                    break;
                case ColorType.Black:
                    AddAdditionalMaterial(Resources.Load<Material>("Materials/BlackChipMaterialHighlighted"), 1);
                    break;
                default:
                    break;
            }

            AddAdditionalMaterial(Resources.Load<Material>("Materials/CanBeEatenMaterial"), 2);
        }

        private void GetPair()
        {
            Pair = _gameManager.Cells[Mathf.RoundToInt(gameObject.transform.position.x), Mathf.RoundToInt(gameObject.transform.position.z)].GetComponent<BaseClickComponent>();
            Pair.Pair = this;
        }
    }
}
