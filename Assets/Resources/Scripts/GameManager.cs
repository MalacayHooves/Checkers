using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Checkers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] Camera _camera;

        [SerializeField] private ChipComponent _chip;
        [SerializeField] private (CellComponent destinationOne, CellComponent destinationTwo) _destinations;

        private bool isVictory = false;

        private ColorType currentPlayer = ColorType.White;

        private Vector3 _cameraBlackPosition = new Vector3(3, 6, 9);
        private Vector3 _cameraBlackRotation = new Vector3(50, 180, 0);
        private Vector3 _cameraWhitePosition = new Vector3(4, 7, -2);
        private Vector3 _cameraWhiteRotation = new Vector3(50, 0, 0);

        private CellComponent[,] _cells = new CellComponent[8,8];
        public CellComponent[,] Cells { get { return _cells; } }

        private void Awake()
        {
            CellComponent[] cells = FindObjectsOfType<CellComponent>();
            foreach (CellComponent cell in cells)
            {
                _cells[Mathf.RoundToInt(cell.gameObject.transform.position.x), Mathf.RoundToInt(cell.gameObject.transform.position.z)] = cell;
            }
        }
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(TurningSwitchRoutine(currentPlayer));

        }
        
        private void OnEnable()
        {
            BaseClickComponent.OnClickEventHandler += OnClick;
        }

        private void OnDisable()
        {
            BaseClickComponent.OnClickEventHandler -= OnClick;
        }

        private void OnClick(BaseClickComponent component)
        {
            var type = component.GetType();
            if (type == typeof(ChipComponent))
            {
                if (_chip != null)
                {
                    _chip.DeselectChip();
                    ChooseCellSelection(BaseClickComponent.HighlightCondition.NotHighlighted, false);
                }
                _chip = (ChipComponent)component;
                _destinations = GetDestinations(_chip.Pair);
                ChooseCellSelection(BaseClickComponent.HighlightCondition.CanMoveToCell, true);
            }
            else if (type == typeof(CellComponent))
            {
                if (component.name == _destinations.destinationOne.name || component.name == _destinations.destinationTwo.name)
                {
                    StartCoroutine(_chip.MoveChip((CellComponent)component));
                }
            }
        }

        private (CellComponent destinationOne, CellComponent destinationTwo) GetDestinations(BaseClickComponent cell)
        {
            int cellIndexX = 0, cellIndexZ = 0;
            CellComponent destinationOne = null, destinationTwo = null;
            for (int x = 0; x < _cells.GetLength(0); x++)
            {
                for (int z = 0; z < _cells.GetLength(1); z++)
                {
                    if (_cells[x, z].name == cell.name)
                    {
                        cellIndexX = x;
                        cellIndexZ = z;
                    }
                }
            }
            if (cellIndexX > 0 && cellIndexZ >= 0 && cellIndexX - 1 < _cells.GetLength(0) && cellIndexZ + 1 < _cells.GetLength(1))
            {
                destinationOne = GetDestination(cellIndexX - 1, cellIndexZ + 1);
            }
            if (cellIndexX >= 0 && cellIndexZ >= 0 && cellIndexX + 1 < _cells.GetLength(0) && cellIndexZ + 1 < _cells.GetLength(1))
            {
                destinationTwo = GetDestination(cellIndexX + 1, cellIndexZ + 1);
            }
            return (destinationOne, destinationTwo);
        }

        private CellComponent GetDestination(int x, int z)
        {
            return _cells[x, z];
        }

        private void ChooseCellSelection(BaseClickComponent.HighlightCondition highlight, bool isSelected)
        {
            if (_destinations.destinationOne != null)
            {
                _destinations.destinationOne.Highlight = highlight;
                _destinations.destinationOne.IsSelected = isSelected;
            }

            if (_destinations.destinationTwo != null)
            {
                _destinations.destinationTwo.Highlight = highlight;
                _destinations.destinationTwo.IsSelected = isSelected;
            }
        }

        private IEnumerator TurningSwitchRoutine(ColorType colorType)
        {
            while (!isVictory)
            {
                if (colorType == ColorType.White)
                    yield return MoveFromTo(_camera.transform.position, _cameraWhitePosition, _camera.transform.eulerAngles, _cameraWhiteRotation, 3f);
                else if (colorType == ColorType.Black)
                    yield return MoveFromTo(_camera.transform.position, _cameraBlackPosition, _camera.transform.eulerAngles, _cameraBlackRotation, 3f);
               
                
                /*
                    colorType = currentPlayer;

                    if (_camera.transform.position == _cameraWhitePosition)
                    {
                        var pos = _cameraWhitePosition;
                        _cameraWhitePosition = _cameraBlackPosition;
                        _cameraBlackPosition = pos;
                    }

                    yield return MoveFromTo(_camera.transform.position, _cameraWhitePosition, 3f);
                    //StartCoroutine (Turning)*/

            }
            //yield return null;
        }


        private IEnumerator MoveFromTo(Vector3 startPosition, Vector3 endPosition, Vector3 startRotation, Vector3 endRotation, float time)
        {
            var currentTime = 0f;//текущее время смещения
            while (currentTime < time)//асинхронный цикл, выполняется time секунд
            {
                //Lerp - в зависимости от времени (в относительных единицах, то есть от 0 до 1
                //смещает объект от startPosition к endPosition
                _camera.transform.position = Vector3.Lerp(startPosition, endPosition, 1 - (time - currentTime) / time);
                _camera.transform.eulerAngles = Vector3.Lerp(startRotation, endRotation, 1 - (time - currentTime) / time);
                currentTime += Time.deltaTime;//обновление времени, для смещения
                yield return null;//ожидание следующего кадра
            }
            //Из-за неточности времени между кадрами, без этой строчки вы не получите точное значение endPosition
            _camera.transform.position = endPosition;
            _camera.transform.eulerAngles = endRotation;
        }
    }



}
