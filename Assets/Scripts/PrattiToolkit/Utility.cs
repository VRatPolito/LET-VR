/* File Utility C# implementation of class Utility */



// global declaration start


using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Component = UnityEngine.Component;
using Math = System.Math;
using Random = UnityEngine.Random;

// global declaration end
namespace PrattiToolkit
{
    public static class Utility
    {
        public static void LogFormatted(string format, params object[] args)
        {
            Log(string.Format(format, args));
        }

        public static void Log(string arg)
        {
            //if(UnityEngine.Debug.isDebugBuild)
            {
#if UNITY_EDITOR || !UNITY_WSA

                StackFrame frame = new StackFrame(1);

                var method = frame.GetMethod();
                var type = method.DeclaringType;
                var name = method.Name;

                UnityEngine.Debug.Log(type + "::" + name + " " + arg);

#else

    	UnityEngine.Debug.Log(arg);
    
#endif
            }
        }

        public static void LogWarning(string arg)
        {
            //if(UnityEngine.Debug.isDebugBuild)
            {
#if UNITY_EDITOR || !UNITY_WSA

                StackFrame frame = new StackFrame(1);

                var method = frame.GetMethod();
                var type = method.DeclaringType;
                var name = method.Name;

                UnityEngine.Debug.LogWarning(type + "::" + name + " " + arg);

#else

    	UnityEngine.Debug.LogWarning(arg);
    
#endif
            }
        }

        public static void LogError(string arg)
        {
            //if(UnityEngine.Debug.isDebugBuild)
            {
#if UNITY_EDITOR || !UNITY_WSA

                StackFrame frame = new StackFrame(1);

                var method = frame.GetMethod();
                var type = method.DeclaringType;
                var name = method.Name;

                UnityEngine.Debug.LogError(type + "::" + name + " " + arg);

#else

    	UnityEngine.Debug.LogError(arg);
    
#endif
            }
        }
    }

    public class Enum<T> where T : struct, IConvertible
    {
        public static T GetRandomEnum()
        {
#if UNITY_WSA
        if (!typeof(T).IsEnum())
#else
            if (!typeof(T).IsEnum)
#endif
                throw new ArgumentException(String.Format("Argumnent {0} is not an Enum", typeof(T).FullName));

            List<T> lis = ((T[]) Enum.GetValues(typeof(T))).ToList();
            lis.Sort();
            var A = lis.ToArray();
            T V = (T) A.GetValue(UnityEngine.Random.Range(0, A.Length));
            return V;
        }

        public static T GetRandomEnum(T avoidValue)
        {
            var v = GetRandomEnum();
            while (v.Equals(avoidValue))
                v = GetRandomEnum();
            return v;
        }
    }

    public static class EnumExtender
    {
        public static List<T> GetValuesList<T>(this T src) where T : struct
        {
#if UNITY_WSA
        if (!typeof(T).IsEnum())
#else
            if (!typeof(T).IsEnum)
#endif
                throw new ArgumentException(String.Format("Argumnent {0} is not an Enum", typeof(T).FullName));
            List<T> lis = ((T[]) Enum.GetValues(src.GetType())).ToList();
            lis.Sort();
            return lis;
        }

        /// <summary>
        /// Iterate over enum values
        /// </summary>
        /// <param name="action">Action called on each enum value</param>
        public static void ForEach<T>(this T src, Action<T> action) where T : struct
        {
#if UNITY_WSA
        if (!typeof(T).IsEnum())
#else
            if (!typeof(T).IsEnum)
#endif
                throw new ArgumentException(String.Format("Argumnent {0} is not an Enum", typeof(T).FullName));
            List<T> lis = ((T[]) Enum.GetValues(src.GetType())).ToList();
            lis.Sort();
            foreach (var item in lis)
                action(item);
        }

