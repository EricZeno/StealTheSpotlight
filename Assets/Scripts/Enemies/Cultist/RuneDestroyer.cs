using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneDestroyer : MonoBehaviour {
    private float m_Time;
    public float time {
        set {
            m_Time = value;
        }
    }

    // Start is called before the first frame update
    void Start() {
        StartCoroutine(Delete());
    }

    IEnumerator Delete() {
        yield return new WaitForSeconds(m_Time);

        Destroy(gameObject);
    }
}
