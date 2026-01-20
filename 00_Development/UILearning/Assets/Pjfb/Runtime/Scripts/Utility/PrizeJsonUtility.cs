using System;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using PrizeJsonWrap = Pjfb.Networking.App.Request.PrizeJsonWrap;
using PrizeJsonArgs = Pjfb.Networking.App.Request.PrizeJsonArgs;

namespace Pjfb
{
    public static class PrizeJsonUtility
    {
        private static string DefaultUnitName = "個";
        public static string LockedText => StringValueAssetLoader.Instance["locked_item.item.locked"];

        public static PrizeContainerData GetPrizeContainerData(Pjfb.Master.PrizeJsonWrap prizeJsonWrap)
        {
            return GetPrizeContainerData(new PrizeJsonWrap()
            {
                type = prizeJsonWrap.type,
                description = prizeJsonWrap.description,
                args = new PrizeJsonArgs()
                {
                    mPointId = prizeJsonWrap.args.mPointId,
                    mAvatarId = prizeJsonWrap.args.mAvatarId,
                    mCharaId = prizeJsonWrap.args.mCharaId,
                    pieceMCharaId = prizeJsonWrap.args.pieceMCharaId,
                    mSkillPartId = prizeJsonWrap.args.mSkillPartId,
                    variableMCharaId = prizeJsonWrap.args.variableMCharaId,
                    variableTrainerMCharaId = prizeJsonWrap.args.variableTrainerMCharaId,
                    mIconId = prizeJsonWrap.args.mIconId,
                    mTitleId = prizeJsonWrap.args.mTitleId,
                    mChatStampId = prizeJsonWrap.args.mChatStampId,
                    lockId = prizeJsonWrap.args.lockId,
                    value = prizeJsonWrap.args.value,
                    get = prizeJsonWrap.args.get,
                    mProfilePartId = prizeJsonWrap.args.mProfilePartId,
                },
            });
        }
        
        public static PrizeContainerData GetPrizeContainerData(PrizeJsonWrap prizeJsonWrap)
        {
            var retVal = new PrizeContainerData();
            if (prizeJsonWrap == null) return retVal;
            
            switch (prizeJsonWrap.type) {
                case "point": {
                    var pointData = MasterManager.Instance.pointMaster.FindData(prizeJsonWrap.args.mPointId);
                    if (pointData != null) retVal.Set(pointData.name, pointData.unitName, prizeJsonWrap.args.value, ItemIconType.Item, prizeJsonWrap);
                    break;
                } case "chara": {
                    var charaData = MasterManager.Instance.charaMaster.FindData(prizeJsonWrap.args.mCharaId);
                    if (charaData != null) retVal.Set(charaData.name, "体", prizeJsonWrap.args.value, ItemIconType.Character, prizeJsonWrap);
                    break;
                } case "charaPiece": {
                    var charaData = MasterManager.Instance.charaMaster.FindData(prizeJsonWrap.args.pieceMCharaId);
                    if (charaData != null) retVal.Set(charaData.name, "体", prizeJsonWrap.args.value, ItemIconType.CharacterPiece, prizeJsonWrap);
                    break;
                } case "charaVariable": {
                    var charaData = MasterManager.Instance.charaMaster.FindData(prizeJsonWrap.args.variableMCharaId);
                    if (charaData != null) retVal.Set(charaData.name, "体", prizeJsonWrap.args.value, ItemIconType.VariableCharacter, prizeJsonWrap);
                    break;
                } case "icon": {
                    var iconData = MasterManager.Instance.iconMaster.FindData(prizeJsonWrap.args.mIconId);
                    if (iconData != null) retVal.Set(iconData.name, DefaultUnitName, prizeJsonWrap.args.value, ItemIconType.UserIcon, prizeJsonWrap);
                    break;
                } case "title": {
                    var titleData = MasterManager.Instance.titleMaster.FindData(prizeJsonWrap.args.mTitleId);
                    if (titleData != null) retVal.Set(titleData.name.ToString(), DefaultUnitName, prizeJsonWrap.args.value, ItemIconType.UserTitle, prizeJsonWrap);
                    break;
                } case "chatStamp": {
                    var chatStampData = MasterManager.Instance.chatStampMaster.FindData(prizeJsonWrap.args.mChatStampId);
                    if (chatStampData != null) retVal.Set(chatStampData.name.ToString(), DefaultUnitName, prizeJsonWrap.args.value, ItemIconType.ChatStamp, prizeJsonWrap);
                    break;
                } case "charaVariableTrainer": {
                    var charaData = MasterManager.Instance.charaMaster.FindData(prizeJsonWrap.args.variableTrainerMCharaId);
                    if (charaData != null) retVal.Set(charaData.name, "体", prizeJsonWrap.args.value, ItemIconType.SupportEquipment, prizeJsonWrap);
                    break;
                }
                case "profilePart":
                {
                    var profilePartMaster = MasterManager.Instance.profilePartMaster.FindData(prizeJsonWrap.args.mProfilePartId);
                    string name = string.Empty;
                    if (profilePartMaster != null)
                    {
                        switch (profilePartMaster.partType)
                        {
                            case ProfilePartMasterObject.ProfilePartType.ProfileFrame:
                                name = MasterManager.Instance.profileFrameMaster.FindData(profilePartMaster.imageId).name;
                                break;
                            case ProfilePartMasterObject.ProfilePartType.DisplayCharacter:
                                name = MasterManager.Instance.profileCharaMaster.FindData(profilePartMaster.imageId).name;
                                break;
                            case ProfilePartMasterObject.ProfilePartType.Emblem:
                                name = MasterManager.Instance.emblemMaster.FindData(profilePartMaster.imageId).name;
                                break;
                            // 未実装なのでエラーを投げる
                            case ProfilePartMasterObject.ProfilePartType.ProfileVoice:
                            case ProfilePartMasterObject.ProfilePartType.DisplayCharacterBackground:
                                throw new NotImplementedException();
                        }

                        retVal.Set(name, DefaultUnitName, prizeJsonWrap.args.value, ItemIconType.ProfilePart, prizeJsonWrap);
                    }
                    break;
                }
                default:
                    retVal.Set(string.Empty, DefaultUnitName, prizeJsonWrap.args.value, ItemIconType.Item, prizeJsonWrap);
                    break;
            }

            return retVal;
        }
        
        public class PrizeContainerData
        {
            public string name;
            public string unitName;
            public long count;
            public ItemIconType itemIconType;
            public PrizeJsonWrap prizeJsonWrap;
            
            
            public void Set(string name, string unitName, long count, ItemIconType itemIconType, PrizeJsonWrap prizeJsonWrap)
            {
                this.name = $"{(prizeJsonWrap.args.lockId > 0 ? LockedText : string.Empty)}{name}";
                this.unitName = unitName;
                this.count = count;
                this.itemIconType = itemIconType;
                this.prizeJsonWrap = prizeJsonWrap;
            }
        }
    }
}