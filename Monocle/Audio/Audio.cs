using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Monocle
{
    static public class Audio
    {
        static internal List<SFX> SFXList = new List<SFX>();

        static private float masterVolume = 1f;
        static public float MasterVolume
        {
            get { return masterVolume; }
            set { masterVolume = MathHelper.Clamp(value, 0, 1); }
        }

        static public int MasterVolumeInt
        {
            get { return (int)Math.Round(masterVolume * 10f); }
            set { MasterVolume = value / 10f; }
        }

        static public void Stop()
        {
            foreach (var s in SFXList)
                s.Stop();
        }

        static public void Pause()
        {
            foreach (var s in SFXList)
                s.Pause();
        }

        static public void Resume()
        {
            foreach (var s in SFXList)
                s.Resume();
        }
    }
}
