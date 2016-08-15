﻿using UnityEngine;
using System.Collections;

public class StandingState : State
{
    private CharacterLogic character;
    private Animator animator;

    private float lingerTimer;

    public StandingState(CharacterLogic character)
    {
        this.character = character;
        animator = character.GetComponent<Animator>();
    }

    public override bool OnEnter(State previousState)
    {
        lingerTimer = 0.0f;
        return true;
    }

    public override void HandleCommand<Type>(Type command)
    {
        if(typeof(Type) == typeof(MoveCommand))
        {
            if(character.stateMachine.ChangeState(character.movingState))
            {
                character.movingState.HandleCommand(command);
                return;
            }
        }
    }

    public override void OnUpdate()
    {
        // Stop character movement animation.
        float verticalSpeed = animator.GetFloat("Movement");
        verticalSpeed = Mathf.MoveTowards(verticalSpeed, 0.0f, Time.fixedDeltaTime);
        animator.SetFloat("Movement", verticalSpeed);

        // Trigger a linger animation.
        lingerTimer += Time.fixedDeltaTime;

        if(lingerTimer > 8.0f)
        {
            animator.SetTrigger("Linger");
            lingerTimer = 0.0f;
        }
    }
}

public class MovingState : State
{
    private CharacterLogic character;
    private Transform transform;
    private Animator animator;

    private Vector3 direction;
    private bool commandReceived;

    public MovingState(CharacterLogic character)
    {
        this.character = character;
        transform = character.GetComponent<Transform>();
        animator = character.GetComponent<Animator>();
    }

    public override bool OnEnter(State previousState)
    {
        direction = Vector3.zero;
        return true;
    }

    public override void HandleCommand<Type>(Type command)
    {
        if(typeof(Type) == typeof(MoveCommand))
        {
            MoveCommand moveCommand = (MoveCommand)(object)command;
            direction = moveCommand.direction;
            commandReceived = true;
        }
    }

    public override void OnUpdate()
    {
        if(commandReceived)
        {
            // Set animation speed.
            float verticalSpeed = animator.GetFloat("Movement");
            verticalSpeed = Mathf.MoveTowards(verticalSpeed, 1.0f, Time.fixedDeltaTime);
            animator.SetFloat("Movement", verticalSpeed);

            // Set rotation direction.
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }
        else
        {
            if(character.stateMachine.ChangeState(character.standingState))
                return;
        }

        // Reset command received flag.
        commandReceived = false;
    }
}
