﻿using System.Collections.Generic;
using Client.UI;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public class BattleHudPanel : UIWindow<BattleScreen>
    {
        public struct RegisterToken : IPanelRegisterToken<BattleHudPanel>
        {
            private readonly PhotonBoltClientListener clientListener;

            public RegisterToken(PhotonBoltClientListener clientListener)
            {
                this.clientListener = clientListener;
            }

            public void Initialize(BattleHudPanel panel)
            {
                panel.clientListener = clientListener;
            }
        }

        public struct UnregisterToken : IPanelUnregisterToken<BattleHudPanel>
        {
            public void Deinitialize(BattleHudPanel panel)
            {
                panel.gameObject.SetActive(false);
            }
        }

        [SerializeField, UsedImplicitly] private UnitFrame playerUnitFrame;
        [SerializeField, UsedImplicitly] private UnitFrame playerTargetUnitFrame;
        [SerializeField, UsedImplicitly] private CastFrame playerCastFrame;
        [SerializeField, UsedImplicitly] private List<ActionBar> actionBars;

        private PhotonBoltClientListener clientListener;

        protected override void PanelInitialized()
        {
            base.PanelInitialized();

            actionBars.ForEach(actionBar => actionBar.Initialize());

            clientListener.EventPlayerControlGained += OnPlayerControlGained;
            clientListener.EventPlayerControlLost += OnPlayerControlLost;

            playerCastFrame.gameObject.SetActive(false);
        }

        protected override void PanelDeinitialized()
        {
            clientListener.EventPlayerControlGained -= OnPlayerControlGained;
            clientListener.EventPlayerControlLost -= OnPlayerControlLost;

            playerUnitFrame.UpdateUnit(null);
            playerTargetUnitFrame.UpdateUnit(null);

            base.PanelDeinitialized();
        }

        protected override void PanelUpdated(float deltaTime)
        {
            base.PanelUpdated(deltaTime);

            playerCastFrame.DoUpdate(deltaTime);

            if (clientListener.LocalPlayer != null)
                foreach (var actionBar in actionBars)
                    actionBar.DoUpdate(deltaTime);
        }

        private void OnPlayerControlGained()
        {
            playerUnitFrame.UpdateUnit(clientListener.LocalPlayer);
            playerCastFrame.UpdateUnit(clientListener.LocalPlayer);
        }

        private void OnPlayerControlLost()
        {
            playerUnitFrame.UpdateUnit(null);
            playerCastFrame.UpdateUnit(null);
        }
    }
}