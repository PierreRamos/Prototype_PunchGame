using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_CameraEffects : MonoBehaviour
{
    System_EventHandler EventHandler;

    [SerializeField]
    GameObject _playerObject;

    [SerializeField]
    float _targetZoomDistance = 5.0f;

    [SerializeField]
    float _lerpDuration;

    [SerializeField]
    AnimationCurve _cameraZoomCurve;

    Camera _camera;
    GameObject _currentEnemyObject;

    Coroutine _lerpCameraToPosition;
    float _originalCameraSize;
    float _originalCameraHeight;
    float _startTime;
    float _startZoomSize;
    bool _battleCamera;
    bool _cameraZoomedIn;

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
        _originalCameraSize = _camera.orthographicSize;
        _originalCameraHeight = _camera.transform.position.y;
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
        _currentEnemyObject = enemy;
        _battleCamera = true;
    }

    void TriggerBattleCamera(GameObject enemy, List<MoveSet> dummy)
    {
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

            // if (_lerpCameraToPosition != null)
            //     StopCoroutine(_lerpCameraToPosition);

            _lerpCameraToPosition = StartCoroutine(LerpCameraToPosition(middlePosition));
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
            cameraPosition.z = _camera.transform.position.z;

            if (_lerpCameraToPosition != null)
                StopCoroutine(_lerpCameraToPosition);

            _lerpCameraToPosition = StartCoroutine(LerpCameraToPosition(cameraPosition));
        }
    }

    Vector3 CalculateMiddlePosition(Vector3 playerPosition, Vector3 enemyPosition)
    {
        return new Vector3(
            (playerPosition.x + enemyPosition.x) / 2f,
            playerPosition.y,
            _camera.transform.position.z
        );
    }

    IEnumerator LerpCameraToPosition(Vector3 targetPosition)
    {
        yield return new WaitForEndOfFrame(); //Buffer (If without, bugs lerping when stopping hold battle)

        float elapsedTime = 0f;
        Vector3 initialPosition = _camera.transform.position;

        while (elapsedTime < _lerpDuration)
        {
            float t = _cameraZoomCurve.Evaluate(elapsedTime / _lerpDuration);
            _camera.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        // Ensure the camera reaches the exact target position.
        _camera.transform.position = targetPosition;
    }

    void Zoom(float targetSize)
    {
        //Checks if camera size is lesser than original size
        if (_camera.orthographicSize < _originalCameraSize)
            _cameraZoomedIn = true;

        if (_camera.orthographicSize == targetSize)
        {
            // _cameraZoomedIn = Mathf.Approximately(_camera.orthographicSize, _targetZoomDistance);
            _startTime = 0;
            return;
        }

        if (_startTime == 0)
        {
            _startTime = Time.unscaledTime;
            _startZoomSize = _camera.orthographicSize;
        }

        float elapsedTime = Time.unscaledTime - _startTime;

        if (elapsedTime >= _lerpDuration)
        {
            _camera.orthographicSize = targetSize;
            // _cameraZoomedIn = Mathf.Approximately(_camera.orthographicSize, _targetZoomDistance);
            _startTime = 0;
        }
        else
        {
            float t = elapsedTime / _lerpDuration;
            float step = _cameraZoomCurve.Evaluate(t);

            _camera.orthographicSize = Mathf.Lerp(_startZoomSize, targetSize, step);
        }
    }
}
