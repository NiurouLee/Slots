using System;
using UnityEngine;

/**********************************************

Copyright(c) 2021 by com.ustar
All right reserved

Author : Jian.Wang 
Date : 2020-09-18 11:51:13
Ver : 1.0.0
Description : 
ChangeLog :
**********************************************/

public class CubicBezier
{
    private static readonly int CUBIC_BEZIER_SPLINE_SAMPLES = 13;
    private static readonly int kMaxNewtonIterations = 4;
    private static readonly double kBezierEpsilon = 1e-7;

    public CubicBezier(double control1X, double control1Y, double control2X, double control2Y)
    {
        InitCoefficients(control1X, control1Y, control2X, control2Y);
        InitGradients(control1X, control1Y, control2X, control2Y);
        InitRange(control1Y, control2Y);
        InitSpline();
    }

    public double SampleCurveX(double t)
    {
        // `ax t^3 + bx t^2 + cx t' expanded using Horner's rule.
        return ((_ax * t + _bx) * t + _cx) * t;
    }

    public double SampleCurveY(double t)
    {
        return ((_ay * t + _by) * t + _cy) * t;
    }

    public double SampleCurveDerivativeX(double t)
    {
        return (3.0 * _ax * t + 2.0 * _bx) * t + _cx;
    }

    public double SampleCurveDerivativeY(double t)
    {
        return (3.0 * _ay * t + 2.0 * _by) * t + _cy;
    }

    public static double GetDefaultEpsilon()
    {
        return kBezierEpsilon;
    }

    // Given an x value, find a parametric value it came from.
    // x must be in [0, 1] range. Doesn't use gradients.
    public double SolveCurveX(double x, double epsilon)
    {
        double t0 = 0;
        double t1 = 0;
        double t2 = x;
        double x2 = 1;
        double d2 = 1;
        int i;
        // Linear interpolation of spline curve for initial guess.
        double deltaT = 1.0 / (CUBIC_BEZIER_SPLINE_SAMPLES - 1);
        for (i = 1; i < CUBIC_BEZIER_SPLINE_SAMPLES; i++)
        {
            if (x <= _splineSamples[i])
            {
                t1 = deltaT * i;
                t0 = t1 - deltaT;
                t2 = t0 + (t1 - t0) * (x - _splineSamples[i - 1]) /
                    (_splineSamples[i] - _splineSamples[i - 1]);
                break;
            }
        }

        // Perform a few iterations of Newton's method -- normally very fast.
        // See https://en.wikipedia.org/wiki/Newton%27s_method.
        double newtonEpsilon = Math.Min(kBezierEpsilon, epsilon);
        for (i = 0; i < kMaxNewtonIterations; i++)
        {
            x2 = SampleCurveX(t2) - x;
            if (Math.Abs(x2) < newtonEpsilon)
                return t2;
            d2 = SampleCurveDerivativeX(t2);
            if (Math.Abs(d2) < kBezierEpsilon)
                break;
            t2 = t2 - x2 / d2;
        }

        if (Math.Abs(x2) < epsilon)
            return t2;

        // Fall back to the bisection method for reliability.
        while (t0 < t1)
        {
            x2 = SampleCurveX(t2);
            if (Math.Abs(x2 - x) < epsilon)
                return t2;
            if (x > x2)
                t0 = t2;
            else
                t1 = t2;
            t2 = (t1 + t0) * .5;
        }

        // Failure.
        return t2;
    }

    // Evaluates y at the given x with default epsilon.
    public double Solve(double x)
    {
        return SolveWithEpsilon(x, kBezierEpsilon);
    }

    // Evaluates y at the given x. The epsilon parameter provides a hint as to the
    // required accuracy and is not guaranteed. Uses gradients if x is
    // out of [0, 1] range.
    public double SolveWithEpsilon(double x, double epsilon)
    {
        if (x < 0.0)
            return 0.0 + _startGradient * x;
        if (x > 1.0)
            return 1.0 + _endGradient * (x - 1.0);
        return SampleCurveY(SolveCurveX(x, epsilon));
    }

    // Returns an approximation of dy/dx at the given x with default epsilon.
    public double Slope(double x)
    {
        return SlopeWithEpsilon(x, kBezierEpsilon);
    }

    // Returns an approximation of dy/dx at the given x.
    // Clamps x to range [0, 1].
    public double SlopeWithEpsilon(double x, double epsilon)
    {
        x = Math.Min(Math.Max(x, 0.0), 1.0);
        double t = SolveCurveX(x, epsilon);
        double dx = SampleCurveDerivativeX(t);
        double dy = SampleCurveDerivativeY(t);
        return dy / dx;
    }

