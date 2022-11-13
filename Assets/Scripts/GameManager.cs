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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager: MonoBehaviour {

   private Question[] _questions = null;
   public Question[] Questions {
      get {
         return _questions;
      }
   }

   [SerializeField] GameEvents events = null;

   [SerializeField] Animator timerAnimtor = null;
   [SerializeField] TextMeshProUGUI timerText = null;
   [SerializeField] Color timerHalfWayOutColor = Color.yellow;
   [SerializeField] Color timerAlmostOutColor = Color.red;
   private Color timerDefaultColor = Color.white;

   private List < AnswerData > PickedAnswers = new List < AnswerData > ();
   private List < int > FinishedQuestions = new List < int > ();
   private int currentQuestion = 0;

   private int timerStateParaHash = 0;

   private IEnumerator IE_WaitTillNextRound = null;
   private IEnumerator IE_StartTimer = null;

   private bool IsFinished {
      get {
         return (FinishedQuestions.Count < Questions.Length) ? false : true;
      }
   }

   void OnEnable() {
      events.UpdateQuestionAnswer += UpdateAnswers;
   }

   void OnDisable() {
      events.UpdateQuestionAnswer -= UpdateAnswers;
   }

   void Awake() {
      events.CurrentFinalScore = 0;
   }

   void Start() {
      events.StartupHighscore = PlayerPrefs.GetInt(GameUtility.SavePrefKey);

      timerDefaultColor = timerText.color;
      LoadQuestions();

      timerStateParaHash = Animator.StringToHash("TimerState");

      var seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
      UnityEngine.Random.InitState(seed);

      Display();
   }

   public void UpdateAnswers(AnswerData newAnswer) {
      if (Questions[currentQuestion].GetAnswerType == Question.AnswerType.Single) {
         foreach(var answer in PickedAnswers) {
            if (answer != newAnswer) {
               answer.Reset();
            }
         }
         PickedAnswers.Clear();
         PickedAnswers.Add(newAnswer);
      } else {
         bool alreadyPicked = PickedAnswers.Exists(x => x == newAnswer);
         if (alreadyPicked) {
            PickedAnswers.Remove(newAnswer);
         } else {
            PickedAnswers.Add(newAnswer);
         }
      }
   }

   public void EraseAnswers() {
      PickedAnswers = new List < AnswerData > ();
   }

   void Display() {
      EraseAnswers();
      var question = GetRandomQuestion();

      if (events.UpdateQuestionUI != null) {
         events.UpdateQuestionUI(question);
      } else {
         Debug.LogWarning("Ups! Something went wrong while trying to display new Question UI Data. GameEvents.UpdateQuestionUI is null. Issue occured in GameManager.Display() method.");
      }

      if (question.UseTimer) {
         UpdateTimer(question.UseTimer);
      }
   }

   public void Accept() {
      UpdateTimer(false);
      bool isCorrect = CheckAnswers();
      FinishedQuestions.Add(currentQuestion);

      UpdateScore((isCorrect) ? Questions[currentQuestion].AddScore : -Questions[currentQuestion].AddScore);

      if (IsFinished) {
         SetHighscore();
      }

      var type = (IsFinished) ?
         UIManager.ResolutionScreenType.Finish :
         (isCorrect) ? UIManager.ResolutionScreenType.Correct :
         UIManager.ResolutionScreenType.Incorrect;

      if (events.DisplayResolutionScreen != null) {
         events.DisplayResolutionScreen(type, Questions[currentQuestion].AddScore);
      }

      AudioManager.Instance.PlaySound((isCorrect) ? "CorrectSFX" : "IncorrectSFX");

      if (type != UIManager.ResolutionScreenType.Finish) {
         if (IE_WaitTillNextRound != null) {
            StopCoroutine(IE_WaitTillNextRound);
         }
         IE_WaitTillNextRound = WaitTillNextRound();
         StartCoroutine(IE_WaitTillNextRound);
      }
   }

   void UpdateTimer(bool state) {
      switch (state) {
      case true:
         IE_StartTimer = StartTimer();
         StartCoroutine(IE_StartTimer);

         timerAnimtor.SetInteger(timerStateParaHash, 2);
         break;
      case false:
         if (IE_StartTimer != null) {
            StopCoroutine(IE_StartTimer);
         }

         timerAnimtor.SetInteger(timerStateParaHash, 1);
         break;
      }
   }
   IEnumerator StartTimer() {
      var totalTime = Questions[currentQuestion].Timer;
      var timeLeft = totalTime;

      timerText.color = timerDefaultColor;
      while (timeLeft > 0) {
         timeLeft--;

         AudioManager.Instance.PlaySound("CountdownSFX");

         if (timeLeft < totalTime / 2 && timeLeft > totalTime / 4) {
            timerText.color = timerHalfWayOutColor;
         }
         if (timeLeft < totalTime / 4) {
            timerText.color = timerAlmostOutColor;
         }

         timerText.text = timeLeft.ToString();
         yield
         return new WaitForSeconds(1.0f);
      }
      Accept();
   }
   IEnumerator WaitTillNextRound() {
      yield
      return new WaitForSeconds(GameUtility.ResolutionDelayTime);
      Display();
   }

   bool CheckAnswers() {
      if (!CompareAnswers()) {
         return false;
      }
      return true;
   }

   bool CompareAnswers() {
      if (PickedAnswers.Count > 0) {
         List < int > c = Questions[currentQuestion].GetCorrectAnswers();
         List < int > p = PickedAnswers.Select(x => x.AnswerIndex).ToList();

         var f = c.Except(p).ToList();
         var s = p.Except(c).ToList();

         return !f.Any() && !s.Any();
      }
      return false;
   }

   void LoadQuestions() {
      Object[] objs = Resources.LoadAll("Questions", typeof (Question));
      _questions = new Question[objs.Length];
      for (int i = 0; i < objs.Length; i++) {
         _questions[i] = (Question) objs[i];
      }
   }

   public void RestartGame() {
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
   }

   public void QuitGame() {
      Application.Quit();
   }

   private void SetHighscore() {
      var highscore = PlayerPrefs.GetInt(GameUtility.SavePrefKey);
      if (highscore < events.CurrentFinalScore) {
         PlayerPrefs.SetInt(GameUtility.SavePrefKey, events.CurrentFinalScore);
      }
   }

   private void UpdateScore(int add) {
      events.CurrentFinalScore += add;

      if (events.ScoreUpdated != null) {
         events.ScoreUpdated();
      }
   }


   Question GetRandomQuestion() {
      var randomIndex = GetRandomQuestionIndex();
      currentQuestion = randomIndex;

      return Questions[currentQuestion];
   }
   int GetRandomQuestionIndex() {
      var random = 0;
      if (FinishedQuestions.Count < Questions.Length) {
         do {
            random = UnityEngine.Random.Range(0, Questions.Length);
         } while (FinishedQuestions.Contains(random) || random == currentQuestion);
      }
      return random;
   }
}