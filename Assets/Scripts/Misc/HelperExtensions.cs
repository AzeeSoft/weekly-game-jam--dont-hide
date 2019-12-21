using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Cinemachine.Utility;
using UnityEngine;

public static class HelperExtensions
{
    #region MonoBehaviour

    public static void WaitAndExecute(this MonoBehaviour self, Action action, float delay)
    {
        self.StartCoroutine(HelperUtilities.WaitAndExecute(action, delay));
    }

    public static void WaitForFrameAndExecute(this MonoBehaviour self, Action action)
    {
        self.StartCoroutine(HelperUtilities.WaitForFrameAndExecute(action));
    }

    #endregion

    #region Transform

    public static Bounds TransformBounds(this Transform self, Bounds bounds)
    {
        var center = self.TransformPoint(bounds.center);
        var points = bounds.GetCorners();

        var result = new Bounds(center, Vector3.zero);
        foreach (var point in points)
            result.Encapsulate(self.TransformPoint(point));
        return result;
    }

    public static Bounds InverseTransformBounds(this Transform self, Bounds bounds)
    {
        var center = self.InverseTransformPoint(bounds.center);
        var points = bounds.GetCorners();

        var result = new Bounds(center, Vector3.zero);
        foreach (var point in points)
            result.Encapsulate(self.InverseTransformPoint(point));
        return result;
    }

    public static void RemoveAllChildren(this Transform self)
    {
        foreach (Transform child in self)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    #endregion

    #region Bounds

    public static List<Vector3> GetCorners(this Bounds obj, bool includePosition = true)
    {
        var result = new List<Vector3>();
        for (int x = -1; x <= 1; x += 2)
        for (int y = -1; y <= 1; y += 2)
        for (int z = -1; z <= 1; z += 2)
            result.Add((includePosition ? obj.center : Vector3.zero) + (obj.size / 2).Times(new Vector3(x, y, z)));
        return result;
    }

    #endregion

    #region Vector3

    public static Vector3 Times(this Vector3 self, Vector3 other)
    {
        return new Vector3(self.x * other.x, self.y * other.y, self.z * other.z);
    }

    #endregion

    #region CinemachinePOV

    public static void ResetRotation(this CinemachinePOV self, Quaternion targetRot)
    {
        Vector3 up = self.VcamState.ReferenceUp;
        Vector3 fwd = Vector3.forward;
        Transform parent = self.VirtualCamera.transform.parent;
        if (parent != null)
            fwd = parent.rotation * fwd;

        self.m_HorizontalAxis.Value = 0;
        self.m_HorizontalAxis.Reset();
        Vector3 targetFwd = targetRot * Vector3.forward;
        Vector3 a = fwd.ProjectOntoPlane(up);
        Vector3 b = targetFwd.ProjectOntoPlane(up);
        if (!a.AlmostZero() && !b.AlmostZero())
            self.m_HorizontalAxis.Value = Vector3.SignedAngle(a, b, up);

        self.m_VerticalAxis.Value = 0;
        self.m_VerticalAxis.Reset();
        fwd = Quaternion.AngleAxis(self.m_HorizontalAxis.Value, up) * fwd;
        Vector3 right = Vector3.Cross(up, fwd);
        if (!right.AlmostZero())
            self.m_VerticalAxis.Value = Vector3.SignedAngle(fwd, targetFwd, right);
    }

    #endregion
}