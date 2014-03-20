unity-ReorderableList
=====================

![](https://dl.dropboxusercontent.com/u/153254465/screenshot/%E3%82%B9%E3%82%AF%E3%83%AA%E3%83%BC%E3%83%B3%E3%82%B7%E3%83%A7%E3%83%83%E3%83%88%202014-03-20%2021.11.26.png)


```
using UnityEngine;

public class Sample : MonoBehaviour
{
    public string[] texts = new string[2];
}
```

```
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Sample))]
public class SampleInspector : Editor
{
    private ReorderableList list = null;
    public override void OnInspectorGUI()
    {
        if (list == null)
        {
            list = new ReorderableList(serializedObject, serializedObject.FindProperty("texts"));
        }
        else
        {
            list.DoList();
        }
    }
}
```

## API

### Constructor

Name|Description
:---|:---
ReorderableList|

```
public ReorderableList(SerializedObject serializedObject, SerializedProperty serializedProperty, bool draggable = true, bool displayHeader = true, bool displayAddButton = true, bool displayRemoveButton = true)
```

Type|Name|Description
:---|:---|:---
SerializedObject|serializedObject|
SerializedProperty|serializedProperty|
bool|draggable| Can elements drag?
bool|displayHeader| Draw header , if true. The default is true.
bool|displayAddButton| Draw AddButton , if true. The default is true.
bool|displayRemoveButton| Draw RemoveButton , if true. The default is true.


### Variables

Type|Name|Description
:---|:---|:---
SerializedObject|serializedObject|
SerializedProperty|serializedProperty|
float|elementHeight| The default is 21.
DrawElementCallback|drawElementCallback|
DrawHeaderCallback|drawHeaderCallback|
AddCallbackDelegate|onAddDelegateCallback|
RemoveCallbackDelegate|onRemoveDelegateCallback|
SelectCallbackDelegate|onSelectCallback|
AddDropdownCallback|onAddDropdownCallback|

### Functions

Name|Description
:---|:---
DoList| Draw ReorderableList

### Delegates

Name|Description
:---|:---
DrawElementCallback|
DrawHeaderCallback|
SelectCallbackDelegate|
AddCallbackDelegate|
RemoveCallbackDelegate|


```
public delegate void DrawElementCallback(Rect rect, int index, bool selected, bool focused);

public delegate void DrawHeaderCallback(Rect rect);

public delegate void SelectCallbackDelegate(int index);

public delegate void AddCallbackDelegate();

public delegate void RemoveCallbackDelegate();
```
