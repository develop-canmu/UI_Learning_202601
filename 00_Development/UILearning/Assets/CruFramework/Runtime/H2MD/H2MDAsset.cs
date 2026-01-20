using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CruFramework.H2MD
{
	public class H2MDAsset : ScriptableObject
	{
		public static H2MDAsset Create(byte[] bytes)
		{
			H2MDAsset asset = ScriptableObject.CreateInstance<H2MDAsset>();
			asset.bytes = bytes;
			return asset;
		}
		
		[SerializeField][HideInInspector]
		private byte[] bytes = null;
		public byte[] Bytes{get{return bytes;}}
	}
}