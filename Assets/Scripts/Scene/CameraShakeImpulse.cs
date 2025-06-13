using UnityEngine;
using Cinemachine;

public class CameraShakeImpulse : MonoBehaviour
{
    public CinemachineImpulseSource impulseSource;

    private void Start()
    {
        // Si no se asignó en el inspector, intenta obtener el componente del objeto actual
        if (impulseSource == null)
        {
            impulseSource = GetComponent<CinemachineImpulseSource>();
        }
    }

    public void GenerateImpulse()
    {
        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse();
        }
        else
        {
            Debug.LogError("Impulse Source no asignado en CameraShakeImpulse");
        }
    }
}
