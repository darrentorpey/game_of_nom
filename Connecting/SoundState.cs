﻿using System;
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
        private static SoundEffect s_PickUpSound;

        private struct SoundInfo
        {
            public int _iSoundIndex;
            public int _iLastPlayTime;
            public int _iDelay;
        }

        private Dictionary<Person, SoundInfo> _AngrySoundMap = new Dictionary<Person,SoundInfo>();
        private Dictionary<FoodSource, SoundInfo> _NomSoundMap = new Dictionary<FoodSource, SoundInfo>();
        private SoundInfo[] _ExplodeSounds = new SoundInfo[3];
        private SoundInfo _PickupSound;
        private bool _bSoundOn = true;

        public static SoundState Instance
        {
            get { return s_Instance; }
        }

        private SoundState()
        {

        }

        public void ToggleSound()
        {
            _bSoundOn = !_bSoundOn;
        }

        public void PlayAngrySound(Person aPerson, GameTime aTime)
        {
            if (!_bSoundOn)
                return;

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
            if (!_bSoundOn)
                return;

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
            if (!_bSoundOn)
                return;

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

        public void PlayPickupSound(GameTime aTime)
        {
            if (!_bSoundOn)
                return;

            if (IsSoundFinished(ref _PickupSound, s_PickUpSound, aTime))
            {
                _PickupSound._iLastPlayTime = (int)aTime.TotalGameTime.TotalMilliseconds; ;
                _PickupSound._iDelay = 200;
                s_PickUpSound.Play();
            }                
        }

        private static bool IsSoundFinished(ref SoundInfo aInfo, SoundEffect aEffect, GameTime aTime)
        {
            if(aInfo._iLastPlayTime == 0)
                return true;
            int ifinishedTime = aInfo._iLastPlayTime + (int)aEffect.Duration.TotalMilliseconds + aInfo._iDelay;
            return ifinishedTime < aTime.TotalGameTime.TotalMilliseconds;
        }

        public static void LoadContent(ContentManager aContent)
        {
            s_AngrySounds = new SoundEffect[] {
                aContent.Load<SoundEffect>("sound/Grumble_01"),
                aContent.Load<SoundEffect>("sound/Grumble_02"),
                aContent.Load<SoundEffect>("sound/Grumble_03"),
                aContent.Load<SoundEffect>("sound/Grumble_04"),
                aContent.Load<SoundEffect>("sound/Grumble_05"),
                aContent.Load<SoundEffect>("sound/Grumble_06"),
                aContent.Load<SoundEffect>("sound/Grumble_07"),
                aContent.Load<SoundEffect>("sound/Grumble_08"),
            };

            s_ExplodeSounds = new SoundEffect[] {
                aContent.Load<SoundEffect>("sound/Burst_01"),
                aContent.Load<SoundEffect>("sound/Burst_02")
            };

            s_NomSounds = new SoundEffect[] {
                aContent.Load<SoundEffect>("sound/Eat_01"),
                aContent.Load<SoundEffect>("sound/Eat_02"),
                aContent.Load<SoundEffect>("sound/Eat_03"),
                aContent.Load<SoundEffect>("sound/Eat_04"),
                aContent.Load<SoundEffect>("sound/Eat_05"),
                aContent.Load<SoundEffect>("sound/Eat_06")
            };

            s_PickUpSound = aContent.Load<SoundEffect>("sound/Pickup");
        }
    }
}