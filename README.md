# Ads4Dads_VWAP_Bands

A **session-based VWAP indicator for NinjaTrader 8** with optional ±1σ and ±2σ volume-weighted standard deviation bands. 
Includes auto-coloring of the VWAP line based on price, full band visibility toggles, session reset, and customizable inputs for scalping, 
swing trading, and market analysis.

## Features
- **Session-anchored VWAP** that resets each trading session
- **Auto-colored VWAP line**: green above price, red below price
- Optional **±1σ and ±2σ volume-weighted bands** with independent visibility toggles
- Configurable **session reset** and **price calculation method** (Typical Price or Close)
- Works with any instrument, any timeframe
- Built to **NinjaScript best practices** for performance and compatibility

## Installation
1. Open **NinjaTrader 8**.
2. Go to `New → NinjaScript Editor`.
3. Right-click the **Indicators** folder → `New Indicator...`.
4. Name it `Ads4Dads_VWAP_Bands`.
5. Paste the entire `.cs` code from this repository.
6. Click **Compile**.
7. Add the indicator to your chart via the `Indicators` menu.

## Parameters
| Parameter                | Description |
|--------------------------|-------------|
| **StdDev (inner band)**  | Standard deviation multiplier for ±1σ bands |
| **StdDev (outer band)**  | Standard deviation multiplier for ±2σ bands |
| **Show inner band**      | Toggle ±1σ visibility |
| **Show outer band**      | Toggle ±2σ visibility |
| **Reset on new session** | Reset VWAP at each session open |
| **Use Typical Price**    | Use (H+L+C)/3 instead of Close for VWAP calc |

## Recommended Use
- **Scalping:** Use VWAP as a trend anchor; fade or follow moves relative to VWAP and bands.
- **Swing trading:** Combine VWAP with higher-timeframe S/R for confirmation.
- **Market analysis:** Monitor VWAP as a fair-value reference point.

## Disclaimer
This indicator is provided for educational and informational purposes only. 
It is not intended as investment advice, and no guarantee is made regarding 
profitability or performance. Trading futures, forex, and other financial 
instruments involves substantial risk of loss and is not suitable for all 
investors. Always conduct your own research and consult with a licensed 
financial advisor before making any trading decisions. **Use at your own risk.**

## About This Indicator
This indicator was created and designed by the Ads4Dads team and is provided completely free for you to use and enjoy.
It is part of our "New Skills" series, which is dedicated to helping you learn and master valuable skills including day trading strategies.

To get the most out of this tool, we encourage you to read our related guide, which includes helpful tips, strategies, 
and examples to support your learning journey at: https://ads4dads.com/new-skills/becoming-a-successful-day-trader-part-4-using-vwap/

Continue your journey on Ads4Dads.com to explore more resources and skill-building tools.
