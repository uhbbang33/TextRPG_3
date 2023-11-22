using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace TextRPG
{
    internal class GameManager
    {
        private static GameManager _instance;
        public static GameManager Instance 
        {
            get 
            {
                if (_instance == null) new GameManager();                
                return _instance; 
            }
        }
        
        public bool isPlay = false;

        Player _player;
        public Player Player { get { return _player; } }
        
        Scene _currentScene;

        QuestList _questList;
        public QuestList QuestList { get { return _questList; } }

        private GameManager()
        {
            _instance = this;

            _questList = new QuestList();
            _player = new Player();
            _currentScene = new TitleScene();
        }

        public void RunGame()
        {
            ChangeScene(_currentScene);
            isPlay = true;

            SoundInit();
        }

        public void GetInput(ConsoleKey key)
        {
            _currentScene.HandleInput(this, key);
        }

        public void ChangeScene(Scene scene)
        {
            Loader.Save(_player);
            _currentScene = scene;
            RefreshScene();
        }

        public void RefreshScene()
        {
            _currentScene.Update(this);
            _currentScene.DrawScene();
        }

        void SoundInit()
        {
            AudioFileReader audioFile = new AudioFileReader(@"..\..\..\Music\strangerThings.wav");
            WaveOutEvent waveOut = new WaveOutEvent();
            VolumeSampleProvider v = new VolumeSampleProvider(audioFile.ToSampleProvider());
            v.Volume = 0.15f;

            // Music Loop
            waveOut.PlaybackStopped += (sender, args) =>
            {
                if (waveOut.PlaybackState != PlaybackState.Stopped)
                    return;

                audioFile.Position = 0;
                waveOut.Play();
            };

            waveOut.Init(v);
            waveOut.Play();
        }
    }
}
