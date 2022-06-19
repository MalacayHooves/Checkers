using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

namespace Checkers
{
    public class CellComponent : BaseClickComponent
    {
        private Dictionary<NeighborType, CellComponent> _neighbors;


        /// <summary>
        /// Возвращает соседа клетки по указанному направлению
        /// </summary>
        /// <param name="type">Перечисление направления</param>
        /// <returns>Клетка-сосед или null</returns>
        public CellComponent GetNeighbors(NeighborType type) => _neighbors[type];


        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (_isSelected) return;
            if (Pair != null && Pair.IsSelected) return;
            Highlight = HighlightCondition.Highlighted;
            CallBackEvent(this, true);
        }


        public override void OnPointerExit(PointerEventData eventData)
        {
            if (_isSelected) return;
            if (Pair != null && Pair.IsSelected) return;
            Highlight = HighlightCondition.NotHighlighted;
            CallBackEvent(this, false);
        }

        private void OnEnable()
        {
            switch (GetColor)
            {
                case ColorType.White:
                    AddAdditionalMaterial(Resources.Load<Material>("Materials/WhiteCellMaterialHighlighted"), 1);
                    break;
                case ColorType.Black:
                    AddAdditionalMaterial(Resources.Load<Material>("Materials/BlackCellMaterialHighlighted"), 1);
                    break;
                default:
                    break;
            }
            
            AddAdditionalMaterial(Resources.Load<Material>("Materials/CanMoveToCellMaterial"), 2);
        }

        //private void OnDisable()
        //{

        //}





        /// <summary>
        /// Конфигурирование связей клеток
        /// </summary>
        public void Configuration(Dictionary<NeighborType, CellComponent> neighbors)
        {
            {
                if (_neighbors != null) return;
                _neighbors = neighbors;
            }


        }
    }
    
    
    /// <summary>
    /// Тип соседа клетки
    /// </summary>
    public enum NeighborType : byte
    {
        /// <summary>
        /// Клетка сверху и слева от данной
        /// </summary>
        TopLeft,
        /// <summary>
        /// Клетка сверху и справа от данной
        /// </summary>
        TopRight,
        /// <summary>
        /// Клетка снизу и слева от данной
        /// </summary>
        BottomLeft,
        /// <summary>
        /// Клетка снизу и справа от данной
        /// </summary>
        BottomRight
    }
}