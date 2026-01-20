using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CruFramework.H2MD
{
	public abstract class H2MDPlayer : MonoBehaviour
	{
		[SerializeField]
		private H2MDAsset h2mdAsset = null;
		/// <summary>H2MDアセット</summary>
		public H2MDAsset H2MDAsset
		{
			get
			{
				return h2mdAsset;
			}
		}

		[SerializeField]
		private bool isLoop = false;
		public bool IsLoop{get{return isLoop;}set{isLoop = value;}}
		
		[SerializeField]
		private bool isFlip = false;
		public bool IsFlip{get{return isFlip;}set{isFlip = value;}}
		
		[SerializeField]
		private  float speed = 1.0f;
		public float Speed{get{return speed;}set{speed = value;}}
		
		[SerializeField]
		private bool playOnLoad = false;
		public bool PlayOnLoad{get{return playOnLoad;}set{playOnLoad = value;}}
		
		[SerializeField]
		private bool playOnEnable = false;
		public bool PlayOnEnable{get{return playOnEnable;}set{playOnEnable = value;}}
		
		[SerializeField]
		private bool disableOnStop = false;
		public bool DisableOnStop{get{return disableOnStop;}set{disableOnStop = value;}}

		private bool isLoaded = false;
		public bool IsLoaded{get{return isLoaded;}}
		
		private bool isPlaying = false;
		public bool IsPlaying{get{return isPlaying;}}
		
		private H2MDMovie h2mdMovie = null;
		
		private Texture2D texture = null;
		public Texture2D MovieTexture{get{return texture;}}
		
		public float Duration{get{return h2mdMovie == null ? 0 : (float)h2mdMovie.GetTotalFrames() / (float)h2mdMovie.GetFrameRate();}}
		
		public float Current{get{return h2mdMovie == null ? 0 : movieTime;}} 
		
		/// <summary>再生終了時</summary>
		public event Action OnMovieFinished = null;
		
		private float movieTime = 0;
		private int movieFrame = -1;

		private H2MDAsset loadedAsset = null;

		protected abstract void OnLoadAsset();
		
		/// <summary>アセットの読み込み</summary>
		public void Load(H2MDAsset asset)
		{
			if(loadedAsset == asset)return;
			// 読み込み済み
			loadedAsset = asset;
			// 停止
			Stop();
			// 読み込みフラグ
			isLoaded = false;

			h2mdAsset = asset;
			// Movieの破棄
			ReleaseMovie();
			
			h2mdMovie = new H2MDMovie();
			// ファイルを開く
			texture = h2mdMovie.OpenMem(h2mdAsset.Bytes);
			// 0フレーム目を取得しておく
			Decode(0);
			
			// アセットの読み込み通知
			OnLoadAsset();
			
			// 読み込みフラグ
			isLoaded = true;
			
			if(playOnLoad || playOnEnable)
			{
				Play();
			}
		}

		/// <summary>再生</summary>
		public void Play()
		{
			if(isLoaded && isPlaying == false)
			{
				isPlaying = true;
			}
		}

		public void Stop()
		{
			if(isPlaying)
			{
				movieFrame = -1;
				movieTime = 0;
				isPlaying = false;
			}
		}

		public void SetMovieTime(float time)
		{
			movieTime = Mathf.Clamp(time, 0, Duration);
		}
		
		private void MovieEnd()
		{
			Stop();
			// 終了通知
			if(OnMovieFinished != null)
			{
				OnMovieFinished();
			}
			
			if(disableOnStop)
			{
				gameObject.SetActive(false);
			}
		}
		
		private void ReleaseMovie()
		{
			if(h2mdMovie != null)
			{ 
				h2mdMovie.Dispose();
				h2mdMovie = null;
			}
			
			if(texture != null)
			{
				Destroy(texture);
				texture = null;
			}

			// if (h2mdAsset != null)
			// {
			// 	Destroy(h2mdAsset);
			// }
		}

		private void Awake()
		{
			if(h2mdAsset != null)
			{
				Load(h2mdAsset);
			}
		}

		private void OnEnable()
		{
			if(playOnEnable)
			{
				Play();
			}
		}

		private void OnDisable()
		{
			Stop();
		}

		private void OnDestroy()
		{
			ReleaseMovie();
		}

		private void Decode(int frame)
		{
			// 前回と同じフレームは処理しない
			if(movieFrame == frame)
			{
				return;
			}
			// フレーム位置更新
			movieFrame = frame;
			
			// デコード
			h2mdMovie.Decode(movieFrame);
			
			if(isFlip)
			{
				h2mdMovie.GetImageFlip();
			}
			else
			{
				h2mdMovie.GetImage();
			}
		}
		

		
		private void LateUpdate()
		{
			if(isPlaying == false)return;
			
			// フレーム
			int currentFrame = (int)( movieTime * h2mdMovie.GetFrameRate() );
			// 時間経過
			movieTime += Time.deltaTime * speed;
			
			// 合計フレーム
			int totalFrame = h2mdMovie.GetTotalFrames();
			// 終了チェック
			if(currentFrame >= totalFrame)
			{
				// ループ
				if(isLoop)
				{
					currentFrame -= totalFrame;
					movieTime -= (float)totalFrame / (float)h2mdMovie.GetFrameRate();
				}
				else
				{
					currentFrame = totalFrame - 1;
					MovieEnd();
				}
			}
			
			Decode(currentFrame);
		}
	}
}