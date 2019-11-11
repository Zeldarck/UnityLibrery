using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{

    protected Rigidbody m_rigidbody;

    public bool Freeze { get; set; }

    protected Vector3 m_movementDirection;


    [SerializeField]
    protected float m_speed;


    // Start is called before the first frame update
    protected virtual void  Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }


    void FixedUpdate()
    {
        if (!Freeze)
        {
            Move();
        }
    }



    protected virtual void Move()
    {

        Vector3 displacement = m_movementDirection.normalized * m_speed * Time.deltaTime;

        m_rigidbody.MovePosition(transform.position + displacement);

        Turning();

    }

    void Turning()
    {
        if (m_movementDirection != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(m_movementDirection.normalized, Vector3.up);
            m_rigidbody.MoveRotation(rotation);
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
