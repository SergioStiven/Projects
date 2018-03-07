using System.Threading.Tasks;
using Xamarin.Forms;

namespace Xpinn.SportsGo.Movil.Abstract
{
    public interface IAudioManager
    {
        #region Computed Properties

        float BackgroundMusicVolume { get; set; }

        bool MusicOn { get; set; }

        bool EffectsOn { get; set; }

        float EffectsVolume { get; set; }

        string SoundPath { get; set; }

        #endregion


        #region Public Methods

        void ActivateAudioSession();

        void DeactivateAudioSession();

        Task PlayNotificationDefaultSound();

        Task<bool> ReactivateAudioSession();

        Task<bool> PlayBackgroundMusic(string filename);

        void StopBackgroundMusic();

        void SuspendBackgroundMusic();

        Task<bool> RestartBackgroundMusic();

        Task<bool> PlaySound(string filename);

        #endregion
    }
}
