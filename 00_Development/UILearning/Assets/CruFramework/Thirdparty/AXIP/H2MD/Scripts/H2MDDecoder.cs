/* H2MD Unity Plugin Native Interface */
/* Copyright 2016-2018 AXELL CORPORATION */

using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Runtime.InteropServices;

public class H2MDDecoder
{

/****************************************************************
 *  定数・構造体定義
 **/

	/* エラーコード */
	
	public const Int32 H2MDDEC_STATUS_SUCCESS			  = 0;   /* 処理は成功した */
	public const Int32 H2MDDEC_STATUS_INVALID_ARGUMENT	  = 1;   /* 不正な引数を指定した */
	public const Int32 H2MDDEC_STATUS_INVALID_STATE		  = 2;   /* 現在の状態ではこの関数を呼ぶことはできない */
	public const Int32 H2MDDEC_STATUS_INVALID_VERSION	  = 3;   /* 指定したバージョンのファイルか構造体に対応していない */
	public const Int32 H2MDDEC_STATUS_FAILED_FILE_API	  = 4;   /* ファイルの読み込みに失敗した */
	public const Int32 H2MDDEC_STATUS_BROKEN			  = 6;   /* 壊れたH2MDファイルが渡された */
	public const Int32 H2MDDEC_STATUS_MEMORY_INSUFFICIENT = 7;   /* メモリが不足している  */
	public const Int32 H2MDDEC_STATUS_OTHER_ERROR		  = 128; /* その他のエラーが発生した */

	/* ムービー情報構造体 */
	
	[StructLayout(LayoutKind.Sequential)]
	public class h2mdMovieInfo
	{
		public UInt32 width;           /* 水平画素数 */
		public UInt32 height;          /* ライン数 */
		public UInt32 image_format;    /* ストリーム形式 */
		public UInt32 total_frames;    /* 保有フレーム数 */
		public UInt32 fps_numerator;   /* 分数表現したフレームレート(フレーム毎秒)の分子 */
		public UInt32 fps_denominator; /* 分数表現したフレームレート(フレーム毎秒)の分母 */
		public UInt32 flags;           /* H2MDDEC_FLAG定数の論理和 */
	}

	/* デコード画像フォーマット */
	
	public const Int32 H2MDDEC_IMAGE_BGRA            = 0; /* 1ピクセル32ビット B,G,R,A順 */
	public const Int32 H2MDDEC_IMAGE_RGBA            = 1; /* 1ピクセル32ビット R,G,B,A順 */

	/* マルチスレッドモード */
	
	public const Int32 H2MDDEC_MULTITHREAD_AUTO      = 0; /* 自動的にスレッドを使用する */
	
	/* デコードストリームフォーマット */

	public const Int32 H2MDDEC_STREAM_TYPE_RGB        = 0x00; /* RGB専用ストリーム */
	public const Int32 H2MDDEC_STREAM_TYPE_IMAGE_MASK = 0x0f; /* イメージ情報を取得するためのマスク */
	public const Int32 H2MDDEC_STREAM_TYPE_ALPHA_MASK = 0x10; /* α情報を取得するためのマスク */
	
	/* バージョン情報 */
	
	public const Int32 H2MDDEC_MOVIE_INFO_VERSION     = 1; /* 構造体バージョン */

	/* ファイルアクセスコールバック構造体 */

	public delegate IntPtr h2mdCallbackOpen(IntPtr args);
	public delegate Int32 h2mdCallbackSeek(IntPtr fp, Int64 offset);
	public delegate Int64 h2mdCallbackTell(IntPtr fp);
	public delegate Int64 h2mdCallbackSize(IntPtr fp);
	public delegate Int32 h2mdCallbackRead(IntPtr dest, Int64 size, IntPtr fp);
	public delegate Int32 h2mdCallbackClose(IntPtr fp);

	[StructLayout(LayoutKind.Sequential)]
	public struct h2mdDecoderCallback
	{
	  public h2mdCallbackOpen  fopen;     /* ユーザ定義fopen関数 */
	  public h2mdCallbackSeek  fseek;     /* ユーザ定義fseek関数 */
	  public h2mdCallbackTell  ftell;     /* ユーザ定義ftell関数 */
	  public h2mdCallbackRead  fread;     /* ユーザ定義fread関数 */
	  public h2mdCallbackSize  fsize;     /* ユーザ定義fsize関数 */
	  public h2mdCallbackClose fclose;    /* ユーザ定義fclose関数 */
	}

	public const Int32 H2MDDEC_CALLBACK_VERSION           = 1;

	/* Native Binary 定義 */
	
	#if (UNITY_IPHONE && !UNITY_EDITOR) || (UNITY_WEBGL && !UNITY_EDITOR) || (UNITY_SWITCH && !UNITY_EDITOR)
		const String LIBRARY_NAME="__Internal";
	#else
		const String LIBRARY_NAME="h2md_dec";
	#endif

	/* 無効なデコーダオブジェクトの定義 */

	public static System.IntPtr DECODER_NULL=IntPtr.Zero;

/**
 *  H2MDデコーダオブジェクトを作成します。
 *  引数:
 *    decoder       - デコーダオブジェクトの格納先へのポインタ
 *    image_type    - imageに格納するカラーフォーマット
 *    num_thread    - H2MDDEC_MULTITHREAD_AUTOもしくはデコードスレッド数
 *  返値:
 *    成功した場合、H2MDDEC_STATUS_SUCCESSを返す。そうでなければ、それ以外のH2MDDEC_STATUS_xxx定数を返す。
 */

