using System;

namespace BoneSoft.CSS {
	/// <summary></summary>
	public enum Unit {
		/// <summary></summary>
		None,
		/// <summary></summary>
		Percent,
		/// <summary>the font size of the element (or, to the parent element's font size if set on the 'font-size' property)</summary>
		EM,
		/// <summary>the x-height of the element's font</summary>
		EX,
		/// <summary>viewing device</summary>
		PX,
		/// <summary>the grid defined by 'layout-grid' described in the CSS3 Text module [CSS3TEXT]</summary>
		GD,
		/// <summary>the font size of the root element</summary>
		REM,
		/// <summary>the viewport's width</summary>
		VW,
		/// <summary>the viewport's height</summary>
		VH,
		/// <summary>the viewport's height or width, whichever is smaller of the two</summary>
		VM,
		/// <summary>The width of the "0" (ZERO, U+0030) glyph found in the font for the font size used to render. If the "0" glyph is not found in the font, the average character width may be used. How is the "average character width" found?</summary>
		CH,
		/// <summary></summary>
		MM,
		/// <summary></summary>
		CM,
		/// <summary></summary>
		IN,
		/// <summary></summary>
		PT,
		/// <summary></summary>
		PC,
		/// <summary>degrees</summary>
		DEG,
		/// <summary>grads</summary>
		GRAD,
		/// <summary>radians</summary>
		RAD,
		/// <summary>turns</summary>
		TURN,
		/// <summary></summary>
		MS,
		/// <summary></summary>
		S,
		/// <summary></summary>
		Hz,
		/// <summary></summary>
		kHz,
	}

	public static class UnitOutput {
		public static string ToString(Unit u) {
			if (u == Unit.Percent) {
				return "%";
			} else if (u == Unit.Hz || u == Unit.kHz) {
				return u.ToString();
			} else if (u == Unit.None) {
				return "";
			}
			return u.ToString().ToLower();
		}
	}
}