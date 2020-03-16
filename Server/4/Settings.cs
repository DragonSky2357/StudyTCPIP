using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4 {
    internal sealed partial class Settings {
        public Settings() {

        }

        private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e) {
            // SettingChangingEvent 이벤트를 처리하는 코드를 여기에 추가하십시오.
        }

        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e) {
            // SettingsSaving 이벤트를 처리하는 코드를 여기에 추가하십시오.
        }
    }
}
