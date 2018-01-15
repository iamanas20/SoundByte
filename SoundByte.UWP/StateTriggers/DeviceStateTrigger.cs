using Windows.System.Profile;
using Windows.UI.Xaml;

namespace SoundByte.UWP.StateTriggers
{
    public class DeviceStateTrigger : StateTriggerBase
    {
        private string _deviceFamily;

        public string DeviceFamily
        {
            get => _deviceFamily;
            set
            {
                _deviceFamily = value;
                SetActive(_deviceFamily == AnalyticsInfo.VersionInfo.DeviceFamily);
            }
        }
    }
}