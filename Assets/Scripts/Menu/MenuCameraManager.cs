using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCameraManager : MonoBehaviour
{

    [SerializeField] private List<CinemachineVirtualCamera>  _virtualCameras = new List<CinemachineVirtualCamera>();
    [SerializeField] private List<GameObject> texts = new List<GameObject>();
    [SerializeField] SceneLoader loader;
    [SerializeField] TweenHandler creditTweenHandler;

    [SerializeField] string sceneToLoad;
    private GameObject currText;
    private GameObject nextText;
    private CinemachineVirtualCamera currVirtualCam;
    [SerializeField] private int _virtualCameraIndex = 0;
    private int _virtualCameraCount = 0;

    private float inputValues;

    private bool inputEnabled = false;

    private void OnEnable()
    {
        //Touch Control Events
        SwipeInput.OnSwipeLeft += LeftInput;
        SwipeInput.OnSwipeRight += RightInput;
        LeftRightScreenTouchInput.OnTouchRight += PlayerControlHandler_OnEnterPressed;
        LeftRightScreenTouchInput.OnTouchLeft += PlayerControlHandler_OnEscPressed;

        //Control Events
        PlayerControlHandler.OnEnterPressed += PlayerControlHandler_OnEnterPressed;
        PlayerControlHandler.OnLRValueChange += PlayerControlHandler_OnLRValueChange;
        PlayerControlHandler.OnEscPressed += PlayerControlHandler_OnEscPressed;
        
        //Dolly events
        DollyDrive.OnDollyFinished += DollyDrive_OnDollyFinished;
    }

    private void OnDisable()
    {
        //Touch Control Events
        SwipeInput.OnSwipeLeft -= LeftInput;
        SwipeInput.OnSwipeRight -= RightInput;
        LeftRightScreenTouchInput.OnTouchRight -= PlayerControlHandler_OnEnterPressed;
        LeftRightScreenTouchInput.OnTouchLeft -= PlayerControlHandler_OnEscPressed;

        //Control Events
        PlayerControlHandler.OnEnterPressed -= PlayerControlHandler_OnEnterPressed;
        PlayerControlHandler.OnLRValueChange -= PlayerControlHandler_OnLRValueChange;
        PlayerControlHandler.OnEscPressed -= PlayerControlHandler_OnEscPressed;
        
        //Dolly Events
        DollyDrive.OnDollyFinished -= DollyDrive_OnDollyFinished;
    }

    void Start()
    {
        _virtualCameraCount = _virtualCameras.Count;
        foreach(GameObject text in texts)
        {
            text.SetActive(false);
        }
    }

    private void Update()
    {
        if(!inputEnabled) return;

        if (inputValues != 0) {
            if (inputValues < 0)
            {
                LeftInput();
            }
            else if (inputValues > 0) 
            {
                RightInput();
            }
        }
    }

    private void LeftInput()
    {
        if (_virtualCameraIndex == 0) return;

        _virtualCameraIndex--;
        SetVirtualCamera(_virtualCameraIndex);
        StartCoroutine(StartWaitTimer());
    }

    private void RightInput()
    {
        if (_virtualCameraIndex == (_virtualCameraCount - 1)) return;

        _virtualCameraIndex++;
        SetVirtualCamera(_virtualCameraIndex);
        StartCoroutine(StartWaitTimer());
    }

    private IEnumerator StartWaitTimer()
    {
        inputEnabled = false;
        yield return new WaitForSeconds(1);
        inputEnabled = true;
    }

    private void PlayerControlHandler_OnEnterPressed()
    {
        if (inputEnabled) {
            switch (_virtualCameraIndex) {
                case 0:
                    loader.CallLoadCoroutine(sceneToLoad);
                    break;
                case 1:
                    creditTweenHandler.PlaySequence();
                    currText.SetActive(false);
                    break;
                case 2:
                    return;
                case 3:
                    Application.Quit();
                    break;
                default:
                    return;
            }
        }

        if(_virtualCameraIndex == 0 && inputEnabled)
        {
            loader.CallLoadCoroutine(sceneToLoad);
        }
    }

    private void PlayerControlHandler_OnEscPressed()
    {
        if (inputEnabled)
        {
            switch (_virtualCameraIndex)
            {
                case 0:
                    return;
                case 1:
                    creditTweenHandler.PlaySequenceReverse();
                    currText.SetActive(true);
                    break;
                default:
                    return;
            }
        }
    }

    private void PlayerControlHandler_OnLRValueChange(float obj)
    {
        inputValues = obj;
    }


    private void DollyDrive_OnDollyFinished()
    {
        inputEnabled = true;
        currVirtualCam = _virtualCameras[0];
        currText = texts[0];
        currText.SetActive(true);
    }

    private void SetVirtualCamera(int virtualCameraIndex)
    {
        CinemachineVirtualCamera nextVCam = _virtualCameras[virtualCameraIndex];
        nextText = texts[virtualCameraIndex];
        if (nextVCam == null || nextText == null)
        {
            Debug.Log("No next cam or text");
            return;
        }

        currVirtualCam.Priority = 0;
        nextVCam.Priority = 10;
        currVirtualCam = nextVCam;

        currText.SetActive(false);
        Invoke(nameof(CallSetNextText), 0.5f);
        
    }

    private void CallSetNextText()
    {
        SetNextText(nextText);
    }

    private void SetNextText(GameObject nextText)
    {
        nextText.SetActive(true);
        currText = nextText;
    }
}
