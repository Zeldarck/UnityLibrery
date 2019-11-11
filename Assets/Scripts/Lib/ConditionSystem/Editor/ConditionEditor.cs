using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;
using System;

[CustomPropertyDrawer(typeof(Condition))]
public class ConditionEditor : PropertyDrawer
{

    float m_currentHeight = 0;

    private void GenerateStyle(Rect position)
    {
        Color color = GUI.color;

        Color faded = new Color(0, 0, 0);
        faded.a = 0.18f;
        GUI.color = faded;
        GUI.Box(position, "");

        GUI.color = new Color(0, 0, 0);
        Rect bottomline = new Rect(position.x, position.y + position.height, position.width, 2);
        GUI.Box(bottomline, "");

        GUI.color = color;
    }


    private SerializedProperty CreatePropertyField(Rect a_position, SerializedProperty a_property, GUIContent a_label, string a_subProperty)
    {
        SerializedProperty subProperty = a_property.FindPropertyRelative(a_subProperty);

        return CreatePropertyField(a_position, subProperty, a_label);
    }


    private SerializedProperty CreatePropertyField(Rect a_position, SerializedProperty a_property, GUIContent a_label)
    {
        Rect rect = GetNextRect(a_position, a_property, a_label);

        EditorGUI.PropertyField(rect, a_property, true);

        return a_property;
    }


    private SerializedProperty CreatePopUp(Rect a_position, SerializedProperty a_property, GUIContent a_label, string a_subProperty, List<string> a_choices, Func<SerializedProperty, string> a_stringGetter, out int out_newIndex)
    {
        SerializedProperty subProperty = a_property.FindPropertyRelative(a_subProperty);
      
        return CreatePopUp(a_position, subProperty, a_label, a_choices, a_stringGetter, out out_newIndex);
    }


    private SerializedProperty CreatePopUp(Rect a_position, SerializedObject a_object, GUIContent a_label, string a_subProperty, List<string> a_choices, Func<SerializedProperty, string> a_stringGetter, out int out_newIndex)
    {
        SerializedProperty subProperty = a_object.FindProperty(a_subProperty);

        return CreatePopUp(a_position, subProperty, a_label, a_choices, a_stringGetter, out out_newIndex);
    }


    private SerializedProperty CreatePopUp(Rect a_position, SerializedProperty a_property, GUIContent a_label, List<string> a_choices, Func<SerializedProperty, string> a_stringGetter, out int out_newIndex)
    {
        int currIndex = -1;
        if (a_property != null)
        {
            for (int i = 0; i < a_choices.Count; ++i)
            {
                if (a_choices[i] == a_stringGetter(a_property))
                {
                    currIndex = i;
                    break;
                }
            }
        }


        Rect rect = GetNextRect(a_position, a_property, a_label);
        out_newIndex = -1;
        out_newIndex = EditorGUI.Popup(rect, a_property.displayName, currIndex, a_choices.ToArray());


        return a_property;
    }




    private Rect GetNextRect(Rect a_position, SerializedProperty a_property, GUIContent a_label)
    {
        Rect rect = new Rect(a_position.x, a_position.y + m_currentHeight + EditorGUIUtility.standardVerticalSpacing, a_position.width, EditorGUI.GetPropertyHeight(a_property, a_label, true));

        m_currentHeight += EditorGUI.GetPropertyHeight(a_property, a_label, true) + EditorGUIUtility.standardVerticalSpacing;

        return rect;
    }





    private SerializedObject GeneratePropetiesFields(SerializedProperty a_property, Rect a_position, GUIContent a_label)
    {
        SerializedObject serializedObj = new UnityEditor.SerializedObject(a_property.objectReferenceValue);


        SerializedProperty property = serializedObj.GetIterator();


        while (property.NextVisible(true))
        {
            Rect rect = GetNextRect(a_position,property,a_label);

            EditorGUI.PropertyField(rect, property, true);
        }


        serializedObj.ApplyModifiedProperties();

        return serializedObj;
    }



    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        m_currentHeight = 0;

        GenerateStyle(position);


        label = EditorGUI.BeginProperty(position, label, property);
        int originalIndent = EditorGUI.indentLevel;



        SerializedProperty isList = CreatePropertyField(position, property, label, "m_isList");
        CreatePropertyField(position, property, label, "m_isNot");


        if (isList.boolValue)
        {
            CreatePropertyField(position, property, label, "m_isOr");
            CreatePropertyField(position, property, label, "m_conditions");
        }
        else
        {
            DisplayCondition(position, property, label);
        }
        EditorGUI.indentLevel = originalIndent;

