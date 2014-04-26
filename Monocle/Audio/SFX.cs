using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using Microsoft.Xna.Framework;

namespace Monocle
{
    public class SFX
    {
        public SoundEffect Data { get; private set; }

        public SFX(string filename)
            : this()
        {
            FileStream stream = new FileStream(Engine.Instance.Content.RootDirectory + filename + ".wav", FileMode.Open);
            Data = SoundEffect.FromStream(stream);
            stream.Close();
        }

        internal SFX()
        {
            Audio.SFXList.Add(this);
        }

        public virtual void Play(float panX = 160, float volume = 1)
        {
            if (Audio.MasterVolume > 0)
            {
                volume *= Audio.MasterVolume;
                Data.Play(volume, 0, CalculatePan(panX));
            }
        }

        public virtual void Stop()
        {
            
        }

        public virtual void Pause()
        {
          
        }

        public virtual void Resume()
        {

        }

        internal virtual void OnPitchChange()
        {

        }

        public virtual void AddLoopingToList()
        {

        }

        static public float CalculatePan(float panX)
        {
            return MathHelper.Lerp(-.5f, .5f, panX / 320f);
        }

        static public float CalculateX(float pan)
        {
            return MathHelper.Lerp(0, 320, pan + .5f);
        }
    }
}
