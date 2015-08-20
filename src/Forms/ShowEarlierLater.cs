// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShowEarlierLater.cs" company="">
//   
// </copyright>
// <summary>
//   The show earlier later.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.Enums;

    /// <summary>
    /// The show earlier later.
    /// </summary>
    public sealed partial class ShowEarlierLater : PositionAndSizeForm
    {
        /// <summary>
        /// The adjust event handler.
        /// </summary>
        /// <param name="adjustMilliseconds">
        /// The adjust milliseconds.
        /// </param>
        /// <param name="selection">
        /// The selection.
        /// </param>
        public delegate void AdjustEventHandler(double adjustMilliseconds, SelectionChoice selection);

        /// <summary>
        /// The _adjust callback.
        /// </summary>
        private AdjustEventHandler _adjustCallback;

        /// <summary>
        /// The _total adjustment.
        /// </summary>
        private TimeSpan _totalAdjustment;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowEarlierLater"/> class.
        /// </summary>
        public ShowEarlierLater()
        {
            this.InitializeComponent();
            this.ResetTotalAdjustment();
            this.timeUpDownAdjust.MaskedTextBox.Text = "000000000";

            this.Text = Configuration.Settings.Language.ShowEarlierLater.Title;
            this.labelHourMinSecMilliSecond.Text = Configuration.Settings.Language.General.HourMinutesSecondsMilliseconds;
            this.buttonShowEarlier.Text = Configuration.Settings.Language.ShowEarlierLater.ShowEarlier;
            this.buttonShowLater.Text = Configuration.Settings.Language.ShowEarlierLater.ShowLater;
            this.radioButtonAllLines.Text = Configuration.Settings.Language.ShowEarlierLater.AllLines;
            this.radioButtonSelectedLinesOnly.Text = Configuration.Settings.Language.ShowEarlierLater.SelectedLinesOnly;
            this.radioButtonSelectedLineAndForward.Text = Configuration.Settings.Language.ShowEarlierLater.SelectedLinesAndForward;
            Utilities.FixLargeFonts(this, this.buttonShowEarlier);
        }

        /// <summary>
        /// The reset total adjustment.
        /// </summary>
        public void ResetTotalAdjustment()
        {
            this._totalAdjustment = TimeSpan.FromMilliseconds(0);
            this.labelTotalAdjustment.Text = string.Empty;
        }

        /// <summary>
        /// The show earlier later_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ShowEarlierLater_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            else if (e.KeyCode == Keys.F1)
            {
                Utilities.ShowHelp("#sync");
                e.SuppressKeyPress = true;
            }
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="adjustCallback">
        /// The adjust callback.
        /// </param>
        /// <param name="onlySelected">
        /// The only selected.
        /// </param>
        internal void Initialize(AdjustEventHandler adjustCallback, bool onlySelected)
        {
            if (onlySelected)
            {
                this.radioButtonSelectedLinesOnly.Checked = true;
            }
            else if (Configuration.Settings.Tools.LastShowEarlierOrLaterSelection == SelectionChoice.SelectionAndForward.ToString())
            {
                this.radioButtonSelectedLineAndForward.Checked = true;
            }
            else
            {
                this.radioButtonAllLines.Checked = true;
            }

            this._adjustCallback = adjustCallback;
            this.timeUpDownAdjust.TimeCode = new TimeCode(Configuration.Settings.General.DefaultAdjustMilliseconds);
        }

        /// <summary>
        /// The get selection choice.
        /// </summary>
        /// <returns>
        /// The <see cref="SelectionChoice"/>.
        /// </returns>
        private SelectionChoice GetSelectionChoice()
        {
            if (this.radioButtonSelectedLinesOnly.Checked)
            {
                return SelectionChoice.SelectionOnly;
            }
            else if (this.radioButtonSelectedLineAndForward.Checked)
            {
                return SelectionChoice.SelectionAndForward;
            }
            else
            {
                return SelectionChoice.AllLines;
            }
        }

        /// <summary>
        /// The button show earlier click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonShowEarlierClick(object sender, EventArgs e)
        {
            TimeCode tc = this.timeUpDownAdjust.TimeCode;
            if (tc != null && tc.TotalMilliseconds > 0)
            {
                this._adjustCallback.Invoke(-tc.TotalMilliseconds, this.GetSelectionChoice());
                this._totalAdjustment = TimeSpan.FromMilliseconds(this._totalAdjustment.TotalMilliseconds - tc.TotalMilliseconds);
                this.ShowTotalAdjustMent();
                Configuration.Settings.General.DefaultAdjustMilliseconds = (int)tc.TotalMilliseconds;
            }
        }

        /// <summary>
        /// The show total adjust ment.
        /// </summary>
        private void ShowTotalAdjustMent()
        {
            TimeCode tc = new TimeCode(this._totalAdjustment);
            this.labelTotalAdjustment.Text = string.Format(Configuration.Settings.Language.ShowEarlierLater.TotalAdjustmentX, tc.ToShortString());
        }

        /// <summary>
        /// The button show later click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonShowLaterClick(object sender, EventArgs e)
        {
            TimeCode tc = this.timeUpDownAdjust.TimeCode;
            if (tc != null && tc.TotalMilliseconds > 0)
            {
                this._adjustCallback.Invoke(tc.TotalMilliseconds, this.GetSelectionChoice());
                this._totalAdjustment = TimeSpan.FromMilliseconds(this._totalAdjustment.TotalMilliseconds + tc.TotalMilliseconds);
                this.ShowTotalAdjustMent();
                Configuration.Settings.General.DefaultAdjustMilliseconds = (int)tc.TotalMilliseconds;
            }
        }

        /// <summary>
        /// The radio button all lines_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void radioButtonAllLines_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButtonSelectedLinesOnly.Checked)
            {
                this.Text = Configuration.Settings.Language.ShowEarlierLater.Title;
            }
            else
            {
                this.Text = Configuration.Settings.Language.ShowEarlierLater.TitleAll;
            }
        }

        /// <summary>
        /// The show earlier later_ form closing.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ShowEarlierLater_FormClosing(object sender, FormClosingEventArgs e)
        {
            Configuration.Settings.Tools.LastShowEarlierOrLaterSelection = this.GetSelectionChoice().ToString();
        }
    }
}