        EditorGUI.EndProperty();




    }




    private void DisplayCondition(Rect a_position, SerializedProperty a_property, GUIContent a_label)
    {

        List<string> allConditionCommandTypeStrings = new List<string>();
        List<System.Type> allConditionCommandType = typeof(ConditionCommand).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(ConditionCommand))).ToList();

        foreach (System.Type type in allConditionCommandType)
        {
            allConditionCommandTypeStrings.Add(type.Name);
        }


        //Create PopUp To choice Class
        EditorGUI.BeginChangeCheck();

        int newIndex = 0;

        SerializedProperty commandProperty = CreatePopUp(a_position, a_property, a_label, "m_conditionCommand", allConditionCommandTypeStrings, (o) => { return (o.objectReferenceValue != null ? o.objectReferenceValue.GetType().Name : null);} , out newIndex);

        if (EditorGUI.EndChangeCheck())
        {
            commandProperty.objectReferenceValue = ScriptableObject.CreateInstance(allConditionCommandType[newIndex]);
        }




        //If Class choosen,display fields and methods popUp
        if (commandProperty.objectReferenceValue != null)
        {
            EditorGUI.indentLevel += 2;


            SerializedObject commandObject = GeneratePropetiesFields(commandProperty, a_position, a_label);



            List<string> methodsNames = new List<string>();
            MethodInfo[] methodInfos = allConditionCommandType[newIndex].GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            for (int j = 0; j < methodInfos.Length; ++j)
            {
                methodsNames.Add(methodInfos[j].Name);
            }

            EditorGUI.BeginChangeCheck();

            SerializedProperty methodProperty = CreatePopUp(a_position, commandObject, a_label, "m_method", methodsNames, (o) => { return o.stringValue; }, out newIndex);

            if (EditorGUI.EndChangeCheck())
            {
                 methodProperty.stringValue = methodsNames[newIndex];
                 commandObject.ApplyModifiedProperties();
            }



            //Parametters of the method
            if (newIndex >= 0)
            {
                EditorGUI.indentLevel+=2;

                MethodInfo method = methodInfos[newIndex];

                for (int i = 0; i < method.GetParameters().Length; i++)
                {
                    ParameterInfo parametter = method.GetParameters()[i];

                    SerializedProperty paramProperty = commandObject.GetIterator();

                    while (paramProperty.Next(true))
                    {
                        if (paramProperty.name == parametter.Name)
                        {
                            break;
                        }
                    }
                    CreatePropertyField(a_position, paramProperty, a_label);
                }
            }

            commandObject.ApplyModifiedProperties();

        }




    }


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float totalHeight = 0;
        SerializedProperty isList = property.FindPropertyRelative("m_isList");

        totalHeight += EditorGUI.GetPropertyHeight(property, label, true) + EditorGUIUtility.standardVerticalSpacing;
        totalHeight += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_isList"), label, true) + EditorGUIUtility.standardVerticalSpacing;
        totalHeight += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_isNot"), label, true) + EditorGUIUtility.standardVerticalSpacing;

        if (!isList.boolValue)
        {
            UnityEngine.Object command = property.FindPropertyRelative("m_conditionCommand").objectReferenceValue;
            if (command != null)
            {

                SerializedObject commandObject = new UnityEditor.SerializedObject(command as ConditionCommand);

                SerializedProperty propertyIterator = commandObject.GetIterator();

                while (propertyIterator.NextVisible(true))
                {
                    totalHeight += EditorGUI.GetPropertyHeight(propertyIterator, label, true) + EditorGUIUtility.standardVerticalSpacing;
                }


                SerializedProperty methodProperty = commandObject.FindProperty("m_method");

                totalHeight += EditorGUI.GetPropertyHeight(methodProperty, label, true) + EditorGUIUtility.standardVerticalSpacing;

                MethodInfo methodInfo = (command as ConditionCommand).GetType().GetMethod(methodProperty.stringValue);
                if (methodInfo != null)
                {
                    for (int i = 0; i < methodInfo.GetParameters().Length; i++)
                    {
                        ParameterInfo parametter = methodInfo.GetParameters()[i];

                        propertyIterator = commandObject.GetIterator();

                        while (propertyIterator.Next(true))
                        {
                            if (propertyIterator.name == parametter.Name)
                            {
                                totalHeight += EditorGUI.GetPropertyHeight(propertyIterator, label, true) + EditorGUIUtility.standardVerticalSpacing;
                                break;
                            }

                        }
                    }
                }


            }
        }
        else
        {
            totalHeight += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_isOr"), label, true) + EditorGUIUtility.standardVerticalSpacing;
            totalHeight += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_conditions"), label, true) + EditorGUIUtility.standardVerticalSpacing;
        }


        totalHeight += EditorGUIUtility.standardVerticalSpacing * 3;

        return totalHeight;
    }




}
