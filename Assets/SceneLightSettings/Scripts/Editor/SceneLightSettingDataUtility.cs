using UnityEngine;
using UnityEditor;
using System.IO;

namespace SceneLightSettings
{
    public static class DataUtility
    {
#region Convert & Get
        public static float[] ToFloatArray(this Vector2 vec2)
        {
            return new float[]{vec2.x, vec2.y};
        }
        public static float[] ToFloatArray(this Vector3 vec3)
        {
            return new float[]{vec3.x, vec3.y, vec3.z};
        }
        public static float[] ToFloatArray(this Vector4 vec4)
        {
            return new float[]{vec4.x, vec4.y, vec4.z, vec4.w};
        }
        public static float[] ToFloatArray(this Quaternion quat)
        {
            return new float[]{quat.x, quat.y, quat.z, quat.w};
        }
        public static float[] ToFloatArray(this Matrix4x4 mat4x4)
        {
            return new float[]
            {
                mat4x4.m00, mat4x4.m01, mat4x4.m02, mat4x4.m03,
                mat4x4.m10, mat4x4.m11, mat4x4.m12, mat4x4.m13,
                mat4x4.m20, mat4x4.m21, mat4x4.m22, mat4x4.m23,
                mat4x4.m30, mat4x4.m31, mat4x4.m32, mat4x4.m33
            };
        }

        public static NullableBool ToNullableBool(this SerializedProperty sp)
        {
            var nullableBool = new NullableBool();
            if (sp != null)
            {
                nullableBool.hasValue = true;
                nullableBool.value    = sp.boolValue;
            }
            return nullableBool;
        }
        public static NullableInt ToNullableInt(this SerializedProperty sp)
        {
            var nullableInt = new NullableInt();
            if (sp != null)
            {
                nullableInt.hasValue = true;
                nullableInt.value    = sp.intValue;
            }
            return nullableInt;
        }
#endregion


#region Convert & Set
        public static Vector2 ToVector2(this float[] floatArray)
        {
            var vec2 = new Vector2();
            if (floatArray.Length >= 1)
            {
                vec2.x = floatArray[0];
            }
            if (floatArray.Length >= 2)
            {
                vec2.y = floatArray[1];
            }
            return vec2;
        }

        public static Vector3 ToVector3(this float[] floatArray)
        {
            var vec3 = new Vector3();
            if (floatArray.Length >= 1)
            {
                vec3.x = floatArray[0];
            }
            if (floatArray.Length >= 2)
            {
                vec3.y = floatArray[1];
            }
            if (floatArray.Length >= 3)
            {
                vec3.z = floatArray[2];
            }
            return vec3;
        }

        public static Vector4 ToVector4(this float[] floatArray)
        {
            var vec4 = new Vector4();
            if (floatArray.Length >= 1)
            {
                vec4.x = floatArray[0];
            }
            if (floatArray.Length >= 2)
            {
                vec4.y = floatArray[1];
            }
            if (floatArray.Length >= 3)
            {
                vec4.z = floatArray[2];
            }
            if (floatArray.Length >= 4)
            {
                vec4.w = floatArray[3];
            }
            return vec4;
        }

        public static Quaternion ToQuaternion(this float[] floatArray)
        {
            var quat = new Quaternion();
            if (floatArray.Length >= 1)
            {
                quat.x = floatArray[0];
            }
            if (floatArray.Length >= 2)
            {
                quat.y = floatArray[1];
            }
            if (floatArray.Length >= 3)
            {
                quat.z = floatArray[2];
            }
            if (floatArray.Length >= 4)
            {
                quat.w = floatArray[3];
            }
            return quat;
        }

