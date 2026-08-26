using UnityEngine;

public class DoorPuzzle : MonoBehaviour
{
    #region  Variables
    [Header("Moving")]
    [SerializeField]private float speed;    
    private float t = 0f;
    private int dir = 1;
    [SerializeField] private bool _doorOpen = false;
    [SerializeField] private Transform OpenPoint;
    [SerializeField] private Transform ClosedPoint;
    [SerializeField ]private Transform DoorObject;
    #endregion
    #region Unity Methods
     private void Update()
    {
        Moving();
    }
    #endregion
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Treasure")) {
            Debug.Log("Buka pintu");
            _doorOpen = true;
        } 
    }
    #region Private Methods
    private void Moving()
    {
       if (!_doorOpen) return;
        DoorObject.position = Vector3.MoveTowards(
            DoorObject.position,
            OpenPoint.position,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(DoorObject.position, OpenPoint.position) <= 0.01f)
        {
            DoorObject.position = OpenPoint.position;
            _doorOpen = false;
        }
    }
    #endregion
}
