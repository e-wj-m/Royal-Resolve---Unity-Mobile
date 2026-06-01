using UnityEngine;

public class FloatAndRotate : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] private float rotateSpeed = 90f;
    [SerializeField] private Vector3 rotateAxis = Vector3.up;

    [Header("Bob")]
    [SerializeField] private float bobHeight = 0.2f;
    [SerializeField] private float bobSpeed = 1f;

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        // Rotate
        transform.Rotate(rotateAxis, rotateSpeed * Time.deltaTime);

        // Bob
        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}