	[DllImport(LIBRARY_NAME)]
	public static extern int h2mdCreate (ref System.IntPtr decoder,Int32 image_type,Int32 num_thread);

/**
 *  H2MDデコーダオブジェクトを廃棄します。
 *  引数:
 *    decoder - デコーダオブジェクト
 */

	[DllImport(LIBRARY_NAME)]
	public static extern void h2mdDestroy (System.IntPtr decoder);

/**
 *  H2MDデコーダオブジェクトを開きます。（ファイル）
 *  引数:
 *    decoder       - デコーダオブジェクト
 *    strm_filename - デコードするH2MDムービーファイル名
 *  返値:
 *    成功した場合、H2MDDEC_STATUS_SUCCESSを返す。そうでなければ、それ以外のH2MDDEC_STATUS_xxx定数を返す。
 *  解説：
 *    デコード対象はH2MDファイル。callbackにNULLが指定されている場合、ファイルは標準APIを使用して読み込みます。
 */

	#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
	[DllImport(LIBRARY_NAME, EntryPoint = "h2mdOpenStreamFileW", CharSet=CharSet.Unicode)]
	public static extern int h2mdOpenStreamFile (System.IntPtr decoder, [MarshalAs(UnmanagedType.LPWStr)]string strm_filename);
	#else
	[DllImport(LIBRARY_NAME, EntryPoint = "h2mdOpenStreamFileA", CharSet=CharSet.Ansi)]
	public static extern int h2mdOpenStreamFile (System.IntPtr decoder, [MarshalAs(UnmanagedType.LPStr)]string strm_filename);
	#endif

/**
*  H2MD_ストリームファイルを開きます。（メモリ）
*  引数:
*    decoder       - h2mdCreate が返したデコーダインスタンスポインタ
*    data          - デコードするH2MDデータへのポインタ
*    data_size     - デコードするH2MDデータのバイトサイズ
*  返値:
*    成功した場合、H2MDDEC_STATUS_SUCCESSを返す。そうでなければ、それ以外のH2MDDEC_STATUS_xxx定数を返す。
*  解説:
*    デコード対象はH2MD_ストリームファイルです。
*/

	[DllImport(LIBRARY_NAME)]
	public static extern int h2mdOpenStreamMem (System.IntPtr decoder, IntPtr code, Int32 code_size);

/**
*  H2MD_ストリームファイルを開きます。（ユーザ定義ファイルアクセスコールバック）
*  引数:
*    decoder       - h2mdCreate が返したデコーダインスタンスポインタ
*    fopen_args    - H2MDDEC_USER_API_FOPENに通知される引数ポインタ
*    callback      - ユーザ定義ファイルアクセスコールバック構造体
*    version       - コールバック構造体のバージョン(H2MDDEC_CALLBACK_VERSION)
*  返値:
*    成功した場合、H2MDDEC_STATUS_SUCCESSを返す。そうでなければ、それ以外のH2MDDEC_STATUS_xxx定数を返す。
*  解説:
*    デコード対象はH2MD_ストリームファイルです。
*    WebGL版では使用できません。
*/

	[DllImport(LIBRARY_NAME)]
	public static extern int h2mdOpenStreamEx(System.IntPtr decoder, IntPtr fopen_args, h2mdDecoderCallback callback, Int32 version);

/**
 *  H2MDムービーのフレームをデコードします。
 *  引数:
 *    decoder     - デコーダオブジェクト
 *    index       - フレームインデックス
 *  返値:
 *    成功した場合、H2MDDEC_STATUS_SUCCESSを返す。そうでなければ、それ以外のH2MDDEC_STATUS_xxx定数を返す。
 *  解説：
 *    該当ムービを内部バッファにデコードします。
 *    この命令の後、以降の画像取得関数が有効になります。
 */

	[DllImport(LIBRARY_NAME)]
	public static extern int h2mdDecode (System.IntPtr decoder, int index);

/**
 *  H2MDムービーのデコードしたフレームの画像を取得します。
 *  引数:
 *    decoder     - デコーダオブジェクト
 *    image       - フレーム画像を取得するバッファへのポインタ
 *    buf_size    - imageのバイト数
 *    image_stride- imageバッファの水平ラインバイト数
 *  返値:
 *    成功した場合、H2MDDEC_STATUS_SUCCESSを返す。そうでなければ、それ以外のH2MDDEC_STATUS_xxx定数を返す。
 *    h2mdGetImageFlipでは上下反転して取得する。
 */

	[DllImport(LIBRARY_NAME)]
	public static extern int h2mdGetImage (System.IntPtr decoder, IntPtr image, Int32 buf_size, Int32 image_stride);

	[DllImport(LIBRARY_NAME)]
	public static extern int h2mdGetImageFlip (System.IntPtr decoder, IntPtr image, Int32 buf_size, Int32 image_stride);

/**
 *  H2MDムービーの全体情報を取得します。
 *  引数:
 *    decoder - デコーダオブジェクト
 *    info    - ムービー情報を取得する h2mdMovieInfo 構造体へのポインタ
 *    version - h2mdMovieInfo構造体のバージョン(H2MDDEC_MOVIE_INFO_VERSION)
 *  返値:
 *    成功した場合、H2MDDEC_STATUS_SUCCESSを返す。そうでなければ、それ以外のH2MDDEC_STATUS_xxx定数を返す。
 *  解説:
 *    versionに適切なバージョンがセットされていない場合、失敗します。
 */

	[DllImport(LIBRARY_NAME)]
	public static extern int h2mdGetMovieInfo (System.IntPtr decoder, [In,Out] h2mdMovieInfo info,Int32 version);
}
