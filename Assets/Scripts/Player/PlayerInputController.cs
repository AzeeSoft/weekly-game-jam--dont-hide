using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerInputController : MonoBehaviour
{
    public class PlayerInput
    {
        public Vector3 Move;
//        public bool Sprint;
        public bool ToggleCamouflageMode;
    }

    private PlayerModel _playerModel;
    public PlayerInput playerInput { get; private set; } = new PlayerInput();

    private void Awake()
    {
        _playerModel = GetComponent<PlayerModel>();
    }

    public void Update()
    {
        UpdatePlayerInput();
    }

    public void UpdatePlayerInput()
    {
        float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
        float vertical = CrossPlatformInputManager.GetAxis("Vertical");

        playerInput.Move = new Vector3(horizontal, 0f, vertical);
        //_playerInput.Sprint = CrossPlatformInputManager.GetButton("Sprint");
        playerInput.ToggleCamouflageMode = CrossPlatformInputManager.GetButtonDown("CamouflageToggle");
    }
}