using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIEffects;
using CruFramework.H2MD;
using UnityEngine;
using CruFramework.UI;
using UnityEngine.UI;

namespace Pjfb.Menu
{
    [CreateAssetMenu(fileName = "TrainerCard", menuName = "Pjfb/TrainerCard/TrainerCardCustomizeConfig")]
    public class TrainerCardCustomizeConfig : ScriptableObject
    {
        [System.Serializable]
        public class GradientConfig
        {
            [SerializeField]
            private Color gradientColor1 = Color.white;
            public Color GradientColor1{get{return gradientColor1;}}
            
            [SerializeField]
            private Color gradientColor2 = Color.white;
            public Color GradientColor2{get{return gradientColor2;}}
        }
        
        [System.Serializable]
        public class GradientDiagonalConfig
        {
            [SerializeField]
            private Color diagonalColorUpLeft = Color.white;
            public Color DiagonalColorUpLeft { get { return diagonalColorUpLeft;}}
        
            [SerializeField]
            private Color diagonalColorUpRight = Color.white;
            public Color DiagonalColorUpRight { get { return diagonalColorUpRight;}}
        
            [SerializeField]
            private Color diagonalColorDownLeft = Color.white;
            public Color DiagonalColorDownLeft { get { return diagonalColorDownLeft;}}
        
            [SerializeField]
            private Color diagonalColorDownRight = Color.white;
            public Color DiagonalColorDownRight { get { return diagonalColorDownRight;}}
        }
        
        [Header("モーダル名部分")]
        // ヘッダー画像
        [SerializeField] private Sprite headerImage = null;
        public Sprite HeaderImage { get { return headerImage; } }
        // ヘッダーカラー
        [SerializeField] private GradientConfig headerColorGradient;
        public GradientConfig HeaderColorGradient { get { return headerColorGradient; } }
        // モーダルタイトル文字カラー
        [SerializeField] private Color titleTextColor = Color.white;
        public Color TitleTextColor { get { return titleTextColor; } }
        
        [Header("カード背景")]
        // トレーナーカード背景画像
        [SerializeField] private Sprite bodyImage = null;
        public Sprite BodyImage { get { return bodyImage; } }
        // トレーナーカード背景H2MD
        [SerializeField] private H2MDAsset bodyH2MD = null;
        public H2MDAsset BodyH2MD { get { return bodyH2MD; } }
        // トレーナーカード背景カラー
        [SerializeField] private GradientDiagonalConfig bodyColorGradient;
        public GradientDiagonalConfig BodyColorGradient { get { return bodyColorGradient; } }
        
        [Header("ユーザーネーム")]
        //ユーザーネームヘッダー画像
        [SerializeField] private Sprite userNameRootImage = null;
        public Sprite UserNameRootImage { get { return userNameRootImage; } }
        // ユーザーネームヘッダーカラー
        [SerializeField] private GradientConfig userNameRootColorGradient;
        public GradientConfig UserNameRootColorGradient { get { return userNameRootColorGradient; } }
        // ユーザーネームテキストカラー
        [SerializeField] private Color userNameTextColor = Color.white;
        public Color UserNameTextColor { get { return userNameTextColor; } }
        
        [Header("自己紹介")]
        [SerializeField] private Sprite introductionHeaderImage = null;
        public Sprite IntroductionHeaderImage { get { return introductionHeaderImage; } }
        [SerializeField] private GradientConfig introductionHeaderColorGradient;
        public GradientConfig IntroductionHeaderColorGradient { get { return introductionHeaderColorGradient; } }
        [SerializeField] private Color introductionHeaderTextColor = Color.white;
        public Color IntroductionHeaderTextColor { get { return introductionHeaderTextColor; } }
        
        [Header("バッジ")]
        // バッジヘッダー画像
        [SerializeField] private Sprite badgeHeaderImage = null;
        public Sprite BadgeHeaderImage { get { return badgeHeaderImage; } }
        // バッジヘッダーカラー
        [SerializeField] private GradientConfig badgeHeaderColorGradient;
        public GradientConfig BadgeHeaderColorGradient { get { return badgeHeaderColorGradient; } }
        // バッジテキストカラー
        [SerializeField] private Color badgeTextColor = Color.white;
        public Color BadgeTextColor { get { return badgeTextColor; } }
        
        [Header("サポート選手")]
        [SerializeField] private Sprite supportCharacterHeaderImage = null;
        public Sprite SupportCharacterHeaderImage { get { return supportCharacterHeaderImage; } }
        [SerializeField] private Color supportCharacterHeaderColor = Color.white;
        public Color SupportCharacterHeaderColor { get { return supportCharacterHeaderColor; } }
        [SerializeField] private Color supportCharacterHeaderTextColor = Color.white;
        public Color SupportCharacterHeaderTextColor { get { return supportCharacterHeaderTextColor; } }
        
        [Header("フレンドコード")]
        // フレンドコード背景画像
        [SerializeField] private Sprite friendCodeBaseImage = null;
        public Sprite FriendCodeBaseImage { get { return friendCodeBaseImage; } }
        // フレンドコード背景カラー
        [SerializeField] private Color friendCodeBaseColor = Color.white;
        public Color FriendCodeBaseColor { get { return friendCodeBaseColor; } }
        // フレンドコードテキストカラー
        [SerializeField] private Color friendCodeTextColor = Color.white;
        public Color FriendCodeTextColor { get { return friendCodeTextColor; } }
        
    }
}