using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.InputSystem;

public class GameAgent : Agent
{
    public Transform Target;
    public float speed = 10f;
    Rigidbody rBody;

    // 1. Setup
    public override void Initialize()
    {
        rBody = GetComponent<Rigidbody>();
    }

    // 2. Start of each "Round" (Episode)
    public override void OnEpisodeBegin()
    {

        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.linearVelocity = Vector3.zero;
        this.transform.localPosition = new Vector3(0, 0.5f, 0);

        // If the Agent fell off the platform, reset it
        // if (this.transform.localPosition.y < 0)
        // {
        //     this.rBody.angularVelocity = Vector3.zero;
        //     this.rBody.linearVelocity = Vector3.zero;
        //     this.transform.localPosition = new Vector3(0, 0.5f, 0);
        // }

        // Move the Target to a new random spot
        Target.localPosition = new Vector3(Random.value * 16 - 8, 0.5f, Random.value * 16 - 8);
    }

    // 3. The "Eyes" (Observations)
    public override void CollectObservations(VectorSensor sensor)
    {
        // The Agent needs to know where IT is
        sensor.AddObservation(this.transform.localPosition);

        // The Agent needs to know where the TARGET is
        sensor.AddObservation(Target.localPosition);
    }

    // 4. The "Muscles" (Actions) & "Motivation" (Rewards)
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Move the agent based on signals from the Brain
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0]; // Signal 1: Move X
        controlSignal.z = actionBuffers.ContinuousActions[1]; // Signal 2: Move Z
        rBody.AddForce(controlSignal * speed);

        // Tiny penalty every decision (e.g., -1/5000th of a point)
        // This encourages the agent to find the ball via the shortest path.

        AddReward(-0.001f);

        if (StepCount > 5000) 
        {
            EndEpisode();
        }

        if (GetCumulativeReward() <= -1f)
        {
            EndEpisode();
        }

        // Calculate distance to target
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        // REWARD: Did we reach the target?
        if (distanceToTarget < 1.5f)
        {
            // SetReward(1.0f); // Good job!
            // EndEpisode();    // Good job! (Implicit reward is ending the run early)
            
            AddReward(1.0f); // Good job!
            Target.localPosition = new Vector3(Random.value * 16 - 8, 0.5f, Random.value * 16 - 8);
        }

        // PUNISHMENT: Did we fall off?
        if (this.transform.localPosition.y < 0)
        {
            SetReward(-1.0f); // Bad job!
            EndEpisode();    // Bad job! (Implicit punishment is ending the run early)
        }
    }
    
    // 5. Manual Control (So you can test it with keyboard)
    // REPLACE THE OLD HEURISTIC FUNCTION WITH THIS:
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        
        // 1. Reset values to 0 (so we don't keep moving if no key is pressed)
        continuousActionsOut[0] = 0;
        continuousActionsOut[1] = 0;

        // 2. Get the current keyboard
        var keyboard = Keyboard.current;
        
        // Safety check: If no keyboard is connected, exit
        if (keyboard == null) return;

        // 3. X-Axis (Left/Right)
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
        {
            continuousActionsOut[0] = 1f;
        }
        else if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
        {
            continuousActionsOut[0] = -1f;
        }

        // 4. Z-Axis (Forward/Back)
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
        {
            continuousActionsOut[1] = 1f;
        }
        else if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
        {
            continuousActionsOut[1] = -1f;
        }
    }
}