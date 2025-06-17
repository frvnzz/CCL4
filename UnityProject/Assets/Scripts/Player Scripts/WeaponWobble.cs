using UnityEngine;
using System.Collections;

public class WeaponWobble : MonoBehaviour
{
    [Header("Wobble Settings")]
    public float gunSwayAmount = 0.05f;
    public float gunSwaySpeed = 4f;
    public float gunBobAmount = 0.02f;
    public float gunBobSpeed = 8f;
    public float gunKnockbackDecay = 10f;

    private Transform gunTransform;
    private Vector3 gunInitialLocalPos;
    private float gunBobTimer;
    private Vector3 gunSwayOffset;
    private float gunKnockbackOffset = 0f;

    private PlayerCamera playerCamera;
    private PlayerMovement playerMovement;

    public void Init(Transform gun, PlayerCamera cam, PlayerMovement move)
    {
        gunTransform = gun;
        gunInitialLocalPos = gun.localPosition;
        playerCamera = cam;
        playerMovement = move;
    }

    public void AddKnockback(float amount)
    {
        gunKnockbackOffset = amount;
    }

    void Update()
    {
        if (gunTransform == null) return;

        gunSwayOffset = Vector3.Lerp(gunSwayOffset, Vector3.zero, Time.deltaTime * gunSwaySpeed);
        gunSwayOffset += new Vector3(playerCamera.LookInput.x, playerCamera.LookInput.y, 0) * gunSwayAmount;
        gunSwayOffset = Vector3.ClampMagnitude(gunSwayOffset, gunSwayAmount * 2f);

        if (playerMovement.MoveInput.sqrMagnitude > 0.01f)
            gunBobTimer += Time.deltaTime * gunBobSpeed;
        else
            gunBobTimer = 0;

        float bobOffset = Mathf.Sin(gunBobTimer) * gunBobAmount;
        Vector3 bob = new Vector3(0, bobOffset, 0);

        gunKnockbackOffset = Mathf.Lerp(gunKnockbackOffset, 0f, Time.deltaTime * gunKnockbackDecay);
        Vector3 knockback = new Vector3(0, 0, -gunKnockbackOffset);

        Vector3 targetPos = gunInitialLocalPos + gunSwayOffset + bob + knockback;
        gunTransform.localPosition = Vector3.Lerp(gunTransform.localPosition, targetPos, Time.deltaTime * gunSwaySpeed);
    }

    public IEnumerator PlayReloadAnimation(float reloadTime)
    {
        if (gunTransform == null) yield break;

        float moveDuration = reloadTime * 0.4f;
        Vector3 startPos = gunInitialLocalPos;
        Vector3 downPos = gunInitialLocalPos + Vector3.down * 1f;

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            gunTransform.localPosition = Vector3.Lerp(startPos, downPos, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        gunTransform.localPosition = downPos;

        yield return new WaitForSeconds(reloadTime * 0.2f);

        elapsed = 0f;
        while (elapsed < moveDuration)
        {
            gunTransform.localPosition = Vector3.Lerp(downPos, gunInitialLocalPos, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        gunTransform.localPosition = gunInitialLocalPos;
    }
}
