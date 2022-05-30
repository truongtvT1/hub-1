using UnityEngine;

public class UpdateZByY : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x,transform.position.y,transform.position.y);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            Update();
        }
    }
}
