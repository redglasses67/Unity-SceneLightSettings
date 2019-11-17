using System.Reflection;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

namespace SceneLightSettings
{
    public class SceneLightSettingExporter
    {
        private static readonly string prop_EnvLightMode           = "m_GISettings.m_EnvironmentLightingMode";
        private static readonly string prop_FG                     = "m_LightmapEditorSettings.m_FinalGather";
        private static readonly string prop_FGRayCount             = "m_LightmapEditorSettings.m_FinalGatherRayCount";
        private static readonly string prop_FGFilter               = "m_LightmapEditorSettings.m_FinalGatherFiltering";
        private static readonly string prop_LightmapParams         = "m_LightmapEditorSettings.m_LightmapParameters";

#if !UNITY_2018_1_OR_NEWER
        private static readonly string prop_PVRBounce              = "m_LightmapEditorSettings.m_PVRBounces";
        private static readonly string prop_LightmapsBakeMode      = "m_LightmapEditorSettings.m_LightmapsBakeMode";
#endif

#if !UNITY_2018_2_OR_NEWER
        private static readonly string prop_MixedBakeMode          = "m_LightmapEditorSettings.m_MixedBakeMode";

        private static readonly string prop_PVRCulling             = "m_LightmapEditorSettings.m_PVRCulling";
        private static readonly string prop_PVRFilteringMode       = "m_LightmapEditorSettings.m_PVRFilteringMode";
        private static readonly string prop_PVRDirectSamples       = "m_LightmapEditorSettings.m_PVRDirectSampleCount";
        private static readonly string prop_PVRIndirectSamples     = "m_LightmapEditorSettings.m_PVRSampleCount";

        private static readonly string prop_PVRGaussRDirect        = "m_LightmapEditorSettings.m_PVRFilteringGaussRadiusDirect";
        private static readonly string prop_PVRGaussRIndirect      = "m_LightmapEditorSettings.m_PVRFilteringGaussRadiusIndirect";
        private static readonly string prop_PVRGaussRAO            = "m_LightmapEditorSettings.m_PVRFilteringGaussRadiusAO";
        private static readonly string prop_PVRAtrousDirectSigma   = "m_LightmapEditorSettings.m_PVRFilteringAtrousPositionSigmaDirect";
        private static readonly string prop_PVRAtrousIndirectSigma = "m_LightmapEditorSettings.m_PVRFilteringAtrousPositionSigmaIndirect";
        private static readonly string prop_PVRAtrousAOSigma       = "m_LightmapEditorSettings.m_PVRFilteringAtrousPositionSigmaAO";
#endif

#if UNITY_2019_1_OR_NEWER
        private static readonly string prop_PVREnvMIS              = "m_LightmapEditorSettings.m_PVREnvironmentMIS";

        private static readonly string prop_PVREnvSampleCount      = "m_LightmapEditorSettings.m_PVREnvironmentSampleCount";
        private static readonly string prop_PVREnvRefPointCount    = "m_LightmapEditorSettings.m_PVREnvironmentReferencePointCount";

        private static readonly string prop_ExportTrainingData     = "m_LightmapEditorSettings.m_ExportTrainingData";
        private static readonly string prop_ExtractAO              = "m_LightmapEditorSettings.m_ExtractAmbientOcclusion";
#endif

// 2019.2 でなくなってそうなのでパス
// #if UNITY_2018_1_OR_NEWER
//         private static readonly string prop_UseRadianceAmbProbe    = "m_UseRadianceAmbientProbe";
// #endif
        private static readonly string prop_HaloTex                = "m_HaloTexture";
        private static readonly string prop_SpotCookie             = "m_SpotCookie";

        private static readonly BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static;


        public static SerializedObject GetSerializedLightmapSettingsObject()
        {
            var getLightmapSettings = typeof(LightmapEditorSettings).GetMethod("GetLightmapSettings", flags);
            var lightmapSettings    = (Object)getLightmapSettings.Invoke(null, null);
            return new SerializedObject(lightmapSettings);
        }

        public static SerializedObject GetSerializedRenderSettingsObject()
        {
            var getRenderSettings = typeof(RenderSettings).GetMethod("GetRenderSettings", flags);
            var renderSettings    = (Object)getRenderSettings.Invoke(null, null);
            return new SerializedObject(renderSettings);
        }


