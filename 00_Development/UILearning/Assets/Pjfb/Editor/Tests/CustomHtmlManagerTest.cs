using System.Collections.Generic;
using NUnit.Framework;
using Pjfb.CustomHtml;

namespace Pjfb.Editor.Tests
{
    public class CustomHtmlManagerTest
    {
        [Test]
        public void TestHeaderTag_1()
        {
            var body = "<crz7-h>123</crz7-h>";
            var result = CustomHtmlManager.ConvertCustomHtmlString(body);
            var expectedResult = new CustomHtmlConvertResult
            {
                objectParamList = new List<HtmlObjectParam>
                {
                    new(new TextHtmlObjectParam(text:"123", isTitle: true)),
                }
            };
            AssertResult(expectedResult, result);
        }
        
        [Test]
        public void TestHeaderTag_2()
        {
            var body = "abc<crz7-h>123</crz7-h>def";
            var result = CustomHtmlManager.ConvertCustomHtmlString(body);
            var expectedResult = new CustomHtmlConvertResult
            {
                objectParamList = new List<HtmlObjectParam>
                {
                    new(new TextHtmlObjectParam(text:"abc", isTitle: false)),
                    new(new TextHtmlObjectParam(text:"123", isTitle: true)),
                    new(new TextHtmlObjectParam(text:"def", isTitle: false)),
                }
            };
            AssertResult(expectedResult, result);
        }
        
        [Test]
        public void TestHeaderTag_3()
        {
            var body = "abc<br><crz7-h>123";
            var result = CustomHtmlManager.ConvertCustomHtmlString(body);
            var expectedResult = new CustomHtmlConvertResult
            {
                objectParamList = new List<HtmlObjectParam>
                {
                    new(new TextHtmlObjectParam(text:"abc<br>", isTitle: false)),
                    new(new TextHtmlObjectParam(text:"123", isTitle: true)),
                }
            };
            AssertResult(expectedResult, result);
        }
        
        [Test]
        public void TestImageTag_1()
        {
            var body = "<crz7-img=banner>";
            var result = CustomHtmlManager.ConvertCustomHtmlString(body);
            var expectedResult = new CustomHtmlConvertResult
            {
                objectParamList = new List<HtmlObjectParam>
                {
                    new(new ImageHtmlObjectParam(imagePath: "banner", onClickString: string.Empty)),
                }
            };
            AssertResult(expectedResult, result);
        }
        
        
        [Test]
        public void TestImageTag_2()
        {
            var body = "<crz7-img=banner onClick=shop:123>";
            var result = CustomHtmlManager.ConvertCustomHtmlString(body);
            var expectedResult = new CustomHtmlConvertResult
            {
                objectParamList = new List<HtmlObjectParam>
                {
                    new(new ImageHtmlObjectParam(imagePath: "banner", onClickString: "shop:123")),
                }
            };
            AssertResult(expectedResult, result);
        }
        
        [Test]
        public void TestImageTag_3()
        {
            var body = "<crz7-img=banner onClick=shop:123><crz7-img=banner onClick=shop:123>";
            var result = CustomHtmlManager.ConvertCustomHtmlString(body);
            var expectedResult = new CustomHtmlConvertResult
            {
                objectParamList = new List<HtmlObjectParam>
                {
                    new(new ImageHtmlObjectParam(imagePath: "banner", onClickString: "shop:123")),
                    new(new ImageHtmlObjectParam(imagePath: "banner", onClickString: "shop:123")),
                }
            };
            AssertResult(expectedResult, result);
        }
        
        [Test]
        public void TestImageTag_4()
        {
            var body = "<crz7-img=banner onClick=shop:123><br><crz7-img=banner onClick=shop:123>";
            var result = CustomHtmlManager.ConvertCustomHtmlString(body);
            var expectedResult = new CustomHtmlConvertResult
            {
                objectParamList = new List<HtmlObjectParam>
                {
                    new(new ImageHtmlObjectParam(imagePath: "banner", onClickString: "shop:123")),
                    new(new TextHtmlObjectParam(isTitle: false, text: "<br>")),
                    new(new ImageHtmlObjectParam(imagePath: "banner", onClickString: "shop:123")),
                }
            };
            AssertResult(expectedResult, result);
        }
        
