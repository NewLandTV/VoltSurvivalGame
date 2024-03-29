using System.Collections;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private float angle;

    private float lerpTime;

    private IEnumerator Start()
    {
        while (true)
        {
            lerpTime += Time.deltaTime * speed;
            transform.rotation = CalculateMovementOfPendulum();

            yield return null;
        }
    }

    private Quaternion CalculateMovementOfPendulum()
    {
        return Quaternion.Lerp(Quaternion.Euler(Vector3.forward * angle), Quaternion.Euler(Vector3.back * angle), GetLerpTParam());
    }

    private float GetLerpTParam()
    {
        return (Mathf.Sin(lerpTime) + 1) * 0.5f;
    }
}
