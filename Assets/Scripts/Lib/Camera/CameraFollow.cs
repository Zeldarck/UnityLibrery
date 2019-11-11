using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : CameraStrategy
{
    Vector3 m_orientation;

    float m_speed;

    Vector3 m_offset;



    public CameraFollow(Vector3 a_offset, Vector3 a_orientation, float a_speed)
    {
        m_orientation = a_orientation;
        m_offset = a_offset;
        m_speed = a_speed;
    }


    public override void Update(Transform a_transform)
    {
        if (m_target)
        {
            a_transform.position = m_target.transform.position + m_offset;
            a_transform.localRotation = Quaternion.Euler(m_orientation);

            if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
            {
                m_offset += a_transform.forward * m_speed;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
            {
                m_offset -= a_transform.forward * m_speed;
            }

        }

    }

}

