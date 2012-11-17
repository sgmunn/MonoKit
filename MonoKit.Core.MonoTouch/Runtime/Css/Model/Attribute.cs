using System;
using System.Xml.Serialization;

namespace BoneSoft.CSS {
	/// <summary></summary>
	public class Attribute {
		private string operand;
		private AttributeOperator? op = null;
		private string val;

		/// <summary></summary>
		[XmlAttribute("operand")]
		public string Operand {
			get { return operand; }
			set { operand = value; }
		}

		/// <summary></summary>
		[XmlIgnore]
		public AttributeOperator? Operator {
			get { return op; }
			set { op = value; }
		}
		[XmlAttribute("operator")]
		public string OperatorString {
			get { if (this.op.HasValue) { return this.op.Value.ToString(); } else { return null; } }
			set { this.op = (AttributeOperator)Enum.Parse(typeof(AttributeOperator), value); }
		}

		/// <summary></summary>
		[XmlAttribute("value")]
		public string Value {
			get { return val; }
			set { val = value; }
		}

		/// <summary></summary>
		/// <returns></returns>
		public override string ToString() {
			System.Text.StringBuilder txt = new System.Text.StringBuilder();
			txt.AppendFormat("[{0}", operand);
			if (op.HasValue) {
				switch (op.Value) {
					case AttributeOperator.Equals: txt.Append("="); break;
					case AttributeOperator.InList: txt.Append("~="); break;
					case AttributeOperator.Hyphenated: txt.Append("|="); break;
					case AttributeOperator.BeginsWith: txt.Append("$="); break;
					case AttributeOperator.EndsWith: txt.Append("^="); break;
					case AttributeOperator.Contains: txt.Append("*="); break;
				}
				txt.Append(val);
			}
			txt.Append("]");
			return txt.ToString();
		}
	}
}