        public static SceneLightingData GetSceneLightingData(
            bool isExportLightingData,
            bool isExportLights,
            bool isExportLightProbeGroups,
            bool isExportReflectionProbes)
        {
            var tempEnvironmentData          = new EnvironmentData();
            var tempLightingData             = new LightingData();
            var tempLightmappingSettingsData = new LightmappingSettingsData();
            var tempOtherSettingsData        = new OtherSettingsData();
            var tempExportWarningMessages    = "";

            var so_lightmapSettings          = GetSerializedLightmapSettingsObject();
            var so_renderSettings            = GetSerializedRenderSettingsObject();

            // Environment ========================================================================
            tempEnvironmentData.skyboxMaterial               = RenderSettings.skybox;

            var _sun = RenderSettings.sun;
            if (DataUtility.ExistsAssetObject(_sun) == true)
            {
                tempEnvironmentData.sunSource                = _sun;
            }
            else
            {
                tempEnvironmentData.sunSourceName            = _sun.name;
                tempExportWarningMessages                    += DataUtility.GetWarningMessage("Environment - Sun Source");
            }

            tempEnvironmentData.lightingSource               = RenderSettings.ambientMode;
            tempEnvironmentData.lightingIntensityMultiplier  = RenderSettings.ambientIntensity;
            tempEnvironmentData.ambientMode                  = so_lightmapSettings.FindProperty(prop_EnvLightMode).ToNullableInt();
            tempEnvironmentData.skyColor                     = RenderSettings.ambientSkyColor;
            tempEnvironmentData.equatorColor                 = RenderSettings.ambientEquatorColor;
            tempEnvironmentData.groundColor                  = RenderSettings.ambientGroundColor;
            tempEnvironmentData.ambientColor                 = RenderSettings.ambientLight;
            tempEnvironmentData.refSource                    = RenderSettings.defaultReflectionMode;
            tempEnvironmentData.refResolution                = RenderSettings.defaultReflectionResolution;
            tempEnvironmentData.refCompression               = LightmapEditorSettings.reflectionCubemapCompression;
            tempEnvironmentData.refIntensityMultiplier       = RenderSettings.reflectionIntensity;
            tempEnvironmentData.refBounces                   = RenderSettings.reflectionBounces;
            tempEnvironmentData.refCubemap                   = RenderSettings.customReflection;


            // Realtime & Mixed Lighting ==========================================================
            tempLightingData.realtimeGlobalIllumination  = Lightmapping.realtimeGI;
            tempLightingData.bakedGlobalIllumination     = Lightmapping.bakedGI;
#if UNITY_2018_2_OR_NEWER
            tempLightingData.lightingMode                = LightmapEditorSettings.mixedBakeMode;
#else
            tempLightingData.lightingMode                = (MixedLightingMode)so_lightmapSettings.FindProperty(prop_MixedBakeMode).intValue;
#endif
            tempLightingData.realtimeShadowColor         = RenderSettings.subtractiveShadowColor;


            // Lightmapping Settings ==============================================================
            tempLightmappingSettingsData.lightmapper                 = LightmapEditorSettings.lightmapper;
            tempLightmappingSettingsData.finalGather                 = so_lightmapSettings.FindProperty(prop_FG).ToNullableBool();
            tempLightmappingSettingsData.finalGatherRayCount         = so_lightmapSettings.FindProperty(prop_FGRayCount).ToNullableInt();
            tempLightmappingSettingsData.finalGatherDenoising        = so_lightmapSettings.FindProperty(prop_FGFilter).ToNullableBool();
#if UNITY_2018_2_OR_NEWER
            tempLightmappingSettingsData.prioritizeView              = LightmapEditorSettings.prioritizeView;
#else
            tempLightmappingSettingsData.prioritizeView              = so_lightmapSettings.FindProperty(prop_PVRCulling).boolValue;
#endif

#if UNITY_2019_1_OR_NEWER
            tempLightmappingSettingsData.multipleImportanceSampling  = so_lightmapSettings.FindProperty(prop_PVREnvMIS).ToNullableBool();
#endif

#if UNITY_2018_2_OR_NEWER
            tempLightmappingSettingsData.directSamples               = LightmapEditorSettings.directSampleCount;
            tempLightmappingSettingsData.indirectSamples             = LightmapEditorSettings.indirectSampleCount;
#else
            tempLightmappingSettingsData.directSamples               = so_lightmapSettings.FindProperty(prop_PVRDirectSamples).intValue;
            tempLightmappingSettingsData.indirectSamples             = so_lightmapSettings.FindProperty(prop_PVRIndirectSamples).intValue;
#endif

#if UNITY_2019_1_OR_NEWER
            tempLightmappingSettingsData.environmentSamples          = so_lightmapSettings.FindProperty(prop_PVREnvSampleCount).ToNullableInt();
            tempLightmappingSettingsData.environmentRefPoints        = so_lightmapSettings.FindProperty(prop_PVREnvRefPointCount).ToNullableInt();
#endif

#if UNITY_2018_1_OR_NEWER
            tempLightmappingSettingsData.bounces                     = LightmapEditorSettings.bounces;
#else
            tempLightmappingSettingsData.bounces                     = so_lightmapSettings.FindProperty(prop_PVRBounce).intValue;
#endif

#if UNITY_2018_2_OR_NEWER
            tempLightmappingSettingsData.filtering                   = LightmapEditorSettings.filteringMode;
#else
            tempLightmappingSettingsData.filtering                   = (LightmapEditorSettings.FilterMode)so_lightmapSettings.FindProperty(prop_PVRFilteringMode).intValue;
#endif

#if UNITY_2019_1_OR_NEWER
            tempLightmappingSettingsData.directDenoiser              = LightmapEditorSettings.denoiserTypeDirect;
#endif

            tempLightmappingSettingsData.directFilter                = LightmapEditorSettings.filterTypeDirect;

#if UNITY_2018_2_OR_NEWER
            tempLightmappingSettingsData.directSigma                 = LightmapEditorSettings.filteringAtrousPositionSigmaDirect;
            tempLightmappingSettingsData.directRadius                = LightmapEditorSettings.filteringGaussRadiusDirect;
#else
            tempLightmappingSettingsData.directSigma                 = so_lightmapSettings.FindProperty(prop_PVRAtrousDirectSigma).floatValue;
            tempLightmappingSettingsData.directRadius                = so_lightmapSettings.FindProperty(prop_PVRGaussRDirect).intValue;
#endif

#if UNITY_2019_1_OR_NEWER
            tempLightmappingSettingsData.indirectDenoiser            = LightmapEditorSettings.denoiserTypeIndirect;
#endif

            tempLightmappingSettingsData.indirectFilter              = LightmapEditorSettings.filterTypeIndirect;

#if UNITY_2018_2_OR_NEWER
            tempLightmappingSettingsData.indirectSigma               = LightmapEditorSettings.filteringAtrousPositionSigmaIndirect;
            tempLightmappingSettingsData.indirectRadius              = LightmapEditorSettings.filteringGaussRadiusIndirect;
#else
            tempLightmappingSettingsData.indirectSigma               = so_lightmapSettings.FindProperty(prop_PVRAtrousIndirectSigma).floatValue;
            tempLightmappingSettingsData.indirectRadius              = so_lightmapSettings.FindProperty(prop_PVRGaussRIndirect).intValue;
#endif

#if UNITY_2019_1_OR_NEWER
            tempLightmappingSettingsData.aoDenoiser                  = LightmapEditorSettings.denoiserTypeAO;
#endif

            tempLightmappingSettingsData.aoFilter                    = LightmapEditorSettings.filterTypeAO;

#if UNITY_2018_2_OR_NEWER
            tempLightmappingSettingsData.aoSigma                     = LightmapEditorSettings.filteringAtrousPositionSigmaAO;
            tempLightmappingSettingsData.aoRadius                    = LightmapEditorSettings.filteringGaussRadiusAO;
#else
            tempLightmappingSettingsData.aoSigma                     = so_lightmapSettings.FindProperty(prop_PVRAtrousAOSigma).floatValue;
            tempLightmappingSettingsData.aoRadius                    = so_lightmapSettings.FindProperty(prop_PVRGaussRAO).intValue;
#endif

            tempLightmappingSettingsData.indirectResolution          = LightmapEditorSettings.realtimeResolution;
            tempLightmappingSettingsData.lightmapResolution          = LightmapEditorSettings.bakeResolution;
            tempLightmappingSettingsData.lightmapPadding             = LightmapEditorSettings.padding;

#if UNITY_2018_1_OR_NEWER
            tempLightmappingSettingsData.lightmapSize                = LightmapEditorSettings.maxAtlasSize;
#else
            tempLightmappingSettingsData.lightmapSize                = LightmapEditorSettings.maxAtlasWidth;
#endif
            tempLightmappingSettingsData.compressLightmaps           = LightmapEditorSettings.textureCompression;

            tempLightmappingSettingsData.enableAO                    = LightmapEditorSettings.enableAmbientOcclusion;
            tempLightmappingSettingsData.aoMaxDistance               = LightmapEditorSettings.aoMaxDistance;
            tempLightmappingSettingsData.aoIndirectContribution      = LightmapEditorSettings.aoExponentIndirect;
            tempLightmappingSettingsData.aoDirectContribution        = LightmapEditorSettings.aoExponentDirect;

#if UNITY_2018_1_OR_NEWER
            tempLightmappingSettingsData.directionalMode             = LightmapEditorSettings.lightmapsMode;
#else
            // tempLightmappingSettingsData.directionalMode             = LightmapSettings.lightmapsMode; // 2017.4.34で、こっちだときちんと取得できなかった… 
            tempLightmappingSettingsData.directionalMode             = (LightmapsMode)so_lightmapSettings.FindProperty(prop_LightmapsBakeMode).intValue;
#endif
            tempLightmappingSettingsData.indirectIntensity           = Lightmapping.indirectOutputScale;
            tempLightmappingSettingsData.albedoBoost                 = Lightmapping.bounceBoost;
            tempLightmappingSettingsData.lightmapParameters          = (LightmapParameters)so_lightmapSettings.FindProperty(prop_LightmapParams).objectReferenceValue;

#if UNITY_2019_1_OR_NEWER
            tempLightmappingSettingsData.exportTrainingData          = so_lightmapSettings.FindProperty(prop_ExportTrainingData).ToNullableBool();
            tempLightmappingSettingsData.extractAmbientOcclusion     = so_lightmapSettings.FindProperty(prop_ExtractAO).ToNullableBool();
#endif

            // Other Settings =====================================================================
            tempOtherSettingsData.fog            = RenderSettings.fog;
            tempOtherSettingsData.fogColor       = RenderSettings.fogColor;
            tempOtherSettingsData.fogMode        = RenderSettings.fogMode;
            tempOtherSettingsData.fogDensity     = RenderSettings.fogDensity;
            tempOtherSettingsData.fogStart       = RenderSettings.fogStartDistance;
            tempOtherSettingsData.fogEnd         = RenderSettings.fogEndDistance;

            tempOtherSettingsData.haloTexture    = (Texture2D)so_renderSettings.FindProperty(prop_HaloTex).objectReferenceValue;
            tempOtherSettingsData.haloStrength   = RenderSettings.haloStrength;
            tempOtherSettingsData.flareFadeSpeed = RenderSettings.flareFadeSpeed;
            tempOtherSettingsData.flareStrength  = RenderSettings.flareStrength;
            tempOtherSettingsData.spotCookie     = (Texture2D)so_renderSettings.FindProperty(prop_SpotCookie).objectReferenceValue;
            tempOtherSettingsData.gIWorkflowMode = Lightmapping.giWorkflowMode;


            var exportSLD = ScriptableObject.CreateInstance<SceneLightingData>();
            exportSLD.environmentData          = (isExportLightingData == true) ? tempEnvironmentData : null;
            exportSLD.lightingData             = (isExportLightingData == true) ? tempLightingData : null;
            exportSLD.lightmappingSettingsData = (isExportLightingData == true) ? tempLightmappingSettingsData : null;
            exportSLD.otherSettingsData        = (isExportLightingData == true) ? tempOtherSettingsData : null;
            exportSLD.sceneLightData           = GetLightDataArray(isExportLights);
            exportSLD.sceneLightProbeData      = GetLightProbeDataArray(isExportLightProbeGroups);
            exportSLD.sceneReflectionProbeData = GetReflectionProbeDataArray(isExportReflectionProbes);
            exportSLD.exportWarningMessages    = tempExportWarningMessages;
            return exportSLD;
        }


