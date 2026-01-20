using System;
using System.Collections.Generic;
using CruFramework.Addressables;
using Cysharp.Threading.Tasks;
using SRF;
using UnityEngine;
using UnityEngine.Playables;
using Object = UnityEngine.Object;

namespace Pjfb.Utility
{
    public class ObjectPoolUtility : MonoBehaviour
    {
        private class ModelCacheData
        {
            public ModelTypeEnum modelType;
            public string id;
            public Object model;
            public bool isUsed;
        }
        public enum ModelTypeEnum
        {
            low,
            middle
        }

        public enum TrainingTypeEnum
        {
            squat = 0,
            Shuttlerun,
            Dribble,
            Shoot,
            Imagetraining
        }
        private const string characterPathPattern = "3D/Character/{1}/{0}/{1}_sd_{0}_unique01.prefab";
        private const string animationPathPattern = "3D/Animation/Chara_Training/{0}_sd/{1}/{2}/{0}_{3}_{2}.anim";

        private static List<ModelCacheData> modelCache = new List<ModelCacheData>();

        private static ObjectPoolUtility instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                enabled = false;
                Destroy(this);
            }
        }

        private void OnDestroy()
        {
            if (instance != null)
            {
                instance = null;
            }
        }

        private static string GetPath(ModelTypeEnum type, string id)
        {
            return string.Format(characterPathPattern, type, id);
        }
        
        private static string GetAnimationPath(ModelTypeEnum type,TrainingTypeEnum trType, string id, string nameKey)
        {
            return string.Format(animationPathPattern, id,  trType, type, nameKey);
        }
        
        public static async UniTask<PlayableDirector> GetPlayableDirectorOnly(TrainingTypeEnum trType)
        {
            var path = $"3D/Common/Training_{trType}.prefab";
            PlayableDirector ret = default;
            if (AddressablesManager.HasResources<GameObject>(path))
            {
                await PageResourceLoadUtility.resourcesLoader.LoadAssetAsync<GameObject>(path, callback: model =>
                {
                    if (model != null)
                    {
                        ret = model.GetComponent<PlayableDirector>();
                    }else
                    {
                        CruFramework.Logger.LogError($"[GetPlayableDirectorOnly] Asset {path} can not be loaded!");
                    }
                });
            }
            else
            {
                CruFramework.Logger.LogError($"[GetPlayableDirectorOnly] Asset {path} was not found!");
            }

            return ret;
        }
        public static async UniTask<AnimationClip> GetAnimationAssetOnly(ModelTypeEnum type, TrainingTypeEnum trType, long id, string nameKey)
        {
            AnimationClip ret = default;
            var strID = id.ToString().Substring(0, 5);
            
            var path = GetAnimationPath(type, trType, strID, nameKey);
            if (AddressablesManager.HasResources<AnimationClip>(path))
            {
                await PageResourceLoadUtility.resourcesLoader.LoadAssetAsync<AnimationClip>(path, callback: model =>
                {
                    if (model != null)
                    {
                        ret = model;
                    }else
                    {
                        CruFramework.Logger.LogError($"[GetAnimationAssetOnly] Asset {path} can not be loaded!");
                    }
                });
            }
            else
            {
                CruFramework.Logger.LogError($"[GetAnimationAssetOnly] Asset {path} was not found!");
            }

            return ret;
        }
        
        public static async UniTask<Object> GetModel(ModelTypeEnum type, long id, LayerMask mask = default, bool isCachedOnly = false)
        {
            Object ret = default;
            var strID = id.ToString().Substring(0, 5);
            var cache = modelCache.Find(x => x.modelType.Equals(type) && x.id.Equals(strID) && !x.isUsed);
            if (cache == default)
            {
                var path = GetPath(type, strID);
                if (AddressablesManager.HasResources<GameObject>(path))
                {
                    await PageResourceLoadUtility.resourcesLoader.LoadAssetAsync<GameObject>(path, callback: model =>
                    {
                        if (model != null)
                        {
                            ret = Object.Instantiate(model, position: Vector3.zero, rotation: Quaternion.identity, parent: instance.transform);
                            modelCache.Add(new ModelCacheData(){id = strID, modelType = type, model = ret, isUsed = !isCachedOnly});
                            var obj = ret as GameObject;
                            obj.SetLayerRecursive(mask);
                            obj.SetActive(!isCachedOnly);
                        }
                        else
                        {
                            CruFramework.Logger.LogError($"[GetModel] Asset {path} can not be loaded!");
                        }
                    });
                }
                else
                {
                    CruFramework.Logger.LogError($"[GetModel] Asset {path} was not found!");
                }
            }
            else
            {
                ret = cache.model;
                cache.isUsed = !isCachedOnly;
                var obj = ret as GameObject;
                obj.SetLayerRecursive(mask);
                obj.SetActive(!isCachedOnly);
            }

            return ret;
        }

        public static void ReturnToPool(Object input)
        {
            var obj = input as GameObject;
            var cache = modelCache.Find(x => x.model.Equals(input));
            if (cache != default)
            {
                cache.isUsed = false;
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;
                obj.transform.SetParent(instance.transform);
                obj.SetLayerRecursive(default);
                obj.SetActive(false);
            }
        }

        public static void RegisterToPool(ModelTypeEnum type, long id, Object target, bool isUse = false)
        {
            if (target == default) return;
            var strID = id.ToString().Substring(0, 5);
            modelCache.Add(new ModelCacheData(){id = strID, modelType = type, model = target, isUsed = isUse});
            if (!isUse)
            {
                var obj = target as GameObject;
                obj.transform.SetParent(instance.transform);
            }
        }

        public static void ResetPool()
        {
            foreach (var cache in modelCache)
            {
                cache.isUsed = false;
                var obj = cache.model as GameObject;
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;
                obj.transform.SetParent(instance.transform);
                obj.SetLayerRecursive(default);
                obj.SetActive(false);
            }
        }

        public static void UnloadAll()
        {
            foreach (var cacheData in modelCache)
            {
                Object.Destroy(cacheData.model); 
            }

            modelCache.Clear();
        }
    }
}