using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.Collections;

public class ReorderableList
{

    #region Constructors
    public ReorderableList(SerializedObject serializedObject, SerializedProperty serializedProperty, bool draggable = true, bool displayHeader = true, bool displayAddButton = true, bool displayRemoveButton = true)
    {
        name = serializedProperty.name;
        ctor =
                constructorInfo_serialized.Invoke(new object[] { serializedObject, serializedProperty, draggable, displayHeader, displayAddButton, displayRemoveButton });
        ctor.GetType().GetProperty("index").SetValue(ctor, 0, null);

        InitdefaultCallback();
    }
    //    TODO
    //    public ReorderableList(IList list, Type elemenType, bool draggable = true, bool displayHeader = true,
    //        bool displayAddButton = true, bool displayRemoveButton = true)
    //    {
    //        ctor =
    //            constructorInfo.Invoke(new object[] { list, elemenType, draggable, displayHeader, displayAddButton, displayRemoveButton });
    //        ctor.GetType().GetProperty("index").SetValue(ctor, 0, null);
    //    }
    #endregion

    #region Propertis
    public SerializedObject serializedObject
    {
        get
        {
            return serializedProperty.serializedObject;
        }
    }

    public SerializedProperty serializedProperty
    {
        get
        {
            return (SerializedProperty)ctor.GetType().GetProperty("serializedProperty").GetValue(ctor, new object[0]);
        }
    }


    public float elementHeight
    {
        get
        {
            return (float)ctor.GetType().GetField("elementHeight").GetValue(ctor);
        }
        set
        {
            ctor.GetType().GetField("elementHeight").SetValue(ctor, value);
        }
    }

    public DrawElementCallback drawElementCallback
    {
        get
        {
            return _drawElementCallback;
        }

        set
        {
            _drawElementCallback += value;
            SetDelegate("ElementCallbackDelegate", "drawElementCallback", "ElementCallback");
        }
    }
    public DrawHeaderCallback drawHeaderCallback
    {
        get
        {
            return _drawHeaderCallback;
        }
        set
        {
            _drawHeaderCallback += value;
            SetDelegate("HeaderCallbackDelegate", "drawHeaderCallback", "HeaderCallback");
        }
    }

    public AddCallbackDelegate onAddDelegateCallback
    {
        get
        {
            return _onAddDelegateCallback;
        }

        set
        {
            _onAddDelegateCallback += value;

            SetDelegate("AddCallbackDelegate", "onAddCallback", "AddCallback");
        }
    }



    public RemoveCallbackDelegate onRemoveDelegateCallback
    {
        get
        {
            return _onRemoveDelegateCallback;
        }

        set
        {
            _onRemoveDelegateCallback += value;
            SetDelegate("RemoveCallbackDelegate", "onRemoveCallback", "RemoveCallback");
        }
    }

    public SelectCallbackDelegate onSelectCallback
    {
        get
        {
            return _onSelectCallback;
        }
        set
        {
            _onSelectCallback += value;
            SetDelegate("SelectCallbackDelegate", "onSelectCallback", "SelectCallback");
        }
    }

    public AddDropdownCallbackDelegate onAddDropdownCallback
    {
        get
        {
            return _onAddDropdownCallback;
        }
        set
        {
            _onAddDropdownCallback += value;
            SetDelegate("AddDropdownCallbackDelegate", "onAddDropdownCallback", "AddDropdownCallback");
        }
    }

    #endregion Propertis


    #region Methods
    public void DoList()
    {
        if (init)
        {
            EditorGUILayout.Space();
            ctor.GetType().GetMethod("DoList").Invoke(ctor, new object[0]);
        }
        init = true;
    }
    #endregion Methods

    #region Refrection ...

    bool init;
    object ctor;
    string name;
    DrawElementCallback _drawElementCallback;
    DrawHeaderCallback _drawHeaderCallback;
    AddCallbackDelegate _onAddDelegateCallback;
    RemoveCallbackDelegate _onRemoveDelegateCallback;
    SelectCallbackDelegate _onSelectCallback;
    AddDropdownCallbackDelegate _onAddDropdownCallback;
    Type type
    {
        get
        {
            return Types.GetType("UnityEditor.ReorderableList", "UnityEditor.dll");
        }
    }