        private static LightData[] GetLightDataArray(bool isExportLights)
        {
            if (isExportLights == false) { return null; }

            var tempSceneLightDataList = new List<LightData>();

            var sceneLights = GameObject.FindObjectsOfType(typeof(Light)) as Light[];
            foreach (var sceneLight in sceneLights)
            {
                var lightGameObject = sceneLight.gameObject;
                var lightTransform  = lightGameObject.transform;
                var so_light        = new SerializedObject(sceneLight);
                so_light.Update();

                var tempSceneLightData                       = new LightData();
                tempSceneLightData.gameObjectData            = new GameObjectData()
                {
                    name  = lightGameObject.name,
                    tag   = lightGameObject.tag,
                    layer = lightGameObject.layer
                };
                tempSceneLightData.transformData             = new TransformData()
                {
                    worldPosition = lightTransform.position.ToFloatArray(),
                    worldRotation = lightTransform.rotation.ToFloatArray(),
                    worldScale    = lightTransform.lossyScale.ToFloatArray()
                };
                tempSceneLightData.enabled                   = sceneLight.enabled;
                tempSceneLightData.type                      = sceneLight.type;
                tempSceneLightData.color                     = sceneLight.color;
                tempSceneLightData.intensity                 = sceneLight.intensity;
                tempSceneLightData.range                     = sceneLight.range;
                tempSceneLightData.spotAngle                 = sceneLight.spotAngle;
#if UNITY_2019_1_OR_NEWER
                tempSceneLightData.innerSpotAngle            = sceneLight.innerSpotAngle;
#endif
                tempSceneLightData.cookieSize                = sceneLight.cookieSize;
                tempSceneLightData.shadow                    = new ShadowData()
                {
                    type                     = sceneLight.shadows,
                    resolution               = sceneLight.shadowResolution,
                    customResolution         = sceneLight.shadowCustomResolution,
                    strength                 = sceneLight.shadowStrength,
                    bias                     = sceneLight.shadowBias,
                    normalBias               = sceneLight.shadowNormalBias,
                    nearPlane                = sceneLight.shadowNearPlane,
#if UNITY_2019_1_OR_NEWER
                    cullingMatrixOverride    = sceneLight.shadowMatrixOverride.ToFloatArray(),
                    useCullingMatrixOverride = sceneLight.useShadowMatrixOverride
#endif
                };
                tempSceneLightData.cookie                    = sceneLight.cookie;

                // DrawHalo が普通に取得できないので、SerializedProperty経由で取得
                tempSceneLightData.drawHalo                  = so_light.FindProperty("m_DrawHalo").ToNullableBool();

                tempSceneLightData.bakingOutput              = new BakingOutputData()
                {
                    probeOcclusionLightIndex = sceneLight.bakingOutput.probeOcclusionLightIndex,
                    occlusionMaskChannel     = sceneLight.bakingOutput.occlusionMaskChannel,
                    lightmapBakeType         = sceneLight.bakingOutput.lightmapBakeType,
                    mixedLightingMode        = sceneLight.bakingOutput.mixedLightingMode,
                    isBaked                  = sceneLight.bakingOutput.isBaked
                };
                tempSceneLightData.flare                     = sceneLight.flare;
                tempSceneLightData.renderMode                = sceneLight.renderMode;
                tempSceneLightData.cullingMask               = sceneLight.cullingMask;
#if UNITY_2019_1_OR_NEWER
                tempSceneLightData.renderingLayerMask        = sceneLight.renderingLayerMask;
#endif
                tempSceneLightData.lightmapping              = sceneLight.lightmapBakeType;
#if UNITY_2018_2_OR_NEWER
                tempSceneLightData.lightShadowCasterMode     = sceneLight.lightShadowCasterMode;
#endif
                tempSceneLightData.areaSize                  = sceneLight.areaSize.ToFloatArray();
                tempSceneLightData.bounceIntensity           = sceneLight.bounceIntensity;
#if UNITY_2019_1_OR_NEWER
                tempSceneLightData.boundingSphereOverride    = sceneLight.boundingSphereOverride.ToFloatArray();
                tempSceneLightData.useBoundingSphereOverride = sceneLight.useBoundingSphereOverride;
#endif

#if UNITY_2018_1_OR_NEWER
                tempSceneLightData.shadowRadius              = sceneLight.shadowRadius;
                tempSceneLightData.shadowAngle               = sceneLight.shadowAngle;
#endif

                tempSceneLightDataList.Add(tempSceneLightData);
            }
            return tempSceneLightDataList.ToArray();
        }

