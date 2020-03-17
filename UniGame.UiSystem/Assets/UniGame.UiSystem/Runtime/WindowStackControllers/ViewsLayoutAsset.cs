﻿using UnityEngine;

namespace UniGame.UiSystem.Runtime.WindowStackControllers
{
    using System;
    using Abstracts;

    public class ViewsLayoutAsset : MonoBehaviour, IViewLayout
    {
        #region inspector

        public Canvas layoutCanvas;

        #endregion

        private Lazy<IViewLayout> layout;

        public IViewLayout StackController => layout.Value;

        public Transform Layout => StackController.Layout;

        #region public methods

        public ViewsLayoutAsset()
        {
            layout = new Lazy<IViewLayout>(Create);
        }

        public void Dispose() => StackController.Dispose();

        public bool Contains(IView view) => StackController.Contains(view);
        public void Hide<T>() where T : Component, IView
        {
            StackController.Hide<T>();
        }

        public void HideAll()
        {
            StackController.HideAll();
        }

        public void HideAll<T>() where T : Component, IView
        {
            StackController.HideAll<T>();
        }

        public void Close<T>() where T : Component, IView
        {
            StackController.Close<T>();
        }

        public void Push<TView>(TView view) where TView : Component, IView
        {
            StackController.Push(view);
        }

        public bool Close<T>(T view) where T : Component, IView
        {
            return StackController.Close<T>(view);
        }

        public TView Get<TView>() where TView : Component, IView
        {
            return StackController.Get<TView>();
        }

        public void CloseAll() => StackController.CloseAll();

        #endregion


        #region private methods

        protected virtual IViewLayout Create()
        {
            return new ViewsStackLayout(layoutCanvas.transform);
        }
        
        #endregion
    }
}