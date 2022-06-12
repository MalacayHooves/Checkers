using System;

using UnityEngine.EventSystems;

namespace Checkers
{
    public class ChipComponent : BaseClickComponent
    {
        CellComponent _pairCell;
        public override void OnPointerEnter(PointerEventData eventData)
        {
            CallBackEvent(_pairCell, true);
            //CallBackEvent((CellComponent)Pair, true);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            CallBackEvent(_pairCell, false);
        }

        private void OnCollisionEnter(UnityEngine.Collision collision)
        {
            _pairCell = collision.rigidbody.GetComponent<CellComponent>();
        }
    }
}
