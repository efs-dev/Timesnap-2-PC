using UnityEngine;
using UnityEngine.EventSystems;

namespace EFS.Timesnap.VR
{
    public class Spinnable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public float ScalingFactor = 1;
        public float MaximumAngleDeltaDegrees = 150;
        public float RotationLerpFactor = 0.2f;
        public float DragCoefficient = 0.8f;
        public bool ShouldAutoAlign = true;

        private bool _dragging;
        private Quaternion targetRotation;
        private Vector3 _cameraRotation;
        private Vector3 _lastMousePosition;
        private Vector3 _currentMousePosition;
        private Vector3 _mouseDelta;
        private Transform _targetTransform;
        private bool _lastRotationDirection;

        private Quaternion _originalRotation;

        public UnityEngine.Events.UnityEvent OnSpin;

        void Start()
        {
            _targetTransform = new GameObject("DragToRotate Intermediate Transform (" + gameObject.name + ")")
                .transform;
            _targetTransform.rotation = transform.rotation;
            _targetTransform.position = transform.position;
            _originalRotation = transform.rotation;

            GetComponent<VRButton>().ClickDown.AddListener(StartDrag);
        }

        public void Reset()
        {
            _dragging = false;
            _mouseDelta = Vector2.zero;
            if (_targetTransform != null)
            {
                _targetTransform.rotation = transform.rotation;
                _targetTransform.position = transform.position;
            }
        }

        void OnEnable()
        {
            if (_targetTransform == null) return;
            transform.rotation = _originalRotation;
            _targetTransform.rotation = transform.rotation;
            _targetTransform.position = transform.position;

        }

        void LateUpdate()
        {
            if (!enabled) return;

            if (_dragging && Input.GetMouseButtonUp(0))// OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger))
            {
                _dragging = false;//
                                  // Camera.main.transform.eulerAngles = _cameraRotation;
                OnSpin?.Invoke();
            }

            if (_dragging)
            {
                _currentMousePosition = GetPointerPosition();
                _mouseDelta = _currentMousePosition - _lastMousePosition;
              //  Camera.main.transform.eulerAngles = _cameraRotation;
                _lastMousePosition = _currentMousePosition;
            }
            else
            {
                _mouseDelta *= DragCoefficient;
            }

            var previousRotation = _targetTransform.rotation;
            _targetTransform.Rotate(_mouseDelta.y * ScalingFactor * 200, -_mouseDelta.x * ScalingFactor * 200, 0, Space.World);

            // todo: if the rotation changed direction but the mouseDelta didn't, then we should revert. 

            if (!_dragging && ShouldAutoAlign)
                _targetTransform.rotation = Quaternion.Lerp(_targetTransform.rotation,
                    Quaternion.Euler(0, _targetTransform.rotation.eulerAngles.y, 0), RotationLerpFactor);

            if (Mathf.Abs(Quaternion.Angle(transform.rotation, _targetTransform.rotation)) >= MaximumAngleDeltaDegrees)
            {
                _targetTransform.rotation = previousRotation;
            }

            transform.rotation = Quaternion.Lerp(transform.rotation, _targetTransform.rotation, RotationLerpFactor);
        }

        void StartDrag()
        {
            Debug.Log("start drag");
            _dragging = true;
            _lastMousePosition = GetPointerPosition();
            _cameraRotation = GetPointerPosition();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            //_dragging = true;
           // _lastMousePosition = GetPointerPosition();
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            //_dragging = false;
        }

        private Vector2 GetPointerPosition()
        {
            return EFS.Timesnap.VR.TimesnapVRPlayer.Instance.Pointer.transform.position;// Camera.main.transform.forward * 10f;// TargetManager.GetCollisionPoint(GetComponent<Collider>());
            //var rayEndpoint = FindObjectOfType<TimesnapVRPlayer>().GetActiveVrImplementation().RayEndpoint;
            //return Camera.main.WorldToScreenPoint(rayEndpoint);
        }
    }
}