        private static LightProbeData[] GetLightProbeDataArray(bool isExportLightProbeGroups)
        {
            if (isExportLightProbeGroups == false) { return null; }

            var tempSceneLightProbeGroupDataList = new List<LightProbeData>();

            var sceneLightProbeGroups = GameObject.FindObjectsOfType(typeof(LightProbeGroup)) as LightProbeGroup[];
            foreach (var sceneLightProbeGroup in sceneLightProbeGroups)
            {
                var lightProbeGroupGameObject = sceneLightProbeGroup.gameObject;
                var lightProbeGroupTransform  = lightProbeGroupGameObject.transform;

                var tempSceneLightProbeGroup             = new LightProbeData();
                tempSceneLightProbeGroup.gameObjectData  = new GameObjectData()
                {
                    name  = lightProbeGroupGameObject.name,
                    tag   = lightProbeGroupGameObject.tag,
                    layer = lightProbeGroupGameObject.layer
                };
                tempSceneLightProbeGroup.transformData  = new TransformData()
                {
                    worldPosition = lightProbeGroupTransform.position.ToFloatArray(),
                    worldRotation = lightProbeGroupTransform.rotation.ToFloatArray(),
                    worldScale    = lightProbeGroupTransform.lossyScale.ToFloatArray()
                };
                tempSceneLightProbeGroup.enabled        = sceneLightProbeGroup.enabled;

                var tempLightProbePositionList          = new List<ProbePosition>();
                foreach (var probePos in sceneLightProbeGroup.probePositions)
                {
                    var tempLightProbePosition = new ProbePosition();
                    tempLightProbePosition.position = probePos.ToFloatArray();
                    tempLightProbePositionList.Add(tempLightProbePosition);
                }
                tempSceneLightProbeGroup.probePositions = tempLightProbePositionList.ToArray();
#if UNITY_2019_1_OR_NEWER
                tempSceneLightProbeGroup.dering         = sceneLightProbeGroup.dering;
#endif
                tempSceneLightProbeGroupDataList.Add(tempSceneLightProbeGroup);
            }
            return tempSceneLightProbeGroupDataList.ToArray();
        }

        private static ReflectionProbeData[] GetReflectionProbeDataArray(bool isExportReflectionProbes)
        {
            if (isExportReflectionProbes == false) { return null; }

            var tempSceneReflectionProbeDataList = new List<ReflectionProbeData>();

            var sceneReflectionProbes = GameObject.FindObjectsOfType(typeof(ReflectionProbe)) as ReflectionProbe[];
            foreach (var sceneReflectionProbe in sceneReflectionProbes)
            {
                var reflectionProbeGameObject = sceneReflectionProbe.gameObject;
                var reflectionProbeTransform  = reflectionProbeGameObject.transform;
                var so_reflectionProbe        = new SerializedObject(sceneReflectionProbe);
                so_reflectionProbe.Update();

                var tempSceneReflectionProbe                  = new ReflectionProbeData();
                tempSceneReflectionProbe.gameObjectData       = new GameObjectData()
                {
                    name  = reflectionProbeGameObject.name,
                    tag   = reflectionProbeGameObject.tag,
                    layer = reflectionProbeGameObject.layer
                };
                tempSceneReflectionProbe.transformData        = new TransformData()
                {
                    worldPosition = reflectionProbeTransform.position.ToFloatArray(),
                    worldRotation = reflectionProbeTransform.rotation.ToFloatArray(),
                    worldScale    = reflectionProbeTransform.lossyScale.ToFloatArray()
                };
                tempSceneReflectionProbe.enabled              = sceneReflectionProbe.enabled;

                tempSceneReflectionProbe.type                 = so_reflectionProbe.FindProperty("m_Type").ToNullableInt();

                tempSceneReflectionProbe.mode                 = sceneReflectionProbe.mode;
                tempSceneReflectionProbe.refreshMode          = sceneReflectionProbe.refreshMode;
                tempSceneReflectionProbe.timeSlicingMode      = sceneReflectionProbe.timeSlicingMode;
                tempSceneReflectionProbe.resolution           = sceneReflectionProbe.resolution;

                tempSceneReflectionProbe.updateFrequency      = so_reflectionProbe.FindProperty("m_UpdateFrequency").ToNullableInt();

                tempSceneReflectionProbe.boxSize              = sceneReflectionProbe.size.ToFloatArray();
                tempSceneReflectionProbe.boxOffset            = sceneReflectionProbe.center.ToFloatArray();
                tempSceneReflectionProbe.nearClip             = sceneReflectionProbe.nearClipPlane;
                tempSceneReflectionProbe.farClip              = sceneReflectionProbe.farClipPlane;
                tempSceneReflectionProbe.shadowDistance       = sceneReflectionProbe.shadowDistance;
                tempSceneReflectionProbe.clearFlags           = sceneReflectionProbe.clearFlags;
                tempSceneReflectionProbe.backgroundColor      = sceneReflectionProbe.backgroundColor;
                tempSceneReflectionProbe.cullingMask          = sceneReflectionProbe.cullingMask;
                tempSceneReflectionProbe.intensityMultiplier  = sceneReflectionProbe.intensity;
                tempSceneReflectionProbe.blendDistance        = sceneReflectionProbe.blendDistance;
                tempSceneReflectionProbe.hdr                  = sceneReflectionProbe.hdr;
                tempSceneReflectionProbe.boxProjection        = sceneReflectionProbe.boxProjection;

                tempSceneReflectionProbe.renderDynamicObjects = so_reflectionProbe.FindProperty("m_RenderDynamicObjects").ToNullableBool();
                tempSceneReflectionProbe.useOcclusionCulling  = so_reflectionProbe.FindProperty("m_UseOcclusionCulling").ToNullableBool();

                tempSceneReflectionProbe.importance           = sceneReflectionProbe.importance;
                tempSceneReflectionProbe.customBakedTexture   = sceneReflectionProbe.customBakedTexture;
                tempSceneReflectionProbe.bakedTexture         = sceneReflectionProbe.bakedTexture;

                tempSceneReflectionProbeDataList.Add(tempSceneReflectionProbe);
            }
            return tempSceneReflectionProbeDataList.ToArray();
        }


