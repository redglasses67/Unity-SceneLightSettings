using System.Reflection;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

namespace SceneLightSettings
{
    public class SceneLightSettingExporter
    {
        private static readonly string prop_EnvLightMode      = "m_GISettings.m_EnvironmentLightingMode";
        private static readonly string prop_FG                = "m_LightmapEditorSettings.m_FinalGather";
        private static readonly string prop_FGRayCount        = "m_LightmapEditorSettings.m_FinalGatherRayCount";
        private static readonly string prop_FGFilter          = "m_LightmapEditorSettings.m_FinalGatherFiltering";
        private static readonly string prop_PVREnvMIS         = "m_LightmapEditorSettings.m_PVREnvironmentMIS";
        private static readonly string prop_PVREnvSampleCount = "m_LightmapEditorSettings.m_PVREnvironmentSampleCount";  
        private static readonly string prop_LightParam        = "m_LightmapEditorSettings.m_LightmapParameters";
        private static readonly string prop_HaloTex           = "m_HaloTexture";
        private static readonly string prop_SpotCookie        = "m_SpotCookie";

        private static readonly BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static;


        private static SerializedObject GetSerializedLightmapSettingsObject()
        {
            var getLightmapSettings = typeof(LightmapEditorSettings).GetMethod("GetLightmapSettings", flags);
            var lightmapSettings    = (Object)getLightmapSettings.Invoke(null, null);
            return new SerializedObject(lightmapSettings);
        }

        private static SerializedObject GetSerializedRenderSettingsObject()
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

            var so_lightmapSettings          = GetSerializedLightmapSettingsObject();
            var so_renderSettings            = GetSerializedRenderSettingsObject();

            // Environment ========================================================================
            tempEnvironmentData.skyboxMaterial               = RenderSettings.skybox;
            tempEnvironmentData.sunSource                    = RenderSettings.sun;
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
            tempLightingData.lightingMode                = LightmapEditorSettings.mixedBakeMode;
            tempLightingData.realtimeShadowColor         = RenderSettings.subtractiveShadowColor;


            // Lightmapping Settings ==============================================================
            tempLightmappingSettingsData.lightmapper                 = LightmapEditorSettings.lightmapper;
            tempLightmappingSettingsData.finalGather                 = so_lightmapSettings.FindProperty(prop_FG).ToNullableBool();
            tempLightmappingSettingsData.finalGatherRayCount         = so_lightmapSettings.FindProperty(prop_FGRayCount).ToNullableInt();
            tempLightmappingSettingsData.finalGatherDenoising        = so_lightmapSettings.FindProperty(prop_FGFilter).ToNullableBool();
            tempLightmappingSettingsData.prioritizeView              = LightmapEditorSettings.prioritizeView;
            tempLightmappingSettingsData.multipleImportanceSampling  = so_lightmapSettings.FindProperty(prop_PVREnvMIS).ToNullableBool();
            tempLightmappingSettingsData.directSamples               = LightmapEditorSettings.directSampleCount;
            tempLightmappingSettingsData.indirectSamples             = LightmapEditorSettings.indirectSampleCount;
            tempLightmappingSettingsData.environmentSamples          = so_lightmapSettings.FindProperty(prop_PVREnvSampleCount).ToNullableInt();
            tempLightmappingSettingsData.bounces                     = LightmapEditorSettings.bounces;
            tempLightmappingSettingsData.filtering                   = LightmapEditorSettings.filteringMode;
            tempLightmappingSettingsData.directDenoiser              = LightmapEditorSettings.denoiserTypeDirect;
            tempLightmappingSettingsData.directFilter                = LightmapEditorSettings.filterTypeDirect;
            tempLightmappingSettingsData.directSigma                 = LightmapEditorSettings.filteringAtrousPositionSigmaDirect;
            tempLightmappingSettingsData.directRadius                = LightmapEditorSettings.filteringGaussRadiusDirect;
            tempLightmappingSettingsData.indirectDenoiser            = LightmapEditorSettings.denoiserTypeIndirect;
            tempLightmappingSettingsData.indirectFilter              = LightmapEditorSettings.filterTypeIndirect;
            tempLightmappingSettingsData.indirectSigma               = LightmapEditorSettings.filteringAtrousPositionSigmaIndirect;
            tempLightmappingSettingsData.indirectRadius              = LightmapEditorSettings.filteringGaussRadiusIndirect;

