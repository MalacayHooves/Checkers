using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Checkers
{
    public abstract class Player : MonoBehaviour
    {
        [SerializeField] protected Camera _camera;
        [SerializeField] protected Player _oppositePlayer;

        [SerializeField] protected ChipComponent _chip;
        [SerializeField] protected CellComponent _destinationOne;
        [SerializeField] protected CellComponent _destinationTwo;
        [SerializeField] protected ChipComponent _targetOne;
        [SerializeField] protected ChipComponent _targetTwo;

        [SerializeField] protected Transform _cameraPosition1;
        [SerializeField] protected Transform _cameraPosition2;
        [SerializeField] protected Transform _cameraPosition3;

        protected bool isVictory = false;

        [SerializeField] protected ColorType _currentPlayer;
        [Tooltip("Время движения фишки"), SerializeField] protected float _chipMoveTime = 1f;
        [Tooltip("Время движения камеры"), SerializeField] protected float _cameraMoveTime = 5f;

        protected CellComponent[,] _cells = new CellComponent[8,8];
        public CellComponent[,] Cells { get { return _cells; } }

        protected void Awake()
        {
            _camera = Camera.main;
            SetPlayerColor();
            SetCellsArray();
            SetOppositePlayer();
            _cameraPosition1 = transform.Find("CameraPosition1");
            _cameraPosition2 = transform.Find("CameraPosition2");
            _cameraPosition3 = transform.Find("CameraPosition3");
        }

        protected abstract void SetPlayerColor();

        protected abstract void SetCellsArray();

        protected abstract void SetOppositePlayer();
        
        protected void OnEnable()
        {
            BaseClickComponent.OnClickEventHandler += OnClick;
            if (Vector3.Distance(_camera.transform.position, _cameraPosition3.position) > 1f)
            {
                StartCoroutine(MoveCamera());
            }
        }

        protected void OnDisable()
        {
            BaseClickComponent.OnClickEventHandler -= OnClick;
        }

        protected void OnClick(BaseClickComponent component)
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
                    StartCoroutine(_chip.MoveChip((CellComponent)component, _chipMoveTime));
                    if (_destinationOne != null && component.name == _destinationOne.name)
                    {
                        if (_targetOne != null) StartCoroutine(DisableChip(_targetOne.gameObject, 0.5f * _chipMoveTime));
                    }
                    else if (_destinationTwo != null && component.name == _destinationTwo.name)
                    {
                        if (_targetTwo != null) StartCoroutine(DisableChip(_targetTwo.gameObject, 0.5f * _chipMoveTime));
                    }

                    _chip = null;
                    _destinationOne = null;
                    _destinationTwo = null;
                    _targetOne = null;
                    _targetTwo = null;
                    StartCoroutine(SwitchTurn(_chipMoveTime));
                }
            }
        }

        protected void GetDestinationsAndTargets(ChipComponent chip, BaseClickComponent cell)
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
                print(destinationTwo.name);
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

        protected CellComponent GetDestination(int x, int z)
        {
            return _cells[x, z];
        }

        protected void SetCellsAndChipsHighlight(BaseClickComponent.HighlightCondition cellsHighlight, BaseClickComponent.HighlightCondition chipsHighlight , bool isSelected)
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

        protected IEnumerator DisableChip(GameObject chip, float time)
        {
            yield return new WaitForSeconds(time);
            chip.SetActive(false);
        }

        protected IEnumerator SwitchTurn(float time)
        {
            yield return new WaitForSeconds(time);
            _oppositePlayer.enabled = true;
            this.enabled = false;
        }

        protected IEnumerator MoveCamera()
        {
            yield return MoveFromTo(_camera.transform.position, _cameraPosition1.position,
                _camera.transform.rotation, _cameraPosition1.transform.rotation, _cameraMoveTime / 4);
            yield return MoveFromTo(_cameraPosition1.position, _cameraPosition2.position,
                _cameraPosition1.rotation, _cameraPosition2.transform.rotation, _cameraMoveTime / 2);
            yield return MoveFromTo(_cameraPosition2.position, _cameraPosition3.position,
                _cameraPosition2.rotation, _cameraPosition3.rotation, _cameraMoveTime / 4);
        }


        protected IEnumerator MoveFromTo(Vector3 startPosition, Vector3 endPosition, Quaternion startRotation, Quaternion endRotation, float time)
        {
            var currentTime = 0f;
            while (currentTime < time)
            {
                _camera.transform.position = Vector3.Lerp(startPosition, endPosition, 1 - (time - currentTime) / time);
                _camera.transform.rotation = Quaternion.Slerp(startRotation, endRotation, 1 - (time - currentTime) / time);
                currentTime += Time.deltaTime;
                yield return null;
            }

            _camera.transform.position = endPosition;
            _camera.transform.rotation = endRotation;
        }
    }



}
