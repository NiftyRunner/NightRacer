using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    [SerializeField] private float setAutoForce = 10f;

    [Header("Wheel Settings")]
    [SerializeField] private Transform[] wheels;
    [SerializeField] private float wheelSpeed = 360f;

    [SerializeField]
    private float autoForce;
    private bool isObstacle = false;
    private static bool movementEnabled = false;

    public void SetAutoForce(float force) => autoForce = force;

    private void OnEnable()
    {
        DollyDrive.OnDollyFinished += DollyDrive_OnDollyFinished;
    }

    private void OnDisable()
    {
        DollyDrive.OnDollyFinished -= DollyDrive_OnDollyFinished;
    }

    private void Start()
    {
        if(this.gameObject.CompareTag("Obstacle"))
        {
            isObstacle = true;
            return;
        }

        autoForce = setAutoForce;
    }

    void Update()
    {
        if(!movementEnabled) return;

        if (isObstacle) { RotateWheels(); }

        MoveEnvironment();   
    }

    private void MoveEnvironment()
    {
        Vector3 movementValues = new Vector3(0, 0, -autoForce * Time.deltaTime);
        transform.Translate(movementValues, Space.World);
    }

    private void RotateWheels()
    {
        if (wheels == null) return;

        //float spinRate = Mathf.Lerp(0f, maxSpinSpeed, spline.MaxSpeed / environmentSpeed);
        float deltaAngle = wheelSpeed * Time.deltaTime;
        foreach (var w in wheels)
        {
            w.Rotate(deltaAngle, 0f, 0f, Space.Self);
        }
    }

    private void DollyDrive_OnDollyFinished()
    {
        movementEnabled = true;
    }
}
