using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

using Pjfb.Master;

namespace Pjfb.Editor.LocalMasterDataEditor
{
    public class LocalMasterDataEditor : EditorWindow
    {
        /// <summary>Window開く</summary>
        [MenuItem("Pjfb/Master/LocalEditor")]
        public static void Open()
        {
            GetWindow<LocalMasterDataEditor>();
        }

        private class MasterInfo
        {
            public PropertyInfo PropertyInfo;
            public Type MasterType;
        }

        /// <summary>データ保管場所</summary>
        private const string DataDictionary = "_dataDictionary";
        /// <summary>生データ保管場所</summary>
        private const string RawData = "rawData";
        /// <summary>配列区切る用</summary>
        private const string ArraySplitString = ", ";
        
        /// <summary>ページ切り替えボタンのサイズ</summary>
        private const int PageButtonWidth = 30;

        /// <summary>1ページに表示される最大数</summary>
        private const int MaxPageIndex = 30;
        /// <summary>テーブル表示時のカラムの幅</summary>
        private const float TableColumnWidth = 150f;
        
        #region 表示用Element
        /// <summary>表示のベース</summary>
        private VisualElement _body;
        /// <summary>テーブル名表示</summary>
        private VisualElement _masterListElement;
        /// <summary>テーブル名検索</summary>
        private ToolbarSearchField _masterSearchField;
        /// <summary>テーブル名表示用</summary>
        private ScrollView _masterNameList;
        /// <summary>カラム表示</summary>
        private VisualElement _masterFieldElement;
        
        /// <summary>テーブル表示</summary>
        private VisualElement _masterDataListView;
        /// <summary>テーブルメイト検索欄表示</summary>
        private VisualElement _masterToolElement;
        /// <summary>テーブル表示時の現在表示しているテーブル名</summary>
        private TextElement _masterNameText;
        /// <summary>レコード検索</summary>
        private ToolbarSearchField _searchRecordField;
        /// <summary>テーブル表示</summary>
        private ScrollView _masterColumnView;
        /// <summary>カラム名表示</summary>
        private VisualElement _columnNameElements;

        /// <summary>移動先表示</summary>
        private VisualElement _pageField;
        /// <summary>一番最初へ移動する</summary>
        private Button _firstPageButton;
        /// <summary>前のページ</summary>
        private Button _prevPageButton;
        /// <summary>ページ選択</summary>
        private DropdownField _pageDropDown;
        /// <summary>次のページ</summary>
        private Button _nextPageButton;
        /// <summary>最後のページ</summary>
        private Button _lastPageButton;
        #endregion
        
        /// <summary>MasterInfoを格納しているクラス</summary>
        private List<MasterInfo> _masterInfoList = new ();
        /// <summary>表示するデータをコピーしておく配列(検索時の為に)</summary>
        private List<MasterInfo> _copyMasterInfoList;
        /// <summary>テーブルの全レコードを格納する</summary>
        private List<object> _masterValues = new ();
        /// <summary>レコードのコピー配列</summary>
        private List<object> _copyMasterValues;
        /// <summary>id検索用のDictionary</summary>
        private Dictionary<long, object> _searchMasterValuesDic = new();

        /// <summary>値の取得先</summary>
        private IDictionary _values;
        /// <summary>現在表示しているテーブルの情報</summary>
        private MasterInfo _currentPageMasterInfo;
        /// <summary>表示する情報格納先</summary>
        private PropertyInfo _rawData;
        /// <summary>表示する情報格納先</summary>
        private FieldInfo[] _fields;
        
        /// <summary>現在のページ数</summary>
        private int _currentPageCount;
        /// <summary>ページ数のstring格納</summary>
        private List<string> _pageStringList = new ();
        
        /// <summary>最大ページ数(0からなので-1しておく)</summary>
        private int GetMaxPageCount() => Mathf.CeilToInt((float)_masterValues.Count / MaxPageIndex) - 1;
        
