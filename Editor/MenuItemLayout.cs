// Copyright (c) 2022 Jonathan Lang

using UnityEngine;

namespace Baracuda.Monitoring.Editor
{
    internal class MenuItemLayout : MonoBehaviour
    {
        [UnityEditor.MenuItem("Window/Runtime Monitoring/Settings", priority = 12405)]
        private static void OpenMonitorSettingsWindow()
        {
            MonitoringSettingsWindow.Open();
        }

        [UnityEditor.MenuItem("Window/Runtime Monitoring/Filter Window", priority = 12406)]
        private static void OpenFiltersWindow()
        {
            MonitoringFilterWindow.Open();
        }
    }
}