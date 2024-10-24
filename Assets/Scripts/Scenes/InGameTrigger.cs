using UnityEngine;

public class InGameTrigger : MonoBehaviour
{
    [SerializeField] private InGame _ingame;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            _ingame.StartGame();
            this.enabled = false;
        }
    }
}
