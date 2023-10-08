using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class System_CameraEffects : MonoBehaviour
{
    System_EventHandler EventHandler;

    [SerializeField]
    CinemachineVirtualCamera _cinemachineCamera;

    [SerializeField]
    GameObject _playerObject;

    [SerializeField]
    float _targetZoomDistance;

    [SerializeField]
    float _lerpDuration;

    [SerializeField]
    AnimationCurve _cameraZoomCurve;

    CinemachineFramingTransposer _framingTransposer;
    Camera _camera;
    GameObject _currentEnemyObject;
    Coroutine _lerpCameraToPosition;
    float _originalCameraSize;
    float _originalCameraHeight;
    float _startTime;
    float _startZoom;
    bool _battleCamera;
    bool _cameraZoomedIn;
    bool _lerpCameraToPositionIsRunning;

    void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;

        EventHandler.Event_TriggeredSoloBattle += TriggerBattleCamera;
        EventHandler.Event_TriggeredHoldBattle += TriggerBattleCamera;
        EventHandler.Event_StoppedSoloBattle += StopBattleCamera;
        EventHandler.Event_StoppedHoldBattle += StopBattleCamera;
    }

    void OnDisable()
    {
        EventHandler.Event_TriggeredSoloBattle -= TriggerBattleCamera;
        EventHandler.Event_TriggeredHoldBattle -= TriggerBattleCamera;
        EventHandler.Event_StoppedSoloBattle -= StopBattleCamera;
        EventHandler.Event_StoppedHoldBattle -= StopBattleCamera;
    }

    void Start()
    {
        _camera = Camera.main;
        if (_camera == null)
        {
            Debug.LogError("Main Camera not found!");
            enabled = false; // Disable the script if the camera is not found.
            return;
        }
        _framingTransposer =
            _cinemachineCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

        _originalCameraSize = _cinemachineCamera.m_Lens.OrthographicSize;
        _originalCameraHeight = _cinemachineCamera.transform.position.y;
    }

    void Update()
    {
        if (!_battleCamera && !_cameraZoomedIn)
            return;

        if (_battleCamera)
        {
            StartBattleCamera(_currentEnemyObject);
            Zoom(_targetZoomDistance);
        }
        else if (_cameraZoomedIn)
        {
            Zoom(_originalCameraSize);
        }
    }

    void TriggerBattleCamera(GameObject enemy)
    {
        _cinemachineCamera.m_Follow = null;
        _currentEnemyObject = enemy;
        _battleCamera = true;
    }

    void TriggerBattleCamera(GameObject enemy, List<MoveSet> dummy)
    {
        _cinemachineCamera.m_Follow = null;
        _currentEnemyObject = enemy;
        _battleCamera = true;
    }

    void StartBattleCamera(GameObject enemy)
    {
        if (_playerObject != null && enemy != null)
        {
            Vector3 middlePosition = CalculateMiddlePosition(
                _playerObject.transform.position,
                enemy.transform.position
            );

            _lerpCameraToPosition = StartCoroutine(LerpCameraToPosition(middlePosition, false));
        }

        //
        Vector3 CalculateMiddlePosition(Vector3 playerPosition, Vector3 enemyPosition)
        {
            return new Vector3(
                (playerPosition.x + enemyPosition.x) / 2f,
                playerPosition.y,
                _cinemachineCamera.transform.position.z
            );
        }
    }

    void StopBattleCamera()
    {
        _battleCamera = false;
        _startTime = 0;
        if (_playerObject != null)
        {
            Vector3 cameraPosition = _playerObject.transform.position;
            cameraPosition.y = _originalCameraHeight;
            cameraPosition.z = _cinemachineCamera.transform.position.z;

            if (_lerpCameraToPositionIsRunning)
            {
                StopCoroutine(_lerpCameraToPosition);
                _lerpCameraToPositionIsRunning = false;
            }

            _lerpCameraToPosition = StartCoroutine(LerpCameraToPosition(cameraPosition, true));
        }
        // _cinemachineCamera.m_Follow = _playerObject.transform;
    }

    void Zoom(float targetSize)
    {
        if (_cinemachineCamera.m_Lens.OrthographicSize < _originalCameraSize)
            _cameraZoomedIn = true;

        if (_cinemachineCamera.m_Lens.OrthographicSize == targetSize)
        {
            _startTime = 0;
            return;
        }

        if (_startTime == 0)
        {
            _startTime = Time.unscaledTime;
            _startZoom = _cinemachineCamera.m_Lens.OrthographicSize;
        }

        float elapsedTime = Time.unscaledTime - _startTime;

        if (elapsedTime >= _lerpDuration)
        {
            _cinemachineCamera.m_Lens.OrthographicSize = targetSize;
            _startTime = 0;
        }
        else
        {
            float t = elapsedTime / _lerpDuration;
            float step = _cameraZoomCurve.Evaluate(t);

            _cinemachineCamera.m_Lens.OrthographicSize = Mathf.Lerp(_startZoom, targetSize, step);
        }
    }

    IEnumerator LerpCameraToPosition(Vector3 targetPosition, bool isStopping)
    {
        yield return new WaitForEndOfFrame(); //Buffer (If without, bugs lerping when stopping hold battle)

        if (_lerpCameraToPositionIsRunning == true)
            yield break;

        _lerpCameraToPositionIsRunning = true;

        float elapsedTime = 0f;
        Vector3 initialPosition = _cinemachineCamera.transform.position;

        while (elapsedTime < _lerpDuration)
        {
            if (isStopping)
                targetPosition.x = _playerObject.transform.position.x;

            float t = _cameraZoomCurve.Evaluate(elapsedTime / _lerpDuration);
            _cinemachineCamera.transform.position = Vector3.Lerp(
                initialPosition,
                targetPosition,
                t
            );
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        // Ensure the camera reaches the exact target position.
        _cinemachineCamera.transform.position = targetPosition;

        if (isStopping)
            _cinemachineCamera.m_Follow = _playerObject.transform;

        _lerpCameraToPositionIsRunning = false;
    }
}
