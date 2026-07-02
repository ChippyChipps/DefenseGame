using UnityEngine;

public class PlayerAnimationsEvent : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    private void DisableMovementAndJump() => player.EnableMovementAndJump(false);

    private void EnableMovementAndJump() => player.EnableMovementAndJump(true);
}
