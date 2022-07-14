using ThirdParties.Truongtv.SoundManager;
using Truongtv.Utilities;
using UnityEngine;

public class ObjectSound : MonoBehaviour
{
    public LayerMask platformLayer, waterLayer;
    public AudioClip collideRaft;
    public AudioClip dropOnWater;
    private bool playedSfx;
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (Extended.IsInLayerMask(other.gameObject, platformLayer))
        {
            if (!playedSfx && collideRaft != null)
            {
                SoundManager.Instance.PlaySfx(collideRaft);
                playedSfx = true;
            }
        }
        else if(Extended.IsInLayerMask(other.gameObject, waterLayer))
        {
            if (!playedSfx && dropOnWater != null)
            {
                SoundManager.Instance.PlaySfx(dropOnWater);
                playedSfx = true;
            }
        }
    }
}
