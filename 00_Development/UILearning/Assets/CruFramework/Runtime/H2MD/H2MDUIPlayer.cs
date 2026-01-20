using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CruFramework.H2MD
{
	[RequireComponent(typeof(RawImage))]
	public class H2MDUIPlayer : H2MDPlayer
	{
		[SerializeField]
		private bool useMovieSize = true;
		
		protected override void OnLoadAsset()
		{
			GetComponent<RawImage>().texture = MovieTexture;
			if(useMovieSize)
			{
				RectTransform t = (RectTransform)transform;
				t.sizeDelta = new Vector2(MovieTexture.width, MovieTexture.height);
			}
		}
	}
}
