using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    private BlockMovement selectedBlock;
    private Camera mainCam;

    void Awake()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        if(Pointer.current == null) return;

        if(Pointer.current.press.wasPressedThisFrame)
        {
            Vector2 pointerPos = mainCam.ScreenToWorldPoint(Pointer.current.position.ReadValue());
            Collider2D hit = Physics2D.OverlapPoint(pointerPos);

            if(hit != null && hit.TryGetComponent<BlockMovement>(out selectedBlock))
            {
                selectedBlock.StartDrag(pointerPos);
            }
        }

        if(Pointer.current.press.isPressed && selectedBlock != null)
        {
            Vector2 pointerPos = mainCam.ScreenToWorldPoint(Pointer.current.position.ReadValue());
            selectedBlock.ProcessDrag(pointerPos);
        }

        if(Pointer.current.press.wasReleasedThisFrame && selectedBlock != null)
        {
            selectedBlock.EndDrag();
            selectedBlock = null;
        }

        if(Keyboard.current != null && Keyboard.current.uKey.wasPressedThisFrame)
        {
            MoveHistoryManager.Instance.Undo();
        }
        if(Keyboard.current.rKey.wasPressedThisFrame)
        {
            LevelSpawner.Instance.ResetLevel();
        }
        if(Keyboard.current.hKey.wasPressedThisFrame)
        {
            HintManager.Instance.ShowHint();
        }
    }
}