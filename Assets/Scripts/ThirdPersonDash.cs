using System.Collections;
using UnityEngine;

public class ThirdPersonDash : MonoBehaviour
{
    PlayerMovement moveScript;
    DashCloudFX cloudFX;
    DashStamina stamina;

    public float dashSpeed = 20f;
    public float dashTime = 0.35f;

    public bool IsDashing { get; private set; }

    void Start()
    {
        moveScript = GetComponent<PlayerMovement>();
        cloudFX = GetComponent<DashCloudFX>();
        stamina = GetComponent<DashStamina>();
    }

    void Update()
    {
        bool ready = stamina == null || stamina.CanDash();

        if (Input.GetMouseButtonDown(0) && !IsDashing && ready)
        {
            if (stamina != null) stamina.Consume();
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        IsDashing = true;

        Vector3 dashDirection = moveScript.moveDir;

        if (cloudFX != null)
            cloudFX.Begin(dashDirection);

        float elapsed = 0f;

        while (elapsed < dashTime)
        {
            float currentSpeed = Mathf.Lerp(dashSpeed, moveScript.moveSpeed, elapsed / dashTime);

            moveScript.controller.Move(
                dashDirection * currentSpeed * Time.deltaTime
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (cloudFX != null)
            cloudFX.End();

        IsDashing = false;
    }
}