        [Test]
        public void TestCombinationTag_1()
        {
            var body = "<crz7-h>title</crz7-h><crz7-img=banner onClick=shop:123><br><crz7-img=banner onClick=shop:123><crz7-h>title</crz7-h>";
            var result = CustomHtmlManager.ConvertCustomHtmlString(body);
            var expectedResult = new CustomHtmlConvertResult
            {
                objectParamList = new List<HtmlObjectParam>
                {
                    new(new TextHtmlObjectParam(isTitle: true, text:"title")),
                    new(new ImageHtmlObjectParam(imagePath: "banner", onClickString: "shop:123")),
                    new(new TextHtmlObjectParam(isTitle: false, text: "<br>")),
                    new(new ImageHtmlObjectParam(imagePath: "banner", onClickString: "shop:123")),
                    new(new TextHtmlObjectParam(isTitle: true, text:"title")),
                }
            };
            AssertResult(expectedResult, result);
        }
        
        [Test]
        public void TestCombinationTag_2()
        {
            var body = "<crz7-h>title</crz7-h>description<crz7-img=banner><br><crz7-img=banner onClick=shop:123><crz7-h>title</crz7-h>";
            var result = CustomHtmlManager.ConvertCustomHtmlString(body);
            var expectedResult = new CustomHtmlConvertResult
            {
                objectParamList = new List<HtmlObjectParam>
                {
                    new(new TextHtmlObjectParam(isTitle: true, text:"title")),
                    new(new TextHtmlObjectParam(isTitle: false, text:"description")),
                    new(new ImageHtmlObjectParam(imagePath: "banner", onClickString: string.Empty)),
                    new(new TextHtmlObjectParam(isTitle: false, text: "<br>")),
                    new(new ImageHtmlObjectParam(imagePath: "banner", onClickString: "shop:123")),
                    new(new TextHtmlObjectParam(isTitle: true, text:"title")),
                }
            };
            AssertResult(expectedResult, result);
        }
        
        [Test]
        public void TestCombinationTag_3()
        {
            var body = "<crz7-h>title</crz7-h>description<crz7-img=banner><br><crz7-img=banner onClick=shop:123><crz7-h>title</crz7-h><crz7-button onClick=gacha:123>";
            var result = CustomHtmlManager.ConvertCustomHtmlString(body);
            var expectedResult = new CustomHtmlConvertResult
            {
                objectParamList = new List<HtmlObjectParam>
                {
                    new(new TextHtmlObjectParam(isTitle: true, text:"title")),
                    new(new TextHtmlObjectParam(isTitle: false, text:"description")),
                    new(new ImageHtmlObjectParam(imagePath: "banner", onClickString: string.Empty)),
                    new(new TextHtmlObjectParam(isTitle: false, text: "<br>")),
                    new(new ImageHtmlObjectParam(imagePath: "banner", onClickString: "shop:123")),
                    new(new TextHtmlObjectParam(isTitle: true, text:"title")),
                    new(new ButtonHtmlObjectParam(buttonText: "詳細を見る", onClickString: "gacha:123")),
                }
            };
            AssertResult(expectedResult, result);
        }
        
        [Test]
        public void TestButtonTag_1()
        {
            var body = "<crz7-button>";
            var result = CustomHtmlManager.ConvertCustomHtmlString(body);
            var expectedResult = new CustomHtmlConvertResult
            {
                objectParamList = new List<HtmlObjectParam>
                {
                    new(new ButtonHtmlObjectParam(buttonText: "詳細を見る", onClickString: string.Empty)),
                }
            };
            AssertResult(expectedResult, result);
        }
        
