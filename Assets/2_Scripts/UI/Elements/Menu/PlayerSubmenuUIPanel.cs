using UnityEngine.UI;

namespace _2_Scripts.UI.Elements.Menu
{
    public class PlayerSubmenuUIPanel : UIPanel
    {
        //UI panel that does not deactivate, only fades out. If need arises it will also move the panel outside of view.
        
        protected override void SetGameObjectActive(bool value)
        {
            // do nothing. We want player UI to be active by default so everything can get updated correctly in the bg
        }

        protected override void SetButtonActive(Button button, bool value)
        {
            //do nothing. Buttons are active by default on player panels.
        }
    }
}