            tempLightmappingSettingsData.aoDenoiser                  = LightmapEditorSettings.denoiserTypeAO;
            tempLightmappingSettingsData.aoFilter                    = LightmapEditorSettings.filterTypeAO;
            tempLightmappingSettingsData.aoSigma                     = LightmapEditorSettings.filteringAtrousPositionSigmaAO;
            tempLightmappingSettingsData.aoRadius                    = LightmapEditorSettings.filteringGaussRadiusAO;

            tempLightmappingSettingsData.indirectResolution          = LightmapEditorSettings.realtimeResolution;
            tempLightmappingSettingsData.lightmapResolution          = LightmapEditorSettings.bakeResolution;
            tempLightmappingSettingsData.lightmapPadding             = LightmapEditorSettings.padding;
            tempLightmappingSettingsData.lightmapSize                = LightmapEditorSettings.maxAtlasSize;
            tempLightmappingSettingsData.compressLightmaps           = LightmapEditorSettings.textureCompression;

            tempLightmappingSettingsData.enableAO                    = LightmapEditorSettings.enableAmbientOcclusion;
            tempLightmappingSettingsData.aoMaxDistance               = LightmapEditorSettings.aoMaxDistance;
            tempLightmappingSettingsData.aoIndirectContribution      = LightmapEditorSettings.aoExponentIndirect;
            tempLightmappingSettingsData.aoDirectContribution        = LightmapEditorSettings.aoExponentDirect;

            tempLightmappingSettingsData.directionalMode             = LightmapEditorSettings.lightmapsMode;
            tempLightmappingSettingsData.indirectIntensity           = Lightmapping.indirectOutputScale;
            tempLightmappingSettingsData.albedoBoost                 = Lightmapping.bounceBoost;
            tempLightmappingSettingsData.lightmapParameters          = (LightmapParameters)so_lightmapSettings.FindProperty(prop_LightParam).objectReferenceValue;


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


            var exportSLD = ScriptableObject.CreateInstance<SceneLightingData>();
            exportSLD.environmentData          = (isExportLightingData == true) ? tempEnvironmentData : null;
            exportSLD.lightingData             = (isExportLightingData == true) ? tempLightingData : null;
            exportSLD.lightmappingSettingsData = (isExportLightingData == true) ? tempLightmappingSettingsData : null;
            exportSLD.otherSettingsData        = (isExportLightingData == true) ? tempOtherSettingsData : null;
            exportSLD.sceneLightData           = GetLightDataArray(isExportLights);
            exportSLD.sceneLightProbeData      = GetLightProbeDataArray(isExportLightProbeGroups);
            exportSLD.sceneReflectionProbeData = GetReflectionProbeDataArray(isExportReflectionProbes);
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
                tempSceneLightData.innerSpotAngle            = sceneLight.innerSpotAngle;
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
                    cullingMatrixOverride    = sceneLight.shadowMatrixOverride.ToFloatArray(),
                    useCullingMatrixOverride = sceneLight.useShadowMatrixOverride
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
                tempSceneLightData.renderingLayerMask        = sceneLight.renderingLayerMask;
                tempSceneLightData.lightmapping              = sceneLight.lightmapBakeType;
                tempSceneLightData.lightShadowCasterMode     = sceneLight.lightShadowCasterMode;
                tempSceneLightData.areaSize                  = sceneLight.areaSize.ToFloatArray();
                tempSceneLightData.bounceIntensity           = sceneLight.bounceIntensity;
                tempSceneLightData.boundingSphereOverride    = sceneLight.boundingSphereOverride.ToFloatArray();
                tempSceneLightData.useBoundingSphereOverride = sceneLight.useBoundingSphereOverride;
                tempSceneLightData.shadowRadius              = sceneLight.shadowRadius;
                tempSceneLightData.shadowAngle               = sceneLight.shadowAngle;

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
                tempSceneLightProbeGroup.dering         = sceneLightProbeGroup.dering;
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


