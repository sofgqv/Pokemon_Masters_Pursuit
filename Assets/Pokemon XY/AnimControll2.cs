using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimControll2 : MonoBehaviour
{
    public float moveSpeed = 2.0f; // Velocidad de movimiento
    public float rotationSpeed = 120f; // Velocidad de giro
    public float idleTime = 2.0f; // Tiempo en estado Idle
    public float walkDistance = 5.0f; // Distancia a caminar

    private Animator animator;
    private Vector3 targetDirection;
    private Vector3 startPosition;
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
    }

    void Update()
    {
        // Controlar el comportamiento según el estado actual
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
    }

    private void SetIdleState()
    {
        currentState = State.Idle;
        idleTimer = idleTime;
        animator.SetBool("IsWalking", false); // Asegurar que está en Idle
    }

    private void UpdateIdle()
    {
        idleTimer -= Time.deltaTime;

        // Si termina el tiempo de Idle, iniciar rotación
        if (idleTimer <= 0f)
        {
            StartRotating();
        }
    }

    private void StartRotating()
    {
        currentState = State.Rotating;
        isRotating = true;

        // Generar una dirección aleatoria en el plano XZ
        targetDirection = Random.insideUnitSphere;
        targetDirection.y = 0; // Mantener en el plano
        targetDirection.Normalize();
    }

    private void RotateTowardsTarget()
    {
        if (isRotating)
        {
            // Rotar suavemente hacia la nueva dirección
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Si la rotación está completa, empezar a caminar
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
        animator.SetBool("IsWalking", true); // Activar animación Walking
        startPosition = transform.position; // Guardar la posición inicial para calcular la distancia
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

