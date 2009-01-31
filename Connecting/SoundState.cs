using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;

namespace Connecting
{
    public class SoundState
    {
        private static SoundState s_Instance = new SoundState();

        private static SoundEffect[] s_AngrySounds;
        private static SoundEffect[] s_ExplodeSounds;
        private static SoundEffect[] s_NomSounds;

        private struct SoundInfo
        {
            public int _iSoundIndex;
            public int _iLastPlayTime;
        }

        private Dictionary<Person, SoundInfo> _AngrySoundMap = new Dictionary<Person,SoundInfo>();
        private Dictionary<FoodSource, SoundInfo> _NomSoundMap = new Dictionary<FoodSource, SoundInfo>();
        private SoundInfo[] _ExplodeSounds = new SoundInfo[3];

        public static SoundState Instance
        {
            get { return s_Instance; }
        }

        private SoundState()
        {

        }

        public void PlayAngrySound(Person aPerson, GameTime aTime)
        {
            // Each person goes into the map so we don't actually attempt
            // to play more than one sound of each person.
            if (!_AngrySoundMap.ContainsKey(aPerson))
            {
                _AngrySoundMap.Add(aPerson, new SoundInfo());
            }

            SoundInfo info = _AngrySoundMap[aPerson];
            if (IsSoundFinished(ref info, s_AngrySounds[info._iSoundIndex], aTime))
            {
                info._iSoundIndex = RandomInstance.Instance.Next(0, s_AngrySounds.Length);
                info._iLastPlayTime = (int)aTime.TotalGameTime.TotalMilliseconds;
                _AngrySoundMap[aPerson] = info;

                s_AngrySounds[info._iSoundIndex].Play();
            }
        }

        public void PlayExplosionSound(GameTime aTime)
        {
            // Limit the number of explosions
            for (int i = 0; i < _ExplodeSounds.Length; ++i)
            {
                if(IsSoundFinished(ref _ExplodeSounds[i], s_ExplodeSounds[_ExplodeSounds[i]._iSoundIndex], aTime))
                {
                    _ExplodeSounds[i]._iSoundIndex = RandomInstance.Instance.Next(0, s_ExplodeSounds.Length);
                    _ExplodeSounds[i]._iLastPlayTime = (int)aTime.TotalGameTime.TotalMilliseconds;
                    s_ExplodeSounds[_ExplodeSounds[i]._iSoundIndex].Play();
                    break;
                }
            }
        }

        public void PlayNomSound(FoodSource aFood, GameTime aTime)
        {
            // Each person goes into the map so we don't actually attempt
            // to play more than one sound of each person.
            if (!_NomSoundMap.ContainsKey(aFood))
            {
                _NomSoundMap.Add(aFood, new SoundInfo());
            }

            SoundInfo info = _NomSoundMap[aFood];
            if (IsSoundFinished(ref info, s_NomSounds[info._iSoundIndex], aTime))
            {
                info._iSoundIndex = RandomInstance.Instance.Next(0, s_NomSounds.Length);
                info._iLastPlayTime = (int)aTime.TotalGameTime.TotalMilliseconds;
                _NomSoundMap[aFood] = info;

                s_NomSounds[info._iSoundIndex].Play();
            }
        }

        private static bool IsSoundFinished(ref SoundInfo aInfo, SoundEffect aEffect, GameTime aTime)
        {
            if(aInfo._iLastPlayTime == 0)
                return true;
            int ifinishedTime = aInfo._iLastPlayTime + (int)aEffect.Duration.TotalMilliseconds;
            return ifinishedTime < aTime.TotalGameTime.TotalMilliseconds;
        }

        public static void LoadContent(ContentManager aContent)
        {
            s_AngrySounds = new SoundEffect[] {
                aContent.Load<SoundEffect>("sound/Angry_Blob_01"),
                aContent.Load<SoundEffect>("sound/Angry_Blob_02"),
                aContent.Load<SoundEffect>("sound/Angry_Blob_03"),
                aContent.Load<SoundEffect>("sound/Angry_Blob_04")
            };

            s_ExplodeSounds = new SoundEffect[] {
                aContent.Load<SoundEffect>("sound/Angry_Disperse_01"),
                aContent.Load<SoundEffect>("sound/Angry_Disperse_02")
            };

            s_NomSounds = new SoundEffect[] {
                aContent.Load<SoundEffect>("sound/Eating_01"),
                aContent.Load<SoundEffect>("sound/Eating_02"),
                aContent.Load<SoundEffect>("sound/Eating_03")
            };
        }
    }
}
