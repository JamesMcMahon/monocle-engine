using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace Monocle
{
    public class SFXInstanced : SFX
    {
        public SoundEffectInstance[] Instances { get; private set; }
        private int lastPlayed;

        public SFXInstanced(string filename, int instances = 2, bool obeysMasterPitch = true)
            : base(filename)
        {
            Instances = new SoundEffectInstance[instances];
            for (int i = 0; i < instances; i++)
                Instances[i] = Data.CreateInstance();
        }

        public override void Play(float panX = 160, float volume = 1)
        {
            if (Audio.MasterVolume <= 0)
                return;
            volume *= Audio.MasterVolume;

            SoundEffectInstance toPlay = Instances[lastPlayed];
            lastPlayed = (lastPlayed + 1) % Instances.Length;

            toPlay.Stop(true);
            toPlay.Pan = CalculatePan(panX);
            toPlay.Volume = volume;
            toPlay.Play();
        }

        public override void Stop()
        {
            foreach (var i in Instances)
                if (i.State != SoundState.Stopped)
                    i.Stop();
        }

        public override void Pause()
        {
            foreach (var i in Instances)
                if (i.State == SoundState.Playing)
                    i.Pause();
        }

        public override void Resume()
        {
            foreach (var i in Instances)
                if (i.State == SoundState.Paused)
                    i.Resume();
        }
    }
}
