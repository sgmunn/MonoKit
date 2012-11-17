using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BoneSoft.CSS {
	/// <summary></summary>
	public class Selector {
		private List<SimpleSelector> simpleselectors = new List<SimpleSelector>();

		/// <summary></summary>
		[XmlArrayItem("SimpleSelector", typeof(SimpleSelector))]
		[XmlArray("SimpleSelectors")]
		public List<SimpleSelector> SimpleSelectors {
			get { return simpleselectors; }
			set { simpleselectors = value; }
		}

		/// <summary></summary>
		/// <returns></returns>
		public override string ToString() {
			/*
selector<out Selector sel> =	(. sel = new Selector();
									SimpleSelector ss = null;
									Combinator? cb = null;
								.)
	simpleselector<out ss>		(. sel.SimpleSelectors.Add(ss); .)
	{ [ ('+'					(. cb = Combinator.PrecededImmediatelyBy; .)
		| '>'					(. cb = Combinator.ChildOf; .)
		| '~'					(. cb = Combinator.PrecededBy; .)
		) ]
		simpleselector<out ss>	(. if (cb.HasValue) { ss.Combinator = cb.Value; }
									sel.SimpleSelectors.Add(ss);
								.)
								(. cb = null; .)
	}
.
			*/
			System.Text.StringBuilder txt = new System.Text.StringBuilder();
			bool first = true;
			foreach (SimpleSelector ss in simpleselectors) {
				if (first) { first = false; } else { txt.Append(" "); }
				txt.Append(ss.ToString());
			}
			return txt.ToString();
		}
	}
}