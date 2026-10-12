using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_GroundManager : MonoBehaviour
{
    [Header("Initialization")]
    [SerializeField]
    Transform _player;

    [SerializeField]
    Transform _groundParent;

    [SerializeField]
    GameObject _groundPrefab;

    [SerializeField]
    GameObject _initialGround;
    Dictionary<GroundLocation, GameObject> _groundLocations =
        new Dictionary<GroundLocation, GameObject>();

    float _groundSize;

    float _groundHalfSize;

    float _lastPlayerPositionX;

    private void Awake()
    {
        _groundLocations.Add(GroundLocation.Center, _initialGround);
        _groundSize = _initialGround.GetComponent<SpriteRenderer>().size.x;
        _groundHalfSize = _groundSize / 2;
    }

    private void Start()
    {
        InstantiateLeftRightGround();
    }

    private void FixedUpdate()
    {
        CheckPlayerPosition();
    }

    private void CheckPlayerPosition()
    {
        if (_lastPlayerPositionX == _player.position.x)
            return;

        _lastPlayerPositionX = _player.position.x;

        var currentGroundMinPosition =
            _groundLocations[GroundLocation.Center].transform.position.x - _groundHalfSize;
        var currentGroundMaxPosition =
            _groundLocations[GroundLocation.Center].transform.position.x + _groundHalfSize;

        if (_lastPlayerPositionX < currentGroundMinPosition)
            RearrangeGroundPosition(Direction.Left);
        else if (_lastPlayerPositionX > currentGroundMaxPosition)
            RearrangeGroundPosition(Direction.Right);
    }

    void InstantiateLeftRightGround()
    {
        Vector3 leftPosition = new Vector2(-_groundSize, 0f);
        Vector3 rightPosition = new Vector2(_groundSize, 0f);

        var leftGround = Instantiate(
            _groundPrefab,
            _initialGround.transform.position + leftPosition,
            _initialGround.transform.rotation
        );
        var rightGround = Instantiate(
            _groundPrefab,
            _initialGround.transform.position + rightPosition,
            _initialGround.transform.rotation
        );

        leftGround.transform.parent = _groundParent;
        rightGround.transform.parent = _groundParent;

        _groundLocations.Add(GroundLocation.Left, leftGround);
        _groundLocations.Add(GroundLocation.Right, rightGround);
    }

    //Rearranges order of ground in _groundLocations dictionary
    void RearrangeGroundPosition(Direction direction)
    {
        var previousCenter = _groundLocations[GroundLocation.Center];

        if (direction == Direction.Left)
        {
            _groundLocations[GroundLocation.Center] = _groundLocations[GroundLocation.Left];
            _groundLocations[GroundLocation.Left] = _groundLocations[GroundLocation.Right];
            _groundLocations[GroundLocation.Right] = previousCenter;
        }
        else if (direction == Direction.Right)
        {
            _groundLocations[GroundLocation.Center] = _groundLocations[GroundLocation.Right];
            _groundLocations[GroundLocation.Right] = _groundLocations[GroundLocation.Left];
            _groundLocations[GroundLocation.Left] = previousCenter;
        }

        MoveGroundToPosition();
    }

    //Moves ground to appropriate positions
    void MoveGroundToPosition()
    {
        var centerGroundPosition = _groundLocations[GroundLocation.Center].transform.position;
        var leftGroundPosition = _groundLocations[GroundLocation.Left].transform.position;
        var rightGroundPosition = _groundLocations[GroundLocation.Right].transform.position;

        Vector3 groundSize = new Vector3(_groundSize, 0f);

        if (leftGroundPosition.x != centerGroundPosition.x - _groundSize)
        {
            leftGroundPosition = centerGroundPosition - groundSize;
            _groundLocations[GroundLocation.Left].transform.position = leftGroundPosition;
        }

        if (rightGroundPosition.x != centerGroundPosition.x + _groundSize)
        {
            rightGroundPosition = centerGroundPosition + groundSize;
            _groundLocations[GroundLocation.Right].transform.position = rightGroundPosition;
        }
    }
}
