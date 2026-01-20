using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CruFramework.Editor.Addressable
{
	[Serializable]
	public class AddressableGroupRuleByDirectory : AddressableGroupRule
	{
		[SerializeField]
		private DefaultAsset directory = null;
		/// <summary>対象ディレクトリ</summary>
		public DefaultAsset Directory
		{
			get { return directory; }
			set { directory = value; }
		}

		public override bool IsMatch(string path)
		{
			if(directory == null) return false;
			return path.StartsWith(AssetDatabase.GetAssetPath(directory));
		}

		public override string GetGroupName(string path)
		{
			switch (PackingMode)
			{
				case AddressableDirectoryBundlePackingMode.PackTogether:
				case AddressableDirectoryBundlePackingMode.PackSeparately:
				{
					// 設定されてるディレクトリでグループ名を構築
					return ReplacePathToGroupName(AssetDatabase.GetAssetPath(directory));
				}
				case AddressableDirectoryBundlePackingMode.PackTogetherByTopDirectory:
				{
					// 直下のディレクトリでグループ名を構築
					string directoryPath = AssetDatabase.GetAssetPath(directory);
					string topDirectory = path.Replace(directoryPath, string.Empty).Trim('/').Split('/')[0];
					return ReplacePathToGroupName($"{directoryPath}/{topDirectory}");
				}
				case AddressableDirectoryBundlePackingMode.PackTogetherByLastDirectory:
				{
					// 末尾のディレクトリでグループ名を構築
					return ReplacePathToGroupName(Path.GetDirectoryName(path));
				}
				default:
				{
					throw new NotImplementedException($"{PackingMode} is not implemented.");
				}
			}
		}
	}
}