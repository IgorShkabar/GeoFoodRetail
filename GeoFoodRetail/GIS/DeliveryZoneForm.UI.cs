/** 
 * Copyright (c) 2024 Igor Shkabar. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GeoFoodRetail.GIS.UI;

namespace GeoFoodRetail.GIS
{
    internal partial class DeliveryZoneForm
    {
        private void InitUI()
        {
            _rightPanel = new Panel();
            _rightPanel.BackColor = SystemColors.Window;
            ConfigureRightPanel();

            BuildSearchControls();            
            BuildStoreListUI();
            BuildInformationLabels();

            this.Controls.Add(_rightPanel);
            _rightPanel.BringToFront(); 

            this.Resize += (s, e) => UpdatePanelLayout();
            UpdatePanelLayout();
            UpdateUI();
        }

        private void BuildSearchControls()
        {
            _mainRightContainer = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(15)
            };
            _rightPanel.Controls.Add(_mainRightContainer);

            Label lblSearchTitle = LabelSearch();

            FlowLayoutPanel buttonPanel = ButtonPanelAndSearchSetUp();

            _btnClearPin = new Button
            {
                Dock = DockStyle.Right,
                Text = "Clear Pin",
                Width = 100,
                Height = 28,
                Margin = new Padding(70, 0, 8, 0),
                Enabled = true,
                Cursor = Cursors.Hand,
                Font = new Font("Arial", 9.5f, FontStyle.Bold),
                ForeColor = SystemColors.Window,
                BackColor = SystemColors.GrayText
            };
            _btnClearPin.Click += OnClearPinClick;

            buttonPanel.Controls.Add(_btnSearch);
            buttonPanel.Controls.Add(_btnClearPin);

            _mainRightContainer.Controls.Add(lblSearchTitle);
            _mainRightContainer.Controls.Add(_txtSearch);
            _mainRightContainer.Controls.Add(_lblSearchError);
            _mainRightContainer.Controls.Add(_searchProgressBar);
            _mainRightContainer.Controls.Add(buttonPanel);
        }

        private FlowLayoutPanel ButtonPanelAndSearchSetUp()
        {
            _searchProgressBar = new ProgressBar
            {
                Width = 280,
                Height = 6,
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 30,
                Visible = false,
                Margin = new Padding(0, 0, 0, 6)
            };

            FlowLayoutPanel buttonPanel = new FlowLayoutPanel
            {
                Width = 280,
                Height = 34,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 0, 0, 15)
            };

            _btnSearch = new Button
            {
                Text = "Search",
                Width = 100,
                Height = 28,
                Margin = new Padding(0, 0, 10, 0),
                Cursor = Cursors.Hand,
                Font = new Font("Arial", 9.5f, FontStyle.Bold),
                ForeColor = SystemColors.WindowText,
                BackColor = SystemColors.GradientInactiveCaption
            };
            _btnSearch.Click += OnSearchClick;
            return buttonPanel;
        }

        private Label LabelSearch()
        {
            Label lblSearchTitle = new Label
            {
                Text = "Search Address (Geocoding API):",
                AutoSize = true,
                Font = new Font("Arial", 9.5f, FontStyle.Bold),
                Margin = new Padding(0, 0, 0, 5)
            };

            _txtSearch = new TextBox
            {
                Text = "New York",
                Width = 280,
                Font = new Font("Arial", 9.5f),
                Margin = new Padding(0, 0, 0, 2)
            };
            _txtSearch.TextChanged += (s, e) => ResetSearchValidation();

            _lblSearchError = new Label
            {
                Text = "Enter an address to search",
                AutoSize = true,
                Font = new Font("Arial", 8f, FontStyle.Bold),
                ForeColor = Color.Red,
                Visible = false,
                Margin = new Padding(0, 0, 0, 4)
            };
            return lblSearchTitle;
        }

        private void BuildInformationLabels()
        {
            _lblCashInfo = new Label
            {
                Width = 280,
                Height = 25,
                Font = new Font("Consolas", 10, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                Margin = new Padding(0, 0, 0, 10)
            };
            _lblSelectedInfo = new Label
            {
                Text = "Please select a region on the map!",
                Width = 280,
                Height = 45,
                Font = new Font("Arial", 9.5f, FontStyle.Regular),
                ForeColor = Color.Red,
                Margin = new Padding(0, 0, 0, 10)
            };
            _lblDistanceInfo = new Label
            {
                Width = 280,
                Height = 250,
                Font = new Font("Consolas", 8.5f, FontStyle.Regular),
                ForeColor = Color.Black,
                Margin = new Padding(0)
            };
            _mainRightContainer.Controls.Add(_lblCashInfo);
            _mainRightContainer.Controls.Add(_lblSelectedInfo);
            _mainRightContainer.Controls.Add(_lblDistanceInfo);
        }

        private bool ValidateSearchInput(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {                
                _txtSearch.BackColor = Color.MistyRose;
                _lblSearchError.Visible = true;
                _txtSearch.Focus();
                return false;
            }
            ResetSearchValidation();
            return true;
        }

        private void UpdatePanelLayout()
        {
            if (_rightPanel == null || _mainRightContainer == null) return;

            // Calculating the ideal size of a panel for its content
            Size preferredSize = _rightPanel.GetPreferredSize(Size.Empty);
            
            int calculatedWidth = Math.Max(320, preferredSize.Width);
            
            _rightPanel.Width = calculatedWidth;


            int elementWidth = _rightPanel.Width - 40;
            _txtSearch.Width = elementWidth;
            _searchProgressBar.Width = elementWidth;
            _lblCashInfo.Width = elementWidth;
            _lblSelectedInfo.Width = elementWidth;
            _lblDistanceInfo.Width = elementWidth;
        }

        public void UpdateUI()
        {
            _lblCashInfo.Text = "Balance: $" + ExternalCashgivenProperty.ToString("F2");
            
            int safeIndex = (_selectedStoreIndex >= 0 && _selectedStoreIndex < StoreNameArr.Count) ? _selectedStoreIndex : 0;

            // Synchronizing state
            _selectedStoreIndex = safeIndex;
            _storeState.SelectedRegionIndex = safeIndex;
            
            if (safeIndex < _storeCoordArray.Length && safeIndex < StoreNameArr.Count)
            {
                PointF storeCoord = _storeCoordArray[safeIndex];
                string storeName = StoreNameArr[safeIndex];
                
                decimal deliveryPrice = (_regionDeliveryPrices != null && safeIndex < _regionDeliveryPrices.Length)
                    ? _regionDeliveryPrices[safeIndex]
                    : 0.0m;

                _lblSelectedInfo.Text = "SELECTED: " + storeName + "\nDelivery Fee: $" + deliveryPrice.ToString("F2");
                _lblSelectedInfo.ForeColor = Color.DarkGreen;
                UpdateDistanceMatrixInfo(safeIndex);
            }
            else
            {
                _selectedStoreIndex = 0;
                _lblSelectedInfo.Text = "No store selected";
            }
        }
    }
}
