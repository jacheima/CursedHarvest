using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public KeyCode up;
    public KeyCode down;
    public KeyCode left;
    public KeyCode right;

    public KeyCode attack;

    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;

    public float movementSpeed;

    private void Update()
    {
        if(Input.GetKey(up))
        {
            transform.position += new Vector3(0f, 1f, 0f) * movementSpeed * Time.deltaTime;
            this.GetComponent<SpriteRenderer>().sprite = upSprite;
        }

        if(Input.GetKey(down))
        {
            transform.position += new Vector3(0f, -1f, 0f) * movementSpeed * Time.deltaTime;
            this.GetComponent<SpriteRenderer>().sprite = downSprite;
        }

        if(Input.GetKey(left))
        {
            transform.position += new Vector3(-1f, 0f, 0f) * movementSpeed * Time.deltaTime;
            this.GetComponent<SpriteRenderer>().sprite = leftSprite;
        }

        if(Input.GetKey(right))
        {
            transform.position += new Vector3(1f, 0f, 0f) * movementSpeed * Time.deltaTime;
            this.GetComponent<SpriteRenderer>().sprite = rightSprite;
        }
    }

}
