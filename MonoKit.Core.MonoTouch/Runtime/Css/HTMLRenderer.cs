using System;
using System.Collections.Generic;
using System.Text;

namespace BoneSoft.CSS {
	public static class HTMLRenderer {
		public static string Render(CSSDocument css) {
			StringBuilder txt = new StringBuilder();

			txt.Append("<pre><span class=\"cssDoc\">");

			foreach (Directive dr in css.Directives) {
				txt.AppendFormat("<span class=\"directive\">{0}</span>\r\n", Render(dr));
			}
			if (txt.Length > 0) { txt.Append("\r\n"); }
			foreach (RuleSet rules in css.RuleSets) {
				txt.AppendFormat("<span class=\"ruleset\">{0}</span>\r\n", Render(rules, 0));
			}

			txt.Append("</span></pre>");

			return txt.ToString();
		}

		static string Render(Directive dir) {
			return Render(dir, 0);
		}
		static string Render(Directive dir, int nesting) {
			string start = "";
			for (int i = 0; i < nesting; i++) {
				start += "\t";
			}

			switch (dir.Type) {
				case DirectiveType.Charset: return RenderCharSet(dir, start);
				case DirectiveType.Page: return RenderPage(dir, start);
				case DirectiveType.Media: return RenderMedia(dir, nesting);
				case DirectiveType.Import: return RenderImport(dir);
				case DirectiveType.FontFace: return RenderFontFace(dir, start);
			}

			System.Text.StringBuilder txt = new System.Text.StringBuilder();

			txt.AppendFormat("<span class=\"directive_name\">{0}</span> ", dir.Name);

			if (dir.Expression != null) { txt.AppendFormat("<span class=\"expression\">{0}</span> ", dir.Expression); }

			bool first = true;
			foreach (Medium m in dir.Mediums) {
				if (first) {
					first = false;
				} else {
					txt.Append(", ");
				}
				txt.AppendFormat(" <span class=\"medium\">{0}</span>", m.ToString());
			}

			bool HasBlock = (dir.Declarations.Count > 0 || dir.Directives.Count > 0 || dir.RuleSets.Count > 0);

			if (!HasBlock) {
				txt.Append(";");
				return txt.ToString();
			}

			txt.Append(" {\r\n" + start);

			foreach (Directive dr in dir.Directives) {
				txt.AppendFormat("<span class=\"directive\">{0}</span>\r\n", Render(dr, nesting + 1));
			}

			foreach (RuleSet rules in dir.RuleSets) {
				txt.AppendFormat("<span class=\"ruleset\">{0}</span>\r\n", rules.ToString(nesting + 1));
			}

			first = true;
			foreach (Declaration dec in dir.Declarations) {
				if (first) { first = false; } else { txt.Append(";"); }
				txt.Append("\r\n\t" + start);
				txt.AppendFormat("<span class=\"declaration\">{0}</span>", Render(dec));
			}

			txt.Append("\r\n}");
			return txt.ToString();
		}
		static string RenderFontFace(Directive dir, string start) {
			System.Text.StringBuilder txt = new System.Text.StringBuilder();
			txt.Append("<span class=\"directive_name\">@font-face</span> {");

			bool first = true;
			foreach (Declaration dec in dir.Declarations) {
				if (first) { first = false; } else { txt.Append(";"); }
				txt.Append("\r\n\t" + start);
				txt.AppendFormat("<span class=\"declaration\">{0}</span>", Render(dec));
			}

			txt.Append("</span>\r\n}");
			return txt.ToString();
		}
		static string RenderImport(Directive dir) {
			System.Text.StringBuilder txt = new System.Text.StringBuilder();
			txt.Append("<span class=\"directive_name\">@import</span> ");
			if (dir.Expression != null) { txt.AppendFormat("<span class=\"expression\">{0}</span> ", Render(dir.Expression)); }
			bool first = true;
			foreach (Medium m in dir.Mediums) {
				if (first) {
					first = false;
				} else {
					txt.Append(", ");
				}
				txt.AppendFormat(" <span class=\"medium\">{0}</span>", m.ToString());
			}
			txt.Append(";");
			return txt.ToString();
		}
		static string RenderMedia(Directive dir, int nesting) {
			System.Text.StringBuilder txt = new System.Text.StringBuilder();
			txt.Append("<span class=\"directive_name\">@media</span>");

			bool first = true;
			foreach (Medium m in dir.Mediums) {
				if (first) {
					first = false;
				} else {
					txt.Append(", ");
				}
				txt.AppendFormat(" <span class=\"medium\">{0}</span>", m.ToString());
			}
			txt.Append(" {\r\n");

			foreach (RuleSet rules in dir.RuleSets) {
				txt.AppendFormat("<span class=\"ruleset\">{0}</span>\r\n", Render(rules, nesting + 1));
			}

			txt.Append("}");
			return txt.ToString();
		}
		static string RenderPage(Directive dir, string start) {
			System.Text.StringBuilder txt = new System.Text.StringBuilder();
			txt.Append("<span class=\"directive_name\">@page</span> ");
			if (dir.Expression != null) { txt.AppendFormat("<span class=\"expression\">{0}</span> ", dir.Expression); }
			txt.Append("{\r\n");

			bool first = true;
			foreach (Declaration dec in dir.Declarations) {
				if (first) { first = false; } else { txt.Append(";"); }
				txt.Append("\r\n\t" + start);
				txt.AppendFormat("<span class=\"declaration\">{0}</span>", Render(dec));
			}

			txt.Append("}");
			return txt.ToString();
		}
		static string RenderCharSet(Directive dir, string start) {
			return string.Format("{2}<span class=\"directive_name\">{0}</span> <span class=\"expression\">{1}</span>", dir.Name, Render(dir.Expression), start);
		}

