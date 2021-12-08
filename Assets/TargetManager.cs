using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TargetManager : MonoBehaviour
{
    public static VRButton Target { get; private set; }
    private static Dictionary<Collider, Vector3> _collisionPoints = new Dictionary<Collider, Vector3>();

    public static bool IsPopupOpen;

    public static Vector3 GetCollisionPoint(Collider collider)
    {
        return _collisionPoints[collider];
    }

    // Update is called once per frame
    void Update()
    {
        
        var lastTarget = Target;
        Target = null;

        var ray = new Ray(Camera.main.transform.position, (EFS.Timesnap.VR.TimesnapVRPlayer.Instance.Pointer.transform.position - Camera.main.transform.position).normalized);

        var popup = FindObjectsOfType<Popup>().ToList().Find(x=>x.enabled);


        IsPopupOpen = popup != null;

        var vrbuttons = !IsPopupOpen ? FindObjectsOfType<VRButton>().ToList() : popup.GetComponentsInChildren<VRButton>().ToList();



        vrbuttons.ForEach(x =>
        {
            var collider = x.GetComponent<Collider>();

            Debug.Log("testing " + x + " for collider " + collider);

            if (collider == null)
                return;

            Debug.Log("   - testing for intersection");
            float distance = 0;
            if (collider.bounds.IntersectRay(ray, out distance))
            {
                Debug.Log("   - has intersection");
                Target = x;
                if (!Input.GetMouseButton(1))
                    x.OnPointerEnter(null);

                var collisionPoint = ray.origin + ray.direction * distance;

                if (!_collisionPoints.ContainsKey(collider))
                    _collisionPoints.Add(collider, collisionPoint);
                else
                    _collisionPoints[collider] = collisionPoint;

                
                if (!Input.GetMouseButton(1))
                {
                    if (Input.GetMouseButtonDown(0))
                        x.OnPointerDown(null);
                    else if (Input.GetMouseButtonUp(0))
                        x.OnPointerUp(null);
                }
            }            
        });

        if (lastTarget != null && lastTarget != Target)
            lastTarget.OnPointerExit(null);
    }
}
