using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

namespace SceneLightSettings
{
    [System.Serializable]
    public class EnvironmentData
    {
        public Material skyboxMaterial;
        public Light sunSource;

        public AmbientMode lightingSource;
        public float lightingIntensityMultiplier;
        public NullableInt ambientMode;

        public Color skyColor;
        public Color equatorColor;
        public Color groundColor;

        public Color ambientColor;

        public DefaultReflectionMode refSource;
        public int refResolution;
        public ReflectionCubemapCompression refCompression;
        public float refIntensityMultiplier;
        public int refBounces;
        public Cubemap refCubemap;
    }

    [System.Serializable]
    public class LightingData
    {
        public bool realtimeGlobalIllumination;
        public bool bakedGlobalIllumination;
        public MixedLightingMode lightingMode;
        public Color realtimeShadowColor;
    }

    [System.Serializable]
    public class LightmappingSettingsData
    {
        public LightmapEditorSettings.Lightmapper lightmapper;
        public NullableBool finalGather;
        public NullableInt finalGatherRayCount;
        public NullableBool finalGatherDenoising;

        public bool prioritizeView;
        public NullableBool multipleImportanceSampling;
        public int directSamples;
        public int indirectSamples;
        public NullableInt environmentSamples;
        public int bounces;

        public LightmapEditorSettings.FilterMode filtering;
        public LightmapEditorSettings.DenoiserType directDenoiser;
        public LightmapEditorSettings.FilterType directFilter;
        public int directRadius;
        public float directSigma;
        public LightmapEditorSettings.DenoiserType indirectDenoiser;
        public LightmapEditorSettings.FilterType indirectFilter;
        public int indirectRadius;
        public float indirectSigma;
        public LightmapEditorSettings.DenoiserType aoDenoiser;
        public LightmapEditorSettings.FilterType aoFilter;
        public int aoRadius;
        public float aoSigma;

        public float indirectResolution;
        public float lightmapResolution;
        public int lightmapPadding;
        public int lightmapSize;
        public bool compressLightmaps;

        public bool enableAO;
        public float aoMaxDistance;
        public float aoIndirectContribution;
        public float aoDirectContribution;

        public LightmapsMode directionalMode;
        public float indirectIntensity;
        public float albedoBoost;
        public LightmapParameters lightmapParameters;
    }

    [System.Serializable]
    public class OtherSettingsData
    {
        public bool fog;
        public Color fogColor;
        public FogMode fogMode;
        public float fogDensity;
        public float fogStart;
        public float fogEnd;

        public Texture2D haloTexture;
        public float haloStrength;
        public float flareFadeSpeed;
        public float flareStrength;
        public Texture2D spotCookie;
    }

	
	[System.Serializable]
    public class GameObjectData
    {
		public string name;
        public string tag;
        public LayerMask layer;
	}

	[System.Serializable]
    public class TransformData
    {
        public float[] worldPosition = new float[3];
        public float[] worldRotation = new float[4];
        public float[] worldScale    = new float[3];
	}

	[System.Serializable]
    public class ShadowData
    {
		public LightShadows type; // public int type;
		public LightShadowResolution resolution; // public int resolution;
		public int customResolution;
		public float strength;
		public float bias;
		public float normalBias;
		public float nearPlane;

		// shadowMatrixOverride ( CullingMatrixOverride ) は Matrix4x4 だがそのまま保存できないのでfloatで
		public float[] cullingMatrixOverride = new float[16];

		public bool useCullingMatrixOverride;
	}

	[System.Serializable]
    public class BakingOutputData
    {
		public int probeOcclusionLightIndex;
		public int occlusionMaskChannel;
		public LightmapBakeType lightmapBakeType;
		public MixedLightingMode mixedLightingMode;
		public bool isBaked;
	}

    [System.Serializable]
    public class LightData
    {
		public GameObjectData gameObjectData;
		public TransformData transformData;
		public bool enabled;
		public LightType type;
		public Color color;
		public float intensity;
		public float range;
		public float spotAngle;
		public float innerSpotAngle;
		public float cookieSize;

		public ShadowData shadow;

		public Texture cookie;
		public NullableBool drawHalo;

		public BakingOutputData bakingOutput;

		public Flare flare;
		public LightRenderMode renderMode;
		public int cullingMask;
		public int renderingLayerMask;
		public LightmapBakeType lightmapping;
		public LightShadowCasterMode lightShadowCasterMode;
		public float[] areaSize = new float[2];
		public float bounceIntensity;
		public float colorTemperature;
		public float[] boundingSphereOverride = new float[4];
		public bool useBoundingSphereOverride;
		public float shadowRadius;
		public float shadowAngle;
    }

    [System.Serializable]
    public class ProbePosition
    {
        public float[] position = new float[3];
    }

    [System.Serializable]
    public class LightProbeData
    {
		public GameObjectData gameObjectData;
		public TransformData transformData;
		public bool enabled;
        public ProbePosition[] probePositions;
		public bool dering;
    }

    [System.Serializable]
    public class ReflectionProbeData
    {
		public GameObjectData gameObjectData;
		public TransformData transformData;
		public bool enabled;
		public NullableInt type;
		public ReflectionProbeMode mode;
		public ReflectionProbeRefreshMode refreshMode;
		public ReflectionProbeTimeSlicingMode timeSlicingMode;
		public int resolution;
		public NullableInt updateFrequency;
		public float[] boxSize = new float[3];
		public float[] boxOffset = new float[3];
		public float nearClip;
		public float farClip;
		public float shadowDistance;
		public ReflectionProbeClearFlags clearFlags;
		public Color backgroundColor;
		public int cullingMask;
		public float intensityMultiplier;
		public float blendDistance;
		public bool hdr;
		public bool boxProjection;
		public NullableBool renderDynamicObjects;
		public NullableBool useOcclusionCulling;
		public int importance;
		public Texture customBakedTexture;
		public Texture bakedTexture;
    }

    public class SceneLightingData : ScriptableObject
    {
        public EnvironmentData environmentData;
        public LightingData lightingData;
        public LightmappingSettingsData lightmappingSettingsData;
        public OtherSettingsData otherSettingsData;

        public LightData[] sceneLightData;
        public LightProbeData[] sceneLightProbeData;
        public ReflectionProbeData[] sceneReflectionProbeData;
    }

    [System.Serializable]
    public class NullableBool
    {
		public bool hasValue = false;
		public bool value;
	}

    [System.Serializable]
    public class NullableInt
    {
		public bool hasValue = false;
		public int value;
	}

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

		public static void SetSerializedProperty(this SerializedProperty sp, NullableInt nullableInt)
		{
			if (sp == null || nullableInt.hasValue == false) { return; }
			if (sp.propertyType != SerializedPropertyType.Integer) { return; }
			sp.intValue = nullableInt.value;
		}
		public static void SetSerializedProperty(this SerializedProperty sp, NullableBool nullableBool)
		{
			if (sp == null || nullableBool.hasValue == false) { return; }
			if (sp.propertyType != SerializedPropertyType.Boolean) { return; }
			sp.boolValue = nullableBool.value;
		}
		public static void SetSerializedProperty(this SerializedProperty sp, Object obj)
		{
			if (sp == null || obj == null) { return; }
			if (sp.propertyType != SerializedPropertyType.ObjectReference) { return; }
			sp.objectReferenceValue = obj;
		}
	}
#endregion
}