		static string Render(RuleSet rls, int nesting) {
			StringBuilder txt = new StringBuilder();

			string start = "";
			for (int i = 0; i < nesting; i++) {
				start += "\t";
			}

			bool first = true;
			foreach (Selector sel in rls.Selectors) {
				if (first) { first = false; txt.Append(start); } else { txt.Append(", "); }
				txt.AppendFormat("<span class=\"selector\">{0}</span>", Render(sel));
			}
			txt.Append(" {\r\n");
			txt.Append(start);

			foreach (Declaration dec in rls.Declarations) {
				txt.AppendFormat("{1}<span class=\"declaration\">{0}</span>;\r\n", Render(dec), start + "\t");
			}

			txt.Append("}");

			return txt.ToString();
		}

		static string Render(Declaration dec) {
			StringBuilder txt = new StringBuilder();

			txt.AppendFormat("{0}: <span class=\"expression\">{1}</span>{2}", dec.Name, Render(dec.Expression), dec.Important ? " !important" : "");

			return txt.ToString();
		}

		static string Render(Expression exp) {
			StringBuilder txt = new StringBuilder();

			bool first = true;
			foreach (Term t in exp.Terms) {
				if (first) {
					first = false;
				} else {
					txt.AppendFormat("{0} ", t.Seperator.HasValue ? t.Seperator.Value.ToString() : "");
				}
				txt.AppendFormat("<span class=\"term\">{0}</span>", Render(t));
			}

			return txt.ToString();
		}

		static string Render(Selector sel) {
			StringBuilder txt = new StringBuilder();

			bool first = true;
			foreach (SimpleSelector ss in sel.SimpleSelectors) {
				if (first) { first = false; } else { txt.Append(" "); }
				txt.AppendFormat("<span class=\"simpleSelector\">{0}</span>", Render(ss));
			}

			return txt.ToString();
		}

		static string Render(SimpleSelector ss) {
			StringBuilder txt = new StringBuilder();

			if (ss.Combinator.HasValue) {
				switch (ss.Combinator.Value) {
					case BoneSoft.CSS.Combinator.PrecededImmediatelyBy: txt.Append(" <span class=\"combinator\">+</span> "); break;
					case BoneSoft.CSS.Combinator.ChildOf: txt.Append(" <span class=\"combinator\">&gt;</span> "); break;
					case BoneSoft.CSS.Combinator.PrecededBy: txt.Append(" <span class=\"combinator\">~</span> "); break;
				}
			}
			if (ss.ElementName != null) { txt.Append(ss.ElementName); }
			if (ss.ID != null) { txt.AppendFormat("<span class=\"id\">#{0}</span>", ss.ID); }
			if (ss.Class != null) { txt.AppendFormat("<span class=\"class\">.{0}</span>", ss.Class); }
			if (ss.Pseudo != null) { txt.AppendFormat("<span class=\"pseudo\">:{0}</span>", ss.Pseudo); }
			if (ss.Attribute != null) { txt.AppendFormat("<span class=\"attribute\">{0}</span>", Render(ss.Attribute)); }
			if (ss.Function != null) { txt.AppendFormat("<span class=\"function\">{0}</span>", Render(ss.Function)); }
			if (ss.Child != null) {
				txt.AppendFormat("{1}<span class=\"simpleSelector\">{0}</span>", Render(ss.Child), ss.Child.ElementName != null ? " " : "");
			}

			return txt.ToString();
		}