        /// <summary>
        /// Get Next enum value ordered
        /// </summary>
        /// <typeparam name="T">Enum</typeparam>
        /// <param name="isCircular">When last value is reached if true go to the first one</param>
        /// <returns>Next enum value ordered</returns>
        public static T Next<T>(this T src, bool isCircular = false) where T : struct
        {
#if UNITY_WSA
        if (!typeof(T).IsEnum())
#else
            if (!typeof(T).IsEnum)
#endif
                throw new ArgumentException(String.Format("Argumnent {0} is not an Enum", typeof(T).FullName));

            List<T> lis = ((T[]) Enum.GetValues(src.GetType())).ToList();
            lis.Sort();
            T[] Arr = lis.ToArray();
            int j = Array.IndexOf<T>(Arr, src) + 1;
            return (Arr.Length == j) ? Arr[isCircular ? 0 : Arr.Length - 1] : Arr[j];
        }

        /// <summary>
        /// Get Previous enum value ordered
        /// </summary>
        /// <typeparam name="T">Enum</typeparam>
        /// <param name="isCircular">When first value is reached if true go to the last one</param>
        /// <returns>Previous enum value ordered</returns>
        public static T Prev<T>(this T src, bool isCircular = false) where T : struct
        {
#if UNITY_WSA
        if (!typeof(T).IsEnum())
#else
            if (!typeof(T).IsEnum)
#endif
                throw new ArgumentException(String.Format("Argumnent {0} is not an Enum", typeof(T).FullName));

            List<T> lis = ((T[]) Enum.GetValues(src.GetType())).ToList();
            lis.Sort();
            T[] Arr = lis.ToArray();
            int j = Array.IndexOf<T>(Arr, src) - 1;
            return (j < 0) ? Arr[isCircular ? Arr.Length - 1 : 0] : Arr[j];
        }

        /// <summary>
        /// Parse string to enum value
        /// </summary>
        /// <typeparam name="T">Enum</typeparam>
        /// <typeparam name="defaultValue">default value</typeparam>
        /// <returns>Parsed Value or default if string is invalid</returns>
        public static T ToEnum<T>(this string value, T defaultValue = default(T)) where T : struct
        {
#if UNITY_WSA
        if (!typeof(T).IsEnum())
#else
            if (!typeof(T).IsEnum)
#endif
                throw new ArgumentException(String.Format("Argumnent {0} is not an Enum", typeof(T).FullName));

            if (string.IsNullOrEmpty(value))
                return defaultValue;
            return (T) Enum.Parse(typeof(T), value, true);
        }
    }

    public static class GeneralExtensionMethods
    {
        public static bool IsNull(this object o)
        {
            return o == null;
        }

        public static bool IsNotNull(this object o)
        {
            return o != null;
        }
    }

    public static class UnityExtender
    {
        #region Vector3

        public static Vector3 ZeroX(this Vector3 v)
        {
            v.x = 0f;
            return v;
        }

        public static Vector3 ZeroY(this Vector3 v)
        {
            v.y = 0f;
            return v;
        }

        public static Vector3 ZeroZ(this Vector3 v)
        {
            v.z = 0f;
            return v;
        }

        /// Returns a Vector3 with only the X and Z components (Y is 0'd)
        public static Vector3 vector3XZOnly(this Vector3 vec)
        {
            return new Vector3(vec.x, 0f, vec.z);
        }

        #endregion

        #region Transform

        public static List<Transform> GetChildren(this Transform t)
        {
            var childrenList = new List<Transform>();
            if (t == null) return null;
            for (int i = 0; i < t.childCount; ++i)
            {
                childrenList.Add(t.GetChild(i));
            }

            return childrenList;
        }

        /// <summary>
        /// walk hierarchy looking for named transform
        /// </summary>
        /// <param name="name">name to look for</param>
        /// <returns>returns found transform or null if none found</returns>
        public static Transform GetChildRecursive(this Transform t, string name)
        {
            int numChildren = t.childCount;
            for (int i = 0; i < numChildren; ++i)
            {
                Transform child = t.GetChild(i);
                if (child.name == name)
                {
                    return child;
                }

                Transform foundIt = child.GetChildRecursive(name);
                if (foundIt != null)
                {
                    return foundIt;
                }
            }

            return null;
        }

