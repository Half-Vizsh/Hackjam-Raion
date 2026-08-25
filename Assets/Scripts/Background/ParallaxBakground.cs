using UnityEngine;
using UnityEngine.UIElements;

public class ParallaxBakground : MonoBehaviour
{
   [SerializeField] private GameObject _cam;
   [SerializeField] private float _parallaxSpeed;
   private float _startPos, _length;
    void Start()
    {
        _startPos = transform.position.x;
        _length = GetComponent<SpriteRenderer>().bounds.size.x;
    }
    void FixedUpdate()
    {
        float distance = _cam.transform.position.x * _parallaxSpeed;
        float movement = _cam.transform.position.x * (1-_parallaxSpeed);
        
        transform.position = new Vector3(_startPos+distance, transform.position.y, transform.position.z);

        if (movement > _startPos + _length)
        {
            _startPos += _length;
        } else if (movement < _startPos - _length)
        {
            _startPos -= _length;
        }
    }
}
