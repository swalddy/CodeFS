using System.Collections;
using System.IO.Pipes;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public bool canMove = true;
    public PipeServer2D pipeServer;
    private bool isMoving;
    private Vector2 input;
    private Animator animator;
    public LayerMask solidObjectLayer;
    public LayerMask NPCLayer;
    private NPC npcInteraksiSaatIni;
    private Coroutine currentMoveCoroutine;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        pipeServer = PipeServer2D.Instance;
    }

    private void Update()
    {
        HandleUpdate();
    }

    public void HandleUpdate()
    {
        if (!canMove)
        {
            if (isMoving && currentMoveCoroutine != null)
            {
                StopCoroutine(currentMoveCoroutine);
                isMoving = false;
            }
            return;
        }

        string predictedDirection = pipeServer.predictedDirection;

        if (string.IsNullOrEmpty(predictedDirection))
            return;

        input = Vector2.zero;
        if (predictedDirection == "Atas")
            input.y = 1;
        else if (predictedDirection == "Bawah")
            input.y = -1;
        else if (predictedDirection == "Kanan")
            input.x = 1;
        else if (predictedDirection == "Kiri")
            input.x = -1;

        animator.SetFloat("MoveX", input.x);
        animator.SetFloat("MoveY", input.y);

        var targetPos = transform.position;
        targetPos.x += input.x * moveSpeed * Time.deltaTime;
        targetPos.y += input.y * moveSpeed * Time.deltaTime;

        if (!isMoving && isWalkable(targetPos))
        {
            currentMoveCoroutine = StartCoroutine(Move(targetPos));
        }

        animator.SetBool("isMoving", input != Vector2.zero);

        if (Input.GetKeyDown(KeyCode.F)) 
        {
            Interact();
        }
    }

    void Interact()
    {
        var facingDir = new Vector3(animator.GetFloat("MoveX"), animator.GetFloat("MoveY"));
        var interactPos = transform.position + facingDir;

        var collider = Physics2D.OverlapCircle(interactPos, 0.5f, NPCLayer);
        if (collider != null)
        {
            var npc = collider.GetComponent<NPC>();
            if (npc != null)
            {
                DisableMovement();
                npc.Interact(EnableMovement);
            }
        }
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        isMoving = false;
    }

    private bool isWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.5f, solidObjectLayer | NPCLayer) != null)
        {
            return false;
        }
        return true;
    }

    public void EnableMovement()
    {
        canMove = true;
        if (currentMoveCoroutine != null)
        {
            StopCoroutine(currentMoveCoroutine);
            currentMoveCoroutine = null;
        }
    }

    public void DisableMovement()
    {
        canMove = false;
        if (currentMoveCoroutine != null)
        {
            StopCoroutine(currentMoveCoroutine);
            currentMoveCoroutine = null;
        }
    }

    private void OnDisable()
    {
        if (currentMoveCoroutine != null)
        {
            StopCoroutine(currentMoveCoroutine);
            currentMoveCoroutine = null;
        }
    }

    public void SetActiveNPC(NPC npc)
    {
        npcInteraksiSaatIni = npc;
    }

    public void ClearActiveNPC()
    {
        npcInteraksiSaatIni = null;
    }
}

