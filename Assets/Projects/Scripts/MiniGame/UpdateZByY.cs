using UnityEngine;

public class UpdateZByY : MonoBehaviour
{
    public bool localPostion, zeroZ;
    // Update is called once per frame
    void Update()
    {
        if (localPostion)
        {
            transform.localPosition = new Vector3(transform.localPosition.x,transform.localPosition.y,zeroZ ? 0 : transform.localPosition.z);
            return;
        }
        transform.position = new Vector3(transform.position.x,transform.position.y,zeroZ ? 0 : transform.position.y);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            Update();
        }
    }
}
