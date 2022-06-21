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
        [SerializeField] private CellComponent _destinationOne;
        [SerializeField] private CellComponent _destinationTwo;
        [SerializeField] private ChipComponent _targetOne;
        [SerializeField] private ChipComponent _targetTwo;

        private bool isVictory = false;

        [SerializeField] private ColorType _currentPlayer = ColorType.White;

        private Vector3 _cameraBlackPosition = new Vector3(3, 6, 9);
        private Vector3 _cameraBlackRotation = new Vector3(50, 180, 0);
        private Vector3 _cameraWhitePosition = new Vector3(4, 7, -2);
        private Vector3 _cameraWhiteRotation = new Vector3(50, 0, 0);

        private CellComponent[,] _cells = new CellComponent[8,8];
        public CellComponent[,] Cells { get { return _cells; } }

        private void Start()
        {
            CellComponent[] cells = FindObjectsOfType<CellComponent>();
            foreach (CellComponent cell in cells)
            {
                _cells[Mathf.RoundToInt(cell.gameObject.transform.position.x), Mathf.RoundToInt(cell.gameObject.transform.position.z)] = cell;
            }
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
            if (type == typeof(ChipComponent) && component.GetColor == _currentPlayer)
            {
                if (_chip != null)
                {
                    _chip.DeselectChip();
                    SetCellsAndChipsHighlight(BaseClickComponent.HighlightCondition.NotHighlighted, BaseClickComponent.HighlightCondition.NotHighlighted, false);
                }
                _chip = (ChipComponent)component;
                GetDestinationsAndTargets(_chip, _chip.Pair);
                SetCellsAndChipsHighlight(BaseClickComponent.HighlightCondition.CanMoveToCell, BaseClickComponent.HighlightCondition.CanBeEatenChip, true);
            }
            else if (type == typeof(CellComponent))
            {
                if (_destinationOne != null && component.name == _destinationOne.name || _destinationTwo != null && component.name == _destinationTwo.name)
                {
                    _chip.DeselectChip();
                    SetCellsAndChipsHighlight(BaseClickComponent.HighlightCondition.NotHighlighted, BaseClickComponent.HighlightCondition.CanBeEatenChip, false);
                    StartCoroutine(_chip.MoveChip((CellComponent)component, 2f));
                    if (_destinationOne != null && component.name == _destinationOne.name)
                    {
                        if (_targetOne != null) StartCoroutine(DisableChip(_targetOne.gameObject, 1f));
                    }
                    else if (_destinationTwo != null && component.name == _destinationTwo.name)
                    {
                        if (_targetTwo != null) StartCoroutine(DisableChip(_targetTwo.gameObject, 1f));
                    }

                    _chip = null;
                    _destinationOne = null;
                    _destinationTwo = null;
                    _targetOne = null;
                    _targetTwo = null;
                }
            }
        }

        private void GetDestinationsAndTargets(ChipComponent chip, BaseClickComponent cell)
        {
            int cellIndexX = 0, cellIndexZ = 0;
            CellComponent destinationOne = null, destinationTwo = null;
            ChipComponent targetOne = null, targetTwo = null;
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
                if (destinationOne.Pair != null)
                {
                    targetOne = (ChipComponent)destinationOne.Pair;
                    if (targetOne.GetColor == chip.GetColor)
                    {
                        destinationOne = null;
                        targetOne = null;
                    }
                    else if (cellIndexX > 0 && cellIndexZ >= 0 && cellIndexX - 2 < _cells.GetLength(0) && cellIndexZ + 2 < _cells.GetLength(1))
                    {
                        destinationOne = GetDestination(cellIndexX - 2, cellIndexZ + 2);
                        if (destinationOne.Pair != null)
                        {
                            destinationOne = null;
                        }
                    }
                }
            }
            if (cellIndexX >= 0 && cellIndexZ >= 0 && cellIndexX + 1 < _cells.GetLength(0) && cellIndexZ + 1 < _cells.GetLength(1))
            {
                destinationTwo = GetDestination(cellIndexX + 1, cellIndexZ + 1);
                if (destinationTwo.Pair != null)
                {
                    targetTwo = (ChipComponent)destinationTwo.Pair;
                    if (targetTwo.GetColor == chip.GetColor)
                    {
                        destinationTwo = null;
                        targetTwo = null;
                    }
                    else if (cellIndexX > 0 && cellIndexZ >= 0 && cellIndexX + 2 < _cells.GetLength(0) && cellIndexZ + 2 < _cells.GetLength(1))
                    {
                        destinationTwo = GetDestination(cellIndexX + 2, cellIndexZ + 2);
                        if (destinationTwo.Pair != null)
                        {
                            destinationTwo = null;
                        }
                    }
                }
            }
            _destinationOne = destinationOne;
            _destinationTwo = destinationTwo;
            _targetOne = targetOne;
            _targetTwo = targetTwo;
        }

        private CellComponent GetDestination(int x, int z)
        {
            return _cells[x, z];
        }

        private void SetCellsAndChipsHighlight(BaseClickComponent.HighlightCondition cellsHighlight, BaseClickComponent.HighlightCondition chipsHighlight , bool isSelected)
        {
            if (_destinationOne != null)
            {
                _destinationOne.Highlight = cellsHighlight;
                _destinationOne.IsSelected = isSelected;
            }

            if (_destinationTwo != null)
            {
                _destinationTwo.Highlight = cellsHighlight;
                _destinationTwo.IsSelected = isSelected;
            }

            if (_targetOne != null)
            {
                _targetOne.Highlight = chipsHighlight;
                _targetOne.IsSelected = isSelected;
            }

            if (_targetTwo != null)
            {
                _targetTwo.Highlight = chipsHighlight;
                _targetTwo.IsSelected = isSelected;
            }
        }

        private IEnumerator DisableChip(GameObject chip, float time)
        {
            yield return new WaitForSeconds(time);
            chip.SetActive(false);
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
