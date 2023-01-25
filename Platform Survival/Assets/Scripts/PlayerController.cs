using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class GameObjectEfx
{
    public static void DrawCircle(this GameObject container, float radius, float lineWidth)
    {
       var segments = 360;
       var lineRenderer = container.AddComponent<LineRenderer>();

       lineRenderer.useWorldSpace = false;
       lineRenderer.startWidth = lineWidth;
       lineRenderer.endWidth = lineWidth;
       lineRenderer.positionCount = segments +1;

      var points = new Vector3[lineRenderer.positionCount];

      for(int i = 0; i < points.Length; i++)
      {
        var rad = Mathf.Deg2Rad * i;
        points[i] = new Vector3(Mathf.Cos(rad) * radius,0,Mathf.Sin(rad) * radius);
      }

       lineRenderer.SetPositions(points);
    }

}



public class PlayerController : MonoBehaviour, IPausable
{
    private Rigidbody m_Rb;
    private GameObject m_Elevator;
    private float m_ElevatorOffsetY;
    public Camera followCamera;
    private Vector3 m_CameraPos;
    private float m_SpeedModifier;

    public UnityEvent OnPlayerLost;


    private void Awake()
    {
        m_Rb = GetComponent<Rigidbody>();
        m_ElevatorOffsetY = 0;
        m_SpeedModifier = 1;

        m_CameraPos = followCamera.transform.position - m_Rb.position;
        enabled = false;
    }
    public float speed;
    // Update is called once per frame
    void FixedUpdate()
    {
        if(transform.position.y <= -15.0f)
        {
             OnPlayerLost.Invoke();
        }

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 playerPos = m_Rb.position;

        Vector3 movement = new Vector3(horizontalInput,0,verticalInput).normalized;
        if (movement == Vector3.zero)
        {
            return;
        }
        Quaternion targetRotation = Quaternion.LookRotation(movement);
     
        if (m_Elevator != null)
        {
            playerPos.y = m_Elevator.transform.position.y + m_ElevatorOffsetY;
        }

        targetRotation = Quaternion.RotateTowards(transform.rotation, 
        targetRotation, 
        360 * Time.fixedDeltaTime);

        m_Rb.MovePosition(playerPos + movement * m_SpeedModifier * speed * Time.fixedDeltaTime);
        m_Rb.MoveRotation(targetRotation);

        
    }
    private void LateUpdate()
    {
        followCamera.transform.position = m_Rb.position + m_CameraPos;
    }

    public void OnGameStart()
    {
        enabled = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PowerUP"))
        {
            Destroy(collision.gameObject);
            m_SpeedModifier = 2;
            StartCoroutine(BonusSpeedCount());
        }
        if (collision.gameObject.CompareTag("Enemy") && m_SpeedModifier > 1)
        {
            Rigidbody EnemyRb = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = collision.transform.position - transform.position;
            EnemyRb.AddForce(awayFromPlayer * 20.0f, ForceMode.Impulse);
        }
    }

    private IEnumerator BonusSpeedCount()
    {
        yield return new WaitForSeconds(20.0f);
        m_SpeedModifier = 1;

    }

    private void OnTriggerEnter(Collider other)
    {
       if (other.CompareTag("Elevator"))
       {
        m_Elevator = other.gameObject;
        m_ElevatorOffsetY = transform.position.y - m_Elevator.transform.position.y;
       }

        
        
       
    }

    private void OnTriggerExit (Collider other)
    {
        if (other.CompareTag("Elevator"))
       {
        m_Elevator = null;
        m_ElevatorOffsetY = 0;
        
        
       }
    }
}
