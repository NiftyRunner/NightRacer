using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private EnvironmentManager envManager;
    [SerializeField] private Transform[] wheels;

    [SerializeField] private float movementMultiplier = 1f;
    [SerializeField] private float autoForce = 1f;
    [SerializeField] private float maxSpinSpeed = 360f;
    [SerializeField] private float accelerationRate = 0.2f; // how fast speed ramps up
    [SerializeField] private float maxSpeedMultiplier = 3f;
    [SerializeField] private float maxLateralOffset = 5.5f; // clamps X so the player can't drive into the roadside walls

    [Header("Tilt Values")]
    [SerializeField] private Animator bikeAnimator;
    [SerializeField] private Animator riderAnimator;
    [SerializeField] private float tiltSpeed = 120f;

    private float movementValues;
    private float currentTilt;
    private Quaternion bikeRestWorldRotation;
    private Quaternion riderRestWorldRotation;

    private Rigidbody rb;

    private bool inputEnabled = false;

    [SerializeField] private float playerStartSpeed = 30f;
    [SerializeField] private float startSpeedMultiplier = 1.5f; // how strong the initial push feels before it ramps to maxSpeedMultiplier
    private float playerSpeed;
    private float playerSpeedMultiplier;

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
        playerSpeedMultiplier = startSpeedMultiplier;

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints |= RigidbodyConstraints.FreezePositionY;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        bikeRestWorldRotation = bikeAnimator.transform.rotation;
        riderRestWorldRotation = riderAnimator.transform.rotation;
        riderAnimator.enabled = false;

        IgnoreSelfCollisions();
    }

    // Now that movement is driven by real physics velocity, real overlaps generate real
    // collision response that fights the intended velocity. The rider's own ragdoll bone
    // colliders naturally overlap the bike (sitting on it), and the road segments' surface
    // colliders naturally overlap the player (Y is frozen, not resting cleanly on top) — both
    // are expected geometry overlap, not a real hit, so they must never physically collide
    // with the player. One-time setup: road segments are pooled/reused, not endlessly created.
    private void IgnoreSelfCollisions()
    {
        var riderColliders = riderAnimator.GetComponentsInChildren<Collider>(true);
        var riderColliderSet = new System.Collections.Generic.HashSet<Collider>(riderColliders);

        // Only the bike/character's own colliders — excludes the rider's own ragdoll bones,
        // which must stay able to collide with the bike and road once a crash triggers ragdoll
        // physics (RagdollControllerNew restores that collision when it activates).
        var bikeColliders = System.Array.FindAll(GetComponentsInChildren<Collider>(true), c => !riderColliderSet.Contains(c));

        foreach (var riderCollider in riderColliders)
        {
            foreach (var bikeCollider in bikeColliders)
            {
                Physics.IgnoreCollision(bikeCollider, riderCollider);
            }
        }

        foreach (var root in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (!root.CompareTag("NextSeg")) continue;

            // Only the "Roads" subgroup holds the solid road-surface colliders. The root's own
            // trigger collider is the segment-boundary trigger OnTriggerExit relies on to call
            // envManager.AdvanceSegment() — it must never be included here.
            var roadsGroup = root.transform.Find("Roads");
            if (roadsGroup == null) continue;

            foreach (var roadCollider in roadsGroup.GetComponentsInChildren<Collider>(true))
            {
                foreach (var bikeCollider in bikeColliders)
                {
                    Physics.IgnoreCollision(bikeCollider, roadCollider);
                }
            }
        }
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

    // rb is non-kinematic (required so OnCollisionEnter fires against the kinematic obstacle
    // rigidbodies), so it must be driven by setting velocity and letting the physics engine
    // integrate motion itself. Directly overwriting the Transform every frame (Translate or
    // MovePosition, tried both) fights the physics engine's own tracking of this body and is
    // what caused the persistent jitter — this is the actual fix, not another position hack.
    private void MovePlayer()
    {
        if (!inputEnabled) movementValues = 0;

        float lateralVelocity = movementValues * movementMultiplier;

        // Soft wall: block further travel past the clamp, but still allow moving back toward center.
        if (transform.position.x >= maxLateralOffset && lateralVelocity > 0) lateralVelocity = 0;
        if (transform.position.x <= -maxLateralOffset && lateralVelocity < 0) lateralVelocity = 0;

        rb.linearVelocity = new Vector3(lateralVelocity, 0, autoForce);
    }

    private void TiltPlayer()
    {
        currentTilt = Mathf.Lerp(currentTilt, movementValues, tiltSpeed * Time.deltaTime);

        bikeAnimator.SetFloat("Tilt", currentTilt);
    }

    // Runs after Mecanim has applied this frame's Tilt pose to the bike, so we can mirror
    // its exact world-space rotation delta onto the rider instead of guessing a local axis.
    private void LateUpdate()
    {
        Quaternion worldDelta = bikeAnimator.transform.rotation * Quaternion.Inverse(bikeRestWorldRotation);
        riderAnimator.transform.rotation = worldDelta * riderRestWorldRotation;
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
