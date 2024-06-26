﻿/****
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

using UnityEngine;

[CreateAssetMenu(fileName = "GameEvents", menuName = "Quiz/new GameEvents")]
public class GameEvents: ScriptableObject {

   public delegate void UpdateQuestionUICallback(Question question);
   public UpdateQuestionUICallback UpdateQuestionUI = null;

   public delegate void UpdateQuestionAnswerCallback(AnswerData pickedAnswer);
   public UpdateQuestionAnswerCallback UpdateQuestionAnswer = null;

   public delegate void DisplayResolutionScreenCallback(UIManager.ResolutionScreenType type, int score);
   public DisplayResolutionScreenCallback DisplayResolutionScreen = null;

   public delegate void ScoreUpdatedCallback();
   public ScoreUpdatedCallback ScoreUpdated = null;

   [HideInInspector]
   public int CurrentFinalScore = 0;
   [HideInInspector]
   public int StartupHighscore = 0;
}