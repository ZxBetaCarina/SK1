using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] Player player;
    public GameObject easy_prefab;
    public GameObject medium_prefab;
    public GameObject hard_prefab;
    public float spawnRate = 1f;
    public float minHeight = -1f;
    public float maxHeight = 2f;


    private void OnEnable()
    {
        InvokeRepeating(nameof(Spawn), spawnRate, spawnRate);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(Spawn));
    }

    private void Spawn()
    {
        GameObject pipes;
        if (GameManager.Instance._difficulty_state == 0)
        {
            pipes = Instantiate(easy_prefab, transform.position, Quaternion.identity);
        }
        else if(GameManager.Instance._difficulty_state == 1)
        {
           pipes = Instantiate(medium_prefab, transform.position, Quaternion.identity);
        }
        else
        {
            pipes = Instantiate(hard_prefab, transform.position, Quaternion.identity);
        }
        pipes.transform.position += Vector3.up * Random.Range(minHeight, maxHeight);
        pipes.GetComponent<Pipes>().Init(player);
    }

}
