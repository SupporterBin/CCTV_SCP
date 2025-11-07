using UnityEngine;

public class Creature3TentacleAnim : MonoBehaviour
{
    Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.Play("Take 001", 0, Random.Range(0f,1f));
    }

}
