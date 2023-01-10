using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KillEnemy : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    private void Start()
    {
        text.enabled = false;
    }
    private void OnTriggerStay(Collider other)
    {
        
        if (other.tag == "Enemy")
        {
            text.enabled = true;

            if (Input.GetKey(KeyCode.E))
            {
                other.GetComponent<EnemyStateManager>().SwitchState(other.GetComponent<EnemyStateManager>().deadState);
                text.enabled = false;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        
        if (other.tag == "Enemy")
        {
            text.enabled = false;
        }
    }

}