        public static Matrix4x4 ToMatrix4x4(this float[] floatArray)
        {
            var mat4x4 = new Matrix4x4();
            if (floatArray.Length >= 1)
            {
                mat4x4.m00 = floatArray[0];
            }
            if (floatArray.Length >= 2)
            {
                mat4x4.m01 = floatArray[1];
            }
            if (floatArray.Length >= 3)
            {
                mat4x4.m02 = floatArray[2];
            }
            if (floatArray.Length >= 4)
            {
                mat4x4.m03 = floatArray[3];
            }
            if (floatArray.Length >= 5)
            {
                mat4x4.m10 = floatArray[4];
            }
            if (floatArray.Length >= 6)
            {
                mat4x4.m11 = floatArray[5];
            }
            if (floatArray.Length >= 7)
            {
                mat4x4.m12 = floatArray[6];
            }
            if (floatArray.Length >= 8)
            {
                mat4x4.m13 = floatArray[7];
            }
            if (floatArray.Length >= 9)
            {
                mat4x4.m20 = floatArray[8];
            }
            if (floatArray.Length >= 10)
            {
                mat4x4.m21 = floatArray[9];
            }
            if (floatArray.Length >= 11)
            {
                mat4x4.m22 = floatArray[10];
            }
            if (floatArray.Length >= 12)
            {
                mat4x4.m23 = floatArray[11];
            }
            if (floatArray.Length >= 13)
            {
                mat4x4.m30 = floatArray[12];
            }
            if (floatArray.Length >= 14)
            {
                mat4x4.m31 = floatArray[13];
            }
            if (floatArray.Length >= 15)
            {
                mat4x4.m32 = floatArray[14];
            }
            if (floatArray.Length >= 16)
            {
                mat4x4.m33 = floatArray[15];
            }
            return mat4x4;
        }

        public static Vector3[] ToVector3Array(this ProbePosition[] probePositions)
        {
            var probePosLength = probePositions.Length;
            var vec3Array = new Vector3[probePosLength];
            for (var i = 0; i < probePosLength; i++)
            {
                vec3Array[i] = probePositions[i].position.ToVector3();
            }
            return vec3Array;
        }

        public static void SetSerializedProperty(this SerializedProperty sp, float floatValue)
        {
            if (sp == null) { return; }
            if (sp.propertyType != SerializedPropertyType.Float) { return; }
            sp.serializedObject.Update();
            sp.floatValue = floatValue;
            sp.serializedObject.ApplyModifiedProperties();
        }
        public static void SetSerializedProperty(this SerializedProperty sp, int intValue)
        {
            if (sp == null) { return; }
            if (sp.propertyType != SerializedPropertyType.Integer) { return; }
            sp.serializedObject.Update();
            sp.intValue = intValue;
            sp.serializedObject.ApplyModifiedProperties();
        }
        public static void SetSerializedProperty(this SerializedProperty sp, NullableInt nullableInt)
        {
            if (sp == null || nullableInt.hasValue == false) { return; }
            if (sp.propertyType != SerializedPropertyType.Integer) { return; }
            sp.serializedObject.Update();
            sp.intValue = nullableInt.value;
            sp.serializedObject.ApplyModifiedProperties();
        }
        public static void SetSerializedProperty(this SerializedProperty sp, bool boolValue)
        {
            if (sp == null) { return; }
            if (sp.propertyType != SerializedPropertyType.Boolean) { return; }
            sp.serializedObject.Update();
            sp.boolValue = boolValue;
            sp.serializedObject.ApplyModifiedProperties();
        }
        public static void SetSerializedProperty(this SerializedProperty sp, NullableBool nullableBool)
        {
            if (sp == null || nullableBool.hasValue == false) { return; }
            if (sp.propertyType != SerializedPropertyType.Boolean) { return; }
            sp.serializedObject.Update();
            sp.boolValue = nullableBool.value;
            sp.serializedObject.ApplyModifiedProperties();
        }
        public static void SetSerializedProperty(this SerializedProperty sp, Object obj)
        {
            if (sp == null || obj == null) { return; }
            if (sp.propertyType != SerializedPropertyType.ObjectReference) { return; }
            sp.serializedObject.Update();
            sp.objectReferenceValue = obj;
            sp.serializedObject.ApplyModifiedProperties();
        }
        public static void SetSerializedProperty(this SerializedProperty sp, Color colorValue)
        {
            if (sp == null) { return; }
            if (sp.propertyType != SerializedPropertyType.Float) { return; }
            sp.serializedObject.Update();
            sp.colorValue = colorValue;
            sp.serializedObject.ApplyModifiedProperties();
        }
#endregion

#region etc.
        public static bool ExistsAssetObject(Object obj)
        {
            var path = AssetDatabase.GetAssetPath(obj.GetInstanceID());
            return File.Exists(path);
        }

