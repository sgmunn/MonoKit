//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="CssExtensions.cs" company="sgmunn">
//    (c) sgmunn 2012  
//
//    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//    documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//    the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
//    to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
//    The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//    the Software.
//
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//    THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//    CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//    IN THE SOFTWARE.
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------

namespace BoneSoft.CSS
{
    using System;
    using System.Collections.Generic;

    public static class CssExtensions
    {
        public static IList<Declaration> ElementDeclarations(this CSSDocument css, string className, string pseudoClassName)
        {
            foreach (var rule in css.RuleSets)
            {
                foreach (var selector in rule.Selectors)
                {
                    foreach (var ss in selector.SimpleSelectors)
                    {
                        // element name should be the Type of the object
                        // 


                        if (ss.ElementName == "apple" && ss.Child.Pseudo == "test")
                        {
                            return rule.Declarations;
                        }
                    }
                }
                
            }

            return new List<Declaration>();
        }
    }
}

