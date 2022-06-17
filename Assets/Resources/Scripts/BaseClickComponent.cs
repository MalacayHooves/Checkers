using System.Linq;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Checkers
{
    public abstract class BaseClickComponent : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        protected GameManager _gameManager;
        //Меш игрового объекта
        private MeshRenderer _mesh;
        //Список материалов на меше объекта
        [SerializeField] private Material[] _meshMaterials = new Material[3];

        [Tooltip("Цветовая сторона игрового объекта"), SerializeField]
        private ColorType _color;

        /// <summary>
        /// Возвращает цветовую сторону игрового объекта
        /// </summary>
        public ColorType GetColor => _color;

        private HighlightCondition _highlight = 0;
        public HighlightCondition Highlight
        {
            get { return _highlight; }
            set { _highlight = value;
                switch (Highlight)
                {
                    case HighlightCondition.NotHighlighted:
                        gameObject.GetComponent<Renderer>().material = _meshMaterials[0];
                        break;
                    case HighlightCondition.Highlighted:
                        gameObject.GetComponent<Renderer>().material = _meshMaterials[1];
                        break;
                    case HighlightCondition.CanMoveToCell:
                        gameObject.GetComponent<Renderer>().material = _meshMaterials[2];
                        break;
                    case HighlightCondition.CanBeEatenChip:
                        gameObject.GetComponent<Renderer>().material = _meshMaterials[2];
                        break;
                    default:
                        break;
                }
            }
        }
        /// <summary>
        /// Возвращает или устанавливает пару игровому объекту
        /// </summary>
        /// <remarks>У клеток пара - фишка, у фишек - клетка</remarks>
        protected BaseClickComponent _pair;
        public BaseClickComponent Pair { get { return _pair; } set { _pair = value; } }

        /// <summary>
        /// Добавляет дополнительный материал
        /// </summary>
        public void AddAdditionalMaterial(Material material, int index = 1)
        {
            if (index < 1 || index > 2)
            {
                Debug.LogError("Попытка добавить лишний материал. Индекс может быть равен только 1 или 2");
                return;
            }
            _meshMaterials[index] = material;
            //_mesh.materials = _meshMaterials.Where(t => t != null).ToArray();
        }

        /// <summary>
        /// Удаляет дополнительный материал
        /// </summary>
        public void RemoveAdditionalMaterial(int index = 1)
        {
            if (index < 1 || index > 2)
            {
                Debug.LogError("Попытка удалить несуществующий материал. Индекс может быть равен только 1 или 2");
                return;
            }
            _meshMaterials[index] = null;
            _mesh.materials = _meshMaterials.Where(t => t != null).ToArray();
        }

        /// <summary>
        /// Событие клика на игровом объекте
        /// </summary>
        public event ClickEventHandler OnClickEventHandler;

        /// <summary>
        /// Событие наведения и сброса наведения на объект
        /// </summary>
        public event FocusEventHandler OnFocusEventHandler;


        //При навадении на объект мышки, вызывается данный метод
        //При наведении на фишку, должна подсвечиваться клетка под ней
        //При наведении на клетку - подсвечиваться сама клетка
        public abstract void OnPointerEnter(PointerEventData eventData);

        //Аналогично методу OnPointerEnter(), но срабатывает когда мышка перестает
        //указывать на объект, соответственно нужно снимать подсветку с клетки
        public abstract void OnPointerExit(PointerEventData eventData);

        //При нажатии мышкой по объекту, вызывается данный метод
        public void OnPointerClick(PointerEventData eventData)
		{
            OnClickEventHandler?.Invoke(this);
        }

        //Этот метод можно вызвать в дочерних классах (если они есть) и тем самым пробросить вызов
        //события из дочернего класса в родительский
        protected void CallBackEvent(BaseClickComponent target, bool isFocus)
        {
            OnFocusEventHandler?.Invoke(target, isFocus);
        }

		protected virtual void Start()
        {
            _gameManager = FindObjectOfType<GameManager>();
            _mesh = GetComponent<MeshRenderer>();
            //Этот список будет использоваться для набора материалов у меша,
            //в данном ДЗ достаточно массива из 3 элементов
            //1 элемент - родной материал меша, он не меняется
            //2 элемент - материал при наведении курсора на клетку/выборе фишки
            //3 элемент - материал клетки, на которую можно передвинуть фишку или фишки которую можно съесть
            _meshMaterials[0] = _mesh.material;
        }

        public enum HighlightCondition
        {
            NotHighlighted,
            Highlighted,
            CanMoveToCell,
            CanBeEatenChip
        }
    }

    public enum ColorType
    {
        White,
        Black
    }

    public delegate void ClickEventHandler(BaseClickComponent component);
    public delegate void FocusEventHandler(BaseClickComponent component, bool isSelect);
}