        /// <summary>
        /// Check is this mesh is in camera view frustum.
        /// </summary>
        /// <param name="camera">Observer Camera</param>
        /// <returns>true if this mesh is within the Camera view frustum</returns>
        public static bool IsTargetVisible(this Transform src, Camera camera)
        {
            Vector3 targetViewportPosition = camera.WorldToViewportPoint(src.position);
            return (targetViewportPosition.x > 0.0 && targetViewportPosition.x < 1 &&
                    targetViewportPosition.y > 0.0 && targetViewportPosition.y < 1 &&
                    targetViewportPosition.z > 0);
        }

        public static Transform GetNearestTransform(this Transform transform, Transform[] transforms)
        {
            Transform tMin = null;
            float minDist = Mathf.Infinity;
            Vector3 currentPos = transform.position;
            foreach (Transform t in transforms)
            {
                float dist = Vector3.Distance(t.position, currentPos);
                if (dist < minDist)
                {
                    tMin = t;
                    minDist = dist;
                }
            }

            return tMin;
        }

        #endregion

        #region Rigidbody

        /// <summary>
        /// Changes the direction of a rigidbody without changing its speed.
        /// </summary>
        /// <param name="rigidbody">Rigidbody.</param>
        /// <param name="direction">New direction.</param>
        public static void ChangeDirection(this Rigidbody rigidbody, Vector3 direction)
        {
            var newVelocity = direction * rigidbody.velocity.magnitude;
            rigidbody.velocity = newVelocity;
        }

        /// <summary>
        /// Must be called in FixedUpdate 
        /// </summary>
        /// <param name="direction">local position direction of movement</param>
        public static void LocalPositionConstraints(this Rigidbody rigidbody, bool freezeX, bool freezeY, bool freezeZ)
        {

            //var local = rigidbody.transform.localPosition;

            //if (freezeX) local.x = 0.0f;
            //if (freezeY) local.y = 0.0f;
            //if (freezeZ) local.z = 0.0f;

            //rigidbody.transform.localPosition = local;

            Vector3 velocity = rigidbody.transform.InverseTransformDirection(rigidbody.velocity);
            Vector3 localPosition = rigidbody.transform.InverseTransformPoint(rigidbody.position);
            if (freezeX) localPosition.x = 0.0f;
            if (freezeY) localPosition.y = 0.0f;
            if (freezeZ) localPosition.z = 0.0f;

            if (freezeX) velocity.x = 0;
            if (freezeY) velocity.y = 0;
            if (freezeZ) velocity.z = 0;
            rigidbody.velocity = rigidbody.transform.TransformDirection(velocity);
            var target = rigidbody.transform.TransformPoint(localPosition);

            Vector3 distance = target - rigidbody.transform.position;
            Vector3 idealPosition = target - rigidbody.transform.forward * distance.magnitude;
            Vector3 correction = idealPosition - rigidbody.transform.position;

            correction = rigidbody.transform.InverseTransformDirection(correction);
            if (!freezeX) correction.x = 0;
            if (!freezeY) correction.y = 0;
            if (!freezeZ) correction.z = 0;
            correction.z = 0;
            correction = rigidbody.transform.TransformDirection(correction);

            rigidbody.velocity += correction;

        }

        #endregion

        #region GameObject

        /// <summary>
        /// Gets a component attached to the given game object.
        /// If one isn't found, a new one is attached and returned.
        /// </summary>
        /// <param name="gameObject">Game object.</param>
        /// <returns>Previously or newly attached component.</returns>
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            var newOrExistingComponent = gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
            return newOrExistingComponent;
        }

