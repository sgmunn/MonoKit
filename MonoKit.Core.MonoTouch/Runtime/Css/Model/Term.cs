using System;
//using System.Drawing;
using System.Xml.Serialization;

namespace BoneSoft.CSS {
	/// <summary>part of a property's value</summary>
	public class Term {
		private char? seperator;
		private char? sign;
		private TermType type;
		private string val;
		private Unit? unit;
		private Function function;

		/// <summary></summary>
		[XmlIgnore]
		public char? Seperator {
			get { return seperator; }
			set { seperator = value; }
		}
		/// <summary></summary>
		[XmlAttribute("seperator")]
		public string SeperatorChar {
			get { return seperator.HasValue ? this.seperator.Value.ToString() : null; }
			set { seperator = !string.IsNullOrEmpty(value) ? value[0] : '\0'; }
		}

		/// <summary></summary>
		[XmlIgnore]
		public char? Sign {
			get { return sign; }
			set { sign = value; }
		}
		/// <summary></summary>
		[XmlAttribute("sign")]
		public string SignChar {
			get { return this.sign.HasValue ? this.sign.Value.ToString() : null; }
			set { this.sign = !string.IsNullOrEmpty(value) ? value[0] : '\0'; }
		}

		/// <summary></summary>
		[XmlAttribute("type")]
		public TermType Type {
			get { return type; }
			set { type = value; }
		}

		/// <summary></summary>
		[XmlAttribute("value")]
		public string Value {
			get { return val; }
			set { val = value; }
		}

		/// <summary></summary>
		[XmlIgnore]
		public Unit? Unit {
			get { return unit; }
			set { unit = value; }
		}
		/// <summary></summary>
		[XmlAttribute("unit")]
		public string UnitString {
			get { if (this.unit.HasValue) { return this.unit.ToString(); } else { return null; } }
			set { this.unit = (Unit)Enum.Parse(typeof(Unit), value); }
		}

		/// <summary></summary>
		[XmlElement("Function")]
		public Function Function {
			get { return function; }
			set { function = value; }
		}

