using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{

    public CharacterController controller;
    public float speed = 6.0f;

    public TextMeshProUGUI scoreUI;

    private int score = 0;

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;//normalising stops faster movement on the diagonal 

        if(direction.magnitude >= 0.1f)
        {
            controller.Move(direction * speed * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Pickup")
        {
            if (!collision.gameObject.GetComponent<Pickup>().IsCollected)
            {
                score++;
                scoreUI.text = ("SCORE: ") + score.ToString();
                collision.gameObject.GetComponent<Pickup>().GetPickedUp();
            }
        }
    }
}
