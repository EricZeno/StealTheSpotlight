using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
public class EnemyGraphics : MonoBehaviour {
    #region Variables
    #region Editor Variables
    [SerializeField]
    [Tooltip("The renderer for the body of the enemy.")]
    private SpriteRenderer m_Body;

    [SerializeField]
    [Tooltip("The renderer for the foreground leg of the enemy.")]
    private SpriteRenderer m_FrontLeg;

    [SerializeField]
    [Tooltip("The renderer for the background leg of the enemy.")]
    private SpriteRenderer m_BackLeg;
    #endregion

    #region Cached Components
    private Animator m_Animator;
    #endregion
    #endregion

    #region Initialization
    private void Awake() {
        m_Animator = GetComponent<Animator>();
    }
    #endregion

    #region Move
    public void Move(Vector2 moveDir) {
        bool isMoving = moveDir.sqrMagnitude > 0;
        m_Animator.SetBool("IsMoving", isMoving);
        if (isMoving) {
            bool isMovingLeft = moveDir.x < 0;
            m_Animator.SetBool("IsMovingLeft", isMovingLeft);
        }
    }
    #endregion

    #region Facing Direction
    public void FacingDirection(Vector2 facingDir) {
        bool isFacingLeft = facingDir.x < 0;
        m_Animator.SetBool("IsFacingLeft", !isFacingLeft);
    }
    #endregion
}