        public static void SetSceneLightingData(
            SceneLightingData importSLD,
            bool doImportEnviromentData,
            bool doImportRealtimeAndMixedLightingData,
            bool doImportLightmappingSettingsData,
            bool doImportOtherSettingsData)
        {
            if (doImportEnviromentData == false
            &&  doImportRealtimeAndMixedLightingData == false
            &&  doImportLightmappingSettingsData == false
            &&  doImportOtherSettingsData == false)
            {
                return;
            }

            var tempEnvironmentData          = importSLD.environmentData;
            var tempLightingData             = importSLD.lightingData;
            var tempLightmappingSettingsData = importSLD.lightmappingSettingsData;
            var tempOtherSettingsData        = importSLD.otherSettingsData;

            var so_lightmapSettings = GetSerializedLightmapSettingsObject();
            so_lightmapSettings.Update();

            var so_renderSettings   = GetSerializedRenderSettingsObject();
            so_renderSettings.Update();

            if (doImportEnviromentData == true)
            {
                // Environment ========================================================================
                RenderSettings.skybox                                       = tempEnvironmentData.skyboxMaterial;
                if (tempEnvironmentData.sunSource != null)
                {
                    RenderSettings.sun                                      = tempEnvironmentData.sunSource;
                }
                else
                {
                    // シーン内に sunSourceName と同じ名前のLightがあるかどうかチェックし、もし存在していたらセットする
                    var _sunSourceObject = Object.FindObjectsOfType(typeof(Light)).FirstOrDefault(o => o.name == tempEnvironmentData.sunSourceName);
                    if (_sunSourceObject != null)
                    {
                        RenderSettings.sun                                  = (Light)_sunSourceObject;
                    }
                }
                RenderSettings.ambientMode                                  = tempEnvironmentData.lightingSource;
                RenderSettings.ambientIntensity                             = tempEnvironmentData.lightingIntensityMultiplier;
                so_lightmapSettings.FindProperty(prop_EnvLightMode).SetSerializedProperty(tempEnvironmentData.ambientMode);
                RenderSettings.ambientSkyColor                              = tempEnvironmentData.skyColor;
                RenderSettings.ambientEquatorColor                          = tempEnvironmentData.equatorColor;
                RenderSettings.ambientGroundColor                           = tempEnvironmentData.groundColor;
                RenderSettings.ambientLight                                 = tempEnvironmentData.ambientColor;
                RenderSettings.defaultReflectionMode                        = tempEnvironmentData.refSource;
                RenderSettings.defaultReflectionResolution                  = tempEnvironmentData.refResolution;
                LightmapEditorSettings.reflectionCubemapCompression         = tempEnvironmentData.refCompression;
                RenderSettings.reflectionIntensity                          = tempEnvironmentData.refIntensityMultiplier;
                RenderSettings.reflectionBounces                            = tempEnvironmentData.refBounces;
                RenderSettings.customReflection                             = tempEnvironmentData.refCubemap;
            }

            if (doImportRealtimeAndMixedLightingData == true)
            {
                // Realtime & Mixed Lighting ==========================================================
                Lightmapping.realtimeGI                                     = tempLightingData.realtimeGlobalIllumination;
                Lightmapping.bakedGI                                        = tempLightingData.bakedGlobalIllumination;
#if UNITY_2018_2_OR_NEWER
                LightmapEditorSettings.mixedBakeMode                        = tempLightingData.lightingMode;
#else
                so_lightmapSettings.FindProperty(prop_MixedBakeMode).SetSerializedProperty((int)tempLightingData.lightingMode);
#endif

                RenderSettings.subtractiveShadowColor                       = tempLightingData.realtimeShadowColor;
#if !UNITY_2018_1_OR_NEWER
                // 2017 ではなぜか
                Debug.Log("2017 ではなぜか subtractiveShadowColor のEditor上での更新");
                var scriptPath = AssetDatabase.GetAssetPath( MonoScript.FromScriptableObject(importSLD) );
                AssetDatabase.ImportAsset(scriptPath);
#endif
            }

            if (doImportLightmappingSettingsData == true)
            {
                // Lightmapping Settings ==============================================================
                LightmapEditorSettings.lightmapper                          = tempLightmappingSettingsData.lightmapper;
                so_lightmapSettings.FindProperty(prop_FG).SetSerializedProperty(tempLightmappingSettingsData.finalGather);
                so_lightmapSettings.FindProperty(prop_FGRayCount).SetSerializedProperty(tempLightmappingSettingsData.finalGatherRayCount);
                so_lightmapSettings.FindProperty(prop_FGFilter).SetSerializedProperty(tempLightmappingSettingsData.finalGatherDenoising);
#if UNITY_2018_2_OR_NEWER
                LightmapEditorSettings.prioritizeView                       = tempLightmappingSettingsData.prioritizeView;
#else
                so_lightmapSettings.FindProperty(prop_PVRCulling).SetSerializedProperty(tempLightmappingSettingsData.prioritizeView);
#endif

#if UNITY_2019_1_OR_NEWER
                so_lightmapSettings.FindProperty(prop_PVREnvMIS).SetSerializedProperty(tempLightmappingSettingsData.multipleImportanceSampling);
#endif

#if UNITY_2018_2_OR_NEWER
                LightmapEditorSettings.directSampleCount                    = tempLightmappingSettingsData.directSamples;
                LightmapEditorSettings.indirectSampleCount                  = tempLightmappingSettingsData.indirectSamples;
#else
                so_lightmapSettings.FindProperty(prop_PVRDirectSamples).SetSerializedProperty(tempLightmappingSettingsData.directSamples);
                so_lightmapSettings.FindProperty(prop_PVRIndirectSamples).SetSerializedProperty(tempLightmappingSettingsData.indirectSamples);
#endif

#if UNITY_2019_1_OR_NEWER
                so_lightmapSettings.FindProperty(prop_PVREnvSampleCount).SetSerializedProperty(tempLightmappingSettingsData.environmentSamples);
                so_lightmapSettings.FindProperty(prop_PVREnvRefPointCount).SetSerializedProperty(tempLightmappingSettingsData.environmentRefPoints);
#endif

#if UNITY_2018_1_OR_NEWER
                LightmapEditorSettings.bounces                              = tempLightmappingSettingsData.bounces;
#else
                so_lightmapSettings.FindProperty(prop_PVRBounce).SetSerializedProperty(tempLightmappingSettingsData.bounces);
#endif

#if UNITY_2018_2_OR_NEWER
                LightmapEditorSettings.filteringMode                        = tempLightmappingSettingsData.filtering;
#else
                so_lightmapSettings.FindProperty(prop_PVRFilteringMode).SetSerializedProperty((int)tempLightmappingSettingsData.filtering);
#endif

#if UNITY_2019_1_OR_NEWER
                LightmapEditorSettings.denoiserTypeDirect                   = tempLightmappingSettingsData.directDenoiser;
#endif

                LightmapEditorSettings.filterTypeDirect                     = tempLightmappingSettingsData.directFilter;

#if UNITY_2018_2_OR_NEWER
                LightmapEditorSettings.filteringAtrousPositionSigmaDirect   = tempLightmappingSettingsData.directSigma;
                LightmapEditorSettings.filteringGaussRadiusDirect           = tempLightmappingSettingsData.directRadius;
#else
                so_lightmapSettings.FindProperty(prop_PVRAtrousDirectSigma).SetSerializedProperty(tempLightmappingSettingsData.directSigma);
                so_lightmapSettings.FindProperty(prop_PVRGaussRDirect).SetSerializedProperty(tempLightmappingSettingsData.directRadius);
#endif

#if UNITY_2019_1_OR_NEWER
                LightmapEditorSettings.denoiserTypeIndirect                 = tempLightmappingSettingsData.indirectDenoiser;
#endif

                LightmapEditorSettings.filterTypeIndirect                   = tempLightmappingSettingsData.indirectFilter;

#if UNITY_2018_2_OR_NEWER
                LightmapEditorSettings.filteringAtrousPositionSigmaIndirect = tempLightmappingSettingsData.indirectSigma;
                LightmapEditorSettings.filteringGaussRadiusIndirect         = tempLightmappingSettingsData.indirectRadius;
#else
                so_lightmapSettings.FindProperty(prop_PVRAtrousIndirectSigma).SetSerializedProperty(tempLightmappingSettingsData.indirectSigma);
                so_lightmapSettings.FindProperty(prop_PVRGaussRIndirect).SetSerializedProperty(tempLightmappingSettingsData.indirectRadius);
#endif

#if UNITY_2019_1_OR_NEWER
                LightmapEditorSettings.denoiserTypeAO                       = tempLightmappingSettingsData.aoDenoiser;
#endif

                LightmapEditorSettings.filterTypeAO                         = tempLightmappingSettingsData.aoFilter;

#if UNITY_2018_2_OR_NEWER
                LightmapEditorSettings.filteringAtrousPositionSigmaAO       = tempLightmappingSettingsData.aoSigma;
                LightmapEditorSettings.filteringGaussRadiusAO               = tempLightmappingSettingsData.aoRadius;
#else
                so_lightmapSettings.FindProperty(prop_PVRAtrousAOSigma).SetSerializedProperty(tempLightmappingSettingsData.aoSigma);
                so_lightmapSettings.FindProperty(prop_PVRGaussRAO).SetSerializedProperty(tempLightmappingSettingsData.aoRadius);
#endif

                LightmapEditorSettings.realtimeResolution                   = tempLightmappingSettingsData.indirectResolution;
                LightmapEditorSettings.bakeResolution                       = tempLightmappingSettingsData.lightmapResolution;
                LightmapEditorSettings.padding                              = tempLightmappingSettingsData.lightmapPadding;

#if UNITY_2018_1_OR_NEWER
                LightmapEditorSettings.maxAtlasSize                         = tempLightmappingSettingsData.lightmapSize;
#else
                LightmapEditorSettings.maxAtlasWidth                        = tempLightmappingSettingsData.lightmapSize;
                LightmapEditorSettings.maxAtlasHeight                       = tempLightmappingSettingsData.lightmapSize;
#endif

                LightmapEditorSettings.textureCompression                   = tempLightmappingSettingsData.compressLightmaps;
                LightmapEditorSettings.enableAmbientOcclusion               = tempLightmappingSettingsData.enableAO;
                LightmapEditorSettings.aoMaxDistance                        = tempLightmappingSettingsData.aoMaxDistance;
                LightmapEditorSettings.aoExponentIndirect                   = tempLightmappingSettingsData.aoIndirectContribution;
                LightmapEditorSettings.aoExponentDirect                     = tempLightmappingSettingsData.aoDirectContribution;

#if UNITY_2018_1_OR_NEWER
                LightmapEditorSettings.lightmapsMode                        = tempLightmappingSettingsData.directionalMode;
#else
                so_lightmapSettings.FindProperty(prop_LightmapsBakeMode).SetSerializedProperty((int)tempLightmappingSettingsData.directionalMode);
#endif

                Lightmapping.indirectOutputScale                            = tempLightmappingSettingsData.indirectIntensity;
                Lightmapping.bounceBoost                                    = tempLightmappingSettingsData.albedoBoost;

                so_lightmapSettings.FindProperty(prop_LightmapParams).SetSerializedProperty(tempLightmappingSettingsData.lightmapParameters);

#if UNITY_2019_1_OR_NEWER
                so_lightmapSettings.FindProperty(prop_ExportTrainingData).SetSerializedProperty(tempLightmappingSettingsData.exportTrainingData);
                so_lightmapSettings.FindProperty(prop_ExtractAO).SetSerializedProperty(tempLightmappingSettingsData.extractAmbientOcclusion);
#endif
            }

            if (doImportOtherSettingsData == true)
            {
                // Other Settings =====================================================================
                RenderSettings.fog                                          = tempOtherSettingsData.fog;
                RenderSettings.fogColor                                     = tempOtherSettingsData.fogColor;
                RenderSettings.fogMode                                      = tempOtherSettingsData.fogMode;
                RenderSettings.fogDensity                                   = tempOtherSettingsData.fogDensity;
                RenderSettings.fogStartDistance                             = tempOtherSettingsData.fogStart;
                RenderSettings.fogEndDistance                               = tempOtherSettingsData.fogEnd;
                so_renderSettings.FindProperty(prop_HaloTex).SetSerializedProperty(tempOtherSettingsData.haloTexture);
                RenderSettings.haloStrength                                 = tempOtherSettingsData.haloStrength;
                RenderSettings.flareFadeSpeed                               = tempOtherSettingsData.flareFadeSpeed;
                RenderSettings.flareStrength                                = tempOtherSettingsData.flareStrength;
                so_renderSettings.FindProperty(prop_SpotCookie).SetSerializedProperty(tempOtherSettingsData.spotCookie);
                Lightmapping.giWorkflowMode                                 = tempOtherSettingsData.gIWorkflowMode;
            }

            so_lightmapSettings.ApplyModifiedProperties();
            so_renderSettings.ApplyModifiedProperties();
        }

