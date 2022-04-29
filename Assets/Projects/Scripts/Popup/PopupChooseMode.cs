using System.Collections.Generic;
using Projects.Scripts.Hub.Component;
using Truongtv.PopUpController;
using UnityEngine;

namespace Projects.Scripts.Popup
{
    public class PopupChooseMode : BasePopup
    {
        [SerializeField] private List<ModeGameLauncher> launcherList;
    }
}