        /// <summary>
        /// Checks whether a game object has a component of type T attached.
        /// </summary>
        /// <param name="gameObject">Game object.</param>
        /// <returns>True when component is attached.</returns>
        public static bool HasComponent<T>(this GameObject gameObject) where T : Component
        {
            var hasComponent = gameObject.GetComponent<T>() != null;
            return hasComponent;
        }

        /// <summary>
        /// Enables or disables the collider on the calling object and all its children.
        /// </summary>
        public static void SetCollidersEnabledRecursively(this GameObject gameObject, bool enabled)
        {
            Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
            foreach (Collider collider in colliders)
            {
                collider.enabled = enabled;
            }
        }

        /// <summary>
        /// Enables or disables the renderer on the calling object and all its children.
        /// </summary>
        public static void SetRenderersEnabledRecursively(this GameObject gameObject, bool enabled)
        {
            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = enabled;
            }
        }

        #endregion

        #region Component

        /// <summary>
        /// Attaches a component to the given component's game object.
        /// </summary>
        /// <param name="component">Component.</param>
        /// <returns>Newly attached component.</returns>
        public static T AddComponent<T>(this Component component) where T : Component
        {
            var newComponent = component.gameObject.AddComponent<T>();
            return newComponent;
        }

        /// <summary>
        /// Gets a component attached to the given component's game object.
        /// If one isn't found, a new one is attached and returned.
        /// </summary>
        /// <param name="component">Component.</param>
        /// <returns>Previously or newly attached component.</returns>
        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            var newOrExistingComponent = component.GetComponent<T>() ?? component.AddComponent<T>();
            return newOrExistingComponent;
        }

        /// <summary>
        /// Checks whether a component's game object has a component of type T attached.
        /// </summary>
        /// <param name="component">Component.</param>
        /// <returns>True when component is attached.</returns>
        public static bool HasComponent<T>(this Component component) where T : Component
        {
            var hasComponent = component.GetComponent<T>() != null;
            return hasComponent;
        }

        #endregion

        #region UI

        public static Vector2 SizeToParent(this RawImage image, float padding = 0)
        {
            float w = 0, h = 0;
            var parent = image.GetComponentInParent<RectTransform>();
            var imageTransform = image.GetComponent<RectTransform>();

            // check if there is something to do
            if (image.texture != null)
            {
                if (!parent)
                {
                    return imageTransform.sizeDelta;
                } //if we don't have a parent, just return our current width;

                padding = 1 - padding;
                float ratio = image.texture.width / (float) image.texture.height;
                var bounds = new Rect(0, 0, parent.rect.width, parent.rect.height);
                if (Mathf.RoundToInt(imageTransform.eulerAngles.z) % 180 == 90)
                {
                    //Invert the bounds if the image is rotated
                    bounds.size = new Vector2(bounds.height, bounds.width);
                }

                //Size by height first
                h = bounds.height * padding;
                w = h * ratio;
                if (w > bounds.width * padding)
                {
                    //If it doesn't fit, fallback to width;
                    w = bounds.width * padding;
                    h = w / ratio;
                }
            }

            imageTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
            imageTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
            return imageTransform.sizeDelta;
        }

        /// <summary>
        /// Get the current active Toggle in this ToggleGroup
        /// </summary>
        /// <returns>Active toggle if there is one, null otherwise</returns>
        public static Toggle GetActiveToggle(this ToggleGroup toggleGroup)
        {
            if (toggleGroup != null && toggleGroup.AnyTogglesOn())
                return toggleGroup.ActiveToggles().FirstOrDefault();

            return null;
        }

        /// <summary>
        /// Get the current active Toggle in this ToggleGroup if toggle are children
        /// Not Recursive!!!!
        /// </summary>
        /// <returns>Active toggle if there is one, null otherwise</returns>
        public static Toggle GetActiveToggleInChildren(this ToggleGroup toggleGroup)
        {
            if (toggleGroup == null) return null;
            return toggleGroup.GetComponentsInChildren<Toggle>().FirstOrDefault(toggle => toggle.isOn);
        }

        #endregion

        #region Mesh

        public static void FillMesh(this Mesh mesh)
        {
            var vertices = mesh.vertices;
            var uv = mesh.uv;
            var normals = mesh.normals;
            var szV = vertices.Length;
            var newVerts = new Vector3[szV * 2];
            var newUv = new Vector2[szV * 2];
            var newNorms = new Vector3[szV * 2];
            for (var j = 0; j < szV; j++)
            {
                // duplicate vertices and uvs:
                newVerts[j] = newVerts[j + szV] = vertices[j];
                newUv[j] = newUv[j + szV] = uv[j];
                // copy the original normals...
                newNorms[j] = normals[j];
                // and revert the new ones
                newNorms[j + szV] = -normals[j];
            }
            var triangles = mesh.triangles;
            var szT = triangles.Length;
            var newTris = new int[szT * 2]; // double the triangles
            for (var i = 0; i < szT; i += 3)
            {
                // copy the original triangle
                newTris[i] = triangles[i];
                newTris[i + 1] = triangles[i + 1];
                newTris[i + 2] = triangles[i + 2];
                // save the new reversed triangle
                var j = i + szT;
                newTris[j] = triangles[i] + szV;
                newTris[j + 2] = triangles[i + 1] + szV;
                newTris[j + 1] = triangles[i + 2] + szV;
            }
            mesh.vertices = newVerts;
            mesh.uv = newUv;
            mesh.normals = newNorms;
            mesh.triangles = newTris; // assign triangles last!
        }

        #endregion

        #region Various

        /// <summary>
        /// Get Vector3 from Vector2 X, Y, and Z=0
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetZFlatVector3(this Vector2 src)
        {
            return new Vector3(src.x, src.y, 0);
        }

        public static bool NearlyEqual(Vector3 a, Vector3 b, float epsilon = float.Epsilon)
        {
            return NearlyEqual(a.x, b.x, epsilon) && NearlyEqual(a.y, b.y, epsilon) && NearlyEqual(a.z, b.z, epsilon);
        }
        public static bool NearlyEqual(Quaternion a, Quaternion b, float epsilon = float.Epsilon)
        {
            return NearlyEqual(a.x, b.x, epsilon) && NearlyEqual(a.y, b.y, epsilon) && NearlyEqual(a.z, b.z, epsilon) && NearlyEqual(a.w, b.w, epsilon);
        }
        public static float[] GetFloatArray(this Color c)
        {
            return new[] {c.r, c.g, c.b, c.a};
        }

        public static bool NearlyEqual(float a, float b, float epsilon = float.Epsilon)
        {
            float absA = Math.Abs(a);
            float absB = Math.Abs(b);
            float diff = Math.Abs(a - b);

            if (a == b)
            {
                return true;
            }
            else if (a == 0 || b == 0 || diff < epsilon)
            {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < epsilon;
            }
            else
            { // use relative error
                return diff / (absA + absB) < epsilon;
            }
        }

        public static Color GetColor(this float[] colorFloatArray)
        {
            if (colorFloatArray.Length != 4)
                return Color.black;

            return new Color(colorFloatArray[0], colorFloatArray[1], colorFloatArray[2], colorFloatArray[3]);

        }

        #endregion

    }

    public static class AndroidUtils
    {
        public static string GetDeviceNameJNI()
        {
#if UNITY_EDITOR
            return "EDITOR";
#else
        var clsUnity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var objActivity = clsUnity.GetStatic<AndroidJavaObject>("currentActivity");
        var objResolver = objActivity.Call<AndroidJavaObject>("getContentResolver");
        var clsSecure = new AndroidJavaClass("android.provider.Settings$Secure");
        return clsSecure.CallStatic<string>("getString", objResolver, "bluetooth_name");
#endif
        }
    }

    public static class NumericExtender
    {
        /// <summary>
        /// Remap this value from a range to another one
        /// </summary>
        /// <param name="fromMin">minimum value of source range</param>
        /// <param name="fromMax">maximum value of source range</param>
        /// <param name="toMin">minimum value of destination range</param>
        /// <param name="toMax">maximum value of destination range</param>
        /// <returns>remapped value</returns>
        public static float Remap(this int value, float fromMin, float fromMax, float toMin, float toMax)
        {
            return ((float) value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
        }

        /// <summary>
        /// Remap this value from a range to another one
        /// </summary>
        /// <param name="fromMin">minimum value of source range</param>
        /// <param name="fromMax">maximum value of source range</param>
        /// <param name="toMin">minimum value of destination range</param>
        /// <param name="toMax">maximum value of destination range</param>
        /// <returns>remapped value</returns>
        public static float Remap(this float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
        }

        /// <summary>
        /// Remap this value from a range to another one
        /// </summary>
        /// <param name="fromMin">minimum value of source range</param>
        /// <param name="fromMax">maximum value of source range</param>
        /// <param name="toMin">minimum value of destination range</param>
        /// <param name="toMax">maximum value of destination range</param>
        /// <returns>remapped value</returns>
        public static double Remap(this double value, double fromMin, double fromMax, double toMin, double toMax)
        {
            return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
        }

        /// <summary>
        /// Calculate Signal to Noise Ratio (SNR)
        /// </summary>
        /// <param name="signal">signal component</param>
        /// <param name="noise">noise related</param>
        /// <returns>dB</returns>
        public static double SNR(this float signal, float noise)
        {
            if (signal <= noise) return double.NegativeInfinity;
            return 10 * Math.Log10(Math.Abs(signal / noise));
        }

        /// <summary>
        /// Calculate Signal to Noise Ratio (SNR)
        /// </summary>
        /// <param name="signal">signal component</param>
        /// <param name="noise">noise related</param>
        /// <returns>dB</returns>
        public static double SNR(this double signal, double noise)
        {
            if (signal <= noise) return double.NegativeInfinity;
            return 10 * Math.Log10(Math.Abs(signal / noise));
        }
    }

    public static class StringExtender
    {
#if !UNITY_WSA
        /// <summary>
        /// Parse value,can throw exceptions
        /// </summary>
        /// <typeparam name="T">Target Type</typeparam>
        /// <returns></returns>
        public static T UnsafeParse<T>(this string value)
        {
            T result = default(T);
            if (!string.IsNullOrEmpty(value))
            {
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(T));
                result = (T) tc.ConvertFrom(value);
            }

            return result;
        }
#endif
        public static bool IsValidIPAddress(this string s)
        {
            return Regex.IsMatch(s,
                @"\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b");
        }

    }

    public static class EnumerableExtensions
    {
        /// <summary>
        /// Returns the index of the first occurrence in a sequence by using the default equality comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="list">A sequence in which to locate a value.</param>
        /// <param name="value">The object to locate in the sequence</param>
        /// <returns>The zero-based index of the first occurrence of value within the entire sequence, if found; otherwise, –1.</returns>
        public static int IndexOf<TSource>(this IEnumerable<TSource> list, TSource value)
            where TSource : IEquatable<TSource>
        {

            return list.IndexOf<TSource>(value, EqualityComparer<TSource>.Default);

        }

        /// <summary>
        /// Returns the index of the first occurrence in a sequence by using a specified IEqualityComparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="list">A sequence in which to locate a value.</param>
        /// <param name="value">The object to locate in the sequence</param>
        /// <param name="comparer">An equality comparer to compare values.</param>
        /// <returns>The zero-based index of the first occurrence of value within the entire sequence, if found; otherwise, –1.</returns>
        public static int IndexOf<TSource>(this IEnumerable<TSource> list, TSource value,
            IEqualityComparer<TSource> comparer)
        {
            int index = 0;
            foreach (var item in list)
            {
                if (comparer.Equals(item, value))
                {
                    return index;
                }

                index++;
            }

            return -1;
        }
    }

    public static class ActionExtensions
    {
        public static void RaiseEvent(this Action action)
        {
            if (action != null)
            {
                action();
            }
        }

        public static void RaiseEvent<T>(this Action<T> action, T arg)
        {
            if (action != null)
            {
                action(arg);
            }
        }

        public static void RaiseEvent<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            if (action != null)
            {
                action(arg1, arg2);
            }
        }

        public static void RaiseEvent<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
        {
            if (action != null)
            {
                action(arg1, arg2, arg3);
            }
        }

        public static void RaiseEvent<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3,
            T4 arg4)
        {
            if (action != null)
            {
                action(arg1, arg2, arg3, arg4);
            }
        }
    }

    public class CircularList<T> : IEnumerable<T>, IEnumerator<T>
    {
        protected T[] items;
        protected int idx;
        protected bool loaded;
        protected int enumIdx;

        /// <summary>
        /// Constructor that initializes the list with the 
        /// required number of items.
        /// </summary>
        public CircularList(int numItems)
        {
            if (numItems <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    "numItems can't be negative or 0.");
            }

            items = new T[numItems];
            idx = 0;
            loaded = false;
            enumIdx = -1;
        }

        /// <summary>
        /// Gets/sets the item value at the current index.
        /// </summary>
        public T Value
        {
            get { return items[idx]; }
            set { items[idx] = value; }
        }

        /// <summary>
        /// Returns the count of the number of loaded items, up to
        /// and including the total number of items in the collection.
        /// </summary>
        public int Count
        {
            get { return loaded ? items.Length : idx; }
        }

        /// <summary>
        /// Returns the length of the items array.
        /// </summary>
        public int Length
        {
            get { return items.Length; }
        }

        /// <summary>
        /// Gets/sets the value at the specified index.
        /// </summary>
        public T this[int index]
        {
            get
            {
                RangeCheck(index);
                return items[index];
            }
            set
            {
                RangeCheck(index);
                items[index] = value;
            }
        }

        /// <summary>
        /// Advances to the next item or wraps to the first item.
        /// </summary>
        public void Next()
        {
            if (++idx == items.Length)
            {
                idx = 0;
                loaded = true;
            }
        }

        /// <summary>
        /// Clears the list, resetting the current index to the 
        /// beginning of the list and flagging the collection as unloaded.
        /// </summary>
        public void Clear()
        {
            idx = 0;
            items.Initialize();
            loaded = false;
        }

        /// <summary>
        /// Sets all items in the list to the specified value, resets
        /// the current index to the beginning of the list and flags the
        /// collection as loaded.
        /// </summary>
        public void SetAll(T val)
        {
            idx = 0;
            loaded = true;

            for (int i = 0; i < items.Length; i++)
            {
                items[i] = val;
            }
        }

        /// <summary>
        /// Internal indexer range check helper. Throws
        /// ArgumentOutOfRange exception if the index is not valid.
        /// </summary>
        protected void RangeCheck(int index)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(
                    "Indexer cannot be less than 0.");
            }

            if (index >= items.Length)
            {
                throw new ArgumentOutOfRangeException(
                    "Indexer cannot be greater than or equal to the number of items in the collection.");
            }
        }

        // Interface implementations:

        public IEnumerator<T> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        public T Current
        {
            get { return this[enumIdx]; }
        }

        public void Dispose()
        {
        }

        object IEnumerator.Current
        {
            get { return this[enumIdx]; }
        }

        public bool MoveNext()
        {
            ++enumIdx;
            bool ret = enumIdx < Count;

            if (!ret)
            {
                Reset();
            }

            return ret;
        }

        public void Reset()
        {
            enumIdx = -1;
        }
    }
}