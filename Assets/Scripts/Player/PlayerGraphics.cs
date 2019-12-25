using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
public class PlayerGraphics : MonoBehaviour {
    #region Variables
    #region Editor Variables
    [SerializeField]
    [Tooltip("The parent game object of all the renderer.")]
    private GameObject m_Parent;

    [SerializeField]
    [Tooltip("The renderer for the body of the player.")]
    private SpriteRenderer m_Body;
    
    [SerializeField]
    [Tooltip("The renderer for the foreground leg of the player.")]
    private SpriteRenderer m_FrontLeg;

    [SerializeField]
    [Tooltip("The renderer for the background leg of the player.")]
    private SpriteRenderer m_BackLeg;
    #endregion

    #region Cached Components
    private Animator m_Animator;
    #endregion
    #endregion

    #region Initialization
    private void Awake() {
        SetupCachedComponents();
    }

    private void SetupCachedComponents() {
        m_Animator = GetComponent<Animator>();
    }

    public void SetupSprites(PlayerSprites sprites) {
        m_Body.sprite = sprites.Body;
        m_FrontLeg.sprite = sprites.FrontLeg;
        m_BackLeg.sprite = sprites.BackLeg;
    }
    #endregion

    #region Hide/Show
    public void Hide() {
        m_Parent.gameObject.SetActive(false);
    }

    public void Show() {
        m_Parent.gameObject.SetActive(true);
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
        m_Animator.SetBool("IsFacingLeft", isFacingLeft);
    }
    #endregion
}
