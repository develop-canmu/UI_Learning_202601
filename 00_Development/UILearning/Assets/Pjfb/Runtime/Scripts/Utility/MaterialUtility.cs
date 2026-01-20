using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Object = System.Object;

namespace Pjfb.Utility
{
    //// <summary> マテリアルのプロパティ情報 </summary>
    [Serializable]
    public class MaterialPropertyInfo
    {
        // マテリアルプロパティの共通インターフェース
        public interface IMaterialProperty
        {
            public Object GetValue();
        }

        [Serializable]
        public struct PropertyColor : IMaterialProperty
        {
            [SerializeField] private Color color;
            public Color Color => color;

            public PropertyColor(Color color)
            {
                this.color = color;
            }

            public Object GetValue()
            {
                return color;
            }
        }

        [Serializable]
        public struct PropertyFloat : IMaterialProperty
        {
            [SerializeField] private float value;
            public float Value => value;
            
            public PropertyFloat(float value)
            {
                this.value = value;
            }

            public Object GetValue()
            {
                return value;
            }
        }

        [Serializable]
        public struct PropertyInt : IMaterialProperty
        {
            [SerializeField] private int value;
            public int Value => value;
            
            public PropertyInt(int value)
            {
                this.value = value;
            }

            public Object GetValue()
            {
                return value;
            }
        }

        [Serializable]
        public class PropertyTexture : IMaterialProperty
        {
            [SerializeField] private Texture texture;
            [SerializeField] private Vector2 tiling;
            [SerializeField] private Vector2 offset;

            public Texture Texture => texture;
            public Vector2 Tiling => tiling;
            public Vector2 Offset => offset;
            
            public PropertyTexture(Texture texture, Vector2 tiling, Vector2 offset)
            {
                this.texture = texture;
                this.tiling = tiling;
                this.offset = offset;
            }

            public Object GetValue()
            {
                return this;
            }
        }

        [Serializable]
        public struct PropertyVector : IMaterialProperty
        {
            [SerializeField] private Vector4 value;
            public Vector4 Value => value;
            
            public PropertyVector(Vector4 value)
            {
                this.value = value;
            }

            public Object GetValue()
            {
                return value;
            }
        }


        // プロパティ値
        [SerializeReference] private IMaterialProperty property = null;
        public IMaterialProperty Property => property;

        // プロパティタイプ
        [SerializeField] private ShaderPropertyType propertyType;
        public ShaderPropertyType PropertyType => propertyType;

        // プロパティの名前
        [SerializeField] private string propertyName = string.Empty;
        public string PropertyName => propertyName;

        public MaterialPropertyInfo(IMaterialProperty property, ShaderPropertyType propertyType, string propertyName)
        {
            this.property = property;
            this.propertyType = propertyType;
            this.propertyName = propertyName;
        }
    }

    public static class MaterialUtility
    {
        //// <summary> プロパティ値の取得 </summary>
        private static Object GetProperty(this Material material, ShaderPropertyType propertyType, string name)
        {
            switch (propertyType)
            {
                case ShaderPropertyType.Color:
                {
                    return material.GetColor(name);
                }
                case ShaderPropertyType.Float:
                case ShaderPropertyType.Range:
                {
                    return material.GetFloat(name);
                }
                case ShaderPropertyType.Int:
                {
                    return material.GetInt(name);
                }
                case ShaderPropertyType.Texture:
                {
                    return material.GetTexture(name);
                }
                case ShaderPropertyType.Vector:
                {
                    return material.GetVector(name);
                }
            }

            return null;
        }

        //// <summary> マテリアルプロパティ情報を取得 </summary>
        private static MaterialPropertyInfo.IMaterialProperty GetMaterialProperty(this Material material, ShaderPropertyType propertyType, string name)
        {
            switch (propertyType)
            {
                case ShaderPropertyType.Color:
                {
                    return new MaterialPropertyInfo.PropertyColor(material.GetColor(name));
                }
                case ShaderPropertyType.Float:
                case ShaderPropertyType.Range:
                {
                    return new MaterialPropertyInfo.PropertyFloat(material.GetFloat(name));
                }
                case ShaderPropertyType.Int:
                {
                    return new MaterialPropertyInfo.PropertyInt(material.GetInt(name));
                }
                case ShaderPropertyType.Texture:
                {
                    return new MaterialPropertyInfo.PropertyTexture(material.GetTexture(name), material.GetTextureScale(name), material.GetTextureOffset(name));
                }
                case ShaderPropertyType.Vector:
                {
                    return new MaterialPropertyInfo.PropertyVector(material.GetVector(name));
                }
            }

            return null;
        }

