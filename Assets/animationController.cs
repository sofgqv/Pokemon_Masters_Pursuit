using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationController : MonoBehaviour
{
    public float moveSpeed = 2.0f; // Velocidad del movimiento
    public float rotationSpeed = 120f; // Velocidad de giro
    public float idleTime = 2.0f; // Tiempo de inactividad
    public float walkingTime = 3.0f; // Tiempo caminando

    private Animator animator;
    private Vector3 targetDirection;
    private float timer = 0f;

    private enum State { Idle, Rotating, Walking }
    private State currentState = State.Idle;

    void Start()
    {
        // Obtener el componente Animator del NPC
        animator = GetComponent<Animator>();

        // Iniciar en el estado Idle
        SetIdleState();
    }

    void Update()
    {
        // Disminuir el temporizador cada frame
        timer -= Time.deltaTime;

        // Manejar los estados seg�n el estado actual
        switch (currentState)
        {
            case State.Idle:
                if (timer <= 0f)
                {
                    StartRotating();
                }
                break;

            case State.Rotating:
                RotateTowardsTarget();
                break;

            case State.Walking:
                MoveForward();
                break;
        }
    }

    // Estado: NPC en Idle
    private void SetIdleState()
    {
        currentState = State.Idle;
        timer = idleTime;

        // Activar el par�metro Idle en el Animator
        animator.SetBool("Idle", true);
        animator.SetBool("Walking", false);
        animator.SetBool("StopMoving", false);
        animator.SetBool("StartMoving", false);
    }

    // Transici�n al estado de rotaci�n
    private void StartRotating()
    {
        currentState = State.Rotating;
        targetDirection = Random.insideUnitSphere;
        targetDirection.y = 0; // Mantener en el plano XZ
        timer = Random.Range(1.0f, 2.0f); // Tiempo de rotaci�n aleatorio

        // Preparar para mover, desactivar Idle
        animator.SetBool("Idle", false);
        animator.SetBool("StartMoving", true);
    }

    // Rotar hacia una direcci�n objetivo
    private void RotateTowardsTarget()
    {
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Si ya se gir� hacia la direcci�n o el tiempo se acab�, iniciar caminar
        if (Quaternion.Angle(transform.rotation, targetRotation) < 5f || timer <= 0f)
        {
            StartWalking();
        }
    }

    // Transici�n al estado de caminar
    private void StartWalking()
    {
        currentState = State.Walking;
        timer = walkingTime;

        // Activar la animaci�n de caminar
        animator.SetBool("StartMoving", false);
        animator.SetBool("Walking", true);
    }

    // Movimiento hacia adelante
    private void MoveForward()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        // Cuando el temporizador se agota, detenerse
        if (timer <= 0f)
        {
            animator.SetBool("StopMoving", true);
            animator.SetBool("Walking", false);
            SetIdleState();
        }
    }

}
