PrefabExtension
===============

## こんな人が使うのおすすめ

* `public GameObject go;`でPrefab以外を入れたくない（インスペクター上で）
* 複数のPrefabから生成したGameObjectの変更点を`一気にApply`したい


## Prefab型の追加

### 使い方

```cs
using UnityEngine;

public class Example : MonoBehaviour
{
	public Prefab prefab;
}
```

![](https://dl.dropboxusercontent.com/u/153254465/screenshot/%E3%82%B9%E3%82%AF%E3%83%AA%E3%83%BC%E3%83%B3%E3%82%B7%E3%83%A7%E3%83%83%E3%83%88%202014-02-11%2020.21.27.png)

#### Prefab

変数名 | 説明
---|---
gameObject| 実際のPrefab


### 仕様

* 見た目はGameObjectと同じ
* Prefabから生成されたGameObject（PrefabInstance）を格納すると強制的にPrefabを格納する


## PrefabExtension

### 使い方

```cs
using UnityEditor;

public class EditorExample
{
	[MenuItem("PrefabExtension/ApplyPrefabState")]
	static void ApplyPrefabState ()
	{
		Selection.gameObjects.ApplyPrefabState ();
	}
}
```

![](https://dl.dropboxusercontent.com/u/153254465/screenshot/%E3%82%B9%E3%82%AF%E3%83%AA%E3%83%BC%E3%83%B3%E3%82%B7%E3%83%A7%E3%83%83%E3%83%88%202014-02-11%2020.25.46.png)

### 仕様

* PrefabInstance以外をApplyすると`NullReferenceException`

```cs
void ApplyPrefabState (this GameObject prefabInstance, bool force = true)
void ApplyPrefabState (this GameObject[] prefabInstances, bool force = true)
```

変数名 | 説明
---|---
prefabInstance|Prefabから生成されたGameObject
prefabInstances|Prefabから生成されたGameObjectの配列
force|確認ダイアログを出す場合はfalse。デフォルトはtrue



## 分けて使いたい人は...

### Prefab型のみ使う

* PrefabDrawer.cs
* Prefab.cs

### PrefabExtensionのみ使う

* PrefabExtension.cs