        // セーブデータ
        private object[] saveInfo = null;

        #region CancellationToken
        private CancellationTokenSource destroyCancellationTokenSource;
        private CancellationToken destroyCancellationToken => destroyCancellationTokenSource.Token;
        
        private void Awake()
        {
            destroyCancellationTokenSource = new CancellationTokenSource();
        }
        
        private void OnDestroy()
        {
            if (destroyCancellationTokenSource != null)
            {
                destroyCancellationTokenSource.Cancel();
                destroyCancellationTokenSource.Dispose();
                destroyCancellationTokenSource = null;
            }
        }
        #endregion
        
        protected void OnEnable()
        {
            // 全ての表示のElement作成
            _body = new TwoPaneSplitView
            {
                name = "body",
                fixedPaneIndex = 0,
                fixedPaneInitialDimension = 250,
                orientation = TwoPaneSplitViewOrientation.Horizontal
            };
            rootVisualElement.Add(_body);
            
            // 表示元のレイアウト作成
            CreateMasterListLayout();
            
            // テーブル表示作成
            CreateColumnPageLayout();

            // テーブル名表示のデータ設定
            CreateMasterList();
        }

        #region レイアウト作成
        /// <summary>テーブルのリスト表示設定</summary>
        private void CreateMasterListLayout()
        {
            // テーブル名表示のElement作成
            _masterListElement = new VisualElement
            {
                name = "masterListElement",
                style =
                {
                    minWidth = 50.0f,
                },
            };
            _body.Add(_masterListElement);

            _masterSearchField = new ToolbarSearchField
            {
                name = "masterSearchField",
                style = { width = StyleKeyword.Auto },
            };
            _masterSearchField.RegisterValueChangedCallback(value =>
            {
                SearchByTableName(value.newValue);
            });
            _masterListElement.Add(_masterSearchField);
            // リストビュー
            _masterNameList = new ScrollView
            {
                name = "masterNameList",
                mode = ScrollViewMode.Vertical,
            };
            _masterListElement.Add(_masterNameList);
            
            // カラム表示用のElement作成
            _masterFieldElement = new VisualElement
            {
                name ="masterFieldElement",
                style =
                {
                    flexDirection = FlexDirection.Column,
                    minWidth = 100f
                }
            };
            _body.Add(_masterFieldElement);
        }
        
        /// <summary>テーブル表示のレイアウト作成</summary>
        private void CreateColumnPageLayout()
        {
            _masterToolElement = new VisualElement
            {
                name = "masterToolElement",
                style =
                {
                    maxHeight = 50,
                    flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row )
                },
            };
            _masterFieldElement.Add(_masterToolElement);
            // テーブル名
            _masterNameText = new TextElement
            {
                name = "masterNameText",
                style =
                {
                    fontSize = 22,
                    flexGrow = 1
                }
            };
            _masterToolElement.Add(_masterNameText);
            
            _searchRecordField = new ToolbarSearchField
            {
                name = "masterSearchField",
                style =
                {
                    unityTextAlign = TextAnchor.MiddleRight,
                    visibility = new StyleEnum<Visibility>(Visibility.Hidden )
                },
            };
            _searchRecordField.RegisterValueChangedCallback(value =>
            {
                SearchRecordById(value.newValue);
            });
            _masterToolElement.Add(_searchRecordField);

            // スクロール
            _masterColumnView = new ScrollView
            {
                name = "masterColumnView",
                mode = ScrollViewMode.Horizontal,
                style =
                {
                    flexGrow = 1.0f
                }
            };
            _masterColumnView.Q<VisualElement>("unity-content-container").style.flexDirection = FlexDirection.Column;
            
            // カラム名
            _columnNameElements = new VisualElement
            {
                name = "columnNameElements",
                style =
                {
                    height = 20.0f,
                    width = StyleKeyword.Auto,
                    flexDirection = FlexDirection.Row
                }
            };
            _masterColumnView.Add(_columnNameElements);
            
