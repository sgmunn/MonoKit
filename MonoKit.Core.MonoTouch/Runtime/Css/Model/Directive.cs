using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace BoneSoft.CSS {
	/// <summary></summary>
	public class Directive : IDeclarationContainer, IRuleSetContainer {
		private DirectiveType type;
		private string name;
		private Expression expression;
		private List<Medium> mediums = new List<Medium>();
		private List<Directive> directives = new List<Directive>();
		private List<RuleSet> rulesets = new List<RuleSet>();
		private List<Declaration> declarations = new List<Declaration>();

		/// <summary></summary>
		[XmlAttribute("type")]
		public DirectiveType Type {
			get { return this.type; }
			set { this.type = value; }
		}

		/// <summary></summary>
		[XmlAttribute("name")]
		public string Name {
			get { return this.name; }
			set { this.name = value; }
		}

		/// <summary></summary>
		[XmlElement("Expression")]
		public Expression Expression {
			get { return this.expression; }
			set { this.expression = value; }
		}

		/// <summary></summary>
		[XmlArrayItem("Medium", typeof(Medium))]
		[XmlArray("Mediums")]
		public List<Medium> Mediums {
			get { return this.mediums; }
			set { this.mediums = value; }
		}

		/// <summary></summary>
		[XmlArrayItem("Directive", typeof(Directive))]
		[XmlArray("Directives")]
		public List<Directive> Directives {
			get { return this.directives; }
			set { this.directives = value; }
		}

		/// <summary></summary>
		[XmlArrayItem("RuleSet", typeof(RuleSet))]
		[XmlArray("RuleSets")]
		public List<RuleSet> RuleSets {
			get { return this.rulesets; }
			set { this.rulesets = value; }
		}

		/// <summary></summary>
		[XmlArrayItem("Declaration", typeof(Declaration))]
		[XmlArray("Declarations")]
		public List<Declaration> Declarations {
			get { return this.declarations; }
			set { this.declarations = value; }
		}

		/// <summary></summary>
		/// <returns></returns>
		public override string ToString() {
			return ToString(0);
		}

		/// <summary></summary>
		/// <param name="nesting"></param>
		/// <returns></returns>
		public string ToString(int nesting) {
			string start = "";
			for (int i = 0; i < nesting; i++) {
				start += "\t";
			}

			switch (type) {
				case DirectiveType.Charset: return ToCharSetString(start);
				case DirectiveType.Page: return ToPageString(start);
				case DirectiveType.Media: return ToMediaString(nesting);
				case DirectiveType.Import: return ToImportString();
				case DirectiveType.FontFace: return ToFontFaceString(start);
			}

			System.Text.StringBuilder txt = new System.Text.StringBuilder();

			txt.AppendFormat("{0} ", name);

			if (expression != null) { txt.AppendFormat("{0} ", expression); }

			bool first = true;
			foreach (Medium m in mediums) {
				if (first) {
					first = false;
					txt.Append(" ");
				} else {
					txt.Append(", ");
				}
				txt.Append(m.ToString());
			}

			bool HasBlock = (this.declarations.Count > 0 || this.directives.Count > 0 || this.rulesets.Count > 0);

			if (!HasBlock) {
				txt.Append(";");
				return txt.ToString();
			}

			txt.Append(" {\r\n" + start);

			foreach (Directive dir in directives) {
				txt.AppendFormat("{0}\r\n", dir.ToCharSetString(start + "\t"));
			}

			foreach (RuleSet rules in rulesets) {
				txt.AppendFormat("{0}\r\n", rules.ToString(nesting + 1));
			}

			first = true;
			foreach (Declaration dec in declarations) {
				if (first) { first = false; } else { txt.Append(";"); }
				txt.Append("\r\n\t" + start);
				txt.Append(dec.ToString());
			}

			txt.Append("\r\n}");
			return txt.ToString();
		}

		private string ToFontFaceString(string start) {
			System.Text.StringBuilder txt = new System.Text.StringBuilder();
			txt.Append("@font-face {");

			bool first = true;
			foreach (Declaration dec in declarations) {
				if (first) { first = false; } else { txt.Append(";"); }
				txt.Append("\r\n\t" + start);
				txt.Append(dec.ToString());
			}

			txt.Append("\r\n}");
			return txt.ToString();
		}

		private string ToImportString() {
			System.Text.StringBuilder txt = new System.Text.StringBuilder();
			txt.Append("@import ");
			if (expression != null) { txt.AppendFormat("{0} ", expression); }
			bool first = true;
			foreach (Medium m in mediums) {
				if (first) {
					first = false;
					txt.Append(" ");
				} else {
					txt.Append(", ");
				}
				txt.Append(m.ToString());
			}
			txt.Append(";");
			return txt.ToString();
		}

		private string ToMediaString(int nesting) {
			System.Text.StringBuilder txt = new System.Text.StringBuilder();
			txt.Append("@media");

			bool first = true;
			foreach (Medium m in mediums) {
				if (first) {
					first = false;
					txt.Append(" ");
				} else {
					txt.Append(", ");
				}
				txt.Append(m.ToString());
			}
			txt.Append(" {\r\n");

			foreach (RuleSet rules in rulesets) {
				txt.AppendFormat("{0}\r\n", rules.ToString(nesting + 1));
			}

			txt.Append("}");
			return txt.ToString();
		}

		private string ToPageString(string start) {
			System.Text.StringBuilder txt = new System.Text.StringBuilder();
			txt.Append("@page ");
			if (expression != null) { txt.AppendFormat("{0} ", expression); }
			txt.Append("{\r\n");

			bool first = true;
			foreach (Declaration dec in declarations) {
				//if (first) { first = false; } else { txt.Append("; "); }
				//txt.Append(dec.ToString());
				if (first) { first = false; } else { txt.Append(";"); }
				txt.Append("\r\n\t" + start);
				txt.Append(dec.ToString());
			}

			txt.Append("}");
			return txt.ToString();
		}

		private string ToCharSetString(string start) {
			return string.Format("{2}{0} {1}", name, expression.ToString(), start);
		}
	}
}