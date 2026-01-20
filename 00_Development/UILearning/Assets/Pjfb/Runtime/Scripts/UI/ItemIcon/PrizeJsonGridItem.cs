using System.Collections;
using System.Collections.Generic;
using CruFramework.Page;
using UnityEngine;

using CruFramework.UI;
using UnityEngine.UI;
using TMPro;
using System;
using Pjfb.Master;

namespace Pjfb
{

    public class PrizeJsonGridItem : ScrollGridItem
    {
        #region Params
        public class Data
        {
            public Master.PrizeJsonWrap masterPrizeJsonWrap;
            public Networking.App.Request.PrizeJsonWrap requestPrizeJsonWrap;
            public bool isEffectActivated;

            public bool isSelected;

            public Data(Master.PrizeJsonWrap _wrap, bool _isEffectActivated = false)
            {
                masterPrizeJsonWrap = _wrap;
                requestPrizeJsonWrap = null;
                isSelected = false;
                isEffectActivated = _isEffectActivated;
            }

            public Data(Networking.App.Request.PrizeJsonWrap _wrap, bool _isEffectActivated = false)
            {
                requestPrizeJsonWrap = _wrap;
                masterPrizeJsonWrap = null;
                isSelected = false;
                isEffectActivated = _isEffectActivated;
            }
        }
        #endregion

        #region SerializeFields
        [SerializeField] private PrizeJsonView prizeJsonView;
        [SerializeField] private Image selectedFrame;
        #endregion

        public Data data;

        protected override void OnSetView(object value)
        {
            data = (Data) value;
            selectedFrame?.gameObject.SetActive(data.isSelected);
            if (data.masterPrizeJsonWrap != null)
            {
                prizeJsonView.SetView(data.masterPrizeJsonWrap);
                // パス効果がある場合の設定
                var args = data.masterPrizeJsonWrap.args;
                // 加算補正倍率と補正が掛かる前の値は補正がかかった場合のみ存在するため２つの値が0より大きい場合はパスの効果が発動しているとする
                var isActivePassEffectReward = args.correctRate > 0 && args.valueOriginal > 0;
                prizeJsonView.SetCountTextColor(isActivePassEffectReward || data.isEffectActivated ? ColorValueAssetLoader.Instance["highlight.orange"] : ColorValueAssetLoader.Instance["white"]);
            }
            else
            {
                prizeJsonView.SetView(data.requestPrizeJsonWrap);
                // パス効果がある場合の設定
                var args = data.requestPrizeJsonWrap.args;
                // 加算補正倍率と補正が掛かる前の値は補正がかかった場合のみ存在するため２つの値が0より大きい場合はパスの効果が発動しているとする
                var isActivePassEffectReward = args.correctRate > 0 && args.valueOriginal > 0;
                prizeJsonView.SetCountTextColor(isActivePassEffectReward || data.isEffectActivated ? ColorValueAssetLoader.Instance["highlight.orange"] : ColorValueAssetLoader.Instance["white"]);
            }
        }
    }
}