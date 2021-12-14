using UnityEngine;
using System.Collections;

/// MouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character:
/// - Create a capsule.
/// - Add the MouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSInputController script to the capsule
///   -> A CharacterMotor and a CharacterController component will be automatically added.

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
/// - Add a MouseLook script to the camera.
///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour
{

    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;

    public bool ClampX;
    public float ClampXValue = 70f;

    public float minimumY = -60F;
    public float maximumY = 60F;


    float rotationY = 0F;

    void Update()
    {

        var isLeft = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
        var isRight = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
        var isUp = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
        var isDown = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);

        var mouseViewPoint = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        var border = 0.05f;

        if (mouseViewPoint.x < border)
            isLeft = true;

        if (mouseViewPoint.x > 1 - border)
            isRight = true;

        if (mouseViewPoint.y < border)
            isDown = true;

        if (mouseViewPoint.y > 1 - border)
            isUp = true;

        if (!Input.GetMouseButton(1) && !Input.GetKey(KeyCode.LeftControl) &&
            !isLeft && !isRight && !isUp && !isDown)
            return;

        var sensitivity = PlayerPrefs.HasKey("timesnap_camera_sensitivity") ? PlayerPrefs.GetFloat("timesnap_camera_sensitivity") : 1;
        var min = 0.5f;
        var max = 5f;

        var range = max - min;
        var modifiedRange = range * sensitivity;
        var realSensitivity = min + modifiedRange;

        var panSpeed = 0.25f;
        var xAxis = isLeft ? -panSpeed : isRight ? panSpeed : Input.GetMouseButton(1) ? Input.GetAxis("Mouse X") : 0;
        var yAxis = isUp ? panSpeed : isDown ? -panSpeed : Input.GetMouseButton(1) ? Input.GetAxis("Mouse Y") : 0;

        if (axes == RotationAxes.MouseXAndY)
        {
            float rotationX = transform.localEulerAngles.y + xAxis * realSensitivity;

            if (ClampX)
            {
                if (rotationX > 180)
                    rotationX -= 360;

                rotationX = Mathf.Clamp(rotationX, -ClampXValue, ClampXValue);
            }

            rotationY += yAxis * realSensitivity;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
        }
        else if (axes == RotationAxes.MouseX)
        {
            transform.Rotate(0, xAxis * realSensitivity, 0);
        }
        else
        {
            rotationY += yAxis * realSensitivity;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
        }
    }

    void Start()
    {
        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;
    }
}