#region Using declarations
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;                 // for [XmlIgnore]
using System.Windows.Media;                     // for Brushes
using NinjaTrader.Data;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Tools;
using NinjaTrader.NinjaScript;
#endregion

// --------------------------------------------------------------------------------------
//   Ads4Dads_VWAP_Bands
// - Session VWAP with auto-color + ±1σ / ±2σ bands
// - Session-anchored, resets each new session
// - Safe for Calculate.OnBarClose or OnEachTick
// - Writes NaN when hidden so bands don't affect scaling
// --------------------------------------------------------------------------------------
namespace NinjaTrader.NinjaScript.Indicators
{
    public class Ads4Dads_VWAP_Bands : Indicator
    {
        // Session accumulators (reset each session)
        private double sumPV;      // Σ(typicalPrice * volume)
        private double sumPV2;     // Σ(typicalPrice^2 * volume)
        private double sumV;       // Σ(volume)

        // Cache
        private double vwap;
        private double sigma;

        // Keep a series reference for VWAP if you use it elsewhere
        private Series<double> vwapSeries;

        // Plot indices
        private const int PlotVWAP = 0;
        private const int PlotUp1  = 1;
        private const int PlotDn1  = 2;
        private const int PlotUp2  = 3;
        private const int PlotDn2  = 4;

        #region User Inputs
        [NinjaScriptProperty]
        [Display(Name = "StdDev (inner band)", Order = 1, GroupName = "Parameters")]
        [Range(0.1, 10.0)]
        public double Deviations1 { get; set; } = 1.0;

        [NinjaScriptProperty]
        [Display(Name = "StdDev (outer band)", Order = 2, GroupName = "Parameters")]
        [Range(0.1, 10.0)]
        public double Deviations2 { get; set; } = 2.0;

        [NinjaScriptProperty]
        [Display(Name = "Show inner band (±1σ)", Order = 3, GroupName = "Parameters")]
        public bool ShowInnerBand { get; set; } = true;  

        [NinjaScriptProperty]
        [Display(Name = "Show outer band (±2σ)", Order = 4, GroupName = "Parameters")]
        public bool ShowSecondBand { get; set; } = true;

        [NinjaScriptProperty]
        [Display(Name = "Reset on new session", Order = 5, GroupName = "Parameters")]
        public bool ResetOnNewSession { get; set; } = true;

        [NinjaScriptProperty]
        [Display(Name = "Use Typical Price (H+L+C)/3", Order = 6, GroupName = "Parameters")]
        public bool UseTypicalPrice { get; set; } = true;
        #endregion

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Name                     = "VWAP_SessionColorLine_Bands";
                Description              = "Session VWAP with auto color + ±1σ / ±2σ volume-weighted bands.";
                Calculate                = Calculate.OnBarClose;      // safe default
                IsOverlay                = true;
                DisplayInDataBox         = true;
                DrawOnPricePanel         = true;
                IsSuspendedWhileInactive = true;

                // Plots (style in UI as you like)
                AddPlot(Brushes.Gray,           "VWAP");     // 0
                AddPlot(Brushes.MediumSeaGreen, "Upper1");   // 1
                AddPlot(Brushes.IndianRed,      "Lower1");   // 2
                AddPlot(Brushes.ForestGreen,    "Upper2");   // 3
                AddPlot(Brushes.Firebrick,      "Lower2");   // 4
            }
            else if (State == State.DataLoaded)
            {
                vwapSeries = new Series<double>(this);
                ResetAccumulators();
            }
        }

        protected override void OnBarUpdate()
        {
            if (CurrentBar < 0)
            {
                SetNaNAll();
                return;
            }

            // Reset once at session start (first tick of the first bar)
            if (ResetOnNewSession && Bars.IsFirstBarOfSession && IsFirstTickOfBar)
                ResetAccumulators();

            // Price input
            double tp  = UseTypicalPrice ? (High[0] + Low[0] + Close[0]) / 3.0 : Close[0];
            double vol = Math.Max(Volume[0], 0);

            // Accumulate
            sumPV  += tp * vol;
            sumPV2 += tp * tp * vol;
            sumV   += vol;

            if (sumV <= double.Epsilon)
            {
                // not enough volume yet
                vwap = tp;
                sigma = 0;
            }
            else
            {
                vwap = sumPV / sumV;
                double ex2 = sumPV2 / sumV;
                double variance = ex2 - (vwap * vwap);
                if (variance < 0) variance = 0; // numeric guard
                sigma = Math.Sqrt(variance);
            }

            // Assign VWAP
            vwapSeries[0]     = vwap;
            Values[PlotVWAP][0] = vwap;

            // Auto color VWAP line
            if (Close[0] > vwap)
                PlotBrushes[PlotVWAP][0] = Brushes.LimeGreen;
            else if (Close[0] < vwap)
                PlotBrushes[PlotVWAP][0] = Brushes.Red;
            else
                PlotBrushes[PlotVWAP][0] = Brushes.Gray;

            // ----- Bands -----
            // Inner ±1σ
            if (ShowInnerBand)
            {
                Values[PlotUp1][0] = vwap + Deviations1 * sigma;
                Values[PlotDn1][0] = vwap - Deviations1 * sigma;
            }
            else
            {
                // Hide from view & scaling
                Values[PlotUp1][0] = double.NaN;
                Values[PlotDn1][0] = double.NaN;
            }

            // Outer ±2σ
            if (ShowSecondBand)
            {
                Values[PlotUp2][0] = vwap + Deviations2 * sigma;
                Values[PlotDn2][0] = vwap - Deviations2 * sigma;
            }
            else
            {
                Values[PlotUp2][0] = double.NaN;
                Values[PlotDn2][0] = double.NaN;
            }
        }

        private void ResetAccumulators()
        {
            sumPV = 0.0;
            sumPV2 = 0.0;
            sumV = 0.0;
            vwap = double.NaN;
            sigma = double.NaN;
            SetNaNAll();   // clears current bar outputs at session open
        }

        private void SetNaNAll()
        {
            Values[PlotVWAP][0] = double.NaN;
            Values[PlotUp1][0]  = double.NaN;
            Values[PlotDn1][0]  = double.NaN;
            Values[PlotUp2][0]  = double.NaN;
            Values[PlotDn2][0]  = double.NaN;
        }

        #region Series accessor (hidden)
        [XmlIgnore, Browsable(false)]
        public Series<double> VWAPSeries => vwapSeries;
        #endregion
    }
}