        public static void SetSceneLightingData(SceneLightingData importSLD)
        {
            var tempEnvironmentData          = importSLD.environmentData;
            var tempLightingData             = importSLD.lightingData;
            var tempLightmappingSettingsData = importSLD.lightmappingSettingsData;
            var tempOtherSettingsData        = importSLD.otherSettingsData;

            var so_lightmapSettings = GetSerializedLightmapSettingsObject();
            so_lightmapSettings.Update();

            var so_renderSettings   = GetSerializedRenderSettingsObject();
            so_renderSettings.Update();

            // Environment ========================================================================
            RenderSettings.skybox                                       = tempEnvironmentData.skyboxMaterial;
            RenderSettings.sun                                          = tempEnvironmentData.sunSource;
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

            // Realtime & Mixed Lighting ==========================================================
            Lightmapping.realtimeGI                                     = tempLightingData.realtimeGlobalIllumination;
            Lightmapping.bakedGI                                        = tempLightingData.bakedGlobalIllumination;
            LightmapEditorSettings.mixedBakeMode                        = tempLightingData.lightingMode;
            RenderSettings.subtractiveShadowColor                       = tempLightingData.realtimeShadowColor;

            // Lightmapping Settings ==============================================================
            LightmapEditorSettings.lightmapper                          = tempLightmappingSettingsData.lightmapper;
            so_lightmapSettings.FindProperty(prop_FG).SetSerializedProperty(tempLightmappingSettingsData.finalGather);
            so_lightmapSettings.FindProperty(prop_FGRayCount).SetSerializedProperty(tempLightmappingSettingsData.finalGatherRayCount);
            so_lightmapSettings.FindProperty(prop_FGFilter).SetSerializedProperty(tempLightmappingSettingsData.finalGatherDenoising);
            LightmapEditorSettings.prioritizeView                       = tempLightmappingSettingsData.prioritizeView;
            so_lightmapSettings.FindProperty(prop_PVREnvMIS).SetSerializedProperty(tempLightmappingSettingsData.multipleImportanceSampling);
            LightmapEditorSettings.directSampleCount                    = tempLightmappingSettingsData.directSamples;
            LightmapEditorSettings.indirectSampleCount                  = tempLightmappingSettingsData.indirectSamples;
            so_lightmapSettings.FindProperty(prop_PVREnvSampleCount).SetSerializedProperty(tempLightmappingSettingsData.environmentSamples);
            LightmapEditorSettings.bounces                              = tempLightmappingSettingsData.bounces;
            LightmapEditorSettings.filteringMode                        = tempLightmappingSettingsData.filtering;
            LightmapEditorSettings.denoiserTypeDirect                   = tempLightmappingSettingsData.directDenoiser;
            LightmapEditorSettings.filterTypeDirect                     = tempLightmappingSettingsData.directFilter;
            LightmapEditorSettings.filteringAtrousPositionSigmaDirect   = tempLightmappingSettingsData.directSigma;
            LightmapEditorSettings.filteringGaussRadiusDirect           = tempLightmappingSettingsData.directRadius;
            LightmapEditorSettings.denoiserTypeIndirect                 = tempLightmappingSettingsData.indirectDenoiser;
            LightmapEditorSettings.filterTypeIndirect                   = tempLightmappingSettingsData.indirectFilter;
            LightmapEditorSettings.filteringAtrousPositionSigmaIndirect = tempLightmappingSettingsData.indirectSigma;
            LightmapEditorSettings.filteringGaussRadiusIndirect         = tempLightmappingSettingsData.indirectRadius;
            LightmapEditorSettings.denoiserTypeAO                       = tempLightmappingSettingsData.aoDenoiser;
            LightmapEditorSettings.filterTypeAO                         = tempLightmappingSettingsData.aoFilter;
            LightmapEditorSettings.filteringAtrousPositionSigmaAO       = tempLightmappingSettingsData.aoSigma;
            LightmapEditorSettings.filteringGaussRadiusAO               = tempLightmappingSettingsData.aoRadius;
            LightmapEditorSettings.realtimeResolution                   = tempLightmappingSettingsData.indirectResolution;
            LightmapEditorSettings.bakeResolution                       = tempLightmappingSettingsData.lightmapResolution;
            LightmapEditorSettings.padding                              = tempLightmappingSettingsData.lightmapPadding;
            LightmapEditorSettings.maxAtlasSize                         = tempLightmappingSettingsData.lightmapSize;
            LightmapEditorSettings.textureCompression                   = tempLightmappingSettingsData.compressLightmaps;
            LightmapEditorSettings.enableAmbientOcclusion               = tempLightmappingSettingsData.enableAO;
            LightmapEditorSettings.aoMaxDistance                        = tempLightmappingSettingsData.aoMaxDistance;
            LightmapEditorSettings.aoExponentIndirect                   = tempLightmappingSettingsData.aoIndirectContribution;
            LightmapEditorSettings.aoExponentDirect                     = tempLightmappingSettingsData.aoDirectContribution;
            LightmapEditorSettings.lightmapsMode                        = tempLightmappingSettingsData.directionalMode;
            Lightmapping.indirectOutputScale                            = tempLightmappingSettingsData.indirectIntensity;
            Lightmapping.bounceBoost                                    = tempLightmappingSettingsData.albedoBoost;
            so_lightmapSettings.FindProperty(prop_LightParam).SetSerializedProperty(tempLightmappingSettingsData.lightmapParameters);

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
                lightComponent.innerSpotAngle            = sceneLightData.innerSpotAngle;
                lightComponent.cookieSize                = sceneLightData.cookieSize;

                lightComponent.shadows                   = sceneLightData.shadow.type;
                lightComponent.shadowResolution          = sceneLightData.shadow.resolution;
                lightComponent.shadowCustomResolution    = sceneLightData.shadow.customResolution;
                lightComponent.shadowStrength            = sceneLightData.shadow.strength;
                lightComponent.shadowBias                = sceneLightData.shadow.bias;
                lightComponent.shadowNormalBias          = sceneLightData.shadow.normalBias;
                lightComponent.shadowNearPlane           = sceneLightData.shadow.nearPlane;
                lightComponent.shadowMatrixOverride      = sceneLightData.shadow.cullingMatrixOverride.ToMatrix4x4();
                lightComponent.useShadowMatrixOverride   = sceneLightData.shadow.useCullingMatrixOverride;
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
                lightComponent.renderingLayerMask        = sceneLightData.renderingLayerMask;
                lightComponent.lightmapBakeType          = sceneLightData.lightmapping;
                lightComponent.lightShadowCasterMode     = sceneLightData.lightShadowCasterMode;
                lightComponent.areaSize                  = sceneLightData.areaSize.ToVector2();
                lightComponent.bounceIntensity           = sceneLightData.bounceIntensity;
                lightComponent.boundingSphereOverride    = sceneLightData.boundingSphereOverride.ToVector4();
                lightComponent.useBoundingSphereOverride = sceneLightData.useBoundingSphereOverride;
                lightComponent.shadowRadius              = sceneLightData.shadowRadius;
                lightComponent.shadowAngle               = sceneLightData.shadowAngle;

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
                lightProbeComponent.dering         = sceneLightProbeData.dering;
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
        public static void DeleteExistingLights()
        {
            var sceneLights           = GameObject.FindObjectsOfType(typeof(Light)) as Light[];
            for (var a = 0; a < sceneLights.Length; a++)
            {
                Undo.DestroyObjectImmediate(sceneLights[a].gameObject);
            }

            var sceneLightProbeGroups = GameObject.FindObjectsOfType(typeof(LightProbeGroup)) as LightProbeGroup[];
            foreach (var sceneLightProbeGroup in sceneLightProbeGroups)
            {
                Undo.DestroyObjectImmediate(sceneLightProbeGroup.gameObject);
            }

            var sceneReflectionProbes = GameObject.FindObjectsOfType(typeof(ReflectionProbe)) as ReflectionProbe[];
            foreach (var sceneReflectionProbe in sceneReflectionProbes)
            {
                Undo.DestroyObjectImmediate(sceneReflectionProbe.gameObject);
            }
        }
    }
}