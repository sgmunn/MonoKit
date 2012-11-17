using System;
using System.Xml.Serialization;

namespace BoneSoft.CSS {
	/// <summary></summary>
	public class SimpleSelector {
		private Combinator? combinator = null;
		private string elementname;
		private string id;
		private string cls;
		private Attribute attribute;
		private string pseudo;
		private Function function;
		private SimpleSelector child;

		/// <summary></summary>
		[XmlIgnore]
		public Combinator? Combinator {
			get { return combinator; }
			set { combinator = value; }
		}
		[XmlAttribute("combinator")]
		public string CombinatorString {
			get { if (this.combinator.HasValue) { return combinator.ToString(); } else { return null; } }
			set { this.combinator = (Combinator)Enum.Parse(typeof(Combinator), value); }
		}

		/// <summary></summary>
		[XmlAttribute("element")]
		public string ElementName {
			get { return elementname; }
			set { elementname = value; }
		}

		/// <summary></summary>
		[XmlAttribute("id")]
		public string ID {
			get { return id; }
			set { id = value; }
		}

		/// <summary></summary>
		[XmlAttribute("class")]
		public string Class {
			get { return cls; }
			set { cls = value; }
		}

		/// <summary></summary>
		[XmlAttribute("pseudo")]
		public string Pseudo {
			get { return pseudo; }
			set { pseudo = value; }
		}

		/// <summary></summary>
		[XmlElement("Attribute")]
		public Attribute Attribute {
			get { return attribute; }
			set { attribute = value; }
		}

		/// <summary></summary>
		[XmlElement("Function")]
		public Function Function {
			get { return function; }
			set { function = value; }
		}

		/// <summary></summary>
		[XmlElement("Child")]
		public SimpleSelector Child {
			get { return child; }
			set { child = value; }
		}

		/// <summary></summary>
		/// <returns></returns>
		public override string ToString() {
			/*
simpleselector<out SimpleSelector ss> =		(. ss = new SimpleSelector(); string psd = null; BoneSoft.CSS.Attribute atb = null; SimpleSelector parent = ss; .)
	(ident						(. ss.ElementName = t.val; .)
	| '*'						(. ss.ElementName = "*"; .)
	| ('#' ident				(. ss.ID = t.val; .)
		| '.' ident				(. ss.Class = t.val; .)
		| attrib<out atb>		(. ss.Attribute = atb; .)
		| pseudo<out psd>		(. ss.Pseudo = psd; .)
		)
	)
	{							(. SimpleSelector child = new SimpleSelector(); .)
		('#' ident				(. child.ID = t.val; .)
		| '.' ident				(. child.Class = t.val; .)
		| attrib<out atb>		(. child.Attribute = atb; .)
		| pseudo<out psd>		(. child.Pseudo = psd; .)
		)						(. parent.Child = child;
									parent = child;
								.)
	}
.
			*/
			System.Text.StringBuilder txt = new System.Text.StringBuilder();
			if (combinator.HasValue) {
				switch (combinator.Value) {
					case BoneSoft.CSS.Combinator.PrecededImmediatelyBy: txt.Append(" + "); break;
					case BoneSoft.CSS.Combinator.ChildOf: txt.Append(" > "); break;
					case BoneSoft.CSS.Combinator.PrecededBy: txt.Append(" ~ "); break;
				}
			}
			if (elementname != null) { txt.Append(elementname); }
			if (id != null) { txt.AppendFormat("#{0}", id); }
			if (cls != null) { txt.AppendFormat(".{0}", cls); }
			if (pseudo != null) { txt.AppendFormat(":{0}", pseudo); }
			if (attribute != null) { txt.Append(attribute.ToString()); }
			if (function != null) { txt.Append(function.ToString()); }
			if (child != null) {
				if (child.ElementName != null) { txt.Append(" "); }
				txt.Append(child.ToString());
			}
			return txt.ToString();
		}
	}
}