        public static void SetSceneLights(SceneLightingData importSLD)
        {
            var sceneLightDataArray = importSLD.sceneLightData;
            foreach (var sceneLightData in sceneLightDataArray)
            {
                var lightGameObject = new GameObject();
                var lightTransform  = lightGameObject.transform;
                var lightComponent  = lightGameObject.AddComponent<Light>();

                var so_light        = new SerializedObject(lightComponent);
                so_light.Update();

                lightGameObject.name                     = sceneLightData.gameObjectData.name;
                lightGameObject.tag                      = sceneLightData.gameObjectData.tag;
                lightGameObject.layer                    = sceneLightData.gameObjectData.layer;

                lightTransform.localPosition             = sceneLightData.transformData.worldPosition.ToVector3();
                lightTransform.localRotation             = sceneLightData.transformData.worldRotation.ToQuaternion();
                lightTransform.localScale                = sceneLightData.transformData.worldScale.ToVector3();

                lightComponent.enabled                   = sceneLightData.enabled;
                lightComponent.type                      = sceneLightData.type;
                lightComponent.color                     = sceneLightData.color;
                lightComponent.intensity                 = sceneLightData.intensity;
                lightComponent.range                     = sceneLightData.range;
                lightComponent.spotAngle                 = sceneLightData.spotAngle;
#if UNITY_2019_1_OR_NEWER
                lightComponent.innerSpotAngle            = sceneLightData.innerSpotAngle;
#endif
                lightComponent.cookieSize                = sceneLightData.cookieSize;

                lightComponent.shadows                   = sceneLightData.shadow.type;
                lightComponent.shadowResolution          = sceneLightData.shadow.resolution;
                lightComponent.shadowCustomResolution    = sceneLightData.shadow.customResolution;
                lightComponent.shadowStrength            = sceneLightData.shadow.strength;
                lightComponent.shadowBias                = sceneLightData.shadow.bias;
                lightComponent.shadowNormalBias          = sceneLightData.shadow.normalBias;
                lightComponent.shadowNearPlane           = sceneLightData.shadow.nearPlane;
#if UNITY_2019_1_OR_NEWER
                lightComponent.shadowMatrixOverride      = sceneLightData.shadow.cullingMatrixOverride.ToMatrix4x4();
                lightComponent.useShadowMatrixOverride   = sceneLightData.shadow.useCullingMatrixOverride;
#endif
                lightComponent.cookie                    = sceneLightData.cookie;

                so_light.FindProperty("m_DrawHalo").SetSerializedProperty(sceneLightData.drawHalo);

                lightComponent.bakingOutput              = new LightBakingOutput()
                {
                    probeOcclusionLightIndex = sceneLightData.bakingOutput.probeOcclusionLightIndex,
                    occlusionMaskChannel     = sceneLightData.bakingOutput.occlusionMaskChannel,
                    lightmapBakeType         = sceneLightData.bakingOutput.lightmapBakeType,
                    mixedLightingMode        = sceneLightData.bakingOutput.mixedLightingMode,
                    isBaked                  = sceneLightData.bakingOutput.isBaked
                };

                lightComponent.flare                     = sceneLightData.flare;
                lightComponent.renderMode                = sceneLightData.renderMode;
                lightComponent.cullingMask               = sceneLightData.cullingMask;
#if UNITY_2019_1_OR_NEWER
                lightComponent.renderingLayerMask        = sceneLightData.renderingLayerMask;
#endif
                lightComponent.lightmapBakeType          = sceneLightData.lightmapping;
#if UNITY_2018_2_OR_NEWER
                lightComponent.lightShadowCasterMode     = sceneLightData.lightShadowCasterMode;
#endif
                lightComponent.areaSize                  = sceneLightData.areaSize.ToVector2();
                lightComponent.bounceIntensity           = sceneLightData.bounceIntensity;
#if UNITY_2019_1_OR_NEWER
                lightComponent.boundingSphereOverride    = sceneLightData.boundingSphereOverride.ToVector4();
                lightComponent.useBoundingSphereOverride = sceneLightData.useBoundingSphereOverride;
#endif

#if UNITY_2018_1_OR_NEWER
                lightComponent.shadowRadius              = sceneLightData.shadowRadius;
                lightComponent.shadowAngle               = sceneLightData.shadowAngle;
#endif

                Undo.RegisterCreatedObjectUndo(lightGameObject, "ImportSceneLightSetting");
            }
        }

