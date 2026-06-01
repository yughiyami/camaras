using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;

public class DoomTigerAI : MonoBehaviour
{
    public enum State { PATROL, CHASE, CAUGHT }
    public State currentState = State.PATROL;

    [Header("Patrol Settings")]
    public float patrolRadius = 20f;
    public float patrolSpeed = 3f;
    public float patrolWaitTime = 2f;

    [Header("Chase Settings")]
    public float chaseThreshold = 12f;
    public float chaseSpeed = 6f;
    public float catchDistance = 1.5f;

    [Header("References")]
    public Transform player;
    public UnityEngine.UI.Image deathFlashImage;
    public AudioSource heartbeatAudio;

    private NavMeshAgent _agent;
    private Animator _animator;
    private Vector3 _playerStartPos;
    private float _patrolTimer = 0f;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();
        
        if (player == null)
        {
            GameObject playerObj = GameObject.Find("DeerPlayer");
            if (playerObj != null) player = playerObj.transform;
        }

        if (player != null)
            _playerStartPos = player.position;

        if (deathFlashImage == null)
        {
            GameObject flash = GameObject.Find("DeathFlash");
            if (flash != null) deathFlashImage = flash.GetComponent<UnityEngine.UI.Image>();
        }

        if (heartbeatAudio == null)
            heartbeatAudio = GetComponent<AudioSource>();

        SetPatrolTarget();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.PATROL:
                HandlePatrol(distance);
                break;
            case State.CHASE:
                HandleChase(distance);
                break;
            case State.CAUGHT:
                break;
        }

        UpdateAnimations();
    }

    void UpdateAnimations()
    {
        if (_animator == null) return;
        float speed = _agent.velocity.magnitude;
        // Parámetros solicitados: isWalking e isRunning
        _animator.SetBool("isWalking", speed > 0.1f && currentState == State.PATROL);
        _animator.SetBool("isRunning", speed > 0.1f && currentState == State.CHASE);
    }

    void HandlePatrol(float playerDistance)
    {
        _agent.speed = patrolSpeed;

        if (playerDistance < chaseThreshold)
        {
            currentState = State.CHASE;
            if (heartbeatAudio != null) heartbeatAudio.Play();
            return;
        }

        if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
        {
            _patrolTimer += Time.deltaTime;
            if (_patrolTimer >= patrolWaitTime)
            {
                SetPatrolTarget();
                _patrolTimer = 0f;
            }
        }
    }

    void HandleChase(float playerDistance)
    {
        _agent.speed = chaseSpeed;
        _agent.SetDestination(player.position);

        if (heartbeatAudio != null)
        {
            float vol = 1f - (playerDistance / chaseThreshold);
            heartbeatAudio.volume = Mathf.Clamp01(vol);
        }

        if (playerDistance > chaseThreshold * 1.5f)
        {
            currentState = State.PATROL;
            if (heartbeatAudio != null) heartbeatAudio.Stop();
            SetPatrolTarget();
        }
        else if (playerDistance < catchDistance)
        {
            currentState = State.CAUGHT;
            StartCoroutine(PerformCaughtSequence());
        }
    }

    void SetPatrolTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, 1))
        {
            _agent.SetDestination(hit.position);
        }
    }

    IEnumerator PerformCaughtSequence()
    {
        _agent.isStopped = true;
        if (heartbeatAudio != null) heartbeatAudio.Stop();

        Camera cam = Camera.main;
        if (cam == null) yield break;
        
        Vector3 originalCamPos = cam.transform.localPosition;
        float elapsed = 0f;
        float duration = 0.6f;

        if (deathFlashImage != null)
        {
            Color c = deathFlashImage.color;
            c.a = 0.8f;
            deathFlashImage.color = c;
        }

        while (elapsed < duration)
        {
            float x = Random.Range(-0.1f, 0.1f);
            float y = Random.Range(-0.1f, 0.1f);
            cam.transform.localPosition = originalCamPos + new Vector3(x, y, 0);

            if (deathFlashImage != null)
            {
                Color c = deathFlashImage.color;
                c.a = Mathf.Lerp(0.8f, 0f, elapsed / 0.5f);
                deathFlashImage.color = c;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.transform.localPosition = originalCamPos;
        if (deathFlashImage != null) deathFlashImage.color = new Color(1, 0, 0, 0);

        player.position = _playerStartPos;
        yield return new WaitForSeconds(1f);
        
        currentState = State.PATROL;
        _agent.isStopped = false;
        SetPatrolTarget();
    }
}

