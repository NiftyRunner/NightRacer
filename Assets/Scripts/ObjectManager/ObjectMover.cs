using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    [SerializeField] private float setAutoForce = 10f;

    private float autoForce;
    private static bool movementEnabled = false;

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
        autoForce = setAutoForce;
    }

    void Update()
    {
        if(!movementEnabled) return;

        MoveEnvironment();   
    }

    private void MoveEnvironment()
    {
        Vector3 movementValues = new Vector3(0, 0, -autoForce * Time.deltaTime);
        transform.Translate(movementValues, Space.World);
    }


    private void DollyDrive_OnDollyFinished()
    {
        movementEnabled = true;
    }
}
