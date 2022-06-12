using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Checkers
{
    public class GameManager : MonoBehaviour
    {
        private BaseClickComponent _baseClickComponent;
        private ChipComponent _chip;
        private CellComponent _cell;

        private PointerEventData _eventData;

        // Start is called before the first frame update
        void Start()
        {

        }

        private void OnEnable()
        {
            _chip.OnClickEventHandler += (chip) => ChipClick();
            _cell.OnClickEventHandler += (cell) => CellClick();
            _baseClickComponent.OnClickEventHandler += (baseClickComponent) => CellClick();

            _chip.OnFocusEventHandler += (cellComponent, isSelect) => ChipFocus();
            _cell.OnFocusEventHandler += (cellComponent, isSelect) => ChipFocus();
            _baseClickComponent.OnFocusEventHandler += (cellComponent, isSelect) => ChipFocus();
            
        }

        private void OnDisable()
        {
            _chip.OnClickEventHandler -= (chip) => ChipClick();
            _cell.OnClickEventHandler -= (cell) => CellClick();
            _baseClickComponent.OnClickEventHandler -= (baseClickComponent) => CellClick();

            _chip.OnFocusEventHandler -= (cellComponent, isSelect) => ChipFocus();
            _cell.OnFocusEventHandler -= (cellComponent, isSelect) => ChipFocus();
            _baseClickComponent.OnFocusEventHandler -= (cellComponent, isSelect) => ChipFocus();
        }

        // Update is called once per frame
        void Update()
        {

        }


        private void ChipClick()
        {

        }

        private void CellClick()
        {

        }

        private void ChipFocus()
        {

        }
    }

   
   
}
