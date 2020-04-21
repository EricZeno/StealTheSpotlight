using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuzzerAttack : EnemyAttack {
    #region Constants
    public const int CLOSE_ATTACK_NUM = 0;
    public const int LONG_ATTACK_NUM = 1;

    private const float LASER_OFFSET = 1.5f;
    #endregion

    #region Variables
    #region Editor Variables
    [SerializeField]
    [Tooltip("How long the laser attack has to charge for")]
    private float m_LaserChargeTime;

    [SerializeField]
    [Tooltip("How long the laser attack takes to complete")]
    private float m_LaserTime;

    [SerializeField]
    [Tooltip("How wide each individual laser will be")]
    private float m_LaserWidth;

    [SerializeField]
    [Tooltip("The prefab used for the laser attack")]
    private GameObject m_Laser;

    [SerializeField]
    [Tooltip("How long the shoot attack takes to complete")]
    private float m_ShootTime;

    [SerializeField]
    [Tooltip("How many bullets the shoot attack will fire")]
    private float m_NumBullets;

    [SerializeField]
    [Tooltip("How fast the bullets will travel")]
    private float m_ShotSpeed;

    [SerializeField]
    [Tooltip("The bullet prefab")]
    private GameObject m_Bullet;
    #endregion
    #endregion

    #region External Calls
    public override void Attack(int attackNum, Vector2 target) {
        switch (attackNum) {
            case CLOSE_ATTACK_NUM:
                StartCoroutine(Laser());
                break;
            case LONG_ATTACK_NUM:
                StartCoroutine(Shoot(target));
                break;
            default:
                throw new System.ArgumentException("Trying to trigger an invalid attack");
        }
    }
    #endregion

    #region Attacks
    private IEnumerator Laser() {
        // Play charging animation

        m_Manager.SetFlash(true);

        int numOfFlashes = 5;
        float flashDelay = m_LaserChargeTime / numOfFlashes;
        Color flashColor = Color.red;
        for (int i = 0; i < numOfFlashes; i++) {
            m_Graphics.SetColor(flashColor);
            yield return new WaitForSeconds(flashDelay);
            m_Graphics.SetColor(Color.white);
            yield return new WaitForSeconds(flashDelay);
        }

        m_Manager.SetFlash(false);

        Vector2 upOrigin = (Vector2)transform.position + new Vector2(0, LASER_OFFSET);
        Vector2 downOrigin = (Vector2)transform.position + new Vector2(0, -LASER_OFFSET);
        Vector2 rightOrigin = (Vector2)transform.position + new Vector2(LASER_OFFSET, 0);
        Vector2 leftOrigin = (Vector2)transform.position + new Vector2(-LASER_OFFSET, 0);

        LayerMask WallLayer = (1 << LayerMask.NameToLayer("Wall")) | (1 << LayerMask.NameToLayer("Object"));
        RaycastHit2D upWall = Physics2D.Raycast(upOrigin, Vector2.up, float.PositiveInfinity, WallLayer);
        RaycastHit2D downWall = Physics2D.Raycast(downOrigin, Vector2.down, float.PositiveInfinity, WallLayer);
        RaycastHit2D rightWall = Physics2D.Raycast(rightOrigin, Vector2.right, float.PositiveInfinity, WallLayer);
        RaycastHit2D leftWall = Physics2D.Raycast(leftOrigin, Vector2.left, float.PositiveInfinity, WallLayer);

        float upDistance = upWall.distance;
        float downDistance = downWall.distance;
        float rightDistance = rightWall.distance;
        float leftDistance = leftWall.distance;

        int damage = m_Manager.GetEnemyData().Damage;

        GameObject upLaser = Instantiate(m_Laser, transform);
        upLaser.GetComponent<BuzzerLaser>().SetupAndFire(upOrigin, upDistance, m_LaserWidth, Vector2.up, damage);

        GameObject downLaser = Instantiate(m_Laser, transform);
        downLaser.GetComponent<BuzzerLaser>().SetupAndFire(downOrigin, downDistance, m_LaserWidth, Vector2.down, damage);

        GameObject rightLaser = Instantiate(m_Laser, transform);
        rightLaser.GetComponent<BuzzerLaser>().SetupAndFire(rightOrigin, rightDistance, m_LaserWidth, Vector2.right, damage);

        GameObject leftLaser = Instantiate(m_Laser, transform);
        leftLaser.GetComponent<BuzzerLaser>().SetupAndFire(leftOrigin, leftDistance, m_LaserWidth, Vector2.left, damage);

        yield return new WaitForSeconds(m_LaserTime - m_LaserChargeTime);

        Destroy(upLaser);
        Destroy(downLaser);
        Destroy(rightLaser);
        Destroy(leftLaser);

        ((BuzzerMovement)m_Movement).Attacking = false;
    }

    private IEnumerator Shoot(Vector2 target) {
        float timeBetweenShots = m_ShootTime / m_NumBullets;
        float attackCone = m_Manager.GetEnemyData().AttackCone;
        float initialDegree = -attackCone / 2f;
        float degreeIncrement = attackCone / m_NumBullets;

        Vector2 firingVector = (target - (Vector2)transform.position).normalized;
        firingVector = Quaternion.Euler(0, 0, initialDegree) * firingVector;
        firingVector.Normalize();

        int damage = m_Manager.GetEnemyData().Damage;

        for (int i = 0; i < m_NumBullets; i++) {
            GameObject bullet = Instantiate(m_Bullet, transform);
            bullet.GetComponent<Projectile>().Setup(damage, firingVector, m_ShotSpeed);

            firingVector = Quaternion.Euler(0, 0, degreeIncrement) * firingVector;
            firingVector.Normalize();

            yield return new WaitForSeconds(timeBetweenShots);
        }

        ((BuzzerMovement)m_Movement).Attacking = false;
        yield return null;
    }
    #endregion
}
