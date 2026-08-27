using UnityEngine;

public class TitleBackground : MonoBehaviour
{
    [System.Serializable]
    public class ScrollLayer
    {
        public string layerName;
        public Transform[] segments; // minimal 2, disusun berdampingan
        public float scrollSpeed; // unit per detik, boleh beda tiap layer
        [HideInInspector] public float segmentWidth;
    }

    [SerializeField] private ScrollLayer[] layers = new ScrollLayer[5];

    private void Start()
    {
        foreach (var layer in layers)
        {
            if (layer.segments == null || layer.segments.Length < 2)
            {
                Debug.LogWarning($"Layer '{layer.layerName}' butuh minimal 2 segment buat looping!");
                continue;
            }

            SpriteRenderer sr = layer.segments[0].GetComponent<SpriteRenderer>();
            layer.segmentWidth = sr.bounds.size.x;
        }
    }

    private void Update()
    {
        foreach (var layer in layers)
        {
            if (layer.segments == null || layer.segments.Length < 2) continue;

            foreach (var segment in layer.segments)
                segment.position += Vector3.left * layer.scrollSpeed * Time.deltaTime;

            RepositionSegments(layer);
        }
    }

    private void RepositionSegments(ScrollLayer layer)
    {
        float totalWidth = layer.segmentWidth * layer.segments.Length;

        foreach (var segment in layer.segments)
        {
            // kalau segment sudah sepenuhnya keluar di sisi kiri, pindahkan ke paling kanan
            if (segment.position.x <= -layer.segmentWidth)
            {
                segment.position += Vector3.right * totalWidth;
            }
        }
    }
}