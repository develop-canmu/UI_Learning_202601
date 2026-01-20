// /* H2MD Unity Plugin Movie Texture Class */
// /* Copyright 2016-2017 AXELL CORPORATION */
//
// /* 任意のオブジェクトにH2MDTexture.csをアタッチした後、
//    pathにH2MDのファイル名を指定することで動画を再生することができます */
//
// using UnityEngine;
// using UnityEngine.UI;
// using System.Collections;
// using System.IO;
// using System;
//
// //ハンドル定義
// using H2MDDecoderHandle = System.IntPtr;
//
// public class H2MDTexture : MonoBehaviour {
//
// /****************************************************************
//  * 設定項目
//  **/
//
// 	[SerializeField, TooltipAttribute("StreamingAssetsに置いたH2MDファイルへのパス (sample.h2mdなど)")]
// 	public String path="";
// 	[SerializeField, TooltipAttribute("設定先のテクスチャID、設定していない場合はmainTexture")]
// 	public String texture_id="";
// 	[SerializeField, TooltipAttribute("自動再生")]
// 	public bool autoplay=true;
// 	[SerializeField, TooltipAttribute("ループ再生")]
// 	public bool loop=true;
// 	[SerializeField, TooltipAttribute("初期化時と再生終了時にテクスチャを透明でフィル")]
// 	public bool fill=true;
// 	[SerializeField, TooltipAttribute("再生終了時に最終フレームをホールド")]
// 	public bool hold=false;
// 	[SerializeField, TooltipAttribute("デコード結果を上下フリップ")]
// 	public bool flip=false;
// 	[SerializeField, TooltipAttribute("サブスレッドでデコード")]
// 	public bool async=false;
// 	[SerializeField, TooltipAttribute("再生終了時に自動的に開放")]
// 	public bool autorelease=false;
// 	[SerializeField, TooltipAttribute("エディタでプレビュー")]
// 	public bool preview=false;
// 	[SerializeField, TooltipAttribute("再生速度")]
// 	public float speed=1.0f;
// 	[SerializeField, TooltipAttribute("再生開始遅延秒数")]
// 	public float delay=0.0f;
// 	[SerializeField, TooltipAttribute("デコード開始フレームオフセット")]
// 	public int offset=0;
// 	[SerializeField, TooltipAttribute("pathにURIを与える（https://www.h2md.jp/sample.h2mdなどを指定可能にする）")]
// 	public bool uri=false;
// 	[SerializeField, TooltipAttribute("temporaryCachePathに格納するキャッシュのファイル名、設定していない場合はキャッシュ無効")]
// 	public string cache="";
//
// /****************************************************************
//  * 公開API
//  **/
//
// 	void Start () {
// 		//動画の読込を開始
// 		//autoplayが有効な場合、読込完了後に自動的に再生が開始
//
// 		LoadMovie(path);
// 	}
//
// 	void Update () {
// 		//現在のステートに応じて動画をデコードし、テクスチャに転送
//
// 		Decode();
// 	}
//
// 	void OnDisable () {
// 		//非表示になったためAnimationController同様に先頭にシーク
//
// 		Stop();
// 	}
//
// 	void OnEnable () {
// 		//前回のロードが途中で終わっていた場合は再開
// 		//CoroutineはSetActive(false)で停止する可能性があるため必要
//
// 		LoadMovie(path);
//
// 		//非表示から表示状態になったため、前回のデコード結果をリセット
//
// 		ClearTexture();
//
// 		//自動再生時は再生を開始する
//
// 		if(autoplay){
// 			Play();
// 		}
// 	}
//
// 	void OnDestroy () {
// 		//デコーダを開放
//
// 		Close();
// 	}
// 	
// 	void OnApplicationQuit () {
// 		//デコーダを開放
//
// 		Close();
// 	}
//
// 	public void Play(){
// 		//再生を開始する
//
// 		if(state==H2MDState.READY || state==H2MDState.STOP){
// 			state=H2MDState.PLAYING;
// 			Decode();
// 		}
// 	}
//
// 	public void Stop(){
// 		//再生を停止する
//
// 		if(state==H2MDState.PLAYING){
// 			state=H2MDState.STOP;
// 			m_time=0.0f;
// 		}
// 	}
//
// 	public void Close(){
// 		//再生を終了する
//
// 		if(m_movie!=null){
// 			//デコーダインスタンスを解放する
// 			m_movie.Dispose();
// 			m_movie=null;
//
// 			//明示的にテクスチャを解放する
// 			if(m_tex!=null){
// 				SetTexture(null,false);
// 				MonoBehaviour.Destroy(m_tex);
// 				m_tex=null;
// 			}
// 		}
// 		state=H2MDState.INIT;
// 	}
//
// 	public bool Ready(){
// 		//動画の読込完了後にtrueを返す
//
// 		return (state!=H2MDState.INIT && state!=H2MDState.FAILED);
// 	}
//
// 	public bool Playing(){
// 		//動画再生中にtrueを返す
//
// 		return (state==H2MDState.PLAYING);
// 	}
//
//
// /****************************************************************
//  * 内部変数
//  **/
//
// 	private H2MDMovie m_movie=null; 	//H2MD ムービーオブジェクト
// 	private double m_time=0;			//デコードフレームを決定するためのタイムカウンタ
// 	private Texture2D m_tex=null;		//H2MD ムービーオブジェクトが返したテクスチャ
// 	private bool m_fill_require=false;	//テクスチャのフィルが必要かどうか
// 	private int m_before_frame=-1;		//前回デコードしたフレーム番号
//
// 	enum H2MDState{
// 		INIT,	//読込待ち
// 		READY,	//読込完了
// 		PLAYING,//再生中
// 		STOP,	//再生終了
// 		FAILED	//読込失敗
// 	};
//
// 	H2MDState state=H2MDState.INIT;
//
// /****************************************************************
//  * 内部API
//  **/
//
// 	//AndroidではStreamingAssets経由ではアクセスできないためコールバック経由でロードする
//
// 	IEnumerator StreamingAssetsRequestGetCode(string filePath){
// 		//WWWクラス経由でStreamingAssetsを読み込む
//
// 		#if (UNITY_ANDROID && !UNITY_EDITOR) || (UNITY_WEBGL && !UNITY_EDITOR)
// 		string prefix="";
// 		#else
// 		string prefix="file://";
// 		#endif
// 		if(uri){
// 			prefix="";
// 		}
// 		WWW www = new WWW(prefix+filePath);
// 		yield return www;
// 		if(www.bytes.Length==0 || !string.IsNullOrEmpty(www.error)){
// 			Debug.Log(www.error);
// 			state=H2MDState.FAILED;
// 			yield return null;
// 		}
//
// 		//読み込んだ動画をキャッシュする
// 		
// 		if(cache!="" && www.bytes.Length>0){
// 			using (FileStream fs = new FileStream(Application.temporaryCachePath + "/"+cache, FileMode.Create, FileAccess.Write))
// 			using (BinaryWriter bw = new BinaryWriter(fs))
// 			{
// 				bw.Write(www.bytes);
// 			}
// 		}
//
// 		//動画を開く
//
// 		OpenMovie(null,www.bytes);
// 	}
//
// 	//動画の読込を行う
//
// 	private void LoadMovie(string filePath){
// 		//読み込み済み
//
// 		if(state!=H2MDState.INIT){
// 			return;
// 		}
//
// 		//Assets/StreamingASsets/[filePath]から動画を読み込み
//
// 		if(filePath==""){
// 			//Debug.Log("Please set H2MD file path");
// 			return;
// 		}
//
// 		if(uri==false){
// 			filePath = Application.streamingAssetsPath+"/"+filePath;
// 		}
//
// 		//キャッシュから読み込む
//
// 		if(cache!=""){
// 			if (System.IO.File.Exists(Application.temporaryCachePath + "/" + cache)){
// 				OpenMovie(Application.temporaryCachePath + "/"+cache,null);
// 				return;
// 			}
// 		}
//
// 		//ストリームの読込を行う
//
// 		bool use_www_class=false;
// 		#if UNITY_ANDROID
// 			use_www_class=true;
// 		#endif
// 		#if UNITY_WEBGL
// 			use_www_class=true;
// 		#endif
// 		
// 		if(use_www_class || uri){
// 			//Androidの場合はコールバック経由で読み込む
//
// 			if(fill){
// 				Texture2D tex=new Texture2D(1,1,TextureFormat.ARGB32,false);
// 				Fill(tex);
// 				SetTexture(tex,false);
// 			}
// 			StartCoroutine(StreamingAssetsRequestGetCode(filePath));
// 		}else{
// 			//Android以外の場合はfopenで読み込む
//
// 			OpenMovie(filePath,null);
// 		}
// 	}
//
// 	//動画ファイルを開く
//
// 	private void OpenMovie(String filePath,byte [] code) {
// 		//デコーダインスタンスを作成
//
// 		Close();
// 		m_movie=new H2MDMovie();
//
// 		//非同期デコードを有効化
//
// 		if(async){
// 			m_movie.SetAsyncDecodeMode(true);
// 		}
//
// 		//動画ファイルを開く
//
// 		if(code != null){
// 			m_tex=m_movie.OpenMem(code);
// 		}else{
// 			m_tex=m_movie.Open(filePath);
// 		}
//
// 		//動画ファイルを開くことに失敗した
//
// 		if(m_tex==null){
// 			Debug.Log("H2MD file not found "+filePath);
// 			Close();
// 			state=H2MDState.FAILED;
// 			return;
// 		}
//
// 		//テクスチャをクリア
//
// 		if(fill){
// 			Fill(m_tex);
// 		}
//
// 		//テクスチャをアタッチ
//
// 		SetTexture(m_tex,false);
//
// 		//読み込み完了
//
// 		state=H2MDState.READY;
// 		m_before_frame=-1;
//
// 		if(autoplay){
// 			Play();
// 		}
// 	}
//
// 	//H2MDのデコード結果をアタッチする
//
// 	private void SetTexture(Texture2D tex,bool editor){
// 		if(GetComponent<RawImage>()){
// 			GetComponent<RawImage>().texture=tex;
// 		}else{
// 			if(texture_id!=""){
// 				if(editor){
// 					GetComponent<Renderer>().sharedMaterial.SetTexture(texture_id,tex);
// 				}else{
// 					GetComponent<Renderer>().material.SetTexture(texture_id,tex);
// 				}
// 			}else{
// 				if(editor){
// 					GetComponent<Renderer>().sharedMaterial.mainTexture=tex;
// 				}else{
// 					GetComponent<Renderer>().material.mainTexture=tex;
// 				}
// 			}
// 		}
// 	}
// 	
// 	//テクスチャを0フィルする
//
// 	private void Fill(Texture2D tex){
// 		Color32 fillColor = new Color32(0,0,0,0);
// 		Color32[] fillColorArray =  tex.GetPixels32();
// 		for(int i = 0; i < fillColorArray.Length; i++){
// 			fillColorArray[i] = fillColor;
// 		}
// 		tex.SetPixels32( fillColorArray );
// 		tex.Apply();
// 	}
//
// 	//デコードを行う
//
// 	private void Decode(){
// 		//デコード可能状態か判定
//
// 		if(m_movie==null){
// 			return;
// 		}
// 		if(state!=H2MDState.PLAYING){
// 			return;
// 		}
//
// 		//デコードフレームを決定する
// 	
// 		int frame=(int)((m_time-delay)*m_movie.GetFrameRate()*speed)+offset;
// 		m_time+=Time.deltaTime;
//
// 		if(frame<0){
// 			return;
// 		}
//
// 		int frames=m_movie.GetTotalFrames()-offset;
// 		if(frame>=frames){
// 			if(loop){
// 				frame=(frame-offset)%frames+offset;
// 			}else{
// 				Stop();
// 				if(hold){
// 					frame=frames-1;
// 				}else{
// 					if(autorelease){
// 						Close();
// 						SetTexture(Texture2D.blackTexture,false);
// 						return;
// 					}else{
// 						ClearTexture();
// 						return;
// 					}
// 				}
// 			}
// 		}
//
// 		//前回と同じフレームはデコードを省略
//
// 		if(!async && m_before_frame==frame){
// 			return;
// 		}
//
// 		//デコードを行ってテクスチャを更新する
//
// 		if(m_movie.Decode(frame)!=H2MDDecoder.H2MDDEC_STATUS_SUCCESS){
// 			return;	//WebGLのデコード遅延を考慮
// 		}
// 		if(flip){
// 			m_movie.GetImageFlip();
// 		}else{
// 			m_movie.GetImage();
// 		}
// 		m_before_frame=frame;
// 		m_fill_require=true;
// 	}
//
// 	//テクスチャをクリアする
//
// 	private void ClearTexture(){
// 		if(fill && m_fill_require && m_tex!=null){
// 			Fill(m_tex);
// 			m_fill_require=false;
// 			m_before_frame=-1;
// 		}
// 	}
//
// 	//動画の差し替えを行う
//
// 	public bool Swap(byte [] stream){
// 		if(stream==null){
// 			return false;
// 		}
// 		if(state==H2MDState.INIT){
// 			return false;
// 		}
// 		m_before_frame=-1;
// 		return m_movie.SwapMem(stream);
// 	}
//
// /****************************************************************
//  * エディタでのプレビュー
//  **/
//
// #if UNITY_EDITOR
// 	private string preview_path=null;
//
// 	void OnValidate(){
// 		//エディタでプレビューを行う
//
// 		if(!preview){
// 			if(preview_path!=null){
// 				SetTexture(null,true);
// 				preview_path=null;
// 			}
// 			return;
// 		}
// 		if(preview_path==path){
// 			return;
// 		}
// 		preview_path=path;
// 		if (!System.IO.File.Exists(Application.streamingAssetsPath+"/"+path)){
// 			return;
// 		}
// 		PreviewOneFrame();
// 	}
//
// 	private void PreviewOneFrame(){
// 		H2MDMovie movie=new H2MDMovie();
// 		Texture2D tex=movie.Open(Application.streamingAssetsPath+"/"+path);
// 		if(tex!=null){
// 			Fill(tex);
// 			movie.Decode(offset);
// 			if(flip){
// 				movie.GetImageFlip();
// 			}else{
// 				movie.GetImage();
// 			}
// 			SetTexture(tex,true);
// 		}
// 		movie.Dispose();
// 	}
// #endif
// }