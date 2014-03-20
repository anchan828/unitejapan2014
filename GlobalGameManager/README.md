GlobalGameManager
==============



## 使い方

1. GlobalGameManagerを継承したクラスを作成する

	```cs
	using UnityEngine;

	public class HogeManager : GlobalGameManager<HogeManager>
	{
	}
	```

	* 同時に`HogeManager.asset`が`Assets/GlobalGameManager/Resources/GlobalGameManager`に作成されます。
	* 同時に`HogeManagerInspector.cs`が`Assets/GlobalGameManager/Editor/Inspectors`に作成されます。

2. フィールドを作る

	```cs
	using UnityEngine;

	public class HogeManager : GlobalGameManager<HogeManager>
	{
    	public int globalInt = 0;
	    public string globalString = "";
    	public Texture2D globalTexture;
	}
	```

3. `Edit -> Project Settings`からアクセスする

	![](https://dl.dropboxusercontent.com/u/153254465/screenshot/%E3%82%B9%E3%82%AF%E3%83%AA%E3%83%BC%E3%83%B3%E3%82%B7%E3%83%A7%E3%83%83%E3%83%88%202014-03-20%2018.41.16.png)



## 仕様

* 作成される`.asset`はScriptableObject

	* なので制限などScriptableObjectの仕様に則る


* ScriptableObjectをResourcesフォルダに入れている

* `.asset`と`~Inspector.cs`が作成される場所は決まっている

	* どうしても変更したい場合は`Creator.cs`のパス（`inspectorFolderPath` , `globalGameManagerFolderPath`）を修正する


* ビルド時に`.asset`が無い場合、ゲーム実行時に`ScriptableObject.CreateInstance`で作成する。
	* デフォルト値はちゃんと設定しておきましょう。

## アドバンス的な

* MenuItemのパスを変更する

	* 自動生成される`~Inspector.cs`のMenuItemのところを変更すればOK

	```cs
	using UnityEditor;
	using UnityEngine;
	using System.Collections;

	[CustomEditor(typeof(HogeManager))]
	public class HogeManagerInspector : GlobalGameManagerInspector
	{
    	[MenuItem("Edit/Project Settings/HogeManager")]
	    private static void ShowManagerSettings()
    	{
        	Show<HogeManager>();
	    }
	}
	```
