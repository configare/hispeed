using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections;
using Telerik.WinControls.WindowAnimation;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// This class provides API for managing <see cref="RadDesktopAlert"/>components.
    /// </summary>
    public class DesktopAlertManager
    {
        #region Events

        /// <summary>
        /// Fires when an instance of the <see cref="RadDesktopAlert"/> class
        /// is registered with this <see cref="DesktopAlertManager"/>.
        /// </summary>
        public event DesktopAlertManagerEventHandler AlertAdded;

        /// <summary>
        /// Fires when an instance of the <see cref="RadDesktopAlert"/> class
        /// is removed from this <see cref="DesktopAlertManager"/>.
        /// </summary>
        public event DesktopAlertManagerEventHandler AlertRemoved;

        #endregion

        #region Fields

        [ThreadStatic]
        private static DesktopAlertManager instance;
        private Screen activeScreen;
        private List<RadDesktopAlert> openedAlerts;

        #endregion

        #region Ctor

        private DesktopAlertManager()
        {
            this.openedAlerts = new List<RadDesktopAlert>();
            this.activeScreen = Screen.PrimaryScreen;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the only instance of the <see cref="DesktopAlertManager"/>
        /// class.
        /// </summary>
        public static DesktopAlertManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DesktopAlertManager();
                }

                return instance;
            }
        }

        /// <summary>
        /// Gets an instance of the <see cref="Screen"/>class
        /// that represents the screen onto which the <see cref="DesktopAlertManager"/>
        /// positions the alert popups.
        /// </summary>
        public Screen ActiveScreen
        {
            get
            {
                return this.activeScreen;
            }
        }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// Gets an instance of the <see cref="Point"/>struct
        /// that represents the location of the current alert
        /// according to its screen position setting and
        /// the currently opened alerts.
        /// </summary>
        /// <param name="alert">An instance of the <see cref="RadDesktopAlert"/>
        /// class that represents the alert which position to define.</param>
        /// <returns>The evaluated position in screen coordinates.</returns>
        public Point GetAlertPopupLocation(RadDesktopAlert alert)
        {
            if (alert.GetLocationModifiedByUser())
            {
                return alert.Popup.Location;
            }

            switch (alert.ScreenPosition)
            {
                case AlertScreenPosition.Manual:
                    return alert.Popup.Location;
                case AlertScreenPosition.BottomRight:
                case AlertScreenPosition.BottomLeft:
                case AlertScreenPosition.BottomCenter:
                    {
                        Point result = new Point();
                        result.X = this.GetHorizontalLocation(alert);
                        result.Y = this.GetBottomTopLocation(alert);
                        return result;
                    }
                case AlertScreenPosition.TopRight:
                case AlertScreenPosition.TopLeft:
                case AlertScreenPosition.TopCenter:
                    {
                        Point result = new Point();
                        result.X = this.GetHorizontalLocation(alert);
                        result.Y = this.GetTopBottomLocation(alert);
                        return result;
                    }
            }

            return Point.Empty;
        }


        /// <summary>
        /// Sets the active screen. The active screen is used
        /// to calculate the positioning of all desktop alerts.
        /// </summary>
        /// <param name="activeScreen">An instance of the <see cref="Screen"/>
        /// class that is the active screen to set.</param>
        public void SetActiveScreen(Screen activeScreen)
        {
            this.activeScreen = activeScreen;
            this.UpdateAlertsOrder();
        }

        /// <summary>
        /// Gets an enumerator for the currently shown dekstop alerts.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<RadDesktopAlert> GetRegisteredAlerts()
        {
            return this.openedAlerts.GetEnumerator();
        }

        /// <summary>
        /// Recalculates the location of all opened alerts
        /// based on their screen position.
        /// </summary>
        public void UpdateAlertsOrder()
        {
            foreach (RadDesktopAlert alert in this.openedAlerts)
            {
                Point newLocation = this.GetAlertPopupLocation(alert);

                alert.OnLocationChangeRequested(newLocation);
            }
        }

        /// <summary>
        /// Registers an instance of the <see cref="RadDesktopAlert"/>and
        /// displays it on the screen according to its
        /// </summary>
        /// <param name="alert"></param>
        public void AddAlert(RadDesktopAlert alert)
        {
            if (this.openedAlerts.Contains(alert))
            {
                return;
            }
            alert.OnLocationChangeRequested(this.GetAlertPopupLocation(alert));
            this.openedAlerts.Add(alert);

            if (this.AlertAdded != null)
            {
                DesktopAlertManagerEventArgs args = new DesktopAlertManagerEventArgs(alert);
                this.AlertAdded(this, args);
            }
        }

        /// <summary>
        /// Unregisters a desktop alert from the manager.
        /// </summary>
        /// <param name="alert">The alert to unregister.</param>
        public void RemoveAlert(RadDesktopAlert alert)
        {
            Debug.Assert(this.openedAlerts.Contains(alert), "Trying to remove non-existing alert.");
            this.openedAlerts.Remove(alert);
            this.UpdateAlertsOrder();

            if (this.AlertRemoved != null)
            {
                DesktopAlertManagerEventArgs args = new DesktopAlertManagerEventArgs(alert);
                this.AlertRemoved(this, args);
            }
        }

        /// <summary>
        /// Evaluates whether a given <see cref="RadDesktopAlert"/>
        /// is registered with the <see cref="DesktopAlertManager"/>.
        /// </summary>
        /// <param name="alert">The <see cref="RadDesktopAlert"/> to check.</param>
        /// <returns></returns>
        public bool ContainsAlert(RadDesktopAlert alert)
        {
            return this.openedAlerts.Contains(alert);
        }

        #endregion

        #region Service methods
        
        private int GetHorizontalLocation(RadDesktopAlert alert)
        {
            Size alertSize = alert.Popup.NonAnimatedSize;
            switch(alert.ScreenPosition)
            {
                case AlertScreenPosition.BottomCenter:
                case AlertScreenPosition.TopCenter:
                    {
                        int screenWidth =  this.activeScreen.WorkingArea.Width;
                        return (this.activeScreen.WorkingArea.X + (screenWidth - alertSize.Width) / 2);
                    }
                case AlertScreenPosition.TopLeft:
                case AlertScreenPosition.BottomLeft:
                    {
                        return this.activeScreen.WorkingArea.X;
                    }
                case AlertScreenPosition.TopRight:
                case AlertScreenPosition.BottomRight:
                    {
                        return this.activeScreen.WorkingArea.X + 
                            this.activeScreen.WorkingArea.Width -
                            alertSize.Width;
                    }
                default:
                    return this.activeScreen.WorkingArea.X;
            }
        }

        private int GetBottomTopLocation(RadDesktopAlert alert)
        {
            Size alertSize = alert.Popup.NonAnimatedSize;
            int yCoord = this.activeScreen.WorkingArea.Height - alertSize.Height;
            yCoord += this.activeScreen.WorkingArea.Y;
            this.EvaluateAlertOffset(alert, ref yCoord, false);
            return  yCoord;
        }

        private int GetTopBottomLocation(RadDesktopAlert alert)
        {
            int yCoord = this.activeScreen.WorkingArea.Y;
            this.EvaluateAlertOffset(alert, ref yCoord, true);
            return yCoord;
        }

        private void EvaluateAlertOffset(RadDesktopAlert alert, ref int initialOffset, bool isTopBottom)
        {
            foreach (RadDesktopAlert currentAlert in this.openedAlerts)
            {
                if (object.ReferenceEquals(currentAlert, alert))
                {
                    break;
                }

                if (currentAlert.GetLocationModifiedByUser())
                {
                    continue;
                }

                Size alertSize = currentAlert.Popup.NonAnimatedSize;

                if (currentAlert.ScreenPosition == alert.ScreenPosition)
                {
                    if (isTopBottom)
                    {
                        initialOffset += alertSize.Height;
                    }
                    else
                    {
                        initialOffset -= alertSize.Height;
                    }
                }
            }
        }

        #endregion

        #endregion
    }
}
