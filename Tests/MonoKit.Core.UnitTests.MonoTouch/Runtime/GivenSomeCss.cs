
using System;
using System.Linq;
using NUnit.Framework;
using BoneSoft.CSS;

namespace MonoKit.Runtime.UnitTests.Bindings
{
    // http://www.codeproject.com/Articles/20450/Simple-CSS-Parser

    [TestFixture]
    public class aGivenSomeCss
    {
        public string Css { get; private set; }

        [SetUp]
        public void SetUp()
        {
            this.Css = @"
view > button > monkey
{
    viewa: TheView1;
}

apple:test
{
    viewa: fruit;
}

#my-id
{
    viewa: TheView1;
}

.Button
{
    viewa: TheView2;
}
            ";
        }

        [Test]
        public void WhenParsingTheCss_ThenWeGetSomething()
        {
            var parser = new BoneSoft.CSS.CSSParser();
            parser.ParseText(this.Css);


            var css = parser.CSSDocument;

            Assert.AreNotEqual(null, css);

            // select by 
            // type -- should be the type of the object
            // all / universal, probably not supported
            // class, user specified style attribute that we'll inject
            // id, user specified style id ????

            //var ps = css.ElementDeclarations("apple", "test");

            foreach (var rule in css.RuleSets)
            {
                foreach (var selector in rule.Selectors)
                {
                    foreach (var ss in selector.SimpleSelectors)
                    {
                        if (ss.ElementName == "apple" && ss.CombinatorString == "test")
                        {
                            var decl = rule.Declarations;

                        }
                    }
                }

            }
           // css.RuleSets.


            //var ruleSet = css.RuleSets.Where(x => x.Selectors.Where(s => s.SimpleSelectors.Where(ss => ss.ElementName == "apple"))).FirstOrDefault();


            //Assert.AreNotEqual(null, ruleSet);

            //var test = parser.Elements["apple:normal"];
            //var test1 = test["view"];
            //Assert.AreEqual("fruit", test1);

        }
    }
}
