using UnityEngine;

public class CherryController : MonoBehaviour
{
    public GameObject cherryPrefab;
    public float spawnDelay = 5f;
    public float speed = 4f;
    public Vector3 levelCenter = new Vector3(13.5f, -14f, 0f);
    public float spawnRadius = 21f;
    private GameObject cherry;
    private Tweener cherryTweener;
    private float timer = 0f;
 
    void Update()
    {
        if (cherry ==null)
        {
            timer += Time.deltaTime;
            if (timer >= spawnDelay)
            {
                SpawnCherry();
                timer = 0f;
            }
        }
        else if (!cherryTweener.IsTweening())
        {
            Destroy(cherry);
        }
    }
 
    void SpawnCherry()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Vector3 direction = new Vector3(randomDirection.x, randomDirection.y, 0f);
 
        Vector3 startPos = levelCenter + direction * spawnRadius;   
        Vector3 endPos = levelCenter - direction * spawnRadius;    
 
        cherry = Instantiate(cherryPrefab, startPos, Quaternion.identity);
        cherryTweener = cherry.GetComponent<Tweener>();
        
        float duration = (spawnRadius * 2f) / speed;
        cherryTweener.AddTween(cherry.transform, startPos, endPos, duration);
    }
}
