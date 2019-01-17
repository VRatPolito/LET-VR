using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshObstacle))]
public class PlayerColliderManager : MonoBehaviour
{
    public Transform CameraEye;
    private float _playerheight;
    private float _prevHeadY;
    public bool CharacterController = false;
    [SerializeField]
    [Tooltip("The CharacterController component attached to the Player, in order to manage the Collider height")]
    CharacterController PlayerCharacterController;
    [SerializeField]
    [Tooltip("The CharacterController component attached to the Player, in order to manage the Collider height")]
    CapsuleCollider PlayerCollider;
    [SerializeField]
    [Tooltip("The NavMeshObstacle component attached to the Player, in order to manage the Collider height")]
    NavMeshObstacle PlayerNavMeshObstacle;

    public enum Position { Standing, Crouched, Proned };
    Position Posizione = Position.Standing;
    // Use this for initialization
    void Start()
    {
        _playerheight = LocomotionManager.Instance.CalibrationData.HeadHeight;
        if (PlayerCharacterController != null)
            {
            var heady = CameraEye.localPosition.y;

            if (heady >= _playerheight)
            {
                PlayerCharacterController.height = _playerheight;
                var c = PlayerCharacterController.center;
                PlayerCharacterController.center = new Vector3(0, _playerheight / 2, 0);

                if (PlayerNavMeshObstacle != null)
                {
                    PlayerNavMeshObstacle.height = _playerheight;
                    PlayerNavMeshObstacle.center = new Vector3(0, _playerheight / 2, 0);
                }

                if (Posizione != Position.Standing)
                    Posizione = Position.Standing;
            }
            else if (heady >= _playerheight / 2 && heady < _playerheight)
            {
                PlayerCharacterController.height = CameraEye.localPosition.y;
                var c = PlayerCharacterController.center;
                PlayerCharacterController.center = new Vector3(0, heady / 2, 0);

                if (PlayerNavMeshObstacle != null)
                {
                    PlayerNavMeshObstacle.height = CameraEye.localPosition.y;
                    PlayerNavMeshObstacle.center = new Vector3(0, heady / 2, 0);
                }

                if (Posizione != Position.Standing)
                    Posizione = Position.Standing;
            }
            else if (heady < _playerheight / 2)
            {
                PlayerCharacterController.height = _playerheight / 2;
                var c = PlayerCharacterController.center;
                PlayerCharacterController.center = new Vector3(0, _playerheight / 4, 0);

                if (PlayerNavMeshObstacle != null)
                {
                    PlayerNavMeshObstacle.height = _playerheight / 2;
                    PlayerNavMeshObstacle.center = new Vector3(0, _playerheight / 4, 0);
                }

                if (Posizione != Position.Crouched)
                    Posizione = Position.Crouched;
                }
            }
        if(PlayerCollider != null)
            {
                var heady = CameraEye.localPosition.y;

                if (heady >= _playerheight)
                {
                    PlayerCollider.height = _playerheight;
                    var c = PlayerCollider.center;
                    PlayerCollider.center = new Vector3(0, _playerheight / 2, 0);

                    if (PlayerNavMeshObstacle != null)
                    {
                        PlayerNavMeshObstacle.height = _playerheight;
                        PlayerNavMeshObstacle.center = new Vector3(0, _playerheight / 2, 0);
                    }

                    if (Posizione != Position.Standing)
                        Posizione = Position.Standing;
                }
                else if (heady >= _playerheight / 2 && heady < _playerheight)
                {
                    PlayerCollider.height = CameraEye.localPosition.y;
                    var c = PlayerCollider.center;
                    PlayerCollider.center = new Vector3(0, heady / 2, 0);

                    if (PlayerNavMeshObstacle != null)
                    {
                        PlayerNavMeshObstacle.height = CameraEye.localPosition.y;
                        PlayerNavMeshObstacle.center = new Vector3(0, heady / 2, 0);
                    }

                    if (Posizione != Position.Standing)
                        Posizione = Position.Standing;
                }
                else if (heady < _playerheight / 2)
                {
                    PlayerCollider.height = _playerheight / 2;
                    var c = PlayerCollider.center;
                    PlayerCollider.center = new Vector3(0, _playerheight / 4, 0);

                    if (PlayerNavMeshObstacle != null)
                    {
                        PlayerNavMeshObstacle.height = _playerheight / 2;
                        PlayerNavMeshObstacle.center = new Vector3(0, _playerheight / 4, 0);
                    }

                    if (Posizione != Position.Crouched)
                        Posizione = Position.Crouched;
                }
            }

        _prevHeadY = CameraEye.localPosition.y;
    }    

