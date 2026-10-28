using UnityEngine;

public class BorderDots : MonoBehaviour
{
    public RectTransform dotPrefab;
    public int dotCount;
    public float speed;
    private RectTransform border;
    private RectTransform[] dots;
    private float distanceTravelled = 0f;
 
    void Start()
    {
        border = GetComponent<RectTransform>();
        dots = new RectTransform[dotCount];
        for (int i = 0; i < dotCount; i++)
        {
            dots[i] = Instantiate(dotPrefab, border);
        }
    }
    void Update()
    {
        float width = border.rect.width;
        float height = border.rect.height;
        float perimeter = 2 * (width + height);
        float gap = perimeter / dotCount;
        distanceTravelled = (distanceTravelled + speed * Time.deltaTime) % perimeter;
 
        for (int i = 0; i < dotCount; i++)
        {
            float distance = (distanceTravelled + i * gap) % perimeter;
            dots[i].anchoredPosition = PointOnBorder(distance, width, height);
        }
    }
    Vector2 PointOnBorder(float distance, float width, float height)
    {
        float left = -width / 2;
        float right = width / 2;
        float top = height / 2;
        float bottom = -height / 2;

        if (distance < width)
        {
            return new Vector2(left + distance, top); 
        }
        distance -= width;

        if (distance < height)
        {
            return new Vector2(right, top - distance); 
        }
        distance -= height;

        if (distance < width)
        {
            return new Vector2(right - distance, bottom); 
        }
        distance -= width;
        return new Vector2(left, bottom + distance); 
    }
}
