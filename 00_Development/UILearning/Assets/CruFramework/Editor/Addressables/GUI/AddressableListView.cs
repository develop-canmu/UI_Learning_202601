using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CruFramework.Editor.Addressable
{
    public class AddressableListView : VisualElement
    {
        private Foldout header = null;
        public Foldout Header
        {
            get { return header; }
        }
        
        private ListView body = null;
        public ListView Body
        {
            get { return body; }
        }
        
        private AddressableListViewFooter footer = null;
        public AddressableListViewFooter Footer
        {
            get { return footer; }
        }
        
        public AddressableListView()
        {
            header = new Foldout();
            Add(header);
            
            body = new ListView();
            // ヘッダーは表示しない
            body.showFoldoutHeader = false;
            // フッターは表示しない
            body.showAddRemoveFooter = false;
            // サイズの表示はしない
            body.showBoundCollectionSize = false;
            // 要素の並び替えを許可する
            body.reorderable = true;
            body.reorderMode = ListViewReorderMode.Animated;
            // アイテムの高さは可変
            body.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            // 背景の色を交互に
            body.showAlternatingRowBackgrounds = AlternatingRowBackground.All;
            header.Add(body);
            
            footer = new AddressableListViewFooter();
            header.Add(footer);
        }
    }
}