		/// <summary></summary>
		/// <returns></returns>
		public override string ToString() {
			/*
term<out Term trm> =			(. trm = new Term();
									string val = "";
									Function func = null;
								.)
	[ ('-'						(. trm.Sign = '-'; .)
	| '+'						(. trm.Sign = '+'; .)
	) ]
	(
		{ digit					(. val += t.val; .)
		}
		[ (
			"%"					(. trm.Unit = Unit.Percent; .)
			| "ex"				(. trm.Unit = Unit.EX; .)
			| "em"				(. trm.Unit = Unit.EM; .)
			| "px"				(. trm.Unit = Unit.PX; .)
			| "cm"				(. trm.Unit = Unit.CM; .)
			| "mm"				(. trm.Unit = Unit.MM; .)
			| "pc"				(. trm.Unit = Unit.PC; .)
			| "in"				(. trm.Unit = Unit.IN; .)
			| "pt"				(. trm.Unit = Unit.PT; .)
			| "deg"				(. trm.Unit = Unit.DEG; .)
			| ["g"				(. trm.Unit = Unit.GRAD; .)
			] "rad"				(. if (trm.Unit != Unit.GRAD) { trm.Unit = Unit.RAD; } .)
			| ["m"				(. trm.Unit = Unit.MS; .)
			] "s"				(. if (trm.Unit != Unit.MS) { trm.Unit = Unit.S; } .)
			| ["k"				(. trm.Unit = Unit.KHZ; .)
			] "hz"				(. if (trm.Unit != Unit.KHZ) { trm.Unit = Unit.HZ; } .)
		) ]						(. trm.Value = val; trm.Type = TermType.Number; .)
	|
		function<out func>		(. trm.Function = func; trm.Type = TermType.Function; .)
	|
		QuotedString<out val>	(. trm.Value = val; trm.Type = TermType.String; .)
	|
		ident					(. trm.Value = t.val; trm.Type = TermType.String; .)
	|
		URI<out val>			(. trm.Value = val; trm.Type = TermType.Url; .)
	|
		"U\\"
		{ (digit|'A'|'B'|'C'|'D'|'E'|'F'|'a'|'b'|'c'|'d'|'e'|'f')
								(. val += t.val; .)
		}						(. trm.Value = val; trm.Type = TermType.Unicode; .)
	|
		hexdigit				(. trm.Value = t.val; trm.Type = TermType.Hex; .)
	)
.
			*/
			System.Text.StringBuilder txt = new System.Text.StringBuilder();
			//if (seperator.HasValue) { txt.Append(seperator.Value); txt.Append(" "); }

			if (type == TermType.Function) {
				txt.Append(function.ToString());
			} else if (type == TermType.Url) {
				txt.AppendFormat("url('{0}')", val);
			} else if (type == TermType.Unicode) {
				txt.AppendFormat("U\\{0}", val.ToUpper());
			} else if (type == TermType.Hex) {
				txt.Append(val.ToUpper());
			} else {
				if (sign.HasValue) { txt.Append(sign.Value); }
				txt.Append(val);
				if (unit.HasValue) {
					if (unit.Value == BoneSoft.CSS.Unit.Percent) {
						txt.Append("%");
					} else {
						txt.Append(UnitOutput.ToString(unit.Value));
					}
				}
			}

			return txt.ToString();
		}

//		public bool IsColor {
//			get {
//				if (((type == TermType.Hex) || (type == TermType.String && val.StartsWith("#")))
//					&& (val.Length == 6 || val.Length == 3 || ((val.Length == 7 || val.Length == 4)
//					&& val.StartsWith("#")))) {
//					bool hex = true;
//					foreach (char c in val) {
//						if (!char.IsDigit(c) && c != '#'
//							&& c != 'a' && c != 'A'
//							&& c != 'b' && c != 'B'
//							&& c != 'c' && c != 'C'
//							&& c != 'd' && c != 'D'
//							&& c != 'e' && c != 'E'
//							&& c != 'f' && c != 'F'
//						) {
//							return false;
//						}
//					}
//					return hex;
//				} else if (type == TermType.String) {
//					bool number = true;
//					foreach (char c in val) {
//						if (!char.IsDigit(c)) {
//							number = false;
//							break;
//						}
//					}
//					if (number) { return false; }
//
//					try {
//						KnownColor kc = (KnownColor)Enum.Parse(typeof(KnownColor), val, true);
//						return true;
//					} catch { }
//				} else if (type == TermType.Function) {
//					/*
//					// 0-255, 0-1
//					rgb(255,0,0)
//					rgba(255,0,0,1)
//					// 0-100%, 0-1
//					rgb(100%,0%,0%)
//					rgba(100%,0%,0%,1)
//
//					// 0-360, 0-100%, 0-100%, 0-1
//					hsl(0, 100%, 50%)
//					hsl(120, 75%, 75%)
//					hsla(240, 100%, 50%, 0.5)
//					hsla(30, 100%, 50%, 0.1)
//					*/
//					if ((function.Name.ToLower().Equals("rgb") && function.Expression.Terms.Count == 3)
//						|| (function.Name.ToLower().Equals("rgba") && function.Expression.Terms.Count == 4)
//						) {
//						for (int i = 0; i < function.Expression.Terms.Count; i++) {
//							if (function.Expression.Terms[i].Type != TermType.Number) { return false; }
//						}
//						return true;
//					} else if ((function.Name.ToLower().Equals("hsl") && function.Expression.Terms.Count == 3)
//						|| (function.Name.ToLower().Equals("hsla") && function.Expression.Terms.Count == 4)
//						) {
//						for (int i = 0; i < function.Expression.Terms.Count; i++) {
//							if (function.Expression.Terms[i].Type != TermType.Number) { return false; }
//						}
//						return true;
//					}
//				}
//				return false;
//			}
//		}
		private int GetRGBValue(Term t) {
			try {
				if (t.Unit.HasValue && t.Unit.Value == BoneSoft.CSS.Unit.Percent) {
					return (int)(255f * float.Parse(t.Value) / 100f);
				}
				return int.Parse(t.Value);
			} catch {}
			return 0;
		}
		private int GetHueValue(Term t) {
			// 0 - 360
			try {
				return (int)(float.Parse(t.Value) * 255f / 360f);
			} catch {}
			return 0;
		}

//		public Color ToColor() {
//			string hex = "000000";
//			if (type == TermType.Hex) {
//				if ((val.Length == 7 || val.Length == 4) && val.StartsWith("#")) {
//					hex = val.Substring(1);
//				} else if (val.Length == 6 || val.Length == 3) {
//					hex = val;
//				}
//			} else if (type == TermType.Function) {
//				if ((function.Name.ToLower().Equals("rgb") && function.Expression.Terms.Count == 3)
//					|| (function.Name.ToLower().Equals("rgba") && function.Expression.Terms.Count == 4)
//					) {
//					int fr = 0, fg = 0, fb = 0;
//					for (int i = 0; i < function.Expression.Terms.Count; i++) {
//						if (function.Expression.Terms[i].Type != TermType.Number) { return Color.Black; }
//						switch (i) {
//							case 0: fr = GetRGBValue(function.Expression.Terms[i]); break;
//							case 1: fg = GetRGBValue(function.Expression.Terms[i]); break;
//							case 2: fb = GetRGBValue(function.Expression.Terms[i]); break;
//						}
//					}
//					return Color.FromArgb(fr, fg, fb);
//				} else if ((function.Name.ToLower().Equals("hsl") && function.Expression.Terms.Count == 3)
//					|| (function.Name.Equals("hsla") && function.Expression.Terms.Count == 4)
//					) {
//					int h = 0, s = 0, v = 0;
//					for (int i = 0; i < function.Expression.Terms.Count; i++) {
//						if (function.Expression.Terms[i].Type != TermType.Number) { return Color.Black; }
//						switch (i) {
//							case 0: h = GetHueValue(function.Expression.Terms[i]); break;
//							case 1: s = GetRGBValue(function.Expression.Terms[i]); break;
//							case 2: v = GetRGBValue(function.Expression.Terms[i]); break;
//						}
//					}
//					HSV hsv = new HSV(h, s, v);
//					return hsv.Color;
//				}
//			} else {
//				try {
//					KnownColor kc = (KnownColor)Enum.Parse(typeof(KnownColor), val, true);
//					Color c = Color.FromKnownColor(kc);
//					return c;
//				} catch { }
//			}
//			if (hex.Length == 3) {
//				string temp = "";
//				foreach (char c in hex) {
//					temp += c.ToString() + c.ToString();
//				}
//				hex = temp;
//			}
//			int r = DeHex(hex.Substring(0, 2));
//			int g = DeHex(hex.Substring(2, 2));
//			int b = DeHex(hex.Substring(4));
//			return Color.FromArgb(r, g, b);
//		}
		private int DeHex(string input) {
			int val;
			int result = 0;
			for (int i = 0; i < input.Length; i++) {
				string chunk = input.Substring(i, 1).ToUpper();
				switch (chunk) {
					case "A":
						val = 10; break;
					case "B":
						val = 11; break;
					case "C":
						val = 12; break;
					case "D":
						val = 13; break;
					case "E":
						val = 14; break;
					case "F":
						val = 15; break;
					default:
						val = int.Parse(chunk); break;
				}
				if (i == 0) {
					result += val * 16;
				} else {
					result += val;
				}
			}
			return result;
		}
	}
}