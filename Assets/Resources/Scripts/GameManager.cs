using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Checkers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] Camera _camera;

        private BaseClickComponent _baseClickComponent;
        private ChipComponent _chip;
        private CellComponent _cell;

        private bool isVictory = false;

        private ColorType currentPlayer = ColorType.White;

        private PointerEventData _eventData;

        private Vector3 _cameraBlackPosition = new Vector3(3, 6, 9);
        private Vector3 _cameraBlackRotation = new Vector3(50, 180, 0);
        private Vector3 _cameraWhitePosition = new Vector3(4, 7, -2);
        private Vector3 _cameraWhiteRotation = new Vector3(50, 0, 0);

        private GameObject[,] _cells = new GameObject[8,8];
        public GameObject[,] Cells { get { return _cells; } }

        private void Awake()
        {
            CellComponent[] cells = FindObjectsOfType<CellComponent>();
            foreach (CellComponent cell in cells)
            {
                _cells[Mathf.RoundToInt(cell.gameObject.transform.position.x), Mathf.RoundToInt(cell.gameObject.transform.position.z)] = cell.gameObject;
            }
        }
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(TurningSwitchRoutine(ColorType.White));

        }
        /*
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
        */
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
