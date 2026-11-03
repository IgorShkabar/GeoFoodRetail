/**
 * Copyright (c) 2024-26 Igor Shkabar. All rights reserved. 
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeoFoodRetail.GIS.UI
{
    internal partial class GisMapScheme
    {
        private void ConfigureRightPanel()
        {
            _rightPanel.Dock = DockStyle.Right;
            _rightPanel.BackColor = Color.White;
            _rightPanel.BorderStyle = BorderStyle.FixedSingle;
        }

        private void UpdatePanelLayout()
        {
            if (_rightPanel == null || _mainRightContainer == null) return;
            // The panel occupies 35% of the form's width.
            int calculatedWidth = (int)(this.ClientSize.Width * 0.35);
            _rightPanel.Width = Math.Max(320, calculatedWidth);

            int elementWidth = _rightPanel.Width - 40;
            _txtSearch.Width = elementWidth;
            _searchProgressBar.Width = elementWidth;
            _lblCashInfo.Width = elementWidth;
            _lblSelectedInfo.Width = elementWidth;
            _lblDistanceInfo.Width = elementWidth;
            _lblNoteInfo.Width = elementWidth;
        }

        internal void BuildSearchControls()
        {
            // Main vertical container for ALL elements of the right panel
            _mainRightContainer = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(15)
            };
            _rightPanel.Controls.Add(_mainRightContainer);

            Label lblSearchTitle = new Label
            {
                Text = "Search Address (Geocoding API):",
                AutoSize = true,
                Font = new Font("Arial", 9, FontStyle.Bold),
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
                Text = "⚠️ Введите адрес для поиска",
                AutoSize = true,
                Font = new Font("Arial", 8f, FontStyle.Bold),
                ForeColor = Color.Red,
                Visible = false,
                Margin = new Padding(0, 0, 0, 4)
            };

            _searchProgressBar = new ProgressBar
            {
                Width = 280,
                Height = 6,
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 30,
                Visible = false,
                Margin = new Padding(0, 0, 0, 6)
            };

            // In-line panel for two buttons
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
                Cursor = Cursors.Hand
            };
            _btnSearch.Click += OnSearchClick;

            _btnClearPin = new Button
            {
                Text = "Clear Pin",
                Width = 100,
                Height = 28,
                Enabled = false,
                Cursor = Cursors.Hand
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

        private void BuildInformationLabels()
        {
            _lblCashInfo = new Label {
                Width = 280,
                Height = 25,
                Font = new Font("Consolas", 10, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                Margin = new Padding(0, 0, 0, 10)
            };

            _lblSelectedInfo = new Label {
                Text = "Please select a region on the map!",
                Width = 280,
                Height = 45,
                Font = new Font("Arial", 9.5f, FontStyle.Regular),
                ForeColor = Color.Red,
                Margin = new Padding(0, 0, 0, 10)
            };

            _lblDistanceInfo = new Label {
                Width = 280,
                Height = 250,
                Font = new Font("Consolas", 8.5f, FontStyle.Regular),
                ForeColor = Color.Black,
                Margin = new Padding(0)
            };

            _lblNoteInfo = new Label {                
                Height = 550,
                Font = new Font("Consolas", 8.5f, FontStyle.Regular),
                ForeColor = Color.Black,
                Margin = new Padding(0)
            };

            _mainRightContainer.Controls.Add(_lblCashInfo);
            _mainRightContainer.Controls.Add(_lblSelectedInfo);
            _mainRightContainer.Controls.Add(_lblDistanceInfo);
            _mainRightContainer.Controls.Add(_lblNoteInfo);
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

        private void ResetSearchValidation()
        {
            _txtSearch.BackColor = SystemColors.Window;
            if (_lblSearchError != null)
            {
                _lblSearchError.Visible = false;
            }
        }

        private void ConfigureRightPanel(Panel rightPanel)
        {
            this._rightPanel = rightPanel;
            rightPanel.Dock = DockStyle.Right;
            rightPanel.BackColor = SystemColors.Window;
            rightPanel.BorderStyle = BorderStyle.FixedSingle;
            rightPanel.BringToFront();

            this.Resize += (s, e) => UpdatePanelWidth();
            UpdatePanelWidth();
        }
        private void UpdatePanelWidth()
        {
            if (_rightPanel == null) return;
            int calculatedWidth = (int)(this.ClientSize.Width * 0.30);
            _rightPanel.Width = Math.Max(280, calculatedWidth);
        }

        private void BuildSearchControls(Panel rightPanel)
        {
            Label lblSearchTitle = new Label { Text = "Search Address (Geocoding API):", Left = 15, Top = 10, Width = 330, Font = new Font("Arial", 9, FontStyle.Bold), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            _txtSearch = new TextBox { Left = 15, Top = 30, Width = 250, Font = new Font("Arial", 9), Text = "New York", Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            _btnSearch = new Button { Text = "Search", Left = 270, Top = 29, Width = 80, Height = 25, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            _btnSearch.Click += OnSearchClick;

            _btnClearPin = new Button { Text = "Clear Pin", Left = 270, Top = 58, Width = 80, Height = 25, Enabled = false, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            _btnClearPin.Click += OnClearPinClick;

            rightPanel.Controls.Add(lblSearchTitle);
            rightPanel.Controls.Add(_txtSearch);
            rightPanel.Controls.Add(_btnSearch);
            rightPanel.Controls.Add(_btnClearPin);

            _btnSearch.BringToFront();
            _btnClearPin.BringToFront();
        }

        private void BuildInformationLabels(Panel rightPanel)
        {
            _lblCashInfo = new Label { Left = 15, Top = 90, Width = 335, Height = 25, Font = new Font("Consolas", 10, FontStyle.Bold), ForeColor = Color.DarkBlue, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            _lblSelectedInfo = new Label { Text = "Please select a region on the map!", Left = 15, Top = 120, Width = 335, Height = 45, Font = new Font("Arial", 9.5f, FontStyle.Regular), ForeColor = Color.Red, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            _lblDistanceInfo = new Label { Left = 15, Top = 175, Width = 335, Height = 250, Font = new Font("Consolas", 8.5f, FontStyle.Regular), ForeColor = Color.Black, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom };
            _lblNoteInfo = new Label { Left = 15, Top = 575, Font = new Font("Consolas", 8.5f, FontStyle.Regular), ForeColor = Color.Black, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom };
        
            rightPanel.Controls.Add(_lblCashInfo);
            rightPanel.Controls.Add(_lblSelectedInfo);
            rightPanel.Controls.Add(_lblDistanceInfo);
            rightPanel.Controls.Add(_lblNoteInfo);
        }

        public void UpdateUI()
        {
            _lblCashInfo.Text = "Balance: $" + ExternalCashGiven.ToString("F2");

            if (_storeState.SelectedRegionIndex != -1)
            {
                int idx = _storeState.SelectedRegionIndex;
                _lblSelectedInfo.Text = "SELECTED: " + _regionNames[idx] + "\nDelivery Fee: $" + _regionDeliveryPrices[idx].ToString("F2");
                _lblSelectedInfo.ForeColor = Color.DarkGreen;
                UpdateDistanceMatrixInfo(idx);
            }
        }
    }
}