    // Update is called once per frame
    void Update()
    {
        var HeadY = CameraEye.localPosition.y;

        if (HeadY != _prevHeadY)
        {
            if (PlayerCharacterController != null)
            {
                var heady = CameraEye.localPosition.y;

                if (heady >= _playerheight)
                {
                    PlayerCharacterController.height = _playerheight;
                    var c = PlayerCharacterController.center;
                    PlayerCharacterController.center = new Vector3(0, _playerheight / 2, 0);

                    if (PlayerNavMeshObstacle != null)
                    {
                        PlayerNavMeshObstacle.height = _playerheight;
                        PlayerNavMeshObstacle.center = new Vector3(0, _playerheight / 2, 0);
                    }

                    if (Posizione != Position.Standing)
                        Posizione = Position.Standing;
                }
                else if (heady >= _playerheight / 2 && heady < _playerheight)
                {
                    PlayerCharacterController.height = CameraEye.localPosition.y;
                    var c = PlayerCharacterController.center;
                    PlayerCharacterController.center = new Vector3(0, heady / 2, 0);

                    if (PlayerNavMeshObstacle != null)
                    {
                        PlayerNavMeshObstacle.height = CameraEye.localPosition.y;
                        PlayerNavMeshObstacle.center = new Vector3(0, heady / 2, 0);
                    }

                    if (Posizione != Position.Standing)
                        Posizione = Position.Standing;
                }
                else if (heady < _playerheight / 2)
                {
                    PlayerCharacterController.height = _playerheight / 2;
                    var c = PlayerCharacterController.center;
                    PlayerCharacterController.center = new Vector3(0, _playerheight / 4, 0);

                    if (PlayerNavMeshObstacle != null)
                    {
                        PlayerNavMeshObstacle.height = _playerheight / 2;
                        PlayerNavMeshObstacle.center = new Vector3(0, _playerheight / 4, 0);
                    }

                    if (Posizione != Position.Crouched)
                        Posizione = Position.Crouched;
                }
            }
            if(PlayerCollider != null)
            {
                var heady = CameraEye.localPosition.y;

                if (heady >= _playerheight)
                {
                    PlayerCollider.height = _playerheight;
                    var c = PlayerCollider.center;
                    PlayerCollider.center = new Vector3(0, _playerheight / 2, 0);

                    if (PlayerNavMeshObstacle != null)
                    {
                        PlayerNavMeshObstacle.height = _playerheight;
                        PlayerNavMeshObstacle.center = new Vector3(0, _playerheight / 2, 0);
                    }

                    if (Posizione != Position.Standing)
                        Posizione = Position.Standing;
                }
                else if (heady >= _playerheight / 2 && heady < _playerheight)
                {
                    PlayerCollider.height = CameraEye.localPosition.y;
                    var c = PlayerCollider.center;
                    PlayerCollider.center = new Vector3(0, heady / 2, 0);

                    if (PlayerNavMeshObstacle != null)
                    {
                        PlayerNavMeshObstacle.height = CameraEye.localPosition.y;
                        PlayerNavMeshObstacle.center = new Vector3(0, heady / 2, 0);
                    }

                    if (Posizione != Position.Standing)
                        Posizione = Position.Standing;
                }
                else if (heady < _playerheight / 2)
                {
                    PlayerCollider.height = _playerheight / 2;
                    var c = PlayerCollider.center;
                    PlayerCollider.center = new Vector3(0, _playerheight / 4, 0);

                    if (PlayerNavMeshObstacle != null)
                    {
                        PlayerNavMeshObstacle.height = _playerheight / 2;
                        PlayerNavMeshObstacle.center = new Vector3(0, _playerheight / 4, 0);
                    }

                    if (Posizione != Position.Crouched)
                        Posizione = Position.Crouched;
                }
            }
        }
        _prevHeadY = HeadY;
        }
}