            // 各テーブル表示
            _masterDataListView = new VisualElement
            {
                name = "masterDataListView",
                style =
                {
                    flexDirection = FlexDirection.Column
                }
            };
            _masterColumnView.Add(_masterDataListView);
            _masterFieldElement.Add(_masterColumnView);
            
            // ページ移動用のElement
            _pageField = new VisualElement
            {
                name = "pageField",
                style =
                {
                    flexDirection = FlexDirection.Row
                }
            };
            _masterFieldElement.Add(_pageField);
        }
        #endregion

        /// <summary>テーブル名の設定</summary>
        private void CreateMasterList()
        {
            _masterInfoList.Clear();
            // テーブルを取得
            PropertyInfo[] masterTables = typeof(MasterManager).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach(PropertyInfo master in masterTables)
            {
                Type type = master.PropertyType;
                Type masterObjectType = null;
                while(true)
                {
                    if(type == null || type == typeof(object))break;
                    
                    // マスタデータか調べる
                    if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(MasterContainerBase<,>))
                    {
                        masterObjectType = type.GetGenericArguments()[1];
                        
                        break;
                    }
                    
                    type = type.BaseType;
                }
                
                if(masterObjectType == null)continue;
                
                MasterInfo masterInfo = new MasterInfo
                {
                    PropertyInfo = master,
                    MasterType = masterObjectType
                };
                _masterInfoList.Add(masterInfo);
            }

            _copyMasterInfoList = new List<MasterInfo>(_masterInfoList);
            