        public static void SetSceneLightProbeGroups(SceneLightingData importSLD)
        {
            var sceneLightProbeDataArray = importSLD.sceneLightProbeData;
            foreach (var sceneLightProbeData in sceneLightProbeDataArray)
            {
                var lightProbeGameObject = new GameObject();
                var lightProbeTransform  = lightProbeGameObject.transform;
                var lightProbeComponent  = lightProbeGameObject.AddComponent<LightProbeGroup>();

                lightProbeGameObject.name          = sceneLightProbeData.gameObjectData.name;
                lightProbeGameObject.tag           = sceneLightProbeData.gameObjectData.tag;
                lightProbeGameObject.layer         = sceneLightProbeData.gameObjectData.layer;

                lightProbeTransform.localPosition  = sceneLightProbeData.transformData.worldPosition.ToVector3();
                lightProbeTransform.localRotation  = sceneLightProbeData.transformData.worldRotation.ToQuaternion();
                lightProbeTransform.localScale     = sceneLightProbeData.transformData.worldScale.ToVector3();

                lightProbeComponent.enabled        = sceneLightProbeData.enabled;
#if UNITY_2019_1_OR_NEWER
                lightProbeComponent.dering         = sceneLightProbeData.dering;
#endif
                lightProbeComponent.probePositions = sceneLightProbeData.probePositions.ToVector3Array();

                Undo.RegisterCreatedObjectUndo(lightProbeGameObject, "ImportSceneLightSetting");
            }
        }

