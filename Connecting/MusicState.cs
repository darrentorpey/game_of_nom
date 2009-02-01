using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;

namespace Connecting
{
    public class MusicState
    {
        private const float c_fCrossFadeRate = 0.2f;

        private static MusicState s_Instance = new MusicState();

        private static SoundEffect[] s_Music;

        private SoundEffectInstance[] _PlayingSongs;
        private int _iActiveSong = 0;

        public int ActiveSong
        {
            get { return _iActiveSong; }
            set { _iActiveSong = value; }
        }

        public static MusicState Instance
        {
            get { return s_Instance; }
        }

        private MusicState()
        {
            
        }

        public void Play()
        {
            if (_PlayingSongs == null)
            {
                _PlayingSongs = new SoundEffectInstance[2];
                _PlayingSongs[0] = s_Music[0].Play(1.0f, 0.0f, 0.0f, true);
                _PlayingSongs[1] = s_Music[1].Play(0.0f, 0.0f, 0.0f, true);
            }
            else
            {
                _PlayingSongs[0].Resume();
                _PlayingSongs[1].Resume();
            }
        }

        public void Pause()
        {
            if (_PlayingSongs == null)
                return; // How did that happen!?
            
            _PlayingSongs[0].Pause();
            _PlayingSongs[1].Pause();
        }

        public void Stop()
        {
            if (_PlayingSongs == null)
                return; // How did that happen!?

            _PlayingSongs[0].Stop();
            _PlayingSongs[1].Stop();

        }

        public void Restart()
        {
            if (_PlayingSongs == null)
                return; // How did that happen!?
            
            _PlayingSongs[0].Play();
            _PlayingSongs[1].Play();
        }

        public void Update(GameTime aTime)
        {
            float fFrameFade = c_fCrossFadeRate * (float)aTime.ElapsedGameTime.TotalSeconds;
            for (int i = 0; i < 2; ++i)
            {
                if (i == _iActiveSong && _PlayingSongs[i].Volume < 1.0f)
                    _PlayingSongs[i].Volume += fFrameFade;
                else if (_PlayingSongs[i].Volume > 0.0f)
                    _PlayingSongs[i].Volume -= fFrameFade;
            }
        }

        public static void LoadContent(ContentManager aContent)
        {
            s_Music = new SoundEffect[] {
                aContent.Load<SoundEffect>("music/Tea Party Theme A"),
                aContent.Load<SoundEffect>("music/Tea Party Theme B")
            };
        }
    }
}
