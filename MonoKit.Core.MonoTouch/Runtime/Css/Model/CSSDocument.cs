using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BoneSoft.CSS {
	/// <summary></summary>
	public class CSSDocument : IRuleSetContainer {
		private string charset;
		private List<Directive> dirs = new List<Directive>();
		private List<RuleSet> rulesets = new List<RuleSet>();

		/// <summary></summary>
		[XmlArrayItem("Directive", typeof(Directive))]
		[XmlArray("Directives")]
		public List<Directive> Directives {
			get { return dirs; }
			set { dirs = value; }
		}

		/// <summary></summary>
		[XmlArrayItem("RuleSet", typeof(RuleSet))]
		[XmlArray("RuleSets")]
		public List<RuleSet> RuleSets {
			get { return rulesets; }
			set { rulesets = value; }
		}

		/// <summary></summary>
		/// <returns></returns>
		public override string ToString() {
			System.Text.StringBuilder txt = new System.Text.StringBuilder();
			foreach (Directive dr in dirs) {
				txt.AppendFormat("{0}\r\n", dr.ToString());
			}
			if (txt.Length > 0) { txt.Append("\r\n"); }
			foreach (RuleSet rules in rulesets) {
				txt.AppendFormat("{0}\r\n", rules.ToString());
			}
			return txt.ToString();
		}
	}
}