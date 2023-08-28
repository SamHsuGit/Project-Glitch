using UnityEngine;
using UnityEngine.InputSystem;
public class InputHandler : MonoBehaviour
{
    [Header("public variables")]
    public Vector2 move;
    public Vector2 look;
    public Vector2 scrollWheel;
    public bool navUp;
    public bool navDown;
    public bool navLeft;
    public bool navRight;
    public bool jump = false;
    public bool shoot = false;
    public bool grenade = false;
    public bool melee = false;
    public bool switchPrimary = false;
    public bool switchSecondary = false;

    public void OnMove(InputAction.CallbackContext ctx) => move = ctx.ReadValue<Vector2>();
    public void OnLook(InputAction.CallbackContext ctx) => look = ctx.ReadValue<Vector2>();
    public void OnNavigateUp(InputAction.CallbackContext ctx) => navUp = ctx.performed;
    public void OnNavigateDown(InputAction.CallbackContext ctx) => navDown = ctx.performed;
    public void OnScrollWheel(InputAction.CallbackContext ctx) => scrollWheel = ctx.ReadValue<Vector2>();
    public void OnNavigateLeft(InputAction.CallbackContext ctx) => navLeft = ctx.performed;
    public void OnNavigateRight(InputAction.CallbackContext ctx) => navRight = ctx.performed;
    public void OnJump(InputAction.CallbackContext ctx) => jump = ctx.performed;
    public void OnShoot(InputAction.CallbackContext ctx) => shoot = ctx.performed;
    public void OnGrenade(InputAction.CallbackContext ctx) => grenade = ctx.performed;
    public void OnMelee(InputAction.CallbackContext ctx) => melee = ctx.performed;
    public void OnSwitchPrimary(InputAction.CallbackContext ctx) => switchPrimary = ctx.performed;
    public void OnSwitchSecondary(InputAction.CallbackContext ctx) => switchSecondary = ctx.performed;

}