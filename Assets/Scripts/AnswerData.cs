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

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnswerData: MonoBehaviour {


      [Header("UI Elements")]
      [SerializeField] TextMeshProUGUI infoTextObject = null;
   [SerializeField] Image toggle = null;

   [Header("Textures")]
   [SerializeField] Sprite uncheckedToggle = null;
   [SerializeField] Sprite checkedToggle = null;

   [Header("References")]
   [SerializeField] GameEvents events = null;

   private RectTransform _rect = null;
   public RectTransform Rect {
      get {
         if (_rect == null) {
            _rect = GetComponent < RectTransform > () ?? gameObject.AddComponent < RectTransform > ();
         }
         return _rect;
      }
   }

   private int _answerIndex = -1;
   public int AnswerIndex {
      get {
         return _answerIndex;
      }
   }

   private bool Checked = false;

   public void UpdateData(string info, int index) {
      infoTextObject.text = info;
      _answerIndex = index;
   }

   public void Reset() {
      Checked = false;
      UpdateUI();
   }

   public void SwitchState() {
      Checked = !Checked;
      UpdateUI();

      if (events.UpdateQuestionAnswer != null) {
         events.UpdateQuestionAnswer(this);
      }
   }

   void UpdateUI() {
      if (toggle == null) return;
      toggle.sprite = (Checked) ? checkedToggle : uncheckedToggle;
   }
}