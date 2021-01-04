using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D m_RigidBody;

    [Header("Movement data")]
    public float m_MoveSpeed = 1.0f;

    // Start is called before the first frame update
    void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody2D>();
    }

    //for affecting rigidbody
    private void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        //cant move diagonally
        Vector3 dir = Vector3.zero;
        if (Input.GetButton("Vertical"))
        {
            dir = new Vector3(0.0f, 1.0f);
        }
        else if (Input.GetButton("Horizontal"))
        {
            dir = new Vector3(1.0f, 0.0f);
        }

        dir.Normalize();
        m_RigidBody.MovePosition(transform.position + dir * m_MoveSpeed * Time.fixedDeltaTime);
    }
}
