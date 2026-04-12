using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("이동")]
    public float moveSpeed = 5f;

    [Header("가상 조이스틱")]
    public VirtualJoystick joystick;

    private Rigidbody2D rb;
    private Vector2 moveDir;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moveDir = GetKeyboardInput();

        if (moveDir == Vector2.zero && joystick != null)
            moveDir = joystick.Direction;

        if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);
    }

    Vector2 GetKeyboardInput()
    {
        if (Keyboard.current == null) return Vector2.zero;

        float h = 0f, v = 0f;
        if (Keyboard.current.aKey.isPressed) h -= 1f;
        if (Keyboard.current.dKey.isPressed) h += 1f;
        if (Keyboard.current.sKey.isPressed) v -= 1f;
        if (Keyboard.current.wKey.isPressed) v += 1f;

        return new Vector2(h, v);
    }
}
