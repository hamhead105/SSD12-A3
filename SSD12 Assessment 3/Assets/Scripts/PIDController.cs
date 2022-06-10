using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Proportional, Integral, Derivative Controller
 * Uses calculus to interpolate a value to a target
 */
public class PIDController : MonoBehaviour
{
    private float kP;
    private float kI;
    private float kD;

    private float errorAccumulation;
    private float errorDerivative;

    private float lastError = 0;
    private float lastTime;

    /*
     * Initialize the PID controller
     */
    public void Init()
    {
        lastTime = Time.time;
    }

    /*
     * Base constructor
     * 
     * @param kP - proportional constant
     * @param kI - integral constant
     * @param KD - derivative constant
     */
    public PIDController(float kP, float kI, float kD)
    {
        this.kP = kP;
        this.kI = kI;
        this.kD = kD;
    }

    /*
     * return the output of the PID controller
     * 
     * @param error - distance to target
     * @return - the output value
     */
    public float GetOutput(float error)
    {
        float deltaTime = Time.time - lastTime;
        lastTime = Time.time;
        float output;
        if (deltaTime > 0)
        {
            errorAccumulation += error * deltaTime * GameManagement.DeltaTimeFrameSpeedConstant;
            errorDerivative = (error - lastError) * deltaTime * GameManagement.DeltaTimeFrameSpeedConstant;
        }
        else
        {
            errorAccumulation = 0;
            errorDerivative = 0;
        }
        output = error * kP * Time.deltaTime * GameManagement.DeltaTimeFrameSpeedConstant + (errorAccumulation * kI) + (errorDerivative * kD);
        return output;
    }
}