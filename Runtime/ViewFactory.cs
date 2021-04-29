﻿using UnityEngine;

namespace UniGame.UiSystem.Runtime
{
    using System;
    using Addressables.Reactive;
    using Cysharp.Threading.Tasks;
    using UniCore.Runtime.ProfilerTools;
    using UniModules.UniCore.Runtime.DataFlow;
    using UniModules.UniCore.Runtime.ObjectPool.Runtime.Extensions;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.UISystem.Runtime.Abstract;
    using UniRx;
    
    using Object = UnityEngine.Object;

    public class ViewFactory : IViewFactory
    {
        private readonly AsyncLazy _readyStatus;
        private readonly IViewResourceProvider resourceProvider;
        
        public ViewFactory(
            AsyncLazy readyStatus,
            IViewResourceProvider viewResourceProvider)
        {
            _readyStatus = readyStatus;
            resourceProvider = viewResourceProvider;
        }

        public async UniTask<IView> Create(
            Type viewType, 
            string skinTag = "", 
            Transform parent = null, 
            string viewName = "",
            bool stayWorldPosition = false)
        {
            await _readyStatus;

            var viewLifeTime = LifeTime.Create();
            var result       = await resourceProvider.LoadViewAsync<Component>(viewType,viewLifeTime,skinTag, viewName:viewName);
            //create view instance
            var view = Create(result, parent,stayWorldPosition);
            
            //if loading failed release resource immediately
            if (view == null) {
                viewLifeTime.Despawn();
                GameLog.LogError($"Factory {this.GetType().Name} View of Type {viewType?.Name} not loaded");
                return null;
            }

            view.LifeTime.AddCleanUpAction(() => viewLifeTime.Despawn());

            return view;
        }
        
        /// <summary>
        /// create view instance
        /// </summary>
        protected virtual IView Create(Component asset, Transform parent = null, bool stayPosition = false)
        {
            if (asset == null) return null;
            //create instance of view
            var view = Object.
                Instantiate(asset.gameObject, parent,stayPosition).
                GetComponent<IView>();
            return view;
        }
    }
}
