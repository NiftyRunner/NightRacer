using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuCameraManager : MonoBehaviour
{

    [SerializeField] private List<CinemachineVirtualCamera>  _virtualCameras = new List<CinemachineVirtualCamera>();
    [SerializeField] private List<GameObject> texts = new List<GameObject>();
    [SerializeField] SceneLoader loader;

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
        PlayerControlHandler.OnEnterPressed += PlayerControlHandler_OnEnterPressed;
        PlayerControlHandler.OnLRValueChange += PlayerControlHandler_OnLRValueChange;
        DollyDrive.OnDollyFinished += DollyDrive_OnDollyFinished;
    }

    private void OnDisable()
    {
        PlayerControlHandler.OnEnterPressed -= PlayerControlHandler_OnEnterPressed;
        PlayerControlHandler.OnLRValueChange -= PlayerControlHandler_OnLRValueChange;
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
                if (_virtualCameraIndex == 0) return;

                _virtualCameraIndex--;
                SetVirtualCamera(_virtualCameraIndex);
                StartCoroutine(StartWaitTimer());
            }
            else if (inputValues > 0) {
                if(_virtualCameraIndex == (_virtualCameraCount - 1)) return;

                _virtualCameraIndex++;
                SetVirtualCamera(_virtualCameraIndex);
                StartCoroutine(StartWaitTimer());
            }
        }
    }

    private IEnumerator StartWaitTimer()
    {
        inputEnabled = false;
        yield return new WaitForSeconds(1);
        inputEnabled = true;
    }

    private void PlayerControlHandler_OnEnterPressed()
    {
        if(_virtualCameraIndex == 0 && inputEnabled)
        {
            loader.CallLoadCoroutine(sceneToLoad);
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
