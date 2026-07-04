using UnityEngine;

public class EntityAnimationsEvents : MonoBehaviour
{
    private Entity entity;

    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
    }

    public void DamageTargets() => entity.DamageEnemies();

    private void DisableMovementAndJump() => entity.EnableMovementAndJump(false);

    private void EnableMovementAndJump() => entity.EnableMovementAndJump(true);
}
