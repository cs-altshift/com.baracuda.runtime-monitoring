﻿// Copyright (c) 2022 Jonathan Lang

using System;
using System.Text.RegularExpressions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Baracuda.Monitoring.Systems
{
    internal sealed class MonitoringUIManager : IMonitoringUI
    {
        #region Data

        // State
        private string _activeFilter;

        [Monitor]
        private MonitoringUI _current;

        #endregion

        #region Setup

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            if (!MonitoringSystems.Settings.AutoInstantiateUI)
            {
                return;
            }

            if (!(MonitoringSystems.UI is MonitoringUIManager uiManager))
            {
                return;
            }

            if (uiManager.GetCurrent<MonitoringUI>() != null)
            {
                return;
            }

            if (MonitoringSystems.Settings.MonitoringUIOverride != null)
            {
                Object.Instantiate(MonitoringSystems.Settings.MonitoringUIOverride);
            }
            else
            {
                var gameObject = new GameObject("Monitoring UI");
                gameObject.AddComponent<MonitoringUIFallback>();
                gameObject.hideFlags = MonitoringSystems.Settings.ShowRuntimeMonitoringObject ? HideFlags.None : HideFlags.HideInHierarchy;
                Object.DontDestroyOnLoad(gameObject);
            }
        }

        #endregion

        #region Visiblity

        /// <summary>
        /// Get or set the visibility of the current monitoring UI.
        /// </summary>
        public bool Visible
        {
            get => _current && _current.Visible;
            set
            {
                if (!_current || _current.Visible == value)
                {
                    return;
                }

                _current.Visible = value;
                VisibleStateChanged?.Invoke(value);
            }
        }

        public event Action<bool> VisibleStateChanged;

        #endregion

        #region Current

        /// <summary>
        /// Get the current monitoring UI instance
        /// </summary>
        public TMonitoringUI GetCurrent<TMonitoringUI>() where TMonitoringUI : MonitoringUI
        {
            return _current as TMonitoringUI;
        }

        internal void SetActiveMonitoringUI(MonitoringUI monitoringUI)
        {
            if (monitoringUI == _current)
            {
                return;
            }
            if (_current != null)
            {
                if (MonitoringSystems.Settings.AllowMultipleUIInstances)
                {
                    _current.gameObject.SetActive(false);
                }
                else
                {
                    Object.Destroy(_current.gameObject);
                }
            }
            _current = monitoringUI;
            VisibleStateChanged?.Invoke(Visible);
        }

        #endregion

        #region Filtering

        private static readonly Regex onlyLetter =
            new Regex(@"[^a-zA-Z0-9<>_]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public void ApplyFilter(string filterString)
        {
            _activeFilter = filterString;
            MonitoringSystems.Ticker.ValidationTickEnabled = false;

            var settings = MonitoringSystems.Settings;
            var and = settings.FilterAppendSymbol;
            var not = settings.FilterNegateSymbol.ToString();
            var absolute = settings.FilterAbsoluteSymbol.ToString();
            var tag = settings.FilterTagsSymbol.ToString();

            var list = MonitoringSystems.Manager.GetAllMonitoringUnits();
            var filters = filterString.Split(and);

            for (var i = 0; i < list.Count; i++)
            {
                var unit = list[i];
                var unitEnabled = false;

                for (var filterIndex = 0; filterIndex < filters.Length; filterIndex++)
                {
                    var filter = filters[filterIndex];
                    var filterOnlyLetters = onlyLetter.Replace(filter, string.Empty);
                    var filterNoSpace = filter.Replace(" ", string.Empty);

                    unitEnabled = filterNoSpace.StartsWith(not);

                    if (filterNoSpace.StartsWith(absolute))
                    {
                        var absoluteFilter = filterNoSpace.Substring(1);
                        if (unit.Name.StartsWith(absoluteFilter))
                        {
                            unitEnabled = true;
                        }

                        goto End;
                    }

                    if (filterNoSpace.StartsWith(tag))
                    {
                        var tagFilter = filterNoSpace.Substring(1);
                        var customTags = unit.Profile.CustomTags;
                        if (string.IsNullOrWhiteSpace(tagFilter))
                        {
                            goto End;
                        }

                        for (var tagIndex = 0; tagIndex < customTags.Length; tagIndex++)
                        {
                            var customTag = customTags[tagIndex];
                            if (customTag.IndexOf(filterOnlyLetters, settings.FilterComparison) < 0)
                            {
                                continue;
                            }

                            unitEnabled = true;
                            goto End;
                        }

                        goto End;
                    }

                    if (unit.Name.IndexOf(filterOnlyLetters, settings.FilterComparison) >= 0)
                    {
                        unitEnabled = !filterNoSpace.StartsWith(not);
                        goto End;
                    }

                    if (unit.TargetName.IndexOf(filterOnlyLetters, settings.FilterComparison) >= 0)
                    {
                        unitEnabled = !filterNoSpace.StartsWith(not);
                        goto End;
                    }

                    // Filter with tags.
                    var tags = unit.Profile.Tags;
                    for (var tagIndex = 0; tagIndex < tags.Length; tagIndex++)
                    {
                        if (tags[tagIndex].Replace(" ", string.Empty)
                                .IndexOf(filterOnlyLetters, settings.FilterComparison) < 0)
                        {
                            continue;
                        }

                        unitEnabled = !filterNoSpace.StartsWith(not);
                        goto End;
                    }
                }

                End:
                unit.Enabled = unitEnabled;
            }
        }

        public void ResetFilter()
        {
            if (string.IsNullOrWhiteSpace(_activeFilter))
            {
                return;
            }

            _activeFilter = null;
            MonitoringSystems.Ticker.ValidationTickEnabled = true;
            var units = MonitoringSystems.Manager.GetAllMonitoringUnits();
            for (var i = 0; i < units.Count; i++)
            {
                var unit = units[i];
                unit.Enabled = unit.Profile.DefaultEnabled;
            }
        }

        #endregion

        #region Oboslete

        [Obsolete]
        public bool IsVisible()
        {
            return Visible;
        }

        [Obsolete]
        public void Show()
        {
            Visible = true;
        }

        [Obsolete]
        public void Hide()
        {
            Visible = false;
        }

        [Obsolete]
        public bool ToggleDisplay()
        {
            Visible = !Visible;
            return Visible;
        }

        [Obsolete]
        public MonitoringUIController GetActiveUIController()
        {
            return _current as MonitoringUIController;
        }

        [Obsolete]
        public TUIController GetActiveUIController<TUIController>() where TUIController : MonitoringUIController
        {
            return _current as TUIController;
        }

        [Obsolete]
        public void CreateMonitoringUI()
        {
        }

        #endregion
    }
}