        //// <summary> マテリアルプロパティのセット </summary>
        public static void SetMaterialProperty(this Material material, MaterialPropertyInfo propertyInfo)
        {
            Object propertyValue = propertyInfo.Property.GetValue();
            
            switch (propertyInfo.PropertyType)
            {
                case ShaderPropertyType.Color:
                {
                    material.SetColor(propertyInfo.PropertyName, (Color)propertyValue);
                    break;
                }
                case ShaderPropertyType.Float:
                case ShaderPropertyType.Range:
                {
                    material.SetFloat(propertyInfo.PropertyName, (float)propertyValue);
                    break;
                }
                case ShaderPropertyType.Int:
                {
                    material.SetInt(propertyInfo.PropertyName, (int)propertyValue);
                    break;
                }
                case ShaderPropertyType.Texture:
                {
                    MaterialPropertyInfo.PropertyTexture propertyTexture = (MaterialPropertyInfo.PropertyTexture)propertyInfo.Property.GetValue();
                    material.SetTexture(propertyInfo.PropertyName, propertyTexture.Texture);
                    material.SetTextureScale(propertyInfo.PropertyName, propertyTexture.Tiling);
                    material.SetTextureOffset(propertyInfo.PropertyName, propertyTexture.Offset);
                    break;
                }
                case ShaderPropertyType.Vector:
                {
                    material.SetVector(propertyInfo.PropertyName, (Vector4)propertyValue);
                    break;
                }
            }
        }

        //// <summary> マテリアルプロパティの比較して変更されているプロパティを返す </summary>
        public static List<MaterialPropertyInfo> CompareMaterialProperty(Material baseMaterial, Material changeMaterial, int[] exceptList)
        {
            List<MaterialPropertyInfo> changePropertyList = new List<MaterialPropertyInfo>();

            // 同一Shaderか
            if (baseMaterial.shader != changeMaterial.shader)
            {
                CruFramework.Logger.LogError("比較対象のマテリアル同士で利用しているShaderが異なっています");
                return changePropertyList;
            }

            int count = baseMaterial.shader.GetPropertyCount();

            // Shaderプロパティを見ていく
            for (int i = 0; i < count; i++)
            {
                // プロパティの型
                ShaderPropertyType propertyType = baseMaterial.shader.GetPropertyType(i);
                // プロパティ名
                string propertyName = baseMaterial.shader.GetPropertyName(i);
                // プロパティが変更されているなら変更後のプロパティ情報を追加
                if (IsChangePropertyValue(baseMaterial, changeMaterial, propertyType, propertyName))
                {
                    // 無視されるプロパティならとばす
                    if (exceptList.Contains(baseMaterial.shader.GetPropertyNameId(i)))
                    {
                        break;
                    }

                    changePropertyList.Add(new MaterialPropertyInfo(changeMaterial.GetMaterialProperty(propertyType, propertyName), propertyType, propertyName));
                }
            }

            return changePropertyList;
        }

        //// <summary> 値が異なっているか </summary>
        private static bool IsChangePropertyValue(Material baseMaterial, Material changeMaterial, ShaderPropertyType propertyType, string name)
        {
            switch (propertyType)
            {
                case ShaderPropertyType.Color:
                {
                    return baseMaterial.GetColor(name) != changeMaterial.GetColor(name);
                }
                case ShaderPropertyType.Float:
                case ShaderPropertyType.Range:
                {
                    return Mathf.Approximately(baseMaterial.GetFloat(name), changeMaterial.GetFloat(name)) == false;
                }
                case ShaderPropertyType.Int:
                {
                    return baseMaterial.GetInt(name) != changeMaterial.GetInt(name);
                }
                case ShaderPropertyType.Texture:
                {
                    // テクスチャの場合はテクスチャの変更だけでなくタイリングとオフセットの値が変更されてるかも見る
                    if (baseMaterial.GetTexture(name) != changeMaterial.GetTexture(name))
                    {
                        return true;
                    }
                    // タイリング
                    if (baseMaterial.GetTextureScale(name) != changeMaterial.GetTextureScale(name))
                    {
                        return true;
                    }
                    // オフセット
                    if (baseMaterial.GetTextureOffset(name) != changeMaterial.GetTextureOffset(name))
                    {
                        return true;
                    }
                    return false;
                }
                case ShaderPropertyType.Vector:
                {
                    return baseMaterial.GetVector(name) != changeMaterial.GetVector(name);
                }
            }

            return false;
        }
    }
}