using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CruFramework.Adv
{
    /// <summary>Adv用の特殊タグ</summary>
    public enum AdvTextTagName
    {
        None = 0,
        spd = 1,
    }
    
    public class AdvTextTag
    {
        private string name = string.Empty;
        /// <summary>タグ名</summary>
        public string Name{get{return name;}set{name = value;}}
        
        private string value = string.Empty;
        /// <summary>値</summary>
        public string Value{get{return value;}set{this.value = value;}}
        
        private int indexWithTag = 0;
        /// <summary>開始位置</summary>
        public int IndexWithTag{get{return indexWithTag;}set{indexWithTag = value;}}
        
        private int index = 0;
        /// <summary>開始位置</summary>
        public int Index{get{return index;}set{index = value;}}
        
        private int tagIndex = -1;
        /// <summary>もう片方のタグ</summary>
        public int TagIndex{get{return tagIndex;}set{tagIndex = value;}}
        
        private bool isCloseTag = false;
        /// <summary>閉じるタグ</summary>
        public bool IsCloseTag{get{return isCloseTag;}set{isCloseTag = value;}}
        
        private AdvTextTagName tagType = AdvTextTagName.None;
        /// <summary>タグの種類</summary>
        public AdvTextTagName TagType{get{return tagType;}set{tagType = value;}}
        
        private bool isCompletedAttach = false;
        /// <summary>文字出力完了時にタグを追加する</summary>
        public bool IsCompletedAttach{get{return isCompletedAttach;}set{isCompletedAttach = value;}}

    }
    
    public class AdvText
    {
        
        private static bool IsCompletedAttachTab(string tag)
        {
            switch(tag)
            {
                case "r":
                case "ruby":
                    return true;
            }
            
            return false;
        }
        
        private string text = string.Empty;
        /// <summary>タグを取り除いた文字列</summary>
        public string Text{get{return text;}}
        
        private string originalText = string.Empty;
        /// <summary>元の文字列</summary>
        public string OriginalText{get{return originalText;}}
        
        // 文字列構築用
        private StringBuilder stringBuilder = new StringBuilder();
        // タグ取得用
        private StringBuilder tagStringBuilder = new StringBuilder();
        
        // タグリスト
        private List<AdvTextTag> tags = new List<AdvTextTag>();
        private List<AdvTextTag> closeTags = new List<AdvTextTag>();
        /// <summary>
        /// タグを取り除いた文字列の長さ
        /// </summary>
        public int Length{get{return text.Length;}}
        
        /// <summary>タグを取得</summary>
        public bool GetTagValue<T>(AdvTextTagName tagType, int index, out T value)
        {
            for(int i=tags.Count-1;i>=0;i--)
            {
                AdvTextTag tag = tags[i];
                if(tag.TagType == tagType && tag.Index <= index && closeTags[tag.TagIndex].Index >= index)
                {
                    value = (T)Convert.ChangeType(tag.Value, typeof(T));
                    return true;
                }
            }
            
            value = default(T);
            return false;
        }

        
        /// <summary>テキスト</summary>
        public void SetText(string text)
        {
            // 文字列
            stringBuilder.Clear();

            // タグ初期化
            tags.Clear();
            closeTags.Clear();
            
            // タグ処理中
            bool isTag = false;
            // タグを取り除いたテキストの位置
            int textIndex = 0;
            // タグ
            AdvTextTag textTag = null;
            
            // 文字列からタグを取得していく
            for(int i=0;i<text.Length;i++)
            {
                char c = text[i];

                switch(c)
                {
                    case '<':
                    {
                        tagStringBuilder.Clear();
                        isTag = true;
                        
                        textTag = new AdvTextTag();
                        textTag.Index = textIndex;
                        textTag.IndexWithTag = i;
                        if(text.Length > i + 1 && text[i + 1] == '/')
                        {
                            i++;
                            textTag.IsCloseTag = true;
                            closeTags.Add(textTag);
                        }
                        else
                        {
                            tags.Add(textTag);
                        }
                        
                        break;
                    }
                    case '>':
                    {
                        isTag = false;
                        // タグを取得
                        string[] values = tagStringBuilder.ToString().Split("=");
                        textTag.Name = values[0];
                        textTag.Value = values.Length == 2 ? values[1] : string.Empty;
                        textTag.IsCompletedAttach = IsCompletedAttachTab(textTag.Name);

                        // テキストに含むか?
                        if(System.Enum.TryParse<AdvTextTagName>(textTag.Name, out AdvTextTagName t))
                        {
                            textTag.TagType = t;
                        }
                        else
                        {
                            textTag.TagType = AdvTextTagName.None;
                        }
                        
                        // ペアを探す
                        if(textTag.IsCloseTag)
                        {
                            for(int n=tags.Count-1;n>=0;n--)
                            {
                                if(tags[n].TagIndex == -1 && tags[n].Name == textTag.Name)
                                {
                                    tags[n].TagIndex = closeTags.Count-1;
                                    textTag.TagIndex = n;
                                    break;
                                }
                            }
                        }
                        break;
                    }
                    
                    default:
                    {
                        if(isTag)
                        {
                            tagStringBuilder.Append(c);
                        }
                        else
                        {
                            stringBuilder.Append(c);
                            textIndex++;
                        }
                        break;
                    }
                }
            }
            
            // 元のテキスト
            originalText = text;
            // タグを取り除いたテキスト
            this.text = stringBuilder.ToString();
            
            // StringBuilder初期化
            stringBuilder.Clear();
            tagStringBuilder.Clear();
        }
        
        /// <summary>指定文字数分取得</summary>
        public string GetText(int count)
        {
            if(text.Length <= count)return originalText;
            
            stringBuilder.Clear();
            int tagIndex = 0;
            int closeTagIndex = 0;
            
            for(int i=0;i<count;i++)
            {
                
                // 終了タグを追加
                while(true)
                {
                    if(closeTagIndex >= tags.Count)break;
                    
                    AdvTextTag tag = closeTags[closeTagIndex];
                    // タグを挿入
                    if(tag.Index == i)
                    {
                        if(tag.TagType == AdvTextTagName.None)
                        {
                            if(tag.IsCompletedAttach)
                            {
                                AdvTextTag startTag = tags[tag.TagIndex];
                                stringBuilder.Insert(startTag.IndexWithTag, $"<{startTag.Name}{(string.IsNullOrEmpty(startTag.Value) ? string.Empty : "=")}{startTag.Value}>");
                            }
                            
                            stringBuilder.Append($"</{tag.Name}>");
                        }
                        closeTagIndex++;
                    }
                    else
                    {
                        break;
                    }
                }
                
                // 開始タグを追加
                while(true)
                {
                    if(tagIndex >= tags.Count)break;
                    
                    AdvTextTag tag = tags[tagIndex];
                    // タグを挿入
                    if(tag.Index == i)
                    {
                        if(tag.TagType == AdvTextTagName.None && tag.IsCompletedAttach == false)
                        {
                            stringBuilder.Append($"<{tag.Name}{(string.IsNullOrEmpty(tag.Value) ? string.Empty : "=")}{tag.Value}>");
                        }
                        tagIndex++;
                    }
                    else
                    {
                        break;
                    }
                }
                

                
                // 文字を追加
                if(text.Length > i)
                {
                    stringBuilder.Append(text[i]);
                }
            }
            
            // 閉じてないタグを閉じる
            foreach(AdvTextTag tag in closeTags)
            {
                if(tag.IsCompletedAttach == false && tag.TagType == AdvTextTagName.None && tag.Index >= count && tags[tag.TagIndex].Index < count)
                {
                    stringBuilder.Append($"</{tag.Name}>");
                }
            }
            
            return stringBuilder.ToString();
        }
    }
}