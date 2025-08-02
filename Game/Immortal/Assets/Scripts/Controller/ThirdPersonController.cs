using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("移动设置")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 15f;

    [Header("相机设置")]
    [SerializeField] private Transform mainCamera;
    [SerializeField] private float cameraDistance = 5f;
    [SerializeField] private float cameraHeight = 2f;
    [SerializeField] private float cameraSmoothSpeed = 0.1f;

    private CharacterController characterController;
    private Vector2 movementInput;
    private Vector3 moveDirection;
    private Vector3 cameraTargetPosition;

    private void Awake()
    {
        // 获取角色控制器组件
        characterController = GetComponent<CharacterController>();

        // 如果未指定相机，则使用主相机
        if (mainCamera == null)
        {
            mainCamera = Camera.main.transform;
        }
    }

    private void Update()
    {
        HandleMovement();
        HandleCameraFollow();
    }

    // 处理移动输入
    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    // 处理角色移动
    private void HandleMovement()
    {
        if (movementInput.sqrMagnitude < 0.1f)
        {
            // 如果没有输入，不移动
            return;
        }

        // 根据相机方向计算移动方向
        Vector3 forward = mainCamera.TransformDirection(Vector3.forward);
        Vector3 right = mainCamera.TransformDirection(Vector3.right);

        // 忽略Y轴，确保在平面上移动
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // 计算最终移动方向
        moveDirection = forward * movementInput.y + right * movementInput.x;
        moveDirection.Normalize();

        // 移动角色
        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);

        // 旋转角色面向移动方向
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    // 处理相机跟随
    private void HandleCameraFollow()
    {
        if (mainCamera == null) return;

        // 计算相机目标位置（角色后方一定距离和高度）
        Vector3 desiredPosition = transform.position - transform.forward * cameraDistance + Vector3.up * cameraHeight;

        // 平滑移动相机到目标位置
        mainCamera.position = Vector3.Lerp(mainCamera.position, desiredPosition, cameraSmoothSpeed);

        // 让相机始终看向角色
        mainCamera.LookAt(transform.position + Vector3.up * cameraHeight * 0.5f);
    }

    // 确保角色控制器在场景视图中可见
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