        [Test]
        public void TestButtonTag_2()
        {
            var body = "<crz7-button onClick=shop:321>";
            var result = CustomHtmlManager.ConvertCustomHtmlString(body);
            var expectedResult = new CustomHtmlConvertResult
            {
                objectParamList = new List<HtmlObjectParam>
                {
                    new(new ButtonHtmlObjectParam(buttonText: "詳細を見る",  onClickString: "shop:321")),
                }
            };
            AssertResult(expectedResult, result);
        }
        
        [Test]
        public void TestButtonTag_3()
        {
            var body = "<crz7-button=ショップへ遷移 onClick=shop:321>";
            var result = CustomHtmlManager.ConvertCustomHtmlString(body);
            var expectedResult = new CustomHtmlConvertResult
            {
                objectParamList = new List<HtmlObjectParam>
                {
                    new(new ButtonHtmlObjectParam(buttonText: "ショップへ遷移",  onClickString: "shop:321")),
                }
            };
            AssertResult(expectedResult, result);
        }
        
        [Test]
        public void TestButtonTag_4()
        {
            // メモ：記入ミスで表記が設定されなかったら、デフォルトが設定される
            var body = "<crz7-button= onClick=shop:321>";
            var result = CustomHtmlManager.ConvertCustomHtmlString(body);
            var expectedResult = new CustomHtmlConvertResult
            {
                objectParamList = new List<HtmlObjectParam>
                {
                    new(new ButtonHtmlObjectParam(buttonText: "詳細を見る",  onClickString: "shop:321")),
                }
            };
            AssertResult(expectedResult, result);
        }

        [Test]
        public void TestArticleSample()
        {
            var body =
@"<crz7-img=news_banner_99999>

<color=#0000FF>◆Vol.1販売期間</color><br>
8/23 0:00 ～ 8/26 14:59

<crz7-h>100万DL記念パックVol.2紹介！</crz7-h>

<color=#FF00FF>祝</color>100万DLを記念した超豪華パック登場！<br>
<br>

最初のパックを購入することで、<br>
次のステップのパックが購入できるパック！<br>
<br>

<size=20>
「<color=#FFAA00>モンスターガチャチケット</color>」<br>
「<color=#FFAA00>装備ガチャチケット</color>」が手に入り、<br>
ステップが上がるほど<color=#FF0000>豪華に！</color><br>
</size>
<br>

<crz7-button onClick=openPage:shop>";
            var result = CustomHtmlManager.ConvertCustomHtmlString(body);
            var expectedResult = new CustomHtmlConvertResult
            {
                objectParamList = new List<HtmlObjectParam>
                {
                    new(new ImageHtmlObjectParam(imagePath: "news_banner_99999", onClickString: string.Empty)),
                    new(new TextHtmlObjectParam(isTitle: false, text: @"

<color=#0000FF>◆Vol.1販売期間</color><br>
8/23 0:00 ～ 8/26 14:59

")),
                    new(new TextHtmlObjectParam(isTitle: true, text: @"100万DL記念パックVol.2紹介！")),
                    new(new TextHtmlObjectParam(isTitle: false, text: @"

<color=#FF00FF>祝</color>100万DLを記念した超豪華パック登場！<br>
<br>

最初のパックを購入することで、<br>
次のステップのパックが購入できるパック！<br>
<br>

<size=20>
「<color=#FFAA00>モンスターガチャチケット</color>」<br>
「<color=#FFAA00>装備ガチャチケット</color>」が手に入り、<br>
ステップが上がるほど<color=#FF0000>豪華に！</color><br>
</size>
<br>

")),
                    new(new ButtonHtmlObjectParam(buttonText: "詳細を見る", onClickString: "openPage:shop")),
                }
            };
            AssertResult(expectedResult, result);
            
        }
        
        private void AssertResult(CustomHtmlConvertResult expected, CustomHtmlConvertResult result)
        {
            Assert.AreEqual(expected.ToString(), result.ToString(), message: $"==expected==\n{expected}\n==result==\n{result}\n=====");
        }
    }
}