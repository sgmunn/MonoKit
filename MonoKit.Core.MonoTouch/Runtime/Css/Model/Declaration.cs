using System;
using System.Xml.Serialization;

namespace BoneSoft.CSS {
	/// <summary>property ( name: stuff; )</summary>
	public class Declaration {
		private string name;
		private Expression expression;
		private bool important;

		/// <summary></summary>
		[XmlAttribute("name")]
		public string Name {
			get { return name; }
			set { name = value; }
		}

		/// <summary></summary>
		[XmlAttribute("important")]
		public bool Important {
			get { return important; }
			set { important = value; }
		}

		/// <summary></summary>
		[XmlElement("Expression")]
		public Expression Expression {
			get { return expression; }
			set { expression = value; }
		}

		/// <summary></summary>
		/// <returns></returns>
		public override string ToString() {
			/*
declaration<out Declaration dec> =
						(. dec = new Declaration();
							Expression exp = null;
						.)
	[ ident				(. dec.Name = t.val; .)
	':' expr<out exp>	(. dec.Expression = exp; .)
		[ "!important"	(. dec.Important = true; .)
	] ]
.
			*/
			System.Text.StringBuilder txt = new System.Text.StringBuilder();
			txt.AppendFormat("{0}: {1}{2}", name, expression.ToString(), important ? " !important" : "");
			return txt.ToString();
		}
	}
}