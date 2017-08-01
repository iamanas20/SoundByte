/* |----------------------------------------------------------------|
 * | Copyright (c) 2017, Grid Entertainment                         |
 * | All Rights Reserved                                            |
 * |                                                                |
 * | This source code is to only be used for educational            |
 * | purposes. Distribution of SoundByte source code in             |
 * | any form outside this repository is forbidden. If you          |
 * | would like to contribute to the SoundByte source code, you     |
 * | are welcome.                                                   |
 * |----------------------------------------------------------------|
 */

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