    // These getters are used rarely. We reverse compute them from coefficients.
    // See CubicBezier::InitCoefficients. The speed has been traded for memory.
    public double GetX1()
    {
        return _cx / 3.0;
    }

    public double GetY1()
    {
        return _cy / 3.0;
    }

    public double GetX2()
    {
        return (_bx + _cx) / 3.0 + GetX1();
    }

    public double GetY2()
    {
        return (_by + _cy) / 3.0 + GetY1();
    }

    // Gets the bezier's minimum y value in the interval [0, 1].
    public double range_min()
    {
        return _rangeMin;
    }

    // Gets the bezier's maximum y value in the interval [0, 1].
    public double range_max()
    {
        return _rangeMax;
    }


    private void InitCoefficients(double control1X, double control1Y, double control2X, double control2Y)
    {
        _cx = 3.0 * control1X;
        _bx = 3.0 * (control2X - control1X) - _cx;
        _ax = 1.0 - _cx - _bx;

        _cy = 3.0 * control1Y;
        _by = 3.0 * (control2Y - control1Y) - _cy;
        _ay = 1.0 - _cy - _by;
    }

    private void InitGradients(double control1X, double control1Y, double control2X, double control2Y)
    {
        if (control1X > 0)
            _startGradient = control1Y / control1X;
        else if (Math.Abs(control1Y) < kBezierEpsilon && control2X > 0)
            _startGradient = control2Y / control2X;
        else if (Math.Abs(control1Y) < kBezierEpsilon && Math.Abs(control2Y) < kBezierEpsilon)
            _startGradient = 1;
        else
            _startGradient = 0;

        if (control2X < 1)
            _endGradient = (control2Y - 1) / (control2X - 1);
        else if (Math.Abs(control2Y - 1) < kBezierEpsilon && control1X < 1)
            _endGradient = (control1Y - 1) / (control1X - 1);
        else if (Math.Abs(control2Y - 1) < kBezierEpsilon && Math.Abs(control1Y - 1) < kBezierEpsilon)
            _endGradient = 1;
        else
            _endGradient = 0;
    }

    private void InitRange(double control1Y, double control2Y)
    {
        _rangeMin = 0;
        _rangeMax = 1;
        if (0 <= control1Y && control1Y < 1 && 0 <= control2Y && control2Y <= 1)
            return;

        double epsilon = kBezierEpsilon;

        // Represent the function's derivative in the form at^2 + bt + c
        // as in sampleCurveDerivativeY.
        // (Technically this is (dy/dt)*(1/3), which is suitable for finding zeros
        // but does not actually give the slope of the curve.)
        double a = 3.0 * _ay;
        double b = 2.0 * _by;
        double c = _cy;

        // Check if the derivative is constant.
        if (Math.Abs(a) < epsilon && Math.Abs(b) < epsilon)
            return;

        // Zeros of the function's derivative.
        double t1 = 0;
        double t2 = 0;

        if (Math.Abs(a) < epsilon)
        {
            // The function's derivative is linear.
            t1 = -c / b;
        }
        else
        {
            // The function's derivative is a quadratic. We find the zeros of this
            // quadratic using the quadratic formula.
            double discriminant = b * b - 4 * a * c;
            if (discriminant < 0)
                return;
            double discriminant_sqrt = Math.Sqrt(discriminant);
            t1 = (-b + discriminant_sqrt) / (2 * a);
            t2 = (-b - discriminant_sqrt) / (2 * a);
        }

        double sol1 = 0;
        double sol2 = 0;

        // If the solution is in the range [0,1] then we include it, otherwise we
        // ignore it.

        // An interesting fact about these beziers is that they are only
        // actually evaluated in [0,1]. After that we take the tangent at that point
        // and linearly project it out.
        if (0 < t1 && t1 < 1)
            sol1 = SampleCurveY(t1);

        if (0 < t2 && t2 < 1)
            sol2 = SampleCurveY(t2);

        _rangeMin = Math.Min(Math.Min(_rangeMin, sol1), sol2);
        _rangeMax = Math.Max(Math.Max(_rangeMax, sol1), sol2);
    }

    private void InitSpline()
    {
        double deltaT = 1.0 / (CUBIC_BEZIER_SPLINE_SAMPLES - 1);
        _splineSamples = new double[CUBIC_BEZIER_SPLINE_SAMPLES];
        for (int i = 0; i < CUBIC_BEZIER_SPLINE_SAMPLES; i++)
        {
            _splineSamples[i] = SampleCurveX(i * deltaT);
        }
    }

    private double _ax;
    private double _bx;
    private double _cx;

    private double _ay;
    private double _by;
    private double _cy;

    private double _startGradient;
    private double _endGradient;

    private double _rangeMin;
    private double _rangeMax;

    private double[] _splineSamples;
}