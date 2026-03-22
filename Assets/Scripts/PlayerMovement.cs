using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("이동")]
    public float moveSpeed = 5f;

    [Header("가상 조이스틱")]
    public VirtualJoystick joystick;

    [Header("맵 경계 (벽 안쪽 기준)")]
    public float minX = 1.5f;
    public float maxX = 38.5f;
    public float minY = 1.5f;
    public float maxY = 28.5f;

    void Update()
    {
        Vector2 dir = GetKeyboardInput();

        if (dir == Vector2.zero && joystick != null)
            dir = joystick.Direction;

        Vector3 move = new Vector3(dir.x, dir.y, 0f);
        if (move.sqrMagnitude > 1f) move.Normalize();

        transform.Translate(move * moveSpeed * Time.deltaTime);

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
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
