using System;
using System.Collections.Generic;
using System.Text;

namespace Pjfb.CustomHtml
{
    public static class CustomHtmlManager
    {
        private static readonly TagData Crz7Tag = new(tag: "<crz7-");
        private static readonly TagData HSubTag = new(tag: "h>", closeTag: "</crz7-h>");
        private static readonly TagData ImgSubTag = new(tag: "img=", closeTag: ">");
        private static readonly TagData ButtonSubTag = new(tag: "button", closeTag: ">");
        private static readonly string ButtonDefaultText = "詳細を見る";
        private static readonly string OnClickTag = "onClick=";
        

        public static CustomHtmlConvertResult ConvertCustomHtmlString(string html)
        {
            var objectParamList = new List<HtmlObjectParam>();
            
            var htmlLength = html.Length;
            var tempIndex = 0;
            var textBuilder = new StringBuilder();
            for (var i = 0; i < htmlLength; i++)
            {
                if (!Crz7Tag.IsTag(html, startIndex: i)) textBuilder.Append(html[i]);
                else
                {
                    if (textBuilder.Length > 0)
                    {
                        objectParamList.Add(new HtmlObjectParam(new TextHtmlObjectParam(isTitle: false, text: textBuilder.ToString())));
                    }
                    textBuilder.Clear();

                    // 「<crz7-」 の次のインデックスになります
                    tempIndex = i + Crz7Tag.tagLength;

                    // タイトルタグを検知
                    if (HSubTag.IsTag(html, startIndex: tempIndex))
                    {
                        // 想定値：<crz7-h>...</crz7-h>
                        var tagResult = HSubTag.GetTagResult(html, startIndex: tempIndex); // タイトル文書を取得
                        objectParamList.Add(new HtmlObjectParam(new TextHtmlObjectParam(isTitle: true, text: tagResult.result)));
                        i = tagResult.lastIndex; // 次のインデックスからループを進行させる
                    }
                    else if (ImgSubTag.IsTag(html, startIndex: tempIndex))
                    {
                        // 想定値：<crz7-img=... onClick=...> or <crz7-img=...> 
                        
                        // 想定result: ... onClick=... or ...
                        var tagResult = ImgSubTag.GetTagResult(html, startIndex: tempIndex);
                        var splitParam = tagResult.result.Split(' ');
                        objectParamList.Add(new HtmlObjectParam(new ImageHtmlObjectParam(
                            imagePath: splitParam.Length >= 1 ? splitParam[0] : string.Empty,
                            onClickString: splitParam.Length >= 2 && splitParam[1].StartsWith(OnClickTag) ? splitParam[1].Substring(OnClickTag.Length) : string.Empty)));
                        i = tagResult.lastIndex;
                    }
                    else if (ButtonSubTag.IsTag(html, startIndex: tempIndex))
                    {
                        // 想定値：<crz7-button onClick=...>
                        var tagResult = ButtonSubTag.GetTagResult(html, startIndex: tempIndex);
                        var splitParam = tagResult.result.Split(' ');
                        objectParamList.Add(new HtmlObjectParam(new ButtonHtmlObjectParam(
                            buttonText: splitParam.Length >= 1 && splitParam[0].StartsWith('=') && splitParam[0].Length > 1 ? splitParam[0][1..] : ButtonDefaultText,
                            onClickString: splitParam.Length >= 2 && splitParam[1].StartsWith(OnClickTag) ? splitParam[1].Substring(OnClickTag.Length) : string.Empty)));
                        i = tagResult.lastIndex;
                    }
                }
            }
            
            if (textBuilder.Length > 0)
            {
                objectParamList.Add(new HtmlObjectParam(new TextHtmlObjectParam(isTitle: false, text: textBuilder.ToString())));
            }
            
            return new CustomHtmlConvertResult{objectParamList = objectParamList};
        }
        
        private class TagData
        {
            private string tag;
            private string closeTag;
            public int tagLength { get; }
            public int closeTagLength { get; }
            
