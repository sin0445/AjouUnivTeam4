using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Media;
using System.IO;
using System.ComponentModel;
using Microsoft.Speech;
using Microsoft.Speech.Synthesis;
using Microsoft.Speech.AudioFormat;

namespace KinectEducationForKids
{
    public class SoundManager
    {
        #region MemberVariables
        private SoundPlayer _EffectPlayer;
        private SoundPlayer _BackgroundPlayer;
        private SoundPlayer _TtsPlayer;
        private SpeechSynthesizer _Tts;
        #endregion MemberVariables

        #region Constructor
        public SoundManager()
        {
            this._EffectPlayer = new SoundPlayer();
            this._BackgroundPlayer = new SoundPlayer();
            this._TtsPlayer = new SoundPlayer();

            this._EffectPlayer.LoadCompleted += new AsyncCompletedEventHandler(PlayerStreamIsLoaded);
            this._BackgroundPlayer.LoadCompleted += new AsyncCompletedEventHandler(PlayerStreamIsLoaded);
            
            //TTS 기본 설정하는 부분 추가
            this._Tts = new SpeechSynthesizer();
            this._Tts.SelectVoice("Microsoft Server Speech Text to Speech Voice (ko-KR, Heami)");
        }

        public void PlayAudio(AudioList.Lists a)
        {
            switch (a)
            {
                case AudioList.Lists.종료하기:
                    LoadAudio("종료하기");
                    break;
                case AudioList.Lists.한글쓰기:
                    LoadAudio("한글쓰기");
                    break;
                case AudioList.Lists.한글퀴즈:
                    LoadAudio("한글퀴즈");
                    break;
                case AudioList.Lists.자음쓰기:
                    LoadAudio("자음쓰기");
                    break;
                case AudioList.Lists.모음쓰기:
                    LoadAudio("모음쓰기");
                    break;
                case AudioList.Lists.뒤로가기:
                    LoadAudio("뒤로가기");
                    break;
                case AudioList.Lists.과일퀴즈:
                    LoadAudio("과일퀴즈");
                    break;
                case AudioList.Lists.동물퀴즈:
                    LoadAudio("동물퀴즈");
                    break;
                case AudioList.Lists.이전문제:
                    LoadAudio("이전문제");
                    break;
                case AudioList.Lists.다음문제:
                    LoadAudio("다음문제");
                    break;
                default:
                    break;
            }
        }
        #endregion Constructor

        #region CoreMethods
        /*
         * LoadAudio()
         * 파라메터로 Stream 받아와서 SoundPlayer에 AsyncLoad 시킴
         */
        private void LoadAudio(SoundPlayer player, Stream stream)
        {
            if (player != null && stream != null)
            {
                player.Stream = stream;
                PauseAudio(player);
                player.LoadAsync();
            }
        }

        private void LoadAudio(string ttsSentence)
        {
            MemoryStream stream = new MemoryStream();
            this._Tts.SetOutputToWaveStream(stream);
            this._Tts.Speak(new Prompt(ttsSentence));
            stream.Position = 0;
            this._TtsPlayer.Stream = stream;
            this._TtsPlayer.Play();
            this._Tts.SetOutputToNull();
        }

        private void PauseAudio(SoundPlayer player)
        {
            player.Stop();
        }

        public void PauseEffect()
        {
            PauseAudio(this._EffectPlayer);
        }

        public void PauseBackground()
        {
            PauseAudio(this._BackgroundPlayer);
        }

        public void PauseTTS()
        {
            if (this._Tts.State == SynthesizerState.Speaking)
            {
                this._Tts.SpeakAsyncCancelAll();
                this._Tts.Pause();
            }
        }
        #endregion CoreMethods

        #region HelperMethods
        #endregion HelperMethods

        #region EventHandler
        private void PlayerStreamIsLoaded(object sender, AsyncCompletedEventArgs e)
        {
            SoundPlayer player = (SoundPlayer)sender;

            if (sender.Equals(this._BackgroundPlayer))
            {
                player.PlayLooping();
            }
            else
            {
                player.Play();
            }
        }
        #endregion EventHandler
    }


    #region AudioList
    public static class AudioList
    {
        public enum Lists
        {
            /*
             * TTS 부분 1~40
             * 효과음 41~70
             * 배경음 71~80
             */
            한글쓰기 = 1,
            한글퀴즈,
            종료하기,
            자음쓰기,
            모음쓰기,
            뒤로가기,
            과일퀴즈,
            동물퀴즈,
            이전문제,
            다음문제
        };
    }
    #endregion AudioList
}

