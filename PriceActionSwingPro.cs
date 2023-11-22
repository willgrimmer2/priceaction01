// #############################################################
// #														   #
// #                    PriceActionSwingPro                    #
// #														   #
// #     19.12.2016 by dorschden, die.unendlichkeit@gmx.de     #
// #														   #
// #        Thanks and comments are highly appreciated         #
// #        Paypal thanks to "die.unendlichkeit@gmx.de"        #
// #														   #
// #############################################################


#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.DrawingTools;
using PriceActionSwing.Base;
#endregion

//This namespace holds Indicators in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Indicators.PriceActionSwing
{
    /// <summary>
    /// PriceActionSwingPro calculates swings and visualizes them in different ways
    /// and displays several information about the swings. Features: 
    /// ABC pattern recognition | Fibonacci retracements/-extensions | Divergence
    /// </summary>
	public class PriceActionSwingPro : Indicator
	{
        #region Variables
        //#########################################################################################
        #region Display
        //=========================================================================================
        /// <summary>
        /// Represents the swing length visualization type for the swings.
        /// </summary>
        private SwingLengthStyle swingLengthType = SwingLengthStyle.Ticks;
        /// <summary>
        /// Represents the swing duration visualization type for the swings.
        /// </summary>
        private SwingDurationStyle swingDurationType = SwingDurationStyle.Bars;
        /// <summary>
        /// Indicates if the swing price is shown for the swings.
        /// </summary>
        private bool showSwingPrice = true;
        /// <summary>
        /// Indicates if the swing label is shown for the swings.
        /// </summary>
        private bool showSwingLabel = false;
        /// <summary>
        /// Indicates if the swing percentage in relation to the last swing is shown for the 
        /// swings.
        /// </summary>
        private bool showSwingPercent = false;
        /// <summary>
        /// Represents the swing time visualization type for the swings. 
        /// </summary>
        private SwingTimeStyle swingTimeType = SwingTimeStyle.False;
        /// <summary>
        /// Indicates if the swing volume is shown for the swings.
        /// </summary>
        private SwingVolumeStyle swingVolumeType = SwingVolumeStyle.Absolute;
		/// <summary>
		/// Represents the name of the indicator.
		/// </summary>
		private string displayName = null;
        //=========================================================================================
        #endregion

        #region Visualize swings
        //=========================================================================================
        /// <summary>
        /// Represents the swing visualization type for the swings.
        /// </summary>
        private VisualizationStyle visualizationType = VisualizationStyle.Dots_ZigZag;
        /// <summary>
        /// Indicates if AutoScale is used for the swings. 
        /// </summary>
        private bool useAutoScale = true;
        /// <summary>
        /// Indicates if the swings are drawn on the price panel. 
        /// </summary>
        private bool drawSwingsOnPricePanel = true;
        /// <summary>
        /// Represents the color of the zig-zag up lines for the swings.
        /// </summary>
        private Brush zigZagColorUp = Brushes.LimeGreen;
        /// <summary>
        /// Represents the color of the zig-zag down lines for the swings.
        /// </summary>
        private Brush zigZagColorDn = Brushes.OrangeRed;
        /// <summary>
        /// Represents the line style of the zig-zag lines for the swings.
        /// </summary>
        private DashStyleHelper zigZagStyle = DashStyleHelper.Solid;
        /// <summary>
        /// Represents the line width of the zig-zag lines for the swings.
        /// </summary>
        private int zigZagWidth = 3;
        /// <summary>
        /// Represents the color of the swing value output for higher highs for the swings.
        /// </summary>
        private Brush textColorHigherHigh = Brushes.White;
        /// <summary>
        /// Represents the color of the swing value output for lower highs for the swings.
        /// </summary>
        private Brush textColorLowerHigh = Brushes.Black;
        /// <summary>
        /// Represents the color of the swing value output for double tops for the swings.
        /// </summary>
        private Brush textColorDoubleTop = Brushes.Gold;
        /// <summary>
        /// Represents the color of the swing value output for higher lows for the swings.
        /// </summary>
        private Brush textColorHigherLow = Brushes.White;
        /// <summary>
        /// Represents the color of the swing value output for lower lows swings for the swings.
        /// </summary>
        private Brush textColorLowerLow = Brushes.Black;
        /// <summary>
        /// Represents the color of the swing value output for double bottoms for the swings.
        /// </summary>
        private Brush textColorDoubleBottom = Brushes.Gold;
        /// <summary>
        /// Represents the text font for the swing value output for the swings.
        /// </summary>
        private SimpleFont textFont = new SimpleFont("Courier", 15);
        /// <summary>
        /// Represents the text offset in pixel for the swing length for the swings.
        /// </summary>
        private int textOffsetLength = 15;
        /// <summary>
        /// Represents the text offset in pixel for the retracement value for the swings.
        /// </summary>
        private int textOffsetPercent = 90;
        /// <summary>
        /// Represents the text offset in pixel for the swing price for the swings.
        /// </summary>
        private int textOffsetPrice = 45;
        /// <summary>
        /// Represents the text offset in pixel for the swing labels for the swings.
        /// </summary>
        private int textOffsetLabel = 60;
        /// <summary>
        /// Represents the text offset in pixel for the swing time for the swings.
        /// </summary>
        private int textOffsetTime = 75;
        /// <summary>
        /// Represents the text offset in pixel for the swing volume for the swings.
        /// </summary>
        private int textOffsetVolume = 30;

        /// <summary>
        /// Indicates if the Gann swings are updated if the last swing high/low is broken. 
        /// </summary>
        private bool useBreakouts = true;
        /// <summary>
        /// Indicates if inside bars are ignored for the Gann swing calculation. If set to 
        /// true it is possible that between consecutive up/down bars are inside bars.
        /// </summary>
        private bool ignoreInsideBars = true;
        /// <summary>
        /// Represents the number of decimal places for the instrument
        /// </summary>
        private int decimalPlaces;
        /// <summary>
        /// Indicates if the change from down to up or up to down swings is indicated on the chart 
        /// for the swings.
        /// </summary>
        private bool showSwingSwitch = false;
        /// <summary>
        /// Represents the number of ticks the swing switch triangle is placed above/below the bar.
        /// </summary>
        private int swingSwitchOffsetInTicks = 10;
        private Brush swingSwitchDownColor = Brushes.Red;
        private Brush swingSwitchUpColor = Brushes.Blue;
        //=========================================================================================
        #endregion

        #region Features
        //=========================================================================================
        /// <summary>
        /// Indicates if and for which direction AB=CD patterns are computed.
        /// </summary>
        private AbcPatternMode abcPattern = AbcPatternMode.Long_Short;
        /// <summary>
        /// Indicates the target in percent of the AB range for the AB=CD pattern for the risk 
        /// management.
        /// </summary>
        private double abcTarget = 100.0;
        private DivergenceMode divergenceMode = DivergenceMode.False;
        private DivergenceDirection divergenceDirection = DivergenceDirection.Long_Short;
        private int param1 = 10;
        private int param2 = 26;
        private int param3 = 9;
        /// <summary>
        /// Indicates if and where a swing statistic is shown.
        /// </summary>
        private StatisticPositionStyle statisticPosition = StatisticPositionStyle.False;
        /// <summary>
        /// Indicates the number of swings which are used for the current swing statistic.
        /// </summary>
        private int statisticLength = 5;
        /// <summary>
        /// Indicates if and where the risk management tools are shown.
        /// </summary>
        private RiskManagementStyle riskManagementPosition = RiskManagementStyle.Tab;
        /// <summary>
        /// Represents the stop loss in ticks for long entries to calculate the risk parameters.
        /// </summary>
        private int riskLongStopInTicks = 50;
        /// <summary>
        /// Represents the take profit in ticks for long entries to calculate the risk parameters.
        /// </summary>
        private int riskLongTargetInTicks = 100;
        /// <summary>
        /// Represents the stop loss in ticks for short entries to calculate the risk parameters.
        /// </summary>
        private int riskShortStopInTicks = 50;
        /// <summary>
        /// Represents the take profit in ticks for short entries to calculate the risk parameters.
        /// </summary>
        private int riskShortTargetInTicks = 100;
        /// <summary>
        /// Represents the account size from which the quantity for a trade is calculated based
        /// on the account risk.
        /// </summary>
        private double accountSize = 100000.00;
        /// <summary>
        /// Represents the percentage of the account size which is risked for each trade.
        /// </summary>
        private double accountRisk = 0.5;
        /// <summary>
        /// Indicates if a swing extension is drawn on the chart.
        /// </summary>
        private bool addSwingExtension = false;
        /// <summary>
        /// Indicates if a swing retracement is drawn on the chart for the current swing.
        /// </summary>
        private bool addSwingRetracementFast = false;
        /// <summary>
        /// Indicates if a swing retracement is drawn on the chart for the last swing.
        /// </summary>
        private bool addSwingRetracementSlow = false;
        //=========================================================================================
        #endregion

        #region Naked swings
        //=========================================================================================
        private SortedList<double, int> nakedSwingHighsList = new SortedList<double, int>();
        private SortedList<double, int> nakedSwingLowsList = new SortedList<double, int>();
        private bool showNakedSwings = false;
        private bool showHistoricalNakedSwings = false;
        private int nakedSwingCounter = 0;
        private Brush nakedSwingHighColor = Brushes.Red;
        private Brush nakedSwingLowColor = Brushes.Blue;
        private DashStyleHelper nakedSwingDashStyle = DashStyleHelper.Solid;
        private int nakedSwingLineWidth = 1;
        //=========================================================================================
        #endregion

        #region ABC pattern
        //=========================================================================================
        private bool abcLabel = true;
        private DashStyleHelper abcLineStyle = DashStyleHelper.Solid;
        private DashStyleHelper abcLineStyleRatio = DashStyleHelper.Dash;
        private int abcLineWidth = 4;
        private int abcLineWidthRatio = 2;
        private SimpleFont abcTextFont = new SimpleFont("Courier", 14);
        private int abcTextOffsetLabel = 50;
        private Brush abcTextColorDn = Brushes.Red;
        private Brush abcTextColorUp = Brushes.Green;
        private Brush abcZigZagColorDn = Brushes.Red;
        private Brush abcZigZagColorUp = Brushes.Green;
        private double abcMaxRetracement = 92.0;
        private double abcMinRetracement = 61.0;
        private DashStyleHelper entryLineStyle = DashStyleHelper.Solid;
        private int entryLineWidth = 4;
        private Brush entryLineColorDn = Brushes.Red;
        private Brush entryLineColorUp = Brushes.Green;
        private bool showEntryArrows = true;
        private bool showEntryLine = true;
        private bool showHistoricalEntryLine = true;
        private int yTickOffset = 5;
        //=========================================================================================
        private Series<double> abcSignals;
        private Series<double> entryLong;
        private Series<double> entryShort;
        private Series<double> entryLevelLine;
        //=========================================================================================
        private int abcEntryTag = 0;
        private bool abcLongChanceInProgress = false;
        private bool abcShortChanceInProgress = false;
        private double retracementEntryValue = 38.0;
        private double entryLevel = 0.0;
        private int entryLineStartBar = 0;
        private int aBar = 0;
        private int bBar = 0;
        private int cBar = 0;
        private int entryBar = 0;
        private int patternCounter = 0;
        private int tmpCounter = 0;
        private int drawTag = 0;
        //=========================================================================================
        #endregion

        #region Alerts
        //=========================================================================================
        private int alertTag = 0;
        private bool alertAbc = true;
        private bool alertAbcEntry = true;
        private Priority alertAbcPriority = Priority.Medium;
        private Priority alertAbcEntryPriority = Priority.High;
        private string alertAbcLongSoundFileName = "AbcLong.wav";
        private string alertAbcLongEntrySoundFileName = "AbcLongEntry.wav";
        private string alertAbcShortSoundFileName = "AbcShort.wav";
        private string alertAbcShortEntrySoundFileName = "AbcShortEntry.wav";
        //=========================================================================================
        private bool alertDoubleBottom = false;
        private Priority alertDoubleBottomPriority =
            Priority.Medium;
        private string alertDoubleBottomMessage = "Double Bottom";
        private string alertDoubleBottomSoundFileName = "DoubleBottom.wav";
        private int alertDoubleBottomRearmSeconds = 0;
        private Brush alertDoubleBottomBackColor = Brushes.Blue;
        private Brush alertDoubleBottomForeColor = Brushes.White;
        //=========================================================================================
        private bool alertDoubleTop = false;
        private Priority alertDoubleTopPriority =
            Priority.Medium;
        private string alertDoubleTopMessage = "Double Top";
        private string alertDoubleTopSoundFileName = "DoubleTop.wav";
        private int alertDoubleTopRearmSeconds = 0;
        private Brush alertDoubleTopBackColor = Brushes.Red;
        private Brush alertDoubleTopForeColor = Brushes.White;
        //=========================================================================================
        private bool alertHigherLow = false;
        private Priority alertHigherLowPriority =
            Priority.Medium;
        private string alertHigherLowMessage = "Higher Low";
        private string alertHigherLowSoundFileName = "HigherLow.wav";
        private int alertHigherLowRearmSeconds = 0;
        private Brush alertHigherLowBackColor = Brushes.Black;
        private Brush alertHigherLowForeColor = Brushes.White;
        //=========================================================================================
        private bool alertLowerHigh = false;
        private Priority alertLowerHighPriority =
            Priority.Medium;
        private string alertLowerHighMessage = "Lower High";
        private string alertLowerHighSoundFileName = "LowerHigh.wav";
        private int alertLowerHighRearmSeconds = 0;
        private Brush alertLowerHighBackColor = Brushes.Black;
        private Brush alertLowerHighForeColor = Brushes.White;
        //=========================================================================================
        private bool alertSwingChange = false;
        private Priority alertSwingChangePriority =
            Priority.Low;
        private string alertSwingChangeMessage = "Swing change";
        private string alertSwingChangeSoundFileName = "SwingChange.wav";
        private int alertSwingChangeRearmSeconds = 0;
        private Brush alertSwingChangeBackColor = Brushes.Black;
        private Brush alertSwingChangeForeColor = Brushes.White;
        //=========================================================================================
        private bool alertDivergenceRegularHigh = true;
        private Priority alertDivergenceRegularHighPriority =
            Priority.Medium;
        private string alertDivergenceRegularHighMessage = "High regular divergence";
        private string alertDivergenceRegularHighSoundFileName = "DivergenceRegularHigh.wav";
        private int alertDivergenceRegularHighRearmSeconds = 0;
        private Brush alertDivergenceRegularHighBackColor = Brushes.Red;
        private Brush alertDivergenceRegularHighForeColor = Brushes.White;
        //=========================================================================================
        private bool alertDivergenceHiddenHigh = true;
        private Priority alertDivergenceHiddenHighPriority =
            Priority.Medium;
        private string alertDivergenceHiddenHighMessage = "High hidden divergence";
        private string alertDivergenceHiddenHighSoundFileName = "DivergenceHiddenHigh.wav";
        private int alertDivergenceHiddenHighRearmSeconds = 0;
        private Brush alertDivergenceHiddenHighBackColor = Brushes.Red;
        private Brush alertDivergenceHiddenHighForeColor = Brushes.White;
        //=========================================================================================
        private bool alertDivergenceRegularLow = true;
        private Priority alertDivergenceRegularLowPriority =
            Priority.Medium;
        private string alertDivergenceRegularLowMessage = "Low regular divergence";
        private string alertDivergenceRegularLowSoundFileName = "DivergenceRegularLow.wav";
        private int alertDivergenceRegularLowRearmSeconds = 0;
        private Brush alertDivergenceRegularLowBackColor = Brushes.Blue;
        private Brush alertDivergenceRegularLowForeColor = Brushes.White;
        //=========================================================================================
        private bool alertDivergenceHiddenLow = true;
        private Priority alertDivergenceHiddenLowPriority =
            Priority.Medium;
        private string alertDivergenceHiddenLowMessage = "Low hidden divergence";
        private string alertDivergenceHiddenLowSoundFileName = "DivergenceHiddenLow.wav";
        private int alertDivergenceHiddenLowRearmSeconds = 0;
        private Brush alertDivergenceHiddenLowBackColor = Brushes.Blue;
        private Brush alertDivergenceHiddenLowForeColor = Brushes.White;
        //=========================================================================================
        #endregion

        #region Divergence
        //=========================================================================================
        private bool showDivergenceRegular = true;
        private bool showDivergenceHidden = true;
        private Series<double> divergenceDataHigh;
        private Series<double> divergenceDataLow;

        private double divLastSwing = 0.0;
        private double divLastOscValue = 0.0;
        private double divCurSwing = 0.0;
        private double divCurOscValue = 0.0;

        private int drawTagDivUp = 0;
        private int drawTagDivDn = 0;
        private DashStyleHelper divDnLineStyle = DashStyleHelper.Dot;
        private int divDnLineWidth = 2;
        private Brush divDnLineColor = Brushes.Red;
        private DashStyleHelper divUpLineStyle = DashStyleHelper.Dot;
        private int divUpLineWidth = 2;
        private Brush divUpLineColor = Brushes.Green;

        private Stochastics stochastics;
        private MACD macd;
        private Series<double> divSignal;
        private bool divHiddenShortActive = false;
        private bool divRegularShortActive = false;
        private bool divHiddenLongActive = false;
        private bool divRegularLongActive = false;
        //=========================================================================================
        #endregion

        #region Forms
        // Statistic forms ========================================================================
        private System.Windows.Forms.Panel panel = null;
        private System.Windows.Forms.Label label = null;
        private System.Windows.Forms.TabControl mainTabControl = null;
        private System.Windows.Forms.TabPage tabABC = null;
        private System.Windows.Forms.TabPage tabSwingLength = null;
        private System.Windows.Forms.TabPage tabSwingRelation = null;
        private System.Windows.Forms.DataGridView lengthList = null;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLengthDirection = null;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLengthSwingCount = null;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLengthLength = null;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLengthLastLength = null;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLengthDuration = null;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLengthLastDuration = null;
        private System.Windows.Forms.DataGridView relationList = null;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRelationSwing = null;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRelationSwingCount = null;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRelationHigherHigh = null;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRelationLowerHigh = null;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRelationHigherLow = null;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRelationLowerLow = null;
        private System.Windows.Forms.Splitter splitter = null;
        // Necessary toolstrip items forms ========================================================
        System.Windows.Forms.ToolStrip toolStrip = null;
        System.Windows.Forms.ToolStripButton toolStripButton = null;
        System.Windows.Forms.ToolStripSeparator toolStripSeparator = null;
        // Risk calculation forms toolstrip =======================================================
        System.Windows.Forms.ToolStripSeparator toolStripSeparator1 = null;
        System.Windows.Forms.ToolStripSeparator toolStripSeparator2 = null;
        System.Windows.Forms.ToolStripSeparator toolStripSeparator3 = null;
        System.Windows.Forms.ToolStripSeparator toolStripSeparator4 = null;
        System.Windows.Forms.ToolStripLabel toolStripLabelQty = null;
        System.Windows.Forms.ToolStripLabel toolStripLabelRiskReward = null;
        System.Windows.Forms.ToolStripLabel toolStripLabelLoss = null;
        System.Windows.Forms.ToolStripLabel toolStripLabelProfit = null;
        System.Windows.Forms.ToolStripLabel toolStripLabelEntry = null;
        System.Windows.Forms.ToolStripButton toolStripButtonLong = null;
        System.Windows.Forms.ToolStripButton toolStripButtonShort = null;
        System.Windows.Forms.ToolStripLabel toolStripLabelStop = null;
        System.Windows.Forms.ToolStripLabel toolStripLabelTarget = null;
        System.Windows.Forms.NumericUpDown toolStripNumericUpDownEntry;
        System.Windows.Forms.NumericUpDown toolStripNumericUpDownStop;
        System.Windows.Forms.NumericUpDown toolStripNumericUpDownTarget;
        System.Windows.Forms.ToolStripControlHost toolStripControlHostEntry = null;
        System.Windows.Forms.ToolStripControlHost toolStripControlHostStop = null;
        System.Windows.Forms.ToolStripControlHost toolStripControlHostTarget = null;
        // Risk calculation forms tab =============================================================
        private System.Windows.Forms.TabPage tabRiskManagement = null;
        private System.Windows.Forms.Label labelQuantity = null;
        private System.Windows.Forms.Label labelRiskReward = null;
        private System.Windows.Forms.Label labelLoss = null;
        private System.Windows.Forms.Label labelWin = null;
        private System.Windows.Forms.Label labelLossValue = null;
        private System.Windows.Forms.Label labelWinValue = null;
        private System.Windows.Forms.Label labelEntry = null;
        private System.Windows.Forms.Label labelStopLoss = null;
        private System.Windows.Forms.Label labelTakeProfit = null;
        private System.Windows.Forms.Button buttonEntryLong = null;
        private System.Windows.Forms.Button buttonEntryShort = null;
        private System.Windows.Forms.NumericUpDown numericUpDownEntry = null;
        private System.Windows.Forms.NumericUpDown numericUpDownStopLoss = null;
        private System.Windows.Forms.NumericUpDown numericUpDownTakeProfit = null;
        //=========================================================================================
        #endregion

        #region Statistic
        //===================================================================
        private double overallAvgDnLength = 0;
        private double overallAvgUpLength = 0;
        private double overallUpLength = 0;
        private double overallDnLength = 0;
        private double overallAvgDnDuration = 0;
        private double overallAvgUpDuration = 0;
        private double overallUpDuration = 0;
        private double overallDnDuration = 0;

        private double avgUpLength = 0;
        private double avgDnLength = 0;
        private double upLength = 0;
        private double dnLength = 0;
        private double avgUpDuration = 0;
        private double avgDnDuration = 0;
        private double upDuration = 0;
        private double dnDuration = 0;

        // Variables for the swing to swing relation statistic ================
        private int hhCount = 0;
        private int hhCountHH = 0;
        private double hhCountHHPercent = 0;
        private int hhCountLH = 0;
        private double hhCountLHPercent = 0;
        private int hhCountHL = 0;
        private double hhCountHLPercent = 0;
        private int hhCountLL = 0;
        private double hhCountLLPercent = 0;

        private int lhCount = 0;
        private int lhCountHH = 0;
        private double lhCountHHPercent = 0;
        private int lhCountLH = 0;
        private double lhCountLHPercent = 0;
        private int lhCountHL = 0;
        private double lhCountHLPercent = 0;
        private int lhCountLL = 0;
        private double lhCountLLPercent = 0;

        private int dtCount = 0;
        private int dtCountHH = 0;
        private double dtCountHHPercent = 0;
        private int dtCountLH = 0;
        private double dtCountLHPercent = 0;
        private int dtCountHL = 0;
        private double dtCountHLPercent = 0;
        private int dtCountLL = 0;
        private double dtCountLLPercent = 0;

        private int llCount = 0;
        private int llCountHH = 0;
        private double llCountHHPercent = 0;
        private int llCountLH = 0;
        private double llCountLHPercent = 0;
        private int llCountHL = 0;
        private double llCountHLPercent = 0;
        private int llCountLL = 0;
        private double llCountLLPercent = 0;

        private int hlCount = 0;
        private int hlCountHH = 0;
        private double hlCountHHPercent = 0;
        private int hlCountLH = 0;
        private double hlCountLHPercent = 0;
        private int hlCountHL = 0;
        private double hlCountHLPercent = 0;
        private int hlCountLL = 0;
        private double hlCountLLPercent = 0;

        private int dbCount = 0;
        private int dbCountHH = 0;
        private double dbCountHHPercent = 0;
        private int dbCountLH = 0;
        private double dbCountLHPercent = 0;
        private int dbCountHL = 0;
        private double dbCountHLPercent = 0;
        private int dbCountLL = 0;
        private double dbCountLLPercent = 0;
        //===================================================================
        #endregion

        #region Class objects and DataSeries
        //=========================================================================================
        /// <summary>
        /// Represents the properties for the swing.
        /// </summary>
        private SwingProperties swingProperties;
        /// <summary>
        /// Represents the values for the current swing.
        /// </summary>
        private SwingCurrent swingCurrent = new SwingCurrent();
        /// <summary>
        /// Represents the swing high values.
        /// </summary>
        private Swings swingHigh = new Swings();
        /// <summary>
        /// Represents the swing low values.
        /// </summary>
        private Swings swingLow = new Swings();
        /// <summary>
        /// Indicates if the swing direction changed form down to up swing for the swings.
        /// </summary>
        private Series<bool> upFlip;
        /// <summary>
        /// Indicates if the swing direction changed form up to down swing for the swings.
        /// </summary>
        private Series<bool> dnFlip;
        /// <summary>
        /// Represents a list of all up swings for the swings.
        /// </summary>
        private List<SwingStruct> swingHighs = new List<SwingStruct>();
        /// <summary>
        /// Represents a list of all down swings for the swings.
        /// </summary>
        private List<SwingStruct> swingLows = new List<SwingStruct>();
        //=========================================================================================
        #endregion
        //#########################################################################################
		#endregion
		
        #region OnStateChange()
        //=========================================================================================
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description							= @"PriceActionSwingPro calculates swings and visualize them in different ways and display several information about them.";
				Name								= "PriceActionSwingPro";
				Calculate							= Calculate.OnBarClose;
				IsOverlay							= true;
				DisplayInDataBox					= true;
				DrawOnPricePanel					= true;
				DrawHorizontalGridLines				= true;
				DrawVerticalGridLines				= true;
				PaintPriceMarkers					= true;
				ScaleJustification					= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive			= true;
				
		        #region Parameters
		        //=========================================================================================
				DtbStrength = 20;
				SwingSize = 7;
				SwingType = SwingStyle.Standard;
				UseCloseValues = false;	
		        //=========================================================================================
		        #endregion	
				
				AddPlot(new Stroke(Brushes.Gold, DashStyleHelper.Solid, 3), PlotStyle.Dot, "DoubleBottom");
				AddPlot(new Stroke(Brushes.Red, DashStyleHelper.Solid, 3), PlotStyle.Dot, "LowerLow");
				AddPlot(new Stroke(Brushes.Green, DashStyleHelper.Solid, 3), PlotStyle.Dot, "HigherLow");

				AddPlot(new Stroke(Brushes.Gold, DashStyleHelper.Solid, 3), PlotStyle.Dot, "DoubleTop");
				AddPlot(new Stroke(Brushes.Red, DashStyleHelper.Solid, 3), PlotStyle.Dot, "LowerHigh");
				AddPlot(new Stroke(Brushes.Green, DashStyleHelper.Solid, 3), PlotStyle.Dot, "HigherHigh");
				
				AddPlot(new Stroke(Brushes.Blue, DashStyleHelper.Solid, 1), PlotStyle.Square, "GannSwing");
			}
			else if (State == State.DataLoaded)
			{
	            // Calculate decimal places
	            decimal increment = Convert.ToDecimal(Instrument.MasterInstrument.TickSize);
	            int incrementLength = increment.ToString().Length;
	            decimalPlaces = 0;
	            if (incrementLength == 1)
	                decimalPlaces = 0;
	            else if (incrementLength > 2)
	                decimalPlaces = incrementLength - 2;
				
				if (SwingType != SwingStyle.Percent)
				{
					SwingSize = SwingSize < 1 ? 1 : Math.Round(SwingSize, MidpointRounding.AwayFromZero);
				}
				
				displayName = Name +  "(" + Instrument.FullName + " (" + BarsPeriod.Value + " " + BarsPeriod.BarsPeriodType + "), " + SwingType + ", " + SwingSize + ", " + DtbStrength + ")";

	            swingProperties = new SwingProperties(SwingType, SwingSize, DtbStrength,
	                swingLengthType, swingDurationType, showSwingPrice, showSwingLabel,
	                showSwingPercent, swingTimeType, swingVolumeType, visualizationType,
	                useBreakouts, ignoreInsideBars, useAutoScale,
	                zigZagColorUp, zigZagColorDn, zigZagStyle, zigZagWidth, textColorHigherHigh,
	                textColorLowerHigh, textColorDoubleTop, textColorHigherLow,
	                textColorLowerLow, textColorDoubleBottom, textFont, textOffsetLength, 
	                textOffsetPercent, textOffsetPrice, textOffsetLabel, textOffsetTime, 
	                textOffsetVolume, UseCloseValues, drawSwingsOnPricePanel);
			}
			else if (State == State.Configure)
			{
				dnFlip = new Series<bool>(this);
				upFlip = new Series<bool>(this);
				
				abcSignals = new Series<double>(this);
				entryLong = new Series<double>(this);
				entryShort = new Series<double>(this);
				entryLevelLine = new Series<double>(this);
				divergenceDataHigh = new Series<double>(this);
				divergenceDataLow = new Series<double>(this);
				divSignal = new Series<double>(this);
				
				#region Divergence
	            //=====================================================================================
	            if (divergenceMode != DivergenceMode.False)
	            {
	                switch (divergenceMode)
	                {
	                    case DivergenceMode.Custom:
	                        // Add custom divergence indicator here
	                        break;
	                    case DivergenceMode.MACD:
	                        macd = MACD(param1, param2, param3);
	                        break;
	                    case DivergenceMode.Stochastics:
	                        stochastics = Stochastics(param1, param2, param3);
	                        break;
	                }
	            }
	            //=====================================================================================
	            #endregion
			}
		}
        //=========================================================================================
		#endregion

		protected override void OnBarUpdate()
		{
            #region Initialize varibles
            //=====================================================================================
            if (IsFirstTickOfBar)
            {
                entryLevelLine[0] = 0;
                abcSignals[0] = 0;

                if (divHiddenShortActive == true)
                    divSignal[0] = 2;
                else if (divRegularShortActive == true)
                    divSignal[0] = 1;
                else if (divHiddenLongActive == true)
                    divSignal[0] = 2;
                else if (divRegularLongActive == true)
                    divSignal[0] = 1;
                else
                    divSignal[0] = 0;

                if (swingLow.CurPrice == 0.0 || swingHigh.CurPrice == 0.0)
                {
                    entryLong[0] = 0;
                    entryShort[0] = 0;
                }
                else
                {
                    entryLong[0] = entryLong[1];
                    entryShort[0] = entryShort[1];
                }

                if (CurrentBar == 1)
                {
                    #region Divergence
                    switch (divergenceMode)
                    {
                        case DivergenceMode.Custom:
                            // Add custom divergence indicator here
                            break;
                        case DivergenceMode.MACD:
                            divergenceDataHigh = macd.Diff;
                            divergenceDataLow = macd.Diff;
                            break;
                        case DivergenceMode.Stochastics:
                            divergenceDataHigh = stochastics.K;
                            divergenceDataLow = stochastics.K;
                            break;
                    }
                    #endregion
                }
            }
            // Checks to ensure there are enough bars before beginning
            if (CurrentBars[BarsInProgress] <= 1 
                || CurrentBars[BarsInProgress] < SwingSize
				|| (divergenceMode != DivergenceMode.False && 
					(CurrentBars[BarsInProgress] <= param1 
					|| CurrentBars[BarsInProgress] <= param2
					|| CurrentBars[BarsInProgress] <= param3)))
                return;
            //=====================================================================================
            #endregion

            #region Swing calculation
            //=====================================================================================
            InitializeSwingCalculation(swingHigh, swingLow, swingCurrent, upFlip, swingHighs, 
                dnFlip, swingLows);

            switch (SwingType)
	        {
                case SwingStyle.Standard:
                    CalculateSwingStandard(swingHigh, swingLow, swingCurrent, swingProperties,
                        upFlip, swingHighs, dnFlip, swingLows, decimalPlaces, UseCloseValues,
                        DoubleBottom, LowerLow, HigherLow, DoubleTop, LowerHigh, HigherHigh, 
                        GannSwing);
                    break;
                case SwingStyle.Gann:
                    CalculateSwingGann(swingHigh, swingLow, swingCurrent, swingProperties, upFlip,
                        swingHighs, dnFlip, swingLows, decimalPlaces, DoubleBottom, LowerLow, 
                        HigherLow, DoubleTop, LowerHigh, HigherHigh, GannSwing);
                    break;
                case SwingStyle.Ticks:
                    CalculateSwingTicks(swingHigh, swingLow, swingCurrent, swingProperties,
                        upFlip, swingHighs, dnFlip, swingLows, decimalPlaces, UseCloseValues,
                        DoubleBottom, LowerLow, HigherLow, DoubleTop, LowerHigh, HigherHigh,
                        GannSwing);
                    break;
                case SwingStyle.Percent:
                    CalculateSwingPercent(swingHigh, swingLow, swingCurrent, swingProperties,
                        upFlip, swingHighs, dnFlip, swingLows, decimalPlaces, UseCloseValues,
                        DoubleBottom, LowerLow, HigherLow, DoubleTop, LowerHigh, HigherHigh,
                        GannSwing);
                    break;
            }
            //=====================================================================================
            #endregion

            #region Fibonacci tools
            //=====================================================================================
            if (swingHigh.New || swingLow.New || swingHigh.Update || swingLow.Update)
            {
                #region Fibonacci extensions
                //---------------------------------------------------------------------------------
                if (addSwingExtension)
                {
                    if (swingHigh.LastPrice == 0.0 || swingLow.LastPrice == 0.0)
                        return;

                    if (swingLows[swingLows.Count - 1].relation == 1 
                        && swingCurrent.SwingSlope == -1)
                    {
                        int anchor1BarsAgo = CurrentBar - swingLow.LastBar;
                        int anchor2BarsAgo = CurrentBar - swingHigh.CurBar;
                        int anchor3BarsAgo = CurrentBar - swingLow.CurBar;
                        double anchor1Y = swingLow.LastPrice;
                        double anchor2Y = swingHigh.CurPrice;
                        double anchor3Y = swingLow.CurPrice;
						Draw.FibonacciExtensions(this, "FibExtUp", false, anchor1BarsAgo, anchor1Y,
                            anchor2BarsAgo, anchor2Y, anchor3BarsAgo, anchor3Y);
                    }
                    else if (swingLows[swingLows.Count - 1].relation == 1 
                        && swingCurrent.SwingSlope == 1)
                    {
                        int anchor1BarsAgo = CurrentBar - swingLow.LastBar;
                        int anchor2BarsAgo = CurrentBar - swingHigh.LastBar;
                        int anchor3BarsAgo = CurrentBar - swingLow.CurBar;
                        double anchor1Y = swingLow.LastPrice;
                        double anchor2Y = swingHigh.LastPrice;
                        double anchor3Y = swingLow.CurPrice;
                        Draw.FibonacciExtensions(this,"FibExtUp", false, anchor1BarsAgo, anchor1Y,
                            anchor2BarsAgo, anchor2Y, anchor3BarsAgo, anchor3Y);
                    }
                    else
                        RemoveDrawObject("FibExtUp");

                    if (swingHighs[swingHighs.Count - 1].relation == -1 
                        && swingCurrent.SwingSlope == 1)
                    {
                        int anchor1BarsAgo = CurrentBar - swingHigh.LastBar;
                        int anchor2BarsAgo = CurrentBar - swingLow.CurBar;
                        int anchor3BarsAgo = CurrentBar - swingHigh.CurBar;
                        double anchor1Y = swingHigh.LastPrice;
                        double anchor2Y = swingLow.CurPrice;
                        double anchor3Y = swingHigh.CurPrice;
                        Draw.FibonacciExtensions(this,"FibExtDn", false, anchor1BarsAgo, anchor1Y,
                            anchor2BarsAgo, anchor2Y, anchor3BarsAgo, anchor3Y);
                    }
                    else if (swingHighs[swingHighs.Count - 1].relation == -1 
                        && swingCurrent.SwingSlope == -1)
                    {
                        int anchor1BarsAgo = CurrentBar - swingHigh.LastBar;
                        int anchor2BarsAgo = CurrentBar - swingLow.LastBar;
                        int anchor3BarsAgo = CurrentBar - swingHigh.CurBar;
                        double anchor1Y = swingHigh.LastPrice;
                        double anchor2Y = swingLow.LastPrice;
                        double anchor3Y = swingHigh.CurPrice;
                        Draw.FibonacciExtensions(this,"FibExtDn", false, anchor1BarsAgo, anchor1Y,
                            anchor2BarsAgo, anchor2Y, anchor3BarsAgo, anchor3Y);
                    }
                    else
                        RemoveDrawObject("FibExtDn");
                }
                //---------------------------------------------------------------------------------
                #endregion

                #region Fibonacci retracements
                //---------------------------------------------------------------------------------
                if (addSwingRetracementFast)
                {
                    int anchor1BarsAgo = 0;
                    int anchor2BarsAgo = 0;
                    double anchor1Y = 0.0;
                    double anchor2Y = 0.0;

                    if (swingCurrent.SwingSlope == 1)
                    {
                        anchor1BarsAgo = CurrentBar - swingLow.CurBar;
                        anchor1Y = swingLow.CurPrice;
                        anchor2BarsAgo = CurrentBar - swingHigh.CurBar;
                        anchor2Y = swingHigh.CurPrice;
                    }
                    else
                    {
                        anchor1BarsAgo = CurrentBar - swingHigh.CurBar;
                        anchor1Y = swingHigh.CurPrice;
                        anchor2BarsAgo = CurrentBar - swingLow.CurBar;
                        anchor2Y = swingLow.CurPrice;
                    }
                    Draw.FibonacciRetracements(this, "FastFibRet", IsAutoScale,
                        anchor1BarsAgo, anchor1Y, anchor2BarsAgo, anchor2Y);
                }

                if (addSwingRetracementSlow)
                {
                    if (swingHigh.LastPrice == 0.0 || swingLow.LastPrice == 0.0) return;

                    int anchor1BarsAgo = 0;
                    int anchor2BarsAgo = 0;
                    double anchor1Y = 0.0;
                    double anchor2Y = 0.0;

                    if (swingCurrent.SwingSlope == 1)
                    {
                        anchor1BarsAgo = CurrentBar - swingHigh.LastBar;
                        anchor1Y = swingHigh.LastPrice;
                        anchor2BarsAgo = CurrentBar - swingLow.CurBar;
                        anchor2Y = swingLow.CurPrice;
                    }
                    else
                    {
                        anchor1BarsAgo = CurrentBar - swingLow.LastBar;
                        anchor1Y = swingLow.LastPrice;
                        anchor2BarsAgo = CurrentBar - swingHigh.CurBar;
                        anchor2Y = swingHigh.CurPrice;
                    }

                    if ((swingCurrent.SwingSlope == 1 && swingHigh.CurPrice < swingHigh.LastPrice) 
                        || (swingCurrent.SwingSlope == -1 
                        && swingLow.CurPrice > swingLow.LastPrice))
                        Draw.FibonacciRetracements(this, "SlowFibRet", IsAutoScale, 
                            anchor1BarsAgo, anchor1Y, anchor2BarsAgo, anchor2Y);
                    else
                        RemoveDrawObject("SlowFibRet");
                }
                //---------------------------------------------------------------------------------
                #endregion
            }
            //=====================================================================================
            #endregion

            #region Swing statistic
            //=====================================================================================
            if (statisticPosition != StatisticPositionStyle.False)
            {
                if (swingLow.New == true && swingLow.Update == false)
                    UpStatistic();
                if (swingHigh.New == true && swingHigh.Update == false)
                    DownStatistic();
            }
            //=====================================================================================
            #endregion

            #region ABC pattern
            //=====================================================================================
            #region ABC long pattern
            //-------------------------------------------------------------------------------------
            if (abcPattern == AbcPatternMode.Long_Short || abcPattern == AbcPatternMode.Long)
            {
                if ((swingLow.Update || swingLow.New) && !abcLongChanceInProgress
                        && swingLow.LastRelation == -1 && swingLow.CurRelation == 1
                        && swingLow.CurPercent > abcMinRetracement && swingLow.CurPercent < abcMaxRetracement)
                {
                    drawTag = swingHigh.Counter;
                    abcLongChanceInProgress = true;
                    entryLineStartBar = CurrentBar;
                    patternCounter++;
                    tmpCounter = swingLow.Counter;
                    abcSignals[0] = 1;
                    aBar = CurrentBar - swingLow.LastBar;
                    bBar = CurrentBar - swingHigh.CurBar;
                    cBar = CurrentBar - swingLow.CurBar;

                    Draw.Line(this, "ABLineUp" + drawTag, IsAutoScale, aBar, swingLow.LastPrice,
                        bBar, swingHigh.CurPrice, abcZigZagColorUp, abcLineStyle, abcLineWidth);
                    Draw.Line(this, "BCLineUp" + drawTag, IsAutoScale, bBar, swingHigh.CurPrice,
                        cBar, swingLow.CurPrice, abcZigZagColorUp, abcLineStyle, abcLineWidth);
                    Draw.Line(this, "ACLineUp" + drawTag, IsAutoScale, aBar, swingLow.LastPrice,
                        cBar, swingLow.CurPrice, abcZigZagColorUp, abcLineStyleRatio, abcLineWidthRatio);
                    if (abcLabel)
                    {
                        Draw.Text(this, "AUp" + drawTag, IsAutoScale, "A", aBar, swingLow.LastPrice,
                            -abcTextOffsetLabel, abcTextColorUp, abcTextFont,
                            TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
                        Draw.Text(this, "BUp" + drawTag, IsAutoScale, "B", bBar, swingHigh.CurPrice,
                            abcTextOffsetLabel, abcTextColorUp, abcTextFont,
                            TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
                        Draw.Text(this, "CUp" + drawTag, IsAutoScale, "C", cBar, swingLow.CurPrice,
                            -abcTextOffsetLabel, abcTextColorUp, abcTextFont,
                            TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
                    }

                    entryLevel = swingLow.CurPrice + Instrument.MasterInstrument.RoundToTickSize((Math.Abs(swingLow.CurLength) * TickSize)
                        / 100 * retracementEntryValue);
                    entryLevelLine[0] = entryLevel;
                    if (showEntryLine)
                        Draw.Line(this, "EntryLine" + CurrentBar.ToString(), IsAutoScale, 1, entryLevelLine[0], 0, entryLevelLine[0],
                            entryLineColorUp, entryLineStyle, entryLineWidth);
                    if (alertAbc)
                        Alert("Alert_Abc_Long" + alertTag++.ToString(), alertAbcPriority, "ABC Long" + " ("
                            + Instrument.FullName + " " + BarsPeriod + ")", alertAbcLongSoundFileName, 0, Brushes.White, Brushes.Blue);
                    entryLong[0] = entryLevel;
                    if (riskManagementPosition != RiskManagementStyle.False)
                    {
                        double entryRM = entryLong[0];
                        double stopRM = swingLow.CurPrice - 1 * TickSize;
                        double target100RM = 0.0;
                        if (swingCurrent.SwingSlope == 1)
                            target100RM = swingLow.CurPrice + swingHigh.LastLength * TickSize * abcTarget / 100;
                        else
                            target100RM = swingLow.CurPrice + swingHigh.CurLength * TickSize * abcTarget / 100;
                        CalculateLongRisk(entryRM, stopRM, target100RM);
                    }
                }

                if (abcLongChanceInProgress)
                {
                    if (swingLow.CurPercent > abcMaxRetracement && tmpCounter == swingLow.Counter)
                    {
                        abcLongChanceInProgress = false;
                        RemoveDrawObject("ABLineUp" + drawTag.ToString());
                        RemoveDrawObject("BCLineUp" + drawTag.ToString());
                        RemoveDrawObject("ACLineUp" + drawTag.ToString());
                        RemoveDrawObject("AUp" + drawTag.ToString());
                        RemoveDrawObject("BUp" + drawTag.ToString());
                        RemoveDrawObject("CUp" + drawTag.ToString());
                        // Remove entryLevelLine (maybe remove more objects as drawn (COBC))
                        if (!showHistoricalEntryLine)
                        {
                            for (int index = 0; index < CurrentBar - entryLineStartBar + 1; index++)
                            {
                                RemoveDrawObject("EntryLine" + (CurrentBar - index).ToString());
                            }
                            entryLineStartBar = 0;
                        }
                    }
                    else if (dnFlip[0] && tmpCounter != swingLow.Counter)
                    {
                        abcLongChanceInProgress = false;
                        entryLineStartBar = 0;
                    }
                    else
                    {
                        if (swingLow.Update && tmpCounter == swingLow.Counter)
                        {
                            aBar = CurrentBar - swingLow.LastBar;
                            bBar = CurrentBar - swingHigh.CurBar;
                            cBar = CurrentBar - swingLow.CurBar;

                            Draw.Line(this, "BCLineUp" + drawTag, IsAutoScale, bBar, swingHigh.CurPrice,
                                cBar, swingLow.CurPrice, abcZigZagColorUp, abcLineStyle, abcLineWidth);
                            Draw.Line(this, "ACLineUp" + drawTag, IsAutoScale, aBar, swingLow.LastPrice,
                                cBar, swingLow.CurPrice, abcZigZagColorUp, abcLineStyleRatio, abcLineWidthRatio);
                            if (abcLabel)
                            {
                                Draw.Text(this, "CUp" + drawTag, IsAutoScale, "C", cBar, swingLow.CurPrice,
                                    -abcTextOffsetLabel, abcTextColorUp, abcTextFont,
                                    TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
                            }

                            entryLevel = swingLow.CurPrice + Instrument.MasterInstrument.RoundToTickSize((Math.Abs(swingLow.CurLength) * TickSize)
                                / 100 * retracementEntryValue);
                            entryLevelLine[0] = entryLevel;

                            if (showEntryLine)
                            {
                                if (entryLevelLine[1] == 0)
                                    Draw.Line(this, "EntryLine" + CurrentBar.ToString(), IsAutoScale, 1, entryLevelLine[0], 0, entryLevelLine[0],
                                        entryLineColorUp, entryLineStyle, entryLineWidth);
                                else
                                    Draw.Line(this, "EntryLine" + CurrentBar.ToString(), IsAutoScale, 1, entryLevelLine[1], 0, entryLevelLine[0],
                                        entryLineColorUp, entryLineStyle, entryLineWidth);
                            }
                            entryLong[0] = entryLevel;
                            if (riskManagementPosition != RiskManagementStyle.False)
                            {
                                double entryRM = entryLong[0];
                                double stopRM = swingLow.CurPrice - 1 * TickSize;
                                double target100RM = 0.0;
                                if (swingCurrent.SwingSlope == 1)
                                    target100RM = swingLow.CurPrice + swingHigh.LastLength * TickSize * abcTarget / 100;
                                else
                                    target100RM = swingLow.CurPrice + swingHigh.CurLength * TickSize * abcTarget / 100;
                                CalculateLongRisk(entryRM, stopRM, target100RM);
                            }
                        }
                        else if (IsFirstTickOfBar)
                        {
                            entryLevelLine[0] = entryLevel;

                            if (showEntryLine)
                            {
                                if (entryLevelLine[1] == 0)
                                    Draw.Line(this, "EntryLine" + CurrentBar.ToString(), IsAutoScale, 1, entryLevelLine[0], 0, entryLevelLine[0],
                                        entryLineColorUp, entryLineStyle, entryLineWidth);
                                else
                                    Draw.Line(this, "EntryLine" + CurrentBar.ToString(), IsAutoScale, 1, entryLevelLine[1], 0, entryLevelLine[0],
                                        entryLineColorUp, entryLineStyle, entryLineWidth);
                            }
                        }
                        abcSignals[0] = 1;

                        bool abcLong = false;
                        if (Calculate == Calculate.OnBarClose || State == State.Historical)
                        {
                            if (Close[0] > entryLevel)
                            {
                                entryLong[0] = Close[0];
                                abcLong = true;
                            }
                        }
                        else
                        {
                            if (IsFirstTickOfBar && Open[0] > entryLevel)
                            {
                                entryLong[0] = Open[0];
                                abcLong = true;
                            }
                        }

                        if (abcLong)
                        {
                            if (showEntryArrows)
                                Draw.ArrowUp(this, "AbcUp" + abcEntryTag++.ToString(), IsAutoScale, 0,
                                    Low[0] - yTickOffset * TickSize, abcTextColorUp);
                            abcLongChanceInProgress = false;
                            entryLineStartBar = 0;
                            entryBar = CurrentBar;
                            abcSignals[0] = 2;
                            if (riskManagementPosition != RiskManagementStyle.False)
                            {
                                double entryRM = entryLong[0];
                                double stopRM = swingLow.CurPrice - 1 * TickSize;
                                double target100RM = 0.0;
                                if (swingCurrent.SwingSlope == 1)
                                    target100RM = swingLow.CurPrice + swingHigh.LastLength * TickSize * abcTarget / 100;
                                else
                                    target100RM = swingLow.CurPrice + swingHigh.CurLength * TickSize * abcTarget / 100;
                                CalculateLongRisk(entryRM, stopRM, target100RM);
                            }
                            if (alertAbcEntry)
                                Alert("Alert_Abc_Long_Entry" + alertTag++.ToString(), alertAbcEntryPriority, "ABC Long Entry" + " ("
                                    + Instrument.FullName + " " + BarsPeriod + ")", alertAbcLongEntrySoundFileName, 0, Brushes.Blue, Brushes.White);
                        }
                    }
                }
            }
            //-------------------------------------------------------------------------------------
            #endregion

            #region ABC short pattern
            //-------------------------------------------------------------------------------------
            if (abcPattern == AbcPatternMode.Long_Short || abcPattern == AbcPatternMode.Short)
            {
                if ((swingHigh.Update || swingHigh.New) && !abcShortChanceInProgress
                        && swingHigh.LastRelation == 1 && swingHigh.CurRelation == -1
                        && swingHigh.CurPercent > abcMinRetracement && swingHigh.CurPercent < abcMaxRetracement)
                {
                    drawTag = swingLow.Counter;
                    abcShortChanceInProgress = true;
                    entryLineStartBar = CurrentBar;
                    patternCounter++;
                    tmpCounter = swingHigh.Counter;
                    abcSignals[0] = -1;

                    aBar = CurrentBar - swingHigh.LastBar;
                    bBar = CurrentBar - swingLow.CurBar;
                    cBar = CurrentBar - swingHigh.CurBar;

                    Draw.Line(this, "ABLineDn" + drawTag, IsAutoScale, aBar, swingHigh.LastPrice,
                        bBar, swingLow.CurPrice, abcZigZagColorDn, abcLineStyle, abcLineWidth);
                    Draw.Line(this, "BCLineDn" + drawTag, IsAutoScale, bBar, swingLow.CurPrice,
                        cBar, swingHigh.CurPrice, abcZigZagColorDn, abcLineStyle, abcLineWidth);
                    Draw.Line(this, "ACLineDn" + drawTag, IsAutoScale, aBar, swingHigh.LastPrice,
                        cBar, swingHigh.CurPrice, abcZigZagColorDn, abcLineStyleRatio, abcLineWidthRatio);
                    if (abcLabel)
                    {
                        Draw.Text(this, "ADn" + drawTag, IsAutoScale, "A", aBar, swingHigh.LastPrice,
                            abcTextOffsetLabel, abcTextColorDn, abcTextFont,
                            TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
                        Draw.Text(this, "BDn" + drawTag, IsAutoScale, "B", bBar, swingLow.CurPrice,
                            -abcTextOffsetLabel, abcTextColorDn, abcTextFont,
                            TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
                        Draw.Text(this, "CDn" + drawTag, IsAutoScale, "C", cBar, swingHigh.CurPrice,
                            abcTextOffsetLabel, abcTextColorDn, abcTextFont,
                            TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
                    }

                    entryLevel = swingHigh.CurPrice - Instrument.MasterInstrument.RoundToTickSize((swingHigh.CurLength * TickSize)
                        / 100 * retracementEntryValue);
                    entryLevelLine[0] = entryLevel;
                    if (showEntryLine)
                        Draw.Line(this, "EntryLine" + CurrentBar.ToString(), IsAutoScale, 1, entryLevelLine[0], 0, entryLevelLine[0],
                            entryLineColorDn, entryLineStyle, entryLineWidth);
                    if (alertAbc)
                        Alert("Alert_Abc_Short" + alertTag++.ToString(), alertAbcPriority, "ABC Short" + " ("
                            + Instrument.FullName + " " + BarsPeriod + ")", alertAbcShortSoundFileName, 0, Brushes.White, Brushes.Red);
                    entryShort[0] = entryLevel;
                    if (riskManagementPosition != RiskManagementStyle.False)
                    {
                        double entryRM = entryShort[0];
                        double stopRM = swingHigh.CurPrice + 1 * TickSize;
                        double target100RM = 0.0;
                        if (swingCurrent.SwingSlope == -1)
                            target100RM = swingHigh.CurPrice + swingLow.LastLength * TickSize * abcTarget / 100;
                        else
                            target100RM = swingHigh.CurPrice + swingLow.CurLength * TickSize * abcTarget / 100;
                        CalculateShortRisk(entryRM, stopRM, target100RM);
                    }
                }

                if (abcShortChanceInProgress)
                {
                    if (swingHigh.CurPercent > abcMaxRetracement && tmpCounter == swingHigh.Counter)
                    {
                        abcShortChanceInProgress = false;
                        RemoveDrawObject("ABLineDn" + drawTag.ToString());
                        RemoveDrawObject("BCLineDn" + drawTag.ToString());
                        RemoveDrawObject("ACLineDn" + drawTag.ToString());
                        RemoveDrawObject("ADn" + drawTag.ToString());
                        RemoveDrawObject("BDn" + drawTag.ToString());
                        RemoveDrawObject("CDn" + drawTag.ToString());
                        // Remove entryLevelLine (maybe remove more objects as drawn (COBC))
                        if (!showHistoricalEntryLine)
                        {
                            for (int index = 0; index < CurrentBar - entryLineStartBar + 1; index++)
                            {
                                RemoveDrawObject("EntryLine" + (CurrentBar - index).ToString());
                            }
                            entryLineStartBar = 0;
                        }
                    }
                    else if (upFlip[0] && tmpCounter != swingHigh.Counter)
                    {
                        abcShortChanceInProgress = false;
                        entryLineStartBar = 0;
                    }
                    else
                    {
                        if (swingHigh.Update && tmpCounter == swingHigh.Counter)
                        {
                            aBar = CurrentBar - swingHigh.LastBar;
                            bBar = CurrentBar - swingLow.CurBar;
                            cBar = CurrentBar - swingHigh.CurBar;

                            Draw.Line(this, "BCLineDn" + drawTag, IsAutoScale, bBar, swingLow.CurPrice,
                                cBar, swingHigh.CurPrice, abcZigZagColorDn, abcLineStyle, abcLineWidth);
                            Draw.Line(this, "ACLineDn" + drawTag, IsAutoScale, aBar, swingHigh.LastPrice,
                                cBar, swingHigh.CurPrice, abcZigZagColorDn, abcLineStyleRatio, abcLineWidthRatio);
                            if (abcLabel)
                            {
                                Draw.Text(this, "CDn" + drawTag, IsAutoScale, "C", cBar, swingHigh.CurPrice,
                                    abcTextOffsetLabel, abcTextColorDn, abcTextFont,
                                    TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
                            }

                            entryLevel = swingHigh.CurPrice - Instrument.MasterInstrument.RoundToTickSize((swingHigh.CurLength * TickSize)
                                / 100 * retracementEntryValue);
                            entryLevelLine[0] = entryLevel;

                            if (showEntryLine)
                            {
                                if (entryLevelLine[1] == 0)
                                    Draw.Line(this, "EntryLine" + CurrentBar.ToString(), IsAutoScale, 1, entryLevelLine[0], 0, entryLevelLine[0],
                                        entryLineColorDn, entryLineStyle, entryLineWidth);
                                else
                                    Draw.Line(this, "EntryLine" + CurrentBar.ToString(), IsAutoScale, 1, entryLevelLine[1], 0, entryLevelLine[0],
                                        entryLineColorDn, entryLineStyle, entryLineWidth);
                            }
                            entryShort[0] = entryLevel;
                            if (riskManagementPosition != RiskManagementStyle.False)
                            {
                                double entryRM = entryShort[0];
                                double stopRM = swingHigh.CurPrice + 1 * TickSize;
                                double target100RM = 0.0;
                                if (swingCurrent.SwingSlope == -1)
                                    target100RM = swingHigh.CurPrice + swingLow.LastLength * TickSize * abcTarget / 100;
                                else
                                    target100RM = swingHigh.CurPrice + swingLow.CurLength * TickSize * abcTarget / 100;
                                CalculateShortRisk(entryRM, stopRM, target100RM);
                            }
                        }
                        else if (IsFirstTickOfBar)
                        {
                            entryLevelLine[0] = entryLevel;

                            if (showEntryLine)
                            {
                                if (entryLevelLine[1] == 0)
                                    Draw.Line(this, "EntryLine" + CurrentBar.ToString(), IsAutoScale, 1, entryLevelLine[0], 0, entryLevelLine[0],
                                        entryLineColorDn, entryLineStyle, entryLineWidth);
                                else
                                    Draw.Line(this, "EntryLine" + CurrentBar.ToString(), IsAutoScale, 1, entryLevelLine[1], 0, entryLevelLine[0],
                                        entryLineColorDn, entryLineStyle, entryLineWidth);
                            }
                        }
                        abcSignals[0] = -1;

                        bool abcShort = false;
                        if (Calculate == Calculate.OnBarClose || State == State.Historical)
                        {
                            if (Close[0] < entryLevel)
                            {
                                entryShort[0] = Close[0];
                                abcShort = true;
                            }
                        }
                        else
                        {
                            if (IsFirstTickOfBar && Open[0] < entryLevel)
                            {
                                entryShort[0] = Open[0];
                                abcShort = true;
                            }
                        }

                        if (abcShort)
                        {
                            if (showEntryArrows)
                                Draw.ArrowDown(this, "AbcDn" + abcEntryTag++.ToString(), IsAutoScale, 0,
                                    High[0] + yTickOffset * TickSize, abcTextColorDn);
                            abcShortChanceInProgress = false;
                            entryLineStartBar = 0;
                            entryBar = CurrentBar;
                            abcSignals[0] = -2;
                            if (riskManagementPosition != RiskManagementStyle.False)
                            {
                                double entryRM = entryShort[0];
                                double stopRM = swingHigh.CurPrice + 1 * TickSize;
                                double target100RM = 0.0;
                                if (swingCurrent.SwingSlope == -1)
                                    target100RM = swingHigh.CurPrice + swingLow.LastLength * TickSize * abcTarget / 100;
                                else
                                    target100RM = swingHigh.CurPrice + swingLow.CurLength * TickSize * abcTarget / 100;
                                CalculateShortRisk(entryRM, stopRM, target100RM);
                            }
                            if (alertAbcEntry)
                                Alert("Alert_Abc_Short_Entry" + alertTag++.ToString(), alertAbcEntryPriority, "ABC Short Entry" + " ("
                                    + Instrument.FullName + " " + BarsPeriod + ")", alertAbcShortEntrySoundFileName, 0, Brushes.Red, Brushes.Black);
                        }
                    }
                }
            }
            //-------------------------------------------------------------------------------------
            #endregion
            //=====================================================================================
            #endregion

            #region Divergence
            //=====================================================================================
            if (divergenceMode != DivergenceMode.False)
            {
                if (divergenceDirection != DivergenceDirection.Short)
                {
                    if (swingHigh.New == true && swingHigh.Update == false)
                    {
						drawTagDivDn++;
                        divLastSwing = swingHigh.LastPrice;
                        divLastOscValue = Math.Max(divergenceDataHigh[CurrentBar - 
                            swingHigh.LastBar + 1], Math.Max(divergenceDataHigh[CurrentBar - 
                            swingHigh.LastBar], divergenceDataHigh[CurrentBar - 
                            swingHigh.LastBar - 1]));
                    }

                    if (swingHigh.New == true || swingHigh.Update == true)
                    {
                        divCurSwing = swingHigh.CurPrice;
                        divCurOscValue = Math.Max(divergenceDataHigh[CurrentBar - 
                            swingHigh.CurBar], divergenceDataHigh[CurrentBar - 
                            swingHigh.CurBar + 1]);

                        if (showDivergenceHidden == true)
                        {
							if (divLastSwing > divCurSwing && divLastOscValue < divCurOscValue)
                            {
                                Draw.Line(this, "DivHidSignalDn" + drawTagDivDn, IsAutoScale,
                                    CurrentBar - swingHigh.LastBar, swingHigh.LastPrice, 
                                    CurrentBar - swingHigh.CurBar, swingHigh.CurPrice, 
                                    divDnLineColor, divDnLineStyle, divDnLineWidth);
                                int textDivBarAgo = Convert.ToInt32(CurrentBar -
                                    (swingHigh.LastBar + swingHigh.CurBar) / 2);
                                double textDivPrice = Instrument.MasterInstrument.RoundToTickSize(
                                    (swingHigh.LastPrice + swingHigh.CurPrice) / 2);
                                Draw.Text(this, "DivHidTextDn" + drawTagDivDn, IsAutoScale, "hDiv",
                                    textDivBarAgo, textDivPrice, 10, divDnLineColor, textFont,
                                    TextAlignment.Center, Brushes.Transparent, Brushes.Transparent,
                                    0);
                                divHiddenShortActive = true;
                                divSignal[0] = -2;
                            }
                            else
                            {
                                RemoveDrawObject("DivHidSignalDn" + drawTagDivDn);
                                RemoveDrawObject("DivHidTextDn" + drawTagDivDn);
                                divHiddenShortActive = false;
                            }
                        }

                        if (showDivergenceRegular == true)
                        {
                            if (divLastSwing < divCurSwing && divLastOscValue > divCurOscValue)
                            {
                                Draw.Line(this, "DivSignalDn" + drawTagDivDn, IsAutoScale,
                                    CurrentBar - swingHigh.LastBar, swingHigh.LastPrice, 
                                    CurrentBar - swingHigh.CurBar, swingHigh.CurPrice, 
                                    divDnLineColor, divDnLineStyle, divDnLineWidth);
                                int textDivBarAgo = Convert.ToInt32(CurrentBar -
                                    (swingHigh.LastBar + swingHigh.CurBar) / 2);
                                double textDivPrice = Instrument.MasterInstrument.RoundToTickSize(
                                    (swingHigh.LastPrice + swingHigh.CurPrice) / 2);
                                Draw.Text(this, "DivRegTextDn" + drawTagDivDn, IsAutoScale, "rDiv",
                                    textDivBarAgo, textDivPrice, 10, divDnLineColor, textFont,
                                    TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 
                                    0);
                                divRegularShortActive = true;
                                divSignal[0] = -1;
                            }
                            else
                            {
                                RemoveDrawObject("DivSignalDn" + drawTagDivDn);
                                RemoveDrawObject("DivRegTextDn" + drawTagDivDn);
                                divRegularShortActive = false;
                            }
                        }
                    }
                }

                if (divergenceDirection != DivergenceDirection.Long)
                {
                    if (swingLow.New == true && swingLow.Update == false)
                    {
                        drawTagDivUp++;
                        divLastSwing = swingLow.LastPrice;
                        divLastOscValue = Math.Min(divergenceDataLow[CurrentBar - 
                            swingLow.LastBar + 1], Math.Min(divergenceDataLow[CurrentBar -
                            swingLow.LastBar], divergenceDataLow[CurrentBar - 
                            swingLow.LastBar - 1]));
                    }

                    if (swingLow.New == true || swingLow.Update == true)
                    {
                        divCurSwing = swingLow.CurPrice;
                        divCurOscValue = Math.Min(divergenceDataLow[CurrentBar - swingLow.CurBar],
                            divergenceDataLow[CurrentBar - swingLow.CurBar + 1]);

                        if (showDivergenceHidden == true)
                        {
                            if (divLastSwing < divCurSwing && divLastOscValue > divCurOscValue)
                            {
                                Draw.Line(this, "DivHidSignalup" + drawTagDivUp, IsAutoScale,
                                    CurrentBar - swingLow.LastBar, swingLow.LastPrice, 
                                    CurrentBar - swingLow.CurBar, swingLow.CurPrice, 
                                    divUpLineColor, divUpLineStyle, divUpLineWidth);
                                int textDivBarAgo = Convert.ToInt32(CurrentBar -
                                    (swingLow.LastBar + swingLow.CurBar) / 2);
                                double textDivPrice = Instrument.MasterInstrument.RoundToTickSize(
                                    (swingLow.LastPrice + swingLow.CurPrice) / 2);
                                Draw.Text(this, "DivHidTextUp" + drawTagDivUp, IsAutoScale, "hDiv",
                                    textDivBarAgo, textDivPrice, -10, divUpLineColor, textFont,
                                    TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 
                                    0);
                                divHiddenLongActive = true;
                                divSignal[0] = 2;
                            }
                            else
                            {
                                RemoveDrawObject("DivHidSignalup" + drawTagDivUp);
                                RemoveDrawObject("DivHidTextUp" + drawTagDivUp);
                                divHiddenLongActive = false;
                            }
                        }

                        if (showDivergenceRegular == true)
                        {
                            if (divLastSwing > divCurSwing && divLastOscValue < divCurOscValue)
                            {
                                Draw.Line(this, "DivSignalUp" + drawTagDivUp, IsAutoScale,
                                    CurrentBar - swingLow.LastBar, swingLow.LastPrice, 
                                    CurrentBar - swingLow.CurBar, swingLow.CurPrice, 
                                    divUpLineColor, divUpLineStyle, divUpLineWidth);
                                int textDivBarAgo = Convert.ToInt32(CurrentBar -
                                    (swingLow.LastBar + swingLow.CurBar) / 2);
                                double textDivPrice = Instrument.MasterInstrument.RoundToTickSize(
                                    (swingLow.LastPrice + swingLow.CurPrice) / 2);
                                Draw.Text(this, "DivRegTextUp" + drawTagDivUp, IsAutoScale, "rDiv",
                                    textDivBarAgo, textDivPrice, -10, divUpLineColor, textFont,
                                    TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 
                                    0);
                                divRegularLongActive = true;
                                divSignal[0] = 1;
                            }
                            else
                            {
                                RemoveDrawObject("DivSignalUp" + drawTagDivUp);
                                RemoveDrawObject("DivRegTextUp" + drawTagDivUp);
                                divRegularLongActive = false;
                            }
                        }
                    }
                }

                if (dnFlip[0] == true)
                {
                    if (divRegularShortActive == true)
                    {
                        divRegularShortActive = false;
                        divSignal[0] = -3;
                    }
                    else if (divHiddenShortActive == true)
                    {
                        divHiddenShortActive = false;
                        divSignal[0] = -4;
                    }
                }
                else if (upFlip[0] == true)
                {
                    if (divRegularLongActive == true)
                    {
                        divRegularLongActive = false;
                        divSignal[0] = 3;
                    }
                    else if (divHiddenLongActive == true)
                    {
                        divHiddenLongActive = false;
                        divSignal[0] = 4;
                    }
                }
            }
            //=====================================================================================
            #endregion

            #region Naked swing
            //=====================================================================================
            if (showNakedSwings == true)
            {
                if (swingLow.New == true && swingLow.Update == false)
                {
                    nakedSwingHighsList.Add(swingHigh.CurPrice, swingHigh.CurBar);
                    Draw.Ray(this, "NakedSwingHigh" + swingHigh.CurPrice.ToString(), false,
                        CurrentBar - swingHigh.CurBar, swingHigh.CurPrice,
                        CurrentBar - swingHigh.CurBar - 1, swingHigh.CurPrice,
                        nakedSwingHighColor, nakedSwingDashStyle,
                        nakedSwingLineWidth);
                }
                if ((swingLow.New == true || swingLow.Update == true)
                    && nakedSwingLowsList.Count > 0)
                {
                    while (nakedSwingLowsList.Count > 0
                        && nakedSwingLowsList.Keys[nakedSwingLowsList.Count
                        - 1] >= swingLow.CurPrice)
                    {
                        int counter = nakedSwingLowsList.Count - 1;
                        double nakedSwingLowPrice = nakedSwingLowsList.Keys[counter];
                        RemoveDrawObject("NakedSwingLow" + nakedSwingLowPrice.ToString());
                        if (showHistoricalNakedSwings == true)
                        {
                            Draw.Line(this, "NakedSwingLow" + nakedSwingCounter++, false,
                                CurrentBar - nakedSwingLowsList.Values[counter],
                                nakedSwingLowPrice, CurrentBar - swingLow.CurBar,
                                nakedSwingLowPrice, nakedSwingLowColor,
                                nakedSwingDashStyle, nakedSwingLineWidth);
                        }
                        nakedSwingLowsList.RemoveAt(counter);
                    }
                }
                if (swingHigh.New == true && swingHigh.Update == false)
                {
                    nakedSwingLowsList.Add(swingLow.CurPrice, swingLow.CurBar);
                    Draw.Ray(this, "NakedSwingLow" + swingLow.CurPrice.ToString(), false,
                        CurrentBar - swingLow.CurBar, swingLow.CurPrice,
                        CurrentBar - swingLow.CurBar - 1, swingLow.CurPrice,
                        nakedSwingLowColor, nakedSwingDashStyle,
                        nakedSwingLineWidth);
                }
                if ((swingHigh.New == true || swingHigh.Update == true)
                    && nakedSwingHighsList.Count > 0)
                {
                    while (nakedSwingHighsList.Count > 0
                        && nakedSwingHighsList.Keys[0] <= swingHigh.CurPrice)
                    {
                        double nakedSwingHighPrice = nakedSwingHighsList.Keys[0];
                        RemoveDrawObject("NakedSwingHigh" + nakedSwingHighPrice.ToString());
                        if (showHistoricalNakedSwings == true)
                        {
                            Draw.Line(this, "NakedSwingHigh" + nakedSwingCounter++, false,
                                CurrentBar - nakedSwingHighsList.Values[0],
                                nakedSwingHighPrice, CurrentBar - swingHigh.CurBar,
                                nakedSwingHighPrice, nakedSwingHighColor,
                                nakedSwingDashStyle, nakedSwingLineWidth);
                        }
                        nakedSwingHighsList.RemoveAt(0);
                    }
                }
            }
            //=====================================================================================
            #endregion

            #region Swing switch
            //=====================================================================================
            if (showSwingSwitch == true)
            {
                if (swingLow.New == true && swingLow.Update == false)
                    Draw.TriangleDown(this, "DnSwingStart", false, 0,
                        Highs[BarsInProgress][0] + swingSwitchOffsetInTicks * TickSize, 
                        swingSwitchDownColor);

                if (swingHigh.New == true && swingHigh.Update == false)
                    Draw.TriangleUp(this, "UpSwingStart", false, 0,
                        Lows[BarsInProgress][0] - swingSwitchOffsetInTicks * TickSize, 
                        swingSwitchUpColor);
            }
            //=====================================================================================
            #endregion

            #region Alerts
            //=====================================================================================
            // Double bottom
            if (alertDoubleBottom == true && swingHigh.New == true && swingHigh.Update == false
                && swingLow.CurRelation == 0)
            {
				Print(alertDoubleTopMessage + " (" + Instrument.FullName + " " + BarsPeriod + ")");
                Alert("alertDoubleBottom" + alertTag++, alertDoubleBottomPriority,
                    alertDoubleBottomMessage + " (" + Instrument.FullName + " " + BarsPeriod + ")",
                    alertDoubleBottomSoundFileName, alertDoubleBottomRearmSeconds,
                    alertDoubleBottomBackColor, alertDoubleBottomForeColor);
            }
            //=====================================================================================
            // Double Top
            if (alertDoubleTop == true && swingLow.New == true && swingLow.Update == false
                && swingHigh.CurRelation == 0)
            {
				Print(alertDoubleTopMessage + " (" + Instrument.FullName + " " + BarsPeriod + ")");
                Alert("alertDoubleTop" + alertTag++, alertDoubleTopPriority,
                    alertDoubleTopMessage + " (" + Instrument.FullName + " " + BarsPeriod + ")",
                    alertDoubleTopSoundFileName, alertDoubleTopRearmSeconds,
                    alertDoubleTopBackColor, alertDoubleTopForeColor);
            }
            //=====================================================================================
            // Swing change
            if (alertSwingChange == true && ((swingHigh.New == true && swingHigh.Update == false)
                || (swingLow.New == true && swingLow.Update == false)))
            {
				Print(alertDoubleTopMessage + " (" + Instrument.FullName + " " + BarsPeriod + ")");
                Alert("alertSwingChange" + alertTag++, alertSwingChangePriority,
                    alertSwingChangeMessage + " (" + Instrument.FullName + " " + BarsPeriod + ")",
                    alertSwingChangeSoundFileName, alertSwingChangeRearmSeconds,
                    alertSwingChangeBackColor, alertSwingChangeForeColor);
            }
            //=====================================================================================
            // Divergence regular high
            if (alertDivergenceRegularHigh == true && swingLow.New == true
                && swingLow.Update == false && divSignal[0] == -3)
            {
                Alert("alertDivergenceRegularHigh" + alertTag++,
                    alertDivergenceRegularHighPriority,
                    alertDivergenceRegularHighMessage + " (" + Instrument.FullName + " " + BarsPeriod + ")",
                    alertDivergenceRegularHighSoundFileName,
                    alertDivergenceRegularHighRearmSeconds, alertDivergenceRegularHighBackColor,
                    alertDivergenceRegularHighForeColor);
            }
            //=====================================================================================
            // Divergence hidden high
            if (alertDivergenceHiddenHigh == true && swingLow.New == true
                && swingLow.Update == false && divSignal[0] == -4)
            {
                Alert("alertDivergenceHiddenHigh" + alertTag++,
                    alertDivergenceHiddenHighPriority,
                    alertDivergenceHiddenHighMessage + " (" + Instrument.FullName + " " + BarsPeriod + ")",
                    alertDivergenceHiddenHighSoundFileName,
                    alertDivergenceHiddenHighRearmSeconds, alertDivergenceHiddenHighBackColor,
                    alertDivergenceHiddenHighForeColor);
            }
            //=====================================================================================
            // Divergence regular low
            if (alertDivergenceRegularLow == true && swingHigh.New == true
                && swingHigh.Update == false && divSignal[0] == 3)
            {
                Alert("alertDivergenceRegularLow" + alertTag++,
                    alertDivergenceRegularLowPriority,
                    alertDivergenceRegularLowMessage + " (" + Instrument.FullName + " " + BarsPeriod + ")",
                    alertDivergenceRegularLowSoundFileName,
                    alertDivergenceRegularLowRearmSeconds, alertDivergenceRegularLowBackColor,
                    alertDivergenceRegularLowForeColor);
            }
            //=====================================================================================
            // Divergence hidden low
            if (alertDivergenceHiddenLow == true && swingHigh.New == true
                && swingHigh.Update == false && divSignal[0] == 4)
            {
                Alert("alertDivergenceHiddenLow" + alertTag++,
                    alertDivergenceHiddenLowPriority,
                    alertDivergenceHiddenLowMessage + " (" + Instrument.FullName + " " + BarsPeriod + ")",
                    alertDivergenceHiddenLowSoundFileName,
                    alertDivergenceHiddenLowRearmSeconds, alertDivergenceHiddenLowBackColor,
                    alertDivergenceHiddenLowForeColor);
            }
            //=====================================================================================
            // Higher low
            if (alertHigherLow == true && swingHigh.New == true
                && swingHigh.Update == false && swingLow.CurRelation == 1)
            {
                Alert("alertHigherLow" + alertTag++,
                    alertHigherLowPriority,
                    alertHigherLowMessage + " (" + Instrument.FullName + " " + BarsPeriod + ")",
                    alertHigherLowSoundFileName,
                    alertHigherLowRearmSeconds, alertHigherLowBackColor,
                    alertHigherLowForeColor);
            }
            //=====================================================================================
            // Lower high
            if (alertLowerHigh == true && swingLow.New == true
                && swingLow.Update == false && swingHigh.CurRelation == -1)
            {
                Alert("alertLowerHigh" + alertTag++,
                    alertLowerHighPriority,
                    alertLowerHighMessage + " (" + Instrument.FullName + " " + BarsPeriod + ")",
                    alertLowerHighSoundFileName,
                    alertLowerHighRearmSeconds, alertLowerHighBackColor,
                    alertLowerHighForeColor);
            }
            //=====================================================================================
            #endregion
		}
		
        #region Initialize swing calculation
        //#########################################################################################
        public void InitializeSwingCalculation(Swings swingHigh, Swings swingLow,
            SwingCurrent swingCur, Series<bool> upFlip, List<SwingStruct> swingHighs,
            Series<bool> dnFlip, List<SwingStruct> swingLows)
        {
            if (IsFirstTickOfBar)
            {
                swingCur.StopOutsideBarCalc = false;

                // Initialize first swing
                if (swingHighs.Count == 0)
                {
                    swingHigh.CurBar = CurrentBars[BarsInProgress];
                    swingHigh.CurPrice = Highs[BarsInProgress][CurrentBars[BarsInProgress]];
                    swingHigh.CurDateTime = swingHigh.LastDateTime =
                        Times[BarsInProgress][CurrentBars[BarsInProgress]];
                    SwingStruct up = new SwingStruct(swingHigh.CurPrice, swingHigh.CurBar,
                        Times[BarsInProgress][CurrentBars[BarsInProgress] - 1], 0, 0, -1,
                        Convert.ToInt64(Volumes[BarsInProgress][0]));
                    swingHighs.Add(up);
                    swingHigh.ListCount = swingHighs.Count;
                }
                if (swingLows.Count == 0)
                {
                    swingLow.CurBar = CurrentBars[BarsInProgress];
                    swingLow.CurPrice = Lows[BarsInProgress][CurrentBars[BarsInProgress]];
                    swingLow.CurDateTime = swingLow.LastDateTime =
                        Times[BarsInProgress][CurrentBars[BarsInProgress]];
                    SwingStruct dn = new SwingStruct(swingLow.CurPrice, swingLow.CurBar,
                        Times[BarsInProgress][CurrentBars[BarsInProgress] - 1], 0, 0, -1,
                        Convert.ToInt64(Volumes[BarsInProgress][0]));
                    swingLows.Add(dn);
                    swingLow.ListCount = swingLows.Count;
                }
            }
            // Set new/update high/low back to false, to avoid function calls which depends on
            // them
            dnFlip[0] = false;
            upFlip[0] = false;
            swingHigh.New = swingLow.New = swingHigh.Update = swingLow.Update = false;
        }
        //#########################################################################################
        #endregion

        #region Calculate Swing Standard
        //#########################################################################################
        protected void CalculateSwingStandard(Swings swingHigh, Swings swingLow, 
            SwingCurrent swingCur, SwingProperties swingProp, Series<bool> upFlip,
            List<SwingStruct> swingHighs, Series<bool> dnFlip, List<SwingStruct> swingLows,
            int decimalPlaces, bool useCloseValues, Series<double> doubleBottom, Series<double> lowerLow,
            Series<double> higherLow, Series<double> doubleTop, Series<double> lowerHigh,
            Series<double> higherHigh, Series<double> gannSwing)
        {
            // Check if high and low values are used or only close values
            PriceSeries[] highs;
            PriceSeries[] lows;
            if (useCloseValues == true)
            {
                lows = Closes;
                highs = Closes;
            }
            else
            {
                lows = Lows;
                highs = Highs;
            }

            // For a new swing high in an uptrend, Highs[BarsInProgress][0] must be 
            // greater than the current swing high
            if (swingCur.SwingSlope == 1 && highs[BarsInProgress][0] <= swingHigh.CurPrice)
                swingHigh.New = false;
            else
                swingHigh.New = true;

            // For a new swing low in a downtrend, Lows[BarsInProgress][0] must be 
            // smaller than the current swing low
            if (swingCur.SwingSlope == -1 && lows[BarsInProgress][0] >= swingLow.CurPrice)
                swingLow.New = false;
            else
                swingLow.New = true;

            // CalculatOnBarClose == true
            if (Calculate == Calculate.OnBarClose)
            {
                // test if Highs[BarsInProgress][0] is higher than the last 
                // calculationSize highs = new swing high
                if (swingHigh.New)
                {
                    for (int i = 1; i < swingProp.SwingSize + 1; i++)
                    {
                        if (highs[BarsInProgress][0] <= highs[BarsInProgress][i])
                        {
                            swingHigh.New = false;
                            break;
                        }
                    }
                }
                // test if Lows[BarsInProgress][0] is lower than the last 
                // calculationSize lows = new swing low
                if (swingLow.New)
                {
                    for (int i = 1; i < swingProp.SwingSize + 1; i++)
                    {
                        if (lows[BarsInProgress][0] >= lows[BarsInProgress][i])
                        {
                            swingLow.New = false;
                            break;
                        }
                    }
                }

                // New swing high and new swing low
                if (swingHigh.New && swingLow.New)
                {
                    // Downtrend - ignore the swing high
                    if (swingCur.SwingSlope == -1)
                        swingHigh.New = false;
                    // Uptrend   - ignore the swing low
                    else
                        swingLow.New = false;
                }
            }
            // CalculatOnBarClose == false
            else
            {
                // Used to control, that only one swing is set for 
                // each bar
                if (IsFirstTickOfBar)
                    swingCur.NewSwing = 0;

                // No swing or an up swing is found
                if (swingCur.NewSwing != -1)
                {
                    // test if Highs[BarsInProgress][0] is higher than the last 
                    // calculationSize highs = new swing high
                    if (swingHigh.New)
                    {
                        for (int i = 1; i < swingProp.SwingSize + 1; i++)
                        {
                            if (highs[BarsInProgress][0] <= highs[BarsInProgress][i])
                            {
                                swingHigh.New = false;
                                break;
                            }
                        }
                        // Found a swing high
                        if (swingHigh.New)
                            swingCur.NewSwing = 1;
                    }
                }

                // No swing or an down swing is found
                if (swingCur.NewSwing != 1)
                {
                    // test if Lows[BarsInProgress][0] is lower than the last 
                    // calculationSize lows = new swing low
                    if (swingLow.New)
                    {
                        for (int i = 1; i < swingProp.SwingSize + 1; i++)
                        {
                            if (lows[BarsInProgress][0] >= lows[BarsInProgress][i])
                            {
                                swingLow.New = false;
                                break;
                            }
                        }
                        // Found a swing low
                        if (swingLow.New)
                            swingCur.NewSwing = -1;
                    }
                }

                // Set newLow back to false
                if (swingCur.NewSwing == 1)
                    swingLow.New = false;
                // Set newHigh back to false
                if (swingCur.NewSwing == -1)
                    swingHigh.New = false;
            }

            // Swing high
            if (swingHigh.New)
            {
                int bar;
                double price;
                // New swing high
                if (swingCur.SwingSlope != 1)
                {
                    bar = CurrentBars[BarsInProgress] -
                        HighestBar(highs[BarsInProgress], CurrentBars[BarsInProgress] -
                        swingLow.CurBar);
                    price = highs[BarsInProgress][HighestBar(highs[BarsInProgress],
                        CurrentBars[BarsInProgress] - swingLow.CurBar)];
                    swingHigh.Update = false;
                }
                // Update swing high
                else
                {
                    bar = CurrentBars[BarsInProgress];
                    price = highs[BarsInProgress][0];
                    swingHigh.Update = true;
                }
                CalcUpSwing(bar, price, swingHigh.Update, swingHigh, swingLow, swingCur,
                    swingProp, upFlip, swingHighs, decimalPlaces, doubleBottom, lowerLow, 
                    higherLow, doubleTop, lowerHigh, higherHigh, gannSwing);
            }
            // Swing low
            else if (swingLow.New)
            {
                int bar;
                double price;
                // New swing low
                if (swingCur.SwingSlope != -1)
                {
                    bar = CurrentBars[BarsInProgress] - LowestBar(lows[BarsInProgress],
                        CurrentBars[BarsInProgress] - swingHigh.CurBar);
                    price = lows[BarsInProgress][LowestBar(lows[BarsInProgress],
                        CurrentBars[BarsInProgress] - swingHigh.CurBar)];
                    swingLow.Update = false;
                }
                // Update swing low
                else
                {
                    bar = CurrentBars[BarsInProgress];
                    price = lows[BarsInProgress][0];
                    swingLow.Update = true;
                }
                CalcDnSwing(bar, price, swingLow.Update, swingHigh, swingLow, swingCur,
                    swingProp, dnFlip, swingLows, decimalPlaces, doubleBottom, lowerLow, 
                    higherLow, doubleTop, lowerHigh, higherHigh, gannSwing);
            }            
        }
        //#########################################################################################
        #endregion

        #region Calculate Swing Gann
        //#########################################################################################
        protected void CalculateSwingGann(Swings swingHigh, Swings swingLow, SwingCurrent swingCur,
            SwingProperties swingProp, Series<bool> upFlip, List<SwingStruct> swingHighs,
            Series<bool> dnFlip, List<SwingStruct> swingLows, int decimalPlaces, 
            Series<double> doubleBottom, Series<double> lowerLow, Series<double> higherLow, 
            Series<double> doubleTop, Series<double> lowerHigh, Series<double> higherHigh, 
            Series<double> gannSwing)
        {
            #region Set bar property
            //=================================================================================
            // Represents the bar type. -1 = Down | 0 = Inside | 1 = Up | 2 = Outside
            int barType = 0;
            if (Highs[BarsInProgress][0] > Highs[BarsInProgress][1])
            {
                if (Lows[BarsInProgress][0] < Lows[BarsInProgress][1])
                    barType = 2;
                else
                    barType = 1;
            }
            else
            {
                if (Lows[BarsInProgress][0] < Lows[BarsInProgress][1])
                    barType = -1;
                else
                    barType = 0;
            }
            //=================================================================================
            #endregion

            #region Up swing
            //=================================================================================
            if (swingCur.SwingSlope == 1)
            {
                switch (barType)
                {
                    // Up bar
                    case 1:
                        swingCur.ConsecutiveBars = 0;
                        swingCur.ConsecutiveBarValue = 0.0;
                        if (Highs[BarsInProgress][0] > swingHigh.CurPrice)
                        {
                            swingHigh.New = true;
                            swingHigh.Update = true;
                            CalcUpSwing(CurrentBars[BarsInProgress],
                                Highs[BarsInProgress][0], swingHigh.Update, swingHigh,
                                swingLow, swingCur, swingProp, upFlip, swingHighs,
                                decimalPlaces, doubleBottom, lowerLow, higherLow, doubleTop, 
                                lowerHigh, higherHigh, gannSwing);
                            if ((swingCur.ConsecutiveBars + 1) == swingProp.SwingSize)
                                swingCur.StopOutsideBarCalc = true;
                        }
                        break;
                    // Down bar
                    case -1:
                        if (swingCur.ConsecutiveBarNumber != CurrentBars[BarsInProgress])
                        {
                            if (swingCur.ConsecutiveBarValue == 0.0)
                            {
                                swingCur.ConsecutiveBars++;
                                swingCur.ConsecutiveBarNumber = CurrentBars[BarsInProgress];
                                swingCur.ConsecutiveBarValue = Lows[BarsInProgress][0];
                            }
                            else if (Lows[BarsInProgress][0] < swingCur.ConsecutiveBarValue)
                            {
                                swingCur.ConsecutiveBars++;
                                swingCur.ConsecutiveBarNumber = CurrentBars[BarsInProgress];
                                swingCur.ConsecutiveBarValue = Lows[BarsInProgress][0];
                            }
                        }
                        else if (Lows[BarsInProgress][0] < swingCur.ConsecutiveBarValue)
                            swingCur.ConsecutiveBarValue = Lows[BarsInProgress][0];
                        if (swingCur.ConsecutiveBars == swingProp.SwingSize ||
                            (swingProp.UseBreakouts && Lows[BarsInProgress][0] <
                            swingLow.CurPrice))
                        {
                            swingCur.ConsecutiveBars = 0;
                            swingCur.ConsecutiveBarValue = 0.0;
                            swingLow.New = true;
                            swingLow.Update = false;
                            int bar = CurrentBars[BarsInProgress] -
                                LowestBar(Lows[BarsInProgress],
                                CurrentBars[BarsInProgress] - swingHigh.CurBar);
                            double price =
                                Lows[BarsInProgress][LowestBar(Lows[BarsInProgress],
                                CurrentBars[BarsInProgress] - swingHigh.CurBar)];
                            CalcDnSwing(bar, price, swingLow.Update, swingHigh, swingLow,
                                swingCur, swingProp, dnFlip, swingLows, decimalPlaces, 
                                doubleBottom, lowerLow, higherLow, doubleTop, lowerHigh, 
                                higherHigh, gannSwing);
                        }
                        break;
                    // Inside bar
                    case 0:
                        if (!swingProp.IgnoreInsideBars)
                        {
                            swingCur.ConsecutiveBars = 0;
                            swingCur.ConsecutiveBarValue = 0.0;
                        }
                        break;
                    // Outside bar
                    case 2:
                        if (Highs[BarsInProgress][0] > swingHigh.CurPrice)
                        {
                            swingHigh.New = true;
                            swingHigh.Update = true;
                            CalcUpSwing(CurrentBars[BarsInProgress],
                                Highs[BarsInProgress][0], swingHigh.Update, swingHigh,
                                swingLow, swingCur, swingProp, upFlip, swingHighs,
                                decimalPlaces, doubleBottom, lowerLow, higherLow, doubleTop,
                                lowerHigh, higherHigh, gannSwing);
                        }
                        else if (!swingCur.StopOutsideBarCalc)
                        {
                            if (swingCur.ConsecutiveBarNumber != CurrentBars[BarsInProgress])
                            {
                                if (swingCur.ConsecutiveBarValue == 0.0)
                                {
                                    swingCur.ConsecutiveBars++;
                                    swingCur.ConsecutiveBarNumber =
                                        CurrentBars[BarsInProgress];
                                    swingCur.ConsecutiveBarValue = Lows[BarsInProgress][0];
                                }
                                else if (Lows[BarsInProgress][0] <
                                    swingCur.ConsecutiveBarValue)
                                {
                                    swingCur.ConsecutiveBars++;
                                    swingCur.ConsecutiveBarNumber =
                                        CurrentBars[BarsInProgress];
                                    swingCur.ConsecutiveBarValue = Lows[BarsInProgress][0];
                                }
                            }
                            else if (Lows[BarsInProgress][0] < swingCur.ConsecutiveBarValue)
                                swingCur.ConsecutiveBarValue = Lows[BarsInProgress][0];
                            if (swingCur.ConsecutiveBars == swingProp.SwingSize ||
                                (swingProp.UseBreakouts && Lows[BarsInProgress][0] <
                                swingLow.CurPrice))
                            {
                                swingCur.ConsecutiveBars = 0;
                                swingCur.ConsecutiveBarValue = 0.0;
                                swingLow.New = true;
                                swingLow.Update = false;
                                int bar = CurrentBars[BarsInProgress] -
                                    LowestBar(Lows[BarsInProgress],
                                    CurrentBars[BarsInProgress] - swingHigh.CurBar);
                                double price =
                                    Lows[BarsInProgress][LowestBar(Lows[BarsInProgress],
                                    CurrentBars[BarsInProgress] - swingHigh.CurBar)];
                                CalcDnSwing(bar, price, swingLow.Update, swingHigh, swingLow,
                                    swingCur, swingProp, dnFlip, swingLows, decimalPlaces, 
                                    doubleBottom, lowerLow, higherLow, doubleTop, lowerHigh,
                                    higherHigh, gannSwing);
                            }
                        }
                        break;
                }
            }
            //=================================================================================
            #endregion

            #region Down swing
            //=================================================================================
            else
            {
                switch (barType)
                {
                    // Dwon bar
                    case -1:
                        swingCur.ConsecutiveBars = 0;
                        swingCur.ConsecutiveBarValue = 0.0;
                        if (Lows[BarsInProgress][0] < swingLow.CurPrice)
                        {
                            swingLow.New = true;
                            swingLow.Update = true;
                            CalcDnSwing(CurrentBars[BarsInProgress],
                                Lows[BarsInProgress][0], swingLow.Update, swingHigh,
                                swingLow, swingCur, swingProp, dnFlip, swingLows,
                                decimalPlaces, doubleBottom, lowerLow, higherLow, doubleTop,
                                lowerHigh, higherHigh, gannSwing);
                            if ((swingCur.ConsecutiveBars + 1) == swingProp.SwingSize)
                                swingCur.StopOutsideBarCalc = true;
                        }
                        break;
                    // Up bar
                    case 1:
                        if (swingCur.ConsecutiveBarNumber != CurrentBars[BarsInProgress])
                        {
                            if (swingCur.ConsecutiveBarValue == 0.0)
                            {
                                swingCur.ConsecutiveBars++;
                                swingCur.ConsecutiveBarNumber = CurrentBars[BarsInProgress];
                                swingCur.ConsecutiveBarValue = Highs[BarsInProgress][0];
                            }
                            else if (Highs[BarsInProgress][0] > swingCur.ConsecutiveBarValue)
                            {
                                swingCur.ConsecutiveBars++;
                                swingCur.ConsecutiveBarNumber = CurrentBars[BarsInProgress];
                                swingCur.ConsecutiveBarValue = Highs[BarsInProgress][0];
                            }
                        }
                        else if (Highs[BarsInProgress][0] > swingCur.ConsecutiveBarValue)
                            swingCur.ConsecutiveBarValue = Highs[BarsInProgress][0];
                        if (swingCur.ConsecutiveBars == swingProp.SwingSize ||
                            (swingProp.UseBreakouts && Highs[BarsInProgress][0] >
                            swingHigh.CurPrice))
                        {
                            swingCur.ConsecutiveBars = 0;
                            swingCur.ConsecutiveBarValue = 0.0;
                            swingHigh.New = true;
                            swingHigh.Update = false;
                            int bar = CurrentBars[BarsInProgress] -
                                HighestBar(Highs[BarsInProgress],
                                CurrentBars[BarsInProgress] - swingLow.CurBar);
                            double price =
                                Highs[BarsInProgress][HighestBar(Highs[BarsInProgress],
                                CurrentBars[BarsInProgress] - swingLow.CurBar)];
                            CalcUpSwing(bar, price, swingHigh.Update, swingHigh, swingLow,
                                swingCur, swingProp, upFlip, swingHighs, decimalPlaces, 
                                doubleBottom, lowerLow, higherLow, doubleTop, lowerHigh,
                                higherHigh, gannSwing);
                        }
                        break;
                    // Inside bar
                    case 0:
                        if (!swingProp.IgnoreInsideBars)
                        {
                            swingCur.ConsecutiveBars = 0;
                            swingCur.ConsecutiveBarValue = 0.0;
                        }
                        break;
                    // Outside bar
                    case 2:
                        if (Lows[BarsInProgress][0] < swingLow.CurPrice)
                        {
                            swingLow.New = true;
                            swingLow.Update = true;
                            CalcDnSwing(CurrentBars[BarsInProgress],
                                Lows[BarsInProgress][0], swingLow.Update, swingHigh,
                                swingLow, swingCur, swingProp, dnFlip, swingLows,
                                decimalPlaces, doubleBottom, lowerLow, higherLow, doubleTop,
                                lowerHigh, higherHigh, gannSwing);
                        }
                        else if (!swingCur.StopOutsideBarCalc)
                        {
                            if (swingCur.ConsecutiveBarNumber != CurrentBars[BarsInProgress])
                            {
                                if (swingCur.ConsecutiveBarValue == 0.0)
                                {
                                    swingCur.ConsecutiveBars++;
                                    swingCur.ConsecutiveBarNumber =
                                        CurrentBars[BarsInProgress];
                                    swingCur.ConsecutiveBarValue = Highs[BarsInProgress][0];
                                }
                                else if (Highs[BarsInProgress][0] >
                                    swingCur.ConsecutiveBarValue)
                                {
                                    swingCur.ConsecutiveBars++;
                                    swingCur.ConsecutiveBarNumber =
                                        CurrentBars[BarsInProgress];
                                    swingCur.ConsecutiveBarValue = Highs[BarsInProgress][0];
                                }
                            }
                            else if (Highs[BarsInProgress][0] > swingCur.ConsecutiveBarValue)
                                swingCur.ConsecutiveBarValue = Highs[BarsInProgress][0];
                            if (swingCur.ConsecutiveBars == swingProp.SwingSize ||
                                (swingProp.UseBreakouts && Highs[BarsInProgress][0] >
                                swingHigh.CurPrice))
                            {
                                swingCur.ConsecutiveBars = 0;
                                swingCur.ConsecutiveBarValue = 0.0;
                                swingHigh.New = true;
                                swingHigh.Update = false;
                                int bar = CurrentBars[BarsInProgress] -
                                    HighestBar(Highs[BarsInProgress],
                                    CurrentBars[BarsInProgress] - swingLow.CurBar);
                                double price =
                                    Highs[BarsInProgress][HighestBar(Highs[BarsInProgress],
                                    CurrentBars[BarsInProgress] - swingLow.CurBar)];
                                CalcUpSwing(bar, price, swingHigh.Update, swingHigh,
                                    swingLow, swingCur, swingProp, upFlip, swingHighs,
                                    decimalPlaces, doubleBottom, lowerLow, higherLow, 
                                    doubleTop, lowerHigh, higherHigh, gannSwing);
                            }
                        }
                        break;
                }
            }
            //=================================================================================
            #endregion
        }
        //#########################################################################################
        #endregion

        #region Calculate Swing Ticks
        //#########################################################################################
        protected void CalculateSwingTicks(Swings swingHigh, Swings swingLow, 
            SwingCurrent swingCur, SwingProperties swingProp, Series<bool> upFlip,
            List<SwingStruct> swingHighs, Series<bool> dnFlip, List<SwingStruct> swingLows,
            int decimalPlaces, bool useCloseValues, Series<double> doubleBottom, Series<double> lowerLow,
            Series<double> higherLow, Series<double> doubleTop, Series<double> lowerHigh,
            Series<double> higherHigh, Series<double> gannSwing)
        {
            // Check if high and low values are used or only close values
            PriceSeries[] highs;
            PriceSeries[] lows;
            if (useCloseValues == true)
            {
                lows = Closes;
                highs = Closes;
            }
            else
            {
                lows = Lows;
                highs = Highs;
            }

            // For a new swing high in an uptrend, Highs[BarsInProgress][0] must be 
            // greater than the current swing high
            if (swingCur.SwingSlope == 1 && highs[BarsInProgress][0] <= swingHigh.CurPrice)
                swingHigh.New = false;
            else
                swingHigh.New = true;

            // For a new swing low in a downtrend, Lows[BarsInProgress][0] must be 
            // smaller than the current swing low
            if (swingCur.SwingSlope == -1 && lows[BarsInProgress][0] >= swingLow.CurPrice)
                swingLow.New = false;
            else
                swingLow.New = true;

            // CalculatOnBarClose == true
            if (Calculate == Calculate.OnBarClose)
            {
                if (swingHigh.New 
                    && highs[BarsInProgress][0] < 
                    (swingLow.CurPrice + swingProp.SwingSize * TickSize))
                        swingHigh.New = false;

                if (swingLow.New
                    && lows[BarsInProgress][0] > 
                    (swingHigh.CurPrice - swingProp.SwingSize * TickSize))
                    swingLow.New = false;

                // New swing high and new swing low
                if (swingHigh.New && swingLow.New)
                {
                    // Downtrend - ignore the swing high
                    if (swingCur.SwingSlope == -1)
                        swingHigh.New = false;
                    // Uptrend   - ignore the swing low
                    else
                        swingLow.New = false;
                }
            }
            // CalculatOnBarClose == false
            else
            {
                // Used to control, that only one swing is set for 
                // each bar
                if (IsFirstTickOfBar)
                    swingCur.NewSwing = 0;

                // No swing or an up swing is found
                if (swingCur.NewSwing != -1)
                {
                    // test if Highs[BarsInProgress][0] is higher than the last 
                    // calculationSize highs = new swing high
                    if (swingHigh.New)
                    {
                        if (highs[BarsInProgress][0] < 
                            (swingLow.CurPrice + swingProp.SwingSize * TickSize))
                            swingHigh.New = false;
                        // Found a swing high
                        if (swingHigh.New)
                            swingCur.NewSwing = 1;
                    }
                }

                // No swing or an down swing is found
                if (swingCur.NewSwing != 1)
                {
                    // test if Lows[BarsInProgress][0] is lower than the last 
                    // calculationSize lows = new swing low
                    if (swingLow.New)
                    {
                        if (lows[BarsInProgress][0] > 
                            (swingHigh.CurPrice - swingProp.SwingSize * TickSize))
                            swingLow.New = false;
                        // Found a swing low
                        if (swingLow.New)
                            swingCur.NewSwing = -1;
                    }
                }

                // Set newLow back to false
                if (swingCur.NewSwing == 1)
                    swingLow.New = false;
                // Set newHigh back to false
                if (swingCur.NewSwing == -1)
                    swingHigh.New = false;
            }

            // Swing high
            if (swingHigh.New)
            {
                int bar;
                double price;
                // New swing high
                if (swingCur.SwingSlope != 1)
                {
                    bar = CurrentBars[BarsInProgress] -
                        HighestBar(highs[BarsInProgress], CurrentBars[BarsInProgress] -
                        swingLow.CurBar);
                    price = highs[BarsInProgress][HighestBar(highs[BarsInProgress],
                        CurrentBars[BarsInProgress] - swingLow.CurBar)];
                    swingHigh.Update = false;
                }
                // Update swing high
                else
                {
                    bar = CurrentBars[BarsInProgress];
                    price = highs[BarsInProgress][0];
                    swingHigh.Update = true;
                }
                CalcUpSwing(bar, price, swingHigh.Update, swingHigh, swingLow, swingCur,
                    swingProp, upFlip, swingHighs, decimalPlaces, doubleBottom, lowerLow,
                    higherLow, doubleTop, lowerHigh, higherHigh, gannSwing);
            }
            // Swing low
            else if (swingLow.New)
            {
                int bar;
                double price;
                // New swing low
                if (swingCur.SwingSlope != -1)
                {
                    bar = CurrentBars[BarsInProgress] - LowestBar(lows[BarsInProgress],
                        CurrentBars[BarsInProgress] - swingHigh.CurBar);
                    price = lows[BarsInProgress][LowestBar(lows[BarsInProgress],
                        CurrentBars[BarsInProgress] - swingHigh.CurBar)];
                    swingLow.Update = false;
                }
                // Update swing low
                else
                {
                    bar = CurrentBars[BarsInProgress];
                    price = lows[BarsInProgress][0];
                    swingLow.Update = true;
                }
                CalcDnSwing(bar, price, swingLow.Update, swingHigh, swingLow, swingCur,
                    swingProp, dnFlip, swingLows, decimalPlaces, doubleBottom, lowerLow,
                    higherLow, doubleTop, lowerHigh, higherHigh, gannSwing);
            }
        }
        //#########################################################################################
        #endregion

        #region Calculate Swing Percent
        //#########################################################################################
        protected void CalculateSwingPercent(Swings swingHigh, Swings swingLow, 
            SwingCurrent swingCur, SwingProperties swingProp, Series<bool> upFlip, 
            List<SwingStruct> swingHighs, Series<bool> dnFlip, List<SwingStruct> swingLows,
            int decimalPlaces, bool useCloseValues, Series<double> doubleBottom, Series<double> lowerLow, 
            Series<double> higherLow, Series<double> doubleTop, Series<double> lowerHigh, 
            Series<double> higherHigh, Series<double> gannSwing)
        {
            // Check if high and low values are used or only close values
            PriceSeries[] highs;
            PriceSeries[] lows;
            if (useCloseValues == true)
            {
                lows = Closes;
                highs = Closes;
            }
            else
            {
                lows = Lows;
                highs = Highs;
            }

            // For a new swing high in an uptrend, Highs[BarsInProgress][0] must be 
            // greater than the current swing high
            if (swingCur.SwingSlope == 1 && highs[BarsInProgress][0] <= swingHigh.CurPrice)
                swingHigh.New = false;
            else
                swingHigh.New = true;

            // For a new swing low in a downtrend, Lows[BarsInProgress][0] must be 
            // smaller than the current swing low
            if (swingCur.SwingSlope == -1 && lows[BarsInProgress][0] >= swingLow.CurPrice)
                swingLow.New = false;
            else
                swingLow.New = true;

            // CalculatOnBarClose == true
            if (Calculate == Calculate.OnBarClose)
            {
                if (swingHigh.New
                    && highs[BarsInProgress][0] < 
                    (swingLow.CurPrice + swingLow.CurPrice / 100 * swingProp.SwingSize))
                    swingHigh.New = false;

                if (swingLow.New
                    && lows[BarsInProgress][0] > 
                    (swingHigh.CurPrice - swingLow.CurPrice / 100 * swingProp.SwingSize))
                    swingLow.New = false;

                // New swing high and new swing low
                if (swingHigh.New && swingLow.New)
                {
                    // Downtrend - ignore the swing high
                    if (swingCur.SwingSlope == -1)
                        swingHigh.New = false;
                    // Uptrend   - ignore the swing low
                    else
                        swingLow.New = false;
                }
            }
            // CalculatOnBarClose == false
            else
            {
                // Used to control, that only one swing is set for 
                // each bar
                if (IsFirstTickOfBar)
                    swingCur.NewSwing = 0;

                // No swing or an up swing is found
                if (swingCur.NewSwing != -1)
                {
                    // test if Highs[BarsInProgress][0] is higher than the last 
                    // calculationSize highs = new swing high
                    if (swingHigh.New)
                    {
                        if (highs[BarsInProgress][0] <
                            (swingLow.CurPrice + swingLow.CurPrice / 100 * swingProp.SwingSize))
                            swingHigh.New = false;
                        // Found a swing high
                        if (swingHigh.New)
                            swingCur.NewSwing = 1;
                    }
                }

                // No swing or an down swing is found
                if (swingCur.NewSwing != 1)
                {
                    // test if Lows[BarsInProgress][0] is lower than the last 
                    // calculationSize lows = new swing low
                    if (swingLow.New)
                    {
                        if (lows[BarsInProgress][0] >
                            (swingHigh.CurPrice - swingLow.CurPrice / 100 * swingProp.SwingSize))
                            swingLow.New = false;
                        // Found a swing low
                        if (swingLow.New)
                            swingCur.NewSwing = -1;
                    }
                }

                // Set newLow back to false
                if (swingCur.NewSwing == 1)
                    swingLow.New = false;
                // Set newHigh back to false
                if (swingCur.NewSwing == -1)
                    swingHigh.New = false;
            }

            // Swing high
            if (swingHigh.New)
            {
                int bar;
                double price;
                // New swing high
                if (swingCur.SwingSlope != 1)
                {
                    bar = CurrentBars[BarsInProgress] -
                        HighestBar(highs[BarsInProgress], CurrentBars[BarsInProgress] -
                        swingLow.CurBar);
                    price = highs[BarsInProgress][HighestBar(highs[BarsInProgress],
                        CurrentBars[BarsInProgress] - swingLow.CurBar)];
                    swingHigh.Update = false;
                }
                // Update swing high
                else
                {
                    bar = CurrentBars[BarsInProgress];
                    price = highs[BarsInProgress][0];
                    swingHigh.Update = true;
                }
                CalcUpSwing(bar, price, swingHigh.Update, swingHigh, swingLow, swingCur,
                    swingProp, upFlip, swingHighs, decimalPlaces, doubleBottom, lowerLow,
                    higherLow, doubleTop, lowerHigh, higherHigh, gannSwing);
            }
            // Swing low
            else if (swingLow.New)
            {
                int bar;
                double price;
                // New swing low
                if (swingCur.SwingSlope != -1)
                {
                    bar = CurrentBars[BarsInProgress] - LowestBar(lows[BarsInProgress],
                        CurrentBars[BarsInProgress] - swingHigh.CurBar);
                    price = lows[BarsInProgress][LowestBar(lows[BarsInProgress],
                        CurrentBars[BarsInProgress] - swingHigh.CurBar)];
                    swingLow.Update = false;
                }
                // Update swing low
                else
                {
                    bar = CurrentBars[BarsInProgress];
                    price = lows[BarsInProgress][0];
                    swingLow.Update = true;
                }
                CalcDnSwing(bar, price, swingLow.Update, swingHigh, swingLow, swingCur,
                    swingProp, dnFlip, swingLows, decimalPlaces, doubleBottom, lowerLow,
                    higherLow, doubleTop, lowerHigh, higherHigh, gannSwing);
            }
        }
        //#########################################################################################
        #endregion

        #region Calculate down swing
        //#########################################################################################
        protected void CalcDnSwing(int bar, double low, bool updateLow, Swings swingHigh,
            Swings swingLow, SwingCurrent swingCur, SwingProperties swingProp, Series<bool> dnFlip,
            List<SwingStruct> swingLows, int decimalPlaces, Series<double> doubleBottom,
            Series<double> lowerLow, Series<double> higherLow, Series<double> doubleTop, Series<double> lowerHigh,
            Series<double> higherHigh, Series<double> gannSwing)
        {
            #region New and update Swing values
            //=====================================================================================
            if (!updateLow)
            {
                if (swingProp.VisualizationType == VisualizationStyle.GannStyle)
                {
                    for (int i = CurrentBar - swingCur.SwingSlopeChangeBar; i >= 0; i--)
                        gannSwing[i] = swingHigh.CurPrice;
                    gannSwing[0] = low;
                }
                swingLow.LastPrice = swingLow.CurPrice;
                swingLow.LastBar = swingLow.CurBar;
                swingLow.LastDateTime = swingLow.CurDateTime;
                swingLow.LastDuration = swingLow.CurDuration;
                swingLow.LastLength = swingLow.CurLength;
                swingLow.LastTime = swingLow.CurTime;
                swingLow.LastPercent = swingLow.CurPercent;
                swingLow.LastRelation = swingLow.CurRelation;
                swingLow.LastVolume = swingLow.CurVolume;
                swingLow.Counter++;
                swingCur.SwingSlope = -1;
                swingCur.SwingSlopeChangeBar = bar;
                dnFlip[0] = true;
            }
            else
            {
                if (swingProp.VisualizationType == VisualizationStyle.Dots
                    || swingProp.VisualizationType == VisualizationStyle.Dots_ZigZag)
                {
                    doubleBottom.Reset(CurrentBar - swingLow.CurBar);
                    higherLow.Reset(CurrentBar - swingLow.CurBar);
                    lowerLow.Reset(CurrentBar - swingLow.CurBar);
                }
                swingLows.RemoveAt(swingLows.Count - 1);
            }
            swingLow.CurBar = bar;
            swingLow.CurPrice = Math.Round(low, decimalPlaces, MidpointRounding.AwayFromZero);
            swingLow.CurTime = ToTime(Times[BarsInProgress][CurrentBars[BarsInProgress] -
                swingLow.CurBar]);
            swingLow.CurDateTime = Times[BarsInProgress][CurrentBars[BarsInProgress] -
                swingLow.CurBar];
            swingLow.CurLength = Convert.ToInt32(Math.Round((swingLow.CurPrice -
                swingHigh.CurPrice) / TickSize, 0, MidpointRounding.AwayFromZero));
            if (swingHigh.CurLength != 0)
                swingLow.CurPercent = Math.Round(100.0 / swingHigh.CurLength *
                    Math.Abs(swingLow.CurLength), 1);
            swingLow.CurDuration = swingLow.CurBar - swingHigh.CurBar;
            double dtbOffset = ATR(BarsArray[BarsInProgress], 14)[CurrentBars[BarsInProgress] -
                swingLow.CurBar] * swingProp.DtbStrength / 100;
            if (swingLow.CurPrice > swingLow.LastPrice - dtbOffset && swingLow.CurPrice <
                swingLow.LastPrice + dtbOffset)
                swingLow.CurRelation = 0;
            else if (swingLow.CurPrice < swingLow.LastPrice)
                swingLow.CurRelation = -1;
            else
                swingLow.CurRelation = 1;
            if (Calculate != Calculate.OnBarClose)
                swingHigh.SignalBarVolume = Volumes[BarsInProgress][0];
            double swingVolume = 0.0;
            for (int i = 0; i < swingLow.CurDuration; i++)
                swingVolume = swingVolume + Volumes[BarsInProgress][i];
            if (Calculate != Calculate.OnBarClose)
                swingVolume = swingVolume + (Volumes[BarsInProgress][CurrentBars[BarsInProgress] -
                    swingHigh.CurBar] - swingLow.SignalBarVolume);
            if (swingProp.SwingVolumeType == SwingVolumeStyle.Relative)
                swingVolume = Math.Round(swingVolume / swingLow.CurDuration, 0,
                    MidpointRounding.AwayFromZero);
            swingLow.CurVolume = Convert.ToInt64(swingVolume);
            //=====================================================================================
            #endregion

            #region Visualize swing
            //=====================================================================================
            switch (swingProp.VisualizationType)
            {
                case VisualizationStyle.False:
                    break;
                case VisualizationStyle.Dots:
                    switch (swingLow.CurRelation)
                    {
                        case 1:
                            higherLow[CurrentBar - swingLow.CurBar] = swingLow.CurPrice;
                            break;
                        case -1:
                            lowerLow[CurrentBar - swingLow.CurBar] = swingLow.CurPrice;
                            break;
                        case 0:
                            doubleBottom[CurrentBar - swingLow.CurBar] = swingLow.CurPrice;
                            break;
                    }
                    break;
                case VisualizationStyle.Dots_ZigZag:
                    switch (swingLow.CurRelation)
                    {
                        case 1:
                            higherLow[CurrentBar - swingLow.CurBar] = swingLow.CurPrice;
                            break;
                        case -1:
                            lowerLow[CurrentBar - swingLow.CurBar] = swingLow.CurPrice;
                            break;
                        case 0:
                            doubleBottom[CurrentBar - swingLow.CurBar] = swingLow.CurPrice;
                            break;
                    }
                    Draw.Line(this, "ZigZagDown" + swingLow.Counter,
                        swingProp.UseAutoScale, CurrentBar - swingHigh.CurBar, swingHigh.CurPrice,
                        CurrentBar - swingLow.CurBar, swingLow.CurPrice, swingProp.ZigZagColorDn,
                        swingProp.ZigZagStyle, swingProp.ZigZagWidth, swingProp.DrawSwingsOnPricePanel);
                    break;
                case VisualizationStyle.ZigZag:
                    Draw.Line(this, "ZigZagDown" + swingLow.Counter,
                        swingProp.UseAutoScale, CurrentBar - swingHigh.CurBar, swingHigh.CurPrice,
                        CurrentBar - swingLow.CurBar, swingLow.CurPrice, swingProp.ZigZagColorDn,
                        swingProp.ZigZagStyle, swingProp.ZigZagWidth, swingProp.DrawSwingsOnPricePanel);
                    break;
                case VisualizationStyle.GannStyle:
                    for (int i = CurrentBar - swingCur.SwingSlopeChangeBar; i >= 0; i--)
                        gannSwing[i] = swingLow.CurPrice;
                    break;
                case VisualizationStyle.ZigZagVolume:
                    if (swingLow.CurVolume > swingHigh.CurVolume)
                        Draw.Line(this, "ZigZagDown" + swingLow.Counter,
                            swingProp.UseAutoScale, CurrentBar - swingHigh.CurBar, 
                            swingHigh.CurPrice, CurrentBar - swingLow.CurBar, swingLow.CurPrice, 
                            swingProp.ZigZagColorDn, swingProp.ZigZagStyle, swingProp.ZigZagWidth,
							swingProp.DrawSwingsOnPricePanel);
                    else
                        Draw.Line(this, "ZigZagDown" + swingLow.Counter,
                            swingProp.UseAutoScale, CurrentBar - swingHigh.CurBar,
                            swingHigh.CurPrice, CurrentBar - swingLow.CurBar, swingLow.CurPrice,
                            swingProp.ZigZagColorUp, swingProp.ZigZagStyle, swingProp.ZigZagWidth, 
							swingProp.DrawSwingsOnPricePanel);
                    break;
            }
            //=====================================================================================
            #endregion

            #region Swing value output
            //=====================================================================================
            string output = "";
            switch (swingProp.SwingLengthType)
            {
                case SwingLengthStyle.False:
                    break;
                case SwingLengthStyle.Ticks:
                    output = swingLow.CurLength.ToString();
                    break;
                case SwingLengthStyle.Ticks_Price:
                    output = swingLow.CurLength.ToString() + " / " + swingLow.CurPrice.ToString();
                    break;
                case SwingLengthStyle.Price_Ticks:
                    output = swingLow.CurPrice.ToString() + " / " + swingLow.CurLength.ToString();
                    break;
                case SwingLengthStyle.Points:
                    output = (swingLow.CurLength * TickSize).ToString();
                    break;
                case SwingLengthStyle.Points_Price:
                    output = (swingLow.CurLength * TickSize).ToString() + " / " +
                        swingLow.CurPrice.ToString();
                    break;
                case SwingLengthStyle.Price_Points:
                    output = swingLow.CurPrice.ToString() + " / " +
                        (swingLow.CurLength * TickSize).ToString();
                    break;
                case SwingLengthStyle.Price:
                    output = swingLow.CurPrice.ToString();
                    break;
                case SwingLengthStyle.Percent:
                    output = (Math.Round((100.0 / swingHigh.CurPrice * (swingLow.CurLength *
                        TickSize)), 2, MidpointRounding.AwayFromZero)).ToString();
                    break;
            }
            string outputDuration = "";
            TimeSpan timeSpan;
            int hours, minutes, seconds = 0;
            switch (swingProp.SwingDurationType)
            {
                case SwingDurationStyle.False:
                    break;
                case SwingDurationStyle.Bars:
                    outputDuration = swingLow.CurDuration.ToString();
                    break;
                case SwingDurationStyle.MMSS:
                    timeSpan = swingLow.CurDateTime.Subtract(swingHigh.CurDateTime);
                    minutes = timeSpan.Minutes;
                    seconds = timeSpan.Seconds;
                    if (minutes == 0)
                        outputDuration = "0:" + seconds.ToString();
                    else if (seconds == 0)
                        outputDuration = minutes + ":00";
                    else
                        outputDuration = minutes + ":" + seconds;
                    break;
                case SwingDurationStyle.HHMM:
                    timeSpan = swingLow.CurDateTime.Subtract(swingHigh.CurDateTime);
                    hours = timeSpan.Hours;
                    minutes = timeSpan.Minutes;
                    if (hours == 0)
                        outputDuration = "0:" + minutes.ToString();
                    else if (minutes == 0)
                        outputDuration = hours + ":00";
                    else
                        outputDuration = hours + ":" + minutes;
                    break;
                case SwingDurationStyle.SecondsTotal:
                    timeSpan = swingLow.CurDateTime.Subtract(swingHigh.CurDateTime);
                    outputDuration = Math.Round(timeSpan.TotalSeconds, 1,
                        MidpointRounding.AwayFromZero).ToString();
                    break;
                case SwingDurationStyle.MinutesTotal:
                    timeSpan = swingLow.CurDateTime.Subtract(swingHigh.CurDateTime);
                    outputDuration = Math.Round(timeSpan.TotalMinutes, 1,
                        MidpointRounding.AwayFromZero).ToString();
                    break;
                case SwingDurationStyle.HoursTotal:
                    timeSpan = swingLow.CurDateTime.Subtract(swingHigh.CurDateTime);
                    outputDuration = timeSpan.TotalHours.ToString();
                    break;
                case SwingDurationStyle.Days:
                    timeSpan = swingLow.CurDateTime.Subtract(swingHigh.CurDateTime);
                    outputDuration = Math.Round(timeSpan.TotalDays, 1,
                        MidpointRounding.AwayFromZero).ToString();
                    break;
            }
            if (swingProp.SwingLengthType != SwingLengthStyle.False)
            {
                if (swingProp.SwingDurationType != SwingDurationStyle.False)
                    output = output + " / " + outputDuration;
            }
            else
                output = outputDuration;

            string swingLabel = null;
            Brush textColor = Brushes.Transparent;
            switch (swingLow.CurRelation)
            {
                case 1:
                    swingLabel = "HL";
                    textColor = swingProp.TextColorHigherLow;
                    break;
                case -1:
                    swingLabel = "LL";
                    textColor = swingProp.TextColorLowerLow;
                    break;
                case 0:
                    swingLabel = "DB";
                    textColor = swingProp.TextColorDoubleBottom;
                    break;
            }
            if (output != null)
                Draw.Text(this, "DnLength" + swingLow.Counter, swingProp.UseAutoScale,
                    output.ToString(), CurrentBar - swingLow.CurBar, swingLow.CurPrice,
                    -swingProp.TextOffsetLength, textColor, swingProp.TextFont,
                    TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
            if (swingProp.ShowSwingLabel)
                Draw.Text(this, "DnLabel" + swingLow.Counter, swingProp.UseAutoScale,
                    swingLabel, CurrentBar - swingLow.CurBar, swingLow.CurPrice,
                    -swingProp.TextOffsetLabel, textColor, swingProp.TextFont,
                    TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
            if (swingProp.ShowSwingPrice)
                Draw.Text(this, "DnPrice" + swingLow.Counter, swingProp.UseAutoScale,
                    String.Format("{0:F" + decimalPlaces + "}", swingLow.CurPrice),
                    CurrentBar - swingLow.CurBar, swingLow.CurPrice, -swingProp.TextOffsetPrice,
                    textColor, swingProp.TextFont, TextAlignment.Center, Brushes.Transparent,
                    Brushes.Transparent, 0);
            if (swingProp.ShowSwingPercent && swingLow.CurPercent != 0)
                Draw.Text(this, "DnPerc" + swingLow.Counter, swingProp.UseAutoScale,
                    String.Format("{0:F1}", swingLow.CurPercent) + "%", CurrentBar - swingLow.CurBar,
                    swingLow.CurPrice, -swingProp.TextOffsetPercent, textColor,
                    swingProp.TextFont, TextAlignment.Center, Brushes.Transparent,
                    Brushes.Transparent, 0);
            if (swingProp.SwingTimeType != SwingTimeStyle.False)
            {
                string timeOutput = "";
                switch (swingProp.SwingTimeType)
                {
                    case SwingTimeStyle.False:
                        break;
                    case SwingTimeStyle.Integer:
                        timeOutput = swingLow.CurTime.ToString();
                        break;
                    case SwingTimeStyle.HHMM:
                        timeOutput = string.Format("{0:t}",
                            Times[BarsInProgress][CurrentBars[BarsInProgress] - swingLow.CurBar]);
                        break;
                    case SwingTimeStyle.HHMMSS:
                        timeOutput = string.Format("{0:T}",
                            Times[BarsInProgress][CurrentBars[BarsInProgress] - swingLow.CurBar]);
                        break;
                    case SwingTimeStyle.DDMM:
                        timeOutput = string.Format("{0:dd.MM}",
                            Times[BarsInProgress][CurrentBars[BarsInProgress] - swingHigh.CurBar]);
                        break;
                }
                Draw.Text(this, "DnTime" + swingLow.Counter, swingProp.UseAutoScale,
                    timeOutput, CurrentBar - swingLow.CurBar, swingLow.CurPrice, -swingProp.TextOffsetTime,
                    textColor, swingProp.TextFont, TextAlignment.Center, Brushes.Transparent,
                    Brushes.Transparent, 0);
            }
            if (swingProp.SwingVolumeType != SwingVolumeStyle.False)
                Draw.Text(this, "DnVolume" + swingLow.Counter, swingProp.UseAutoScale,
                    TruncIntToStr(swingLow.CurVolume), CurrentBar - swingLow.CurBar,
                    swingLow.CurPrice, -swingProp.TextOffsetVolume, textColor, swingProp.TextFont,
                    TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
            //=====================================================================================
            #endregion

            SwingStruct dn = new SwingStruct(swingLow.CurPrice, swingLow.CurBar,
                swingLow.CurDateTime, swingLow.CurDuration, swingLow.CurLength,
                swingLow.CurRelation, swingLow.CurVolume);
            swingLows.Add(dn);
            swingLow.ListCount = swingLows.Count - 1;
        }
        //#########################################################################################
        #endregion

        #region Calculate up swing
        //#########################################################################################
        private void CalcUpSwing(int bar, double high, bool updateHigh, Swings swingHigh,
            Swings swingLow, SwingCurrent swingCur, SwingProperties swingProp, Series<bool> upFlip,
            List<SwingStruct> swingHighs, int decimalPlaces, Series<double> doubleBottom, 
            Series<double> lowerLow, Series<double> higherLow, Series<double> doubleTop, Series<double> lowerHigh, 
            Series<double> higherHigh, Series<double> gannSwing)
        {
            #region New and update swing values
            //=====================================================================================
            if (!updateHigh)
            {
                if (swingProp.VisualizationType == VisualizationStyle.GannStyle)
                {
                    for (int i = CurrentBar - swingCur.SwingSlopeChangeBar; i >= 0; i--)
                        gannSwing[i] = swingLow.CurPrice;
                    gannSwing[0] = high;
                }
                swingHigh.LastPrice = swingHigh.CurPrice;
                swingHigh.LastBar = swingHigh.CurBar;
                swingHigh.LastDateTime = swingHigh.CurDateTime;
                swingHigh.LastDuration = swingHigh.CurDuration;
                swingHigh.LastLength = swingHigh.CurLength;
                swingHigh.LastTime = swingHigh.CurTime;
                swingHigh.LastPercent = swingHigh.CurPercent;
                swingHigh.LastRelation = swingHigh.CurRelation;
                swingHigh.LastVolume = swingHigh.CurVolume;
                swingHigh.Counter++;
                swingCur.SwingSlope = 1;
                swingCur.SwingSlopeChangeBar = bar;
                upFlip[0] = true;
            }
            else
            {
                if (swingProp.VisualizationType == VisualizationStyle.Dots
                    || swingProp.VisualizationType == VisualizationStyle.Dots_ZigZag)
                {
                    doubleTop.Reset(CurrentBar - swingHigh.CurBar);
                    higherHigh.Reset(CurrentBar - swingHigh.CurBar);
                    lowerHigh.Reset(CurrentBar - swingHigh.CurBar);
                }
                swingHighs.RemoveAt(swingHighs.Count - 1);
            }
            swingHigh.CurBar = bar;
            swingHigh.CurPrice = Math.Round(high, decimalPlaces, MidpointRounding.AwayFromZero);
            swingHigh.CurTime = ToTime(Times[BarsInProgress][CurrentBars[BarsInProgress] -
                swingHigh.CurBar]);
            swingHigh.CurDateTime = Times[BarsInProgress][CurrentBars[BarsInProgress] -
                swingHigh.CurBar];
            swingHigh.CurLength = Convert.ToInt32(Math.Round((swingHigh.CurPrice -
                swingLow.CurPrice) / TickSize, 0, MidpointRounding.AwayFromZero));
            if (swingLow.CurLength != 0)
                swingHigh.CurPercent = Math.Round(100.0 / Math.Abs(swingLow.CurLength) *
                    swingHigh.CurLength, 1);
            swingHigh.CurDuration = swingHigh.CurBar - swingLow.CurBar;
            double dtbOffset = ATR(BarsArray[BarsInProgress], 14)[CurrentBars[BarsInProgress] -
                swingHigh.CurBar] * swingProp.DtbStrength / 100;
            if (swingHigh.CurPrice > swingHigh.LastPrice - dtbOffset && swingHigh.CurPrice <
                swingHigh.LastPrice + dtbOffset)
                swingHigh.CurRelation = 0;
            else if (swingHigh.CurPrice < swingHigh.LastPrice)
                swingHigh.CurRelation = -1;
            else
                swingHigh.CurRelation = 1;
            if (Calculate != Calculate.OnBarClose)
                swingLow.SignalBarVolume = Volumes[BarsInProgress][0];
            double swingVolume = 0.0;
            for (int i = 0; i < swingHigh.CurDuration; i++)
                swingVolume = swingVolume + Volumes[BarsInProgress][i];
            if (Calculate != Calculate.OnBarClose)
                swingVolume = swingVolume + (Volumes[BarsInProgress][CurrentBars[BarsInProgress] -
                    swingLow.CurBar] - swingHigh.SignalBarVolume);
            if (swingProp.SwingVolumeType == SwingVolumeStyle.Relative)
                swingVolume = Math.Round(swingVolume / swingHigh.CurDuration, 0,
                    MidpointRounding.AwayFromZero);
            swingHigh.CurVolume = Convert.ToInt64(swingVolume);
            //=====================================================================================
            #endregion

            #region Visualize swing
            //=====================================================================================
            switch (swingProp.VisualizationType)
            {
                case VisualizationStyle.False:
                    break;
                case VisualizationStyle.Dots:
                    switch (swingHigh.CurRelation)
                    {
                        case 1:
                            higherHigh[CurrentBar - swingHigh.CurBar] = swingHigh.CurPrice;
                            break;
                        case -1:
                            lowerHigh[CurrentBar - swingHigh.CurBar] = swingHigh.CurPrice;
                            break;
                        case 0:
                            doubleTop[CurrentBar - swingHigh.CurBar] = swingHigh.CurPrice;
                            break;
                    }
                    break;
                case VisualizationStyle.Dots_ZigZag:
                    switch (swingHigh.CurRelation)
                    {
                        case 1:
                            higherHigh[CurrentBar - swingHigh.CurBar] = swingHigh.CurPrice;
                            break;
                        case -1:
                            lowerHigh[CurrentBar - swingHigh.CurBar] = swingHigh.CurPrice;
                            break;
                        case 0:
                            doubleTop[CurrentBar - swingHigh.CurBar] = swingHigh.CurPrice;
                            break;
                    }
                    Draw.Line(this, "ZigZagUp" + swingHigh.Counter,
                        swingProp.UseAutoScale, CurrentBar - swingLow.CurBar, swingLow.CurPrice,
                        CurrentBar - swingHigh.CurBar, swingHigh.CurPrice, swingProp.ZigZagColorUp,
                        swingProp.ZigZagStyle, swingProp.ZigZagWidth);
                    break;
                case VisualizationStyle.ZigZag:
                    Draw.Line(this, "ZigZagUp" + swingHigh.Counter,
                        swingProp.UseAutoScale, CurrentBar - swingLow.CurBar, swingLow.CurPrice,
                        CurrentBar - swingHigh.CurBar, swingHigh.CurPrice, swingProp.ZigZagColorUp,
                        swingProp.ZigZagStyle, swingProp.ZigZagWidth);
                    break;
                case VisualizationStyle.GannStyle:
                    for (int i = CurrentBar - swingCur.SwingSlopeChangeBar; i >= 0; i--)
                        gannSwing[i] = swingHigh.CurPrice;
                    break;
                case VisualizationStyle.ZigZagVolume:
                    if (swingHigh.CurVolume > swingLow.CurVolume)
                        Draw.Line(this, "ZigZagUp" + swingHigh.Counter,
                            swingProp.UseAutoScale, CurrentBar - swingLow.CurBar, 
                            swingLow.CurPrice, CurrentBar - swingHigh.CurBar, swingHigh.CurPrice, 
                            swingProp.ZigZagColorUp, swingProp.ZigZagStyle, swingProp.ZigZagWidth);
                    else
                        Draw.Line(this, "ZigZagUp" + swingHigh.Counter,
                            swingProp.UseAutoScale, CurrentBar - swingLow.CurBar,
                            swingLow.CurPrice, CurrentBar - swingHigh.CurBar, swingHigh.CurPrice,
                            swingProp.ZigZagColorDn, swingProp.ZigZagStyle, swingProp.ZigZagWidth);
                    break;
            }
            //=====================================================================================
            #endregion

            #region Swing value output
            //=====================================================================================
            string output = "";
            switch (swingProp.SwingLengthType)
            {
                case SwingLengthStyle.False:
                    break;
                case SwingLengthStyle.Ticks:
                    output = swingHigh.CurLength.ToString();
                    break;
                case SwingLengthStyle.Ticks_Price:
                    output = swingHigh.CurLength.ToString() + " / " +
                        swingHigh.CurPrice.ToString();
                    break;
                case SwingLengthStyle.Price_Ticks:
                    output = swingHigh.CurPrice.ToString() + " / " +
                        swingHigh.CurLength.ToString();
                    break;
                case SwingLengthStyle.Points:
                    output = (swingHigh.CurLength * TickSize).ToString();
                    break;
                case SwingLengthStyle.Points_Price:
                    output = (swingHigh.CurLength * TickSize).ToString() + " / " +
                        swingHigh.CurPrice.ToString();
                    break;
                case SwingLengthStyle.Price_Points:
                    output = swingHigh.CurPrice.ToString() + " / " +
                        (swingHigh.CurLength * TickSize).ToString();
                    break;
                case SwingLengthStyle.Price:
                    output = swingHigh.CurPrice.ToString();
                    break;
                case SwingLengthStyle.Percent:
                    output = (Math.Round((100.0 / swingLow.CurPrice * (swingHigh.CurLength *
                        TickSize)), 2, MidpointRounding.AwayFromZero)).ToString();
                    break;
            }
            string outputDuration = "";
            TimeSpan timeSpan;
            int hours, minutes, seconds = 0;
            switch (swingProp.SwingDurationType)
            {
                case SwingDurationStyle.False:
                    break;
                case SwingDurationStyle.Bars:
                    outputDuration = swingHigh.CurDuration.ToString();
                    break;
                case SwingDurationStyle.MMSS:
                    timeSpan = swingHigh.CurDateTime.Subtract(swingLow.CurDateTime);
                    minutes = timeSpan.Minutes;
                    seconds = timeSpan.Seconds;
                    if (minutes == 0)
                        outputDuration = "0:" + seconds.ToString();
                    else if (seconds == 0)
                        outputDuration = minutes + ":00";
                    else
                        outputDuration = minutes + ":" + seconds;
                    break;
                case SwingDurationStyle.HHMM:
                    timeSpan = swingHigh.CurDateTime.Subtract(swingLow.CurDateTime);
                    hours = timeSpan.Hours;
                    minutes = timeSpan.Minutes;
                    if (hours == 0)
                        outputDuration = "0:" + minutes.ToString();
                    else if (minutes == 0)
                        outputDuration = hours + ":00";
                    else
                        outputDuration = hours + ":" + minutes;
                    break;
                case SwingDurationStyle.SecondsTotal:
                    timeSpan = swingHigh.CurDateTime.Subtract(swingLow.CurDateTime);
                    outputDuration = Math.Round(timeSpan.TotalSeconds, 1,
                        MidpointRounding.AwayFromZero).ToString();
                    break;
                case SwingDurationStyle.MinutesTotal:
                    timeSpan = swingHigh.CurDateTime.Subtract(swingLow.CurDateTime);
                    outputDuration = Math.Round(timeSpan.TotalMinutes, 1,
                        MidpointRounding.AwayFromZero).ToString();
                    break;
                case SwingDurationStyle.HoursTotal:
                    timeSpan = swingHigh.CurDateTime.Subtract(swingLow.CurDateTime);
                    outputDuration = Math.Round(timeSpan.TotalHours, 1,
                        MidpointRounding.AwayFromZero).ToString();
                    break;
                case SwingDurationStyle.Days:
                    timeSpan = swingHigh.CurDateTime.Subtract(swingLow.CurDateTime);
                    outputDuration = Math.Round(timeSpan.TotalDays, 1,
                        MidpointRounding.AwayFromZero).ToString();
                    break;
            }
            if (swingProp.SwingLengthType != SwingLengthStyle.False)
            {
                if (swingProp.SwingDurationType != SwingDurationStyle.False)
                    output = output + " / " + outputDuration;
            }
            else
                output = outputDuration;

            string swingLabel = null;
            Brush textColor = Brushes.Transparent;
            switch (swingHigh.CurRelation)
            {
                case 1:
                    swingLabel = "HH";
                    textColor = swingProp.TextColorHigherHigh;
                    break;
                case -1:
                    swingLabel = "LH";
                    textColor = swingProp.TextColorLowerHigh;
                    break;
                case 0:
                    swingLabel = "DT";
                    textColor = swingProp.TextColorDoubleTop;
                    break;
            }
            if (output != null)
                Draw.Text(this, "UpLength" + swingHigh.Counter,
                    swingProp.UseAutoScale, output.ToString(), CurrentBar - swingHigh.CurBar,
                    swingHigh.CurPrice, swingProp.TextOffsetLength + Convert.ToInt32(textFont.Size), textColor, swingProp.TextFont,
                    TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
            if (swingProp.ShowSwingLabel)
                Draw.Text(this, "UpLabel" + swingHigh.Counter, swingProp.UseAutoScale,
                    swingLabel, CurrentBar - swingHigh.CurBar, swingHigh.CurPrice,
                    swingProp.TextOffsetLabel + Convert.ToInt32(textFont.Size), textColor, swingProp.TextFont,
                    TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
            if (swingProp.ShowSwingPrice)
                Draw.Text(this, "UpPrice" + swingHigh.Counter, swingProp.UseAutoScale,
                    String.Format("{0:F" + decimalPlaces + "}", swingHigh.CurPrice),
                    CurrentBar - swingHigh.CurBar, swingHigh.CurPrice, swingProp.TextOffsetPrice + Convert.ToInt32(textFont.Size),
                    textColor, swingProp.TextFont, TextAlignment.Center, Brushes.Transparent,
                    Brushes.Transparent, 0);
            if (swingProp.ShowSwingPercent && swingHigh.CurPercent != 0)
                Draw.Text(this, "UpPerc" + swingHigh.Counter, swingProp.UseAutoScale,
                    String.Format("{0:F1}", swingHigh.CurPercent) + "%", CurrentBar - swingHigh.CurBar,
                    swingHigh.CurPrice, swingProp.TextOffsetPercent + Convert.ToInt32(textFont.Size), textColor, swingProp.TextFont,
                    TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
            if (swingProp.SwingTimeType != SwingTimeStyle.False)
            {
                string timeOutput = "";
                switch (swingProp.SwingTimeType)
                {
                    case SwingTimeStyle.False:
                        break;
                    case SwingTimeStyle.Integer:
                        timeOutput = swingHigh.CurTime.ToString();
                        break;
                    case SwingTimeStyle.HHMM:
                        timeOutput = string.Format("{0:t}",
                            Times[BarsInProgress][CurrentBars[BarsInProgress] - swingHigh.CurBar]);
                        break;
                    case SwingTimeStyle.HHMMSS:
                        timeOutput = string.Format("{0:T}",
                            Times[BarsInProgress][CurrentBars[BarsInProgress] - swingHigh.CurBar]);
                        break;
                    case SwingTimeStyle.DDMM:
                        timeOutput = string.Format("{0:dd.MM}",
                            Times[BarsInProgress][CurrentBars[BarsInProgress] - swingHigh.CurBar]);
                        break;
                }
                Draw.Text(this, "UpTime" + swingHigh.Counter, swingProp.UseAutoScale,
                    timeOutput, CurrentBar - swingHigh.CurBar, swingHigh.CurPrice,
                    swingProp.TextOffsetTime+ Convert.ToInt32(textFont.Size), textColor, swingProp.TextFont,
                    TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
            }
            if (swingProp.SwingVolumeType != SwingVolumeStyle.False)
                Draw.Text(this, "UpVolume" + swingHigh.Counter, swingProp.UseAutoScale,
                    TruncIntToStr(swingHigh.CurVolume), CurrentBar - swingHigh.CurBar,
                    swingHigh.CurPrice, swingProp.TextOffsetVolume + Convert.ToInt32(textFont.Size), textColor, swingProp.TextFont,
                    TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
            //=========================================================================================
            #endregion

            SwingStruct up = new SwingStruct(swingHigh.CurPrice, swingHigh.CurBar,
                swingHigh.CurDateTime, swingHigh.CurDuration, swingHigh.CurLength,
                swingHigh.CurRelation, swingHigh.CurVolume);
            swingHighs.Add(up);
            swingHigh.ListCount = swingHighs.Count - 1;
        }
        //#########################################################################################
        #endregion

        #region Trunc integer to string
        //#########################################################################################
        /// <summary>
        /// Converts long integer numbers in a number-string format.
        /// </summary>
        protected string TruncIntToStr(long number)
        {
            long numberAbs = Math.Abs(number);
            string output = "";
            double convertedNumber = 0.0;
            if (numberAbs > 1000000000)
            {
                convertedNumber = Math.Round(number / 1000000000.0, 1,
                    MidpointRounding.AwayFromZero);
                output = convertedNumber.ToString() + "B";
            }
            else if (numberAbs > 1000000)
            {
                convertedNumber = Math.Round(number / 1000000.0, 1,
                    MidpointRounding.AwayFromZero);
                output = convertedNumber.ToString() + "M";
            }
            else if (numberAbs > 1000)
            {
                convertedNumber = Math.Round(number / 1000.0, 1,
                    MidpointRounding.AwayFromZero);
                output = convertedNumber.ToString() + "K";
            }
            else
                output = number.ToString();

            return output;
        }
        //#########################################################################################
        #endregion
		
		#region Properties
		
		#region Plots
        // Plots ==================================================================================
		[Browsable(false)]
        [XmlIgnore]
        public Series<double> DoubleBottom
        {
            get { return Values[0]; }
        }
		[Browsable(false)]
        [XmlIgnore]
        public Series<double> LowerLow
        {
            get { return Values[1]; }
        }
		[Browsable(false)]
        [XmlIgnore]
        public Series<double> HigherLow
        {
            get { return Values[2]; }
        }
		[Browsable(false)]
        [XmlIgnore]
        public Series<double> DoubleTop
        {
            get { return Values[3]; }
        }
		[Browsable(false)]
        [XmlIgnore]
        public Series<double> LowerHigh
        {
            get { return Values[4]; }
        }
		[Browsable(false)]
        [XmlIgnore]
        public Series<double> HigherHigh
        {
            get { return Values[5]; }
        }
		[Browsable(false)]
        [XmlIgnore]
        public Series<double> GannSwing
        {
            get { return Values[6]; }
        }
		//=========================================================================================
        #endregion

        #region Parameters
        //=========================================================================================		
		[NinjaScriptProperty]
		[Display(Name="Swing type", Description="Represents the swing type for the swings.", Order=1, GroupName="Parameters")]
		public SwingStyle SwingType
		{ get; set; }
		
		[Range(0.00000001, double.MaxValue)]
		[NinjaScriptProperty]
		[Display(Name="Swing size", Description="Represents the swing size. e.g. 1 = small swings and 5 = bigger swings.", Order=2, GroupName="Parameters")]
		public double SwingSize
		{ get; set; }
		
		[Range(1, int.MaxValue)]
		[NinjaScriptProperty]
		[Display(Name="Double top/-bottom strength", Description="Represents the double top/-bottom strength. Increase the value to get more DB/DT.", Order=3, GroupName="Parameters")]
		public int DtbStrength
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="Use close values", Description="Indicates if high and low prices are used for the swing calculations or close values.", Order=4, GroupName="Parameters")]
		public bool UseCloseValues
		{ get; set; }
        //=========================================================================================
        #endregion

        #region Swing features
        //=========================================================================================
        [NinjaScriptProperty]
        [Display(Name="ABC Pattern", Description="Indicates if and for which direction AB=CD patterns are computed." , Order=1, GroupName="0. Features")]
        public AbcPatternMode AbcPattern
        {
            get { return abcPattern; }
            set { abcPattern = value; }
        }
//        [NinjaScriptProperty]
//        [Display(Name="ABC target in percent", Description="Indicates the target in percent of the AB range for the AB=CD pattern for the risk management." , Order=2, GroupName="0. Features")]
//        public double AbcTarget
//        {
//            get { return abcTarget; }
//            set { abcTarget = Math.Max(1.0, value); }
//        }
        [NinjaScriptProperty]
        [Display(Name="Divergence indicator", Description="Represents the indicator for the divergence calculations." , Order=3, GroupName="0. Features")]
        public DivergenceMode DivergenceIndicatorMode
        {
            get { return divergenceMode; }
            set { divergenceMode = value; }
        }
        [NinjaScriptProperty]
        [Display(Name="Divergence long and short", Description="Represents the direction the divergences are calculated for." , Order=4, GroupName="0. Features")]
        public DivergenceDirection DivergenceDirectionMode
        {
            get { return divergenceDirection; }
            set { divergenceDirection = value; }
        }
        [NinjaScriptProperty]
        [Display(Name="Divergence Parameter 1", Description="Represents the first parameter for the indicator choosen in 'Divergence indicator'." , Order=5, GroupName="0. Features")]
        public int Param1
        {
            get { return param1; }
            set { param1 = Math.Max(1, value); }
        }
        [NinjaScriptProperty]
        [Display(Name="Divergence Parameter 2", Description="Represents the first parameter for the indicator choosen in 'Divergence indicator'." , Order=6, GroupName="0. Features")]
        public int Param2
        {
            get { return param2; }
            set { param2 = Math.Max(1, value); }
        }
        [NinjaScriptProperty]
        [Display(Name="Divergence Parameter 3", Description="Represents the first parameter for the indicator choosen in 'Divergence indicator'." , Order=7, GroupName="0. Features")]
        public int Param3
        {
            get { return param3; }
            set { param3 = Math.Max(1, value); }
        }
        [NinjaScriptProperty]
        [Display(Name="Naked swing lines", Description="Indicates if naked swing lines are shown." , Order=8, GroupName="0. Features")]
        public bool ShowNakedSwings
        {
            get { return showNakedSwings; }
            set { showNakedSwings = value; }
        }
//        [NinjaScriptProperty]
//        [Display(Name="Statistic", Description="Indicates if and where a swing statistic is shown." , Order=9, GroupName="0. Features")]
//        public StatisticPositionStyle StatisticPosition
//        {
//            get { return statisticPosition; }
//            set { statisticPosition = value; }
//        }
//        [NinjaScriptProperty]
//        [Display(Name="Statistic number of swings", Description="Indicates the number of swings which are used for the current swing statistic." , Order=10, GroupName="0. Features")]
//        public int StatisticLength
//        {
//            get { return statisticLength; }
//            set { statisticLength = Math.Max(1, value); }
//        }
//        [NinjaScriptProperty]
//        [Display(Name="Risk management", Description="Indicates if the risk management tools are shown. If ABC pattern is on, it calculates the risk reward ratio and the trade quantity." , Order=11, GroupName="0. Features")]
//        public RiskManagementStyle RiskManagementPosition
//        {
//            get { return riskManagementPosition; }
//            set { riskManagementPosition = value; }
//        }
//        [NinjaScriptProperty]
//        [Display(Name="RM long stop in ticks", Description="Indicates the number of ticks for a long stop. Used to get the current values when the risk management buttons are clicked." , Order=12, GroupName="0. Features")]
//        public int RiskLongStopInTicks
//        {
//            get { return riskLongStopInTicks; }
//            set { riskLongStopInTicks = Math.Max(1, value); }
//        }
//        [NinjaScriptProperty]
//        [Display(Name="RM long target in ticks", Description="Indicates the number of ticks for a long target. Used to get the current values when the risk management buttons are clicked." , Order=13, GroupName="0. Features")]
//        public int RiskLongTargetInTicks
//        {
//            get { return riskLongTargetInTicks; }
//            set { riskLongTargetInTicks = Math.Max(1, value); }
//        }
//        [NinjaScriptProperty]
//        [Display(Name="RM short stop in ticks", Description="Indicates the number of ticks for a short stop. Used to get the current values when the risk management buttons are clicked." , Order=14, GroupName="0. Features")]
//        public int RiskShortStopInTicks
//        {
//            get { return riskShortStopInTicks; }
//            set { riskShortStopInTicks = Math.Max(1, value); }
//        }
//        [NinjaScriptProperty]
//        [Display(Name="RM short target in ticks", Description="Indicates the number of ticks for a short target. Used to get the current values when the risk management buttons are clicked." , Order=15, GroupName="0. Features")]
//        public int RiskShortTargetInTicks
//        {
//            get { return riskShortTargetInTicks; }
//            set { riskShortTargetInTicks = Math.Max(1, value); }
//        }
//        [NinjaScriptProperty]
//        [Display(Name="Account size", Description="Represents the account size from which the quantity for a trade is calculated based on the account risk." , Order=16, GroupName="0. Features")]
//        public double AccountSize
//        {
//            get { return accountSize; }
//            set { accountSize = Math.Max(1.0, value); }
//        }
//        [NinjaScriptProperty]
//        [Display(Name="Account risk per trade", Description="Represents the percentage of the account size which is risked for each trade." , Order=17, GroupName="0. Features")]
//        public double AccountRisk
//        {
//            get { return accountRisk; }
//            set { accountRisk = Math.Max(0.001, value); }
//        }
        [NinjaScriptProperty]
        [Display(Name="Swing extensions", Description="Indicates if a swing extension is drawn on the chart." , Order=18, GroupName="0. Features")]
       	public bool AddSwingExtension
        {
            get { return addSwingExtension; }
            set { addSwingExtension = value; }
        }
        [NinjaScriptProperty]
        [Display(Name="Swing retracement (current)", Description="Indicates if a swing retracement is drawn on the chart for the current swing." , Order=19, GroupName="0. Features")]
        public bool AddSwingRetracementFast
        {
            get { return addSwingRetracementFast; }
            set { addSwingRetracementFast = value; }
        }
        [NinjaScriptProperty]
        [Display(Name="Swing retracement (last)", Description="Indicates if a swing retracement is drawn on the chart for the last swing." , Order=20, GroupName="0. Features")]
        public bool AddSwingRetracementSlow
        {
            get { return addSwingRetracementSlow; }
            set { addSwingRetracementSlow = value; }
        }
        //=========================================================================================
        #endregion

		#region Swings Values
		//=========================================================================================
		[NinjaScriptProperty]
		[Display(Name = "Length", Description = "Represents the swing length visualization type for the swings.", Order = 1, GroupName = "1. Swing Values")]
		public SwingLengthStyle SwingLengthType
		{
			get { return swingLengthType; }
			set { swingLengthType = value; }
		}

		[NinjaScriptProperty]
		[Display(Name = "Duration", Description = "Represents the swing duration visualization type for the swings.", Order = 2, GroupName = "1. Swing Values")]
		public SwingDurationStyle SwingDurationType
		{
			get { return swingDurationType; }
			set { swingDurationType = value; }
		}

		[NinjaScriptProperty]
		[Display(Name = "Show Price", Description = "Indicates if the swing price is shown for the swings.", Order = 3, GroupName = "1. Swing Values")]
		public bool ShowSwingPrice
		{
			get { return showSwingPrice; }
			set { showSwingPrice = value; }
		}

		[NinjaScriptProperty]
		[Display(Name = "Show Labels", Description = "Indicates if the swing label is shown (HH, HL, LL, LH, DB, DT).", Order = 4, GroupName = "1. Swing Values")]
		public bool ShowSwingLabel
		{
			get { return showSwingLabel; }
			set { showSwingLabel = value; }
		}

		[NinjaScriptProperty]
		[Display(Name = "Percentage", Description = "Indicates if the swing percentage in relation to the last swing is shown.", Order = 5, GroupName = "1. Swing Values")]
		public bool ShowSwingPercent
		{
			get { return showSwingPercent; }
			set { showSwingPercent = value; }
		}

		[NinjaScriptProperty]
		[Display(Name = "Time", Description = "Represents the swing time visualization type for the swings.", Order = 6, GroupName = "1. Swing Values")]
		public SwingTimeStyle SwingTimeType
		{
			get { return swingTimeType; }
			set { swingTimeType = value; }
		}

		[NinjaScriptProperty]
		[Display(Name = "Volume", Description = "Represents the swing volume visualization type for the swings.", Order = 7, GroupName = "1. Swing Values")]
		public SwingVolumeStyle SwingVolumeType
		{
			get { return swingVolumeType; }
			set { swingVolumeType = value; }
		}
		//=========================================================================================
		#endregion

		#region Visualize Swings
		//=========================================================================================
		[NinjaScriptProperty]
		[Display(Name = "Visualization Type", Description = "Represents the swing visualization type for the swings.", Order = 1, GroupName = "2. Visualize Swings")]
		public VisualizationStyle VisualizationType
		{
			get { return visualizationType; }
			set { visualizationType = value; }
		}

		[NinjaScriptProperty]
		[Display(Name = "Text Font", Description = "Represents the text font for the swing value output.", Order = 2, GroupName = "2. Visualize Swings")]
		public NinjaTrader.Gui.Tools.SimpleFont TextFont
		{
			get { return textFont; }
			set { textFont = value; }
		}

		[XmlIgnore]
		[Display(Name = "Text Color Higher High", Description = "Represents the color of the swing value output for higher highs.", Order = 3, GroupName = "2. Visualize Swings")]
		public Brush TextColorHigherHigh
		{
			get { return textColorHigherHigh; }
			set { textColorHigherHigh = value; }
		}

		[Browsable(false)]
		public string TextColorHigherHighSerializable
		{
			get { return Serialize.BrushToString(textColorHigherHigh); }
			set { textColorHigherHigh = Serialize.StringToBrush(value); }
		}

		[XmlIgnore]
		[Display(Name = "Text Color Lower High", Description = "Represents the color of the swing value output for lower highs.", Order = 4, GroupName = "2. Visualize Swings")]
		public Brush TextColorLowerHigh
		{
			get { return textColorLowerHigh; }
			set { textColorLowerHigh = value; }
		}
		[Browsable(false)]
		public string TextColorLowerHighSerializable
		{
			get { return Serialize.BrushToString(textColorLowerHigh); }
			set { textColorLowerHigh = Serialize.StringToBrush(value); }
		}

		[XmlIgnore]
		[Display(Name = "Text Color Double Top", Description = "Represents the color of the swing value output for double tops.", Order = 5, GroupName = "2. Visualize Swings")]
		public Brush TextColorDoubleTop
		{
			get { return textColorDoubleTop; }
			set { textColorDoubleTop = value; }
		}
		[Browsable(false)]
		public string TextColorDoubleTopSerializable
		{
			get { return Serialize.BrushToString(textColorDoubleTop); }
			set { textColorDoubleTop = Serialize.StringToBrush(value); }
		}

		[XmlIgnore]
		[Display(Name = "Text Color Higher Low", Description = "Represents the color of the swing value output for higher lows.", Order = 6, GroupName = "2. Visualize Swings")]
		public Brush TextColorHigherLow
		{
			get { return textColorHigherLow; }
			set { textColorHigherLow = value; }
		}
		[Browsable(false)]
		public string TextColorHigherLowSerializable
		{
			get { return Serialize.BrushToString(textColorHigherLow); }
			set { textColorHigherLow = Serialize.StringToBrush(value); }
		}

		[XmlIgnore]
		[Display(Name = "Text Color Lower Low", Description = "Represents the color of the swing value output for lower lows.", Order = 7, GroupName = "2. Visualize Swings")]
		public Brush TextColorLowerLow
		{
			get { return textColorLowerLow; }
			set { textColorLowerLow = value; }
		}
		[Browsable(false)]
		public string TextColorLowerLowSerializable
		{
			get { return Serialize.BrushToString(textColorLowerLow); }
			set { textColorLowerLow = Serialize.StringToBrush(value); }
		}

		[XmlIgnore]
		[Display(Name = "Text Color Double Bottom", Description = "Represents the color of the swing value output for double bottoms.", Order = 8, GroupName = "2. Visualize Swings")]
		public Brush TextColorDoubleBottom
		{
			get { return textColorDoubleBottom; }
			set { textColorDoubleBottom = value; }
		}
		[Browsable(false)]
		public string TextColorDoubleBottomSerializable
		{
			get { return Serialize.BrushToString(textColorDoubleBottom); }
			set { textColorDoubleBottom = Serialize.StringToBrush(value); }
		}

		[Range(1, int.MaxValue)]
		[NinjaScriptProperty]
		[Display(Name = "Text Offset Length/Duration", Description = "Represents the text offset in pixel for the swing length/duration.", Order = 9, GroupName = "2. Visualize Swings")]
		public int TextOffsetLength
		{
			get { return textOffsetLength; }
			set { textOffsetLength = Math.Max(1, value); }
		}

		[Range(1, int.MaxValue)]
		[NinjaScriptProperty]
		[Display(Name = "Text Offset Volume", Description = "Represents the text offset in pixel for the swing volume.", Order = 10, GroupName = "2. Visualize Swings")]
		public int TextOffsetVolume
		{
			get { return textOffsetVolume; }
			set { textOffsetVolume = Math.Max(1, value); }
		}

		[Range(1, int.MaxValue)]
		[NinjaScriptProperty]
		[Display(Name = "Text Offset Price", Description = "Represents the text offset in pixel for the swing price for the swings.", Order = 11, GroupName = "2. Visualize Swings")]
		public int TextOffsetPrice
		{
			get { return textOffsetPrice; }
			set { textOffsetPrice = Math.Max(1, value); }
		}

		[Range(1, int.MaxValue)]
		[NinjaScriptProperty]
		[Display(Name = "Text Offset Swing Labels", Description = "Represents the text offset in pixel for the swing labels.", Order = 12, GroupName = "2. Visualize Swings")]
		public int TextOffsetLabel
		{
			get { return textOffsetLabel; }
			set { textOffsetLabel = Math.Max(1, value); }
		}

		[Range(1, int.MaxValue)]
		[NinjaScriptProperty]
		[Display(Name = "Text Offset Time", Description = "Represents the text offset in pixel for the time value.", Order = 13, GroupName = "2. Visualize Swings")]
		public int TextOffsetTime
		{
			get { return textOffsetTime; }
			set { textOffsetTime = Math.Max(1, value); }
		}

		[Range(1, int.MaxValue)]
		[NinjaScriptProperty]
		[Display(Name = "Text Offset Percent", Description = "Represents the text offset in pixel for the retracement value.", Order = 14, GroupName = "2. Visualize Swings")]
		public int TextOffsetPercent
		{
			get { return textOffsetPercent; }
			set { textOffsetPercent = Math.Max(1, value); }
		}

		[XmlIgnore]
		[Display(Name = "Zig-Zag Color Up", Description = "Represents the color of the zig-zag up lines.", Order = 15, GroupName = "2. Visualize Swings")]
		public Brush ZigZagColorUp
		{
			get { return zigZagColorUp; }
			set { zigZagColorUp = value; }
		}
		[Browsable(false)]
		public string ZigZagColorUpSerializable
		{
			get { return Serialize.BrushToString(zigZagColorUp); }
			set { zigZagColorUp = Serialize.StringToBrush(value); }
		}

		[XmlIgnore]
		[Display(Name = "Zig-Zag Color Down", Description = "Represents the color of the zig-zag down lines.", Order = 16, GroupName = "2. Visualize Swings")]
		public Brush ZigZagColorDn
		{
			get { return zigZagColorDn; }
			set { zigZagColorDn = value; }
		}
		[Browsable(false)]
		public string ZigZagColorDnSerializable
		{
			get { return Serialize.BrushToString(zigZagColorDn); }
			set { zigZagColorDn = Serialize.StringToBrush(value); }
		}

		[NinjaScriptProperty]
		[Display(Name = "Zig-Zag Style", Description = "Represents the line style of the zig-zag lines.", Order = 17, GroupName = "2. Visualize Swings")]
		public DashStyleHelper ZigZagStyle
		{
			get { return zigZagStyle; }
			set { zigZagStyle = value; }
		}

		[Range(1, int.MaxValue)]
		[NinjaScriptProperty]
		[Display(Name = "Zig-Zag Width", Description = "Represents the line width of the zig-zag lines.", Order = 18, GroupName = "2. Visualize Swings")]
		public int ZigZagWidth
		{
			get { return zigZagWidth; }
			set { zigZagWidth = Math.Max(1, value); }
		}
        [NinjaScriptProperty]
        [Display(Name = "Swing switch", Description = "Indicates if the change from down to up or up to down swings is indicated on the chart for the swings.", Order = 18, GroupName = "2. Visualize Swings")]
        public bool ShowSwingSwitch
        {
            get { return showSwingSwitch; }
            set { showSwingSwitch = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "Swing switch offset in ticks", Description = "Represents the offset in ticks for the swing switch triangles.", Order = 20, GroupName = "2. Visualize Swings")]
        public int SwingSwitchOffsetInTicks
        {
            get { return swingSwitchOffsetInTicks; }
            set { swingSwitchOffsetInTicks = value; }
        }
        
		[XmlIgnore]
        [Display(Name = "Swing switch down color", Description = "Represents the color of the swing switch down triangle.", Order = 21, GroupName = "2. Visualize Swings")]
        public Brush SwingSwitchDownColor
        {
            get { return swingSwitchDownColor; }
            set { swingSwitchDownColor = value; }
        }
        [Browsable(false)]
        public string SwingSwitchDownColorSerialize
        {
            get { return Serialize.BrushToString(swingSwitchDownColor); }
            set { swingSwitchDownColor = Serialize.StringToBrush(value); }
        }
        
		[XmlIgnore]
        [Display(Name = "Swing switch up color", Description = "Represents the color of the swing switch up triangle.", Order = 22, GroupName = "2. Visualize Swings")]
        public Brush SwingSwitchUpColor
        {
            get { return swingSwitchUpColor; }
            set { swingSwitchUpColor = value; }
        }
        [Browsable(false)]
        public string SwingSwitchUpColorSerialize
        {
            get { return Serialize.BrushToString(swingSwitchUpColor); }
            set { swingSwitchUpColor = Serialize.StringToBrush(value); }
        }
		//=========================================================================================
		#endregion

        #region ABC visualization
        //=====================================================================
        [NinjaScriptProperty]
        [Display(Name="Line style", Description="Represents the line style for pattern lines.", Order = 1, GroupName = "4. ABC Visualization")]
        public DashStyleHelper AbcLineStyle
        {
            get { return abcLineStyle; }
            set { abcLineStyle = value; }
        }
        [NinjaScriptProperty]
        [Display(Name="Line style ratio", Description="Represents the line style for pattern ratio lines.", Order = 2, GroupName = "4. ABC Visualization")]
        public DashStyleHelper AbcLineStyleRatio
        {
            get { return abcLineStyleRatio; }
            set { abcLineStyleRatio = value; }
        }
		[Range(1, int.MaxValue)]
        [NinjaScriptProperty]
        [Display(Name="Line width", Description="Represents the line width for pattern lines.", Order = 3, GroupName = "4. ABC Visualization")]
        public int AbcLineWidth
        {
            get { return abcLineWidth; }
            set { abcLineWidth = Math.Max(1, value); }
        }
		[Range(1, int.MaxValue)]
        [NinjaScriptProperty]
        [Display(Name="Line width ratio", Description="Represents the line width for pattern ratio lines.", Order = 4, GroupName = "4. ABC Visualization")]
        public int AbcLineWidthRatio
        {
            get { return abcLineWidthRatio; }
            set { abcLineWidthRatio = Math.Max(1, value); }
        }
        [XmlIgnore()]
        [NinjaScriptProperty]
        [Display(Name="Text font", Description="Represents the text font for the displayed swing information.", Order = 5, GroupName = "4. ABC Visualization")]
        public NinjaTrader.Gui.Tools.SimpleFont AbcTextFont
        {
            get { return abcTextFont; }
            set { abcTextFont = value; }
        }
		[Range(1, int.MaxValue)]
        [NinjaScriptProperty]
        [Display(Name="Text offset label", Description="Represents the offset value in pixels from within the text box area that display the swing label.", Order = 6, GroupName = "4. ABC Visualization")]
        public int AbcTextOffsetLabel
        {
            get { return abcTextOffsetLabel; }
            set { abcTextOffsetLabel = Math.Max(1, value); }
        }
        
		[XmlIgnore]
        [Display(Name="Text color down", Description="Represents the text color for down patterns.", Order = 7, GroupName = "4. ABC Visualization")]
        public Brush AbcTextColorDn
        {
            get { return abcTextColorDn; }
            set { abcTextColorDn = value; }
        }
        [Browsable(false)]
        public string AbcTextColorDnSerialize
        {
            get { return Serialize.BrushToString(abcTextColorDn); }
            set { abcTextColorDn = Serialize.StringToBrush(value); }
        }
        
		[XmlIgnore]
        [Display(Name="Text color up", Description="Represents the text color for up patterns.", Order = 8, GroupName = "4. ABC Visualization")]
        public Brush AbcTextColorUp
        {
            get { return abcTextColorUp; }
            set { abcTextColorUp = value; }
        }
        [Browsable(false)]
        public string AbcTextColorUpSerialize
        {
            get { return Serialize.BrushToString(abcTextColorUp); }
            set { abcTextColorUp = Serialize.StringToBrush(value); }
        }
        
		[XmlIgnore]
        [Display(Name="Line color down", Description="Represents the line color for down patterns.", Order = 9, GroupName = "4. ABC Visualization")]
        public Brush AbcZigZagColorDn
        {
            get { return abcZigZagColorDn; }
            set { abcZigZagColorDn = value; }
        }
        [Browsable(false)]
        public string AbcZigZagColorDnSerialize
        {
            get { return Serialize.BrushToString(abcZigZagColorDn); }
            set { abcZigZagColorDn = Serialize.StringToBrush(value); }
        }
        
		[XmlIgnore]
        [Display(Name="Line color up", Description="Represents the line color for up patterns.", Order = 10, GroupName = "4. ABC Visualization")]
        public Brush AbcZigZagColorUp
        {
            get { return abcZigZagColorUp; }
            set { abcZigZagColorUp = value; }
        }
        [Browsable(false)]
        public string AbcZigZagColorUpSerialize
        {
            get { return Serialize.BrushToString(abcZigZagColorUp); }
            set { abcZigZagColorUp = Serialize.StringToBrush(value); }
        }
		[Range(1, 99)]
        [NinjaScriptProperty]
        [Display(Name="Retracement maximum (percent)", Description="Represents the maximum value in percent for a retracement in relation to the last swing. The price must retrace between this two values, otherwise the pattern is not valid.", Order = 11, GroupName = "4. ABC Visualization")]
        public double AbcMaxRetracement
        {
            get { return abcMaxRetracement; }
            set { abcMaxRetracement = Math.Max(1, Math.Min(99, value)); }
        }
		[Range(1, 99)]
        [NinjaScriptProperty]
        [Display(Name="Retracement minimum (percent)", Description="Represents the minimum value in percent for a retracement in relation to the last swing. The price must retrace between this two values, otherwise the pattern is not valid.", Order = 12, GroupName = "4. ABC Visualization")]
        public double AbcMinRetracement
        {
            get { return abcMinRetracement; }
            set { abcMinRetracement = Math.Max(1, Math.Min(99, value)); }
        }
        
		[XmlIgnore]
        [Display(Name="Entry line color down", Description="Represents the entry line color for down patterns.", Order = 13, GroupName = "4. ABC Visualization")]
        public Brush EntryLineColorDn
        {
            get { return entryLineColorDn; }
            set { entryLineColorDn = value; }
        }
        [Browsable(false)]
        public string EntryLineColorDnSerialize
        {
            get { return Serialize.BrushToString(entryLineColorDn); }
            set { entryLineColorDn = Serialize.StringToBrush(value); }
        }
        
		[XmlIgnore]
        [Display(Name="Entry line color up", Description="Represents the entry line color for up patterns.", Order = 14, GroupName = "4. ABC Visualization")]
        public Brush EntryLineColorUp
        {
            get { return entryLineColorUp; }
            set { entryLineColorUp = value; }
        }
        [Browsable(false)]
        public string EntryLineColorUpSerialize
        {
            get { return Serialize.BrushToString(entryLineColorUp); }
            set { entryLineColorUp = Serialize.StringToBrush(value); }
        }
        [NinjaScriptProperty]
        [Display(Name="Entry line style", Description="Represents the line style for the entry lines.", Order = 15, GroupName = "4. ABC Visualization")]
        public DashStyleHelper EntryLineStyle
        {
            get { return entryLineStyle; }
            set { entryLineStyle = value; }
        }
		[Range(1, int.MaxValue)]
        [NinjaScriptProperty]
        [Display(Name="Entry line width", Description="Represents the line width for pattern lines.", Order = 16, GroupName = "4. ABC Visualization")]
        public int EntryLineWidth
        {
            get { return entryLineWidth; }
            set { entryLineWidth = Math.Max(1, value); }
        }
        [NinjaScriptProperty]
        [Display(Name="Entry retracement", Description="If bar close above/below the entry retracement an entry is triggered.", Order = 17, GroupName = "4. ABC Visualization")]
        public double RetracementEntryValue
        {
            get { return retracementEntryValue; }
            set { retracementEntryValue = Math.Max(1.0, Math.Min(99.0, value)); }
        }
        [NinjaScriptProperty]
        [Display(Name="Entry arrows", Description="Indicates if entry arrows are displayed.", Order = 18, GroupName = "4. ABC Visualization")]
        public bool ShowEntryArrows
        {
            get { return showEntryArrows; }
            set { showEntryArrows = value; }
        }
        [NinjaScriptProperty]
        [Display(Name="Entry line historical", Description="Indicates if historical entry lines are displayed.", Order = 19, GroupName = "4. ABC Visualization")]
        public bool ShowHistoricalEntryLine
        {
            get { return showHistoricalEntryLine; }
            set { showHistoricalEntryLine = value; }
        }
		[Range(0, int.MaxValue)]
        [NinjaScriptProperty]
        [Display(Name="Entry arrows offset (ticks)", Description="Represents the offset value in ticks from the high/low that triggered the entry.", Order = 20, GroupName = "4. ABC Visualization")]
        public int YTickOffset
        {
            get { return yTickOffset; }
            set { yTickOffset = Math.Max(0, value); }
        }
        //=====================================================================
        #endregion

        #region Divergence Visualization
        //=========================================================================================
        [NinjaScriptProperty]
        [Display(Name="Show regular divergence", Description="Indicates if regalur divergence is shown.", Order = 1, GroupName = "5. Divergence Visualization")]
        public bool ShowDivergenceRegular
        {
            get { return showDivergenceRegular; }
            set { showDivergenceRegular = value; }
        }
        [NinjaScriptProperty]
        [Display(Name="Show hidden divergence", Description="Indicates if hidden divergence is shown.", Order = 2, GroupName = "5. Divergence Visualization")]
        public bool ShowDivergenceHidden
        {
            get { return showDivergenceHidden; }
            set { showDivergenceHidden = value; }
        }
        [NinjaScriptProperty]
        [Display(Name="Down line style", Description="Represents the line style for down divergence.", Order = 3, GroupName = "5. Divergence Visualization")]
        public DashStyleHelper DivDnLineStyle
        {
            get { return divDnLineStyle; }
            set { divDnLineStyle = value; }
        }
        [NinjaScriptProperty]
        [Display(Name="Up line style", Description="Represents the line style for hidden divergence.", Order = 4, GroupName = "5. Divergence Visualization")]
        public DashStyleHelper DivUpLineStyle
        {
            get { return divUpLineStyle; }
            set { divUpLineStyle = value; }
        }
		[Range(1, int.MaxValue)]
        [NinjaScriptProperty]
        [Display(Name="Down line width", Description="Represents the line width for regular divergence.", Order = 5, GroupName = "5. Divergence Visualization")]
        public int DivDnLineWidth
        {
            get { return divDnLineWidth; }
            set { divDnLineWidth = Math.Max(1, value); }
        }
		[Range(1, int.MaxValue)]
        [NinjaScriptProperty]
        [Display(Name="Up line widt", Description="Represents the line width for hidden divergence.", Order = 6, GroupName = "5. Divergence Visualization")]
        public int DivUpLineWidth
        {
            get { return divUpLineWidth; }
            set { divUpLineWidth = Math.Max(1, value); }
        }
        
		[XmlIgnore]
        [Display(Name="Down text color", Description="Represents the text color for regular divergence.", Order = 7, GroupName = "5. Divergence Visualization")]
        public Brush DivDnLineColor
        {
            get { return divDnLineColor; }
            set { divDnLineColor = value; }
        }
        [Browsable(false)]
        public string DivDnLineColorSerialize
        {
            get { return Serialize.BrushToString(divDnLineColor); }
            set { divDnLineColor = Serialize.StringToBrush(value); }
        }
        
		[XmlIgnore]
        [Display(Name="Up text color", Description="Represents the text color for hidden divergence.", Order = 8, GroupName = "5. Divergence Visualization")]
        public Brush DivUpLineColor
        {
            get { return divUpLineColor; }
            set { divUpLineColor = value; }
        }
        [Browsable(false)]
        public string DivUpLineColorSerialize
        {
            get { return Serialize.BrushToString(divUpLineColor); }
            set { divUpLineColor = Serialize.StringToBrush(value); }
        }
        //=========================================================================================
        #endregion

        #region Naked swings visualization
        //=========================================================================================
        [NinjaScriptProperty]
        [Display(Name="Show historical naked swing lines", Description="Indicates if historical naked swing lines are shown.", Order = 1, GroupName = "6. Naked swings")]
        public bool ShowHistoricalNakedSwings
        {
            get { return showHistoricalNakedSwings; }
            set { showHistoricalNakedSwings = value; }
        }
        
		[XmlIgnore]
        [Display(Name="Naked swing high color", Description="Represents the color of the naked swing high lines.", Order = 2, GroupName = "6. Naked swings")]
        public Brush NakedSwingHighColor
        {
            get { return nakedSwingHighColor; }
            set { nakedSwingHighColor = value; }
        }
        [Browsable(false)]
        public string NakedSwingHighColorSerialize
        {
            get { return Serialize.BrushToString(nakedSwingHighColor); }
            set { nakedSwingHighColor = Serialize.StringToBrush(value); }
        }
        
		[XmlIgnore]
        [Display(Name="Naked swing low color", Description="Represents the color of the naked swing low lines.", Order = 3, GroupName = "6. Naked swings")]
        public Brush NakedSwingLowColor
        {
            get { return nakedSwingLowColor; }
            set { nakedSwingLowColor = value; }
        }
        [Browsable(false)]
        public string NakedSwingLowColorSerialize
        {
            get { return Serialize.BrushToString(nakedSwingLowColor); }
            set { nakedSwingLowColor = Serialize.StringToBrush(value); }
        }
        [NinjaScriptProperty]
        [Display(Name="Naked swing line style", Description="Represents the line style of the naked swing lines.", Order = 4, GroupName = "6. Naked swings")]
        public DashStyleHelper NakedSwingDashStyle
        {
            get { return nakedSwingDashStyle; }
            set { nakedSwingDashStyle = value; }
        }
		[Range(1, int.MaxValue)]
        [NinjaScriptProperty]
        [Display(Name="Naked swing line width", Description="Represents the line width of the naked swing lines.", Order = 5, GroupName = "6. Naked swings")]
        public int NakedSwingLineWidth
        {
            get { return nakedSwingLineWidth; }
            set { nakedSwingLineWidth = Math.Max(1, value); }
        }
        //=========================================================================================
        #endregion
		
		#region Gann Swings
		//=========================================================================================
		[NinjaScriptProperty]
		[Display(Name = "Ignore Inside Bars", Description = "Indicates if inside bars are ignored. If set to true it is possible that between consecutive up/down bars are inside bars. Only used if calculationSize > 1.", Order = 1, GroupName = "3. Gann Swings")]
		public bool IgnoreInsideBars
		{
			get { return ignoreInsideBars; }
			set { ignoreInsideBars = value; }
		}

		[NinjaScriptProperty]
		[Display(Name = "Use Breakouts", Description = "Indicates if the swings are updated if the last swing high/low is broken. Only used if calculationSize > 1.", Order = 2, GroupName = "3. Gann Swings")]
		public bool UseBreakouts
		{
			get { return useBreakouts; }
			set { useBreakouts = value; }
		}
		//=========================================================================================
		#endregion

        #region Alerts
        //=========================================================================================
        #region ABC
        //-----------------------------------------------------------------------------------------
        [NinjaScriptProperty]
        [Display(Name = "01. Alert ABC", Order = 01, GroupName = "7. Alerts")]
        public bool AlertAbc
        {
            get { return alertAbc; }
            set { alertAbc = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "02. Alert ABC entry", Order = 02, GroupName = "7. Alerts")]
        public bool AlertAbcEntry
        {
            get { return alertAbcEntry; }
            set { alertAbcEntry = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "03. Priority", Order = 03, GroupName = "7. Alerts")]
        public Priority AlertAbcPriority
        {
            get { return alertAbcPriority; }
            set { alertAbcPriority = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "04. Entry priority", Order = 04, GroupName = "7. Alerts")]
        public Priority AlertAbcEntryPriority
        {
            get { return alertAbcEntryPriority; }
            set { alertAbcEntryPriority = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "05. Long sound file name", Order = 05, GroupName = "7. Alerts")]
        public string AlertAbcLongSoundFileName
        {
            get { return alertAbcLongSoundFileName; }
            set { alertAbcLongSoundFileName = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "06. Long entry sound file name", Order = 06, GroupName = "7. Alerts")]
        public string AlertAbcLongEntrySoundFileName
        {
            get { return alertAbcLongEntrySoundFileName; }
            set { alertAbcLongEntrySoundFileName = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "07. Short sound file name", Order = 07, GroupName = "7. Alerts")]
        public string AlertAbcShortSoundFileName
        {
            get { return alertAbcShortSoundFileName; }
            set { alertAbcShortSoundFileName = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "08. Short entry sound file name", Order = 08, GroupName = "7. Alerts")]
        public string AlertAbcShortEntrySoundFileName
        {
            get { return alertAbcShortEntrySoundFileName; }
            set { alertAbcShortEntrySoundFileName = value; }
        }
        //-----------------------------------------------------------------------------------------
        #endregion

        #region Double bottem
        //-----------------------------------------------------------------------------------------
        [NinjaScriptProperty]
        [Display(Name = "11. Alert double bottem", Order = 11, GroupName = "7. Alerts")]
        public bool AlertDoubleBottom
        {
            get { return alertDoubleBottom; }
            set { alertDoubleBottom = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "12. Priority", Order = 12, GroupName = "7. Alerts")]
        public Priority AlertDoubleBottomPriority
        {
            get { return alertDoubleBottomPriority; }
            set { alertDoubleBottomPriority = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "13. Message", Order = 13, GroupName = "7. Alerts")]
        public string AlertDoubleBottomMessage
        {
            get { return alertDoubleBottomMessage; }
            set { alertDoubleBottomMessage = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "14. Sound file name", Order = 14, GroupName = "7. Alerts")]
        public string AlertDoubleBottomSoundFileName
        {
            get { return alertDoubleBottomSoundFileName; }
            set { alertDoubleBottomSoundFileName = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "15. Rearm seconds", Order = 15, GroupName = "7. Alerts")]
        public int AlertDoubleBottomRearmSeconds
        {
            get { return alertDoubleBottomRearmSeconds; }
            set { alertDoubleBottomRearmSeconds = value; }
        }
        
		[XmlIgnore]
        [Display(Name = "16. Back color", Order = 16, GroupName = "7. Alerts")]
        public Brush AlertDoubleBottomBackColor
        {
            get { return alertDoubleBottomBackColor; }
            set { alertDoubleBottomBackColor = value; }
        }
        [Browsable(false)]
        public string AlertDoubleBottomBackColorSerialize
        {
            get { return Serialize.BrushToString(alertDoubleBottomBackColor); }
            set { alertDoubleBottomBackColor = Serialize.StringToBrush(value); }
        }
        
		[XmlIgnore]
        [Display(Name = "17. Fore color", Order = 17, GroupName = "7. Alerts")]
        public Brush AlertDoubleBottomForeColor
        {
            get { return alertDoubleBottomForeColor; }
            set { alertDoubleBottomForeColor = value; }
        }
        [Browsable(false)]
        public string AlertDoubleBottomForeColorSerialize
        {
            get { return Serialize.BrushToString(alertDoubleBottomForeColor); }
            set { alertDoubleBottomForeColor = Serialize.StringToBrush(value); }
        }
        //-----------------------------------------------------------------------------------------
        #endregion

        #region Double top
        //-----------------------------------------------------------------------------------------
        [NinjaScriptProperty]
        [Display(Name = "21. Alert double top", Order = 21, GroupName = "7. Alerts")]
        public bool AlertDoubleTop
        {
            get { return alertDoubleTop; }
            set { alertDoubleTop = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "22. Priority", Order = 22, GroupName = "7. Alerts")]
        public Priority AlertDoubleTopPriority
        {
            get { return alertDoubleTopPriority; }
            set { alertDoubleTopPriority = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "23. Message", Order = 23, GroupName = "7. Alerts")]
        public string AlertDoubleTopMessage
        {
            get { return alertDoubleTopMessage; }
            set { alertDoubleTopMessage = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "24. Sound file name", Order = 24, GroupName = "7. Alerts")]
        public string AlertDoubleTopSoundFileName
        {
            get { return alertDoubleTopSoundFileName; }
            set { alertDoubleTopSoundFileName = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "25. Rearm seconds", Order = 25, GroupName = "7. Alerts")]
        public int AlertDoubleTopRearmSeconds
        {
            get { return alertDoubleTopRearmSeconds; }
            set { alertDoubleTopRearmSeconds = value; }
        }
        
		[XmlIgnore]
        [Display(Name = "26. Back color", Order = 26, GroupName = "7. Alerts")]
        public Brush AlertDoubleTopBackColor
        {
            get { return alertDoubleTopBackColor; }
            set { alertDoubleTopBackColor = value; }
        }
        [Browsable(false)]
        public string AlertDoubleTopBackColorSerialize
        {
            get { return Serialize.BrushToString(alertDoubleTopBackColor); }
            set { alertDoubleTopBackColor = Serialize.StringToBrush(value); }
        }
        
		[XmlIgnore]
        [Display(Name = "27. Fore color", Order = 27, GroupName = "7. Alerts")]
        public Brush AlertDoubleTopForeColor
        {
            get { return alertDoubleTopForeColor; }
            set { alertDoubleTopForeColor = value; }
        }
        [Browsable(false)]
        public string AlertDoubleTopForeColorSerialize
        {
            get { return Serialize.BrushToString(alertDoubleTopForeColor); }
            set { alertDoubleTopForeColor = Serialize.StringToBrush(value); }
        }
        //-----------------------------------------------------------------------------------------
        #endregion

        #region Higher low
        //-----------------------------------------------------------------------------------------
        [NinjaScriptProperty]
        [Display(Name = "31. Alert higher low", Order = 31, GroupName = "7. Alerts")]
        public bool AlertHigherLow
        {
            get { return alertHigherLow; }
            set { alertHigherLow = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "32. Priority", Order = 32, GroupName = "7. Alerts")]
        public Priority AlertHigherLowPriority
        {
            get { return alertHigherLowPriority; }
            set { alertHigherLowPriority = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "33. Message", Order = 33, GroupName = "7. Alerts")]
        public string AlertHigherLowMessage
        {
            get { return alertHigherLowMessage; }
            set { alertHigherLowMessage = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "34. Sound file name", Order = 34, GroupName = "7. Alerts")]
        public string AlertHigherLowSoundFileName
        {
            get { return alertHigherLowSoundFileName; }
            set { alertHigherLowSoundFileName = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "35. Rearm seconds", Order = 35, GroupName = "7. Alerts")]
        public int AlertHigherLowRearmSeconds
        {
            get { return alertHigherLowRearmSeconds; }
            set { alertHigherLowRearmSeconds = value; }
        }
        
		[XmlIgnore]
        [Display(Name = "36. Back color", Order = 36, GroupName = "7. Alerts")]
        public Brush AlertHigherLowBackColor
        {
            get { return alertHigherLowBackColor; }
            set { alertHigherLowBackColor = value; }
        }
        [Browsable(false)]
        public string AlertHigherLowBackColorSerialize
        {
            get { return Serialize.BrushToString(alertHigherLowBackColor); }
            set { alertHigherLowBackColor = Serialize.StringToBrush(value); }
        }
        
		[XmlIgnore]
        [Display(Name = "37. Fore color", Order = 37, GroupName = "7. Alerts")]
        public Brush AlertHigherLowForeColor
        {
            get { return alertHigherLowForeColor; }
            set { alertHigherLowForeColor = value; }
        }
        [Browsable(false)]
        public string AlertHigherLowForeColorSerialize
        {
            get { return Serialize.BrushToString(alertHigherLowForeColor); }
            set { alertHigherLowForeColor = Serialize.StringToBrush(value); }
        }
        //-----------------------------------------------------------------------------------------
        #endregion

        #region Lower high
        //-----------------------------------------------------------------------------------------
        [NinjaScriptProperty]
        [Display(Name = "41. Alert lower high", Order = 41, GroupName = "7. Alerts")]
        public bool AlertLowerHigh
        {
            get { return alertLowerHigh; }
            set { alertLowerHigh = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "42. Priority", Order = 42, GroupName = "7. Alerts")]
        public Priority AlertLowerHighPriority
        {
            get { return alertLowerHighPriority; }
            set { alertLowerHighPriority = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "43. Message", Order = 43, GroupName = "7. Alerts")]
        public string AlertLowerHighMessage
        {
            get { return alertLowerHighMessage; }
            set { alertLowerHighMessage = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "44. Sound file name", Order = 44, GroupName = "7. Alerts")]
        public string AlertLowerHighSoundFileName
        {
            get { return alertLowerHighSoundFileName; }
            set { alertLowerHighSoundFileName = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "45. Rearm seconds", Order = 45, GroupName = "7. Alerts")]
        public int AlertLowerHighRearmSeconds
        {
            get { return alertLowerHighRearmSeconds; }
            set { alertLowerHighRearmSeconds = value; }
        }
        
		[XmlIgnore]
        [Display(Name = "46. Back color", Order = 46, GroupName = "7. Alerts")]
        public Brush AlertLowerHighBackColor
        {
            get { return alertLowerHighBackColor; }
            set { alertLowerHighBackColor = value; }
        }
        [Browsable(false)]
        public string AlertLowerHighBackColorSerialize
        {
            get { return Serialize.BrushToString(alertLowerHighBackColor); }
            set { alertLowerHighBackColor = Serialize.StringToBrush(value); }
        }
        
		[XmlIgnore]
        [Display(Name = "47. Fore color", Order = 47, GroupName = "7. Alerts")]
        public Brush AlertLowerHighForeColor
        {
            get { return alertLowerHighForeColor; }
            set { alertLowerHighForeColor = value; }
        }
        [Browsable(false)]
        public string AlertLowerHighForeColorSerialize
        {
            get { return Serialize.BrushToString(alertLowerHighForeColor); }
            set { alertLowerHighForeColor = Serialize.StringToBrush(value); }
        }
        //-----------------------------------------------------------------------------------------
        #endregion

        #region Swing change
        //-----------------------------------------------------------------------------------------
        [NinjaScriptProperty]
        [Display(Name = "51. Alert swing change", Order = 51, GroupName = "7. Alerts")]
        public bool AlertSwingChange
        {
            get { return alertSwingChange; }
            set { alertSwingChange = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "52. Priority", Order = 52, GroupName = "7. Alerts")]
        public Priority AlertSwingChangePriority
        {
            get { return alertSwingChangePriority; }
            set { alertSwingChangePriority = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "53. Message", Order = 53, GroupName = "7. Alerts")]
        public string AlertSwingChangeMessage
        {
            get { return alertSwingChangeMessage; }
            set { alertSwingChangeMessage = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "54. Sound file name", Order = 54, GroupName = "7. Alerts")]
        public string AlertSwingChangeSoundFileName
        {
            get { return alertSwingChangeSoundFileName; }
            set { alertSwingChangeSoundFileName = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "55. Rearm seconds", Order = 55, GroupName = "7. Alerts")]
        public int AlertSwingChangeRearmSeconds
        {
            get { return alertSwingChangeRearmSeconds; }
            set { alertSwingChangeRearmSeconds = value; }
        }
        
		[XmlIgnore]
        [Display(Name = "56. Back color", Order = 56, GroupName = "7. Alerts")]
        public Brush AlertSwingChangeBackColor
        {
            get { return alertSwingChangeBackColor; }
            set { alertSwingChangeBackColor = value; }
        }
        [Browsable(false)]
        public string AlertSwingChangeBackColorSerialize
        {
            get { return Serialize.BrushToString(alertSwingChangeBackColor); }
            set { alertSwingChangeBackColor = Serialize.StringToBrush(value); }
        }
        
		[XmlIgnore]
        [Display(Name = "57. Fore color", Order = 57, GroupName = "7. Alerts")]
        public Brush AlertSwingChangeForeColor
        {
            get { return alertSwingChangeForeColor; }
            set { alertSwingChangeForeColor = value; }
        }
        [Browsable(false)]
        public string AlertSwingChangeForeColorSerialize
        {
            get { return Serialize.BrushToString(alertSwingChangeForeColor); }
            set { alertSwingChangeForeColor = Serialize.StringToBrush(value); }
        }
        //-----------------------------------------------------------------------------------------
        #endregion

        #region Divergence regular high
        //-----------------------------------------------------------------------------------------
        [NinjaScriptProperty]
        [Display(Name = "61. Alert divergence regular high", Order = 61, GroupName = "7. Alerts")]
        public bool AlertDivergenceRegularHigh
        {
            get { return alertDivergenceRegularHigh; }
            set { alertDivergenceRegularHigh = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "62. Priority", Order = 62, GroupName = "7. Alerts")]
        public Priority AlertDivergenceRegularHighPriority
        {
            get { return alertDivergenceRegularHighPriority; }
            set { alertDivergenceRegularHighPriority = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "63. Message", Order = 63, GroupName = "7. Alerts")]
        public string AlertDivergenceRegularHighMessage
        {
            get { return alertDivergenceRegularHighMessage; }
            set { alertDivergenceRegularHighMessage = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "64. Sound file name", Order = 64, GroupName = "7. Alerts")]
        public string AlertDivergenceRegularHighSoundFileName
        {
            get { return alertDivergenceRegularHighSoundFileName; }
            set { alertDivergenceRegularHighSoundFileName = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "65. Rearm seconds", Order = 65, GroupName = "7. Alerts")]
        public int AlertDivergenceRegularHighRearmSeconds
        {
            get { return alertDivergenceRegularHighRearmSeconds; }
            set { alertDivergenceRegularHighRearmSeconds = value; }
        }
        
		[XmlIgnore]
        [Display(Name = "66. Back color", Order = 66, GroupName = "7. Alerts")]
        public Brush AlertDivergenceRegularHighBackColor
        {
            get { return alertDivergenceRegularHighBackColor; }
            set { alertDivergenceRegularHighBackColor = value; }
        }
        [Browsable(false)]
        public string AlertDivergenceRegularHighBackColorSerialize
        {
            get { return Serialize.BrushToString(alertDivergenceRegularHighBackColor); }
            set { alertDivergenceRegularHighBackColor = Serialize.StringToBrush(value); }
        }
        
		[XmlIgnore]
        [Display(Name = "67. Fore color", Order = 67, GroupName = "7. Alerts")]
        public Brush AlertDivergenceRegularHighForeColor
        {
            get { return alertDivergenceRegularHighForeColor; }
            set { alertDivergenceRegularHighForeColor = value; }
        }
        [Browsable(false)]
        public string AlertDivergenceRegularHighForeColorSerialize
        {
            get { return Serialize.BrushToString(alertDivergenceRegularHighForeColor); }
            set { alertDivergenceRegularHighForeColor = Serialize.StringToBrush(value); }
        }
        //-----------------------------------------------------------------------------------------
        #endregion

        #region Divergence hidden high
        //-----------------------------------------------------------------------------------------
        [NinjaScriptProperty]
        [Display(Name = "71. Alert divergence hidden high", Order = 71, GroupName = "7. Alerts")]
        public bool AlertDivergenceHiddenHigh
        {
            get { return alertDivergenceHiddenHigh; }
            set { alertDivergenceHiddenHigh = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "72. Priority", Order = 72, GroupName = "7. Alerts")]
        public Priority AlertDivergenceHiddenHighPriority
        {
            get { return alertDivergenceHiddenHighPriority; }
            set { alertDivergenceHiddenHighPriority = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "73. Message", Order = 73, GroupName = "7. Alerts")]
        public string AlertDivergenceHiddenHighMessage
        {
            get { return alertDivergenceHiddenHighMessage; }
            set { alertDivergenceHiddenHighMessage = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "74. Sound file name", Order = 74, GroupName = "7. Alerts")]
        public string AlertDivergenceHiddenHighSoundFileName
        {
            get { return alertDivergenceHiddenHighSoundFileName; }
            set { alertDivergenceHiddenHighSoundFileName = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "75. Rearm seconds", Order = 75, GroupName = "7. Alerts")]
        public int AlertDivergenceHiddenHighRearmSeconds
        {
            get { return alertDivergenceHiddenHighRearmSeconds; }
            set { alertDivergenceHiddenHighRearmSeconds = value; }
        }
        
		[XmlIgnore]
        [Display(Name = "76. Back color", Order = 76, GroupName = "7. Alerts")]
        public Brush AlertDivergenceHiddenHighBackColor
        {
            get { return alertDivergenceHiddenHighBackColor; }
            set { alertDivergenceHiddenHighBackColor = value; }
        }
        [Browsable(false)]
        public string AlertDivergenceHiddenHighBackColorSerialize
        {
            get { return Serialize.BrushToString(alertDivergenceHiddenHighBackColor); }
            set { alertDivergenceHiddenHighBackColor = Serialize.StringToBrush(value); }
        }
        
		[XmlIgnore]
        [Display(Name = "77. Fore color", Order = 77, GroupName = "7. Alerts")]
        public Brush AlertDivergenceHiddenHighForeColor
        {
            get { return alertDivergenceHiddenHighForeColor; }
            set { alertDivergenceHiddenHighForeColor = value; }
        }
        [Browsable(false)]
        public string AlertDivergenceHiddenHighForeColorSerialize
        {
            get { return Serialize.BrushToString(alertDivergenceHiddenHighForeColor); }
            set { alertDivergenceHiddenHighForeColor = Serialize.StringToBrush(value); }
        }
        //-----------------------------------------------------------------------------------------
        #endregion

        #region Divergence regular low
        //-----------------------------------------------------------------------------------------
        [NinjaScriptProperty]
        [Display(Name = "81. Alert divergence regular low", Order = 81, GroupName = "7. Alerts")]
        public bool AlertDivergenceRegularLow
        {
            get { return alertDivergenceRegularLow; }
            set { alertDivergenceRegularLow = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "82. Priority", Order = 82, GroupName = "7. Alerts")]
        public Priority AlertDivergenceRegularLowPriority
        {
            get { return alertDivergenceRegularLowPriority; }
            set { alertDivergenceRegularLowPriority = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "83. Message", Order = 83, GroupName = "7. Alerts")]
        public string AlertDivergenceRegularLowMessage
        {
            get { return alertDivergenceRegularLowMessage; }
            set { alertDivergenceRegularLowMessage = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "84. Sound file name", Order = 84, GroupName = "7. Alerts")]
        public string AlertDivergenceRegularLowSoundFileName
        {
            get { return alertDivergenceRegularLowSoundFileName; }
            set { alertDivergenceRegularLowSoundFileName = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "85. Rearm seconds", Order = 85, GroupName = "7. Alerts")]
        public int AlertDivergenceRegularLowRearmSeconds
        {
            get { return alertDivergenceRegularLowRearmSeconds; }
            set { alertDivergenceRegularLowRearmSeconds = value; }
        }
        
		[XmlIgnore]
        [Display(Name = "86. Back color", Order = 86, GroupName = "7. Alerts")]
        public Brush AlertDivergenceRegularLowBackColor
        {
            get { return alertDivergenceRegularLowBackColor; }
            set { alertDivergenceRegularLowBackColor = value; }
        }
        [Browsable(false)]
        public string AlertDivergenceRegularLowBackColorSerialize
        {
            get { return Serialize.BrushToString(alertDivergenceRegularLowBackColor); }
            set { alertDivergenceRegularLowBackColor = Serialize.StringToBrush(value); }
        }
        
		[XmlIgnore]
        [Display(Name = "87. Fore color", Order = 87, GroupName = "7. Alerts")]
        public Brush AlertDivergenceRegularLowForeColor
        {
            get { return alertDivergenceRegularLowForeColor; }
            set { alertDivergenceRegularLowForeColor = value; }
        }
        [Browsable(false)]
        public string AlertDivergenceRegularLowForeColorSerialize
        {
            get { return Serialize.BrushToString(alertDivergenceRegularLowForeColor); }
            set { alertDivergenceRegularLowForeColor = Serialize.StringToBrush(value); }
        }
        //-----------------------------------------------------------------------------------------
        #endregion

        #region Divergence hidden low
        //-----------------------------------------------------------------------------------------
        [NinjaScriptProperty]
        [Display(Name = "91. Alert divergence hidden low", Order = 91, GroupName = "7. Alerts")]
        public bool AlertDivergenceHiddenLow
        {
            get { return alertDivergenceHiddenLow; }
            set { alertDivergenceHiddenLow = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "92. Priority", Order = 92, GroupName = "7. Alerts")]
        public Priority AlertDivergenceHiddenLowPriority
        {
            get { return alertDivergenceHiddenLowPriority; }
            set { alertDivergenceHiddenLowPriority = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "93. Message", Order = 93, GroupName = "7. Alerts")]
        public string AlertDivergenceHiddenLowMessage
        {
            get { return alertDivergenceHiddenLowMessage; }
            set { alertDivergenceHiddenLowMessage = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "94. Sound file name", Order = 94, GroupName = "7. Alerts")]
        public string AlertDivergenceHiddenLowSoundFileName
        {
            get { return alertDivergenceHiddenLowSoundFileName; }
            set { alertDivergenceHiddenLowSoundFileName = value; }
        }
        [NinjaScriptProperty]
        [Display(Name = "95. Rearm seconds", Order = 95, GroupName = "7. Alerts")]
        public int AlertDivergenceHiddenLowRearmSeconds
        {
            get { return alertDivergenceHiddenLowRearmSeconds; }
            set { alertDivergenceHiddenLowRearmSeconds = value; }
        }
        
		[XmlIgnore]
        [Display(Name = "96. Back color", Order = 96, GroupName = "7. Alerts")]
        public Brush AlertDivergenceHiddenLowBackColor
        {
            get { return alertDivergenceHiddenLowBackColor; }
            set { alertDivergenceHiddenLowBackColor = value; }
        }
        [Browsable(false)]
        public string AlertDivergenceHiddenLowBackColorSerialize
        {
            get { return Serialize.BrushToString(alertDivergenceHiddenLowBackColor); }
            set { alertDivergenceHiddenLowBackColor = Serialize.StringToBrush(value); }
        }
        
		[XmlIgnore]
        [Display(Name = "97. Fore color", Order = 97, GroupName = "7. Alerts")]
        public Brush AlertDivergenceHiddenLowForeColor
        {
            get { return alertDivergenceHiddenLowForeColor; }
            set { alertDivergenceHiddenLowForeColor = value; }
        }
        [Browsable(false)]
        public string AlertDivergenceHiddenLowForeColorSerialize
        {
            get { return Serialize.BrushToString(alertDivergenceHiddenLowForeColor); }
            set { alertDivergenceHiddenLowForeColor = Serialize.StringToBrush(value); }
        }
        //-----------------------------------------------------------------------------------------
        #endregion
        //=========================================================================================
        #endregion
		
		#endregion

        #region Statistic
        //#########################################################################################
        #region UpStatistic()
        //=========================================================================================
        private void UpStatistic()
        {
            int upCount = swingHighs.Count - 1;
            if (upCount == 0)
                return;

            overallUpLength = overallUpLength + swingHighs[upCount].length;
            overallAvgUpLength = Math.Round(overallUpLength / upCount, 0, 
                MidpointRounding.AwayFromZero);

            overallUpDuration = overallUpDuration + swingHighs[upCount].duration;
            overallAvgUpDuration = Math.Round(overallUpDuration / upCount, 0, 
                MidpointRounding.AwayFromZero);

            if (upCount >= statisticLength)
            {
                upLength = 0;
                upDuration = 0;
                for (int i = 0; i < statisticLength; i++)
                {
                    upLength = upLength + swingHighs[upCount - i].length;
                    upDuration = upDuration + swingHighs[upCount - i].duration;
                }
                avgUpLength = Math.Round(upLength / statisticLength, 0, 
                    MidpointRounding.AwayFromZero);
                avgUpDuration = Math.Round(upDuration / statisticLength, 0, 
                    MidpointRounding.AwayFromZero);
            }

//            lengthList[1, 0].Value = upCount;
//            lengthList[2, 0].Value = overallAvgUpLength;
//            lengthList[3, 0].Value = avgUpLength;
//            lengthList[4, 0].Value = overallAvgUpDuration;
//            lengthList[5, 0].Value = avgUpDuration;

            if (swingHigh.LastRelation == 1)
            {
                hhCount++;

                if (swingHigh.CurRelation == 1) hhCountHH++;
                if (swingHigh.CurRelation == -1) hhCountLH++;
                if (swingLow.LastRelation == 1) hhCountHL++;
                if (swingLow.LastRelation == -1) hhCountLL++;

                hhCountLHPercent = Math.Round(100.0 / hhCount * hhCountLH, 1, 
                    MidpointRounding.AwayFromZero);
                hhCountHHPercent = Math.Round(100.0 / hhCount * hhCountHH, 1, 
                    MidpointRounding.AwayFromZero);
                hhCountHLPercent = Math.Round(100.0 / hhCount * hhCountHL, 1, 
                    MidpointRounding.AwayFromZero);
                hhCountLLPercent = Math.Round(100.0 / hhCount * hhCountLL, 1, 
                    MidpointRounding.AwayFromZero);
            }

            if (swingHigh.LastRelation == -1)
            {
                lhCount++;

                if (swingHigh.CurRelation == 1) lhCountHH++;
                if (swingHigh.CurRelation == -1) lhCountLH++;
                if (swingLow.LastRelation == 1) lhCountHL++;
                if (swingLow.LastRelation == -1) lhCountLL++;

                lhCountLHPercent = Math.Round(100.0 / lhCount * lhCountLH, 1,
                    MidpointRounding.AwayFromZero);
                lhCountHHPercent = Math.Round(100.0 / lhCount * lhCountHH, 1,
                    MidpointRounding.AwayFromZero);
                lhCountHLPercent = Math.Round(100.0 / lhCount * lhCountHL, 1, 
                    MidpointRounding.AwayFromZero);
                lhCountLLPercent = Math.Round(100.0 / lhCount * lhCountLL, 1, 
                    MidpointRounding.AwayFromZero);
            }

            if (swingHigh.LastRelation == 0)
            {
                dtCount++;

                if (swingHigh.CurRelation == 1) dtCountHH++;
                if (swingHigh.CurRelation == -1) dtCountLH++;
                if (swingLow.LastRelation == 1) dtCountHL++;
                if (swingLow.LastRelation == -1) dtCountLL++;

                dtCountLHPercent = Math.Round(100.0 / dtCount * dtCountLH, 1, 
                    MidpointRounding.AwayFromZero);
                dtCountHHPercent = Math.Round(100.0 / dtCount * dtCountHH, 1, 
                    MidpointRounding.AwayFromZero);
                dtCountHLPercent = Math.Round(100.0 / dtCount * dtCountHL, 1, 
                    MidpointRounding.AwayFromZero);
                dtCountLLPercent = Math.Round(100.0 / dtCount * dtCountLL, 1, 
                    MidpointRounding.AwayFromZero);
            }
//            relationList[1, 0].Value = hhCount;
//            relationList[2, 0].Value = hhCountHHPercent + "%";
//            relationList[3, 0].Value = hhCountLHPercent + "%";
//            relationList[4, 0].Value = hhCountHLPercent + "%";
//            relationList[5, 0].Value = hhCountLLPercent + "%";

//            relationList[1, 1].Value = lhCount;
//            relationList[2, 1].Value = lhCountHHPercent + "%";
//            relationList[3, 1].Value = lhCountLHPercent + "%";
//            relationList[4, 1].Value = lhCountHLPercent + "%";
//            relationList[5, 1].Value = lhCountLLPercent + "%";

//            relationList[1, 2].Value = dtCount;
//            relationList[2, 2].Value = dtCountHHPercent + "%";
//            relationList[3, 2].Value = dtCountLHPercent + "%";
//            relationList[4, 2].Value = dtCountHLPercent + "%";
//            relationList[5, 2].Value = dtCountLLPercent + "%";
        }
        //=========================================================================================
        #endregion

        #region DownStatistic()
        //=========================================================================================
        private void DownStatistic()
        {
            int dnCount = swingLows.Count - 1;
            if (dnCount == 0)
                return;

            overallDnLength = overallDnLength + swingLows[dnCount].length;
            overallAvgDnLength = Math.Round(overallDnLength / dnCount, 0, 
                MidpointRounding.AwayFromZero);

            overallDnDuration = overallDnDuration + swingLows[dnCount].duration;
            overallAvgDnDuration = Math.Round(overallDnDuration / dnCount, 0, 
                MidpointRounding.AwayFromZero);

            if (dnCount >= statisticLength)
            {
                dnLength = 0;
                dnDuration = 0;
                for (int i = 0; i < statisticLength; i++)
                {
                    dnLength = dnLength + swingLows[dnCount - i].length;
                    dnDuration = dnDuration + swingLows[dnCount - i].duration;
                }
                avgDnLength = Math.Round(dnLength / statisticLength, 0, 
                    MidpointRounding.AwayFromZero);
                avgDnDuration = Math.Round(dnDuration / statisticLength, 0, 
                    MidpointRounding.AwayFromZero);
            }

//            lengthList[1, 1].Value = dnCount;
//            lengthList[2, 1].Value = overallAvgDnLength;
//            lengthList[3, 1].Value = avgDnLength;
//            lengthList[4, 1].Value = overallAvgDnDuration;
//            lengthList[5, 1].Value = avgDnDuration;

            if (swingLow.LastRelation == -1)
            {
                llCount++;

                if (swingHigh.LastRelation == 1) llCountHH++;
                if (swingHigh.LastRelation == -1) llCountLH++;
                if (swingLow.CurRelation == 1) llCountHL++;
                if (swingLow.CurRelation == -1) llCountLL++;

                llCountLHPercent = Math.Round(100.0 / llCount * llCountLH, 1, 
                    MidpointRounding.AwayFromZero);
                llCountHHPercent = Math.Round(100.0 / llCount * llCountHH, 1, 
                    MidpointRounding.AwayFromZero);
                llCountHLPercent = Math.Round(100.0 / llCount * llCountHL, 1, 
                    MidpointRounding.AwayFromZero);
                llCountLLPercent = Math.Round(100.0 / llCount * llCountLL, 1, 
                    MidpointRounding.AwayFromZero);
            }

            if (swingLow.LastRelation == 1)
            {
                hlCount++;

                if (swingHigh.LastRelation == 1) hlCountHH++;
                if (swingHigh.LastRelation == -1) hlCountLH++;
                if (swingLow.CurRelation == 1) hlCountHL++;
                if (swingLow.CurRelation == -1) hlCountLL++;

                hlCountLHPercent = Math.Round(100.0 / hlCount * hlCountLH, 1, 
                    MidpointRounding.AwayFromZero);
                hlCountHHPercent = Math.Round(100.0 / hlCount * hlCountHH, 1, 
                    MidpointRounding.AwayFromZero);
                hlCountHLPercent = Math.Round(100.0 / hlCount * hlCountHL, 1, 
                    MidpointRounding.AwayFromZero);
                hlCountLLPercent = Math.Round(100.0 / hlCount * hlCountLL, 1, 
                    MidpointRounding.AwayFromZero);
            }

            if (swingLow.LastRelation == 0)
            {
                dbCount++;

                if (swingHigh.LastRelation == 1) dbCountHH++;
                if (swingHigh.LastRelation == -1) dbCountLH++;
                if (swingLow.CurRelation == 1) dbCountHL++;
                if (swingLow.CurRelation == -1) dbCountLL++;

                dbCountLHPercent = Math.Round(100.0 / dbCount * dbCountLH, 1, 
                    MidpointRounding.AwayFromZero);
                dbCountHHPercent = Math.Round(100.0 / dbCount * dbCountHH, 1, 
                    MidpointRounding.AwayFromZero);
                dbCountHLPercent = Math.Round(100.0 / dbCount * dbCountHL, 1, 
                    MidpointRounding.AwayFromZero);
                dbCountLLPercent = Math.Round(100.0 / dbCount * dbCountLL, 1, 
                    MidpointRounding.AwayFromZero);
            }
//            relationList[1, 3].Value = llCount;
//            relationList[2, 3].Value = llCountHHPercent + "%";
//            relationList[3, 3].Value = llCountLHPercent + "%";
//            relationList[4, 3].Value = llCountHLPercent + "%";
//            relationList[5, 3].Value = llCountLLPercent + "%";

//            relationList[1, 4].Value = hlCount;
//            relationList[2, 4].Value = hlCountHHPercent + "%";
//            relationList[3, 4].Value = hlCountLHPercent + "%";
//            relationList[4, 4].Value = hlCountHLPercent + "%";
//            relationList[5, 4].Value = hlCountLLPercent + "%";

//            relationList[1, 5].Value = dbCount;
//            relationList[2, 5].Value = dbCountHHPercent + "%";
//            relationList[3, 5].Value = dbCountLHPercent + "%";
//            relationList[4, 5].Value = dbCountHLPercent + "%";
//            relationList[5, 5].Value = dbCountLLPercent + "%";
        }
        //=========================================================================================
        #endregion
        //#########################################################################################
        #endregion

        #region CalculateLongRisk
        //#########################################################################################
        /// <summary>
        /// Calculate risk for a long trade.
        /// </summary>
        private void CalculateLongRisk(double entryPrice, double stopLossPrice, 
            double takeProfitPrice)
        {
            double loss = Math.Round(((entryPrice - stopLossPrice) * 
                Instrument.MasterInstrument.PointValue), 2, MidpointRounding.AwayFromZero);
            int quantity = (int)Math.Truncate(accountSize / 100.0 * accountRisk / loss);
            double win = Math.Round(((takeProfitPrice - entryPrice) * 
                Instrument.MasterInstrument.PointValue), 2, MidpointRounding.AwayFromZero);
            double riskReward = Math.Round(win / loss, 2, MidpointRounding.AwayFromZero);

//            if (riskManagementPosition == RiskManagementStyle.ToolStrip)
//            {
//                this.toolStripNumericUpDownEntry.Value = (decimal)entryPrice;
//                this.toolStripNumericUpDownStop.Value = (decimal)stopLossPrice;
//                this.toolStripNumericUpDownTarget.Value = (decimal)takeProfitPrice;

//                this.toolStripLabelQty.Text = "Qty: " + quantity;
//                this.toolStripLabelQty.ForeColor = Color.Green;

//                this.toolStripLabelRiskReward.Text = "R/R: " + riskReward;
//                this.toolStripLabelLoss.Text = "Loss: " + loss;
//                this.toolStripLabelProfit.Text = "Win: " + win;
//                if (riskReward < 1.0)
//                    this.toolStripLabelRiskReward.ForeColor = Color.Red;
//                else if (riskReward < 1.6)
//                    this.toolStripLabelRiskReward.ForeColor = Color.Black;
//                else
//                    this.toolStripLabelRiskReward.ForeColor = Color.Green;
//            }
//            else
//            {
//                this.numericUpDownEntry.Value = (decimal)entryPrice;
//                this.numericUpDownStopLoss.Value = (decimal)stopLossPrice;
//                this.numericUpDownTakeProfit.Value = (decimal)takeProfitPrice;
//                this.labelQuantity.Text = "Qty: " + quantity;
//                this.labelQuantity.ForeColor = Color.Green;

//                this.labelRiskReward.Text = "R/R: " + riskReward;
//                this.labelLossValue.Text = "-" + String.Format("{0:F" + 2 + "}", loss);
//                this.labelWinValue.Text = String.Format("{0:F" + 2 + "}", win);
//                if (riskReward < 1.0)
//                    this.labelRiskReward.ForeColor = Color.Red;
//                else if (riskReward < 1.6)
//                    this.labelRiskReward.ForeColor = Color.Black;
//                else
//                    this.labelRiskReward.ForeColor = Color.Green;
//            }
        }
        //#########################################################################################
        #endregion

        #region CalculateShortRisk
        //#########################################################################################
        /// <summary>
        /// Calculate risk for a short trade.
        /// </summary>
        private void CalculateShortRisk(double entryPrice, double stopLossPrice,
            double takeProfitPrice)
        {
            double loss = Math.Round(((stopLossPrice - entryPrice) *
                Instrument.MasterInstrument.PointValue), 2, MidpointRounding.AwayFromZero);
            int quantity = (int)Math.Truncate(accountSize / 100.0 * accountRisk / loss);
            double win = Math.Round(((entryPrice - takeProfitPrice) *
                Instrument.MasterInstrument.PointValue), 2, MidpointRounding.AwayFromZero);
            double riskReward = Math.Round(win / loss, 2, MidpointRounding.AwayFromZero);

//            if (riskManagementPosition == RiskManagementStyle.ToolStrip)
//            {
//                this.toolStripNumericUpDownEntry.Value = (decimal)entryPrice;
//                this.toolStripNumericUpDownStop.Value = (decimal)stopLossPrice;
//                this.toolStripNumericUpDownTarget.Value = (decimal)takeProfitPrice;

//                this.toolStripLabelQty.Text = "Qty: " + quantity;
//                this.toolStripLabelQty.ForeColor = Color.Red;

//                this.toolStripLabelRiskReward.Text = "R/R: " + riskReward;
//                this.toolStripLabelLoss.Text = "Loss: " + loss;
//                this.toolStripLabelProfit.Text = "Win: " + win;
//                if (riskReward < 1.0)
//                    this.toolStripLabelRiskReward.ForeColor = Color.Red;
//                else if (riskReward < 1.6)
//                    this.toolStripLabelRiskReward.ForeColor = Color.Black;
//                else
//                    this.toolStripLabelRiskReward.ForeColor = Color.Green;
//            }
//            else
//            {
//                this.numericUpDownEntry.Value = (decimal)entryPrice;
//                this.numericUpDownStopLoss.Value = (decimal)stopLossPrice;
//                this.numericUpDownTakeProfit.Value = (decimal)takeProfitPrice;

//                this.labelQuantity.Text = "Qty: " + quantity;
//                this.labelQuantity.ForeColor = Color.Red;

//                this.labelRiskReward.Text = "R/R: " + riskReward;
//                this.labelLossValue.Text = "-" + String.Format("{0:F" + 2 + "}", loss);
//                this.labelWinValue.Text = String.Format("{0:F" + 2 + "}", win);
//                if (riskReward < 1.0)
//                    this.labelRiskReward.ForeColor = Color.Red;
//                else if (riskReward < 1.6)
//                    this.labelRiskReward.ForeColor = Color.Black;
//                else
//                    this.labelRiskReward.ForeColor = Color.Green;
//            }
        }
        //#########################################################################################
        #endregion
		
        #region DisplayName
        //#########################################################################################
        public override string DisplayName
        {
            get { return (displayName != null ? displayName : Name); }
		}        
        //#########################################################################################
		#endregion
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private PriceActionSwing.PriceActionSwingPro[] cachePriceActionSwingPro;
		public PriceActionSwing.PriceActionSwingPro PriceActionSwingPro(SwingStyle swingType, double swingSize, int dtbStrength, bool useCloseValues, AbcPatternMode abcPattern, DivergenceMode divergenceIndicatorMode, DivergenceDirection divergenceDirectionMode, int param1, int param2, int param3, bool showNakedSwings, bool addSwingExtension, bool addSwingRetracementFast, bool addSwingRetracementSlow, SwingLengthStyle swingLengthType, SwingDurationStyle swingDurationType, bool showSwingPrice, bool showSwingLabel, bool showSwingPercent, SwingTimeStyle swingTimeType, SwingVolumeStyle swingVolumeType, VisualizationStyle visualizationType, NinjaTrader.Gui.Tools.SimpleFont textFont, int textOffsetLength, int textOffsetVolume, int textOffsetPrice, int textOffsetLabel, int textOffsetTime, int textOffsetPercent, DashStyleHelper zigZagStyle, int zigZagWidth, bool showSwingSwitch, int swingSwitchOffsetInTicks, DashStyleHelper abcLineStyle, DashStyleHelper abcLineStyleRatio, int abcLineWidth, int abcLineWidthRatio, NinjaTrader.Gui.Tools.SimpleFont abcTextFont, int abcTextOffsetLabel, double abcMaxRetracement, double abcMinRetracement, DashStyleHelper entryLineStyle, int entryLineWidth, double retracementEntryValue, bool showEntryArrows, bool showHistoricalEntryLine, int yTickOffset, bool showDivergenceRegular, bool showDivergenceHidden, DashStyleHelper divDnLineStyle, DashStyleHelper divUpLineStyle, int divDnLineWidth, int divUpLineWidth, bool showHistoricalNakedSwings, DashStyleHelper nakedSwingDashStyle, int nakedSwingLineWidth, bool ignoreInsideBars, bool useBreakouts, bool alertAbc, bool alertAbcEntry, Priority alertAbcPriority, Priority alertAbcEntryPriority, string alertAbcLongSoundFileName, string alertAbcLongEntrySoundFileName, string alertAbcShortSoundFileName, string alertAbcShortEntrySoundFileName, bool alertDoubleBottom, Priority alertDoubleBottomPriority, string alertDoubleBottomMessage, string alertDoubleBottomSoundFileName, int alertDoubleBottomRearmSeconds, bool alertDoubleTop, Priority alertDoubleTopPriority, string alertDoubleTopMessage, string alertDoubleTopSoundFileName, int alertDoubleTopRearmSeconds, bool alertHigherLow, Priority alertHigherLowPriority, string alertHigherLowMessage, string alertHigherLowSoundFileName, int alertHigherLowRearmSeconds, bool alertLowerHigh, Priority alertLowerHighPriority, string alertLowerHighMessage, string alertLowerHighSoundFileName, int alertLowerHighRearmSeconds, bool alertSwingChange, Priority alertSwingChangePriority, string alertSwingChangeMessage, string alertSwingChangeSoundFileName, int alertSwingChangeRearmSeconds, bool alertDivergenceRegularHigh, Priority alertDivergenceRegularHighPriority, string alertDivergenceRegularHighMessage, string alertDivergenceRegularHighSoundFileName, int alertDivergenceRegularHighRearmSeconds, bool alertDivergenceHiddenHigh, Priority alertDivergenceHiddenHighPriority, string alertDivergenceHiddenHighMessage, string alertDivergenceHiddenHighSoundFileName, int alertDivergenceHiddenHighRearmSeconds, bool alertDivergenceRegularLow, Priority alertDivergenceRegularLowPriority, string alertDivergenceRegularLowMessage, string alertDivergenceRegularLowSoundFileName, int alertDivergenceRegularLowRearmSeconds, bool alertDivergenceHiddenLow, Priority alertDivergenceHiddenLowPriority, string alertDivergenceHiddenLowMessage, string alertDivergenceHiddenLowSoundFileName, int alertDivergenceHiddenLowRearmSeconds)
		{
			return PriceActionSwingPro(Input, swingType, swingSize, dtbStrength, useCloseValues, abcPattern, divergenceIndicatorMode, divergenceDirectionMode, param1, param2, param3, showNakedSwings, addSwingExtension, addSwingRetracementFast, addSwingRetracementSlow, swingLengthType, swingDurationType, showSwingPrice, showSwingLabel, showSwingPercent, swingTimeType, swingVolumeType, visualizationType, textFont, textOffsetLength, textOffsetVolume, textOffsetPrice, textOffsetLabel, textOffsetTime, textOffsetPercent, zigZagStyle, zigZagWidth, showSwingSwitch, swingSwitchOffsetInTicks, abcLineStyle, abcLineStyleRatio, abcLineWidth, abcLineWidthRatio, abcTextFont, abcTextOffsetLabel, abcMaxRetracement, abcMinRetracement, entryLineStyle, entryLineWidth, retracementEntryValue, showEntryArrows, showHistoricalEntryLine, yTickOffset, showDivergenceRegular, showDivergenceHidden, divDnLineStyle, divUpLineStyle, divDnLineWidth, divUpLineWidth, showHistoricalNakedSwings, nakedSwingDashStyle, nakedSwingLineWidth, ignoreInsideBars, useBreakouts, alertAbc, alertAbcEntry, alertAbcPriority, alertAbcEntryPriority, alertAbcLongSoundFileName, alertAbcLongEntrySoundFileName, alertAbcShortSoundFileName, alertAbcShortEntrySoundFileName, alertDoubleBottom, alertDoubleBottomPriority, alertDoubleBottomMessage, alertDoubleBottomSoundFileName, alertDoubleBottomRearmSeconds, alertDoubleTop, alertDoubleTopPriority, alertDoubleTopMessage, alertDoubleTopSoundFileName, alertDoubleTopRearmSeconds, alertHigherLow, alertHigherLowPriority, alertHigherLowMessage, alertHigherLowSoundFileName, alertHigherLowRearmSeconds, alertLowerHigh, alertLowerHighPriority, alertLowerHighMessage, alertLowerHighSoundFileName, alertLowerHighRearmSeconds, alertSwingChange, alertSwingChangePriority, alertSwingChangeMessage, alertSwingChangeSoundFileName, alertSwingChangeRearmSeconds, alertDivergenceRegularHigh, alertDivergenceRegularHighPriority, alertDivergenceRegularHighMessage, alertDivergenceRegularHighSoundFileName, alertDivergenceRegularHighRearmSeconds, alertDivergenceHiddenHigh, alertDivergenceHiddenHighPriority, alertDivergenceHiddenHighMessage, alertDivergenceHiddenHighSoundFileName, alertDivergenceHiddenHighRearmSeconds, alertDivergenceRegularLow, alertDivergenceRegularLowPriority, alertDivergenceRegularLowMessage, alertDivergenceRegularLowSoundFileName, alertDivergenceRegularLowRearmSeconds, alertDivergenceHiddenLow, alertDivergenceHiddenLowPriority, alertDivergenceHiddenLowMessage, alertDivergenceHiddenLowSoundFileName, alertDivergenceHiddenLowRearmSeconds);
		}

		public PriceActionSwing.PriceActionSwingPro PriceActionSwingPro(ISeries<double> input, SwingStyle swingType, double swingSize, int dtbStrength, bool useCloseValues, AbcPatternMode abcPattern, DivergenceMode divergenceIndicatorMode, DivergenceDirection divergenceDirectionMode, int param1, int param2, int param3, bool showNakedSwings, bool addSwingExtension, bool addSwingRetracementFast, bool addSwingRetracementSlow, SwingLengthStyle swingLengthType, SwingDurationStyle swingDurationType, bool showSwingPrice, bool showSwingLabel, bool showSwingPercent, SwingTimeStyle swingTimeType, SwingVolumeStyle swingVolumeType, VisualizationStyle visualizationType, NinjaTrader.Gui.Tools.SimpleFont textFont, int textOffsetLength, int textOffsetVolume, int textOffsetPrice, int textOffsetLabel, int textOffsetTime, int textOffsetPercent, DashStyleHelper zigZagStyle, int zigZagWidth, bool showSwingSwitch, int swingSwitchOffsetInTicks, DashStyleHelper abcLineStyle, DashStyleHelper abcLineStyleRatio, int abcLineWidth, int abcLineWidthRatio, NinjaTrader.Gui.Tools.SimpleFont abcTextFont, int abcTextOffsetLabel, double abcMaxRetracement, double abcMinRetracement, DashStyleHelper entryLineStyle, int entryLineWidth, double retracementEntryValue, bool showEntryArrows, bool showHistoricalEntryLine, int yTickOffset, bool showDivergenceRegular, bool showDivergenceHidden, DashStyleHelper divDnLineStyle, DashStyleHelper divUpLineStyle, int divDnLineWidth, int divUpLineWidth, bool showHistoricalNakedSwings, DashStyleHelper nakedSwingDashStyle, int nakedSwingLineWidth, bool ignoreInsideBars, bool useBreakouts, bool alertAbc, bool alertAbcEntry, Priority alertAbcPriority, Priority alertAbcEntryPriority, string alertAbcLongSoundFileName, string alertAbcLongEntrySoundFileName, string alertAbcShortSoundFileName, string alertAbcShortEntrySoundFileName, bool alertDoubleBottom, Priority alertDoubleBottomPriority, string alertDoubleBottomMessage, string alertDoubleBottomSoundFileName, int alertDoubleBottomRearmSeconds, bool alertDoubleTop, Priority alertDoubleTopPriority, string alertDoubleTopMessage, string alertDoubleTopSoundFileName, int alertDoubleTopRearmSeconds, bool alertHigherLow, Priority alertHigherLowPriority, string alertHigherLowMessage, string alertHigherLowSoundFileName, int alertHigherLowRearmSeconds, bool alertLowerHigh, Priority alertLowerHighPriority, string alertLowerHighMessage, string alertLowerHighSoundFileName, int alertLowerHighRearmSeconds, bool alertSwingChange, Priority alertSwingChangePriority, string alertSwingChangeMessage, string alertSwingChangeSoundFileName, int alertSwingChangeRearmSeconds, bool alertDivergenceRegularHigh, Priority alertDivergenceRegularHighPriority, string alertDivergenceRegularHighMessage, string alertDivergenceRegularHighSoundFileName, int alertDivergenceRegularHighRearmSeconds, bool alertDivergenceHiddenHigh, Priority alertDivergenceHiddenHighPriority, string alertDivergenceHiddenHighMessage, string alertDivergenceHiddenHighSoundFileName, int alertDivergenceHiddenHighRearmSeconds, bool alertDivergenceRegularLow, Priority alertDivergenceRegularLowPriority, string alertDivergenceRegularLowMessage, string alertDivergenceRegularLowSoundFileName, int alertDivergenceRegularLowRearmSeconds, bool alertDivergenceHiddenLow, Priority alertDivergenceHiddenLowPriority, string alertDivergenceHiddenLowMessage, string alertDivergenceHiddenLowSoundFileName, int alertDivergenceHiddenLowRearmSeconds)
		{
			if (cachePriceActionSwingPro != null)
				for (int idx = 0; idx < cachePriceActionSwingPro.Length; idx++)
					if (cachePriceActionSwingPro[idx] != null && cachePriceActionSwingPro[idx].SwingType == swingType && cachePriceActionSwingPro[idx].SwingSize == swingSize && cachePriceActionSwingPro[idx].DtbStrength == dtbStrength && cachePriceActionSwingPro[idx].UseCloseValues == useCloseValues && cachePriceActionSwingPro[idx].AbcPattern == abcPattern && cachePriceActionSwingPro[idx].DivergenceIndicatorMode == divergenceIndicatorMode && cachePriceActionSwingPro[idx].DivergenceDirectionMode == divergenceDirectionMode && cachePriceActionSwingPro[idx].Param1 == param1 && cachePriceActionSwingPro[idx].Param2 == param2 && cachePriceActionSwingPro[idx].Param3 == param3 && cachePriceActionSwingPro[idx].ShowNakedSwings == showNakedSwings && cachePriceActionSwingPro[idx].AddSwingExtension == addSwingExtension && cachePriceActionSwingPro[idx].AddSwingRetracementFast == addSwingRetracementFast && cachePriceActionSwingPro[idx].AddSwingRetracementSlow == addSwingRetracementSlow && cachePriceActionSwingPro[idx].SwingLengthType == swingLengthType && cachePriceActionSwingPro[idx].SwingDurationType == swingDurationType && cachePriceActionSwingPro[idx].ShowSwingPrice == showSwingPrice && cachePriceActionSwingPro[idx].ShowSwingLabel == showSwingLabel && cachePriceActionSwingPro[idx].ShowSwingPercent == showSwingPercent && cachePriceActionSwingPro[idx].SwingTimeType == swingTimeType && cachePriceActionSwingPro[idx].SwingVolumeType == swingVolumeType && cachePriceActionSwingPro[idx].VisualizationType == visualizationType && cachePriceActionSwingPro[idx].TextFont == textFont && cachePriceActionSwingPro[idx].TextOffsetLength == textOffsetLength && cachePriceActionSwingPro[idx].TextOffsetVolume == textOffsetVolume && cachePriceActionSwingPro[idx].TextOffsetPrice == textOffsetPrice && cachePriceActionSwingPro[idx].TextOffsetLabel == textOffsetLabel && cachePriceActionSwingPro[idx].TextOffsetTime == textOffsetTime && cachePriceActionSwingPro[idx].TextOffsetPercent == textOffsetPercent && cachePriceActionSwingPro[idx].ZigZagStyle == zigZagStyle && cachePriceActionSwingPro[idx].ZigZagWidth == zigZagWidth && cachePriceActionSwingPro[idx].ShowSwingSwitch == showSwingSwitch && cachePriceActionSwingPro[idx].SwingSwitchOffsetInTicks == swingSwitchOffsetInTicks && cachePriceActionSwingPro[idx].AbcLineStyle == abcLineStyle && cachePriceActionSwingPro[idx].AbcLineStyleRatio == abcLineStyleRatio && cachePriceActionSwingPro[idx].AbcLineWidth == abcLineWidth && cachePriceActionSwingPro[idx].AbcLineWidthRatio == abcLineWidthRatio && cachePriceActionSwingPro[idx].AbcTextFont == abcTextFont && cachePriceActionSwingPro[idx].AbcTextOffsetLabel == abcTextOffsetLabel && cachePriceActionSwingPro[idx].AbcMaxRetracement == abcMaxRetracement && cachePriceActionSwingPro[idx].AbcMinRetracement == abcMinRetracement && cachePriceActionSwingPro[idx].EntryLineStyle == entryLineStyle && cachePriceActionSwingPro[idx].EntryLineWidth == entryLineWidth && cachePriceActionSwingPro[idx].RetracementEntryValue == retracementEntryValue && cachePriceActionSwingPro[idx].ShowEntryArrows == showEntryArrows && cachePriceActionSwingPro[idx].ShowHistoricalEntryLine == showHistoricalEntryLine && cachePriceActionSwingPro[idx].YTickOffset == yTickOffset && cachePriceActionSwingPro[idx].ShowDivergenceRegular == showDivergenceRegular && cachePriceActionSwingPro[idx].ShowDivergenceHidden == showDivergenceHidden && cachePriceActionSwingPro[idx].DivDnLineStyle == divDnLineStyle && cachePriceActionSwingPro[idx].DivUpLineStyle == divUpLineStyle && cachePriceActionSwingPro[idx].DivDnLineWidth == divDnLineWidth && cachePriceActionSwingPro[idx].DivUpLineWidth == divUpLineWidth && cachePriceActionSwingPro[idx].ShowHistoricalNakedSwings == showHistoricalNakedSwings && cachePriceActionSwingPro[idx].NakedSwingDashStyle == nakedSwingDashStyle && cachePriceActionSwingPro[idx].NakedSwingLineWidth == nakedSwingLineWidth && cachePriceActionSwingPro[idx].IgnoreInsideBars == ignoreInsideBars && cachePriceActionSwingPro[idx].UseBreakouts == useBreakouts && cachePriceActionSwingPro[idx].AlertAbc == alertAbc && cachePriceActionSwingPro[idx].AlertAbcEntry == alertAbcEntry && cachePriceActionSwingPro[idx].AlertAbcPriority == alertAbcPriority && cachePriceActionSwingPro[idx].AlertAbcEntryPriority == alertAbcEntryPriority && cachePriceActionSwingPro[idx].AlertAbcLongSoundFileName == alertAbcLongSoundFileName && cachePriceActionSwingPro[idx].AlertAbcLongEntrySoundFileName == alertAbcLongEntrySoundFileName && cachePriceActionSwingPro[idx].AlertAbcShortSoundFileName == alertAbcShortSoundFileName && cachePriceActionSwingPro[idx].AlertAbcShortEntrySoundFileName == alertAbcShortEntrySoundFileName && cachePriceActionSwingPro[idx].AlertDoubleBottom == alertDoubleBottom && cachePriceActionSwingPro[idx].AlertDoubleBottomPriority == alertDoubleBottomPriority && cachePriceActionSwingPro[idx].AlertDoubleBottomMessage == alertDoubleBottomMessage && cachePriceActionSwingPro[idx].AlertDoubleBottomSoundFileName == alertDoubleBottomSoundFileName && cachePriceActionSwingPro[idx].AlertDoubleBottomRearmSeconds == alertDoubleBottomRearmSeconds && cachePriceActionSwingPro[idx].AlertDoubleTop == alertDoubleTop && cachePriceActionSwingPro[idx].AlertDoubleTopPriority == alertDoubleTopPriority && cachePriceActionSwingPro[idx].AlertDoubleTopMessage == alertDoubleTopMessage && cachePriceActionSwingPro[idx].AlertDoubleTopSoundFileName == alertDoubleTopSoundFileName && cachePriceActionSwingPro[idx].AlertDoubleTopRearmSeconds == alertDoubleTopRearmSeconds && cachePriceActionSwingPro[idx].AlertHigherLow == alertHigherLow && cachePriceActionSwingPro[idx].AlertHigherLowPriority == alertHigherLowPriority && cachePriceActionSwingPro[idx].AlertHigherLowMessage == alertHigherLowMessage && cachePriceActionSwingPro[idx].AlertHigherLowSoundFileName == alertHigherLowSoundFileName && cachePriceActionSwingPro[idx].AlertHigherLowRearmSeconds == alertHigherLowRearmSeconds && cachePriceActionSwingPro[idx].AlertLowerHigh == alertLowerHigh && cachePriceActionSwingPro[idx].AlertLowerHighPriority == alertLowerHighPriority && cachePriceActionSwingPro[idx].AlertLowerHighMessage == alertLowerHighMessage && cachePriceActionSwingPro[idx].AlertLowerHighSoundFileName == alertLowerHighSoundFileName && cachePriceActionSwingPro[idx].AlertLowerHighRearmSeconds == alertLowerHighRearmSeconds && cachePriceActionSwingPro[idx].AlertSwingChange == alertSwingChange && cachePriceActionSwingPro[idx].AlertSwingChangePriority == alertSwingChangePriority && cachePriceActionSwingPro[idx].AlertSwingChangeMessage == alertSwingChangeMessage && cachePriceActionSwingPro[idx].AlertSwingChangeSoundFileName == alertSwingChangeSoundFileName && cachePriceActionSwingPro[idx].AlertSwingChangeRearmSeconds == alertSwingChangeRearmSeconds && cachePriceActionSwingPro[idx].AlertDivergenceRegularHigh == alertDivergenceRegularHigh && cachePriceActionSwingPro[idx].AlertDivergenceRegularHighPriority == alertDivergenceRegularHighPriority && cachePriceActionSwingPro[idx].AlertDivergenceRegularHighMessage == alertDivergenceRegularHighMessage && cachePriceActionSwingPro[idx].AlertDivergenceRegularHighSoundFileName == alertDivergenceRegularHighSoundFileName && cachePriceActionSwingPro[idx].AlertDivergenceRegularHighRearmSeconds == alertDivergenceRegularHighRearmSeconds && cachePriceActionSwingPro[idx].AlertDivergenceHiddenHigh == alertDivergenceHiddenHigh && cachePriceActionSwingPro[idx].AlertDivergenceHiddenHighPriority == alertDivergenceHiddenHighPriority && cachePriceActionSwingPro[idx].AlertDivergenceHiddenHighMessage == alertDivergenceHiddenHighMessage && cachePriceActionSwingPro[idx].AlertDivergenceHiddenHighSoundFileName == alertDivergenceHiddenHighSoundFileName && cachePriceActionSwingPro[idx].AlertDivergenceHiddenHighRearmSeconds == alertDivergenceHiddenHighRearmSeconds && cachePriceActionSwingPro[idx].AlertDivergenceRegularLow == alertDivergenceRegularLow && cachePriceActionSwingPro[idx].AlertDivergenceRegularLowPriority == alertDivergenceRegularLowPriority && cachePriceActionSwingPro[idx].AlertDivergenceRegularLowMessage == alertDivergenceRegularLowMessage && cachePriceActionSwingPro[idx].AlertDivergenceRegularLowSoundFileName == alertDivergenceRegularLowSoundFileName && cachePriceActionSwingPro[idx].AlertDivergenceRegularLowRearmSeconds == alertDivergenceRegularLowRearmSeconds && cachePriceActionSwingPro[idx].AlertDivergenceHiddenLow == alertDivergenceHiddenLow && cachePriceActionSwingPro[idx].AlertDivergenceHiddenLowPriority == alertDivergenceHiddenLowPriority && cachePriceActionSwingPro[idx].AlertDivergenceHiddenLowMessage == alertDivergenceHiddenLowMessage && cachePriceActionSwingPro[idx].AlertDivergenceHiddenLowSoundFileName == alertDivergenceHiddenLowSoundFileName && cachePriceActionSwingPro[idx].AlertDivergenceHiddenLowRearmSeconds == alertDivergenceHiddenLowRearmSeconds && cachePriceActionSwingPro[idx].EqualsInput(input))
						return cachePriceActionSwingPro[idx];
			return CacheIndicator<PriceActionSwing.PriceActionSwingPro>(new PriceActionSwing.PriceActionSwingPro(){ SwingType = swingType, SwingSize = swingSize, DtbStrength = dtbStrength, UseCloseValues = useCloseValues, AbcPattern = abcPattern, DivergenceIndicatorMode = divergenceIndicatorMode, DivergenceDirectionMode = divergenceDirectionMode, Param1 = param1, Param2 = param2, Param3 = param3, ShowNakedSwings = showNakedSwings, AddSwingExtension = addSwingExtension, AddSwingRetracementFast = addSwingRetracementFast, AddSwingRetracementSlow = addSwingRetracementSlow, SwingLengthType = swingLengthType, SwingDurationType = swingDurationType, ShowSwingPrice = showSwingPrice, ShowSwingLabel = showSwingLabel, ShowSwingPercent = showSwingPercent, SwingTimeType = swingTimeType, SwingVolumeType = swingVolumeType, VisualizationType = visualizationType, TextFont = textFont, TextOffsetLength = textOffsetLength, TextOffsetVolume = textOffsetVolume, TextOffsetPrice = textOffsetPrice, TextOffsetLabel = textOffsetLabel, TextOffsetTime = textOffsetTime, TextOffsetPercent = textOffsetPercent, ZigZagStyle = zigZagStyle, ZigZagWidth = zigZagWidth, ShowSwingSwitch = showSwingSwitch, SwingSwitchOffsetInTicks = swingSwitchOffsetInTicks, AbcLineStyle = abcLineStyle, AbcLineStyleRatio = abcLineStyleRatio, AbcLineWidth = abcLineWidth, AbcLineWidthRatio = abcLineWidthRatio, AbcTextFont = abcTextFont, AbcTextOffsetLabel = abcTextOffsetLabel, AbcMaxRetracement = abcMaxRetracement, AbcMinRetracement = abcMinRetracement, EntryLineStyle = entryLineStyle, EntryLineWidth = entryLineWidth, RetracementEntryValue = retracementEntryValue, ShowEntryArrows = showEntryArrows, ShowHistoricalEntryLine = showHistoricalEntryLine, YTickOffset = yTickOffset, ShowDivergenceRegular = showDivergenceRegular, ShowDivergenceHidden = showDivergenceHidden, DivDnLineStyle = divDnLineStyle, DivUpLineStyle = divUpLineStyle, DivDnLineWidth = divDnLineWidth, DivUpLineWidth = divUpLineWidth, ShowHistoricalNakedSwings = showHistoricalNakedSwings, NakedSwingDashStyle = nakedSwingDashStyle, NakedSwingLineWidth = nakedSwingLineWidth, IgnoreInsideBars = ignoreInsideBars, UseBreakouts = useBreakouts, AlertAbc = alertAbc, AlertAbcEntry = alertAbcEntry, AlertAbcPriority = alertAbcPriority, AlertAbcEntryPriority = alertAbcEntryPriority, AlertAbcLongSoundFileName = alertAbcLongSoundFileName, AlertAbcLongEntrySoundFileName = alertAbcLongEntrySoundFileName, AlertAbcShortSoundFileName = alertAbcShortSoundFileName, AlertAbcShortEntrySoundFileName = alertAbcShortEntrySoundFileName, AlertDoubleBottom = alertDoubleBottom, AlertDoubleBottomPriority = alertDoubleBottomPriority, AlertDoubleBottomMessage = alertDoubleBottomMessage, AlertDoubleBottomSoundFileName = alertDoubleBottomSoundFileName, AlertDoubleBottomRearmSeconds = alertDoubleBottomRearmSeconds, AlertDoubleTop = alertDoubleTop, AlertDoubleTopPriority = alertDoubleTopPriority, AlertDoubleTopMessage = alertDoubleTopMessage, AlertDoubleTopSoundFileName = alertDoubleTopSoundFileName, AlertDoubleTopRearmSeconds = alertDoubleTopRearmSeconds, AlertHigherLow = alertHigherLow, AlertHigherLowPriority = alertHigherLowPriority, AlertHigherLowMessage = alertHigherLowMessage, AlertHigherLowSoundFileName = alertHigherLowSoundFileName, AlertHigherLowRearmSeconds = alertHigherLowRearmSeconds, AlertLowerHigh = alertLowerHigh, AlertLowerHighPriority = alertLowerHighPriority, AlertLowerHighMessage = alertLowerHighMessage, AlertLowerHighSoundFileName = alertLowerHighSoundFileName, AlertLowerHighRearmSeconds = alertLowerHighRearmSeconds, AlertSwingChange = alertSwingChange, AlertSwingChangePriority = alertSwingChangePriority, AlertSwingChangeMessage = alertSwingChangeMessage, AlertSwingChangeSoundFileName = alertSwingChangeSoundFileName, AlertSwingChangeRearmSeconds = alertSwingChangeRearmSeconds, AlertDivergenceRegularHigh = alertDivergenceRegularHigh, AlertDivergenceRegularHighPriority = alertDivergenceRegularHighPriority, AlertDivergenceRegularHighMessage = alertDivergenceRegularHighMessage, AlertDivergenceRegularHighSoundFileName = alertDivergenceRegularHighSoundFileName, AlertDivergenceRegularHighRearmSeconds = alertDivergenceRegularHighRearmSeconds, AlertDivergenceHiddenHigh = alertDivergenceHiddenHigh, AlertDivergenceHiddenHighPriority = alertDivergenceHiddenHighPriority, AlertDivergenceHiddenHighMessage = alertDivergenceHiddenHighMessage, AlertDivergenceHiddenHighSoundFileName = alertDivergenceHiddenHighSoundFileName, AlertDivergenceHiddenHighRearmSeconds = alertDivergenceHiddenHighRearmSeconds, AlertDivergenceRegularLow = alertDivergenceRegularLow, AlertDivergenceRegularLowPriority = alertDivergenceRegularLowPriority, AlertDivergenceRegularLowMessage = alertDivergenceRegularLowMessage, AlertDivergenceRegularLowSoundFileName = alertDivergenceRegularLowSoundFileName, AlertDivergenceRegularLowRearmSeconds = alertDivergenceRegularLowRearmSeconds, AlertDivergenceHiddenLow = alertDivergenceHiddenLow, AlertDivergenceHiddenLowPriority = alertDivergenceHiddenLowPriority, AlertDivergenceHiddenLowMessage = alertDivergenceHiddenLowMessage, AlertDivergenceHiddenLowSoundFileName = alertDivergenceHiddenLowSoundFileName, AlertDivergenceHiddenLowRearmSeconds = alertDivergenceHiddenLowRearmSeconds }, input, ref cachePriceActionSwingPro);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.PriceActionSwing.PriceActionSwingPro PriceActionSwingPro(SwingStyle swingType, double swingSize, int dtbStrength, bool useCloseValues, AbcPatternMode abcPattern, DivergenceMode divergenceIndicatorMode, DivergenceDirection divergenceDirectionMode, int param1, int param2, int param3, bool showNakedSwings, bool addSwingExtension, bool addSwingRetracementFast, bool addSwingRetracementSlow, SwingLengthStyle swingLengthType, SwingDurationStyle swingDurationType, bool showSwingPrice, bool showSwingLabel, bool showSwingPercent, SwingTimeStyle swingTimeType, SwingVolumeStyle swingVolumeType, VisualizationStyle visualizationType, NinjaTrader.Gui.Tools.SimpleFont textFont, int textOffsetLength, int textOffsetVolume, int textOffsetPrice, int textOffsetLabel, int textOffsetTime, int textOffsetPercent, DashStyleHelper zigZagStyle, int zigZagWidth, bool showSwingSwitch, int swingSwitchOffsetInTicks, DashStyleHelper abcLineStyle, DashStyleHelper abcLineStyleRatio, int abcLineWidth, int abcLineWidthRatio, NinjaTrader.Gui.Tools.SimpleFont abcTextFont, int abcTextOffsetLabel, double abcMaxRetracement, double abcMinRetracement, DashStyleHelper entryLineStyle, int entryLineWidth, double retracementEntryValue, bool showEntryArrows, bool showHistoricalEntryLine, int yTickOffset, bool showDivergenceRegular, bool showDivergenceHidden, DashStyleHelper divDnLineStyle, DashStyleHelper divUpLineStyle, int divDnLineWidth, int divUpLineWidth, bool showHistoricalNakedSwings, DashStyleHelper nakedSwingDashStyle, int nakedSwingLineWidth, bool ignoreInsideBars, bool useBreakouts, bool alertAbc, bool alertAbcEntry, Priority alertAbcPriority, Priority alertAbcEntryPriority, string alertAbcLongSoundFileName, string alertAbcLongEntrySoundFileName, string alertAbcShortSoundFileName, string alertAbcShortEntrySoundFileName, bool alertDoubleBottom, Priority alertDoubleBottomPriority, string alertDoubleBottomMessage, string alertDoubleBottomSoundFileName, int alertDoubleBottomRearmSeconds, bool alertDoubleTop, Priority alertDoubleTopPriority, string alertDoubleTopMessage, string alertDoubleTopSoundFileName, int alertDoubleTopRearmSeconds, bool alertHigherLow, Priority alertHigherLowPriority, string alertHigherLowMessage, string alertHigherLowSoundFileName, int alertHigherLowRearmSeconds, bool alertLowerHigh, Priority alertLowerHighPriority, string alertLowerHighMessage, string alertLowerHighSoundFileName, int alertLowerHighRearmSeconds, bool alertSwingChange, Priority alertSwingChangePriority, string alertSwingChangeMessage, string alertSwingChangeSoundFileName, int alertSwingChangeRearmSeconds, bool alertDivergenceRegularHigh, Priority alertDivergenceRegularHighPriority, string alertDivergenceRegularHighMessage, string alertDivergenceRegularHighSoundFileName, int alertDivergenceRegularHighRearmSeconds, bool alertDivergenceHiddenHigh, Priority alertDivergenceHiddenHighPriority, string alertDivergenceHiddenHighMessage, string alertDivergenceHiddenHighSoundFileName, int alertDivergenceHiddenHighRearmSeconds, bool alertDivergenceRegularLow, Priority alertDivergenceRegularLowPriority, string alertDivergenceRegularLowMessage, string alertDivergenceRegularLowSoundFileName, int alertDivergenceRegularLowRearmSeconds, bool alertDivergenceHiddenLow, Priority alertDivergenceHiddenLowPriority, string alertDivergenceHiddenLowMessage, string alertDivergenceHiddenLowSoundFileName, int alertDivergenceHiddenLowRearmSeconds)
		{
			return indicator.PriceActionSwingPro(Input, swingType, swingSize, dtbStrength, useCloseValues, abcPattern, divergenceIndicatorMode, divergenceDirectionMode, param1, param2, param3, showNakedSwings, addSwingExtension, addSwingRetracementFast, addSwingRetracementSlow, swingLengthType, swingDurationType, showSwingPrice, showSwingLabel, showSwingPercent, swingTimeType, swingVolumeType, visualizationType, textFont, textOffsetLength, textOffsetVolume, textOffsetPrice, textOffsetLabel, textOffsetTime, textOffsetPercent, zigZagStyle, zigZagWidth, showSwingSwitch, swingSwitchOffsetInTicks, abcLineStyle, abcLineStyleRatio, abcLineWidth, abcLineWidthRatio, abcTextFont, abcTextOffsetLabel, abcMaxRetracement, abcMinRetracement, entryLineStyle, entryLineWidth, retracementEntryValue, showEntryArrows, showHistoricalEntryLine, yTickOffset, showDivergenceRegular, showDivergenceHidden, divDnLineStyle, divUpLineStyle, divDnLineWidth, divUpLineWidth, showHistoricalNakedSwings, nakedSwingDashStyle, nakedSwingLineWidth, ignoreInsideBars, useBreakouts, alertAbc, alertAbcEntry, alertAbcPriority, alertAbcEntryPriority, alertAbcLongSoundFileName, alertAbcLongEntrySoundFileName, alertAbcShortSoundFileName, alertAbcShortEntrySoundFileName, alertDoubleBottom, alertDoubleBottomPriority, alertDoubleBottomMessage, alertDoubleBottomSoundFileName, alertDoubleBottomRearmSeconds, alertDoubleTop, alertDoubleTopPriority, alertDoubleTopMessage, alertDoubleTopSoundFileName, alertDoubleTopRearmSeconds, alertHigherLow, alertHigherLowPriority, alertHigherLowMessage, alertHigherLowSoundFileName, alertHigherLowRearmSeconds, alertLowerHigh, alertLowerHighPriority, alertLowerHighMessage, alertLowerHighSoundFileName, alertLowerHighRearmSeconds, alertSwingChange, alertSwingChangePriority, alertSwingChangeMessage, alertSwingChangeSoundFileName, alertSwingChangeRearmSeconds, alertDivergenceRegularHigh, alertDivergenceRegularHighPriority, alertDivergenceRegularHighMessage, alertDivergenceRegularHighSoundFileName, alertDivergenceRegularHighRearmSeconds, alertDivergenceHiddenHigh, alertDivergenceHiddenHighPriority, alertDivergenceHiddenHighMessage, alertDivergenceHiddenHighSoundFileName, alertDivergenceHiddenHighRearmSeconds, alertDivergenceRegularLow, alertDivergenceRegularLowPriority, alertDivergenceRegularLowMessage, alertDivergenceRegularLowSoundFileName, alertDivergenceRegularLowRearmSeconds, alertDivergenceHiddenLow, alertDivergenceHiddenLowPriority, alertDivergenceHiddenLowMessage, alertDivergenceHiddenLowSoundFileName, alertDivergenceHiddenLowRearmSeconds);
		}

		public Indicators.PriceActionSwing.PriceActionSwingPro PriceActionSwingPro(ISeries<double> input , SwingStyle swingType, double swingSize, int dtbStrength, bool useCloseValues, AbcPatternMode abcPattern, DivergenceMode divergenceIndicatorMode, DivergenceDirection divergenceDirectionMode, int param1, int param2, int param3, bool showNakedSwings, bool addSwingExtension, bool addSwingRetracementFast, bool addSwingRetracementSlow, SwingLengthStyle swingLengthType, SwingDurationStyle swingDurationType, bool showSwingPrice, bool showSwingLabel, bool showSwingPercent, SwingTimeStyle swingTimeType, SwingVolumeStyle swingVolumeType, VisualizationStyle visualizationType, NinjaTrader.Gui.Tools.SimpleFont textFont, int textOffsetLength, int textOffsetVolume, int textOffsetPrice, int textOffsetLabel, int textOffsetTime, int textOffsetPercent, DashStyleHelper zigZagStyle, int zigZagWidth, bool showSwingSwitch, int swingSwitchOffsetInTicks, DashStyleHelper abcLineStyle, DashStyleHelper abcLineStyleRatio, int abcLineWidth, int abcLineWidthRatio, NinjaTrader.Gui.Tools.SimpleFont abcTextFont, int abcTextOffsetLabel, double abcMaxRetracement, double abcMinRetracement, DashStyleHelper entryLineStyle, int entryLineWidth, double retracementEntryValue, bool showEntryArrows, bool showHistoricalEntryLine, int yTickOffset, bool showDivergenceRegular, bool showDivergenceHidden, DashStyleHelper divDnLineStyle, DashStyleHelper divUpLineStyle, int divDnLineWidth, int divUpLineWidth, bool showHistoricalNakedSwings, DashStyleHelper nakedSwingDashStyle, int nakedSwingLineWidth, bool ignoreInsideBars, bool useBreakouts, bool alertAbc, bool alertAbcEntry, Priority alertAbcPriority, Priority alertAbcEntryPriority, string alertAbcLongSoundFileName, string alertAbcLongEntrySoundFileName, string alertAbcShortSoundFileName, string alertAbcShortEntrySoundFileName, bool alertDoubleBottom, Priority alertDoubleBottomPriority, string alertDoubleBottomMessage, string alertDoubleBottomSoundFileName, int alertDoubleBottomRearmSeconds, bool alertDoubleTop, Priority alertDoubleTopPriority, string alertDoubleTopMessage, string alertDoubleTopSoundFileName, int alertDoubleTopRearmSeconds, bool alertHigherLow, Priority alertHigherLowPriority, string alertHigherLowMessage, string alertHigherLowSoundFileName, int alertHigherLowRearmSeconds, bool alertLowerHigh, Priority alertLowerHighPriority, string alertLowerHighMessage, string alertLowerHighSoundFileName, int alertLowerHighRearmSeconds, bool alertSwingChange, Priority alertSwingChangePriority, string alertSwingChangeMessage, string alertSwingChangeSoundFileName, int alertSwingChangeRearmSeconds, bool alertDivergenceRegularHigh, Priority alertDivergenceRegularHighPriority, string alertDivergenceRegularHighMessage, string alertDivergenceRegularHighSoundFileName, int alertDivergenceRegularHighRearmSeconds, bool alertDivergenceHiddenHigh, Priority alertDivergenceHiddenHighPriority, string alertDivergenceHiddenHighMessage, string alertDivergenceHiddenHighSoundFileName, int alertDivergenceHiddenHighRearmSeconds, bool alertDivergenceRegularLow, Priority alertDivergenceRegularLowPriority, string alertDivergenceRegularLowMessage, string alertDivergenceRegularLowSoundFileName, int alertDivergenceRegularLowRearmSeconds, bool alertDivergenceHiddenLow, Priority alertDivergenceHiddenLowPriority, string alertDivergenceHiddenLowMessage, string alertDivergenceHiddenLowSoundFileName, int alertDivergenceHiddenLowRearmSeconds)
		{
			return indicator.PriceActionSwingPro(input, swingType, swingSize, dtbStrength, useCloseValues, abcPattern, divergenceIndicatorMode, divergenceDirectionMode, param1, param2, param3, showNakedSwings, addSwingExtension, addSwingRetracementFast, addSwingRetracementSlow, swingLengthType, swingDurationType, showSwingPrice, showSwingLabel, showSwingPercent, swingTimeType, swingVolumeType, visualizationType, textFont, textOffsetLength, textOffsetVolume, textOffsetPrice, textOffsetLabel, textOffsetTime, textOffsetPercent, zigZagStyle, zigZagWidth, showSwingSwitch, swingSwitchOffsetInTicks, abcLineStyle, abcLineStyleRatio, abcLineWidth, abcLineWidthRatio, abcTextFont, abcTextOffsetLabel, abcMaxRetracement, abcMinRetracement, entryLineStyle, entryLineWidth, retracementEntryValue, showEntryArrows, showHistoricalEntryLine, yTickOffset, showDivergenceRegular, showDivergenceHidden, divDnLineStyle, divUpLineStyle, divDnLineWidth, divUpLineWidth, showHistoricalNakedSwings, nakedSwingDashStyle, nakedSwingLineWidth, ignoreInsideBars, useBreakouts, alertAbc, alertAbcEntry, alertAbcPriority, alertAbcEntryPriority, alertAbcLongSoundFileName, alertAbcLongEntrySoundFileName, alertAbcShortSoundFileName, alertAbcShortEntrySoundFileName, alertDoubleBottom, alertDoubleBottomPriority, alertDoubleBottomMessage, alertDoubleBottomSoundFileName, alertDoubleBottomRearmSeconds, alertDoubleTop, alertDoubleTopPriority, alertDoubleTopMessage, alertDoubleTopSoundFileName, alertDoubleTopRearmSeconds, alertHigherLow, alertHigherLowPriority, alertHigherLowMessage, alertHigherLowSoundFileName, alertHigherLowRearmSeconds, alertLowerHigh, alertLowerHighPriority, alertLowerHighMessage, alertLowerHighSoundFileName, alertLowerHighRearmSeconds, alertSwingChange, alertSwingChangePriority, alertSwingChangeMessage, alertSwingChangeSoundFileName, alertSwingChangeRearmSeconds, alertDivergenceRegularHigh, alertDivergenceRegularHighPriority, alertDivergenceRegularHighMessage, alertDivergenceRegularHighSoundFileName, alertDivergenceRegularHighRearmSeconds, alertDivergenceHiddenHigh, alertDivergenceHiddenHighPriority, alertDivergenceHiddenHighMessage, alertDivergenceHiddenHighSoundFileName, alertDivergenceHiddenHighRearmSeconds, alertDivergenceRegularLow, alertDivergenceRegularLowPriority, alertDivergenceRegularLowMessage, alertDivergenceRegularLowSoundFileName, alertDivergenceRegularLowRearmSeconds, alertDivergenceHiddenLow, alertDivergenceHiddenLowPriority, alertDivergenceHiddenLowMessage, alertDivergenceHiddenLowSoundFileName, alertDivergenceHiddenLowRearmSeconds);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.PriceActionSwing.PriceActionSwingPro PriceActionSwingPro(SwingStyle swingType, double swingSize, int dtbStrength, bool useCloseValues, AbcPatternMode abcPattern, DivergenceMode divergenceIndicatorMode, DivergenceDirection divergenceDirectionMode, int param1, int param2, int param3, bool showNakedSwings, bool addSwingExtension, bool addSwingRetracementFast, bool addSwingRetracementSlow, SwingLengthStyle swingLengthType, SwingDurationStyle swingDurationType, bool showSwingPrice, bool showSwingLabel, bool showSwingPercent, SwingTimeStyle swingTimeType, SwingVolumeStyle swingVolumeType, VisualizationStyle visualizationType, NinjaTrader.Gui.Tools.SimpleFont textFont, int textOffsetLength, int textOffsetVolume, int textOffsetPrice, int textOffsetLabel, int textOffsetTime, int textOffsetPercent, DashStyleHelper zigZagStyle, int zigZagWidth, bool showSwingSwitch, int swingSwitchOffsetInTicks, DashStyleHelper abcLineStyle, DashStyleHelper abcLineStyleRatio, int abcLineWidth, int abcLineWidthRatio, NinjaTrader.Gui.Tools.SimpleFont abcTextFont, int abcTextOffsetLabel, double abcMaxRetracement, double abcMinRetracement, DashStyleHelper entryLineStyle, int entryLineWidth, double retracementEntryValue, bool showEntryArrows, bool showHistoricalEntryLine, int yTickOffset, bool showDivergenceRegular, bool showDivergenceHidden, DashStyleHelper divDnLineStyle, DashStyleHelper divUpLineStyle, int divDnLineWidth, int divUpLineWidth, bool showHistoricalNakedSwings, DashStyleHelper nakedSwingDashStyle, int nakedSwingLineWidth, bool ignoreInsideBars, bool useBreakouts, bool alertAbc, bool alertAbcEntry, Priority alertAbcPriority, Priority alertAbcEntryPriority, string alertAbcLongSoundFileName, string alertAbcLongEntrySoundFileName, string alertAbcShortSoundFileName, string alertAbcShortEntrySoundFileName, bool alertDoubleBottom, Priority alertDoubleBottomPriority, string alertDoubleBottomMessage, string alertDoubleBottomSoundFileName, int alertDoubleBottomRearmSeconds, bool alertDoubleTop, Priority alertDoubleTopPriority, string alertDoubleTopMessage, string alertDoubleTopSoundFileName, int alertDoubleTopRearmSeconds, bool alertHigherLow, Priority alertHigherLowPriority, string alertHigherLowMessage, string alertHigherLowSoundFileName, int alertHigherLowRearmSeconds, bool alertLowerHigh, Priority alertLowerHighPriority, string alertLowerHighMessage, string alertLowerHighSoundFileName, int alertLowerHighRearmSeconds, bool alertSwingChange, Priority alertSwingChangePriority, string alertSwingChangeMessage, string alertSwingChangeSoundFileName, int alertSwingChangeRearmSeconds, bool alertDivergenceRegularHigh, Priority alertDivergenceRegularHighPriority, string alertDivergenceRegularHighMessage, string alertDivergenceRegularHighSoundFileName, int alertDivergenceRegularHighRearmSeconds, bool alertDivergenceHiddenHigh, Priority alertDivergenceHiddenHighPriority, string alertDivergenceHiddenHighMessage, string alertDivergenceHiddenHighSoundFileName, int alertDivergenceHiddenHighRearmSeconds, bool alertDivergenceRegularLow, Priority alertDivergenceRegularLowPriority, string alertDivergenceRegularLowMessage, string alertDivergenceRegularLowSoundFileName, int alertDivergenceRegularLowRearmSeconds, bool alertDivergenceHiddenLow, Priority alertDivergenceHiddenLowPriority, string alertDivergenceHiddenLowMessage, string alertDivergenceHiddenLowSoundFileName, int alertDivergenceHiddenLowRearmSeconds)
		{
			return indicator.PriceActionSwingPro(Input, swingType, swingSize, dtbStrength, useCloseValues, abcPattern, divergenceIndicatorMode, divergenceDirectionMode, param1, param2, param3, showNakedSwings, addSwingExtension, addSwingRetracementFast, addSwingRetracementSlow, swingLengthType, swingDurationType, showSwingPrice, showSwingLabel, showSwingPercent, swingTimeType, swingVolumeType, visualizationType, textFont, textOffsetLength, textOffsetVolume, textOffsetPrice, textOffsetLabel, textOffsetTime, textOffsetPercent, zigZagStyle, zigZagWidth, showSwingSwitch, swingSwitchOffsetInTicks, abcLineStyle, abcLineStyleRatio, abcLineWidth, abcLineWidthRatio, abcTextFont, abcTextOffsetLabel, abcMaxRetracement, abcMinRetracement, entryLineStyle, entryLineWidth, retracementEntryValue, showEntryArrows, showHistoricalEntryLine, yTickOffset, showDivergenceRegular, showDivergenceHidden, divDnLineStyle, divUpLineStyle, divDnLineWidth, divUpLineWidth, showHistoricalNakedSwings, nakedSwingDashStyle, nakedSwingLineWidth, ignoreInsideBars, useBreakouts, alertAbc, alertAbcEntry, alertAbcPriority, alertAbcEntryPriority, alertAbcLongSoundFileName, alertAbcLongEntrySoundFileName, alertAbcShortSoundFileName, alertAbcShortEntrySoundFileName, alertDoubleBottom, alertDoubleBottomPriority, alertDoubleBottomMessage, alertDoubleBottomSoundFileName, alertDoubleBottomRearmSeconds, alertDoubleTop, alertDoubleTopPriority, alertDoubleTopMessage, alertDoubleTopSoundFileName, alertDoubleTopRearmSeconds, alertHigherLow, alertHigherLowPriority, alertHigherLowMessage, alertHigherLowSoundFileName, alertHigherLowRearmSeconds, alertLowerHigh, alertLowerHighPriority, alertLowerHighMessage, alertLowerHighSoundFileName, alertLowerHighRearmSeconds, alertSwingChange, alertSwingChangePriority, alertSwingChangeMessage, alertSwingChangeSoundFileName, alertSwingChangeRearmSeconds, alertDivergenceRegularHigh, alertDivergenceRegularHighPriority, alertDivergenceRegularHighMessage, alertDivergenceRegularHighSoundFileName, alertDivergenceRegularHighRearmSeconds, alertDivergenceHiddenHigh, alertDivergenceHiddenHighPriority, alertDivergenceHiddenHighMessage, alertDivergenceHiddenHighSoundFileName, alertDivergenceHiddenHighRearmSeconds, alertDivergenceRegularLow, alertDivergenceRegularLowPriority, alertDivergenceRegularLowMessage, alertDivergenceRegularLowSoundFileName, alertDivergenceRegularLowRearmSeconds, alertDivergenceHiddenLow, alertDivergenceHiddenLowPriority, alertDivergenceHiddenLowMessage, alertDivergenceHiddenLowSoundFileName, alertDivergenceHiddenLowRearmSeconds);
		}

		public Indicators.PriceActionSwing.PriceActionSwingPro PriceActionSwingPro(ISeries<double> input , SwingStyle swingType, double swingSize, int dtbStrength, bool useCloseValues, AbcPatternMode abcPattern, DivergenceMode divergenceIndicatorMode, DivergenceDirection divergenceDirectionMode, int param1, int param2, int param3, bool showNakedSwings, bool addSwingExtension, bool addSwingRetracementFast, bool addSwingRetracementSlow, SwingLengthStyle swingLengthType, SwingDurationStyle swingDurationType, bool showSwingPrice, bool showSwingLabel, bool showSwingPercent, SwingTimeStyle swingTimeType, SwingVolumeStyle swingVolumeType, VisualizationStyle visualizationType, NinjaTrader.Gui.Tools.SimpleFont textFont, int textOffsetLength, int textOffsetVolume, int textOffsetPrice, int textOffsetLabel, int textOffsetTime, int textOffsetPercent, DashStyleHelper zigZagStyle, int zigZagWidth, bool showSwingSwitch, int swingSwitchOffsetInTicks, DashStyleHelper abcLineStyle, DashStyleHelper abcLineStyleRatio, int abcLineWidth, int abcLineWidthRatio, NinjaTrader.Gui.Tools.SimpleFont abcTextFont, int abcTextOffsetLabel, double abcMaxRetracement, double abcMinRetracement, DashStyleHelper entryLineStyle, int entryLineWidth, double retracementEntryValue, bool showEntryArrows, bool showHistoricalEntryLine, int yTickOffset, bool showDivergenceRegular, bool showDivergenceHidden, DashStyleHelper divDnLineStyle, DashStyleHelper divUpLineStyle, int divDnLineWidth, int divUpLineWidth, bool showHistoricalNakedSwings, DashStyleHelper nakedSwingDashStyle, int nakedSwingLineWidth, bool ignoreInsideBars, bool useBreakouts, bool alertAbc, bool alertAbcEntry, Priority alertAbcPriority, Priority alertAbcEntryPriority, string alertAbcLongSoundFileName, string alertAbcLongEntrySoundFileName, string alertAbcShortSoundFileName, string alertAbcShortEntrySoundFileName, bool alertDoubleBottom, Priority alertDoubleBottomPriority, string alertDoubleBottomMessage, string alertDoubleBottomSoundFileName, int alertDoubleBottomRearmSeconds, bool alertDoubleTop, Priority alertDoubleTopPriority, string alertDoubleTopMessage, string alertDoubleTopSoundFileName, int alertDoubleTopRearmSeconds, bool alertHigherLow, Priority alertHigherLowPriority, string alertHigherLowMessage, string alertHigherLowSoundFileName, int alertHigherLowRearmSeconds, bool alertLowerHigh, Priority alertLowerHighPriority, string alertLowerHighMessage, string alertLowerHighSoundFileName, int alertLowerHighRearmSeconds, bool alertSwingChange, Priority alertSwingChangePriority, string alertSwingChangeMessage, string alertSwingChangeSoundFileName, int alertSwingChangeRearmSeconds, bool alertDivergenceRegularHigh, Priority alertDivergenceRegularHighPriority, string alertDivergenceRegularHighMessage, string alertDivergenceRegularHighSoundFileName, int alertDivergenceRegularHighRearmSeconds, bool alertDivergenceHiddenHigh, Priority alertDivergenceHiddenHighPriority, string alertDivergenceHiddenHighMessage, string alertDivergenceHiddenHighSoundFileName, int alertDivergenceHiddenHighRearmSeconds, bool alertDivergenceRegularLow, Priority alertDivergenceRegularLowPriority, string alertDivergenceRegularLowMessage, string alertDivergenceRegularLowSoundFileName, int alertDivergenceRegularLowRearmSeconds, bool alertDivergenceHiddenLow, Priority alertDivergenceHiddenLowPriority, string alertDivergenceHiddenLowMessage, string alertDivergenceHiddenLowSoundFileName, int alertDivergenceHiddenLowRearmSeconds)
		{
			return indicator.PriceActionSwingPro(input, swingType, swingSize, dtbStrength, useCloseValues, abcPattern, divergenceIndicatorMode, divergenceDirectionMode, param1, param2, param3, showNakedSwings, addSwingExtension, addSwingRetracementFast, addSwingRetracementSlow, swingLengthType, swingDurationType, showSwingPrice, showSwingLabel, showSwingPercent, swingTimeType, swingVolumeType, visualizationType, textFont, textOffsetLength, textOffsetVolume, textOffsetPrice, textOffsetLabel, textOffsetTime, textOffsetPercent, zigZagStyle, zigZagWidth, showSwingSwitch, swingSwitchOffsetInTicks, abcLineStyle, abcLineStyleRatio, abcLineWidth, abcLineWidthRatio, abcTextFont, abcTextOffsetLabel, abcMaxRetracement, abcMinRetracement, entryLineStyle, entryLineWidth, retracementEntryValue, showEntryArrows, showHistoricalEntryLine, yTickOffset, showDivergenceRegular, showDivergenceHidden, divDnLineStyle, divUpLineStyle, divDnLineWidth, divUpLineWidth, showHistoricalNakedSwings, nakedSwingDashStyle, nakedSwingLineWidth, ignoreInsideBars, useBreakouts, alertAbc, alertAbcEntry, alertAbcPriority, alertAbcEntryPriority, alertAbcLongSoundFileName, alertAbcLongEntrySoundFileName, alertAbcShortSoundFileName, alertAbcShortEntrySoundFileName, alertDoubleBottom, alertDoubleBottomPriority, alertDoubleBottomMessage, alertDoubleBottomSoundFileName, alertDoubleBottomRearmSeconds, alertDoubleTop, alertDoubleTopPriority, alertDoubleTopMessage, alertDoubleTopSoundFileName, alertDoubleTopRearmSeconds, alertHigherLow, alertHigherLowPriority, alertHigherLowMessage, alertHigherLowSoundFileName, alertHigherLowRearmSeconds, alertLowerHigh, alertLowerHighPriority, alertLowerHighMessage, alertLowerHighSoundFileName, alertLowerHighRearmSeconds, alertSwingChange, alertSwingChangePriority, alertSwingChangeMessage, alertSwingChangeSoundFileName, alertSwingChangeRearmSeconds, alertDivergenceRegularHigh, alertDivergenceRegularHighPriority, alertDivergenceRegularHighMessage, alertDivergenceRegularHighSoundFileName, alertDivergenceRegularHighRearmSeconds, alertDivergenceHiddenHigh, alertDivergenceHiddenHighPriority, alertDivergenceHiddenHighMessage, alertDivergenceHiddenHighSoundFileName, alertDivergenceHiddenHighRearmSeconds, alertDivergenceRegularLow, alertDivergenceRegularLowPriority, alertDivergenceRegularLowMessage, alertDivergenceRegularLowSoundFileName, alertDivergenceRegularLowRearmSeconds, alertDivergenceHiddenLow, alertDivergenceHiddenLowPriority, alertDivergenceHiddenLowMessage, alertDivergenceHiddenLowSoundFileName, alertDivergenceHiddenLowRearmSeconds);
		}
	}
}

#endregion