        public static string GetWarningMessage(string warningPath)
        {
            return (Application.systemLanguage == SystemLanguage.Japanese) ?
                warningPath + " : 設定されたオブジェクトは保存されたアセットではありませんでした。\n" :
                warningPath + " : The set object is not a saved asset.\n";
        }

        public static void GetSerializedProperties(SerializedObject so, ref StreamWriter sw)
        {
            var sp = so.GetIterator();

            while (sp.Next(true))
            {
                switch (sp.propertyType)
                {
                    case SerializedPropertyType.Integer:
                        sw.WriteLine ("Integer        : " + sp.propertyPath + " ( " + sp.displayName + " ) ----- " + sp.intValue);
                        break;
                    case SerializedPropertyType.Float:
                        sw.WriteLine ("Float          : " + sp.propertyPath + " ( " + sp.displayName + " ) ----- " + sp.floatValue);
                        break;
                    case SerializedPropertyType.Boolean:
                        sw.WriteLine ("Boolean        : " + sp.propertyPath + " ( " + sp.displayName + " ) ----- " + sp.boolValue);
                        break;
                    case SerializedPropertyType.String:
                        sw.WriteLine ("String         : " + sp.propertyPath + " ( " + sp.displayName + " ) ----- " + sp.stringValue);
                        break;
                    case SerializedPropertyType.Enum:
                        sw.WriteLine ("Enum           : " + sp.propertyPath + " ( " + sp.displayName + " ) ----- " + sp.enumValueIndex);
                        break;
                    case SerializedPropertyType.ArraySize:
                        sw.WriteLine ("ArraySize      : " + sp.propertyPath + " ( " + sp.displayName + " ) ----- " + sp.intValue);
                        break;
                    case SerializedPropertyType.AnimationCurve:
                        sw.WriteLine ("AnimationCurve : " + sp.propertyPath + " ( " + sp.displayName + " ) ----- " + sp.propertyType);
                        break;
                    case SerializedPropertyType.Quaternion:
                        sw.WriteLine ("Quaternion     : " + sp.propertyPath + " ( " + sp.displayName + " ) ----- " + sp.quaternionValue);
                        break;
                    case SerializedPropertyType.Generic:
                        sw.WriteLine ("Generic        : " + sp.propertyPath + " ( " + sp.displayName + " ) ----- " + sp.propertyType);
                        break;
                    case SerializedPropertyType.ObjectReference:
                        sw.WriteLine ("ObjectReference: " + sp.propertyPath + " ( " + sp.displayName + " ) ----- " + sp.objectReferenceValue);
                        break;
                    case SerializedPropertyType.LayerMask:
                        sw.WriteLine ("LayerMask      : " + sp.propertyPath + " ( " + sp.displayName + " ) ----- " + sp.propertyType);
                        break;
                    case SerializedPropertyType.Character:
                        sw.WriteLine ("Character      : " + sp.propertyPath + " ( " + sp.displayName + " ) ----- " + sp.propertyType);
                        break;
                    case SerializedPropertyType.Bounds:
                        sw.WriteLine ("Bounds         : " + sp.propertyPath + " ( " + sp.displayName + " ) ----- " + sp.boundsValue);
                        break;
                    case SerializedPropertyType.Vector2:
                        sw.WriteLine ("Vector2        : " + sp.propertyPath + " ( " + sp.displayName + " ) ----- " + sp.vector2Value);
                        break;
                    case SerializedPropertyType.Vector3:
                        sw.WriteLine ("Vector3        : " + sp.propertyPath + " ( " + sp.displayName + " ) ----- " + sp.vector3Value);
                        break;
                    case SerializedPropertyType.Vector4:
                        sw.WriteLine ("Vector4        : " + sp.propertyPath + " ( " + sp.displayName + " ) ----- " + sp.vector4Value);
                        break;
                    case SerializedPropertyType.Gradient:
                        sw.WriteLine ("Gradient       : " + sp.propertyPath + " ( " + sp.displayName + " ) ----- " + sp.propertyType);
                        break;
                    default:
                        sw.WriteLine ("????       : " + sp.propertyPath + " ( " + sp.displayName + " ) ----- " + sp.propertyType);
                        break;
                }
            }
        }
#endregion
    }
}