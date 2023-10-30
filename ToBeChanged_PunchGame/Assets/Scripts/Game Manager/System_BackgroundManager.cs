using UnityEngine;
using UnityEngine.UI;

public class System_BackgroundManager : MonoBehaviour
{
    System_EventHandler EventHandler;

    [Header("Initialization")]
    [SerializeField]
    Animator _backgroundDimAnimator;

    [SerializeField]
    Transform _playerObject;

    [SerializeField]
    RawImage _backgroundBase;

    [SerializeField]
    RawImage _backgroundMiddle;

    [SerializeField]
    RawImage _backgroundLights;

    [Header("Background Settings")]
    [Space]
    [SerializeField]
    float _baseMoveSensitivity;

    [Range(0f, 1f)]
    [SerializeField]
    float _middleSensitivityPercentage;

    [Range(0f, 1f)]
    [SerializeField]
    float _lightsSensitivityPercentage;

    // Declare a variable to store the previous player position.
    Vector3 _previousPlayerPositionBase; // Separate variable for _backgroundBase
    Vector3 _previousPlayerPositionMiddle; // Separate variable for _backgroundBase
    Vector3 _previousPlayerPositionLights; // Separate variable for _backgroundLights

    private void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;

        EventHandler.Event_TriggeredHoldBattle += (dummy) =>
        {
            _backgroundDimAnimator.SetTrigger("Dim");
        };
        EventHandler.Event_TriggeredSoloBattle += (dummy, dummy2) =>
        {
            _backgroundDimAnimator.SetTrigger("Dim");
        };

        EventHandler.Event_StoppedHoldBattle += () =>
        {
            _backgroundDimAnimator.SetTrigger("Normal");
        };
        EventHandler.Event_StoppedSoloBattle += () =>
        {
            _backgroundDimAnimator.SetTrigger("Normal");
        };

        EventHandler.Event_SpecialActive += (specialActive, specialDuration) =>
        {
            string animation = specialActive ? "Dim" : "Normal";
            _backgroundDimAnimator.SetTrigger(animation);
        };
    }

    void Update()
    {
        MoveBackgroundBasedFromPlayer(
            _backgroundBase,
            _baseMoveSensitivity,
            ref _previousPlayerPositionBase
        );
        MoveBackgroundBasedFromPlayer(
            _backgroundMiddle,
            _baseMoveSensitivity * _middleSensitivityPercentage,
            ref _previousPlayerPositionMiddle
        );
        MoveBackgroundBasedFromPlayer(
            _backgroundLights,
            _baseMoveSensitivity * _lightsSensitivityPercentage,
            ref _previousPlayerPositionLights
        );
    }

    void MoveBackgroundBasedFromPlayer(
        RawImage background,
        float moveSensitivity,
        ref Vector3 previousPlayerPosition
    )
    {
        // Calculate the distance the player has moved since the last frame.
        Vector3 currentPlayerPosition = _playerObject.transform.position;
        float distanceMoved =
            (currentPlayerPosition.x - previousPlayerPosition.x) * moveSensitivity * Time.deltaTime;

        // Update the background's uvRect by the distance moved.
        Rect uvRect = background.uvRect;
        uvRect.position += new Vector2(distanceMoved, 0f);

        // Assign the updated uvRect back to the RawImage component.
        background.uvRect = uvRect;

        // Store the current player position as the previous position for the next frame.
        previousPlayerPosition = currentPlayerPosition;
    }
}
