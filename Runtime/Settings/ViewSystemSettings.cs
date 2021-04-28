﻿using UniModules.UniGame.ViewSystem.Runtime.ContextFlow.Abstract;

namespace UniGame.UiSystem.Runtime.Settings
{
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniModules.UniGame.ViewSystem.Runtime.ContextFlow;
    using System;
    using System.Collections.Generic;
    using Addressables.Reactive;
    using UniCore.Runtime.ProfilerTools;
    using UniModules.UniGame.Core.Runtime.Attributes;
    using UniModules.UniGame.UISystem.Runtime.Abstract;
    using UniRx;
    using UnityEngine;
    using ViewsFlow;

    /// <summary>
    /// Base View system settings. Contains info about all available view abd type info
    /// </summary>
    [CreateAssetMenu(menuName = "UniGame/ViewSystem/Settings/ViewSystemSettings", fileName = nameof(ViewSystemSettings))]
    public class ViewSystemSettings : ViewsSettings, ICompletionStatus
    {
        #region inspector

        [SerializeField] public List<NestedViewSourceSettings> sources = new List<NestedViewSourceSettings>();

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.DrawWithUnity]
#endif
        [Space]
        [Tooltip("Layout Flow Behaviour")]
        [SerializeField]
        [AssetFilter(typeof(ViewFlowControllerAsset))]
        public ViewFlowControllerAsset layoutFlow;

        [Space]
        [Header("model factory object")]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.InlineEditor(Expanded = false)]
        [Sirenix.OdinInspector.HideLabel]
#endif
        public ViewModelFactorySettings viewsModelProviderSettings;

        [Space]
        [Header("views and models map")]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.InlineProperty]
        [Sirenix.OdinInspector.HideLabel]
#endif
        public ViewModelTypeMap viewModelTypeMap = new ViewModelTypeMap();

        #endregion

        private UiResourceProvider uiResourceProvider;

        [NonSerialized] private bool isStarted;

        public bool IsComplete { get; private set; } = false;

        public IViewModelTypeMap ViewModelTypeMap => viewModelTypeMap;

        public IViewResourceProvider<Component> ResourceProvider => uiResourceProvider;

        public IViewFlowController FlowController { get; protected set; }

        #region public methods
        
        public async UniTask WaitForInitialize()
        {
            while (!LifeTime.IsTerminated && !IsComplete)
            {
                await UniTask.Yield();
            }
        }

        public void Initialize()
        {
            if (isStarted) return;

            IsComplete = false;
            isStarted = true;

            FlowController = layoutFlow.Create();

            uiResourceProvider = uiResourceProvider ?? new UiResourceProvider(viewModelTypeMap);

            viewsModelProviderSettings?.Initialize();
            
            DownloadAllAsyncSources().Forget();
        }

        #endregion
        
        #region private methods

        private async UniTask DownloadAllAsyncSources()
        {
            IsComplete = false;

            foreach (var source in sources.Where(x => !x.awaitLoading))
            {
                LoadAsyncSource(source.viewSourceReference).Forget();
            }

            //load ui views async
            foreach (var viewSource in sources.Where(x => x.awaitLoading))
            {
                await LoadAsyncSource(viewSource.viewSourceReference);
            }

            IsComplete = true;
        }

        private async UniTask LoadAsyncSource(AssetReferenceViewSource reference)
        {
            try
            {
                var settings = await reference
                    .ToObservable()
                    .First();

                if (!settings)
                {
                    GameLog.LogError($"UiManagerSettings Load EMPTY Settings {reference.AssetGUID}");
                    return;
                }
            }
            catch (Exception e)
            {
                GameLog.LogError($"UiManagerSettings Load Ui Source failed {reference.AssetGUID}");
                GameLog.LogError(e);
            }
        }

        private void OnDisable() => Dispose();

        #endregion
    }
}