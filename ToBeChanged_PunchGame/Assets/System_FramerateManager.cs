using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_FramerateManager : MonoBehaviour
{
    [Header("Framerate Settings")]
    [SerializeField]
    private int _targetFramerate;

    [SerializeField]
    private bool _limitFramerate;
    private bool _framerateLimited;

    private void Update()
    {
        if (_framerateLimited && _limitFramerate)
            return;
        else if (_framerateLimited && !_limitFramerate)
        {
            Application.targetFrameRate = 0;
            _framerateLimited = false;
        }

        if (!_framerateLimited && _limitFramerate)
        {
            Application.targetFrameRate = _targetFramerate;
            _framerateLimited = true;
        }
    }
}
