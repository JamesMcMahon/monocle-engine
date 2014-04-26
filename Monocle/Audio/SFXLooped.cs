using Microsoft.Xna.Framework.Audio;

namespace Monocle
{
    public class SFXLooped : SFX
    {
        public SoundEffectInstance Instance { get; private set; }
        public int Plays { get; private set; }

        private float setVolume;

        public SFXLooped(string filename, bool obeysMasterPitch = true)
            : base(filename)
        {
            Instance = Data.CreateInstance();
            Instance.IsLooped = true;
        }

        public override void Play(float panX = 160, float volume = 1)
        {
            if (volume * Audio.MasterVolume > 0)
            {
                setVolume = volume;
                Instance.Volume = volume * Audio.MasterVolume;
                Instance.Pan = CalculatePan(panX);
                if (Instance.State != SoundState.Playing)
                    Instance.Play();
            }
        }

        public override void Stop()
        {
            if (Instance.State != SoundState.Stopped)
                Instance.Stop();
        }

        public override void Pause()
        {
            if (Instance.State == SoundState.Playing)
                Instance.Pause();
        }

        public override void Resume()
        {
            if (Instance.State == SoundState.Paused)
                Instance.Resume();
        }

        public void SetVolume(float volume)
        {
            setVolume = volume;
            Instance.Volume = volume * Audio.MasterVolume;
        }
    }
}
