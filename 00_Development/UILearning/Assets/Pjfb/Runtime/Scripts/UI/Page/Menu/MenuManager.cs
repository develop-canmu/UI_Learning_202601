using System;
using System.Globalization;
using System.Linq;
using Pjfb.Master;
using Pjfb.Storage;
using Pjfb.UserData;
using Logger = CruFramework.Logger;

namespace Pjfb.Menu
{
    public static class MenuManager
    {
        #region PublicProperties
        // TODO: バッジ実装
        public static string menuBadgeString => UnityEngine.Random.Range(0, 2) == 1 ? "!" : string.Empty;
        #endregion

        #region StaticMethods
        public static void OnClickMenuButton()
        {
            Logger.Log("MenuManager.OnClickMenuButton");
            MenuModalWindow.Open(new MenuModalWindow.WindowParams());
        }

        public static bool IsTrainerCardBadge()
        {
            return IsTitleBadge() || IsProfileFrameBadge() || IsEmblemBadge() || IsProfileCharaBadge();
        }

        // プロフィールキャラ所持バッジ
        public static bool IsProfileCharaBadge()
        {
            foreach (ProfileCharaMasterObject charaMaster in MasterManager.Instance.profileCharaMaster.values)
            {
                if(charaMaster.IsHave == false) continue;
                if (LocalSaveManager.saveData.viewedDisplayCharaIds.Contains(charaMaster.id) == false)
                {
                    return true;
                }
            }
            return false;
        }
        
        // Emblemの所持バッジ
        public static bool IsEmblemBadge()
        {
            foreach (EmblemMasterObject emblemMaster in MasterManager.Instance.emblemMaster.values)
            {
                if(emblemMaster.IsHave == false) continue;
                if (LocalSaveManager.saveData.viewedMyBadgeIds.Contains(emblemMaster.id) == false)
                {
                    return true;
                }
            }
            return false;
        }
        
        // 着せ替えの所持バッジ
        public static bool IsProfileFrameBadge()
        {
            foreach (ProfileFrameMasterObject frameMaster in MasterManager.Instance.profileFrameMaster.values)
            {
                if(frameMaster.IsHave == false) continue;
                if (LocalSaveManager.saveData.viewedCustomizeFrameIds.Contains(frameMaster.id) == false)
                {
                    return true;
                }
            }
            return false;
        }
        
        // 称号の所持バッジ
        public static bool IsTitleBadge()
        {
            foreach (long titleId in UserDataManager.Instance.title)
            {
                if(LocalSaveManager.saveData.viewedTitles.Contains(titleId) == false)
                {
                    return true;
                }
            }
            return false;
        }
        
        #endregion
    }
}