        public static void SetSceneReflectionProbes(SceneLightingData importSLD)
        {
            var sceneReflectionProbeDataArray = importSLD.sceneReflectionProbeData;
            foreach (var sceneReflectionProbeData in sceneReflectionProbeDataArray)
            {
                var reflectionProbeGameObject = new GameObject();
                var reflectionProbeTransform  = reflectionProbeGameObject.transform;
                var reflectionProbeComponent  = reflectionProbeGameObject.AddComponent<ReflectionProbe>();
                var so_reflectionProbe        = new SerializedObject(reflectionProbeComponent);
                so_reflectionProbe.Update();

                reflectionProbeGameObject.name              = sceneReflectionProbeData.gameObjectData.name;
                reflectionProbeGameObject.tag               = sceneReflectionProbeData.gameObjectData.tag;
                reflectionProbeGameObject.layer             = sceneReflectionProbeData.gameObjectData.layer;

                reflectionProbeTransform.localPosition      = sceneReflectionProbeData.transformData.worldPosition.ToVector3();
                reflectionProbeTransform.localRotation      = sceneReflectionProbeData.transformData.worldRotation.ToQuaternion();
                reflectionProbeTransform.localScale         = sceneReflectionProbeData.transformData.worldScale.ToVector3();

                reflectionProbeComponent.enabled            = sceneReflectionProbeData.enabled;
                so_reflectionProbe.FindProperty("m_Type").SetSerializedProperty(sceneReflectionProbeData.type);

                reflectionProbeComponent.mode               = sceneReflectionProbeData.mode;
                reflectionProbeComponent.refreshMode        = sceneReflectionProbeData.refreshMode;
                reflectionProbeComponent.timeSlicingMode    = sceneReflectionProbeData.timeSlicingMode;
                reflectionProbeComponent.resolution         = sceneReflectionProbeData.resolution;

                so_reflectionProbe.FindProperty("m_UpdateFrequency").SetSerializedProperty(sceneReflectionProbeData.updateFrequency);

                reflectionProbeComponent.size               = sceneReflectionProbeData.boxSize.ToVector3();
                reflectionProbeComponent.center             = sceneReflectionProbeData.boxOffset.ToVector3();
                reflectionProbeComponent.nearClipPlane      = sceneReflectionProbeData.nearClip;
                reflectionProbeComponent.farClipPlane       = sceneReflectionProbeData.farClip;
                reflectionProbeComponent.shadowDistance     = sceneReflectionProbeData.shadowDistance;
                reflectionProbeComponent.clearFlags         = sceneReflectionProbeData.clearFlags;
                reflectionProbeComponent.backgroundColor    = sceneReflectionProbeData.backgroundColor;
                reflectionProbeComponent.cullingMask        = sceneReflectionProbeData.cullingMask;
                reflectionProbeComponent.intensity          = sceneReflectionProbeData.intensityMultiplier;
                reflectionProbeComponent.blendDistance      = sceneReflectionProbeData.blendDistance;
                reflectionProbeComponent.hdr                = sceneReflectionProbeData.hdr;
                reflectionProbeComponent.boxProjection      = sceneReflectionProbeData.boxProjection;

                so_reflectionProbe.FindProperty("m_RenderDynamicObjects").SetSerializedProperty(sceneReflectionProbeData.renderDynamicObjects);
                so_reflectionProbe.FindProperty("m_UseOcclusionCulling").SetSerializedProperty(sceneReflectionProbeData.useOcclusionCulling);

                reflectionProbeComponent.importance         = sceneReflectionProbeData.importance;
                reflectionProbeComponent.customBakedTexture = sceneReflectionProbeData.customBakedTexture;
                reflectionProbeComponent.bakedTexture       = sceneReflectionProbeData.bakedTexture;

                Undo.RegisterCreatedObjectUndo(reflectionProbeGameObject, "ImportSceneLightSetting");
            }
        }

        /// <summary>
        /// シーン内の Light, LightProbeGroup, ReflectionProbe のオブジェクトを削除する
        /// </summary>
        public static void DeleteExistingLights(bool doDeleteLights, bool doDeleteLightProbeGroups, bool doDeleteReflectionProbes)
        {
            if (doDeleteLights == true)
            {
                var sceneLights = GameObject.FindObjectsOfType(typeof(Light)) as Light[];
                for (var a = 0; a < sceneLights.Length; a++)
                {
                    Undo.DestroyObjectImmediate(sceneLights[a].gameObject);
                }
            }

            if (doDeleteLightProbeGroups == true)
            {
                var sceneLightProbeGroups = GameObject.FindObjectsOfType(typeof(LightProbeGroup)) as LightProbeGroup[];
                foreach (var sceneLightProbeGroup in sceneLightProbeGroups)
                {
                    Undo.DestroyObjectImmediate(sceneLightProbeGroup.gameObject);
                }
            }

            if (doDeleteReflectionProbes == true)
            {
                var sceneReflectionProbes = GameObject.FindObjectsOfType(typeof(ReflectionProbe)) as ReflectionProbe[];
                foreach (var sceneReflectionProbe in sceneReflectionProbes)
                {
                    Undo.DestroyObjectImmediate(sceneReflectionProbe.gameObject);
                }
            }
        }
    }
}