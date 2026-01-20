using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pjfb.Networking.App.Request;
using UnityEngine;

namespace Pjfb.Gacha
{
    public class GachaTopPageData
    {
        private Dictionary<GachaType, List<GachaSettingData>> gachaDatas = new Dictionary<GachaType, List<GachaSettingData>>();
        
        private GachaSettingData currentGachaData = null;
        /// <summary>現在ガチャデータ</summary>
        public GachaSettingData CurrentGachaData
        {
            get { return currentGachaData; }
            set { currentGachaData = value; }
        }

        private GachaSettingData currentTicketGachaData = null;
        /// <summary>現在チケットガチャデータ</summary>
        public GachaSettingData CurrentTicketGachaData
        {
            get { return currentTicketGachaData; }
            set { currentTicketGachaData = value; }
        }

        private bool isFocusTicketGacha = false;
        /// <summary>現在チケットガチャデータ</summary>
        public bool IsFocusTicketGacha
        {
            get { return isFocusTicketGacha; }
        }

        

        public GachaTopPageData(long focusGachaSettingId, bool focusTicketGacha, TopSetting[] topSettings)
        {
            isFocusTicketGacha = false;
            // 優先度でソート
            topSettings = topSettings.OrderByDescending(data => data.priority).ToArray();
            // データに変換
            foreach (TopSetting topSetting in topSettings)
            {
                // ガチャ情報
                GachaSettingData settingData = new GachaSettingData(topSetting);
                //チケットガチャの場合引けない場合はデータに入れない
                if( settingData.GachaType == GachaType.Ticket ) {
                    if( settingData.SingleGachaData.GachaCategoryId == 0  ) {
                        continue;
                    }
                    var drawCount = GachaUtility.CalcMultiDrawCount(settingData.SingleGachaData);
                    if( drawCount <= 0  ) {
                        continue;
                    }
                }
                // キーが存在しない
                if(!gachaDatas.ContainsKey(settingData.GachaType))
                {
                    // キー追加
                    gachaDatas.Add(settingData.GachaType, new List<GachaSettingData>());
                }
                // データを追加
                gachaDatas[settingData.GachaType].Add(settingData);
                
                // 初期表示するガチャかどうか
                if(settingData.GachaSettingId == focusGachaSettingId)
                {
                    currentGachaData = settingData;
                    if( currentGachaData.GachaType == GachaType.Ticket ) {
                        currentTicketGachaData = currentGachaData;
                    }
                }
            }

            // ガチャ種別ごとのindexを割り振る
            foreach (List<GachaSettingData> gachaSettingDataList in gachaDatas.Values)
            {
                for(int i = 0; i < gachaSettingDataList.Count; i++)
                {
                    if( gachaSettingDataList[i].GachaType == GachaType.Ticket ) {
                        gachaSettingDataList[i].Index = gachaDatas[GachaType.Normal].Count();
                    } else {
                        gachaSettingDataList[i].Index = i;
                    }
                    
                }
            }

            // 初期表示の指定がないので通常ガチャの最初のガチャを表示
            if(currentGachaData == null)
            {
                currentGachaData = GetGachaDatas(GachaType.Normal)[0];
                //チケットガチャのフォーカス設定がされていたらフラグを立てる
                if( focusTicketGacha ) {
                    isFocusTicketGacha = true;
                }
            }
            
            if(ExistsGachaDatas(GachaType.Ticket) && currentTicketGachaData == null )
            {
            
                currentTicketGachaData = GetGachaDatas(GachaType.Ticket)[0];
            }
        }
        
        /// <summary>ガチャ情報が存在するか</summary>
        public bool ExistsGachaDatas(GachaType gachaType)
        {
            if(!gachaDatas.ContainsKey(gachaType)) return false;
            if(gachaDatas[gachaType].Count <= 0) return false;
            return true;
        }
        
        /// <summary>ガチャ情報を取得</summary>
        public List<GachaSettingData> GetGachaDatas(GachaType gachaType)
        {
            if(gachaDatas.ContainsKey(gachaType))
            {
                return gachaDatas[gachaType];
            }
            return null;
        }

        public List<GachaSettingData> GetAllGachaDatas()
        {
            var data = new List<GachaSettingData>();
            foreach( var key in gachaDatas.Keys ){
                if( gachaDatas[key] != null ) {
                    data.AddRange(gachaDatas[key]);
                }
            }
            return data;
        }

       
    }
}