            // テーブル選択のボタン生成
            CreateButtons();
        }

        /// <summary>カラムボタン生成</summary>
        private void CreateButtons()
        {
            foreach (var masterInfo in _masterInfoList)
            {
                Button button = new Button
                {
                    style =
                    {
                        unityTextAlign = TextAnchor.LowerLeft
                    },
                    name = masterInfo.PropertyInfo.Name,
                    text = masterInfo.PropertyInfo.Name,
                    clickable = new Clickable(() => { SelectMaster(masterInfo); })
                };
                _masterNameList.Add(button);
            }
        }

        /// <summary>テーブルの作成</summary>
        private void SelectMaster(MasterInfo masterInfo)
        {
            // 初期化
            _currentPageCount = 0;
            _masterValues.Clear();
            _searchMasterValuesDic.Clear();
            _currentPageMasterInfo = masterInfo;
            // テーブル名名前表示
            _masterNameText.text = _currentPageMasterInfo.MasterType.Name;
            _searchRecordField.style.visibility = new StyleEnum<Visibility>(Visibility.Visible);

            #region カラム名表示
            // カラム名
            _columnNameElements.Clear();
            // publicかつinstanceされているプロパティの取得
            _rawData = _currentPageMasterInfo.MasterType.GetProperty(RawData, BindingFlags.Instance | BindingFlags.Public);
            // 存在チェック
            if (_rawData == null)
            {
                CruFramework.Logger.LogError($"not found: {RawData} in {_currentPageMasterInfo.PropertyInfo.Name}");
                return;
            }
            // publicではなくinstanceされていて検索した型で宣言されている
            _fields = _rawData.PropertyType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

            // カラム名表示用のボタン作成
            foreach (FieldInfo field in _fields)
            {
                Button button = new Button
                {
                    style =
                    {
                        width = TableColumnWidth
                    },
                    text = $"{field.Name} [{field.FieldType.Name}]"
                };
                _columnNameElements.Add(button);
            }
            #endregion

            #region データの調査
            // 実態を取得
            object valueObject = _currentPageMasterInfo.PropertyInfo.GetValue(MasterManager.Instance, null);
            // フィールドを取得
            FieldInfo dataField = valueObject.GetType().GetField(DataDictionary, BindingFlags.Instance | BindingFlags.NonPublic);
            // 存在チェック
            if (dataField == null)
            {
                CruFramework.Logger.LogError($"not found: {DataDictionary} in {_currentPageMasterInfo.PropertyInfo.Name}");
                return;
            }
            // 値を取得
            _values = (IDictionary)dataField.GetValue(valueObject);

            // for文で回せるように(indexで参照できるように)Listにする
            foreach (var value in _values.Values)
            {
                _masterValues.Add(value);
            }

            _copyMasterValues = new List<object>(_masterValues);

            // 検索用データ追加
            foreach (object values in _masterValues)
            {
                object rawDataValue = _rawData.GetValue(values);
                long v = (long)_fields[0].GetValue(rawDataValue);
                _searchMasterValuesDic.Add(v, values);
            }
            
            #endregion
            // テーブル表示
            ShowPage(true);
        }

        /// <summary>テーブルの表示</summary>
        private void ShowPage(bool refreshPageDropDown = false)
        {
            // 初期化
            _masterDataListView.Clear();
            // ページ移動を表示しなおす
            CreateMovePageLayout(refreshPageDropDown);

            if (refreshPageDropDown)
            {
                _masterDataListView.style.translate = new Translate(0, 0);
                _masterColumnView.scrollOffset = new Vector2(0, 0);
            }

            // 1レコードずつ表示作成する
            for (int i = MaxPageIndex * _currentPageCount; i < MaxPageIndex * (_currentPageCount + 1) && i < _masterValues.Count; i++)
            {
                VisualElement fieldElements = new VisualElement
                {
                    style =
                    {
                        flexDirection = FlexDirection.Row,
                        minHeight = 20.0f
                    }
                };
                _masterDataListView.Add(fieldElements);

                // 各カラムのデータ表示
                foreach (FieldInfo field in _fields)
                {
                    object rawDataValue = _rawData.GetValue(_masterValues[i]);
                    object v = field.GetValue(rawDataValue);

                    VisualElement inputElement;
                    // キャストできるかで判定
                    switch (v)
                    {
                        // int
                        case int intValue:
                            inputElement = new IntegerField
                            {
                                value = intValue
                            };
                            inputElement.RegisterCallback<ChangeEvent<int>>(newValue =>
                            {
                                field.SetValue(rawDataValue, newValue.newValue);
                                SaveMaster();
                            });
                            break;
                        // intの配列
                        case int[] intArray:
                            inputElement = new TextField
                            {
                                value = string.Join(ArraySplitString, intArray)
                            };
                            inputElement.RegisterCallback<ChangeEvent<string>>(newValue =>
                            {
                                var newStringValues = newValue.newValue.Split(ArraySplitString);
                                List<long> result = new ();
                                foreach (var newStringValue in newStringValues)
                                {
                                    if (int.TryParse(newStringValue, out int value))
                                    {
                                        result.Add(value);
                                    }
                                }
                                field.SetValue(rawDataValue, result.ToArray());
                                SaveMaster();
                            });
                            break;
                        // long
                        case long longValue:
                            inputElement = new LongField
                            {
                                value = longValue
                            };
                            inputElement.RegisterCallback<ChangeEvent<long>>(newValue =>
                            {
                                field.SetValue(rawDataValue, newValue.newValue);
                                SaveMaster();
                            });
                            break;
                        // longの配列
                        case long[] longArray:
                            inputElement = new TextField
                            {
                                value = string.Join(ArraySplitString, longArray)
                            };
                            inputElement.RegisterCallback<ChangeEvent<string>>(newValue =>
                            {
                                var newStringValues = newValue.newValue.Split(ArraySplitString);
                                List<long> result = new ();
                                foreach (var newStringValue in newStringValues)
                                {
                                    if (long.TryParse(newStringValue, out long value))
                                    {
                                        result.Add(value);
                                    }
                                }
                                field.SetValue(rawDataValue, result.ToArray());
                                SaveMaster();
                            });
                            break;
                        // string
                        case string stringValue:
                            inputElement = new TextField
                            {
                                value = stringValue
                            };
                            inputElement.RegisterCallback<ChangeEvent<string>>(newValue =>
                            {
                                field.SetValue(rawDataValue, newValue.newValue);
                                SaveMaster();
                            });
                            break;
                        // bool
                        case bool boolValue:
                            inputElement = new Toggle
                            {
                                style =
                                {
                                    alignSelf = Align.Center
                                },
                                value = boolValue
                            };
                            inputElement.RegisterCallback<ChangeEvent<bool>>(newValue =>
                            {
                                field.SetValue(rawDataValue, newValue.newValue);
                                SaveMaster();
                            });
                            break;
                        // 例外はJson表示
                        default:
                            inputElement = new Button
                            {
                                text = v.ToString(),
                                clickable = new Clickable(() => { OpenJson(destroyCancellationToken).Forget(); })
                            };
                            // Json表示
                            async UniTask OpenJson(CancellationToken token)
                            {
                                token.ThrowIfCancellationRequested();
                                string vJson = JsonConvert.SerializeObject(v);
                                string alignmentJson = DebugToolUtility.AlignmentJson(vJson);
                                LocalMasterDataJsonView jsonView = LocalMasterDataJsonView.Open(alignmentJson);
                                string newValue = await jsonView.WaitCloseAsync(token);
                                object value = JsonConvert.DeserializeObject(newValue, v.GetType());
                                field.SetValue(rawDataValue, value);
                                v = field.GetValue(rawDataValue);
                                SaveMaster();
                            }
                            break;
                    }

                    // ChangeEvent<object>だとデータをとってこれなかったので履歴用にコメントアウト
                    /*
                    // ボタンとデータが配列以外の場合
                    if (inputElement.GetType() != typeof(Button) && !field.FieldType.IsArray)
                    {
                        inputElement.RegisterCallback<ChangeEvent<object>>(newValue =>
                        {
                            field.SetValue(rawDataValue, newValue.newValue);
                        });
                    }
                    */

                    inputElement.style.width = TableColumnWidth;
                    fieldElements.Add(inputElement);
                }
            }

            #region データ追加用
            // 追加用のElement
            VisualElement addField = new VisualElement
            {
                name = "addField",
                style =
                {
                    flexDirection = FlexDirection.Row
                }
            };
            _masterDataListView.Add(addField);

            // 追加用のid入力フォーム
            LongField idField = new LongField
            {
                name = "idField",
                style =
                {
                    width = 60.0f
                }
            };
            addField.Add(idField);

            // 追加ボタン
            Button addButton = new Button
            {
                style =
                {
                    width = 20.0f
                },
                text = "+",
                clickable = new Clickable(() =>
                {
                    long id = idField.value;
                    _values.Add(id, Activator.CreateInstance(_currentPageMasterInfo.MasterType, Activator.CreateInstance(_rawData.PropertyType)));
                    // 再読み込み
                    SelectMaster(_currentPageMasterInfo);
                })
            };
            addField.Add(addButton);

            #endregion
        }
        
        /// <summary>Masterの保存</summary>
        private void SaveMaster()
        {
            // 保存用の情報取得
            if (saveInfo == null)
            {
                saveInfo = new object[3];
                saveInfo[0] = (string)typeof(MasterManager).GetMethod("CreateDataDirectory", BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(MasterManager.Instance, null);
                saveInfo[1] = (string)typeof(MasterManager).GetField("saveIV", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null);
                saveInfo[2] = (string)typeof(MasterManager).GetField("savePassKey", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null);
            }
            
            // コンテナ取得
            object value = _currentPageMasterInfo.PropertyInfo.GetValue(MasterManager.Instance, null);
            // コンテナのインターフェース取得
            Type type = value.GetType().GetInterfaces()[0];
            // 保存用のメソッド取得
            MethodInfo method = type.GetMethod("SaveMaster");
            // 保存
            method!.Invoke(value, saveInfo);
        }
        
        /// <summary>ページ移動の表示</summary>
        private void CreateMovePageLayout(bool refreshDropDown = false)
        {
            _pageField.Clear();

            // 前のページへの移動
            if (_currentPageCount > 0)
            {
                _firstPageButton ??= new Button
                {
                    name = "firstPageButton",
                    text = "<<",
                    style =
                    {
                        width = PageButtonWidth
                    },
                    clickable = new Clickable(() =>
                    {
                        _currentPageCount = 0;
                        ShowPage();
                    })
                };
                _pageField.Add(_firstPageButton);

                // 前のページ
                _prevPageButton ??= new Button
                {
                    name = "prevPageButton",
                    text = "<",
                    style =
                    {
                        width = PageButtonWidth
                    },
                    clickable = new Clickable(() =>
                    {
                        if (_currentPageCount > 0)
                            _currentPageCount--;
                        ShowPage();
                    })
                };
                _pageField.Add(_prevPageButton);
            }

            // ドロップダウン
            if (_pageDropDown == null)
            {
                _pageDropDown = new DropdownField
                {
                    name = "pageDropDown",
                    choices = _pageStringList,
                };
                _pageDropDown.RegisterValueChangedCallback(value => { ChangePage(value.newValue); });
            }

            if (refreshDropDown)
            {
                // ページのドロップダウンの作り直し
                _pageStringList.Clear();
                for (int i = _currentPageCount; i <= GetMaxPageCount(); i++)
                {
                    _pageStringList.Add((i + 1).ToString());
                }
            }

            _pageDropDown.SetValueWithoutNotify((_currentPageCount + 1).ToString());
            _pageField.Add(_pageDropDown);

            // 後ろのページへの移動
            if (_currentPageCount < GetMaxPageCount())
            {
                _nextPageButton ??= new Button
                {
                    name = "nextPageButton",
                    text = ">",
                    style =
                    {
                        width = PageButtonWidth
                    },
                    clickable = new Clickable(() =>
                    {
                        if (_currentPageCount < GetMaxPageCount())
                        {
                            _currentPageCount++;
                            ShowPage();
                        }
                    })
                };

                _pageField.Add(_nextPageButton);

                // 最後のページ表示
                _lastPageButton ??= new Button
                {
                    name = "lastPageButton",
                    text = ">>",
                    style =
                    {
                        width = PageButtonWidth
                    },
                    clickable = new Clickable(() =>
                    {
                        if (_currentPageCount < GetMaxPageCount())
                        {
                            _currentPageCount = GetMaxPageCount();
                            ShowPage();
                        }
                    })
                };

                _pageField.Add(_lastPageButton);
            }
        }

        /// <summary>ページの移動</summary>
        private void ChangePage(string value)
        {
            int.TryParse(value, out int result);
            _currentPageCount = result - 1;
            ShowPage();
        }

        /// <summary>テーブル名検索</summary>
        private void SearchByTableName(string value)
        {
            _masterNameList.Clear();
            _masterInfoList = _copyMasterInfoList.Where(data => data.PropertyInfo.Name.Contains(value)).ToList();
            CreateButtons();
        }

        /// <summary>レコード検索</summary>
        private void SearchRecordById(string value)
        {
            // 初期化
            _currentPageCount = 0;
            _masterDataListView.Clear();
            
            if (int.TryParse(value, out int id))
            {
                if (_searchMasterValuesDic.TryGetValue(id, out object values))
                {
                    _masterValues = new List<object>
                    {
                        values
                    };
                }
                else
                {
                    
                    CruFramework.Logger.LogError($"not found record by id: {id} in {_currentPageMasterInfo.PropertyInfo.Name}");
                    return;
                }
            }
            else
            {
                if (!_masterValues.Equals(_copyMasterValues))
                {
                    _masterValues = _copyMasterValues;
                }
            }
            ShowPage(true);
        }
    }
}
