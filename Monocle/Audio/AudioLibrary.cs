using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Monocle
{
    public class AudioLibrary
    {
        public string AudioDirectory { get; private set; }
        public bool Loaded { get; private set; }

        private Dictionary<string, SoundEffect> sounds;
        public Dictionary<string, SoundEffectInstance> Instances { get; private set; }

        public AudioLibrary(string audioDirectory, bool load = true)
        {
#if DEBUG
            if (!Directory.Exists(audioDirectory))
                throw new Exception("The directory does not exist!");
#endif
            AudioDirectory = audioDirectory;
            sounds = new Dictionary<string, SoundEffect>();
            Instances = new Dictionary<string, SoundEffectInstance>();

            if (load)
                Load();
        }

        public SoundEffectInstance this[string index]
        {
            get
            {
                return Instances[index];
            }
        }

        public void Play(string sound, float x = 160)
        {
            SoundEffectInstance instance = Instances[sound];

            instance.Stop();
            instance.Pan = MathHelper.Lerp(-.5f, .5f, x / 320f);
            instance.Play();
        }

        public void PlayOnce(string sound, float x = 160)
        {
            SoundEffectInstance instance = Instances[sound];

            instance.Pan = MathHelper.Lerp(-.5f, .5f, x / 320f);
            instance.Play();
        }

        public void Stop(string sound)
        {
            Instances[sound].Stop();
        }

        public void StopAll()
        {
            foreach (var kv in Instances)
                kv.Value.Stop();
        }

        public void PauseAll()
        {
            foreach (var kv in Instances)
            {
                if (kv.Value.State == SoundState.Playing)
                    kv.Value.Pause();
            }
        }

        public void ResumeAll()
        {
            foreach (var kv in Instances)
            {
                if (kv.Value.State == SoundState.Paused)
                    kv.Value.Resume();
            }
        }

        public void Load()
        {
#if DEBUG
            if (Loaded)
                throw new Exception("Audio Library is already loaded!");
#endif
            Loaded = true;

            string prefix = AudioDirectory + @"\";
            foreach (var file in Directory.EnumerateFiles(AudioDirectory, "*.wav", SearchOption.AllDirectories))
            {
                string name = file.Remove(0, prefix.Length);
                name = name.Remove(name.IndexOf(".wav"));

                FileStream stream = new FileStream(file, FileMode.Open);
                SoundEffect sound = SoundEffect.FromStream(stream);
                stream.Close();

                sounds.Add(name, sound);
                Instances.Add(name, sound.CreateInstance());
            }
        }

        public void Unload()
        {
#if DEBUG
            if (!Loaded)
                throw new Exception("Audio Library is already unloaded!");
#endif

            Loaded = false;

            foreach (var kv in sounds)
                kv.Value.Dispose();
            foreach (var kv in Instances)
                kv.Value.Dispose();
            sounds.Clear();
            Instances.Clear();
        }
    }
}
