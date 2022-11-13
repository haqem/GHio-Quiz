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

using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable()]
public struct Answer {
   [SerializeField] private string _info;
   public string Info {
      get {
         return _info;
      }
   }

   [SerializeField] private bool _isCorrect;
   public bool IsCorrect {
      get {
         return _isCorrect;
      }
   }
}
[CreateAssetMenu(fileName = "New Question", menuName = "Quiz/new Question")]
public class Question: ScriptableObject {

   public enum AnswerType {
      Multi,
      Single
   }

   [SerializeField] private String _info = String.Empty;
   public String Info {
      get {
         return _info;
      }
   }

   [SerializeField] Answer[] _answers = null;
   public Answer[] Answers {
      get {
         return _answers;
      }
   }

   //Parameters

   [SerializeField] private bool _useTimer = false;
   public bool UseTimer {
      get {
         return _useTimer;
      }
   }

   [SerializeField] private int _timer = 0;
   public int Timer {
      get {
         return _timer;
      }
   }

   [SerializeField] private AnswerType _answerType = AnswerType.Multi;
   public AnswerType GetAnswerType {
      get {
         return _answerType;
      }
   }

   [SerializeField] private int _addScore = 10;
   public int AddScore {
      get {
         return _addScore;
      }
   }

   public List < int > GetCorrectAnswers() {
      List < int > CorrectAnswers = new List < int > ();
      for (int i = 0; i < Answers.Length; i++) {
         if (Answers[i].IsCorrect) {
            CorrectAnswers.Add(i);
         }
      }
      return CorrectAnswers;
   }
}