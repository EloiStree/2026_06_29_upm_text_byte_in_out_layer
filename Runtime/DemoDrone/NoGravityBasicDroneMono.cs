using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eloi.TBIO
{


    public class NoGravityBasicFpvDroneMono : MonoBehaviour
    {
        public Transform m_whatToAffect;

        [Header("Percent Input")]
        [Range(-1, 1)]
        public float m_rotateLeftRightPercent;

        [Range(-1, 1)]
        public float m_pitchBackForwardPercent;

        [Range(-1, 1)]
        public float m_rollLeftRightPercent;

        [Range(-1, 1)]
        public float m_throttleFrontPercent;


        [Header("Speed and Angle")]
        public float m_rotateLeftRightAngle = 180;
        public float m_pitchBackForwardAngle = 180;
        public float m_rollLeftRightAngle = 180;
        public float m_throttleFrontSpeed = 0.5f;
        public float m_throttleBackSpeed = 0.1f;


        public void SetWith1899887766(byte[] valueLittleEndian)
        {
            if (valueLittleEndian.Length == 4)
            {
                int value = System.BitConverter.ToInt32(valueLittleEndian, 0);
                SetWith1899887766(value);
            }
        }

        public float Convert99ToPercent(int value)
        {
            if (value==0) 
                return 0;
            else 
                return ( (value / 100f) - 0.5f ) * 2f ;

        }

        public void SetWith1899887766(string value)
        {
            if (int.TryParse(value, out int result))
            {
                SetWith1899887766(result);
            }
        }

        public void SetWith1899887766(int value) {
            if (value >= 1800000000 && value < 1900000000)
            {
                value = value % 100000000;
                float valueLeftX = Convert99ToPercent(value / 1000000 % 100);
                float valueLeftY = Convert99ToPercent(value / 10000 % 100);
                float valueRightX = Convert99ToPercent(value / 100 % 100);
                float valueRightY = Convert99ToPercent(value % 100);
                SetDoubleJoystick(new Vector2(valueLeftX, valueLeftY), new Vector2(valueRightX, valueRightY));
            }
        }

        public void SetDoubleJoystick(Vector2 leftJoystick, Vector2 rightJoystick)
        {
            m_rotateLeftRightPercent = leftJoystick.x;
            m_throttleFrontPercent = leftJoystick.y;
            m_pitchBackForwardPercent = rightJoystick.y;
            m_rollLeftRightPercent = rightJoystick.x;
        }
        public void SetRotateLeftRight(float percent)
        {
            m_rotateLeftRightPercent = percent;
        }
        public void SetPitchBackForward(float percent)
        {
            m_pitchBackForwardPercent = percent;
        }
        public void SetRollLeftRight(float percent)
        {
            m_rollLeftRightPercent = percent;
        }
        public void SetThrottleFront(float percent)
        {
            m_throttleFrontPercent = percent;
        }

        private void Reset()
        {
            m_whatToAffect = transform;
        }

        void LateUpdate()
        {
            if (this.enabled == false)
                return;

            float throttleSpeed = m_throttleFrontPercent > 0 ? m_throttleFrontSpeed : m_throttleBackSpeed;
            float speed = m_throttleFrontPercent * throttleSpeed;
                m_whatToAffect.Rotate(Vector3.up , m_rotateLeftRightAngle * m_rotateLeftRightPercent * Time.deltaTime, Space.Self);
                m_whatToAffect.Rotate(Vector3.right, m_pitchBackForwardAngle * m_pitchBackForwardPercent * Time.deltaTime, Space.Self);
                m_whatToAffect.Rotate(Vector3.forward, -m_rollLeftRightAngle * m_rollLeftRightPercent * Time.deltaTime, Space.Self);
                m_whatToAffect.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
           
        }

    }
}

