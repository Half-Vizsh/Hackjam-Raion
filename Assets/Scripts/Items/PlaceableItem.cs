using UnityEngine;

public class PlaceableItem : MonoBehaviour
{
    [SerializeField] private Collider2D _placeableCollider;
    public Collider2D GetCollider()
    {
        return this._placeableCollider;
    }
}
