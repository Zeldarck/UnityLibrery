using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : Npc
{

    [SerializeField]
    PlayerController m_player;

    [SerializeField]
    float m_distToPlayer = 1.75f;

    float m_baseSpeed;

    protected override void Awake()
    {
        base.Awake();

        m_baseSpeed = m_speed;
    }

    protected override void Move()
    {

        m_movementDirection = m_player.transform.position - transform.position;

        if(m_movementDirection.magnitude < m_distToPlayer)
        {
            m_speed = 0;
        }
        else
        {
            m_speed = m_baseSpeed * System.Math.Min(1, m_movementDirection.magnitude / 5.5f);
        }

        base.Move();
    }


}
