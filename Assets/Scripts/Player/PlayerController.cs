using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed;

    bool isMoving;
    Vector2 input;

    Animator animator;

    [SerializeField] LayerMask solidObjectsLayer;
    [SerializeField] LayerMask longGrassLayer;
    public UnityAction OnEncouted;
    //[SerializeField] GameController gameController;
    [SerializeField] float encounterRate;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    public void HandleUpdate()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0)
            {
                input.y = 0;
            }

            if (input != Vector2.zero)
            {
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);
                Vector2 targetPos = transform.position;
                targetPos += input;
                if (IsWalkbale(targetPos))
                {
                    StartCoroutine(Move(targetPos));
                }
            }
        }
        animator.SetBool("isMoving", isMoving);        
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                moveSpeed*Time.deltaTime
                );
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;
        CheckForEncounters();
    }

    bool IsWalkbale(Vector2 targetPos)
    {
        bool hit = Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectsLayer);
        return !hit;
    }

    void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, longGrassLayer))
        {
            if (Random.Range(0, 100) < encounterRate)
            {
                Debug.Log("モンスターが現れた");
                //gameController.StartBatlle();
                OnEncouted();
                animator.SetBool("isMoving", false);
            }
        }
    }
}