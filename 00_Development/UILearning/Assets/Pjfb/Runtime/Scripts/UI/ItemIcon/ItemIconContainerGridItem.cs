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

    public class ItemIconContainerGridItem : ScrollGridItem
    {
        #region Params
        public class Data
        {
            public ItemIconType iconType;
            public long id;
            public long possessionValue;
            public long necessaryValue;
            public bool isSelected;
            public bool isCoverActive;

            public Data(ItemIconType _iconType, long _id, long _possessionValue, long _necessaryValue, bool _isCoverActive = false)
            {
                iconType = _iconType;
                id = _id;
                possessionValue = _possessionValue;
                necessaryValue = _necessaryValue;
                isSelected = false;
                isCoverActive = _isCoverActive;
            }
        }
        #endregion

        #region SerializeFields
        [SerializeField] private ItemIconContainer itemIconContainer;
        [SerializeField] private GameObject selectedFrame;
        [SerializeField] private GameObject cover;
        #endregion

        public Data data;

        protected override void OnSetView(object value)
        {
            data = (Data) value;
            if (selectedFrame != null) selectedFrame.SetActive(data.isSelected);
            if (cover != null) cover.SetActive(data.isCoverActive);
            itemIconContainer.SetIcon(data.iconType, data.id);
            itemIconContainer.SetCountDigitString(data.possessionValue, data.necessaryValue);
        }
    }
}