using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D m_RigidBody;

    Vector3 m_Dir;

    [Header("Movement data")]
    public float m_MoveSpeed = 1.0f;

    [Header("Visual")]
    public Animator m_Animator;

    // Start is called before the first frame update
    void Awake()
    {
        m_Dir = Vector3.zero;
        m_RigidBody = GetComponent<Rigidbody2D>();
    }

    public void Update()
    {
        //cant move diagonally
        m_Dir = Vector3.zero;
        if (Input.GetButton("Vertical"))
        {
            m_Dir = new Vector3(0.0f, Input.GetAxis("Vertical"));
        }
        else if (Input.GetButton("Horizontal"))
        {
            m_Dir = new Vector3(Input.GetAxis("Horizontal"), 0.0f);
        }

        UpdateAnimation();
    }

    public void UpdateAnimation()
    {
        if (m_Animator != null)
        {
            m_Animator.SetFloat("Horizontal", m_Dir.x);
            m_Animator.SetFloat("Vertical", m_Dir.y);
        }
    }

    //for affecting rigidbody
    private void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        m_Dir.Normalize();
        m_RigidBody.MovePosition(transform.position + m_Dir * m_MoveSpeed * Time.fixedDeltaTime);
    }

    public void Pushing(bool pushing)
    {
        //start animation
        if (m_Animator != null)
            m_Animator.SetBool("Pushing", pushing);
    }
}
