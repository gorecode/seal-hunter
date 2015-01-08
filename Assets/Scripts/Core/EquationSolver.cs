using UnityEngine;
using System.Collections;

public class EquationSolver {
    public delegate float EquationFunc(float x);

    public static float Dihotomy(EquationFunc F, float targetValue, float min, float max, float e = 0.001f)
    {
        float length = max - min;
        float err = length;
        float x = (min + max) / 2f;

        while (err > e && F(x) - targetValue != 0)
        {
            x = (min + max) / 2f;

            //Debug.Log("x = " + x + " f(x) = " + F(x) + " min = " + min + " f(min) = " + F(min) + " max = " + max + " F(max) = " + F(max)) ;

            if ((F(x) - targetValue) * (F(min) - targetValue) <= 0) max = x;
            else if ((F(x) - targetValue) * (F(max) - targetValue) <= 0) min = x;
            err = (max - min) / length;
        }

        return x;
    }

    private EquationSolver() { }
}
