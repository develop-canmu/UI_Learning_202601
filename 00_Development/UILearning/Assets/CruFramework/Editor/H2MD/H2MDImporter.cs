using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.AssetImporters;
using System.IO;
using CruFramework.H2MD;

namespace CruFramework.Editor.H2MD
{
	[ScriptedImporter(1, "h2md")]
	public class H2MDImporter : ScriptedImporter
	{
		public override void OnImportAsset(AssetImportContext ctx)
		{
			H2MDAsset asset = H2MDAsset.Create(File.ReadAllBytes(ctx.assetPath));
			ctx.AddObjectToAsset("h2md", asset);
		}
	}
}