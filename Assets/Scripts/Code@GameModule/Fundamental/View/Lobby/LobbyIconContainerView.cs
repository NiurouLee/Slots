// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/19/15:49
// Ver : 1.0.0
// Description : LobbyIconContainerView.cs
// ChangeLog :
// **********************************************

using UnityEngine;

namespace GameModule
{
    public class LobbyIconContainerView : View
    {
        [ComponentBinder("TopContainer")] public Transform topContainer;
        [ComponentBinder("BottomContainer")] public Transform bottomContainer;

        private LobbySingleIconContainerView topIconContainerView;
        private LobbySingleIconContainerView bottomIconContainerView;

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();

            topIconContainerView = AddChild<LobbySingleIconContainerView>(topContainer);
            bottomIconContainerView = AddChild<LobbySingleIconContainerView>(bottomContainer);
        }

        public void UpdateContent(int bannerListViewItemIndex, int iconIndex, int machineCount)
        {
            var groupIndex = iconIndex / 3;
            var columnIndex = iconIndex % 3;

            var topMachineIndex = groupIndex * 6 + columnIndex;
           
            topIconContainerView.transform.gameObject.SetActive(topMachineIndex < machineCount);
           
            if (topMachineIndex < machineCount)
            {
                topIconContainerView.viewController.UpdateContent(bannerListViewItemIndex, topMachineIndex);
            }

            var bottomMachineIndex = groupIndex * 6 + 3 + columnIndex;

            bottomIconContainerView.transform.gameObject.SetActive(bottomMachineIndex < machineCount);
         
            if (bottomMachineIndex < machineCount)
            {
                bottomIconContainerView.viewController.UpdateContent(bannerListViewItemIndex, bottomMachineIndex);
            }
        }
    }
}