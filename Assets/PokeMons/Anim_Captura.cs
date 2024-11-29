using UnityEngine;

public class AnimControll3 : MonoBehaviour
{
    public float moveSpeed = 2.0f; // Velocidad de movimiento
    public float rotationSpeed = 120f; // Velocidad de giro
    public float walkDistance = 5.0f; // Distancia a caminar
    public float additionalRotation = 10.0f; // Rotaci�n adicional despu�s del ciclo

    private Animator animator;
    private Rigidbody rb; // Para manejar la f�sica
    private Vector3 startPosition;
    private float distanceMoved = 0f;
    private bool returning = false; // Indica si el objeto est� regresando

    private enum State { WalkingForward, Turning, WalkingBackward }
    private State currentState = State.WalkingForward;

    void Start()
    {
        // Obtener componentes
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Se necesita un Rigidbody en el objeto.");
            return;
        }

        // Configurar el estado inicial
        StartWalkingForward();
    }

    void Update()
    {
        // Actualizar animaci�n seg�n velocidad
        UpdateAnimator();

        // Controlar el comportamiento seg�n el estado actual
        switch (currentState)
        {
            case State.WalkingForward:
                UpdateWalkingForward();
                break;

            case State.Turning:
                UpdateTurning();
                break;

            case State.WalkingBackward:
                UpdateWalkingBackward();
                break;
        }
    }

    private void UpdateAnimator()
    {
        // Calcular la velocidad del objeto
        float speed = rb.velocity.magnitude;

        // Si la velocidad es mayor a un umbral m�nimo, activa la animaci�n de Walking
        animator.SetBool("IsWalking", speed > 0.01f);
    }

    private void StartWalkingForward()
    {
        currentState = State.WalkingForward;
        startPosition = transform.position;
        distanceMoved = 0f;
        returning = false;
    }

    private void UpdateWalkingForward()
    {
        MoveForward();

        if (distanceMoved >= walkDistance)
        {
            StartTurning(180f); // Girar 180 grados para regresar
        }
    }

    private void StartWalkingBackward()
    {
        currentState = State.WalkingBackward;
        startPosition = transform.position;
        distanceMoved = 0f;
        returning = true;
    }

    private void UpdateWalkingBackward()
    {
        MoveForward();

        if (distanceMoved >= walkDistance)
        {
            StartTurning(additionalRotation); // Giro adicional
        }
    }

    private void MoveForward()
    {
        // Usar Rigidbody para movimiento f�sico
        Vector3 movement = transform.forward * moveSpeed;
        rb.velocity = movement;

        // Calcular la distancia recorrida
        distanceMoved += movement.magnitude * Time.deltaTime;
    }

    private void StartTurning(float angle)
    {
        currentState = State.Turning;
        StartCoroutine(TurnCoroutine(angle));
    }

    private void UpdateTurning()
    {
        // Este estado est� controlado por la corrutina, no necesita l�gica adicional aqu�.
    }

    private System.Collections.IEnumerator TurnCoroutine(float angle)
    {
        float rotatedAngle = 0f;

        while (rotatedAngle < Mathf.Abs(angle))
        {
            float step = rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, step * Mathf.Sign(angle));
            rotatedAngle += step;
            yield return null;
        }

        // Cambiar al estado adecuado seg�n si est� regresando o no
        if (returning)
        {
            StartWalkingForward();
        }
        else
        {
            StartWalkingBackward();
        }
    }
}
