﻿using UnityEngine;
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
}