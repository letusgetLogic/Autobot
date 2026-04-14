using Unity.Mathematics;
using UnityEngine;

public class Rotate : MonoBehaviour
{

    [Header("Settings")]
    [SerializeField] private float degreesPerSecond;

    private quaternion defaultRotation;

    private void OnEnable()
    {
        defaultRotation = transform.rotation;
    }

    private void OnDisable()
    {
        transform.rotation = defaultRotation;
    }

    private void FixedUpdate()
    {
        transform.Rotate(0f, 0f, -degreesPerSecond * Time.fixedDeltaTime);
    }
}
