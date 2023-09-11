using UnityEngine;

public class FlameScript : MonoBehaviour
{
    public float speed;
    private Rigidbody2D _rb;
    private Vector2 _moveDirection;
    private PlayerController _target;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _target = PlayerController.instance;

        _moveDirection = (_target.transform.position - transform.position).normalized * speed;
        _rb.velocity = new Vector2(_moveDirection.x, _moveDirection.y);
    }
}
