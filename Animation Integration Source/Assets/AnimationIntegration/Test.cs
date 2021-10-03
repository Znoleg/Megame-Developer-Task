using UnityEngine;

public class Test : MonoBehaviour
{
    public Transform targetBone;
    public Vector3 rot;

    private void Start()
    {
        rot = targetBone.localEulerAngles;
    }

    public void LateUpdate()
    {
        targetBone.transform.Rotate(new Vector3(0f, 2.5f,0f), Space.World);
        //targetBone.RotateAround(new Vector3(0.0f, 0f, 0f), Vector3.up, 5f);
    }
}
