    using UnityEngine;
namespace Eloi.TBIO
{

        public class NoGravityBasicTelloDroneMono : MonoBehaviour
        {
            public Transform m_whatToAffect;

            [Header("Percent Input")]
            [Range(-1, 1)]
            public float m_rotateLeftRightPercent;

            [Range(-1, 1)]
            public float m_moveDownUpPercent;

            [Range(-1, 1)]
            public float m_moveLeftRightPercent;

            [Range(-1, 1)]
            public float m_moveBackFrontPercent;


            [Header("Speed and Angle")]
            public float m_rotateLeftRightAngle = 180;
            public float m_movingSpeed = 0.5f;


            public float m_leftRightDirectionAngle=0;

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
                if (value == 0)
                    return 0;
                else
                    return ((value / 100f) - 0.5f) * 2f;

            }

            public void SetWith1899887766(string value)
            {
                if (int.TryParse(value, out int result))
                {
                    SetWith1899887766(result);
                }
            }

            public void SetWith1899887766(int value)
            {
                if (value >= 1800000000 && value < 1900000000)
                {
                    value = value % 100000000;
                    float valueLeftX = Convert99ToPercent(value  / 1000000 % 100);
                    float valueLeftY = Convert99ToPercent(value  / 10000   % 100);
                    float valueRightX = Convert99ToPercent(value / 100     % 100);
                    float valueRightY = Convert99ToPercent(value           % 100);
                    SetDoubleJoystick(new Vector2(valueLeftX, valueLeftY), new Vector2(valueRightX, valueRightY));
                }
            }

            public void SetDoubleJoystick(Vector2 leftJoystick, Vector2 rightJoystick)
            {



                m_rotateLeftRightPercent = leftJoystick.x;
                m_moveDownUpPercent = leftJoystick.y;
                m_moveLeftRightPercent = rightJoystick.x;
                m_moveBackFrontPercent = rightJoystick.y;
            }
            public void SetRotateLeftRight(float percent)
            {
                m_rotateLeftRightPercent = percent;
            }
            public void SetMoveLeftRight(float percent)
            {
                m_moveLeftRightPercent = percent;
            }
            public void SetMoveDownUp(float percent)
            {
                m_moveDownUpPercent = percent;
            }
            public void SetMoveBackFront(float percent)
            {
                m_moveBackFrontPercent = percent;
            }

            private void Reset()
            {
                m_whatToAffect = transform;
            }

            void LateUpdate()
            {

                if (this.enabled == false)
                    return;

                float deltaAngle = m_rotateLeftRightAngle * m_rotateLeftRightPercent * Time.deltaTime;
                m_leftRightDirectionAngle+= deltaAngle;
                m_whatToAffect.localRotation = Quaternion.Euler(0, m_leftRightDirectionAngle, 0);
                float dt = Time.deltaTime;
                m_whatToAffect.Translate(Vector3.forward * m_moveBackFrontPercent * dt, Space.Self);
                m_whatToAffect.Translate(Vector3.right * m_moveLeftRightPercent * dt, Space.Self);
                m_whatToAffect.Translate(Vector3.up * m_moveDownUpPercent * dt, Space.Self);

            }
        }
    }
