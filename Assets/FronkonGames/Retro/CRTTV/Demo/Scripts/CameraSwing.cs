using UnityEngine;

namespace FronkonGames.Retro.CRTTV
{
  /// <summary> Camera swing. </summary>
  /// <remarks> This code is designed for a simple demo, not for production environments. </remarks>
  [RequireComponent(typeof(Camera))]
  public sealed class CameraSwing : MonoBehaviour
  {
    [SerializeField]
    private Transform target;

    [SerializeField]
    private Vector3 lookAtOffset;

    [SerializeField]
    private Vector3 swingStrength;

    [SerializeField]
    private Vector3 swingVelocity;
    
    private new Camera camera;

    private Vector3 originalPosition;

    private void Awake()
    {
      camera = GetComponent<Camera>();
      originalPosition = camera.transform.position;
    }

    private void Update()
    {
      Vector3 position = originalPosition;
      position.x += Mathf.Sin(Time.time * swingVelocity.x) * swingStrength.x;
      position.y += Mathf.Cos(Time.time * swingVelocity.y) * swingStrength.y;
      position.z += Mathf.Sin(Time.time * swingVelocity.z) * swingStrength.z;
      
      camera.transform.position = position;

      camera.transform.LookAt(target.position + lookAtOffset);
    }
  }  
}