    ConstructorInfo constructorInfo_serialized
    {
        get
        {
            return type.GetConstructor(new[]
                {
                    typeof (SerializedObject), typeof (SerializedProperty), typeof (bool), typeof (bool), typeof (bool),
                    typeof (bool)
                });
        }
    }
    ConstructorInfo constructorInfo
    {
        get
        {
            return type.GetConstructor(new[]
                {
                    typeof (IList), typeof (Type), typeof (bool), typeof (bool), typeof (bool),
                    typeof (bool)
                });
        }
    }

    int index
    {
        get
        {
            return (int)ctor.GetType().GetProperty("index").GetValue(ctor, new object[0]);
        }
    }


    void InitdefaultCallback()
    {
        drawElementCallback = (rect, index, selected, focused) => { };
        drawHeaderCallback = rect => { };
        onAddDelegateCallback = () => { };
        onRemoveDelegateCallback = () => { };
        onSelectCallback = index => { };
    }

    void ElementCallback(Rect rect, int index, bool selected, bool focused)
    {
        serializedObject.Update();
        if (drawElementCallback.GetInvocationList().Length > 1)
            drawElementCallback(rect, index, selected, focused);
        else
        {
            SerializedProperty property = serializedProperty.GetArrayElementAtIndex(index);

            float propertyHeight = EditorGUI.GetPropertyHeight(serializedProperty.GetArrayElementAtIndex(index));
            rect.height = propertyHeight;
            if (elementHeight > propertyHeight)
                rect.y += (elementHeight - propertyHeight) * 0.5f;
            else
            {
                elementHeight = propertyHeight;
            }

            EditorGUI.PropertyField(rect, property, GUIContent.none);
        }

        serializedObject.ApplyModifiedProperties();
    }

    void HeaderCallback(Rect rect)
    {
        if (drawHeaderCallback.GetInvocationList().Length > 1)
            drawHeaderCallback(rect);
        else
            GUI.Label(rect, name);
    }

    void AddCallback(object obj)
    {
        serializedObject.Update();
        if (onAddDelegateCallback.GetInvocationList().Length > 1)
            onAddDelegateCallback();
        else
        {
            serializedProperty.InsertArrayElementAtIndex(index);
        }

        serializedObject.ApplyModifiedProperties();
    }

    void RemoveCallback(object obj)
    {
        serializedObject.Update();
        if (onRemoveDelegateCallback.GetInvocationList().Length > 1)
            onRemoveDelegateCallback();
        else
            serializedProperty.DeleteArrayElementAtIndex(index);
        serializedObject.ApplyModifiedProperties();
    }

    void SelectCallback(object obj)
    {
        onSelectCallback(index);
    }

    void AddDropdownCallback(Rect rect, object obj)
    {
        onAddDropdownCallback(rect);
    }

    void SetDelegate(string typePath, string delegateName, string callbackName)
    {
        FieldInfo fieldInfo = ctor.GetType()
            .GetField(delegateName, BindingFlags.Public | BindingFlags.Instance);
        Type deleg = Types.GetType("UnityEditor.ReorderableList+" + typePath, "UnityEditor.dll");
        Delegate @delegate = Delegate.CreateDelegate(deleg, this,
            GetType().GetMethod(callbackName, BindingFlags.NonPublic | BindingFlags.Instance));
        fieldInfo.SetValue(ctor, @delegate);
    }
    #endregion Refrection ...
    #region Delegates

    public delegate void DrawElementCallback(Rect rect, int index, bool selected, bool focused);

    public delegate void DrawHeaderCallback(Rect rect);

    public delegate void SelectCallbackDelegate(int index);

    public delegate void AddCallbackDelegate();
    
    public delegate void RemoveCallbackDelegate();
    
    public delegate void AddDropdownCallbackDelegate(Rect buttonRect);

    #endregion Delegates
}