		static string Render(Term t) {
			StringBuilder txt = new StringBuilder();
			// leave off seperator

			if (t.Type == TermType.Function) {
				txt.AppendFormat("<span class=\"value_function function\">{0}</span>", Render(t.Function));
			} else if (t.Type == TermType.Url) {
				txt.AppendFormat("<span class=\"value_url\">url('{0}')</span>", t.Value);
			} else if (t.Type == TermType.Unicode) {
				txt.AppendFormat("<span class=\"value_unicode\">U\\{0}</span>", t.Value.ToUpper());
			} else if (t.Type == TermType.Hex) {
				txt.AppendFormat("<span class=\"value_hex{1}\">{0}</span>", t.Value.ToUpper(), t.IsColor ? " value_color" : "");
			} else {
				if (t.Type == TermType.Number) {
					txt.Append("<span class=\"value_number\">");
				} else {
					txt.AppendFormat("<span class=\"value_string{0}\">", t.IsColor ? " value_color" : "");
				}
				if (t.Sign.HasValue) { txt.AppendFormat("<span class=\"values_sign\">{0}</span>", t.Sign.Value); }
				txt.Append(t.Value);
				if (t.Unit.HasValue) {
					txt.Append("<span class=\"values_unit\">");
					if (t.Unit.Value == BoneSoft.CSS.Unit.Percent) {
						txt.Append("%");
					} else {
						txt.Append(UnitOutput.ToString(t.Unit.Value));
					}
					txt.Append("</span>");
				}
				txt.Append("</span>");
			}

			return txt.ToString();
		}

		static string Render(Attribute atr) {
			StringBuilder txt = new StringBuilder();

			txt.AppendFormat("[<span class=\"attribute_operand\">{0}</span>", atr.Operand);
			if (atr.Operator.HasValue) {
				switch (atr.Operator.Value) {
					case AttributeOperator.Equals: txt.Append("<span class=\"operator\">=</span>"); break;
					case AttributeOperator.InList: txt.Append("<span class=\"operator\">~=</span>"); break;
					case AttributeOperator.Hyphenated: txt.Append("<span class=\"operator\">|=</span>"); break;
					case AttributeOperator.BeginsWith: txt.Append("<span class=\"operator\">$=</span>"); break;
					case AttributeOperator.EndsWith: txt.Append("<span class=\"operator\">^=</span>"); break;
					case AttributeOperator.Contains: txt.Append("<span class=\"operator\">*=</span>"); break;
				}
				txt.AppendFormat("<span class=\"attribute_value\">{0}</span>", atr.Value);
			}
			txt.Append("]");

			return txt.ToString();
		}

		static string Render(Function func) {
			StringBuilder txt = new StringBuilder();

			txt.AppendFormat("<span class=\"function_name\">{0}</span>(", func.Name);
			txt.AppendFormat("<span class=\"expression\">{0}</span>", Render(func.Expression));
			//if (expression != null) {
			//    bool first = true;
			//    foreach (Term t in expression.Terms) {
			//        txt.Append(t.ToString());
			//    }
			//}
			txt.Append(")");

			return txt.ToString();
		}
	}
}

/*
<style>
.cssDoc {
}
.ruleset {
}
.directive {
}
.directive_name {
}
.selector {
}
.simpleselector {
}
.medium {
}
.combinator {
}
.class {
}
.id {
}
.pseudo {
}
.attribute {
}
.attribute_operand {
}
.operator {
}
.attribute_value {
}
.function {
}
.function_name {
}
.declaration {
}
.expression {
}
.term {
}
.value_number {
}
.value_string {
}
.value_hex {
}
.value_unicode {
}
.value_function {
}
.value_url {
}
.value_color {
}
.values_sign {
}
.values_unit {
}
</style>
*/