            public TagData(string tag, string closeTag = "")
            {
                this.tag = tag;
                this.closeTag = closeTag;
                tagLength = tag.Length;
                closeTagLength = closeTag.Length;
            }

            public bool IsTag(string text, int startIndex)
            {
                // メモ：処理最適化のためにfor処理で判定する
                for (var i = 0; i < tagLength; i++)
                {
                    if (!text[startIndex + i].Equals(tag[i])) return false;
                }

                return true;
            }

            public TagResult GetTagResult(string text, int startIndex)
            {
                startIndex = startIndex + tagLength;
                var closeTagIndex = text.IndexOf(closeTag, startIndex, StringComparison.Ordinal);
                var result = closeTagIndex == -1 ? text.Substring(startIndex) : text.Substring(startIndex, length: closeTagIndex - startIndex);
                var lastIndex = closeTagIndex == -1 ? text.Length : (closeTagIndex + closeTagLength - 1);
                
                // Debug.Log(nameof(GetTagResult) + $" tag:{tag} result:{result} lastIndex:{lastIndex}\nstartIndex:{startIndex}\ntext:{text}");
                return new TagResult{result = result, lastIndex = lastIndex};
            }
        }

        private class TagResult
        {
            public string result;
            public int lastIndex;
        }
    }

    public class HtmlObjectParam
    {
        public TextHtmlObjectParam nullableText;
        public ImageHtmlObjectParam nullableImage;
        public ButtonHtmlObjectParam nullableButton;

        public HtmlObjectParam(ImageHtmlObjectParam imageParam)
        {
            nullableText = null;
            nullableImage = imageParam;
            nullableButton = null;
        }
        
        public HtmlObjectParam(TextHtmlObjectParam textParam)
        {
            nullableText = textParam;
            nullableImage = null;
            nullableButton = null;
        }
        
        public HtmlObjectParam(ButtonHtmlObjectParam buttonParam)
        {
            nullableText = null;
            nullableImage = null;
            nullableButton = buttonParam;
        }

        public override string ToString()
        {
            return $"nullableText:{nullableText}\nnullableImage:{nullableImage}\nnullableButton:{nullableButton}";
        }
    }
    
    public class CustomHtmlConvertResult
    {
        public List<HtmlObjectParam> objectParamList;

        /// <summary>
        /// デバッグとテストランナー用
        /// </summary>
        public override string ToString()
        {
            var retVal = string.Empty;
            objectParamList?.ForEach(aParam => retVal += aParam);
            return retVal;
        }
    }
    
    public class TextHtmlObjectParam
    {
        /// <summary>
        /// isTitle:trueは<crz7-h></crz7-h>に囲まれたテキスト
        /// </summary>
        public bool isTitle;
        public string text;

        public TextHtmlObjectParam(bool isTitle, string text)
        {
            // バグ対策：最後の空行はRectTransform.height計算に入らない、最後の空行にスペースを入れることで空行がなくなる対策を入れさせていただきます
            if (text.EndsWith('\n'))
            {
                text += " ";
            }
            
            this.isTitle = isTitle;
            this.text = text;
        }

        public override string ToString()
        {
            return $"[isTitle:{isTitle} text:{text}]";
        }
    }
    
    public class ImageHtmlObjectParam
    {
        public string imagePath;
        public string onClickString;

        public ImageHtmlObjectParam(string imagePath, string onClickString)
        {
            this.imagePath = imagePath;
            this.onClickString = onClickString;
        }

        public override string ToString()
        {
            return $"[imagePath:{imagePath} onClickString:{onClickString}]";
        }
    }
    
    public class ButtonHtmlObjectParam
    {
        public string buttonText;
        public string onClickString;

        public ButtonHtmlObjectParam(string buttonText, string onClickString)
        {
            this.buttonText = buttonText;
            this.onClickString = onClickString;
        }

        public override string ToString()
        {
            return $"[buttonText:{buttonText} onClickString:{onClickString}]";
        }
    }
}
