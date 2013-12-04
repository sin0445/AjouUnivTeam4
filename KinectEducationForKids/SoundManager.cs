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
using WMPLib;


namespace KinectEducationForKids
{
    public class SoundManager
    {
        #region MemberVariables
        private SoundPlayer _EffectPlayer;
        private WindowsMediaPlayer _BackgroundPlayer;
        private SoundPlayer _TtsPlayer;
        private SpeechSynthesizer _Tts;
        #endregion MemberVariables

        #region Constructor
        public SoundManager()
        {
            this._EffectPlayer = new SoundPlayer();
            this._BackgroundPlayer = new WindowsMediaPlayer();
            this._TtsPlayer = new SoundPlayer();

            this._EffectPlayer.LoadCompleted += new AsyncCompletedEventHandler(PlayerStreamIsLoaded);
            this._BackgroundPlayer.settings.setMode("loop", true);

            //TTS 기본 설정하는 부분 추가
            this._Tts = new SpeechSynthesizer();
            this._Tts.SelectVoice("Microsoft Server Speech Text to Speech Voice (ko-KR, Heami)");
            this._Tts.TtsVolume = 100;
        }

        public void PlayAudio(AudioList.Lists a)
        {
            switch (a)
            {
                case AudioList.Lists.종료하기:
                    LoadAudio(this._TtsPlayer, this._Tts, "종료하기");
                    break;
                case AudioList.Lists.한글쓰기:
                    LoadAudio(this._TtsPlayer, this._Tts, "한글쓰기");
                    break;
                case AudioList.Lists.한글퀴즈:
                    LoadAudio(this._TtsPlayer, this._Tts, "한글퀴즈");
                    break;
                case AudioList.Lists.자음쓰기:
                    LoadAudio(this._TtsPlayer, this._Tts, "자음쓰기");
                    break;
                case AudioList.Lists.모음쓰기:
                    LoadAudio(this._TtsPlayer, this._Tts, "모음쓰기");
                    break;
                case AudioList.Lists.뒤로가기:
                    LoadAudio(this._TtsPlayer, this._Tts, "뒤로가기");
                    break;
                case AudioList.Lists.과일퀴즈:
                    LoadAudio(this._TtsPlayer, this._Tts, "과일퀴즈");
                    break;
                case AudioList.Lists.동물퀴즈:
                    LoadAudio(this._TtsPlayer, this._Tts, "동물퀴즈");
                    break;
                case AudioList.Lists.이전문제:
                    LoadAudio(this._TtsPlayer, this._Tts, "이전문제");
                    break;
                case AudioList.Lists.다음문제:
                    LoadAudio(this._TtsPlayer, this._Tts, "다음문제");
                    break;
                case AudioList.Lists.물방울효과:
                    LoadAudio(this._EffectPlayer, Properties.Resources.hand_over);
                    break;
                case AudioList.Lists.메인배경:
                    LoadAudio(this._BackgroundPlayer, "bgm_main.mp3");
                    break;
                case AudioList.Lists.따라쓰기배경:
                    LoadAudio(this._BackgroundPlayer, "bgm_draw.mp3");
                    break;
                case AudioList.Lists.퀴즈배경:
                    LoadAudio(this._BackgroundPlayer, "bgm_quiz.mp3");
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

        private void LoadAudio(SoundPlayer player, SpeechSynthesizer tts, string ttsSentence)
        {
            MemoryStream stream = new MemoryStream();
            tts.SetOutputToWaveStream(stream);
            tts.Speak(new Prompt(ttsSentence));
            stream.Position = 0;
            player.Stream = stream;
            player.Play();
            tts.SetOutputToNull();
        }

        private void LoadAudio(WindowsMediaPlayer player, string soundName)
        {
            string fullPathUrl = Environment.CurrentDirectory + "\\Media\\" + soundName;

            if (player.playState == WMPPlayState.wmppsPlaying)
                player.controls.stop();
            player.URL = fullPathUrl;
            player.controls.play();
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
            this._BackgroundPlayer.controls.stop();
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
            player.Play();
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
            다음문제,
            물방울효과 = 41,
            메인배경 = 71,
            따라쓰기배경,
            퀴즈배경
        };
    }
    #endregion AudioList
}

