using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimControll2 : MonoBehaviour
{
    public float moveSpeed = 2.0f; // Velocidad de movimiento
    public float rotationSpeed = 120f; // Velocidad de giro
    public float idleTime = 2.0f; // Tiempo en estado Idle
    public float walkDistance = 5.0f; // Distancia a caminar
    public float movementThreshold = 0.01f; // Distancia m�nima para detectar movimiento

    private Animator animator;
    private Vector3 targetDirection;
    private Vector3 startPosition;
    private Vector3 previousPosition; // Posici�n previa para calcular movimiento
    private float distanceMoved = 0f;
    private float idleTimer = 0f;
    private bool isRotating = false;

    private enum State { Idle, Rotating, Walking }
    private State currentState = State.Idle;

    void Start()
    {
        // Obtener el componente Animator
        animator = GetComponent<Animator>();

        // Configurar el estado inicial
        SetIdleState();

        // Inicializar la posici�n previa
        previousPosition = transform.position;
    }

    void Update()
    {
        // Actualizar el par�metro de animaci�n seg�n el movimiento
        ForceWalkingAnimation();

        // Controlar el comportamiento seg�n el estado actual
        switch (currentState)
        {
            case State.Idle:
                UpdateIdle();
                break;

            case State.Rotating:
                RotateTowardsTarget();
                break;

            case State.Walking:
                UpdateWalking();
                break;
        }

        // Actualizar la posici�n previa
        previousPosition = transform.position;
    }

    private void ForceWalkingAnimation()
    {
        // Calcular el desplazamiento en este frame
        float distance = Vector3.Distance(previousPosition, transform.position);

        // Forzar el estado del Animator si se supera el umbral de movimiento
        if (distance >= movementThreshold)
        {
            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }
    }

    private void SetIdleState()
    {
        currentState = State.Idle;
        idleTimer = idleTime;
        animator.SetBool("IsWalking", false); // Asegurar que est� en Idle
    }

    private void UpdateIdle()
    {
        idleTimer -= Time.deltaTime;

        // Si termina el tiempo de Idle, iniciar rotaci�n
        if (idleTimer <= 0f)
        {
            StartRotating();
        }
    }

    private void StartRotating()
    {
        currentState = State.Rotating;
        isRotating = true;

        // Generar una direcci�n aleatoria en el plano XZ
        targetDirection = Random.insideUnitSphere;
        targetDirection.y = 0; // Mantener en el plano
        targetDirection.Normalize();
    }

    private void RotateTowardsTarget()
    {
        if (isRotating)
        {
            // Rotar suavemente hacia la nueva direcci�n
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Si la rotaci�n est� completa, empezar a caminar
            if (Quaternion.Angle(transform.rotation, targetRotation) < 5f)
            {
                isRotating = false;
                StartWalking();
            }
        }
    }

    private void StartWalking()
    {
        currentState = State.Walking;
        startPosition = transform.position; // Guardar la posici�n inicial para calcular la distancia
        distanceMoved = 0f;
    }

    private void UpdateWalking()
    {
        // Mover al NPC hacia adelante
        Vector3 movement = transform.forward * moveSpeed * Time.deltaTime;
        transform.Translate(movement, Space.World);

        // Calcular la distancia total recorrida
        distanceMoved += movement.magnitude;

        // Si se alcanza la distancia deseada, volver al estado Idle
        if (distanceMoved >= walkDistance)
        {
            SetIdleState();
        }
    }
}
