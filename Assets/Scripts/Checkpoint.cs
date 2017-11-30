using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.GetComponent<PlayerTag>() != null)
        {
            Destroy(this.gameObject);
        }
    }
}
