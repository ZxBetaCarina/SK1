using UnityEngine;

public class Pipes : MonoBehaviour
{
    private Player _player;
    public Transform top;
    public Transform bottom;
    public float speed = 5f;

    private float leftEdge;

    private void Start()
    {
        leftEdge = Camera.main.ScreenToWorldPoint(Vector3.zero).x - 1f;
    }

    public void Init(Player player)
    {
        _player = player;
    }

    private void Update()
    {
        if (_player.GetPlayerState == PlayerState.Paused || _player == null)
        {
            return;
        }

        transform.position += speed * Time.deltaTime * Vector3.left;

        if (transform.position.x < leftEdge)
        {
            Destroy(gameObject);
        }
    }

}
