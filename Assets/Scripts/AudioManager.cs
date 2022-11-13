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
using UnityEngine;

[System.Serializable()]
public struct SoundParameters {
   [Range(0, 1)]
   public float Volume;
   [Range(-3, 3)]
   public float Pitch;
   public bool Loop;
}
[System.Serializable()]
public class Sound {

      [SerializeField] String name = String.Empty;
   public String Name {
      get {
         return name;
      }
   }

   [SerializeField] AudioClip clip = null;
   public AudioClip Clip {
      get {
         return clip;
      }
   }

   [SerializeField] SoundParameters parameters = new SoundParameters();
   public SoundParameters Parameters {
      get {
         return parameters;
      }
   }

   [HideInInspector]
   public AudioSource Source = null;


   public void Play() {
      Source.clip = Clip;

      Source.volume = Parameters.Volume;
      Source.pitch = Parameters.Pitch;
      Source.loop = Parameters.Loop;

      Source.Play();
   }
   public void Stop() {
      Source.Stop();
   }
}
public class AudioManager: MonoBehaviour {

   public static AudioManager Instance = null;

   [SerializeField] Sound[] sounds = null;
   [SerializeField] AudioSource sourcePrefab = null;

   [SerializeField] String startupTrack = String.Empty;

   void Awake() {
      if (Instance != null) {
         Destroy(gameObject);
      } else {
         Instance = this;
         DontDestroyOnLoad(gameObject);
      }
      InitSounds();
   }

   void Start() {
      if (string.IsNullOrEmpty(startupTrack) != true) {
         PlaySound(startupTrack);
      }
   }

   void InitSounds() {
      foreach(var sound in sounds) {
         AudioSource source = (AudioSource) Instantiate(sourcePrefab, gameObject.transform);
         source.name = sound.Name;

         sound.Source = source;
      }
   }

   public void PlaySound(string name) {
      var sound = GetSound(name);
      if (sound != null) {
         sound.Play();
      } else {
         Debug.LogWarning("Sound by the name " + name + " is not found! Issues occured at AudioManager.PlaySound()");
      }
   }

   public void StopSound(string name) {
      var sound = GetSound(name);
      if (sound != null) {
         sound.Stop();
      } else {
         Debug.LogWarning("Sound by the name " + name + " is not found! Issues occured at AudioManager.StopSound()");
      }
   }

   Sound GetSound(string name) {
      foreach(var sound in sounds) {
         if (sound.Name == name) {
            return sound;
         }
      }
      return null;
   }

}