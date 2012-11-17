using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BoneSoft.CSS {
	/// <summary></summary>
	public class RuleSet : IDeclarationContainer {
		private List<Selector> selectors = new List<Selector>();
		private List<Declaration> declarations = new List<Declaration>();

		/// <summary></summary>
		[XmlArrayItem("Selector", typeof(Selector))]
		[XmlArray("Selectors")]
		public List<Selector> Selectors {
			get { return selectors; }
			set { selectors = value; }
		}

		/// <summary></summary>
		[XmlArrayItem("Declaration", typeof(Declaration))]
		[XmlArray("Declarations")]
		public List<Declaration> Declarations {
			get { return declarations; }
			set { declarations = value; }
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
			/*
ruleset<out RuleSet rset> =		(. rset = new RuleSet();
									Selector sel = null;
									Declaration dec = null;
								.)
	selector<out sel>			(. rset.Selectors.Add(sel); .)
	{ ',' selector<out sel>		(. rset.Selectors.Add(sel); .)
	}
    '{' declaration<out dec>	(. rset.Declarations.Add(dec); .)
	{ ';' declaration<out dec>	(. rset.Declarations.Add(dec); .)
	} [ ';' ] '}'
.
			*/
			string start = "";
			for (int i = 0; i < nesting; i++) {
				start += "\t";
			}

			System.Text.StringBuilder txt = new System.Text.StringBuilder();
			bool first = true;
			foreach (Selector sel in selectors) {
				if (first) { first = false; txt.Append(start); } else { txt.Append(", "); }
				txt.Append(sel.ToString());
			}
			txt.Append(" {\r\n");
			txt.Append(start);

			foreach (Declaration dec in declarations) {
				txt.AppendFormat("\t{0};\r\n{1}", dec.ToString(), start);
			}

			txt.Append("}");
			return txt.ToString();
		}
	}
}