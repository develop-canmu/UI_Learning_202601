using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

using Pjfb.Master;
using Pjfb.UserData;

namespace Pjfb
{
	public class CharacterEventNameLabel : MonoBehaviour
	{
		[SerializeField]
		private TMPro.TMP_Text nameText = null;
		
		/// <summary>名前</summary>
		public void SetName(string name)
		{
			nameText.text = name;
		}

	}
}