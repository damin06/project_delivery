using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_Controller : MonoBehaviour
{
    public enum CarMove
    {
        Rear,
        All
    };
 public enum ControlMode
    {
        Keyboard,
        Buttons
    };

    public enum Axel
    {
        Front,
        Rear
    }

    [Serializable]
    public struct Wheel
    {
        public GameObject wheelModel;
        public WheelCollider wheelCollider;
        public GameObject wheelEffectObj;
        //public ParticleSystem smokeParticle;
        public Axel axel;
    }

    public ControlMode control;
    [SerializeField]private CarMove carmove;

    public float maxAcceleration = 30.0f;
    public float brakeAcceleration = 50.0f;
    [SerializeField]private float downForceValue;

    //public float  = 1.0f;
    public float maxSteerAngle = 30.0f;

    public Vector3 _centerOfMass;

    public List<Wheel> wheels;

    float moveInput;
    float steerInput;

    private Rigidbody rb;
    private float radius=6;
    private float currentSpeed;

    //private CarLights carLights;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = _centerOfMass;

        //carLights = GetComponent<CarLights>();
    }

    void Update()
    {
        AddForce();
        currentSpeed = rb.velocity.magnitude*3.6f;
        radius = 4 + currentSpeed/9;
        GetInputs();
        AnimateWheels();
        //WheelEffects();
    }

    void LateUpdate()
    {
        Move();
        Steer();
        Brake();
    }

    public void MoveInput(float input)
    {
        moveInput = input;
    }

    public void SteerInput(float input)
    {
        steerInput = input;
    }

    void GetInputs()
    {
        if(control == ControlMode.Keyboard)
        {
            moveInput = Input.GetAxis("Vertical");
            steerInput = Input.GetAxis("Horizontal");
        }
    }

    void Move()
    {
        foreach(var wheel in wheels)
        {
            if(carmove==CarMove.Rear)
            {
                if(wheel.axel == Axel.Rear)
                {
                    wheel.wheelCollider.motorTorque = moveInput * 1200 * maxAcceleration * Time.deltaTime;
                }
            }
            else
            {
                wheel.wheelCollider.motorTorque = moveInput * 600 * maxAcceleration * Time.deltaTime;
            }
            
        }
    }

    void Steer()
    {
        foreach(var wheel in wheels)
        {
            if (wheel.axel == Axel.Front)
            {
                var _steerAngle = steerInput * 1  * maxSteerAngle;
                //wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, _steerAngle, 0.6f);
                //wheel.wheelCollider.steerAngle = _steerAngle;
                wheel.wheelCollider.steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f /(radius + (1.5f/2))) * steerInput;
            }
        }
    }

    void Brake()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 300 * brakeAcceleration * Time.deltaTime;
            }

            //carLights.isBackLightOn = true;
            //carLights.OperateBackLights();
        }
        else
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 0;
            }

            //carLights.isBackLightOn = false;
            //carLights.OperateBackLights();
        }
    }

    void AnimateWheels()
    {
        foreach(var wheel in wheels)
        {
            Quaternion rot = Quaternion.identity;
            Vector3 pos = Vector3.zero;
            wheel.wheelCollider.GetWorldPose(out pos, out rot);
            wheel.wheelModel.transform.position = pos;
            wheel.wheelModel.transform.rotation = rot;
        }
    }

    private void AddForce()
    {
        rb.AddForce(-transform.up * downForceValue * rb.velocity.magnitude);
    }

    // void WheelEffects()
    // {
    //     foreach (var wheel in wheels)
    //     {
    //         //var dirtParticleMainSettings = wheel.smokeParticle.main;

    //         if (Input.GetKey(KeyCode.Space) && wheel.axel == Axel.Rear && wheel.wheelCollider.isGrounded == true && carRb.velocity.magnitude >= 10.0f)
    //         {
    //             wheel.wheelEffectObj.GetComponentInChildren<TrailRenderer>().emitting = true;
    //             //wheel.smokeParticle.Emit(1);
    //         }
    //         else
    //         {
    //             wheel.wheelEffectObj.GetComponentInChildren<TrailRenderer>().emitting = false;
    //         }
    //     }
    // }
}
