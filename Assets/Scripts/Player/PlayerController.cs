using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private EnvironmentManager envManager;
    [SerializeField] private Transform[] wheels;

    [SerializeField] private float movementMultiplier = 1f;
    [SerializeField] private float autoForce = 1f;
    [SerializeField] private float maxSpinSpeed = 360f;
    [SerializeField] private float accelerationRate = 0.2f; // how fast speed ramps up
    [SerializeField] private float maxSpeedMultiplier = 3f;

    [Header("Tilt Values")]
    [SerializeField] private Animator bikeAnimator;
    [SerializeField] private Animator riderAnimator;
    [SerializeField] private float tiltSpeed = 120f;

    private float movementValues;
    private float currentTilt;

    private bool inputEnabled = false;

    private float playerStartSpeed = 30f;
    private float playerSpeed;
    private float playerSpeedMultiplier = 1f;

    public float GetPlayerSpeedMultiplier() => playerSpeedMultiplier;
    public float GetPlayerSpeed() => playerSpeed;


    private void OnEnable()
    {
        PlayerControlHandler.OnLRValueChange += LeftRightValueSetter;
        SwipeInput.OnTouchValueChange += LeftRightValueSetter;
        DollyDrive.OnDollyFinished += DollyDrive_OnDollyFinished;
        CollisionHandlerNew.OnPlayerCollision += CollisionHandlerNew_OnPlayerCollision;
    }

    private void OnDisable()
    {
        PlayerControlHandler.OnLRValueChange -= LeftRightValueSetter;
        SwipeInput.OnTouchValueChange -= LeftRightValueSetter;
        DollyDrive.OnDollyFinished -= DollyDrive_OnDollyFinished;
        CollisionHandlerNew.OnPlayerCollision -= CollisionHandlerNew_OnPlayerCollision;
    }

    private void Start()
    {
        inputEnabled = true;
    }

    private void LeftRightValueSetter(float inputValues)
    {
        movementValues = inputValues;
    }
    
    void Update()
    {
        RotateWheels();
        MovePlayer();
        TiltPlayer();

        float targetMultiplier = maxSpeedMultiplier;
        playerSpeedMultiplier = Mathf.MoveTowards(playerSpeedMultiplier, targetMultiplier, accelerationRate * Time.deltaTime);

        playerSpeed = playerStartSpeed * playerSpeedMultiplier;
    }

    private void RotateWheels()
    {
        if (wheels == null) return;

        //float spinRate = Mathf.Lerp(0f, maxSpinSpeed, spline.MaxSpeed / environmentSpeed);
        float deltaAngle = maxSpinSpeed * Time.deltaTime;
        foreach (var w in wheels)
        {
            w.Rotate(-deltaAngle, 0f, 0f, Space.Self);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("NextSeg"))
        {
            envManager.AdvanceSegment();
        }
    }

    private void MovePlayer()
    {
        if(!inputEnabled) movementValues = 0;

        Vector3 movementForce = new Vector3(
            movementValues * movementMultiplier * Time.deltaTime, 0, autoForce*Time.deltaTime
            );
        
        transform.Translate(movementForce, Space.World);
        //p_Body.AddForce(movementForce, ForceMode.Force);
    }

    private void TiltPlayer()
    {
        currentTilt = Mathf.Lerp(currentTilt, movementValues, tiltSpeed * Time.deltaTime);

        bikeAnimator.SetFloat("Tilt", currentTilt);
        riderAnimator.SetFloat("Tilt", currentTilt);
    }


    private void CollisionHandlerNew_OnPlayerCollision()
    {
        inputEnabled = false;
    }

    private void DollyDrive_OnDollyFinished()
    {
        autoForce = 0;
    }
}
