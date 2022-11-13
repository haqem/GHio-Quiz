/****
Resources: https://github.com/haqem/GHio-game
YTD: 11/2022

MIT License

Copyright (c) 2022 Ahmad Luqman Haqem

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
****/

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof (Question))]
public class Question_Editor: Editor {

   #region Variables

   #region Serialized Properties
   SerializedProperty questionInfoProp = null;
   SerializedProperty answersProp = null;
   SerializedProperty useTimerProp = null;
   SerializedProperty timerProp = null;
   SerializedProperty answerTypeProp = null;
   SerializedProperty addScoreProp = null;

   SerializedProperty arraySizeProp {
      get {
         return answersProp.FindPropertyRelative("Array.size");
      }
   }
   #endregion

   private bool showParameters = false;

   #endregion

   #region Default Unity methods

   void OnEnable() {
      #region Fetch Properties
      questionInfoProp = serializedObject.FindProperty("_info");
      answersProp = serializedObject.FindProperty("_answers");
      useTimerProp = serializedObject.FindProperty("_useTimer");
      timerProp = serializedObject.FindProperty("_timer");
      answerTypeProp = serializedObject.FindProperty("_answerType");
      addScoreProp = serializedObject.FindProperty("_addScore");
      #endregion

      #region Get Values
      showParameters = EditorPrefs.GetBool("Question_showParameters_State");
      #endregion
   }

   public override void OnInspectorGUI() {
      GUILayout.Label("Question", EditorStyles.miniLabel);
      GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea) {
         fontSize = 15,
            fixedHeight = 30,
            alignment = TextAnchor.MiddleLeft
      };
      questionInfoProp.stringValue = EditorGUILayout.TextArea(questionInfoProp.stringValue, textAreaStyle);
      GUILayout.Space(7.5f);

      GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout) {
         fontSize = 10
      };
      EditorGUI.BeginChangeCheck();
      showParameters = EditorGUILayout.Foldout(showParameters, "Parameters", foldoutStyle);
      if (EditorGUI.EndChangeCheck()) {
         EditorPrefs.SetBool("Question_showParameters_State", showParameters);
      }
      if (showParameters) {
         EditorGUILayout.PropertyField(useTimerProp, new GUIContent("Use Timer", "Should this question have a timer?"));
         if (useTimerProp.boolValue) {
            timerProp.intValue = EditorGUILayout.IntSlider(new GUIContent("Time"), timerProp.intValue, 1, 120);
         }
         GUILayout.Space(2);
         EditorGUI.BeginChangeCheck();
         EditorGUILayout.PropertyField(answerTypeProp, new GUIContent("Answer Type", "Specify this question answer type."));
         if (EditorGUI.EndChangeCheck()) {
            if (answerTypeProp.enumValueIndex == (int) Question.AnswerType.Single) {
               if (GetCorrectAnswersCount() > 1) {
                  UncheckCorrectAnswers();
               }
            }
         }
         addScoreProp.intValue = EditorGUILayout.IntSlider(new GUIContent("Add Score"), addScoreProp.intValue, 0, 100);
      }
      GUILayout.Space(7.5f);
      GUILayout.Label("Answers", EditorStyles.miniLabel);
      DrawAnswers();

      serializedObject.ApplyModifiedProperties();
   }

   #endregion

   void DrawAnswers() {
      EditorGUILayout.BeginVertical();

      EditorGUILayout.PropertyField(arraySizeProp);
      GUILayout.Space(5);

      EditorGUI.indentLevel++;
      for (int i = 0; i < arraySizeProp.intValue; i++) {
         EditorGUI.BeginChangeCheck();
         EditorGUILayout.PropertyField(answersProp.GetArrayElementAtIndex(i));
         if (EditorGUI.EndChangeCheck()) {
            if (answerTypeProp.enumValueIndex == (int) Question.AnswerType.Single) {
               SerializedProperty isCorrectProp = answersProp.GetArrayElementAtIndex(i).FindPropertyRelative("_isCorrect");

               if (isCorrectProp.boolValue) {
                  UncheckCorrectAnswers();
                  answersProp.GetArrayElementAtIndex(i).FindPropertyRelative("_isCorrect").boolValue = true;

                  serializedObject.ApplyModifiedProperties();
               }
            }
         }
         GUILayout.Space(5);
      }

      EditorGUILayout.EndVertical();
      EditorGUI.indentLevel--;
   }

   void UncheckCorrectAnswers() {
      for (int i = 0; i < arraySizeProp.intValue; i++) {
         answersProp.GetArrayElementAtIndex(i).FindPropertyRelative("_isCorrect").boolValue = false;
      }
   }

   int GetCorrectAnswersCount() {
      int count = 0;
      for (int i = 0; i < arraySizeProp.intValue; i++) {
         if (answersProp.GetArrayElementAtIndex(i).FindPropertyRelative("_isCorrect").boolValue) {
            count++;
         }
      }
      return count;
   }
}