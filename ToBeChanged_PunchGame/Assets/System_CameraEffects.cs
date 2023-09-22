using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_CameraEffects : MonoBehaviour
{
    System_EventHandler EventHandler;

    [SerializeField]
    GameObject _playerObject;

    [SerializeField]
    float _zoomSpeed;

    [SerializeField]
    float _targetZoomDistance;

    [SerializeField]
    AnimationCurve _cameraZoomCurve;

    Camera _camera;

    GameObject _currentEnemyObject;

    float _originalCameraSize;

    bool _soloBattleCamera,
        _cameraZoomedIn;
    private float _startTime;
    private float _startZoomSize;

    void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;

        EventHandler.Event_TriggeredSoloBattle += TriggerSoloBattleCamera;
        EventHandler.Event_DeactivatedSoloBattle += StopSoloBattleCamera;
    }

    void OnDisable()
    {
        EventHandler.Event_TriggeredSoloBattle -= TriggerSoloBattleCamera;
        EventHandler.Event_DeactivatedSoloBattle -= StopSoloBattleCamera;
    }

    void Start()
    {
        _camera = Camera.main;
        _originalCameraSize = _camera.orthographicSize;
    }

    void Update()
    {
        if (_soloBattleCamera)
        {
            SoloBattleCamera(_currentEnemyObject);
            ZoomIn();
        }
        else if (_cameraZoomedIn)
            ZoomOut();
    }

    void TriggerSoloBattleCamera(GameObject enemy, List<MoveSet> listOfMoves)
    {
        _currentEnemyObject = enemy;
        _soloBattleCamera = true;
    }

    void StopSoloBattleCamera()
    {
        var playerCameraPosition = _playerObject.transform.position;
        playerCameraPosition.z = _camera.transform.position.z;
        _camera.transform.position = playerCameraPosition;
        _soloBattleCamera = false;
        _startTime = 0;
    }

    void SoloBattleCamera(GameObject enemy)
    {
        var middlePosition = (_playerObject.transform.position.x + enemy.transform.position.x) / 2;

        _camera.transform.position = new Vector3(
            middlePosition,
            _camera.transform.position.y,
            _camera.transform.position.z
        );
    }

    void ZoomIn()
    {
        if (_camera.orthographicSize <= _targetZoomDistance)
        {
            _cameraZoomedIn = true;
            return;
        }

        if (_startTime == 0)
        {
            _startTime = Time.unscaledTime;
            _startZoomSize = _camera.orthographicSize;
        }

        float progress = (Time.unscaledTime - _startTime) * _zoomSpeed;
        float step = _cameraZoomCurve.Evaluate(progress);

        _camera.orthographicSize = Mathf.Lerp(_startZoomSize, _targetZoomDistance, step);
    }

    void ZoomOut()
    {
        if (_camera.orthographicSize >= _originalCameraSize)
        {
            _cameraZoomedIn = false;
            _startTime = 0;
            return;
        }

        if (_startTime == 0)
        {
            _startTime = Time.unscaledTime;
            _startZoomSize = _camera.orthographicSize;
        }

        float progress = (Time.unscaledTime - _startTime) * _zoomSpeed;
        float step = _cameraZoomCurve.Evaluate(progress);

        _camera.orthographicSize = Mathf.Lerp(_startZoomSize, _originalCameraSize, step);
    }
}
