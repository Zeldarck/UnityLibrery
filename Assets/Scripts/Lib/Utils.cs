using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif  //UNITY_EDITOR


public static class Utils
{

    static System.Random m_random = new System.Random();

    public static void DestroyChilds(Transform a_transform)
    {
        for (int i = a_transform.childCount - 1; i >= 0; --i)
        {
            GameObject.Destroy(a_transform.GetChild(i).gameObject);
        }
    }


    public static void RandomizeChildren(Transform a_transform, System.Random r = null)
    {
        if (r == null)
        {
            r = m_random;
        }

        for (int i = 0; i < a_transform.childCount; i++)
        {
            int randomIndex = r.Next(0, a_transform.childCount);
            a_transform.GetChild(i).SetSiblingIndex(randomIndex);
        }
    }



    public static bool RandomBool( int probability, System.Random r = null)
    {
        if (r == null)
        {
            r = m_random;
        }

        int v = r.Next(0, 100);
        bool result = v < probability;
        return result;
    }

    public static float RandomFloat(float min, float max, System.Random r = null)
    {
        if(r == null)
        {
            r = m_random;
        }
        return (float)r.NextDouble() * (max - min) + min;
    }


    public static int RandomInt(int min, int max, System.Random r = null)
    {
        if (r == null)
        {
            r = m_random;
        }
        return r.Next(min, max);
    }


    public static int SignWithZero(float a_value, float a_epsilon = 0)
    {
        return a_value >= -a_epsilon && a_value <= a_epsilon ? 0 : (int)Mathf.Sign(a_value);
    }

    public static float RandomGaussianDouble(float a_mean, float a_stdDev, System.Random r = null)
    {
        if (r == null)
        {
            r = m_random;
        }


        float u, v, S;
        
        do
        {
            u = 2.0f * (float)r.NextDouble() - 1.0f;
            v = 2.0f * (float)r.NextDouble() - 1.0f;
            S = u * u + v * v;
        } while (S >= 1.0f || S == 0f);

        float fac = Mathf.Sqrt((-2.0f * Mathf.Log(S) / S));
        return a_mean + a_stdDev * u * fac;
    }

    public static Vector2 ClampVector(Vector2 a_vector, Vector2 a_vectorMin, Vector2 a_vectorMax)
    {
        a_vector.Normalize();
        a_vectorMin.Normalize();
        a_vectorMax.Normalize();

        Vector2 res = a_vector;
        float current = (Vector2.SignedAngle(new Vector2(1, 0), a_vector) + 360) % 360;
        float negCurrent = current - 360;
        float min = ((Vector2.SignedAngle(new Vector2(1, 0), a_vectorMin) + 360) % 360) - 360;
        float max = (Vector2.SignedAngle(new Vector2(1, 0), a_vectorMax) + 360) % 360;

        if (current > max && negCurrent < min)
        {
            if (current - max < Mathf.Abs(negCurrent - min))
            {
                res = a_vectorMax;
                Debug.Log("Get Max cur " + current + " neg " + negCurrent + " min " + min + " max " + max);

            }
            else

            {

                res = a_vectorMin;
                Debug.Log("Get Min");

            }
        }


        return res;

        //Seem to work only with 0 - 180 ?
        //return Vector2.Min(a_vectorMax, Vector2.Max(a_vectorMin, a_vector));

    }

    public static Vector2 ClampVector(Vector2 a_vector, float a_degreMin, float a_degreMax)
    {
        return ClampVector(a_vector, new Vector2(Mathf.Cos(Mathf.Deg2Rad * a_degreMin), Mathf.Sin(Mathf.Deg2Rad * a_degreMin)), new Vector2(Mathf.Cos(Mathf.Deg2Rad * a_degreMax), Mathf.Sin(Mathf.Deg2Rad * a_degreMax)));
    }

    public static bool IsColorBright(Color color)
    {
        double a = 1 - (0.299 * color.r + 0.587 * color.g + 0.114 * color.b);

        return a < 0.5;
    }



    public static void TriggerNextFrame(Action a_callback)
    {
        MonoBehaviourSingleton.Instance.StartCoroutine(TriggerNextFrameCoroutine(a_callback));
    }

    static IEnumerator TriggerNextFrameCoroutine(Action a_callback)
    {
        yield return new WaitForEndOfFrame();
        a_callback();
    }


    public static void TriggerWaitForSeconds(float a_seconds, Action a_callback)
    {
        MonoBehaviourSingleton.Instance.StartCoroutine(TriggerWaitForSecondsCoroutine(a_seconds, a_callback));
    }

    static IEnumerator TriggerWaitForSecondsCoroutine(float a_seconds, Action a_callback)
    {
        yield return new WaitForSeconds(a_seconds);
        a_callback();
    }


    public static Color ParseColor(string a_hexcolor)
    {
        if (a_hexcolor.StartsWith("#"))
        {
            a_hexcolor = a_hexcolor.Substring(1);
        }

        if (a_hexcolor.StartsWith("0x"))
        {
            a_hexcolor = a_hexcolor.Substring(2);
        }

        if (a_hexcolor.Length != 8)
        {
            throw new Exception(string.Format("{0} is not a valid color string.", a_hexcolor));
        }

        byte r = byte.Parse(a_hexcolor.Substring(0, 2), NumberStyles.HexNumber);
        byte g = byte.Parse(a_hexcolor.Substring(2, 2), NumberStyles.HexNumber);
        byte b = byte.Parse(a_hexcolor.Substring(4, 2), NumberStyles.HexNumber);
        byte a = byte.Parse(a_hexcolor.Substring(6, 2), NumberStyles.HexNumber);

        return new Color32(r, g, b, a);
    }


    public static bool IsTapping(Touch a_touch, int m_minTap = 1)
    {
        return a_touch.tapCount > m_minTap && a_touch.phase == TouchPhase.Began;
    }


    public static void QuitApp()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif //UNITY_EDITOR
    }

    public static bool IsSameSign(int a_number1, int a_number2)
    {
        return (a_number1 ^ a_number2) >= 0;
    }

    public static bool IsSameSign(float a_number1, float a_number2)
    {
        return IsSameSign((int)Mathf.Floor(a_number1), (int)Mathf.Floor(a_number2));
    }
}
