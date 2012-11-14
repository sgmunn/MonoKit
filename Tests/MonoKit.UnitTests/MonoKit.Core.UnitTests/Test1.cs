using System;

namespace MonoKit.Core.UnitTests
{
    public class Test1
    {
    }

    // todo: write tests for GetPropertyInfo for nested properties - ReflectionPropertyAccessor
    
    // todo: determine if we want to have to always use a property accessor - I think yes at the moment. (Binding / Expression)

    // todo: add support for binding to attached properties - should be fast compared to SetValue / GetValue thru reflection
    // - we can use binding assistants for this - the property name is the injected property name, and the assistant 
    // uses casts and injected properties to set / get the value
    // we'll need a unit test for that
    //
    // for this to work, we need to have IProprtyAccessor know about the property type of the injected property, so that converters can use it
    // we also need to allow binding to other injected properties

}

