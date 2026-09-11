using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Volume))]
public class PlatformVolumeProfile : MonoBehaviour
{
    [SerializeField] private VolumeProfile mobileProfile;

    private void Awake()
    {
        if (!Application.isMobilePlatform || mobileProfile == null) return;

        GetComponent<Volume>().sharedProfile = mobileProfile;
    }
}
